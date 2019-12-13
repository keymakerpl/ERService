using ERService.Business;
using ERService.Infrastructure.Base;
using ERService.MSSQLDataAccess;
using Prism.Events;
using Prism.Regions;

namespace ERService.HardwareModule.ViewModels
{
    public class HardwareListViewModel : ListModelBase<Hardware, ERServiceDbContext>
    {
        public HardwareListViewModel(ERServiceDbContext context, IRegionManager regionManager, IEventAggregator eventaggregator) 
            : base(context, regionManager, eventaggregator)
        {
        }

        public override void OnAddExecute()
        {
            
        }

        public override void OnMouseDoubleClickExecute()
        {
            
        }
    }
}
