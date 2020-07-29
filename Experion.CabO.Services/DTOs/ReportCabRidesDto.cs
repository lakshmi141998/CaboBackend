using System.Collections.Generic;

namespace Experion.CabO.Services.DTOs
{
    public class ReportCabRidesDto
    {
        public string cabName { get; set; }
        public double Distance { get; set; }
        public int RideCount { get; set; }
        public ICollection<CabRides> CabRides { get; set; }
    }
}
