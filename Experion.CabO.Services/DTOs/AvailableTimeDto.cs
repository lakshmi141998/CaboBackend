using System;

namespace Experion.CabO.Services.DTOs
{
    public class AvailableTimeDto
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsDeleted { get; set; }
        public int OfficeCommutationId { get; set; }
    }
}
