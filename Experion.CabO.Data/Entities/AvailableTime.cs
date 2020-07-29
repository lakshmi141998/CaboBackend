using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Experion.CabO.Data.Entities
{
    public class AvailableTime
    {
        [Key]
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsDeleted { get; set; }
        
        [ForeignKey("OfficeCommutationId")]
        public virtual OfficeCommutation OfficeCommutation { get; set; }
        public int OfficeCommutationId { get; set; }
    }
}
