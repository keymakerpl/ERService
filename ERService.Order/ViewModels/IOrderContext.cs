using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ERService.Business;
using ERService.CustomerModule.Wrapper;
using ERService.HardwareModule;
using ERService.HardwareModule.ViewModels;
using ERService.OrderModule.Wrapper;

namespace ERService.OrderModule.ViewModels
{
    public interface IOrderContext
    {
        CustomerWrapper Customer { get; set; }
        CustomerAddress CustomerAddress { get; }
        HardwareWrapper Hardware { get; set; }
        OrderWrapper Order { get; set; }

        ObservableCollection<Blob> Attachments { get; }
        ObservableCollection<Customer> Customers { get; }
        ObservableCollection<DisblayableCustomItem> DisplayableCustomItems { get; }
        ObservableCollection<HwCustomItem> HardwareCustomItems { get; }
        ObservableCollection<HardwareType> HardwareTypes { get; }
        ObservableCollection<OrderStatus> OrderStatuses { get; }
        ObservableCollection<OrderType> OrderTypes { get; }

        Customer SelectedCustomer { get; set; }
        HardwareType SelectedHardwareType { get; set; }
        OrderStatus SelectedOrderStatus { get; set; }
        OrderType SelectedOrderType { get; set; }
        Blob SelectedAttachment { get; set; }

        bool HasErrors { get; }
        bool HasChanges { get; }

        void InitializeAddress(CustomerAddress customerAddress);
        void InitializeCustomer(Customer customer);
        Task InitializeHardware(Hardware hardware);
        Task InitializeOrder(Order order);
        Task Save();
    }
}