using ERService.Business;
using ERService.RBAC.Data.Repository;
using System.Collections.Generic;

namespace ERService.RBAC
{
    public class RBACManager
    {
        private IUserRepository _userRepository;

        public RBACManager(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public bool Authorize()
        {
            return false;
        }

        public void AddUserToRole(User user, Role role)
        {

        }

        public void AddAclToRole(AclVerb aclVerb)
        {

        }

        public bool UserExists(string login)
        {
            return false;
        }

        public bool RoleExists(string roleName)
        {
            return false;
        }

        public bool UserIsInRole(string login, Role role)
        {
            return false;
        }

        public List<Acl> GetRolePermissions(Role role)
        {
            return new List<Acl>();
        }

        public List<Acl> GetAclList()
        {
            return new List<Acl>();
        }

        public Role GetUserRole(User user)
        {
            return new Role();
        }
    }
}
