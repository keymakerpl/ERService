using ERService.Business;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Dialogs;
using ERService.Infrastructure.Helpers;
using ERService.RBAC;
using ERService.RBAC.Data.Repository;
using ERService.Settings.Wrapper;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace ERService.Settings.ViewModels
{
    public class UserDetailViewModel : DetailViewModelBase
    {
        private UserWrapper _user;
        private Role _selectedRole;
        private IRBACManager _rbacManager;
        private IRegionManager _regionManager;
        private IPasswordHasher _passwordHasher;
        private IUserRepository _userRepository;
        private IRegionNavigationService _navigationService;

        public UserDetailViewModel(IUserRepository userRepository, IEventAggregator eventAggregator, 
            IRegionManager regionManager, IRBACManager rBACManager,
            IPasswordHasher passwordHasher, IMessageDialogService messageDialogService) : base(eventAggregator, messageDialogService)
        {
            _rbacManager = rBACManager;
            _regionManager = regionManager;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;

            UserRoles = new ObservableCollection<Role>();
            SaveCommand = new DelegateCommand<object>(OnSaveExecute, OnSaveCanExecute);
        }

        public new DelegateCommand<object> SaveCommand { get; set; }

        public Role SelectedRole
        {
            get { return _selectedRole; }
            set { SetProperty(ref _selectedRole, value); User.Model.RoleId = value.Id; }
        }

        public UserWrapper User
        {
            get { return _user; }
            set { SetProperty(ref _user, value); }
        }

        public ObservableCollection<Role> UserRoles { get; }

        public override void Load(Guid id)
        {
            var user = id != Guid.Empty ? _userRepository
                        .FindByInclude(u => u.Id == id, r => r.Role)
                        .ToList()
                        .FirstOrDefault()
                        : GetNewDetail();

            ID = id;

            InitializeRoleCombo();
            Initialize(user);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            _navigationService = navigationContext.NavigationService;

            var id = navigationContext.Parameters.GetValue<Guid>("ID");

            Load(id);

            if (!_rbacManager.LoggedUserHasPermission(AclVerbNames.UserConfiguration))
                IsReadOnly = true;
        }

        protected override void OnCancelEditExecute()
        {
            _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.UserSettingsView);
        }

        protected bool OnSaveCanExecute(object parameter)
        {
            return true;// User != null && !User.HasErrors && HasChanges;
        }

        protected async void OnSaveExecute(object parameter)
        {
            HashPassword(parameter);

            await SaveWithOptimisticConcurrencyAsync(_userRepository.SaveAsync, () =>
            {
                HasChanges = _userRepository.HasChanges(); // Po zapisie ustawiamy flagę na false jeśli nie ma zmian w repo
                ID = User.Id; //odśwież Id wrappera

                //Powiadom agregator eventów, że zapisano
                RaiseDetailSavedEvent(User.Id, $"{User.FirstName} {User.LastName}");

                _regionManager.Regions[RegionNames.ContentRegion].RemoveAll();
                _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.SettingsView);
            });
        }

        private User GetNewDetail()
        {
            var user = new User();

            _userRepository.Add(user);
            return user;
        }

        //TODO: Refactor? - To nie jest solid. Zasada pojedynczej odpowiedzialnosci, przeniesc do RBAC
        private void HashPassword(object parameter)
        {
            string hashedPassword = "";
            string salt = "";
            var passwordBox = parameter as PasswordBox;
            if (passwordBox != null)
            {
                var encryptedPassword = passwordBox.Password;
                _passwordHasher.GenerateSaltedHash(encryptedPassword, out hashedPassword, out salt);
            }

            if (!String.IsNullOrWhiteSpace(hashedPassword) && !String.IsNullOrWhiteSpace(salt))
            {
                User.PasswordHash = hashedPassword;
                User.Salt = salt;
            }
        }

        private void Initialize(User user)
        {
            if (user == null) return;

            User = new UserWrapper(user);

            User.PropertyChanged += ((sender, args) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _userRepository.HasChanges();
                }

                //sprawdzamy czy zmieniony propert w modelu ma błędy i ustawiamy SaveButton
                if (args.PropertyName == nameof(User.HasErrors))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            });

            SaveCommand.RaiseCanExecuteChanged();

            if (User.Id == Guid.Empty)
            {
                User.Login = "";
            }
        }

        private async void InitializeRoleCombo()
        {
            var roles = await _rbacManager.GetAllRolesAsync();
            foreach (var role in roles)
            {
                UserRoles.Add(role);
            }

            if (ID != Guid.Empty)
                SelectedRole = User.Model.Role;
        }
    }
}