using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Experion.CabO.Data.Entities
{
    public class RideStatus
    {
        [Key]
        public int Id { get; set; }
        public string StatusName { get; set; }
        public virtual ICollection<Ride> Ride { get; set; }
    }
}
