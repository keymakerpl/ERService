using ERService.Business;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Dialogs;
using ERService.RBAC;
using ERService.Settings.Wrapper;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ERService.Settings.ViewModels
{
    public class UsersSettingsViewModel : DetailViewModelBase
    {
        private IRBACManager _rbacManager;
        private IRegionManager _regionManager;
        private Role _selectedRole;
        private User _selectedUser;

        public UsersSettingsViewModel(IEventAggregator eventAggregator,
            IRegionManager regionManager, IRBACManager rBACManager,
            IMessageDialogService messageDialogService) : base(eventAggregator, messageDialogService)
        {
            Title = "Użytkownicy";

            _regionManager = regionManager;
            _rbacManager = rBACManager;

            Users = new ObservableCollection<User>();
            Roles = new ObservableCollection<Role>();
            RoleACLs = new ObservableCollection<AclWrapper>();

            AddUserCommand = new DelegateCommand(OnAddUserExecute);
            EditUserCommand = new DelegateCommand(OnEditUserExecute, OnEditUserCanExecute);
            RemoveUserCommand = new DelegateCommand(OnRemoveUserExecute, OnRemoveUserCanExecute);
            AddRoleCommand = new DelegateCommand(OnAddRoleExecute);
            EditRoleCommand = new DelegateCommand(OnEditRoleExecute);
            RemoveRoleCommand = new DelegateCommand(OnRemoveRoleExecute, OnRemoveRoleCanExecute);
        }

        public DelegateCommand AddRoleCommand { get; }
        public DelegateCommand AddUserCommand { get; }
        public DelegateCommand EditRoleCommand { get; }
        public DelegateCommand EditUserCommand { get; }
        public DelegateCommand RemoveRoleCommand { get; }
        public DelegateCommand RemoveUserCommand { get; }
        public ObservableCollection<AclWrapper> RoleACLs { get; }
        public ObservableCollection<Role> Roles { get; }
        public ObservableCollection<User> Users { get; }

        public Role SelectedRole
        {
            get { return _selectedRole; }
            set { SetProperty(ref _selectedRole, value); LoadRoleAcls(value); RemoveRoleCommand.RaiseCanExecuteChanged(); }
        }

        public User SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                SetProperty(ref _selectedUser, value);
                EditUserCommand.RaiseCanExecuteChanged(); RemoveUserCommand.RaiseCanExecuteChanged();
            }
        }

        public override async Task LoadAsync()
        {
            await LoadUsers();
            await LoadRoles();
        }

        private void LoadRoleAcls(Role role)
        {
            if (role == null) return;

            RoleACLs.Clear();
            foreach (var acl in role.ACLs)
            {
                RoleACLs.Add(new AclWrapper(acl));
            }
        }

        private async Task LoadRoles()
        {
            Roles.Clear();
            var roles = await _rbacManager.GetAllRolesAsync();

            foreach (var role in roles)
            {
                Roles.Add(role);
            }
        }

        private async Task LoadUsers()
        {
            var users = await _rbacManager.GetAllUsersAsync();

            foreach (var user in users)
            {
                Users.Add(user);
            }
        }

        #region Events and Event Handlers

        protected override void OnCancelEditExecute()
        {
            _regionManager.Regions[RegionNames.ContentRegion].RemoveAll();
            _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.StartPageView);
        }

        protected override bool OnSaveCanExecute()
        {
            return true;
        }

        protected override async void OnSaveExecute()
        {
            await SaveWithOptimisticConcurrencyAsync(_rbacManager.SaveAsync, () => { });

            _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.StartPageView);
        }

        private async void OnAddRoleExecute()
        {
            var dialogResult = await _messageDialogService
                .ShowInputMessageAsync(this, "Nowa rola", "Podaj nazwę dla nowej roli:");

            if (!String.IsNullOrWhiteSpace(dialogResult))
            {
                if (await _rbacManager.RoleExistsAsync(dialogResult))
                {
                    await _messageDialogService
                        .ShowInformationMessageAsync(this, "Rola już istnieje...", "Rola o podanej nazwie już istnieje.");
                    return;
                }
                else
                {
                    var role = await _rbacManager.GetNewRole(dialogResult);
                    _rbacManager.AddRole(role);
                    Roles.Add(role);
                }
            }
        }

        private void OnAddUserExecute()
        {
            _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.UserDetailView);
        }

        private void OnEditRoleExecute()
        {
            throw new NotImplementedException();
        }

        private bool OnEditUserCanExecute()
        {
            return SelectedUser != null;
        }

        private void OnEditUserExecute()
        {
            var parameters = new NavigationParameters();
            parameters.Add("ID", SelectedUser.Id);

            _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.UserDetailView, parameters);
        }

        private bool OnRemoveRoleCanExecute()
        {
            return SelectedRole != null;
        }

        private async void OnRemoveRoleExecute()
        {
            if (SelectedUser.IsSystem)
            {
                var dialogResult = await _messageDialogService
                    .ShowInformationMessageAsync(this, "Nie można usunąć roli...", "Nie można usunąć roli systemowej.");

                return;
            }

            var confirmDialogResult = await _messageDialogService
                .ShowConfirmationMessageAsync(this, "Czy usunąć rolę?", $"Czy usunąć rolę {SelectedRole.Name}?");

            if (confirmDialogResult == DialogResult.Cancel) return;

            _rbacManager.RemoveRole(SelectedRole);
            Roles.Remove(SelectedRole);
            RoleACLs.Clear();
        }

        private bool OnRemoveUserCanExecute()
        {
            return SelectedUser != null;
        }

        private async void OnRemoveUserExecute()
        {
            if (SelectedUser.IsSystem)
            {
                var dialogResult = await _messageDialogService
                    .ShowInformationMessageAsync(this, "Nie można usunąć użytkownika...", "Nie można usunąć użytkownika systemowego.");

                return;
            }

            var confirmDialogResult = await _messageDialogService
                .ShowConfirmationMessageAsync(this, "Czy usunąć użytkownika?", $"Czy usunąć użytkownika {SelectedUser.FirstName} {SelectedUser.LastName}?");

            if (confirmDialogResult == DialogResult.Cancel) return;

            _rbacManager.RemoveUser(SelectedUser);
            Users.Remove(SelectedUser);
        }

        #endregion Events and Event Handlers

        #region Navigation

        public override bool KeepAlive => true;

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            await LoadAsync();
        }

        #endregion Navigation
    }
}