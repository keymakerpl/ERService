using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERService.TemplateEditor.Interpreter
{
    public interface IInterpreter
    {
        IContext Context { set; }
        object[] DataSource { set; }
        Expression[] Expressions { set; }
        IContext GetInterpretedContext();
        Task<IEnumerable<Index>> GetIndexesAsync();
    }
}