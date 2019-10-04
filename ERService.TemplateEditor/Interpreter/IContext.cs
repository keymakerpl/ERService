namespace ERService.TemplateEditor.Interpreter
{
    public interface IContext
    {
        string Input { get; set; }
        string Output { get; set; }
    }
}