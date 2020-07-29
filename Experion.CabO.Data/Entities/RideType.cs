using System.Collections.Generic;

namespace Experion.CabO.Data.Entities
{
    public class RideType
    {
        public int Id { get; set; }
        public string rideType { get; set; }
        public virtual ICollection<Ride> Ride { get; set; }
    }
}
