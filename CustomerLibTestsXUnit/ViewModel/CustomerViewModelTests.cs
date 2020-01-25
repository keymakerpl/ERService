using ERService.Business;
using ERService.CustomerModule.Repository;
using ERService.CustomerModule.ViewModels;
using ERService.CustomerModule.Wrapper;
using ERService.Infrastructure.Dialogs;
using ERService.Infrastructure.Events;
using ERService.RBAC;
using Moq;
using Prism.Events;
using Prism.Regions;
using System;
using System.Threading.Tasks;
using Unity;
using Xunit;
using CustomerLibTestsXUnit.Extensions;

namespace CustomerLibTestsXUnit
{
    public class CustomerViewModelTests
    {
        private CustomerViewModel _customerViewModel;

        public CustomerViewModelTests()
        {
            var container = new UnityContainer();

            //var customerMock = new Mock<Customer>(); //TODO: hmmmm?
            var customerRepositoryMock = new Mock<ICustomerRepository>();
            customerRepositoryMock.Setup(c => c.GetByIdAsync(It.IsAny<Guid>()))
                .Returns<Guid>(id => Task.FromResult(new Customer { Id = id, FirstName = "Radek", LastName = "Kurek" }));

            var regionManagerMock = new Mock<IRegionManager>();

            var detailSavedEventMock = new Mock<AfterDetailSavedEvent>();
            var eventAggregatorMock = new Mock<IEventAggregator>();
            eventAggregatorMock.Setup(ea => ea.GetEvent<AfterDetailSavedEvent>())
                .Returns(detailSavedEventMock.Object);

            var detailDeletedEventMock = new Mock<AfterDetailDeletedEvent>();
            eventAggregatorMock.Setup(ea => ea.GetEvent<AfterDetailDeletedEvent>())
                .Returns(detailDeletedEventMock.Object);

            var detailClosedEventMock = new Mock<AfterDetailClosedEvent>();
            eventAggregatorMock.Setup(ea => ea.GetEvent<AfterDetailClosedEvent>())
                .Returns(detailClosedEventMock.Object);

            var messageDialogServiceMock = new Mock<IMessageDialogService>();

            var rbacManagerMock = new Mock<IRBACManager>();

            _customerViewModel = new CustomerViewModel(customerRepositoryMock.Object, regionManagerMock.Object, eventAggregatorMock.Object, 
                messageDialogServiceMock.Object, rbacManagerMock.Object);
        }

        [Theory]
        [InlineData("7AAAAE3E-6028-452C-AF23-1A1A97B3371A")]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public async void LoadAsync_ShouldGetCustomerAndInitializeAfter(Guid id)
        {
            await _customerViewModel.LoadAsync(id);

            Assert.NotNull(_customerViewModel.Customer);
        }

        [Fact]
        public void ShouldRaisePropertyChengedEventForCustomer()
        {
            var customerWrapperMock = new Mock<ICustomerWrapper>();
            var fired = _customerViewModel.IsPropertyChangedFired(() =>
            {
                _customerViewModel.Customer = customerWrapperMock.Object;
            }, nameof(_customerViewModel.Customer));

            Assert.True(fired);
        }
    }
}
