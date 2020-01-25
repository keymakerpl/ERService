using ERService.Business;
using ERService.Infrastructure.Attributes;
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

        [Interpreter(Name = "Nazwa urządzenia", Pattern = "[%h_name%]")]
        public string Name
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _name, value); }
        }

        [Interpreter(Name = "Numer seryjny", Pattern = "[%h_SerialNumber%]")]
        public string SerialNumber
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _name, value); }
        }
    }
}
