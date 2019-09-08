using ERService.Infrastructure.HtmlEditor.Interpreter;
using System;
using Xunit;

namespace InfrastructureLibTestXUnit
{
    public class InterpreterTests
    {
        private Context _context;

        public InterpreterTests()
        {
            var contextInput = "Numer zlecenia [%numer%], Data: [%data%] ";

            _context = new Context(contextInput);
        }

        [Theory]
        [InlineData("[%numer%]", "12345")]
        public void ShouldReplaceKeyToString(string key, string value)
        {
            var stringExpression = new StringExpression(key, value);            
            stringExpression.Interpret(_context);
            var output = _context.Output;

            Assert.Equal("Numer zlecenia 12345, Data: [%data%] ", output);
        }

        [Theory]
        [InlineData("[%data%]")]
        public void ShouldReplaceKeyToDateTimeString(string key)
        {
            var date = DateTime.Now;
            var dateTimeExpression = new DateTimeExpression(key, date);
            dateTimeExpression.Interpret(_context);
            var output = _context.Output;

            Assert.Equal($"Numer zlecenia [%numer%], Data: {date} ", output);
        }
    }
}
