using ERService.Infrastructure.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERService.Business
{
    public class HwCustomItem : IVersionedRow
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public Guid CustomItemId { get; set; }
        public CustomItem CustomItem { get; set; }

        [StringLength(200)]
        public string Value { get; set; }

        public Guid HardwareId { get; set; }
        public Hardware Hardware { get; set; }

        [ConcurrencyCheck]
        public long RowVersion { get; set; }
    }
}
