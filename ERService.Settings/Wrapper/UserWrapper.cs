using ERService.Business;
using ERService.Infrastructure.Wrapper;
using System;

namespace ERService.Settings.Wrapper
{
    public class UserWrapper : ModelWrapper<User>
    {
        public UserWrapper(User model) : base(model)
        {
        }

        public Guid Id { get { return Model.Id; } }

        private string _login;

        public string Login
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _login, value); }
        }

        private string _passwordHash;

        public string PasswordHash
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _passwordHash, value); }
        }

        private string _firstName;

        public string FirstName
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _firstName, value); }
        }

        private string _lastName;

        public string LastName
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _lastName, value); }
        }

        private string _phoneNumber;

        public string PhoneNumber
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _phoneNumber, value); }
        }

        private bool _isActive;

        public bool IsActive
        {
            get { return GetValue<bool>(); }
            set { SetProperty(ref _isActive, value); }
        }

        private bool _isAdmin;

        public bool IsAdmin
        {
            get { return GetValue<bool>(); }
            set { SetProperty(ref _isAdmin, value); }
        }

        private string _salt;

        public string Salt
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _salt, value); }
        }

        private Guid? _roleId;
        public Guid? RoleId { get { return GetValue<Guid>(); } set { SetProperty(ref _roleId, value); } }


        private Role _role;
        public Role Role
        {
            get { return GetValue<Role>(); }
            set { SetProperty(ref _role, value); }
        }
    }
}