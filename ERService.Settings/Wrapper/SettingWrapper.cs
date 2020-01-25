using ERService.Business;
using ERService.Infrastructure.Wrapper;

namespace ERService.Settings.Wrapper
{
    public class SettingWrapper : ModelWrapper<Setting>
    {
        public SettingWrapper(Setting model) : base(model)
        {
        }

        private string _key;
        public string Key { get { return GetValue<string>(); } set { SetProperty(ref _key, value); } }

        private string _value;
        public string Value { get { return GetValue<string>(); } set { SetProperty(ref _value, value); } }

        private string _category;
        public string Category { get { return GetValue<string>(); } set { SetProperty(ref _category, value); } }

        private string _description;
        public string Description { get { return GetValue<string>(); } set { SetProperty(ref _description, value); } }
    }
}
