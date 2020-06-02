using ERService.Infrastructure.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERService.Business
{
    public class Blob 
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(300)]
        public string FileName { get; set; }

        public string Description { get; set; }

        public string Checksum { get; set; }

        public int Size { get; set; }

        [Required]
        public byte[] Data { get; set; }
    }
}
