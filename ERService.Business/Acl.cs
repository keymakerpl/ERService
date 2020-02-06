using ERService.Infrastructure.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERService.Business
{
    public class Acl : IVersionedRow
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }        
        
        [Required]
        public Guid AclVerbId { get; set; }

        public AclVerb AclVerb { get; set; }

        public Guid RoleId { get; set; }

        public int Value { get; set; }

        [ConcurrencyCheck]
        public long RowVersion { get; set; }
    }
}
