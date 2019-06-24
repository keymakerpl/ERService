using ERService.Business;
using ERService.Infrastructure.Wrapper;
using System;

namespace ERService.HardwareModule
{
    public class HardwareWrapper : ModelWrapper<Hardware>
    {
        public HardwareWrapper(Hardware model) : base(model)
        {
        }

        public Guid Id { get { return Model.Id; } }

        private string _name;
        public string Name
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _name, value); }
        }
    }
}
