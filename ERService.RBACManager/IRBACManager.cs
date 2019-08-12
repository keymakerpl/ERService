using System.Collections.Generic;
using System.Threading.Tasks;
using ERService.Business;

namespace ERService.RBAC
{
    //TODO: Podzielić interfejs na mniejsze?
    public interface IRBACManager
    {
        void AddAclToRole(AclVerb aclVerb);
        void AddUserToRole(User user, Role role);
        void AddRole(Role role);
        Task<Role> GetNewRole(string roleName);
        void RemoveRole(Role role);
        Task<IEnumerable<Role>> GetAllRolesAsync();
        bool Authorize(string login, string password);
        List<Acl> GetAclList();
        List<Acl> GetRolePermissions(Role role);
        Role GetUserRole(User user);
        Task<bool> RoleExistsAsync(string roleName);
        bool RoleExists(string roleName);
        bool UserExists(string login);
        bool UserIsInRole(string login, Role role);
        Task SaveAsync();
    }
}