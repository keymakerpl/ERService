using System.Dynamic;
using ERService.Infrastructure.Attributes;
using ERService.Settings.Wrapper;

namespace ERService.Settings.Manager
{
    public class CompanyInfoConfig : Config, ICompanyInfoConfig
    {
        private string _companyNip;
        private string _companyPostCode;
        private string _companyCity;
        private string _companyHouseNumber;
        private string _companyStreet;
        private string _companyName;

        public CompanyInfoConfig(ExpandoObject config) : base(config)
        {
        }

        [Interpreter(Name = "Nazwa serwisu/firmy", Pattern = "[%s_CompanyName%]")]
        public string CompanyName 
        { 
            get { return ((SettingWrapper)this["CompanyName"]).Value; }
            set { ((SettingWrapper)this["CompanyName"]).Value = value; SetProperty(ref _companyName, value); }
        }

        [Interpreter(Name = "Ulica serwisu/firmy", Pattern = "[%s_CompanyStreet%]")]
        public string CompanyStreet 
        { 
            get { return ((SettingWrapper)this["CompanyStreet"]).Value; }
            set { ((SettingWrapper)this["CompanyStreet"]).Value = value; SetProperty(ref _companyStreet, value); }
        }

        [Interpreter(Name = "Numer serwisu/firmy", Pattern = "[%s_CompanyNumber%]")]
        public string CompanyHouseNumber 
        { 
            get { return ((SettingWrapper)this["CompanyNumber"]).Value; }
            set { ((SettingWrapper)this["CompanyNumber"]).Value = value; SetProperty(ref _companyHouseNumber, value); }
        }

        [Interpreter(Name = "Miasto serwisu/firmy", Pattern = "[%s_CompanyCity%]")]
        public string CompanyCity 
        { 
            get { return ((SettingWrapper)this["CompanyCity"]).Value; }
            set { ((SettingWrapper)this["CompanyCity"]).Value = value; SetProperty(ref _companyCity, value); }
        }

        [Interpreter(Name = "Kod pocztowy serwisu/firmy", Pattern = "[%s_CompanyPostCode%]")]
        public string CompanyPostCode 
        { 
            get { return ((SettingWrapper)this["CompanyPostCode"]).Value; }
            set { ((SettingWrapper)this["CompanyPostCode"]).Value = value; SetProperty(ref _companyPostCode, value); }
        }

        [Interpreter(Name = "NIP serwisu/firmy", Pattern = "[%s_CompanyNIP%]")]
        public string CompanyNIP 
        { 
            get { return ((SettingWrapper)this["CompanyNIP"]).Value; }
            set { ((SettingWrapper)this["CompanyNIP"]).Value = value;  SetProperty(ref _companyNip, value); }
        }
    }
}
