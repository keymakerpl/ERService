﻿using ERService.Infrastructure.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERService.Business
{
    public class OrderType : IVersionedRow
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [ConcurrencyCheck]
        public long RowVersion { get; set; }
    }
}
