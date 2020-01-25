using ERService.Infrastructure.Interfaces;
using Portable.Licensing;
using Portable.Licensing.Validation;
using System;
using System.Collections.Generic;
using System.IO;

namespace ERService.Licensing.Providers
{
    public sealed class PortableLicenseProvider : ILicenseProvider
    {
        private License _license;
        private Stream _stream;

        public PortableLicenseProvider(Stream stream)
        {
            _stream = stream;

            Initialize();
            Owner = GetLicenseOwner();
        }

        private void Initialize()
        {
            _license = License.Load(_stream);
        }

        private IOwnerInfo GetLicenseOwner()
        {
            return new OwnerInfo() 
            { 
                Name = _license.Customer.Name,
                Email = _license.Customer.Email,
                City = AdditionalAttributes["City"],
                Street = AdditionalAttributes["Street"],
                ZIPCode = AdditionalAttributes["ZIPCode"],
                NIP = AdditionalAttributes["NIP"]
            };
        }

        public IDictionary<string, string> AdditionalAttributes { get => _license.AdditionalAttributes.GetAll(); }

        public IDictionary<string, string> ProductFeatures { get => _license.ProductFeatures.GetAll(); }

        public DateTime Expiration { get => DateTime.Parse(_license.AdditionalAttributes.Get("ExpirationDate")); }

        public IOwnerInfo Owner { get; }

        public IEnumerable<IValidationError> ValidateLicense(string publicKey)
        {
            var failures = _license.Validate()
                                   .ExpirationDate()
                                   .And()
                                   .Signature(publicKey)
                                   .AssertValidLicense();

            foreach (var error in failures)
            {
                yield return new ValidationError() { Message = error.Message, Solution = error.HowToResolve };
            }
        }

        private class ValidationError : IValidationError
        {
            public string Message { get; set; }
            public string Solution { get; set; }
        }

        private class OwnerInfo : IOwnerInfo
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string City { get; set; }
            public string Street { get; set; }
            public string ZIPCode { get; set; }
            public string NIP { get; set; }
        }
    }
}