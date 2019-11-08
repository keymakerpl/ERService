using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Dialogs;
using ERService.Infrastructure.Interfaces;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ERService.Settings.ViewModels
{
    public class CompanySettingsViewModel : DetailViewModelBase
    {
        private readonly IRegionManager _regionManager;
        private readonly ISettingsManager _settingsManager;

        public DelegateCommand LoadLogoCommand { get; }

        private object _companySetting;

        public CompanySettingsViewModel(
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IRegionManager regionManager,
            ISettingsManager settingsManager) : base(eventAggregator, messageDialogService)
        {
            Title = "Dane firmy";

            _regionManager = regionManager;
            _settingsManager = settingsManager;

            LoadLogoCommand = new DelegateCommand(OnLoadLogoExecute);
        }

        private string _selectedImageFile;
        public string SelectedImageFile
        {
            get { return _selectedImageFile; }
            set { SetProperty(ref _selectedImageFile, value); UpdateImage(); }
        }

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

        private void OnLoadLogoExecute()
        {
            var chooseFileDialog = new OpenFileDialog();
            chooseFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures);
            chooseFileDialog.Filter = "Obrazy (*.jpg)|*.jpg";
            var result = chooseFileDialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                SelectedImageFile = chooseFileDialog.FileName;
            }
            
        }

        public override bool KeepAlive => true;

        public object CompanyConfig
        {
            get { return _companySetting; }
            private set { SetProperty(ref _companySetting, value); }
        }

        protected override void OnSaveExecute()
        {
            _settingsManager.SaveAsync();
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
            CompanyConfig = await _settingsManager.GetConfigAsync(ConfigNames.CompanyInfoConfig);
        }
    }
}
