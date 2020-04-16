using ERService.Infrastructure.Constants;
using System;
using System.Dynamic;

namespace ERService.Settings.Manager
{
    public class ConfigFactory
    {
        public Config GetConfig(string configCategory, ExpandoObject config)
        {
            switch (configCategory)
            {
                case ConfigNames.CompanyInfoConfig:
                    return new CompanyInfoConfig(config);
                
                default:
                    throw new ArgumentException("Cant create config");
            }
        }
    }
}
