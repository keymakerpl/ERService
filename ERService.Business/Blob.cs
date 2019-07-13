using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERService.Business
{
    public class Blob
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(300)]
        public string FileName { get; set; }

        public string Description { get; set; }

        public string Checksum { get; set; }

        public int Size { get; set; }

        public Guid? OrderId { get; set; }
        public Order Order { get; set; }

        [Required]
        public byte[] Data { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
