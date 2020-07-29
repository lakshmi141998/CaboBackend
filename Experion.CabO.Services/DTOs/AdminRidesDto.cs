using System;
using System.Collections.Generic;

namespace Experion.CabO.Services.DTOs
{
    public class AdminRidesDto
    {
        public DateTime RideDate { get; set; }
        public string RideYear { get; set; }
        public string RideMonth { get; set; }
        public string RideDay { get; set; }
        public ICollection<RidesDto> Rides { get; set; }
    }
}
