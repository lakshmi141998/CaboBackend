using Experion.CabO.Data.Entities;
using Experion.CabO.Services.DTOs;
using System.Collections.Generic;

namespace Experion.CabO.Services.Services
{
    public interface ICabService
    {
        int AddCab(cabdto cabdetails);
        IEnumerable<Cab> GetCabs();
        int UpdateCab(int id, cabdto cabdetails);
        int DeleteCab(int id);
        cabdto GetCabById(int id);
    }
}
