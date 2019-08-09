using ERService.Business;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Dialogs;
using ERService.RBAC;
using ERService.RBAC.Data.Repository;
using ERService.Settings.Wrapper;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Threading.Tasks;

namespace ERService.Settings.ViewModels
{
    public class UsersSettingsViewModel : DetailViewModelBase, INavigationAware
    {
        private IRegionManager _regionManager;
        private IRBACManager _rbacManager;
        private IRoleRepository _roleRepository;
        private Role _selectedRole;
        private User _selectedUser;
        private IUserRepository _userRepository;

        public UsersSettingsViewModel(IEventAggregator eventAggregator, IUserRepository userRepository,
            IRoleRepository roleRepository, IRegionManager regionManager, IRBACManager rBACManager,
            IMessageDialogService messageDialogService) : base(eventAggregator, messageDialogService)
        {
            Title = "Użytkownicy";

            _userRepository = userRepository; //TODO: Zamienić na RBACManager
            _roleRepository = roleRepository; //
            _regionManager = regionManager;
            _rbacManager = rBACManager;

            Users = new ObservableCollection<User>();
            Roles = new ObservableCollection<Role>();
            RoleACLs = new ObservableCollection<AclWrapper>();

            AddUserCommand = new DelegateCommand(OnAddUserExecute);
            EditUserCommand = new DelegateCommand(OnEditUserExecute, OnEditUserCanExecute);
            RemoveUserCommand = new DelegateCommand(OnRemoveUserExecute, OnRemoveUserCanExecute);
            AddRoleCommand = new DelegateCommand(OnAddRoleCommand);
            EditRoleCommand = new DelegateCommand(OnEditRoleCommand);
            RemoveRoleCommand = new DelegateCommand(OnRemoveRoleCommand, OnRemoveRoleCanExecute);
        }        

        public DelegateCommand AddUserCommand { get; }
        public DelegateCommand EditUserCommand { get; }
        public DelegateCommand RemoveUserCommand { get; }
        public DelegateCommand AddRoleCommand { get; }
        public DelegateCommand EditRoleCommand { get; }
        public DelegateCommand RemoveRoleCommand { get; }
        public ObservableCollection<User> Users { get; }
        public ObservableCollection<Role> Roles { get; }
        public ObservableCollection<AclWrapper> RoleACLs { get; }
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

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            await LoadAsync();
        }
       
        private void LoadRoleAcls(Role role)
        {
            RoleACLs.Clear();
            foreach (var acl in role.ACLs)
            {
                RoleACLs.Add(new AclWrapper(acl));
            }
        }

        public override async Task LoadAsync()
        {
            await LoadUsers();
            await LoadRoles();
        }

        private async Task LoadRoles()
        {
            var roles = await _roleRepository.GetAllAsync();

            foreach (var role in roles)
            {
                Roles.Add(role);
            }
        }

        private async Task LoadUsers()
        {
            var users = await _userRepository.GetAllAsync();

            foreach (var user in users)
            {
                Users.Add(user);
            }
        }

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
            foreach (var acl in RoleACLs)
            {
                _roleRepository.SetEntityStatus(acl.Model, EntityState.Modified);
            }

            await SaveWithOptimisticConcurrencyAsync(_userRepository.SaveAsync, () => { });
            await SaveWithOptimisticConcurrencyAsync(_roleRepository.SaveAsync, () =>
            {
                _regionManager.Regions[RegionNames.ContentRegion].RemoveAll();
                _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.StartPageView);
            });
        }

        private void OnAddUserExecute()
        {
            _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.UserDetailView);
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

        private bool OnRemoveUserCanExecute()
        {
            return SelectedUser != null && SelectedUser.IsAdmin == 0;
        }

        private async void OnRemoveUserExecute()
        {
            var dialogResult = await _messageDialogService
                .ShowConfirmationMessageAsync(this, "Czy usunąć użytkownika?", $"Czy usunąć użytkownika {SelectedUser.FirstName} {SelectedUser.LastName}?");

            if (dialogResult == DialogResult.Cancel) return;

            Users.Remove(SelectedUser);
            _userRepository.Remove(SelectedUser);
        }

        private void OnEditRoleCommand()
        {
            throw new NotImplementedException();
        }

        private async void OnAddRoleCommand()
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
                     //TODO: RBAC
                }
            }
        }

        private async void OnRemoveRoleCommand()
        {
            var dialogResult = await _messageDialogService
                .ShowConfirmationMessageAsync(this, "Czy usunąć rolę?", $"Czy usunąć rolę {SelectedRole.Name}?");

            if (dialogResult == DialogResult.Cancel) return;

            Roles.Remove(SelectedRole);
            _roleRepository.Remove(SelectedRole);
        }

        private bool OnRemoveRoleCanExecute()
        {
            return SelectedRole != null;
        }
    }
}