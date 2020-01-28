using ERService.Infrastructure.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERService.Business
{
    public class CustomerAddress : IVersionedRow
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }
        
        public string Street { get; set; }

        public string HouseNumber { get; set; }

        public string City { get; set; }

        public string Postcode { get; set; }

        public Customer Customer { get; set; }

        [ConcurrencyCheck]
        public long RowVersion { get; set; }
    }
}
