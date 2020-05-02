namespace ERService.TemplateEditor.Interpreter
{
    public interface IExpression
    {
        void Interpret(IContext context);
    }
}