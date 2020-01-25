using System;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;

namespace ERService.Navigation.ViewModels
{
    public class NavigationViewModel : BindableBase
    {
        public DelegateCommand<object> OpenDetailViewCommand { get; }

        public IRegionManager _regionManager { get; }

        private bool _isEnabled;
        private readonly IEventAggregator _eventAggregator;

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetProperty(ref _isEnabled, value); }
        }

        private string _currentContentName;

        public string CurrentContentName
        {
            get { return _currentContentName; }
            set { SetProperty(ref _currentContentName, value); }
        }

        public NavigationViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            IsEnabled = false;

            _eventAggregator.GetEvent<AfterUserLoggedinEvent>().Subscribe((o) => { IsEnabled = true; }, true);
            _eventAggregator.GetEvent<AfterUserLoggedoutEvent>().Subscribe((o) => { IsEnabled = false; }, true);
            _eventAggregator.GetEvent<AfterDetailOpenedEvent>().Subscribe(OnContentChanged, true);

            OpenDetailViewCommand = new DelegateCommand<object>(OnOpenDetailViewExecute);
        }

        private void OnContentChanged(AfterDetailOpenedEventArgs args)
        {
            CurrentContentName = args.DisplayableName;
        }

        private void OnOpenDetailViewExecute(object viewName)
        {
            _regionManager.Regions[RegionNames.ContentRegion].RemoveAll(); //Na ten moment nawigujemy tylko po widokach w wybranym module
            _regionManager.RequestNavigate(RegionNames.ContentRegion, viewName.ToString());            
        }
    }
}
