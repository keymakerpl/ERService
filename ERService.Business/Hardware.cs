using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERService.Business
{
    public class Hardware
    {
        public Hardware()
        {
            Init();
        }

        private void Init()
        {
            HardwareCustomItems = new Collection<HwCustomItem>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public ICollection<HwCustomItem> HardwareCustomItems { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
