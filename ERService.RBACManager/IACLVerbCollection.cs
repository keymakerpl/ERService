using ERService.Business;

namespace ERService.RBAC
{
    public interface IACLVerbCollection
    {
        AclVerb this[string verbName] { get; }
    }
}