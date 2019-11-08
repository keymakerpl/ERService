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
        private object[] _modelWrappers;
        private IndexExpression _expression;
        private readonly ISettingsManager _settingsManager;

        public IContext Context { set => _context = value; }
        public object[] Wrappers { set => _modelWrappers = value; }
        public Expression Expression 
        { 
            set 
            {
                switch (value)
                {
                    case Expression.IndexExpression:
                        _expression = new IndexExpression();
                        break;
                    default:
                        _expression = new IndexExpression();
                        break;
                }
            } 
        }

        public Interpreter(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        public async Task<IEnumerable<Index>> GetIndexesAsync()
        {
            var result = new Collection<Index>();
            var fromAssemblys = await GetIndexesFromAssemblies();

            result.AddRange(fromAssemblys); 

            return result;
        }

        private async Task<Collection<Index>> GetIndexesFromAssemblies()
        {
            var result = GetIndexes();

            return await Task.FromResult(result);
        }

        private Collection<Index> GetIndexes()
        {
            var collection = new Collection<Index>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
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
            foreach (var model in _modelWrappers)
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

                            _expression.Key = attribute.Pattern;
                            _expression.Value = prop.GetValue(model, null);
                            _expression.Interpret(_context);
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
}