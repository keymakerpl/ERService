using System.Collections.Generic;

namespace ERService.Infrastructure.PrintTemplateEditor.Interpreter
{
    public static class IndexCollection
    {
        static IEnumerable<Index> _indexList;

        static IndexCollection()
        {
            _indexList = new List<Index>
                                { new Index { Name = "Imię klienta", Pattern = "[%o_Cus_FirstName%]" },
                                new Index { Name = "Nazwisko klienta", Pattern = "[%o_Cus_LastName%]" },
                                new Index { Name = "Status naprawy", Pattern = "[%o_Status%]" },
                                new Index { Name = "Typ naprawy", Pattern = "[%o_Type%]" },
                                new Index { Name = "Numer naprawy", Pattern = "[%o_number%]" },
                                new Index { Name = "Postęp naprawy", Pattern = "[%o_progress%]" },
                                new Index { Name = "Numer zewnętrzny", Pattern = "[%o_ext_number%]" },
                                new Index { Name = "Koszt naprawy", Pattern = "[%o_cost%]" },
                                new Index { Name = "Usterka", Pattern = "[%o_failure%]" },
                                new Index { Name = "Opis naprawy", Pattern = "[%o_solution%]" },
                                new Index { Name = "Data dodania", Pattern = "[%o_DateAdded%]" },
                                new Index { Name = "Data zakończenia", Pattern = "[%o_DateEnded%]" }
            };
        }

        public static IEnumerable<Index> IndexList { get { return _indexList; } }
    }

    public class Index
    {
        public string Name { get; set; }

        public string Pattern { get; set; }

        public object Value { get; set; }
    }
}
