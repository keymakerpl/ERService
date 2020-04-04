using ERService.Infrastructure.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERService.Business
{
    public class CustomItem : IVersionedRow
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Key { get; set; }
        
        public Guid HardwareTypeId { get; set; }
        public HardwareType HardwareType { get; set; }

        [ConcurrencyCheck]
        public long RowVersion { get; set; }
    }
}
