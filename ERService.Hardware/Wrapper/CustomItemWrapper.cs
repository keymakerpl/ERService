using ERService.Business;
using ERService.Infrastructure.Wrapper;

namespace ERService.HardwareModule.Wrapper
{
    public class CustomItemWrapper : ModelWrapper<CustomItem>
    {
        public CustomItemWrapper(CustomItem model) : base(model)
        {

        }

        private string _key;
        public string Key { get { return GetValue<string>(); } set { SetProperty(ref _key, value); } }

        private HardwareType _hardwareType;
        public HardwareType HardwareType { get { return GetValue<HardwareType>(); } set { SetProperty(ref _hardwareType, value); } }
    }
}
