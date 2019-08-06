using ERService.Business;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using ERService.RBAC.Data.Repository;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ERService.Settings.ViewModels
{
    public class UsersSettingsViewModel : DetailViewModelBase, INavigationAware
    {
        private IUserRepository _userRepository;
        private IRegionManager _regionManager;

        public ObservableCollection<User> Users { get; }
        public DelegateCommand AddUserCommand { get; }
        public DelegateCommand EditUserCommand { get; }
        private User _selectedUser;

        public User SelectedUser
        {
            get { return _selectedUser; }
            set { SetProperty(ref _selectedUser, value); EditUserCommand.RaiseCanExecuteChanged(); }
        }


        public Role SelectedRole { get; set; }

        public UsersSettingsViewModel(IEventAggregator eventAggregator, IUserRepository userRepository, IRegionManager regionManager) : base(eventAggregator)
        {
            Title = "Użytkownicy";

            _userRepository = userRepository;
            _regionManager = regionManager;

            Users = new ObservableCollection<User>();

            AddUserCommand = new DelegateCommand(OnAddUserExecute);
            EditUserCommand = new DelegateCommand(OnEditUserExecute, OnEditUserCanExecute);
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

        private void OnAddUserExecute()
        {
            _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.UserDetailView);
        }

        public override async Task LoadAsync()
        {
            await LoadUsers();
        }

        private async Task LoadUsers()
        {
            var users = await _userRepository.GetAllAsync();

            foreach (var user in users)
            {
                Users.Add(user);
            }
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

        protected override void OnCancelEditExecute()
        {
            
        }

        protected override bool OnSaveCanExecute()
        {
            return true;
        }

        protected override void OnSaveExecute()
        {
            
        }
    }
}
