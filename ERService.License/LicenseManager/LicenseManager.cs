using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Events;
using ERService.Infrastructure.Interfaces;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ERService.Licensing.Manager
{
    public class LicenseManager : ILicenseManager
    {
        private readonly string licensePath = $"{AppDomain.CurrentDomain.BaseDirectory}license.lic";

        private readonly ILicenseProviderFactory _licenseProviderFactory;
        private readonly IEventAggregator _eventAggregator;

        public LicenseManager(ILicenseProviderFactory licenseProviderFactory, IEventAggregator eventAggregator)
        {
            _licenseProviderFactory = licenseProviderFactory;
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<LicenseValidationRequestEvent>().Subscribe(OnLicenseValidationRequest);

            Errors = new List<IValidationError>();

            Load();
        }

        private void OnLicenseValidationRequest()
        {
            _eventAggregator.GetEvent<AfterLicenseValidationRequestEvent>().Publish(new AfterLicenseValidationRequestEventArgs { IsValid = !LicenseHasErrors } );
        }

        public ILicenseProvider LicenseProvider { get; private set; }

        public IEnumerable<IValidationError> Errors { get; private set; }

        public bool LicenseHasErrors
        {
            get{ return Errors.Any(); }
        }

        public void Load()
        {
            if (!File.Exists(licensePath)) { LoadTrial(); return; }

            using (var stream = new FileStream(licensePath, FileMode.Open))
            {
                LicenseProvider = _licenseProviderFactory.GetLicenseProvider(stream);
                var failures = LicenseProvider.ValidateLicense(PublicKeys.PublicKey);

                ((List<IValidationError>)Errors).Clear();
                foreach (var error in failures)
                {
                    ((List<IValidationError>)Errors).Add(error);
                }
            }

            _eventAggregator.GetEvent<AfterLicenseValidationRequestEvent>().Publish(new AfterLicenseValidationRequestEventArgs { IsValid = !LicenseHasErrors });
        }

        private void LoadTrial()
        {
            //TODO: check timestamp from registry and file, generated durring install
        }
    }
}
