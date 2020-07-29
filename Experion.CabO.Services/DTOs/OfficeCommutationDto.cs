using System;
using System.Collections.Generic;

namespace Experion.CabO.Services.DTOs
{
   public  class OfficeCommutationDto
    {
        public string Location1 { get; set; }
        public string Location2 { get; set; }
        public string CabId { get; set; }
        public bool IsDeleted { get; set; }
    }
    public class OfficeDisplayDto
    {
        public int Id { get; set; }
        public string Location1 { get; set; }
        public string Location2 { get; set; }
        public int Location1Id { get; set; }
        public int Location2Id { get; set; }

        public int CabId { get; set; }
        public string CabName { get; set; }
        public IEnumerable<AvailableTimeDto> Times { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
