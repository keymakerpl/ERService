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
            _regionManager.RegisterViewWithRegion("ToolBarRegion", typeof(ToolBarView));
            _container.Resolve<ToolBarView>();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}