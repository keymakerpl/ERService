using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERService.Business
{
    public class Acl
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }        
        
        [Required]
        public Guid AclVerbId { get; set; }

        public AclVerb AclVerb { get; set; }

        public Guid RoleId { get; set; }

        public int Value { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
