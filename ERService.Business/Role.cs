using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERService.Business
{
    public class Role
    {
        public Role()
        {
            Init();
        }

        private void Init()
        {
            ACLs = new Collection<Acl>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public bool IsSystem { get; set; }

        public ICollection<Acl> ACLs { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
