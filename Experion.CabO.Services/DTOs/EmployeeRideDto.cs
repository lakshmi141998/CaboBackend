namespace Experion.CabO.Services.DTOs
{
    public class EmployeeRideDto : EmployeeRideBaseDto
    {
            public int RideId { get; set; }
            public string RideStatus { get; set; }
            public string CancelReason { get; set; }        
            public string RideTypeName { get; set; }
            public string CabType { get; set; }
            public string ExternalCabName { get; set; }
    }
}
