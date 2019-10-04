namespace ERService.TemplateEditor.Interpreter
{
    public interface IExpression<TKey, TValue>
        where TKey : class
        where TValue : class
    {
        TKey Key { get; set; }
        TValue Value { get; set; }

        void Interpret(IContext context);
    }
}