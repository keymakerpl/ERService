using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERService.Business
{
    public class HwCustomItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public Guid CustomItemId { get; set; }

        [StringLength(200)]
        public string Value { get; set; }

        public Guid HardwareId { get; set; }

        public Hardware Hardware { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
