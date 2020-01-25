using System;
using System.Collections.Generic;

namespace ERService.Infrastructure.Interfaces
{
    public interface IValidationError
    {
        string Message { get; set; }
        string Solution { get; set; }
    }

    public interface IOwnerInfo
    {
        string Name { get; set; }
        string Email { get; set; }
        string City { get; set; }
        string Street { get; set; }
        string ZIPCode { get; set; }
        string NIP { get; set; }
    }

    public interface ILicenseProvider
    {
        IOwnerInfo Owner { get; }
        IDictionary<string, string> AdditionalAttributes { get; }
        IDictionary<string, string> ProductFeatures { get; }
        DateTime Expiration { get; }
        IEnumerable<IValidationError> ValidateLicense(string publicKey);  
    }
}
