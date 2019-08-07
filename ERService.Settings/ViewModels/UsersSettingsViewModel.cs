using ERService.Business;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using ERService.RBAC.Data.Repository;
using ERService.Settings.Wrapper;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Threading.Tasks;

namespace ERService.Settings.ViewModels
{
    public class UsersSettingsViewModel : DetailViewModelBase, INavigationAware
    {
        private IRegionManager _regionManager;
        private IRoleRepository _roleRepository;
        private Role _selectedRole;
        private User _selectedUser;
        private IUserRepository _userRepository;

        public UsersSettingsViewModel(IEventAggregator eventAggregator, IUserRepository userRepository,
            IRoleRepository roleRepository, IRegionManager regionManager) : base(eventAggregator)
        {
            Title = "Użytkownicy";

            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _regionManager = regionManager;

            Users = new ObservableCollection<User>();
            Roles = new ObservableCollection<Role>();
            RoleACLs = new ObservableCollection<AclWrapper>();

            AddUserCommand = new DelegateCommand(OnAddUserExecute);
            EditUserCommand = new DelegateCommand(OnEditUserExecute, OnEditUserCanExecute);
            RemoveUserCommand = new DelegateCommand(OnRemoveUserExecute, OnRemoveUserCanExecute);
        }

        public DelegateCommand AddUserCommand { get; }
        public DelegateCommand EditUserCommand { get; }
        public DelegateCommand RemoveUserCommand { get; }
        public ObservableCollection<User> Users { get; }
        public ObservableCollection<Role> Roles { get; }
        public ObservableCollection<AclWrapper> RoleACLs { get; }
        public Role SelectedRole
        {
            get { return _selectedRole; }
            set { SetProperty(ref _selectedRole, value); LoadRoleAcls(value); }
        }
        public User SelectedUser
        {
            get { return _selectedUser; }
            set { SetProperty(ref _selectedUser, value); EditUserCommand.RaiseCanExecuteChanged(); }
        }        

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public async void OnNavigatedTo(NavigationContext navigationContext)
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
            return SelectedUser != null;
        }

        private void OnRemoveUserExecute()
        {
            Users.Remove(SelectedUser);
            _userRepository.Remove(SelectedUser);
        }
    }
}