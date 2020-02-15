using ERService.Business;
using ERService.Infrastructure.Events;
using ERService.Infrastructure.Repositories;
using ERService.OrderModule.Repository;
using ERService.Services.Tasks;
using Prism.Events;
using System;
using System.Threading.Tasks;

namespace ERService.OrderModule.Tasks
{
    public class NewOrdersNotificationTask : ITaskRunnable
    {
        private readonly IEventAggregator _eventAggregator;        
        private readonly IOrderRepository _orderRepository;

        private static DateTime? LastUpdateTime { get; set; } = DateTime.Now;

        public NewOrdersNotificationTask(IEventAggregator eventAggregator, IOrderRepository orderRepository)
        {
            _eventAggregator = eventAggregator;            
            _orderRepository = orderRepository;

            if(!LastUpdateTime.HasValue)
                LastUpdateTime = DateTime.Now;
        }

        public async Task Run()
        {
            var query = new QueryBuilder<Order>();
            query.Where(nameof(Order.DateAdded), QueryBuilder<Order>.Operators.GreaterOrEqual, LastUpdateTime.Value);

            var ids = await _orderRepository.GetIDsBy(query);

            if (ids.Length > 0)
            {
                RaiseNewOrdersAdded(ids);
                LastUpdateTime = DateTime.Now;
            }            
        }

        private void RaiseNewOrdersAdded(Guid[] ids)
        {
            _eventAggregator.GetEvent<AfterNewOrdersAddedEvent>().Publish(new AfterNewOrdersAddedEventArgs() { NewItemsIDs = ids });
        }
    }
}
