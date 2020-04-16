using ERService.Business;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Base.Common;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Dialogs;
using ERService.Infrastructure.Helpers;
using ERService.Infrastructure.Interfaces;
using ERService.Settings.Manager;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ERService.Settings.ViewModels
{
    public class CompanySettingsViewModel : DetailViewModelBase
    {
        private readonly IRegionManager _regionManager;
        private readonly ILicenseManager _licenseManager;
        private readonly ISettingsManager _settingsManager;
        private readonly IImagesCollection _imagesCollection;

        public DelegateCommand LoadLogoCommand { get; }
        public DelegateCommand CopyFromLicenseCommand { get; }

        private dynamic _companySetting;

        public CompanySettingsViewModel(
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IRegionManager regionManager,
            ILicenseManager licenseManager,
            ISettingsManager settingsManager,
            IImagesCollection imagesCollection) : base(eventAggregator, messageDialogService)
        {
            Title = "Dane firmy";

            _regionManager = regionManager;
            _licenseManager = licenseManager;
            _settingsManager = settingsManager;
            _imagesCollection = imagesCollection;

            LoadLogoCommand = new DelegateCommand(OnLoadLogoExecute);
            CopyFromLicenseCommand = new DelegateCommand(OnCopyFromLicenseExecute);
        }

        private void OnCopyFromLicenseExecute()
        {
            var owner = _licenseManager.LicenseProvider.Owner;

            CompanyConfig.CompanyName = owner.Name;
            CompanyConfig.CompanyStreet = owner.Street;
            CompanyConfig.CompanyCity = owner.City;
            CompanyConfig.CompanyPostCode = owner.ZIPCode;
            CompanyConfig.CompanyNIP = owner.NIP;
        }

        private string _selectedImageFile;
        public string SelectedImageFile
        {
            get { return _selectedImageFile; }
            set { SetProperty(ref _selectedImageFile, value); UpdateImage(); }
        }

        public ERimage LogoImage { get; private set; }

        private ImageSource _selectedImage;
        public ImageSource SelectedImageSource
        {
            get { return _selectedImage; }
            set { SetProperty(ref _selectedImage, value); }
        }

        private async void UpdateImage()
        {
            var file = SelectedImageFile;
            SelectedImageSource = await GenerateBitmap(file, 320);
        }

        //TODO: refactor, move to infrastructure
        private Task<BitmapImage> GenerateBitmap(string file, int scale)
        {
            return Task.Run(() =>
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(file);
                image.DecodePixelWidth = scale;
                image.EndInit();
                image.Freeze();

                return image;
            });
        }

        private Task<BitmapImage> GenerateBitmap(Stream stream, int scale)
        {
            return Task.Run(() =>
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.DecodePixelWidth = scale;
                image.EndInit();
                image.Freeze();

                return image;
            });
        }

        private async void OnLoadLogoExecute()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures);
            openFileDialog.Filter = "Obrazy (*.jpg)|*.jpg";
            var result = openFileDialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                var fileBinarry = FileUtils.GetFileBinary(openFileDialog.FileName);
                using (var stream = new MemoryStream(fileBinarry))
                {
                    SelectedImageSource = await GenerateBitmap(stream, 320);
                }

                LogoImage = new ERimage();
                LogoImage.Tag = "logo";
                LogoImage.Description = "Logo firmy/serwisu";
                LogoImage.FileName = openFileDialog.SafeFileName;
                LogoImage.Checksum = Cryptography.CalculateMD5(openFileDialog.FileName);

                
                LogoImage.ImageData = fileBinarry;
                LogoImage.Size = fileBinarry.Length;
            }            
        }

        public override bool KeepAlive => true;

        public ICompanyInfoConfig CompanyConfig
        {
            get { return _companySetting; }
            private set { SetProperty(ref _companySetting, value); }
        }

        protected override void OnSaveExecute()
        {
            _settingsManager.SaveAsync();

            _imagesCollection["logo"] = LogoImage;
            _imagesCollection.Save();

            _regionManager.Regions[RegionNames.ContentRegion].RemoveAll();
            _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.SettingsView);
        }

        protected override bool OnSaveCanExecute()
        {
            return true;
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            await LoadAsync();
        }

        public override async Task LoadAsync()
        {
            CompanyConfig = await _settingsManager.GetConfigAsync(ConfigNames.CompanyInfoConfig) as ICompanyInfoConfig;
            LoadLogo();
        }

        private async void LoadLogo()
        {
            var image = _imagesCollection["logo"];
            if (image != null)
            {
                using (var stream = new MemoryStream(image.ImageData))
                {
                    SelectedImageSource = await GenerateBitmap(stream, 320);
                }
            }
        }
    }
}
