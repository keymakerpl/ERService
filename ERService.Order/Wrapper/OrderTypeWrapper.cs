using ERService.Business;
using ERService.Infrastructure.Wrapper;

namespace ERService.OrderModule.Wrapper
{
    public class OrderTypeWrapper : ModelWrapper<OrderType>
    {
        public OrderTypeWrapper(OrderType model) : base(model)
        {
        }

        private string _name;
        public string Name
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _name, value); }
        }
    }
}
