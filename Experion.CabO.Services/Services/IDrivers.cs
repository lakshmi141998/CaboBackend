using Experion.CabO.Data.Entities;
using Experion.CabO.Services.DTOs;
using System.Collections.Generic;

namespace Experion.CabO.Services.Services
{
   public  interface IDrivers
    {
        string  PostDriverDetails(DriverDetails dtls);
        IEnumerable<Driver> GetDriverDetails();
        Driver GetDriver(int id);
        string UpdateDriverDetails(int id, DriverDetails chng);
        string DeleteDriver(int id);
    }
}
