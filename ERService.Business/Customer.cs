using ERService.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERService.Business
{
    public class Customer : IVersionedRow
    {
        public Customer()
        {
            Init();
        }

        private void Init()
        {
            CustomerAddresses = new Collection<CustomerAddress>();
            Orders = new Collection<Order>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(50)]
        public string CompanyName { get; set; }

        [StringLength(50)]
        public string NIP { get; set; }

        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(50)]
        [EmailAddress]
        public string Email2 { get; set; }

        [Required]
        [Phone]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [Phone]
        [MaxLength(20)]
        public string PhoneNumber2 { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [ConcurrencyCheck]
        public long RowVersion { get; set; }

        #region Relacje

        public ICollection<CustomerAddress> CustomerAddresses { get; set; }
        public ICollection<Order> Orders { get; set; }

        #endregion

    }
}
