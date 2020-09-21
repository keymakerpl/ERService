using ERService.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERService.Business
{
    public class Order : IVersionedRow, IModificationHistory
    {        
        public Order()
        {
            Initialize();
        }

        private void Initialize()
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

        public Guid? CustomerId { get; set; }
        public Customer Customer { get; set; }

        [StringLength(50)]
        public string Number { get; set; }

        [NotMapped]
        public string OrderNumber { get { return $"{OrderId}/{Number}"; } }

        [Column(TypeName = "DateTime")]
        public DateTime DateRegistered { get; set; }

        [Column(TypeName = "DateTime")]
        public DateTime? DateEnded { get; set; }

        public Guid? OrderStatusId { get; set; }
        public OrderStatus OrderStatus { get; set; }

        public Guid? OrderTypeId { get; set; }
        public OrderType OrderType { get; set; }

        public Guid? UserId { get; set; }
        public User User { get; set; }

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

        [ConcurrencyCheck]
        public long RowVersion { get; set; }

        [Column(TypeName = "DateTime")]
        public DateTime DateAdded { get; set; }

        [Column(TypeName = "DateTime")]
        public DateTime? DateModified { get; set; }        
    }
}
