﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERService.Business
{
    public class HardwareType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
