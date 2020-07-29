using Experion.CabO.Data.Entities;
using Experion.CabO.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Experion.CabO.Services.Services
{
    public interface IRideListingService
    {
        IEnumerable<RidesDto> GetRides(string status);
        IEnumerable<RidesDto> GetLatestRide();
        IEnumerable<RideStatus> GetOptions();
        Task<ICollection<TTSDetailById>> ViewUserDetails(int id);
        ICollection<RidesDto> GetRidesDateWise(DateTime start_date, DateTime end_date, string status);
    }
}
