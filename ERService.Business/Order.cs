using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERService.Business
{
    public class Order
    {        
        public Order()
        {
            Init();
        }

        private void Init()
        {
            Hardwares = new Collection<Hardware>();
            Attachments = new Collection<Blob>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int OrderId { get; set; }

        public Customer Customer { get; set; }

        [StringLength(50)]
        public string Number { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateAdded { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateEnded { get; set; }

        public Guid OrderStatusId { get; set; }
        public OrderStatus OrderStatus { get; set; }

        public Guid OrderTypeId { get; set; }
        public OrderType OrderType { get; set; }

        [StringLength(50)]
        public string Cost { get; set; }

        [StringLength(1000)]
        public string Fault { get; set; }

        [StringLength(1000)]
        public string Solution { get; set; }

        [StringLength(1000)]
        public string Comment { get; set; }

        [StringLength(50)]
        public string ExternalNumber { get; set; }
        
        public int Progress { get; set; }

        public ICollection<Hardware> Hardwares { get; set; }

        public ICollection<Blob> Attachments { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
