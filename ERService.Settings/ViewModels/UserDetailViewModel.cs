using ERService.Business;
using ERService.Infrastructure.Base;
using ERService.RBAC.Data.Repository;
using ERService.Settings.Wrapper;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERService.Settings.ViewModels
{
    public class UserDetailViewModel : DetailViewModelBase, INavigationAware
    {
        private UserWrapper _user;
        private IUserRepository _userRepository;

        public UserWrapper User
        {
            get { return _user; }
            set { SetProperty(ref _user, value); }
        }

        public UserDetailViewModel(IUserRepository userRepository, IEventAggregator eventAggregator) : base(eventAggregator)
        {
            _userRepository = userRepository;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            var id = navigationContext.Parameters.GetValue<Guid>("ID");

            if (id != null)
            {
                await LoadAsync(id);
            }
            else
            {
                await LoadAsync();
            }
        }

        public override async Task LoadAsync() //TODO: Dodać Load()
        {
            var user = GetNewDetail();

            Initialize(user);
        }

        public override async Task LoadAsync(Guid id)
        {
            var user = await _userRepository.FindByAsync(u => u.Id == id);

            Initialize(user.FirstOrDefault());
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
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }

                //sprawdzamy czy zmieniony propert w modelu ma błędy i ustawiamy SaveButton
                if (args.PropertyName == nameof(User.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }

            });

            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();

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
    }
}
