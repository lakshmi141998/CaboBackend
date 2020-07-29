using Experion.CabO.Services.DTOs;
using System;
using System.Collections.Generic;

namespace Experion.CabO.Services.Services
{
    public interface IDashboardService
    {
        ICollection<AdminRidesDto> GetAdminRides(DateTime start, DateTime end);
        IEnumerable<EmployeeRideDto> GetRide(int userId);
        RidesDto GetRideById(string guid);
    }
}
