using System;

namespace Experion.CabO.Services.DTOs
{
   public class DriverRidesDto
    {
        public int RideId { get; set; }
        public DateTime RideDate { get; set; }
        public DateTime RideTime{ get; set; }
        public string UserName { get; set; }
        public string From { get; set; }
        public  string To { get; set; }
        public string PhoneNo { get; set; }
        public string CabName { get; set; }
        public string VehicleNo { get; set; }
        public string RideStausName { get; set; }
        public string RideYear { get; set; }
        public string RideMonth { get; set; }
        public string RideDay { get; set; }
        public int RideHour { get; set; }
        public int RideMinutes { get; set; }
        public string TimePeriod { get; set; }
        public string ShiftName { get; set; }
        public bool OnTime { get; set; }
    }
}
