using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERService.Business;
using ERService.Infrastructure.Helpers;
using ERService.RBAC;
using ERService.RBAC.Data.Repository;
using Prism.Events;
using Xunit;
using System.Linq;
using Moq;
using ERService.Infrastructure.Events;
using Unity;

namespace RBACLibTestsXUnit
{
    public class RBACManagerTests
    {
        private RBACManager _rbacManager;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IRoleRepository> _roleRepositoryMock;

        public RBACManagerTests()
        {
            var container = new UnityContainer();
            container.RegisterType<IPasswordHasher, PasswordHasher>();

            //TODO: Refactor to Generic RepositoryFactory
            _userRepositoryMock = new Mock<IUserRepository>();
            _userRepositoryMock.Setup(dp => dp.GetAllAsync())
                .Returns(Task.FromResult(GetUsers()));

            _roleRepositoryMock = new Mock<IRoleRepository>();
            _roleRepositoryMock.Setup(dp => dp.GetAllAsync())
                .Returns(Task.FromResult(GetRoles()));

            var aclRepositoryMock = new Mock<IAclRepository>();
            var passwordHasherMock = new Mock<IPasswordHasher>();

            var loggedInEventMock = new Mock<AfterUserLoggedinEvent>();
            var eventAggregatorMock = new Mock<IEventAggregator>();
            eventAggregatorMock.Setup(ea => ea.GetEvent<AfterUserLoggedinEvent>())
                .Returns(loggedInEventMock.Object);

            var loggedOutEventMock = new Mock<AfterUserLoggedoutEvent>();
            eventAggregatorMock.Setup(ea => ea.GetEvent<AfterUserLoggedoutEvent>())
                .Returns(loggedOutEventMock.Object);

            var aclVerbRepositoryMock = new Mock<IAclVerbRepository>();
            aclVerbRepositoryMock.Setup(v => v.GetAllAsync())
                .Returns(Task.FromResult(GetACLVerbs()));

            _rbacManager = new RBACManager(_userRepositoryMock.Object, _roleRepositoryMock.Object, aclRepositoryMock.Object,
                container.Resolve<IPasswordHasher>(), eventAggregatorMock.Object, aclVerbRepositoryMock.Object);
        }

        private IEnumerable<User> GetUsers()
        {
            yield return new User { Id = Guid.NewGuid(), Login = "administrator",
                PasswordHash = "/HMO54rRxNa+SBxAH3Mamqn2gbiaydN80pO9BNyPxcB5LMCPTobg6fR9rTTLgo8w9lV4IFdnR0QyKUTfMgFdRvTxQMGIK0zOKXdDT3uQg86Qa7DPAMkiAYv/ipg+9mUbuGwhvSCTEAfA8yQ4JXKiNo6acqWKlSsHN9Ezh48dwX1D4GupU4DsSRigeGZ0eIMoLuH0ofPwCMWeLo/tzaJirGwzeBHvECqWeLjhLaBKQaXPvvrMxzAOaaYSFmFmiSmJoM4hxaj0Y9Sg/vyritqkmN6cjvcPFj71bJTk79Jh8t7rFSR4qUNzqfKC6t6X3lHL2Xh3VAarhxJ+h5P5AsbYMQ==",
                Salt = "ScbGvPwGi2xjUD5wfe0/Ty3Rot3e6G4NRpXbrIpJ8tf4U6H+dQe414sbeJey3NPifszbOpI0BxSa1O/npi32AQ==",                
            };
            yield return new User { Id = Guid.NewGuid(), Login = "rkk", PasswordHash = "21h31239183281u28", Salt = "fj2f2hj09jd3" };
        }

        private IEnumerable<Role> GetRoles()
        {
            yield return new Role { Id = Guid.NewGuid(), Name = "Administrator", IsSystem = true };
            yield return new Role { Id = Guid.NewGuid(), Name = "Serwisant", IsSystem = false };
        }

        private IEnumerable<AclVerb> GetACLVerbs()
        {
            yield return new AclVerb { Id = Guid.NewGuid(), Name = "Usuwanie napraw", DefaultValue = 0};
            yield return new AclVerb { Id = Guid.NewGuid(), Name = "Usuwanie klientów", DefaultValue = 1 };
        }

        [Fact]
        public void GetAllUsersAsync_ShouldReturnNonEmptyIEnumerable()
        {
            var users = _rbacManager.Users;

            Assert.Equal(2, users.Count());

            var user = users.FirstOrDefault(u => u.Login == "administrator");
            Assert.NotNull(user);

            user = users.FirstOrDefault(u => u.Login == "rkk");
            Assert.NotNull(user);
        }        

        [Fact]
        public void GetAllRolesAsync_ShouldReturnNonEmptyIEnumerable()
        {
            var roles = _rbacManager.Roles;

            Assert.Equal(2, roles.Count());

            var role = roles.FirstOrDefault(r => r.Name == "Administrator");
            Assert.NotNull(role);

            role = roles.FirstOrDefault(r => r.Name == "Serwisant");
            Assert.NotNull(role);
        }

        [Theory]
        [InlineData(true, "administrator", "123123")]
        public void SuccessLoginShouldReturnTrue(bool expectedResult, string login, string password)
        {
            var result = _rbacManager.Login(login, password);

            Assert.Equal(expectedResult, result);
            Assert.NotNull(_rbacManager.LoggedUser);
        }

        [Theory]
        [InlineData(false, "administrator", "4656423")]
        [InlineData(false, "adm1n1strator", "123123")]
        public void FailLoginShouldReturnFalse(bool expectedResult, string login, string password)
        {
            var result = _rbacManager.Login(login, password);

            Assert.Equal(expectedResult, result);
            Assert.Null(_rbacManager.LoggedUser);
        }

        [Fact]
        public void LoggedUserShouldReturnNullAfterLogout()
        {
            _rbacManager.Logout();

            Assert.Null(_rbacManager.LoggedUser);
        }

        [Fact]
        public async void GetACLVerbsShouldReturnNonEmptyIEnumerable()
        {
            var verbs = await _rbacManager.GetAclVerbsAsync();

            Assert.True(verbs.Any());
        }

        [Fact]
        public void LoggedUserShouldEqualOneOfUsers()
        {

        }

        [Theory]
        [InlineData("Administrator")]
        public async void GetNewRole_ShouldReturnRoleObject(string roleName)
        {
            var role = await _rbacManager.GetNewRole(roleName);
            var expectedType = typeof(Role);

            Assert.NotNull(role);
            Assert.Equal(role.Name, roleName);
            Assert.IsType(expectedType, role);
            Assert.True(role.ACLs.Any());
        }
    }
}
