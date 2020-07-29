using Experion.CabO.Data.Entities;

namespace Experion.CabO.Services.DTOs
{
    public class cabdto
    {
        internal static Cab cab;
        public string Model { get; set; }
        public string Capacity { get; set; }
        public string VehicleNo { get; set; }
        public bool IsDeleted { get; set; }
    }
}
