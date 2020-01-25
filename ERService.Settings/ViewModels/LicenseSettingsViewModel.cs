using System;
using System.IO;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Dialogs;
using ERService.Infrastructure.Interfaces;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using System.Linq;
using Prism.Regions;

namespace ERService.Settings.ViewModels
{
    public class LicenseSettingsViewModel : DetailViewModelBase
    {
        private readonly string licensePath = $"{AppDomain.CurrentDomain.BaseDirectory}license.lic";
        private readonly ILicenseManager _licenseManager;
        private IOwnerInfo _owner;
        private DateTime _expiration;

        public LicenseSettingsViewModel(ILicenseManager licenseManager, 
                                        IEventAggregator eventAggregator,
                                        IMessageDialogService messageDialogService) 
            : base(eventAggregator, messageDialogService)
        {
            Title = "Licencja";

            _licenseManager = licenseManager;

            LoadLicenseCommand = new DelegateCommand(OnLoadLicenseExecute);
        }

        private void OnLoadLicenseExecute()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            openFileDialog.Filter = "License file (*.lic)|*.lic";

            var dialogResult = openFileDialog.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                if (licensePath != openFileDialog.FileName)
                {
                    File.Delete(licensePath);
                    File.Copy(openFileDialog.FileName, licensePath, true);
                }

                _licenseManager.Load();
                InitializeLicense();
            }
        }

        private void InitializeLicense()
        {
            if (_licenseManager.Errors.Any())
            {
                _messageDialogService.ShowInformationMessageAsync(this, "Licencja...", "Licencja nieważna. Funkcjonalność programu będzie ograniczona.");
            }

            Owner = _licenseManager?.LicenseProvider?.Owner;
            
            if(_licenseManager.LicenseProvider?.Expiration != null)
                Expiration = _licenseManager.LicenseProvider.Expiration;
        }

        public override bool KeepAlive => true;

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            InitializeLicense();
        }

        public DelegateCommand LoadLicenseCommand { get; }

        public IOwnerInfo Owner
        {
            get { return _owner; }
            private set { SetProperty(ref _owner, value); }
        }

        public DateTime Expiration
        {
            get { return _expiration; }
            private set { SetProperty(ref _expiration, value); }
        }
    }
}
