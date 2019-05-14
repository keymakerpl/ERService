using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERService.Business
{
    public class Customer
    {
        public Customer()
        {
            Init();
        }

        private void Init()
        {
            CustomerAddresses = new Collection<CustomerAddress>();            
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }

        #region Relacje

        public ICollection<CustomerAddress> CustomerAddresses { get; set; }

        #endregion

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
