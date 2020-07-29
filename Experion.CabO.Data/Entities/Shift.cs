using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Experion.CabO.Data.Entities
{
    public class Shift
    {
        [Key]
        public int Id { get; set; }
        public string ShiftName { get; set; }
        public DateTime ShiftStart { get; set; }
        public DateTime ShiftEnd { get; set; }
        public virtual ICollection<RideAssignment> RideAssignment { get; set; }
    }
}
