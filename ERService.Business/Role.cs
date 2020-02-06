using ERService.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERService.Business
{
    public class Role : IVersionedRow
    {
        public Role()
        {
            Init();
        }

        private void Init()
        {
            ACLs = new Collection<Acl>();
            Users = new Collection<User>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public bool IsSystem { get; set; }

        public ICollection<Acl> ACLs { get; set; }

        public ICollection<User> Users { get; set; }

        [ConcurrencyCheck]
        public long RowVersion { get; set; }
    }
}
