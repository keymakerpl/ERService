using ERService.Business;
using ERService.Infrastructure.Wrapper;

namespace ERService.OrderModule.Wrapper
{
    public class OrderStatusWrapper : ModelWrapper<OrderStatus>
    {
        public OrderStatusWrapper(OrderStatus model) : base(model)
        {
        }

        private string _name;
        public string Name { get { return GetValue<string>(); } set { SetProperty(ref _name, value); } }
    }
}
