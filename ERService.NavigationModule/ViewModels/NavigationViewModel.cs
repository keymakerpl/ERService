using ERService.Infrastructure.Constants;
using Prism.Commands;
using Prism.Modularity;
using Prism.Regions;

namespace ERService.Navigation.ViewModels
{
    public class NavigationViewModel
    {
        public DelegateCommand<object> OpenDetailViewCommand { get; }

        public IRegionManager _regionManager { get; }
        public IModuleManager _moduleManager { get; }

        public NavigationViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            OpenDetailViewCommand = new DelegateCommand<object>(OnOpenDetailViewExecute);
        }

        private void OnOpenDetailViewExecute(object viewType)
        {
            _regionManager.Regions[RegionNames.ContentRegion].RemoveAll(); //Na ten moment nawigujemy tylko po widokach w wybranym module
            _regionManager.RequestNavigate(RegionNames.ContentRegion, viewType.ToString());            
        }
    }
}
