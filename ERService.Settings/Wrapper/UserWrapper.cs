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
            get { return _login; }
            set { SetProperty(ref _login, value); }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        private string _firstName;

        public string FirstName
        {
            get { return _firstName; }
            set { SetProperty(ref _firstName, value); }
        }

        private string _lastName;

        public string LastName
        {
            get { return _lastName; }
            set { SetProperty(ref _lastName, value); }
        }

        private string _phoneNumber;

        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set { SetProperty(ref _phoneNumber, value); }
        }

        private int _isActive;

        public int IsActive
        {
            get { return _isActive; }
            set { SetProperty(ref _isActive, value); }
        }

        private int _isAdministrator;

        public int IsAdministrator
        {
            get { return _isAdministrator; }
            set { SetProperty(ref _isAdministrator, value); }
        }

    }
}
