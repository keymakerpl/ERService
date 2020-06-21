﻿using ERService.Business;
using ERService.Infrastructure.Wrapper;
using System;

namespace ERService.Settings.Wrapper
{
    public class UserWrapper : ModelWrapper<User>
    {
        public UserWrapper(User model) : base(model)
        {
        }

        public Guid Id
        {
            get
            {
                return Model.Id;
            }
        }

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

        private string _salt;

        public string Salt
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _salt, value); }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    RaisePropertyChanged();
                }

                _password = value;                
            }
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

        private string _fullName;

        public string FullName
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _fullName, value); }
        }

        private string _initials;

        public string Initials
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _initials, value); }
        }

        private string _phoneNumber;

        public string PhoneNumber
        {
            get { return GetValue<string>(); }
            set
            {
                SetProperty(ref _phoneNumber, String.IsNullOrEmpty(value) ? null : value);
            }
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

        public bool IsSystem
        {
            get { return GetValue<bool>(); }
            set { SetProperty(ref _isSystem, value); }
        }

        private Guid? _roleId;
        public Guid? RoleId
        {
            get => GetValue<Guid>();
            set => SetProperty(ref _roleId, value);
        }

        private Role _role;
        private bool _isSystem;

        public Role Role
        {
            get { return GetValue<Role>(); }
            set { SetProperty(ref _role, value); }
        }
    }
}