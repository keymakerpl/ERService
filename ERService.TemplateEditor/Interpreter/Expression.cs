using System;

namespace ERService.TemplateEditor.Interpreter
{
    public abstract class Expression<TKey, TValue> : IExpression<TKey, TValue>
        where TKey : class
        where TValue : class
    {
        public Expression()
        {

        }

        public Expression(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public void Interpret(IContext context)
        {
            if (String.IsNullOrWhiteSpace(context.Input))
                return;

            var key = Key as string;

            if (key == null) return;

            var outputValue = "";
            if (Value != null)
            {
                var valueType = Value.GetType();
                switch (Type.GetTypeCode(valueType))
                {
                    case TypeCode.String:
                        outputValue = Value as string;
                        break;
                    case TypeCode.DateTime:
                        var dateTime = Value as DateTime?;
                        if (dateTime.HasValue) outputValue = dateTime.Value.ToString();
                        break;
                }
            }

            context.Output = context.Output.Replace(key, outputValue);
        }

        public abstract TKey Key { get; set; }
        public abstract TValue Value { get; set; }
    }

    public sealed class IndexExpression : Expression<string, object>
    {
        public IndexExpression()
        {
        }

        public IndexExpression(string key, object value) : base(key, value)
        {
        }

        public override string Key { get; set; }
        public override object Value { get; set; }
    }
}
