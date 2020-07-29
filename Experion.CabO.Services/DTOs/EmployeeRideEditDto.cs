namespace Experion.CabO.Services.DTOs
{
    public class EmployeeRideEditDto : EmployeeRideBaseDto
    {
        public string ProjectCode { get; set; }
        public string RideType { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}
