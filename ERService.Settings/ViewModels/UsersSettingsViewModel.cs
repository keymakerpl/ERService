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
using System.Linq;
using ERService.Infrastructure.Events;

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
            EditRoleCommand = new DelegateCommand(OnEditRoleExecute, OnEditRoleCanExecute);
            RemoveRoleCommand = new DelegateCommand(OnRemoveRoleExecute, OnRemoveRoleCanExecute);

            _eventAggregator.GetEvent<AfterDetailSavedEvent>().Subscribe(OnUserDetailSaved);
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
            set
            {
                SetProperty(ref _selectedRole, value);
                LoadRoleAcls(value);
                RemoveRoleCommand.RaiseCanExecuteChanged();
                EditRoleCommand.RaiseCanExecuteChanged();
            }
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

        public override void Load()
        {
            LoadUsers();
            LoadRoles();
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

        private void LoadRoles()
        {
            Roles.Clear();
            foreach (var role in _rbacManager.Roles)
            {
                Roles.Add(role);
            }
        }

        private void LoadUsers()
        {
            Users.Clear();
            foreach (var user in _rbacManager.Users)
            {
                Users.Add(user);
            }
        }

        #region Events and Event Handlers

        private void OnUserDetailSaved(AfterDetailSavedEventArgs args)
        {
            if (args.ViewModelName == typeof(UserDetailViewModel).Name)
            {
                LoadUsers();
            }
        }

        protected override void OnCancelEditExecute()
        {
            //TODO: _navigationservice
            _rbacManager.RollBackChanges();

            _regionManager.Regions[RegionNames.ContentRegion].RemoveAll();
            _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.StartPageView);
        }

        protected override bool OnSaveCanExecute()
        {
            return true;
        }

        protected override async void OnSaveExecute()
        {
            await SaveWithOptimisticConcurrencyAsync(_rbacManager.SaveAsync, async () => 
            {
                HasChanges = _rbacManager.HasChanges();
                SaveCommand.RaiseCanExecuteChanged();

                await _rbacManager.Refresh();
            });
        }

        private async void OnAddRoleExecute()
        {
            if (!_rbacManager.LoggedUser.IsAdmin && !_rbacManager.LoggedUserHasPermission(AclVerbNames.UserConfiguration))
            {
                await _messageDialogService.ShowAccessDeniedMessageAsync(this);
                return;
            }

            var dialogResult = await _messageDialogService
                .ShowInputMessageAsync(this, "Nowa rola...", "Podaj nazwę nowej roli:");

            if (!String.IsNullOrWhiteSpace(dialogResult))
            {
                if (await _rbacManager.RoleExistsAsync(dialogResult))
                {
                    await _messageDialogService
                        .ShowInformationMessageAsync(this, "Rola już istnieje...", "Rola o podanej nazwie już istnieje.");
                }
                else
                {
                    var role = await _rbacManager.GetNewRole(dialogResult);
                    _rbacManager.AddRole(role);
                    Roles.Add(role);
                }
            }
        }

        private async void OnAddUserExecute()
        {
            if (!_rbacManager.LoggedUser.IsAdmin && !_rbacManager.LoggedUserHasPermission(AclVerbNames.UserConfiguration))
            {
                await _messageDialogService.ShowAccessDeniedMessageAsync(this);
                return;
            }

            _eventAggregator.GetEvent<AfterSideMenuExpandToggled>()
                .Publish(new AfterSideMenuExpandToggledArgs
                {
                    Flyout = SideFlyouts.DetailFlyout,
                    ViewName = ViewNames.UserDetailView
                });
        }

        private async void OnEditRoleExecute()
        {
            var newRoleName = await _messageDialogService.ShowInputMessageAsync(this, "Edycja roli...", "Wprowadź nową nazwę roli:");
            if (!String.IsNullOrWhiteSpace(newRoleName))
            {
                SelectedRole.Name = newRoleName;
                LoadRoles();
            }
        }

        private bool OnRemoveRoleCanExecute()
        {
            return SelectedRole != null;
        }

        private bool OnEditUserCanExecute()
        {
            return SelectedUser != null;
        }

        private void OnEditUserExecute()
        {
            var parameters = new NavigationParameters();
            parameters.Add("ID", SelectedUser.Id);

            _eventAggregator.GetEvent<AfterSideMenuExpandToggled>()
                .Publish(new AfterSideMenuExpandToggledArgs
                {
                    Flyout = SideFlyouts.DetailFlyout,
                    ViewName = ViewNames.UserDetailView,
                    DetailID = SelectedUser.Id
                });

            //_regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.UserDetailView, parameters);
        }

        private bool OnEditRoleCanExecute()
        {
            return SelectedRole != null;
        }

        private async void OnRemoveRoleExecute()
        {
            if (!_rbacManager.LoggedUser.IsAdmin && !_rbacManager.LoggedUserHasPermission(AclVerbNames.UserConfiguration))
            {
                await _messageDialogService.ShowAccessDeniedMessageAsync(this);
                return;
            }

            if (SelectedRole.IsSystem)
            {
                await _messageDialogService
                    .ShowInformationMessageAsync(this, "Nie można usunąć roli...", "Nie można usunąć roli systemowej.");

                return;
            }

            if (SelectedRole.Id == _rbacManager.LoggedUser.RoleId)
            {
                await _messageDialogService
                    .ShowInformationMessageAsync(this, "Nie można usunąć roli...", "Nie można usunąć obecnie używanej roli.");
            }

            if (Users.Any(u => u.RoleId == SelectedRole.Id))
            {
                await _messageDialogService.ShowInformationMessageAsync(this, "Rola w użyciu...", "Wybrana rola jest w użyciu, nie można jej usunąć.");
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
            if (!_rbacManager.LoggedUser.IsAdmin && !_rbacManager.LoggedUserHasPermission(AclVerbNames.UserConfiguration))
            {
                await _messageDialogService.ShowAccessDeniedMessageAsync(this);
                return;
            }

            if (SelectedUser.IsSystem)
            {
                await _messageDialogService
                    .ShowInformationMessageAsync(this, "Nie można usunąć użytkownika...", "Nie można usunąć użytkownika systemowego.");

                return;
            }

            if (SelectedUser.Id == _rbacManager.LoggedUser.Id)
            {
                await _messageDialogService
                    .ShowInformationMessageAsync(this, "Nie można usunąć użytkownika...", "Nie można usunąć użytkownika z którego obecnie korzystasz.");
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

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            Load();

            if (!_rbacManager.LoggedUserHasPermission(AclVerbNames.UserConfiguration))
                IsReadOnly = true;
        }
        
        #endregion Navigation
    }
}