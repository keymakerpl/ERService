﻿using ERService.Business;
using ERService.Infrastructure.Dialogs;
using ERService.Infrastructure.Events;
using ERService.Infrastructure.Notifications.ToastNotifications;
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
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator _eventAggregator;
        private readonly IOrderRepository _orderRepository;
        private readonly IMessageDialogService _messageService;

        private static DateTime? LastUpdateTime { get; set; } = DateTime.Now;

        public NewOrdersNotificationTask(IEventAggregator eventAggregator, IOrderRepository orderRepository, IMessageDialogService messageService)
        {
            _eventAggregator = eventAggregator;            
            _orderRepository = orderRepository;
            _messageService = messageService;

            if (!LastUpdateTime.HasValue)
                LastUpdateTime = DateTime.Now;           
        }

        public async Task Run()
        {
            var query = new QueryBuilder<Order>();
            query.Where(nameof(Order.DateAdded), SQLOperators.GreaterOrEqual, LastUpdateTime.Value);

            _logger.Debug($"LastUpdateTime: {LastUpdateTime}");

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
            _messageService.ShowOverTaskBar("ERService", $"W systemie pojawiły się nowe zgłoszenia: {ids.Length}", NotificationTypes.Information);
        }
    }
}
