using ERService.Business;
using ERService.Infrastructure.Wrapper;

namespace ERService.Settings.Wrapper
{
    public class AclWrapper : ModelWrapper<Acl>
    {
        public AclWrapper(Acl model) : base(model)
        {
        }

        private AclVerb _aclVerb;

        public AclVerb AclVerb
        {
            get { return GetValue<AclVerb>(); }
            set { SetProperty(ref _aclVerb, value); }
        }

        private int _value;

        public int Value
        {
            get { return GetValue<int>(); }
            set { SetProperty(ref _value, value); }
        }

    }
}
