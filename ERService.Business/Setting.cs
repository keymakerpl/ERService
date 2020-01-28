using ERService.Infrastructure.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERService.Business
{
    public class Setting :  ISetting, IVersionedRow
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public string ValueType { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        [ConcurrencyCheck]
        public long RowVersion { get; set; }
    }
}
