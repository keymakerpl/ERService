using ERService.Business;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERService.RBAC
{
    public interface IRBACManager
    {
        Task LoadAsync();

        void AddAclToRole(AclVerb aclVerb);

        void AddRole(Role role);

        void AddUserToRole(User user, Role role);

        void RemoveRole(Role role);

        void RemoveUser(User user);

        User LoggedUser { get; set; }

        bool Login(string login, string password);

        void Logout();        

        List<Acl> GetAclList();

        Task<IEnumerable<Role>> GetAllRolesAsync();

        Task<IEnumerable<User>> GetAllUsersAsync();

        Task<Role> GetNewRole(string roleName);

        List<Acl> GetRolePermissions(Role role);

        Role GetUserRole(User user);     

        Task<bool> RoleExistsAsync(string roleName);

        bool LoggedUserHasPermission(string verbName);        

        bool UserExists(string login);

        bool UserIsInRole(string login, Role role);

        Task SaveAsync();

        void RollBackChanges();        
    }
}