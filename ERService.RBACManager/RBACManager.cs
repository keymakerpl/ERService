using ERService.Business;
using ERService.Infrastructure.Events;
using ERService.Infrastructure.Helpers;
using ERService.RBAC.Data.Repository;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERService.RBAC
{
    public class RBACManager : IRBACManager
    {
        private IUserRepository _userRepository;
        private IRoleRepository _roleRepository;
        private IAclVerbRepository _aclVerbRepository;
        private IPasswordHasher _passwordHasher;
        private IEventAggregator _eventAggregator;
        private IEnumerable<User> _users;
        private IEnumerable<Role> _roles;

        public RBACManager(IUserRepository userRepository, IRoleRepository roleRepository, 
            IPasswordHasher passwordHasher, IEventAggregator eventAggregator, IAclVerbRepository aclVerbRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _aclVerbRepository = aclVerbRepository;
            _passwordHasher = passwordHasher;
            _eventAggregator = eventAggregator;

            LoadUsers();
            LoadRoles();
        }

        private async void LoadRoles()
        {
            _roles = await _roleRepository.GetAllAsync();
        }

        private async void LoadUsers()
        {
            _users = await _userRepository.GetAllAsync();
        }

        public bool Authorize(string login, string password)
        {
            //TODO: Null Guard
            var user = _users.SingleOrDefault(u => u.Login == login);
            if (user == null) return false;
          
            if (_passwordHasher.VerifyPassword(password, user.PasswordHash, user.Salt))
            {
                _eventAggregator.GetEvent<AfterAuthorisedEvent>()
                    .Publish(new AfterAuthorisedEventArgs
                                { UserID = user.Id, UserLogin = user.Login, UserName = user.FirstName, UserLastName = user.LastName });

                return true;
            }

            return false;
        }

        public void AddUserToRole(User user, Role role)
        {

        }

        /// <summary>
        /// Add role to repository
        /// </summary>
        /// <param name="roleName">Role</param>
        public void AddRole(Role role)
        {
            _roleRepository.Add(role);
        }

        public async Task<Role> GetNewRole(string roleName)
        {
            var aclVerbs = await GetAclVerbsAsync();
            var acls = new List<Acl>();
            foreach (var verb in aclVerbs)
            {
                acls.Add(new Acl { AclVerbId = verb.Id, Value = 0 });
            }

            var role = new Role { Name = roleName, IsSystem = false, ACLs = acls };            

            return role;
        }

        public void RemoveRole(Role role)
        {
            _roleRepository.Remove(role);
        }

        public void RemoveUser(User user)
        {
            _userRepository.Remove(user);
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _roleRepository.GetAllAsync();
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public void AddAclToRole(AclVerb aclVerb)
        {

        }

        public bool UserExists(string login)
        {
            return false;
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            var role = await _roleRepository.FindByAsync(r => r.Name == roleName);

            return role.Any();
        }

        public bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        public bool UserIsInRole(string login, Role role)
        {
            return false;
        }

        public async Task<List<AclVerb>> GetAclVerbsAsync()
        {
            var aclVerbs = await _aclVerbRepository.GetAllAsync();

            return aclVerbs.ToList();
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

        public async Task SaveAsync()
        {
            await _roleRepository.SaveAsync();
            await _userRepository.SaveAsync();
        }
    }
}
