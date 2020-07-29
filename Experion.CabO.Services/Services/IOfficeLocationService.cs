using Experion.CabO.Data.Entities;
using Experion.CabO.Services.DTOs;
using System.Collections.Generic;

namespace Experion.CabO.Services.Services
{
    public interface IOfficeLocationService 
    {
        int AddLocation(OfficeLocationDto location);
        IEnumerable<OfficeLocation> GetLocations();
        int DeleteLocation(int id);
    }
}
