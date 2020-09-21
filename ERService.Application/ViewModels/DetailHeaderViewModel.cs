using System;
using ERService.Infrastructure.Events;
using Prism.Events;
using Prism.Mvvm;

namespace ERService.ViewModels
{
    public class DetailHeaderViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private string _detailTitle;
        private bool _isCollapsed = true;

        public DetailHeaderViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<AfterUserLoggedoutEvent>().Subscribe(OnUserLogedout);
            _eventAggregator.GetEvent<AfterDetailOpenedEvent>().Subscribe(OnDetailOpened);
            _eventAggregator.GetEvent<AfterDetailClosedEvent>().Subscribe(OnDetailClosed);
        }

        private void OnUserLogedout(UserAuthorizationEventArgs args)
        {
            Hide();
        }

        private void OnDetailOpened(AfterDetailOpenedEventArgs args)
        {
            IsCollapsed = false;
            DetailTitle = args.DisplayableName;
        }

        private void OnDetailClosed(AfterDetailClosedEventArgs args)
        {
            Hide();
        }

        private void Hide()
        {
            IsCollapsed = true;
            DetailTitle = String.Empty;
        }        

        public string DetailTitle
        {
            get { return _detailTitle; }
            set { SetProperty(ref _detailTitle, value); }
        }

        public bool IsCollapsed
        {
            get { return _isCollapsed; }
            set { SetProperty(ref _isCollapsed, value); }
        }
    }
}