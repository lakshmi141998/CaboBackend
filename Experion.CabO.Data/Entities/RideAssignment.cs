using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Experion.CabO.Data.Entities
{
    public class RideAssignment
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("CabId")]
        public virtual Cab Cab { get; set; }
        public int CabId { get; set; }

        [ForeignKey("DriverId")]
        public virtual Driver Driver { get; set; }
        public int DriverId { get; set; }
        public DateTime DailyDate { get; set; }
        public int? InitialReading { get; set; }
        public int? FinalReading { get; set; }
        [ForeignKey("ShiftId")]
        public virtual Shift Shift { get; set; }
        public int ShiftId { get; set; }
        public bool? IsDeleted { get; set; }
        public virtual ICollection<Ride> Ride { get; set; }

    }
}
