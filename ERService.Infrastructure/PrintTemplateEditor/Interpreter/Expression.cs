using System;

namespace ERService.Infrastructure.HtmlEditor.Interpreter
{
    public abstract class Expression<TKey, TValue> 
        where TKey : class
        where TValue : class
    {
        public Expression(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public void Interpret(Context context)
        {
            if (String.IsNullOrWhiteSpace(context.Input))
                return;

            var key = Key as string;

            if (key == null)
                return;

            var value = "";
            var valueType = Value.GetType();
            switch (Type.GetTypeCode(valueType))
            {
                case TypeCode.String:
                    value = Value as string;
                    break;
                case TypeCode.DateTime:
                    var dateTime = Value as DateTime?;
                    if (dateTime.HasValue) value = dateTime.Value.ToString();
                    break;
                default:
                    break;
            }

            context.Output = context.Input.Replace(key, value ?? "");
        }

        public abstract TKey Key { get; set; }
        public abstract TValue Value { get; set; }
    }

    public sealed class StringExpression : Expression<string, string>
    {
        public StringExpression(string key, string value) : base(key, value)
        {
        }

        public override string Key { get; set; }
        public override string Value { get; set; }
    }

    public sealed class DateTimeExpression : Expression<string, object>
    {
        public DateTimeExpression(string key, object value) : base(key, value)
        {
        }

        public override string Key { get; set; }
        public override object Value { get; set; }
    }
}
