using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Experion.CabO.Data.Entities
{
    public class OfficeCommutation
    {
        [Key]
        public int Id { get; set; }
        public int Location1 { get; set; }
        public int Location2 { get; set; }

        [ForeignKey("CabId")]
        public virtual Cab Cab { get; set; }
        public int CabId { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<AvailableTime> AvailableTime { get; set; }
    }
}
