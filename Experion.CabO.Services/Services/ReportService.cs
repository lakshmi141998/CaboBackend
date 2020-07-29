using System;
using System.Collections.Generic;
using Experion.CabO.Data.Entities;
using Experion.CabO.Services.DTOs;
using System.Linq;
using Experion.CabO.Data.Constant;
using Experion.CabO.Data.Enums;

namespace Experion.CabO.Services.Services
{
    public class ReportService : IReportService
    {
        private static CabODbContext cabODbContext;
        private TtsApiService apiService;
        public ReportService(CabODbContext _cabODbContext, TtsApiService _apiService)
        {
            cabODbContext = _cabODbContext;
            apiService = _apiService;
        }
        public ICollection<CabListDto> GetCabList()
        {
            try
            {
                return cabODbContext.Cab.Where(x => x.IsDeleted == false).Select(cab => new CabListDto
                {
                    cabId = cab.Id,
                    cabName = cab.Model
                }).ToList();
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public ICollection<ProjectDto> GetProjectsList()
        {
            try
            {
                var a= cabODbContext.Ride.Where(x => x.ProjectCode != "Nil" && x.ProjectCode != null && x.ProjectCode != "")
                    .Join(cabODbContext.RideStatus, r => r.RideStatusId, rs=> rs.Id, (r,rs)=> new { r.CabType, r.ProjectCode, rs.StatusName})
                    .Where(x=> (x.StatusName == RideStatuses.Completed.ToString())|| (x.StatusName == RideStatuses.Approved.ToString() && x.CabType == CabTypes.External.ToString()))
                    .Select(x => new ProjectDto
                {
                    ProjectCode = x.ProjectCode
                }).Distinct().ToList();
                return a;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ICollection<DriverDto> GetDriverList()
        {
            try
            {
                return cabODbContext.Driver.Where(x => x.IsDeleted == false).Select(driver => new DriverDto
                {
                    Id = driver.Id,
                    Name = driver.Name
                }).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ReportCabRidesDto GetCabReportStatic(int cabId, DateTime startDate, DateTime endDate)
        {
            double totalDistance = 0;
            int rideCount = 0;
            var cabNam = cabODbContext.Cab.Where(c => c.Id == cabId).Select(x => x.Model).FirstOrDefault();
            var cabReport = cabODbContext.Ride.Where(r => r.RideDate.Date >= startDate.Date && r.RideDate.Date <= endDate.Date)
            .Join(cabODbContext.RideAssignment, y => y.RideAssignmentId, l => l.Id, (y, l) => new { y, l.CabId, l.DriverId }).Where(x => x.CabId == cabId)
            .Join(cabODbContext.Cab, x => x.CabId, c => c.Id, (x, c) => new { x, c.Model })
            .Join(cabODbContext.Driver, ab => ab.x.DriverId, d => d.Id, (ab, d) => new { ab, d.Name })
            .Join(cabODbContext.RideRequestor, ay => ay.ab.x.y.RideRequestorId, f => f.Id, (ay, f) => new { ay, f.UserName })
            .Join(cabODbContext.RideStatus, by => by.ay.ab.x.y.RideStatusId, o => o.Id, (by, o) => new { by, o.StatusName }).Where(r => r.StatusName == RideStatuses.Completed.ToString())
            .Select(x => new CabRides
            {
                From = x.by.ay.ab.x.y.From,
                To = x.by.ay.ab.x.y.To,
                DateT = getIst(x.by.ay.ab.x.y.RideDate),
                Date = getIst(x.by.ay.ab.x.y.RideDate).ToString("dd-MMM-yyyy"),
                Time = getIst(x.by.ay.ab.x.y.RideTime).ToString("hh:mm tt"),
                RequestedBy = x.by.UserName,
                Project = x.by.ay.ab.x.y.ProjectCode,
                PassengerCount = x.by.ay.ab.x.y.PassengerCount,
                Purpose = x.by.ay.ab.x.y.Purpose,
                Driver = x.by.ay.Name,
                Kilometer = (x.by.ay.ab.x.y.FinalReading - x.by.ay.ab.x.y.InitialReading)
            }).ToList();

            foreach (var report in cabReport)
            {
                rideCount += 1;
                totalDistance += Convert.ToDouble(report.Kilometer);
            }
            return new ReportCabRidesDto
            {
                cabName = cabNam,
                Distance = totalDistance,
                RideCount = rideCount,
                CabRides = cabReport.OrderBy(x => x.DateT.Date).ThenBy(x => x.Time).ToList()
            };
        }
        public static ReportDriverRidesDto GetDriverReportStatic(int driverId, DateTime startDate, DateTime endDate)
        {
            int rideCount = 0;
            double totalDistance = 0;
            var driverNam = cabODbContext.Driver.Where(d => d.Id == driverId).Select(x => x.Name).FirstOrDefault();
            var driverReport = cabODbContext.Ride.Where(r => r.RideDate.Date >= startDate.Date && r.RideDate.Date <= endDate.Date)
           .Join(cabODbContext.RideAssignment, y => y.RideAssignmentId, l => l.Id, (y, l) => new { y, l.CabId, l.DriverId }).Where(x => x.DriverId == driverId)
           .Join(cabODbContext.Cab, x => x.CabId, c => c.Id, (x, c) => new { x, c.Model })
           .Join(cabODbContext.Driver, ab => ab.x.DriverId, d => d.Id, (ab, d) => new { ab, d.Name })
           .Join(cabODbContext.RideRequestor, ay => ay.ab.x.y.RideRequestorId, f => f.Id, (ay, f) => new { ay, f.UserName })
           .Join(cabODbContext.RideStatus, by => by.ay.ab.x.y.RideStatusId, o => o.Id, (by, o) => new { by, o.StatusName }).Where(r => r.StatusName == RideStatuses.Completed.ToString())
           .Select(x => new DriverRides
           {
               From = x.by.ay.ab.x.y.From,
               To = x.by.ay.ab.x.y.To,
               DateT = getIst(x.by.ay.ab.x.y.RideDate),
               Date = getIst(x.by.ay.ab.x.y.RideDate).ToString("dd-MMM-yyyy"),
               Time = getIst(x.by.ay.ab.x.y.RideTime).ToString("hh:mm tt"),
               RequestedBy = x.by.UserName,
               Project = x.by.ay.ab.x.y.ProjectCode,
               PassengerCount = x.by.ay.ab.x.y.PassengerCount,
               Purpose = x.by.ay.ab.x.y.Purpose,
               Cab = x.by.ay.ab.Model,
               Kilometer = (x.by.ay.ab.x.y.FinalReading - x.by.ay.ab.x.y.InitialReading)
           }).ToList();
            foreach (var report in driverReport)
            {
                rideCount += 1;
                totalDistance += Convert.ToDouble(report.Kilometer);
            }
            return new ReportDriverRidesDto
            {
                driverName = driverNam,
                Distance = totalDistance,
                RideCount = rideCount,
                DriverRides = driverReport.OrderBy(x => x.DateT.Date).ThenBy(x => x.Time).ToList()
            };

        }

        public static ReportProjectRidesDto GetProjectReportStatic(string projectCode, DateTime startDate, DateTime endDate)
        {
            int rideCount = 0;
            double totalDistance = 0;
            var projectReport = cabODbContext.Ride.Where(r => r.RideDate.Date >= startDate.Date && r.RideDate.Date <= endDate.Date && r.ProjectCode == projectCode)
           .Join(cabODbContext.RideAssignment, y => y.RideAssignmentId, l => l.Id, (y, l) => new { y, l.CabId, l.DriverId })
           .Join(cabODbContext.Cab, x => x.CabId, c => c.Id, (x, c) => new { x, c.Model })
           .Join(cabODbContext.Driver, ab => ab.x.DriverId, d => d.Id, (ab, d) => new { ab, d.Name })
           .Join(cabODbContext.RideRequestor, ay => ay.ab.x.y.RideRequestorId, f => f.Id, (ay, f) => new { ay, f.UserName })
           .Join(cabODbContext.RideStatus, by => by.ay.ab.x.y.RideStatusId, o => o.Id, (by, o) => new { by, o.StatusName }).Where(r => r.StatusName == RideStatuses.Completed.ToString())
           .Select(x => new ProjectRides
           {                             
               From = x.by.ay.ab.x.y.From,
               To = x.by.ay.ab.x.y.To,
               RequestedBy = x.by.UserName,
               DateT = getIst(x.by.ay.ab.x.y.RideDate),
               Date = getIst(x.by.ay.ab.x.y.RideDate).ToString("dd-MMM-yyyy"),
               Time = getIst(x.by.ay.ab.x.y.RideTime).ToString("hh:mm tt"),
               Driver = x.by.ay.Name,
               Cab = x.by.ay.ab.Model,
               PassengerCount = x.by.ay.ab.x.y.PassengerCount,
               Purpose = x.by.ay.ab.x.y.Purpose,
               Kilometer = (x.by.ay.ab.x.y.FinalReading - x.by.ay.ab.x.y.InitialReading)
           }).ToList();

            foreach (var report in projectReport)
            {
                rideCount += 1;
                totalDistance += Convert.ToDouble(report.Kilometer);
            }
            return new ReportProjectRidesDto
            {
                RideCount = rideCount,
                ProjectRides = projectReport.OrderBy(x => x.DateT.Date).ThenBy(x => x.Time).ToList()
            };
        }
        public static DateTime getIst(DateTime date)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(date, TimeZoneInfo.FindSystemTimeZoneById(Constant.TimeZone.India));
        }
        public static ICollection<ReportRidesDto> GetRideReportStatic(string rideType, string rideStatus, DateTime start, DateTime end)
        {
            try
            {
                if (rideType == CabTypes.External.ToString())
                {
                    var externalRides = cabODbContext.Ride.Where(r => r.CabType == CabTypes.External.ToString() && r.RideDate.Date >= start.Date && r.RideDate.Date <= end.Date)
                        .Join(cabODbContext.RideRequestor, r => r.RideRequestorId, rr=> rr.Id, (r,rr)=> new { r, rr.UserName})
                        .Select(x => new ReportRidesDto {
                        From = x.r.From,
                        To = x.r.To,
                        DateT = getIst(x.r.RideDate),
                        Date = getIst(x.r.RideDate).ToString("dd-MMM-yyyy"),
                        Time = getIst(x.r.RideTime).ToString("hh:mm tt"),
                        RequestedBy = x.UserName,
                        Driver = "External Cab",
                        ProjectCode = x.r.ProjectCode,
                        Cab = x.r.ExternalCabName,
                        Purpose = x.r.Purpose,
                    }).ToList();
                    return externalRides.OrderBy(x=> x.DateT.Date).ThenBy(x=> x.Time).ToList();
                }
                else if(rideType == RideTypes.RideTypeAdmin.ToString())
                {
                    var adminRides = cabODbContext.Ride.Where(x => x.RideDate.Date >= start.Date && x.RideDate.Date <= end.Date)
                           .Join(cabODbContext.RideStatus, r => r.RideStatusId, rs => rs.Id, (r, rs) => new { r, rs.StatusName }).Where(x =>( x.r.CabType == CabTypes.External.ToString() && x.StatusName == RideStatuses.Approved.ToString()) || x.StatusName == RideStatuses.Completed.ToString())
                           .Join(cabODbContext.RideType, r => r.r.RideTypeId, rt => rt.Id, (r, rt) => new { r, rt.rideType }).Where(x => x.rideType == rideType)
                           .Join(cabODbContext.RideRequestor, r => r.r.r.RideRequestorId, rr => rr.Id, (r, rr) => new { r, rr.UserName })
                           .GroupJoin(cabODbContext.RideAssignment, r => r.r.r.r.RideAssignmentId, ra => ra.Id, (r, ra) => new { r, ra })
                           .SelectMany(r => r.ra.DefaultIfEmpty(), (r, ra) => new { r.r, ra })
                           .GroupJoin(cabODbContext.Cab, rra => rra.ra.CabId, c => c.Id, (rra, c) => new { rra, c })
                           .SelectMany(ra => ra.c.DefaultIfEmpty(), (ra, c) => new { ra.rra, c.Model })
                           .GroupJoin(cabODbContext.Driver, rrc => rrc.rra.ra.DriverId, d => d.Id, (rrc, d) => new { rrc, d })
                           .SelectMany(ra => ra.d.DefaultIfEmpty(), (ra, d) => new { ra.rrc, d.Name })
                           .Select(x => new ReportRidesDto
                           {
                               From = x.rrc.rra.r.r.r.r.From,
                               To = x.rrc.rra.r.r.r.r.To,
                               DateT = getIst(x.rrc.rra.r.r.r.r.RideDate),
                               Date = getIst(x.rrc.rra.r.r.r.r.RideDate).ToString("dd-MMM-yyyy"),
                               Time = getIst(x.rrc.rra.r.r.r.r.RideTime).ToString("hh:mm tt"),
                               RequestedBy = x.rrc.rra.r.UserName,
                               Driver = x.Name,
                               ProjectCode = x.rrc.rra.r.r.r.r.ProjectCode,
                               Cab = x.rrc.Model,
                               Status = x.rrc.rra.r.r.r.StatusName,
                               Kilometer = (x.rrc.rra.r.r.r.r.FinalReading - x.rrc.rra.r.r.r.r.InitialReading),
                               Purpose = x.rrc.rra.r.r.r.r.Purpose
                           }).ToList();
                    return adminRides.OrderBy(x => x.DateT.Date).ThenBy(x => x.Time).ToList();
                }
                else
                {
                    if (rideStatus == "All")
                    {
                        var allRides= cabODbContext.Ride.Where(x => x.RideDate.Date >= start.Date && x.RideDate.Date <= end.Date)
                            .Join(cabODbContext.RideStatus, r => r.RideStatusId, rs => rs.Id, (r, rs) => new { r, rs.StatusName })
                            .Join(cabODbContext.RideType, r => r.r.RideTypeId, rt => rt.Id, (r, rt) => new { r, rt.rideType }).Where(x => x.rideType == rideType)
                            .Join(cabODbContext.RideRequestor, r => r.r.r.RideRequestorId, rr => rr.Id, (r, rr) => new { r, rr.UserName })
                            .GroupJoin(cabODbContext.RideAssignment, r => r.r.r.r.RideAssignmentId, ra => ra.Id, (r, ra) => new { r,ra })
                            .SelectMany(r => r.ra.DefaultIfEmpty(), (r, ra) => new { r.r, ra })
                            .GroupJoin(cabODbContext.Cab, rra=> rra.ra.CabId, c=> c.Id,(rra,c)=> new { rra, c})
                            .SelectMany(ra => ra.c.DefaultIfEmpty(), (ra, c) => new { ra.rra, c.Model })
                            .GroupJoin(cabODbContext.Driver, rrc=> rrc.rra.ra.DriverId, d=> d.Id, (rrc,d)=>new { rrc,d})
                            .SelectMany(ra => ra.d.DefaultIfEmpty(), (ra, d) => new { ra.rrc, d.Name })
                            .Select(x => new ReportRidesDto
                            {
                                From = x.rrc.rra.r.r.r.r.From,
                                To = x.rrc.rra.r.r.r.r.To,
                                DateT = getIst(x.rrc.rra.r.r.r.r.RideDate),
                                Date = getIst(x.rrc.rra.r.r.r.r.RideDate).ToString("dd-MMM-yyyy"),
                                Time = getIst(x.rrc.rra.r.r.r.r.RideTime).ToString("hh:mm tt"),
                                RequestedBy = x.rrc.rra.r.UserName,
                                Driver = x.Name,
                                ProjectCode = x.rrc.rra.r.r.r.r.ProjectCode,
                                Cab = x.rrc.Model,
                                Purpose = x.rrc.rra.r.r.r.r.Purpose,
                                Status = x.rrc.rra.r.r.r.StatusName,
                                Kilometer = (x.rrc.rra.r.r.r.r.FinalReading - x.rrc.rra.r.r.r.r.InitialReading)

                            }).ToList();
                        return allRides.OrderBy(x => x.DateT.Date).ThenBy(x => x.Time).ToList();

                    }
                    else
                    {
                        var rides = cabODbContext.Ride.Where(x => x.RideDate.Date >= start.Date && x.RideDate.Date <= end.Date)
                           .Join(cabODbContext.RideStatus, r => r.RideStatusId, rs => rs.Id, (r, rs) => new { r, rs.StatusName }).Where(x=>x.StatusName== rideStatus)
                           .Join(cabODbContext.RideType, r => r.r.RideTypeId, rt => rt.Id, (r, rt) => new { r, rt.rideType }).Where(x => x.rideType == rideType)
                           .Join(cabODbContext.RideRequestor, r => r.r.r.RideRequestorId, rr => rr.Id, (r, rr) => new { r, rr.UserName })
                           .GroupJoin(cabODbContext.RideAssignment, r => r.r.r.r.RideAssignmentId, ra => ra.Id, (r, ra) => new { r, ra })
                           .SelectMany(r => r.ra.DefaultIfEmpty(), (r, ra) => new { r.r, ra })
                           .GroupJoin(cabODbContext.Cab, rra => rra.ra.CabId, c => c.Id, (rra, c) => new { rra, c })
                           .SelectMany(ra => ra.c.DefaultIfEmpty(), (ra, c) => new { ra.rra, c.Model })
                           .GroupJoin(cabODbContext.Driver, rrc => rrc.rra.ra.DriverId, d => d.Id, (rrc, d) => new { rrc, d })
                           .SelectMany(ra => ra.d.DefaultIfEmpty(), (ra, d) => new { ra.rrc, d.Name })
                           .Select(x => new ReportRidesDto
                           {
                               From = x.rrc.rra.r.r.r.r.From,
                               To = x.rrc.rra.r.r.r.r.To,
                               DateT = getIst(x.rrc.rra.r.r.r.r.RideDate),
                               Date = getIst(x.rrc.rra.r.r.r.r.RideDate).ToString("dd-MMM-yyyy"),
                               Time = getIst(x.rrc.rra.r.r.r.r.RideTime).ToString("hh:mm tt"),
                               RequestedBy = x.rrc.rra.r.UserName,
                               Driver = x.Name,
                               ProjectCode = x.rrc.rra.r.r.r.r.ProjectCode,
                               Cab = x.rrc.Model,
                               Status = x.rrc.rra.r.r.r.StatusName,
                               Kilometer = (x.rrc.rra.r.r.r.r.FinalReading - x.rrc.rra.r.r.r.r.InitialReading),
                               Purpose = x.rrc.rra.r.r.r.r.Purpose
                           }).ToList();
                        return rides.OrderBy(x => x.DateT.Date).ThenBy(x => x.Time).ToList();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public RatingDto GetDriverRating(int driverId)
        {
            try
            {
                var driverRating = new RatingDto();
                var ratings= cabODbContext.RideAssignment.Where(r => r.DriverId == driverId && r.IsDeleted == false)
                    .Join(cabODbContext.Ride, ra => ra.Id, r => r.RideAssignmentId, (ra,r) => new { r.Id, r.RideStatusId})
                    .Join(cabODbContext.RideStatus, ri=> ri.RideStatusId, rs => rs.Id, (ri,rs)=> new {ri.Id, rs.StatusName })
                    .Where(x => x.StatusName == RideStatuses.Completed.ToString()).Join(cabODbContext.Rating, r=>r.Id, rt => rt.RideId, (r,rt) => new { rt } )
                    .Select(x => x.rt).ToList();
                if (ratings.Count > 0)
                {
                        float timingSum, behaviourSum, overallSum;
                        timingSum = behaviourSum = overallSum = 0;
                        foreach (var rating in ratings)
                        {
                            timingSum += Convert.ToInt32(rating.Timing);
                            behaviourSum += Convert.ToInt32(rating.Behaviour);
                            overallSum += Convert.ToInt32(rating.Overall);
                        }
                        float avgTiming, avgBehaviour, avgOverall;
                        avgTiming = timingSum / ratings.Count;
                        avgBehaviour = behaviourSum / ratings.Count;
                        avgOverall = overallSum / ratings.Count;
                        driverRating.Timing = getRating(avgTiming);
                        driverRating.Behaviour = getRating(avgBehaviour);
                        driverRating.Overall = getRating(avgOverall);
                        driverRating.TimingTotal = Convert.ToSingle(Math.Round(avgTiming, 1));
                        driverRating.BehaviourTotal = Convert.ToSingle(Math.Round(avgBehaviour, 1)); ;
                        driverRating.OverallTotal = Convert.ToSingle(Math.Round(avgTiming, 1)); ;
                        driverRating.rideCount = ratings.Count;
                }
                return driverRating;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public IEnumerable<float> getRating(float ratingValue)
        {
            try
            {
                var starRating = new List<float>();
                if (ratingValue > 4)
                {
                    starRating.Add(1);
                    starRating.Add(1);
                    starRating.Add(1);
                    starRating.Add(1);
                    starRating.Add(ratingValue - 4);
                }
                else if (ratingValue > 3)
                {
                    starRating.Add(1);
                    starRating.Add(1);
                    starRating.Add(1);
                    starRating.Add(ratingValue - 3);
                    starRating.Add(0);
                }
                else if (ratingValue > 2)
                {
                    starRating.Add(1);
                    starRating.Add(1);
                    starRating.Add(ratingValue - 2);
                    starRating.Add(0);
                    starRating.Add(0);
                }
                else if (ratingValue > 1)
                {
                    starRating.Add(1);
                    starRating.Add(ratingValue - 1);
                    starRating.Add(0);
                    starRating.Add(0);
                    starRating.Add(0);
                }
                else if (ratingValue > 0)
                {
                    starRating.Add(ratingValue);
                    starRating.Add(0);
                    starRating.Add(0);
                    starRating.Add(0);
                    starRating.Add(0);
                }
                return starRating;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ReportCabRidesDto GetCabReport(int cabId, DateTime start, DateTime end)
        {
            try
            {
                return GetCabReportStatic(cabId, start, end);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ReportDriverRidesDto GetDriverReport(int driverId, DateTime start, DateTime end)
        {
            try
            {
                return GetDriverReportStatic(driverId, start, end);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ReportProjectRidesDto GetProjectReport(string projectCode, DateTime start, DateTime end)
        {
            try
            {
                return GetProjectReportStatic(projectCode, start, end);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ICollection<ReportRidesDto> GetRideReport(string rideType, string rideStatus, DateTime start, DateTime end)
        {
            try
            {
                return GetRideReportStatic(rideType, rideStatus, start, end);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
