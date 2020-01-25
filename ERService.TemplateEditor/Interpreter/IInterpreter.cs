using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERService.TemplateEditor.Interpreter
{
    public interface IInterpreter
    {
        IContext Context { set; }
        object[] Wrappers { set; }
        Expression Expression { set; }
        IContext GetInterpretedContext();
        Task<IEnumerable<Index>> GetIndexesAsync();
    }
}