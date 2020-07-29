using Experion.CabO.Services.DTOs;
using System.Collections.Generic;

namespace Experion.CabO.Services.Services
{
    public interface IRideAssignmentService
    {
        int AddRide(RideAssignmentDto rideDto);
        IEnumerable<RideDisplayDto> ViewAssignment();
        int UpdateRide(int id, RideAssignmentDto updateRide);
        int DeleteRide(int id);
        IEnumerable<CabDetailDto> GetCabs();
        IEnumerable<DriverDto> GetDrivers();
        IEnumerable<ShiftDto> GetShifts();
    }
}
