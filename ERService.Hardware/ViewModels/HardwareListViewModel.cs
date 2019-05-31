using ERService.Business;
using ERService.Infrastructure.Base;
using ERService.MSSQLDataAccess;
using Prism.Regions;
using System;

namespace ERService.HardwareModule.ViewModels
{
    public class HardwareListViewModel : ListModelBase<Hardware, ERServiceDbContext>
    {
        public HardwareListViewModel(ERServiceDbContext context, IRegionManager regionManager) : base(context, regionManager)
        {
        }

        public override void OnMouseDoubleClickExecute()
        {
            throw new NotImplementedException();
        }
    }
}
