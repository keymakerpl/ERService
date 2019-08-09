﻿using ERService.Business;
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
        private IPasswordHasher _passwordHasher;
        private IEventAggregator _eventAggregator;
        private IEnumerable<User> _users;

        public RBACManager(IUserRepository userRepository, IRoleRepository roleRepository, 
            IPasswordHasher passwordHasher, IEventAggregator eventAggregator)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
            _eventAggregator = eventAggregator;

            LoadUsers();
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

            return role != null;
        }

        public bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
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
