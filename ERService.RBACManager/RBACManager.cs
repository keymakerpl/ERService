using ERService.Business;
using ERService.RBAC.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERService.RBAC
{
    public class RBACManager : IRBACManager
    {
        private IUserRepository _userRepository;
        private IEnumerable<User> _users;

        public RBACManager(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            LoadUsers();
        }

        private async void LoadUsers()
        {
            _users = await _userRepository.GetAllAsync();
        }

        public bool Authorize(string login, string password)
        {
            //TODO: Null Guard 
            //TODO: Password Hashing
            var user = _users.SingleOrDefault(u => u.Login == login && u.Password == password);

            return user != null;
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
