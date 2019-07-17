using ERService.Business;
using ERService.Infrastructure.Wrapper;

namespace ERService.Settings.ViewModels
{
    public class NumerationWrapper : ModelWrapper<Numeration>
    {
        public NumerationWrapper(Numeration model) : base(model)
        {
        }

        private string _name;

        public string Name
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _name, value); }
        }

        private string _pattern;

        public string Pattern
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _pattern, value); }
        }


    }
}