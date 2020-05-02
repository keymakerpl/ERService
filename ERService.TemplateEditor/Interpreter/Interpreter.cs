using ERService.Business;
using ERService.Infrastructure.Attributes;
using ERService.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ERService.TemplateEditor.Interpreter
{
    public class Interpreter : IInterpreter
    {
        private IContext _context;
        private object[] _dataSource;
        private Expression[] _expressions;
        private readonly ISettingsManager _settingsManager;

        public IContext Context { set => _context = value; }
        public object[] DataSource { set => _dataSource = value; }
        public Expression[] Expressions { set => _expressions = value; }

        public Interpreter(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        public async Task<IEnumerable<Index>> GetIndexesAsync()
        {
            var result = new Collection<Index>();
            var fromAssemblys = await GetIndexesFromAssemblies();

            foreach (var indx in fromAssemblys)
            {
                result.Add(indx);
            } 

            return result;
        }

        private async Task<Collection<Index>> GetIndexesFromAssemblies()
        {
            //var result = GetIndexes();
            var task = new Task<Collection<Index>>(() => GetIndexes());
            task.Start();

            return await task;
        }

        private Collection<Index> GetIndexes()
        {
            var collection = new Collection<Index>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if ((assembly.FullName.IndexOf("ERSERVICE", StringComparison.OrdinalIgnoreCase) == -1))
                    continue;

                foreach (var type in assembly.GetTypes())
                {
                    foreach (var prop in type.GetProperties())
                    {
                        var attributes = Attribute.GetCustomAttributes(prop, typeof(InterpreterAttribute), false).ToArray();
                        if (attributes.Any())
                        {
                            for (int i = 0; i < attributes.Length; i++)
                            {
                                var attribute = attributes[i] as InterpreterAttribute;
                                if (attribute == null) continue;

                                collection.Add(new Index() { Name = attribute.Name, Pattern = attribute.Pattern });
                            }
                        }
                    }
                }
            }
            return collection;
        }

        public IContext GetInterpretedContext()
        {
            foreach (var model in _dataSource)
            {
                if (model == null) continue;

                var properties = model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var prop in properties)
                {
                    var interpreterAttributes = Attribute.GetCustomAttributes(prop, typeof(InterpreterAttribute), false).ToArray();
                    if (interpreterAttributes.Any())
                    {
                        for (int i = 0; i < interpreterAttributes.Length; i++)
                        {
                            var attribute = interpreterAttributes[i] as InterpreterAttribute;
                            if (attribute == null) continue;

                            foreach (var expr in _expressions)
                            {
                                var expression = ExpressionFactory.GetExpression(
                                    expr,
                                    attribute.Pattern,
                                    prop.GetValue(model, null));                                

                                expression.Interpret(_context);
                            }
                        }
                    }
                }
            }
            return _context;
        }
    }

    public enum Expression
    {
        IndexExpression
    }

    public static class ExpressionFactory
    {
        public static IExpression GetExpression(Expression expression, string key, object value)
        {
            switch (expression)
            {
                default:
                    return new IndexExpression(key, value);
            }
        }
    }
}