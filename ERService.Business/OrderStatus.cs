using ERService.Infrastructure.Interfaces;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERService.Business
{
    public class OrderStatus : IVersionedRow
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]        
        public StatusGroup Group { get; set; }

        [ConcurrencyCheck]
        public long RowVersion { get; set; }
    }
    
    //TODO: Multilanguage
    public enum StatusGroup
    {
        [Description("Otwarte")]
        Open,
        [Description("W trakcie")]
        InProgress,
        [Description("Zamknięte")]
        Finished
    }
}
