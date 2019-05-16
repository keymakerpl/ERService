using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERService.Business
{
    public class CustomerAddress
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }
        
        public string Street { get; set; }

        public string City { get; set; }

        public string Postcode { get; set; }

        public Customer Customer { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
