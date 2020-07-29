using Experion.CabO.Services.DTOs;
using System;
using System.Collections.Generic;

namespace Experion.CabO.Services.Services
{
    public interface IReportService
    {
        ICollection<CabListDto> GetCabList();
        ICollection<DriverDto> GetDriverList();
        ICollection<ProjectDto> GetProjectsList();
        ReportCabRidesDto GetCabReportStatic(int cabId, DateTime start, DateTime end);
        ReportDriverRidesDto GetDriverReport(int driverId, DateTime start, DateTime end);
        ReportProjectRidesDto GetProjectReport(string projectCode, DateTime start, DateTime end);
        ICollection<ReportRidesDto> GetRideReport(string rideType,string rideStatus, DateTime start, DateTime end);
        RatingDto GetDriverRating(int driverId);
    }
}
