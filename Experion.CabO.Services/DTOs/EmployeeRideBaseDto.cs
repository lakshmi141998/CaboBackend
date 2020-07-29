using System;

namespace Experion.CabO.Services.DTOs
{
    public class EmployeeRideBaseDto
    {
        public DateTime RideDate { get; set; }
        public DateTime RideTime { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public int PassengerCount { get; set; }
        public string Purpose { get; set; }
        public string UserId { get; set; }
        public string ContactNo { get; set; }
    }
}
