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
        public string Name
        {
            get { return GetValue<string>(); }
            set{ SetProperty(ref _name, value); }
        }

        private StatusGroup _group;
        public StatusGroup Group
        {
            get { return GetValue<StatusGroup>(); }
            set { SetProperty(ref _group, value); }
        }

        //TODO: multilanguage
        public string GroupDisplayableName
        {
            get
            {
                switch (Model.Group)
                {
                    case StatusGroup.Open:
                        return "Otwarte";

                    case StatusGroup.InProgress:
                        return "W trakcie";

                    case StatusGroup.Finished:
                        return "Zamknięte";

                    default:
                        return "Brak nazwy";
                }
            }
        }
    }
}
