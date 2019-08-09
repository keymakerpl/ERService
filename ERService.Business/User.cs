using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERService.Business
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Index(IsUnique = true)]
        public string Login { get; set; }
        
        public string PasswordHash { get; set; }

        public string Salt { get; set; }

        [MaxLength(50)]
        public string FirstName { get; set; }
        
        [MaxLength(50)]
        public string LastName { get; set; }
        
        [Phone]
        [MaxLength(50)]
        public string PhoneNumber { get; set; }

        public int IsActive { get; set; }

        public int IsAdmin { get; set; }

        public bool IsSystem { get; set; }

        public Guid? RoleId { get; set; }

        public Role Role { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
