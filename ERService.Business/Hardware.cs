using ERService.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERService.Business
{
    public class Hardware : IVersionedRow, IModificationHistory
    {
        public Hardware()
        {
            Initialize();
        }

        private void Initialize()
        {
            HardwareCustomItems = new Collection<HwCustomItem>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(80)]
        public string Name { get; set; }

        [Required]
        [StringLength(80)]
        public string SerialNumber { get; set; }
        
        public Guid? HardwareTypeID { get; set; }
        public HardwareType HardwareType { get; set; }

        public ICollection<HwCustomItem> HardwareCustomItems { get; set; }

        [ConcurrencyCheck]
        public long RowVersion { get; set; }

        [Column(TypeName = "DateTime")]
        public DateTime DateAdded { get; set; }

        [Column(TypeName = "DateTime")]
        public DateTime? DateModified { get; set; }        
    }
}
