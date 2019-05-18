using ERService.Infrastructure.Constants;
using ERService.Toolbar.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ERService.Toolbar
{
    public class ToolbarModule : IModule
    {
        private IContainerProvider _container;
        private IRegionManager _regionManager;

        public ToolbarModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;            
            
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _container = containerProvider;
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}