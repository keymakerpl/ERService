using System;

namespace ERService.TemplateEditor.Interpreter
{
    public abstract class Expression<TKey, TValue> : IExpression
        where TKey : class
        where TValue : class
    {
        public Expression(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public abstract void Interpret(IContext context);

        public abstract TKey Key { get; set; }
        public abstract TValue Value { get; set; }
    }

    public sealed class IndexExpression : Expression<string, object>
    {
        public IndexExpression(string key, object value) : base(key, value)
        {
        }

        public override void Interpret(IContext context)
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
                    case TypeCode.Object:
                        if (Value is byte[])
                        {
                            outputValue = GetImageInBase64(Value as byte[]);
                        }
                        break;
                }
            }

            context.Output = context.Output.Replace(key, outputValue);
        }

        private string GetImageInBase64(byte[] imageBytes)
        {
            var base64 = Convert.ToBase64String(imageBytes);

            var tx = string.Format("<img src=\"data:image/gif;base64,{0}\" alt=\"logo\" width=\"320\" height=\"240\" />",
                    base64);

            return tx;
        }

        public override string Key { get; set; }
        public override object Value { get; set; }
    }
}
