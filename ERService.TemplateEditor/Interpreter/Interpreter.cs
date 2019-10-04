using ERService.Infrastructure.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ERService.TemplateEditor.Interpreter
{
    public class Interpreter : IInterpreter
    {
        private IContext _context;
        private object[] _modelWrappers;
        private IndexExpression _expression;

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

        public IEnumerable<Index> Indexes 
        { 
            get { return GetIndexesFromAssemblies(); } 
        }

        private IEnumerable<Index> GetIndexesFromAssemblies()
        {
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

                                yield return new Index() { Name = attribute.Name, Pattern = attribute.Pattern };
                            }
                        }
                    }
                }
            }

        }

        public IContext GetInterpretedContext()
        {
            foreach (var model in _modelWrappers)
            {
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