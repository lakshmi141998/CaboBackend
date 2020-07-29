using System;

namespace Experion.CabO.Services.DTOs
{
   public  class RideAssignmentDto
    {
        public DateTime DailyDate { get; set; }
        public int Days { get; set; }
        public string DriverId { get; set; }
        public string CabId { get; set; }
        public string ShiftId { get; set; }
        public bool IsDeleted { get; set; }
    }
    public class RideDisplayDto
    {
        public int RideId { get; set; }
        public DateTime Date { get; set; }
        public string DriverName { get; set; }
        public int DriverId { get; set; }
        public int CabId { get; set; }
        public int ShiftId { get; set; }
        public string CabName { get; set; }
        public string ShiftName { get; set; }
        public bool isDeleted { get; set; }
    }
}
