using ERService.Business;
using ERService.Infrastructure.Events;
using ERService.Infrastructure.Helpers;
using ERService.Infrastructure.Repositories;
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
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private IAclRepository _aclRepository;
        private IEnumerable<Guid> _aclsIDsToDelete;
        private IAclVerbRepository _aclVerbRepository;
        private readonly IUnitOfWork _unitOfWork;
        private IEventAggregator _eventAggregator;
        private IPasswordHasher _passwordHasher;
        private IRoleRepository _roleRepository;
        private IUserRepository _userRepository;

        public RBACManager(IUserRepository userRepository, IRoleRepository roleRepository, IAclRepository aclRepository,
            IPasswordHasher passwordHasher, IEventAggregator eventAggregator, IAclVerbRepository aclVerbRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _aclVerbRepository = aclVerbRepository;
            _unitOfWork = unitOfWork;
            _aclRepository = aclRepository;
            _passwordHasher = passwordHasher;
            _eventAggregator = eventAggregator;

            _aclsIDsToDelete = new List<Guid>(); //TODO: Czy można się tego pozbyć?

            Users = new List<User>();
            Roles = new List<Role>();
        }
        
        public async Task LoadAsync()
        {
            await LoadUsers();
            await LoadRoles();
        }        

        public bool Login(string login, string password)
        {
            if (String.IsNullOrWhiteSpace(login))
            {
                return false;
            }

            var user = Users.SingleOrDefault(u => u.Login == login);
            if (user == null) return false;

            if (_passwordHasher.VerifyPassword(password, user.PasswordHash, user.Salt))
            {
                _eventAggregator.GetEvent<AfterUserLoggedinEvent>()
                    .Publish(new UserAuthorizationEventArgs
                    {
                        UserID = user.Id,
                        UserLogin = user.Login,
                        UserName = user.FirstName,
                        UserLastName = user.LastName
                    });

                LoggedUser = user;
                return true;
            }

            return false;
        }

        public void Logout()
        {
            if (LoggedUser == null) return;

            _eventAggregator.GetEvent<AfterUserLoggedoutEvent>()
                .Publish(new UserAuthorizationEventArgs
                {
                    UserID = LoggedUser.Id,
                    UserLogin = LoggedUser.Login
                });

            LoggedUser = null;
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

        public void RollBackChanges()
        {
            _roleRepository.RollBackChanges();
            _userRepository.RollBackChanges();
            _aclRepository.RollBackChanges();
        }

        public bool HasChanges()
        {
                return _userRepository.HasChanges()
                    || _roleRepository.HasChanges()
                    || _aclRepository.HasChanges()
                    || _aclVerbRepository.HasChanges();
        }

        public async Task RefreshAsync()
        {
            await _aclRepository.ReloadEntitiesAsync();
            await _userRepository.ReloadEntitiesAsync();
            await LoadAsync();
        }

        #region User

        public User LoggedUser { get; set; }

        public List<User> Users { get; set; }        

        private async Task LoadUsers()
        {
            var users = await _userRepository.GetAllAsync();

            Users.Clear();
            foreach (var user in users)
            {
                Users.Add(user);
            }

        }

        public void AddUser(User user)
        {
            Users.Add(user);
            _userRepository.Add(user);
        }

        public void RemoveUser(User user)
        {
            Users.Remove(user);
            _userRepository.Remove(user);
        }

        #endregion

        #region Role

        public List<Role> Roles { get; set; }

        private async Task LoadRoles()
        {
            Roles.Clear();
            var roles = await _roleRepository.GetAllAsync();
            if (roles != null)
            {
                foreach (var role in roles)
                {
                    Roles.Add(role);
                }
            }
        }

        /// <summary>
        /// Add role to repository
        /// </summary>
        /// <param name="roleName">Role</param>
        public void AddRole(Role role)
        {
            Roles.Add(role);
            _roleRepository.Add(role);
        }

        public void RemoveRole(Role role)
        {
            _aclsIDsToDelete = role.ACLs.Select(a => a.Id).ToList();
            Roles.Remove(role);
            _roleRepository.Remove(role);
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            var t = await Task.Run<bool>(() =>
            {
                var exists = Roles.Any(r => r.Name == roleName);
                return exists;
            });

            return t;
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
        
        #endregion

        #region ACL

        public async Task<List<AclVerb>> GetAclVerbsAsync()
        {
            var aclVerbs = await _aclVerbRepository.GetAllAsync();

            return aclVerbs.ToList();
        }

        public List<Acl> GetAclList()
        {
            return new List<Acl>();
        }

        public List<Acl> GetRolePermissions(Role role)
        {
            return new List<Acl>();
        }

        private async Task RemoveACLs(List<Guid> ids)
        {
            var aclsToDelete = await _aclRepository.FindByAsync(a => ids.Contains(a.Id));

            foreach (var acl in aclsToDelete)
            {
                _aclRepository.Remove(acl);
            }
        }

        public bool LoggedUserHasPermission(string verbName)
        {
            if (!LoggedUser.RoleId.HasValue)
                return false;

            var sql = new SQLQueryBuilder(nameof(Acl));

                sql .Select(nameof(Acl.Value))
                    .Join(nameof(AclVerb), nameof(Acl.AclVerbId), $"{nameof(AclVerb)}.{nameof(AclVerb.Id)}")
                    .Where(nameof(Acl.RoleId), LoggedUser.RoleId)
                    .Where(nameof(AclVerb.Name), verbName);

            var parameters = new object[0];
            var query = sql.Compile(out parameters);

            var result = _aclRepository.Get<int?>(query, parameters).FirstOrDefault();
            return result.HasValue && result == 1;
        }

        #endregion

        #region Index

        public User this[string login]
        {
            get
            {
                return Users.SingleOrDefault(u => u.Login.Equals(login, StringComparison.OrdinalIgnoreCase));
            }
        }

        public User this[Guid userId]
        {
            get
            {
                return Users.SingleOrDefault(u => u.Id == userId);
            }
        }

        #endregion
    }
}