using ERService.OrderModule.Repository;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Reflection;
using System.Linq;
using ERService.Infrastructure.Events;
using ERService.RBAC;
using Prism.Commands;
using Prism.Regions;
using ERService.Infrastructure.Constants;

namespace ERService.Header.ViewModels
{
    public class HeaderViewModel : BindableBase
    {
        private int _inProgressCounter;

        private string _currentUserFullName;

        public string CurrentUserFullName
        {
            get { return _currentUserFullName; }
            set { SetProperty(ref _currentUserFullName, value); }
        }

        public int InProgressCounter
        {
            get { return _inProgressCounter; }
            set { SetProperty(ref _inProgressCounter, value); }
        }

        private int _expiredOrderCounter;
        private IOrderRepository _orderRepository;
        private IEventAggregator _eventAggregator;
        //private readonly IRBACManager _rBACManager;
        private readonly IRegionManager _regionManager;

        public DelegateCommand UserLogoutCommand { get; private set; }
        public DelegateCommand UserSettingsCommand { get; private set; }

        public int ExpiredOrderCounter
        {
            get { return _expiredOrderCounter; }
            set { SetProperty(ref _expiredOrderCounter, value); }
        }        

        public HeaderViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            //_orderRepository = orderRepository;
            _eventAggregator = eventAggregator;
            //_rBACManager = rBACManager;
            _regionManager = regionManager;

            UserLogoutCommand = new DelegateCommand(OnUserLogoutExecute);
            UserSettingsCommand = new DelegateCommand(OnUserSettingsExecute);

            _eventAggregator.GetEvent<AfterUserLoggedinEvent>().Subscribe(OnUserLogged, true);
            _eventAggregator.GetEvent<AfterUserLoggedoutEvent>().Subscribe(OnUserLoggedout, true);
        }

        private void OnUserSettingsExecute()
        {
            //if (_rBACManager.LoggedUser == null) return;

            var parameters = new NavigationParameters();
            //parameters.Add("ID", _rBACManager.LoggedUser.Id);

            _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.UserDetailView, parameters);
        }

        private void OnUserLogoutExecute()
        {
            //_rBACManager.Logout();
        }

        private void OnUserLoggedout(UserAuthorizationEventArgs obj)
        {
            CurrentUserFullName = String.Empty;
        }

        private void OnUserLogged(UserAuthorizationEventArgs args)
        {
            CurrentUserFullName = !String.IsNullOrEmpty(args.UserLastName) ? $"{args.UserName} {args.UserLastName}" : args.UserLogin;
            RefreshCounters();
        }

        public async void RefreshCounters()
        {
            //var orders = await _orderRepository.GetAllAsync();
            //if (orders != null)
            //{
            //    InProgressCounter = orders.Count(o => DateTime.Now >= o.DateAdded && DateTime.Now <= o.DateEnded);
            //    ExpiredOrderCounter = orders.Count(o => o.DateEnded < DateTime.Now);
            //}
        }
    }
}
