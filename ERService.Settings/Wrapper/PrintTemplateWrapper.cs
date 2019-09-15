using ERService.Business;
using ERService.Infrastructure.Wrapper;

namespace ERService.Settings.Wrapper
{
    public class PrintTemplateWrapper : ModelWrapper<PrintTemplate>
    {
        public PrintTemplateWrapper(PrintTemplate model) : base(model)
        {            
        }

        private string _name;

        public string Name
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _name, value); }
        }

        private string _template;

        public string Template
        {
            get { return GetValue<string>() ?? ""; }
            set { SetProperty(ref _template, value); }
        }
    }
}
