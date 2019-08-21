﻿using ERService.Business;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERService.RBAC
{
    //TODO: Podzielić interfejs na mniejsze?
    public interface IRBACManager
    {
        void AddAclToRole(AclVerb aclVerb);

        void AddRole(Role role);

        void AddUserToRole(User user, Role role);

        bool Authorize(string login, string password);

        List<Acl> GetAclList();

        Task<IEnumerable<Role>> GetAllRolesAsync();

        Task<IEnumerable<User>> GetAllUsersAsync();

        Task<Role> GetNewRole(string roleName);

        List<Acl> GetRolePermissions(Role role);

        Role GetUserRole(User user);

        void RemoveRole(Role role);

        void RemoveUser(User user);

        bool RoleExists(string roleName);

        Task<bool> RoleExistsAsync(string roleName);

        bool LoggedUserHasAccess(string verbName);

        Task SaveAsync();

        bool UserExists(string login);

        bool UserIsInRole(string login, Role role);

        User LoggedUser { get; set; }
    }
}