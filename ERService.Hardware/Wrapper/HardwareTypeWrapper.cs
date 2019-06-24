using ERService.Business;
using ERService.Infrastructure.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERService.HardwareModule.Wrapper
{
    public class HardwareTypeWrapper : ModelWrapper<HardwareType>
    {
        public HardwareTypeWrapper(HardwareType model) : base(model)
        {
        }

        public Guid Id
        {
            get { return Model.Id; }
        }

        private string _name;
        public string Name
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _name, value); }
        }
    }
}
