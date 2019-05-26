using System;
using System.Collections;
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
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public Customer Customer { get; set; }

        [StringLength(50)]
        public string Number { get; set; }
        
        public DateTime DateAdded { get; set; }
        
        public DateTime DateEnded { get; set; }

        public OrderStatus OrderStatus { get; set; }

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

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
