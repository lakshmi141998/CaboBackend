using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Experion.CabO.Data.Entities
{
    public class Cab
    {
        [Key]
        public int Id { get; set; }
        public string Model { get; set; }
        public int Capacity { get; set; }
        public string VehicleNo { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<RideAssignment> RideAssignment { get; set; }
        public virtual ICollection<OfficeCommutation> OfficeCommutation { get; set; }
    }
}
