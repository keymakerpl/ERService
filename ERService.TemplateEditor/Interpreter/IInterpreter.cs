using System.Collections.Generic;

namespace ERService.TemplateEditor.Interpreter
{
    public interface IInterpreter
    {
        IContext Context { set; }
        object[] Wrappers { set; }
        Expression Expression { set; }
        IContext GetInterpretedContext();
        IEnumerable<Index> Indexes { get; }
    }
}