using ERService.Business;
using ERService.CustomerModule.Wrapper;
using ERService.HardwareModule;
using ERService.OrderModule.Wrapper;
using ERService.TemplateEditor.Interpreter;
using Moq;
using System;
using Unity;
using Xunit;

namespace TemplateEditorLibTestsXUnit.InterpreterTests
{
    public class InterpreterTests
    {
        private Interpreter _interpreter;
        private Mock<IContext> _contextMock;
        private Context _context2;

        public InterpreterTests()
        {
            var container = new UnityContainer();
            //container.RegisterType<IExpression<string, object>, IndexExpression>();

            _contextMock = new Mock<IContext>();
            _contextMock.Setup(o => o.Input).Returns("[%o_number%] [%c_firstName%] [%c_lastName%]");

            var context = new Context("[%o_number%] [%c_firstName%] [%c_lastName%] [%h_SerialNumber%]");

            var contextInput = "Numer zlecenia [%numer%], Data: [%data%] ";
            _context2 = new Context(contextInput);

            var orderModel = new CustomerWrapper(new Customer() { Id = Guid.NewGuid(), FirstName = "Jan", LastName = "Nowak" });
            var customerModel = new OrderWrapper(new Order() { Id = Guid.NewGuid(), Number = "01/092019" });
            var hardwareModel = new HardwareWrapper(new Hardware() { Id = Guid.NewGuid(), SerialNumber = "12332151HD" });

            //_interpreter = new Interpreter(context, Expression.IndexExpression, orderModel, customerModel, hardwareModel);
        }

        [Fact]
        public void InterpreterShouldChangeContext()
        {
            var output = _interpreter.GetInterpretedContext();

            Console.WriteLine(output.Output);

            Assert.NotNull(output.Input);
            Assert.NotNull(output.Output);
            Assert.NotEqual(output.Output, _contextMock.Object.Input);
        }

        [Theory]
        [InlineData("[%numer%]", "12345")]
        public void ShouldReplaceKeyToString(string key, string value)
        {
            var stringExpression = new IndexExpression(key, value);
            stringExpression.Interpret(_context2);
            var output = _context2.Output;

            Assert.Equal("Numer zlecenia 12345, Data: [%data%] ", output);
        }

        [Theory]
        [InlineData("[%data%]")]
        public void ShouldReplaceKeyToDateTimeString(string key)
        {
            var date = DateTime.Now;
            var dateTimeExpression = new IndexExpression(key, date);
            dateTimeExpression.Interpret(_context2);
            var output = _context2.Output;

            Assert.Equal($"Numer zlecenia [%numer%], Data: {date} ", output);
        }
    }
}
