using ERService.Business;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERService.RBAC
{
    public interface IRBACManager
    {
        Task LoadAsync();

        Task Refresh();

        User LoggedUser { get; set; }

        bool Login(string login, string password);

        void Logout();

        List<User> Users { get; set; }

        void AddUser(User user);

        void RemoveUser(User user);

        List<Role> Roles { get; set; }

        void AddRole(Role role);

        void RemoveRole(Role role);

        Task<Role> GetNewRole(string roleName);

        List<Acl> GetRolePermissions(Role role);

        Task<bool> RoleExistsAsync(string roleName);

        bool LoggedUserHasPermission(string verbName);

        List<Acl> GetAclList();

        Task SaveAsync();

        void RollBackChanges();        

        User this[string login] { get; }        

        User this[Guid id] { get; }

        bool HasChanges();
    }
}