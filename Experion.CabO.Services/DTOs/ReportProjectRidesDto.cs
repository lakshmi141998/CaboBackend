using System.Collections.Generic;

namespace Experion.CabO.Services.DTOs
{
    public class ReportProjectRidesDto
    {
        public int RideCount { get; set; }
        public ICollection<ProjectRides> ProjectRides { get; set; }
    }
}
