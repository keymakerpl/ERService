using ERService.Business;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Dialogs;
using ERService.Infrastructure.Helpers;
using ERService.RBAC.Data.Repository;
using ERService.Settings.Wrapper;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace ERService.Settings.ViewModels
{
    public class UserDetailViewModel : DetailViewModelBase, INavigationAware
    {
        private UserWrapper _user;
        private IRegionManager _regionManager;
        private IUserRepository _userRepository;
        private IPasswordHasher _passwordHasher;
        private IRegionNavigationService _navigationService;
        public new DelegateCommand<object> SaveCommand { get; set; }

        public UserWrapper User
        {
            get { return _user; }
            set { SetProperty(ref _user, value); }
        }

        public UserDetailViewModel(IUserRepository userRepository, IEventAggregator eventAggregator, IRegionManager regionManager, 
            IPasswordHasher passwordHasher, IMessageDialogService messageDialogService) : base(eventAggregator, messageDialogService)
        {
            _regionManager = regionManager;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;

            SaveCommand = new DelegateCommand<object>(OnSaveExecute, OnSaveCanExecute);
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            _navigationService = navigationContext.NavigationService;

            var id = navigationContext.Parameters.GetValue<Guid>("ID");

            await LoadAsync(id);
        }

        public override async Task LoadAsync(Guid id)
        {
            var user = id != Guid.Empty ? await _userRepository.GetByIdAsync(id) : GetNewDetail();

            Initialize(user);
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

        private User GetNewDetail()
        {
            var user = new User();

            _userRepository.Add(user);
            return user;
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

        //TODO: Refactor?
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
    }
}
