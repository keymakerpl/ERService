using System;
using System.ComponentModel.DataAnnotations;

namespace ERService.Business
{
    public class CustomerAddress
    {
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
