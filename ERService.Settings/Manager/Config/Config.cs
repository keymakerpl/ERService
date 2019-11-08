using System.Collections.Generic;
using System.Dynamic;

namespace ERService.Settings.Manager
{

    public abstract class Config
    {
        private readonly dynamic _config;

        public Config(ExpandoObject config)
        {
            _config = config;
        }

        protected object this[string key]
        {
            get { return ((IDictionary<string, object>)_config)[key]; }
        }
    }
}
