using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERService.Business
{
    public class CustomItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Key { get; set; }

        public HardwareType HardwareType { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
