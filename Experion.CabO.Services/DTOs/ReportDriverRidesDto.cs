using System.Collections.Generic;

namespace Experion.CabO.Services.DTOs
{
    public class ReportDriverRidesDto
    {
        public string driverName { get; set; }
        public double Distance { get; set; }
        public int RideCount { get; set; }
        public ICollection<DriverRides> DriverRides { get; set; }
    }
}
