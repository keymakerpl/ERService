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
            _eventAggregator.GetEvent<AfterDetailOpenedEvent>().Subscribe(OnDetailOpened);
            _eventAggregator.GetEvent<AfterDetailClosedEvent>().Subscribe(OnDetailClosed);
        }

        private void OnDetailClosed(AfterDetailClosedEventArgs args)
        {
            IsCollapsed = true;
            DetailTitle = String.Empty;
        }

        private void OnDetailOpened(AfterDetailOpenedEventArgs args)
        {
            IsCollapsed = false;
            DetailTitle = args.DisplayableName;
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