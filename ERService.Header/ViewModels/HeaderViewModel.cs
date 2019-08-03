using ERService.OrderModule.Repository;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Reflection;
using System.Linq;

namespace ERService.Header.ViewModels
{
    public class HeaderViewModel : BindableBase
    {
        private int _inProgressCounter;

        private string _currentUserFullName;
        public string CurrentUserFullName
        {
            get { return "Radosław Kurek"; }
        }

        public int InProgressCounter
        {
            get { return _inProgressCounter; }
            set { SetProperty(ref _inProgressCounter, value); }
        }

        private int _expiredOrderCounter;
        private IOrderRepository _orderRepository;

        public int ExpiredOrderCounter
        {
            get { return _expiredOrderCounter; }
            set { SetProperty(ref _expiredOrderCounter, value); }
        }        

        public HeaderViewModel(IOrderRepository orderRepository, IEventAggregator eventAggregator)
        {
            _orderRepository = orderRepository;            

            RefreshCounters();
        }

        public async void RefreshCounters()
        {
            var orders = await _orderRepository.GetAllAsync();
            if (orders != null)
            {
                InProgressCounter = orders.Count(o => DateTime.Now >= o.DateAdded && DateTime.Now <= o.DateEnded);
                ExpiredOrderCounter = orders.Count(o => o.DateEnded < DateTime.Now);
            }
        }
    }
}
