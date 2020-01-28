using ERService.Infrastructure.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERService.Business
{
    public class AclVerb : IVersionedRow
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public int DefaultValue { get; set; }
        
        [MaxLength(50)]
        public string Description { get; set; }

        [ConcurrencyCheck]
        public long RowVersion { get; set; }
    }
}
