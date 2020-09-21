using ERService.Business;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Dialogs;
using ERService.Infrastructure.Events;
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
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ERService.Settings.ViewModels
{
    public class UserDetailViewModel : DetailViewModelBase
    {
        private UserWrapper _user;
        private Role _selectedRole;        
        private IRegionManager _regionManager;
        private readonly IUserRepository _userRepository;
        private readonly IRBACManager _rbacManager;
        private IPasswordHasher _passwordHasher;
        private IRegionNavigationService _navigationService;        
        private bool PasswordChanged;

        public UserDetailViewModel(IEventAggregator eventAggregator, 
            IRegionManager regionManager, IUserRepository userRepository, IRBACManager rBACManager,
            IPasswordHasher passwordHasher, IMessageDialogService messageDialogService) : base(eventAggregator, messageDialogService)
        {
            _regionManager = regionManager;
            _userRepository = userRepository;
            _rbacManager = rBACManager;
            _passwordHasher = passwordHasher;

            UserRoles = new ObservableCollection<Role>();
            SaveCommand = new DelegateCommand<object>(OnSaveExecute, OnSaveCanExecute);
            RemovePasswordCommand = new DelegateCommand(() => User.Password = "");
        }

        public new DelegateCommand<object> SaveCommand { get; set; }
        public DelegateCommand RemovePasswordCommand { get; }

        public Role SelectedRole
        {
            get { return _selectedRole; }
            set { SetProperty(ref _selectedRole, value); User.RoleId = value.Id; }
        }

        public UserWrapper User
        {
            get { return _user; }
            set { SetProperty(ref _user, value); }
        }

        public ObservableCollection<Role> UserRoles { get; }

        public override async Task LoadAsync(Guid id)
        {
            var user = id != Guid.Empty ? await _userRepository.GetByIdAsync(id)
                        : GetNewDetail();

            ID = id;

            InitializeRoleCombo();
            Initialize(user);
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            _navigationService = navigationContext.NavigationService;

            var id = navigationContext.Parameters.GetValue<Guid>("ID");

            await LoadAsync(id);

            if (!_rbacManager.LoggedUserHasPermission(AclVerbNames.UserConfiguration))
                IsReadOnly = true;
        }

        protected override void OnCancelEditExecute()
        {
            _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.UserSettingsView);
        }

        protected bool OnSaveCanExecute(object parameter)
        {
            return User != null && !User.HasErrors && HasChanges;
        }

        protected async void OnSaveExecute(object parameter)
        {
            var confirmPswd = await ConfirmPassword();
            if (!confirmPswd)
            {
                await _messageDialogService.ShowInformationMessageAsync(this, "Hasła są różne...", "Wprowadzone hasło różni się od zmienionego hasła");
                return;
            }

            HashPassword();

            await SaveWithOptimisticConcurrencyAsync(_userRepository.SaveAsync, () =>
            {
                HasChanges = _userRepository.HasChanges();
                ID = User.Id;
                
                _eventAggregator
                    .GetEvent<AfterSideMenuExpandToggled>()
                    .Publish(new AfterSideMenuExpandToggledArgs
                    {
                        Flyout = SideFlyouts.DetailFlyout
                    });
            });
        }

        private async Task<bool> ConfirmPassword()
        {
            if (!PasswordChanged)
                return true;

            var password = await _messageDialogService.ShowInputMessageAsync(this, "Hasło zostało zmienione...", "Wprowadź ponownie nowe hasło:");
            return password == User.Password;
        }

        private User GetNewDetail()
        {
            var user = new User();
            _userRepository.Add(user);

            return user;
        }
        
        private void HashPassword()
        {
            if (PasswordChanged || ID == Guid.Empty)
            {
                string hashedPassword = "";
                string salt = "";

                _passwordHasher.GenerateSaltedHash(User.Password, out hashedPassword, out salt);

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

                if (!HasChanges && args.PropertyName == nameof(UserWrapper.Password))
                {
                    HasChanges = true;
                    PasswordChanged = true;
                }

                SaveCommand.RaiseCanExecuteChanged();
            });

            SaveCommand.RaiseCanExecuteChanged();

            if (User.Id == Guid.Empty)
            {
                User.Login = "";
                User.Password = "";
                User.IsActive = true;
                User.IsAdmin = false;

                var role = UserRoles.FirstOrDefault(r => r.IsSystem);
                if (role != null)
                    SelectedRole = role;
            }

            if (ID != Guid.Empty && User.Model.Role != null)
                SelectedRole = User.Model.Role;
        }

        private void InitializeRoleCombo()
        {
            foreach (var role in _rbacManager.Roles.Where(r => r.Id != Guid.Empty))
            {
                UserRoles.Add(role);
            }            
        }
    }
}