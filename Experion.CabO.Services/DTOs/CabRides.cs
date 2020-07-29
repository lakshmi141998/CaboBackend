using System;

namespace Experion.CabO.Services.DTOs
{
    public class CabRides
    {
        public string From { get; set; }
        public string To { get; set; }
        public string RequestedBy { get; set; }
        public DateTime DateT { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Project { get; set; }
        public string Driver { get; set; }
        public int PassengerCount { get; set; }
        public string Purpose { get; set; }
        public long? Kilometer { get; set; }
    }
}
