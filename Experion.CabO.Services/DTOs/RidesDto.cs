using System;

namespace Experion.CabO.Services.DTOs
{
    public class RidesDto
    {
        public int Id { get; set; }
        public string RideYear { get; set; }
        public string RideMonth { get; set; }
        public string RideDay { get; set; }
        public int RideHour { get; set; }
        public int RideMinutes { get; set; }
        public string TimePeriod { get; set; }
        public int RideRequestorId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string ContactNo { get; set; }
        public int PassengerCount { get; set; }
        public string Purpose { get; set; }
        public string ProjectCode { get; set; }
        public string RideStatusName { get; set; }
        public string Designation { get; set; }
        public string ReportingManager { get; set; }
        public int RequestCount { get; set; }
        public DateTime RideDate { get; set; }
        public DateTime RideTime { get; set; }
        public bool OnTime { get; set; }
        public long InitialReading { get; set; }
        public long FinalReading { get; set; }
        public string Guid { get; set; }
        public string RideType { get; set; }
    }
}
