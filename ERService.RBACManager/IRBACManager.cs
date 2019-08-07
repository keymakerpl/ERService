﻿using System.Collections.Generic;
using ERService.Business;

namespace ERService.RBAC
{
    public interface IRBACManager
    {
        void AddAclToRole(AclVerb aclVerb);
        void AddUserToRole(User user, Role role);
        bool Authorize(string login, string password);
        List<Acl> GetAclList();
        List<Acl> GetRolePermissions(Role role);
        Role GetUserRole(User user);
        bool RoleExists(string roleName);
        bool UserExists(string login);
        bool UserIsInRole(string login, Role role);
    }
}