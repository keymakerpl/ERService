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
        private IAclRepository _aclRepository;
        private IEnumerable<Guid> _aclsIDsToDelete;
        private IAclVerbRepository _aclVerbRepository;
        private IEventAggregator _eventAggregator;
        private IPasswordHasher _passwordHasher;
        private IRoleRepository _roleRepository;
        private List<Role> _roles;
        private List<User> _users;
        private IUserRepository _userRepository;
        private User _loggedUser;

        public RBACManager(IUserRepository userRepository, IRoleRepository roleRepository, IAclRepository aclRepository,
            IPasswordHasher passwordHasher, IEventAggregator eventAggregator, IAclVerbRepository aclVerbRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _aclVerbRepository = aclVerbRepository;
            _aclRepository = aclRepository;
            _passwordHasher = passwordHasher;
            _eventAggregator = eventAggregator;

            _aclsIDsToDelete = new List<Guid>();

            _users = new List<User>();
            _roles = new List<Role>();
        }

        public void Load()
        {
            LoadUsers();
            LoadRoles();
        }

        public void AddAclToRole(AclVerb aclVerb)
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

        public void AddUserToRole(User user, Role role)
        {
        }

        public bool Login(string login, string password)
        {
            //TODO: Null Guard
            var user = _users.SingleOrDefault(u => u.Login == login);
            if (user == null) return false;

            if (_passwordHasher.VerifyPassword(password, user.PasswordHash, user.Salt))
            {
                _eventAggregator.GetEvent<AfterUserLoggedinEvent>()
                    .Publish(new UserAuthorizationEventArgs
                    { UserID = user.Id, UserLogin = user.Login, UserName = user.FirstName, UserLastName = user.LastName });

                LoggedUser = user;
                return true;
            }

            return false;
        }

        public void Logout()
        {
            if (LoggedUser == null) return;

            _eventAggregator.GetEvent<AfterUserLoggedoutEvent>()
                .Publish(new UserAuthorizationEventArgs { UserID = LoggedUser.Id, UserLogin = LoggedUser.Login });

            LoggedUser = null;            
        }

        public List<Acl> GetAclList()
        {
            return new List<Acl>();
        }

        public async Task<List<AclVerb>> GetAclVerbsAsync()
        {
            var aclVerbs = await _aclVerbRepository.GetAllAsync();

            return aclVerbs.ToList();
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _roleRepository.GetAllAsync();
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
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

        public List<Acl> GetRolePermissions(Role role)
        {
            return new List<Acl>();
        }

        public Role GetUserRole(User user)
        {
            return new Role();
        }

        public void RemoveRole(Role role)
        {
            _aclsIDsToDelete = role.ACLs.Select(a => a.Id).ToList();
            _roleRepository.Remove(role);
        }

        public void RemoveUser(User user)
        {
            _userRepository.Remove(user);
        }

        public bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        public bool LoggedUserHasPermission(string verbName)
        {
            var acl = LoggedUser.Role.ACLs.SingleOrDefault(a => a.AclVerb.Name == verbName);

            return acl != null && acl.Value == 1;
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            var role = await _roleRepository.FindByAsync(r => r.Name == roleName);

            return role.Any();
        }

        public async Task SaveAsync()
        {
            await _roleRepository.SaveAsync();
            await _userRepository.SaveAsync();
            if (_aclsIDsToDelete.Count() > 0)
            {
                await RemoveACLs(_aclsIDsToDelete.ToList());
                await _aclRepository.SaveAsync();
            }
        }

        public bool UserExists(string login)
        {
            return false;
        }

        public bool UserIsInRole(string login, Role role)
        {
            return false;
        }

        private void LoadRoles()
        {
            _roles.Clear();
            var roles = _roleRepository.GetAll();
            if (roles != null)
            {
                _roles.AddRange(roles);
            }
        }

        private void LoadUsers()
        {
            _users.Clear();
            var users = _userRepository.GetAll();
            if (users != null)
            {
                _users.AddRange(users);
            }
        }        

        public User LoggedUser
        {
            get { return _loggedUser; }
            set { _loggedUser = value; }
        }

        private async Task RemoveACLs(List<Guid> list)
        {
            var aclsToDelete = await _aclRepository.FindByAsync(a => list.Contains(a.Id));

            foreach (var acl in aclsToDelete)
            {
                _aclRepository.Remove(acl);
            }
        }

        public void RollBackChanges()
        {
            _roleRepository.RollBackChanges();
            _userRepository.RollBackChanges();
        }
    }

    public class ACLVerbCollection : IACLVerbCollection
    {
        private IAclVerbRepository _aclVerbRepository;

        public List<AclVerb> ACLVerbs { get; private set; }

        public ACLVerbCollection(IAclVerbRepository aclVerbRepository)
        {
            _aclVerbRepository = aclVerbRepository;

            ACLVerbs = new List<AclVerb>();

            LoadVerbs();
        }

        private async void LoadVerbs()
        {
            ACLVerbs.Clear();
            var verbs = await _aclVerbRepository.GetAllAsync();
            foreach (var verb in verbs)
            {
                ACLVerbs.Add(verb);
            }
        }

        public AclVerb this[string verbName]
        {
            get
            {
                var acl = GetAclVerb(verbName);
                return acl;
            }
        }

        private AclVerb GetAclVerb(string verbName)
        {
            return ACLVerbs.SingleOrDefault(v => v.Name == verbName);
        }
    }
}