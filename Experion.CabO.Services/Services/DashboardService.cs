using Experion.CabO.Data.Entities;
using Experion.CabO.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Experion.CabO.Data.Constant;
using Experion.CabO.Data.Enums;

namespace Experion.CabO.Services.Services
{
    public class DashboardService: IDashboardService
    {
        private static CabODbContext cabODbContext;

        public DashboardService(CabODbContext _cabODbContext)
        {
            cabODbContext = _cabODbContext;
        }
        public static IEnumerable<EmployeeRideDto> GetRideStatic(int userId) 
        {
            try
            {
                var empRides = cabODbContext.Ride
                    .Join(cabODbContext.RideRequestor, r => r.RideRequestorId, rr => rr.Id, (r, rr) => new { r, rr.UserId })
                    .Join(cabODbContext.RideStatus, r => r.r.RideStatusId, rs => rs.Id, (r, rs) => new { r.r, r.UserId, rs.StatusName })
                    .Join(cabODbContext.RideType, r => r.r.RideTypeId, rt => rt.Id, (r,rt)=> new { r.r, r.UserId ,r.StatusName, rt.rideType })
                    .Where(x => x.UserId == userId && x.r.RideDate.Date >= DateTime.Today.Date)
                    .Select(ride => new EmployeeRideDto
                    {
                        UserId = ride.UserId.ToString(),
                        RideId = ride.r.Id,
                        To = ride.r.To,
                        From = ride.r.From,
                        RideDate = getIst(ride.r.RideDate),
                        RideStatus = ride.StatusName,
                        RideTime = getIst(ride.r.RideTime),
                        PassengerCount = ride.r.PassengerCount,
                        CancelReason = ride.r.CancelReason,
                        Purpose = ride.r.Purpose,
                        ContactNo = ride.r.ContactNo,
                        RideTypeName = ride.rideType,
                        CabType = ride.r.CabType,
                        ExternalCabName = ride.r.ExternalCabName
                    }).ToList();
                return empRides.OrderBy(x => x.RideTime.TimeOfDay)
                .OrderBy(x => x.RideDate.Year)
                .OrderBy(x => x.RideDate.Month)
                .OrderBy(x => x.RideDate.Date);
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public static ICollection<AdminRidesDto> GetAdminRidesStatic(DateTime startDate, DateTime endDate)
        {
            try
            {
                DateTime start = new DateTime(startDate.Year, startDate.Month, startDate.Day, 00, 00, 00);
                DateTime end = new DateTime(endDate.Year, endDate.Month, endDate.Day, 00, 00, 00);
                var adminRide = new List<AdminRidesDto>();
                DateTime[] distinctDates = new DateTime[7];
                distinctDates[0] = start;
                for (int i = 1; i < 6; i++)
                {
                    distinctDates[i] = distinctDates[i - 1].AddDays(1);
                }
                distinctDates[6] = end;
                for (int i = 0; i < 7; i++)
                {
                    var rides = cabODbContext.Ride
                    .Join(cabODbContext.RideType, r => r.RideTypeId, rt => rt.Id, (r, rt) => new { r, rt.rideType })
                    .Where(x => x.rideType != RideTypes.RideTypeOffice.ToString() && x.r.RideDate.Date == distinctDates[i].Date)
                    .Join(cabODbContext.RideStatus, rid => rid.r.RideStatusId, rs => rs.Id, (rid, rs) => new { rid, rs.StatusName })
                    .Where(x=> x.StatusName== RideStatuses.Approved.ToString() || x.StatusName == RideStatuses.Completed.ToString() || x.StatusName == RideStatuses.Pending.ToString())
                    .Join(cabODbContext.RideRequestor, r => r.rid.r.RideRequestorId, rr => rr.Id, (r, rr) => new { r, rr.UserName, rr.UserId })
                    .Select(x => new RidesDto
                    {
                        Id = x.r.rid.r.Id,
                        RideYear = getIst(distinctDates[i]).Year.ToString(),
                        RideMonth = getIstMonth(x.r.rid.r.RideDate),
                        RideDay = getIst(x.r.rid.r.RideDate).Day.ToString(),
                        RideHour = getIst(x.r.rid.r.RideTime).Hour,
                        RideMinutes = getIst(x.r.rid.r.RideTime).Minute,
                        TimePeriod = getTimePeriod(x.r.rid.r.RideTime),
                        RideRequestorId = x.r.rid.r.RideRequestorId,
                        UserId = x.UserId,
                        UserName = x.UserName,
                        From = x.r.rid.r.From,
                        To = x.r.rid.r.To,
                        ContactNo = x.r.rid.r.ContactNo,
                        PassengerCount = x.r.rid.r.PassengerCount,
                        Purpose = x.r.rid.r.Purpose,
                        ProjectCode = x.r.rid.r.ProjectCode,
                        RideStatusName = x.r.StatusName,
                        RequestCount = checkMultipleRequests(x.r.rid.r.RideTime, x.r.rid.r.RideDate),
                        RideDate = x.r.rid.r.RideDate,
                        RideTime = x.r.rid.r.RideTime,
                        OnTime = checkOnTime(x.r.rid.r.RideDate, x.r.rid.r.RideTime),
                    }).ToList();
                    var tempAdminRide = new AdminRidesDto
                    {
                        RideDate = distinctDates[i],
                        RideYear = TimeZoneInfo.ConvertTimeFromUtc(distinctDates[i], TimeZoneInfo.FindSystemTimeZoneById(Constant.TimeZone.India)).Year.ToString(),
                        RideMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(TimeZoneInfo.ConvertTimeFromUtc(distinctDates[i], TimeZoneInfo.FindSystemTimeZoneById(Constant.TimeZone.India)).Month).ToString(),
                        RideDay = TimeZoneInfo.ConvertTimeFromUtc(distinctDates[i], TimeZoneInfo.FindSystemTimeZoneById(Constant.TimeZone.India)).Day.ToString(),
                        Rides = rides
                    };
                    adminRide.Add(tempAdminRide);
                }
                return adminRide;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public ICollection<AdminRidesDto> GetAdminRides(DateTime start, DateTime end)
        {
            try
            {
                return GetAdminRidesStatic(start, end);
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public IEnumerable<EmployeeRideDto> GetRide(int userId)
        {
            try
            {
                return GetRideStatic(userId);
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public static int checkMultipleRequests(DateTime rideTime, DateTime rideDate)
        {
            try
            {
                var tempContext = new CabODbContext();
                var requestCount = (from r in tempContext.Ride
                                    join s in tempContext.RideStatus on r.RideStatusId equals s.Id
                                    join rt in tempContext.RideType on r.RideTypeId equals rt.Id
                                    where r.RideTime.Hour == rideTime.Hour && r.RideTime.Minute == rideTime.Minute && r.RideDate == rideDate &&
                                    s.StatusName != RideStatuses.Rejected.ToString() && s.StatusName != RideStatuses.Cancelled.ToString() && rt.rideType != RideTypes.RideTypeOffice.ToString()
                                    select r).Count();
                tempContext.Dispose();
                return requestCount;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public static DateTime getIst(DateTime date)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(date, TimeZoneInfo.FindSystemTimeZoneById(Constant.TimeZone.India));
        }
        public static string getIstMonth(DateTime date)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(TimeZoneInfo.ConvertTimeFromUtc(date, TimeZoneInfo.FindSystemTimeZoneById(Constant.TimeZone.India)).Month).ToString();
        }
        public static string getTimePeriod(DateTime time)
        {
            time = getIst(time);
            var timePeriod = time.Hour >= 12 ? "PM" : "AM";
            if (time.Hour == 24)
                timePeriod = "AM";
            return timePeriod;
        }
        public static bool checkOnTime(DateTime rideDate, DateTime rideTime)
        {
            var dateNow = DateTime.UtcNow;
            var onDate = DateTime.Compare(rideDate.Date, dateNow.Date);
            if (onDate >= 0)
            {
                if (onDate == 0)
                {
                    var onTime = TimeSpan.Compare(rideTime.TimeOfDay, dateNow.TimeOfDay);
                    if (onTime < 0)
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public static RidesDto GetRideByIdStatic(string guid)
        {
            try
            {
                var ride = cabODbContext.Ride.Where(r => r.Guid == guid)
                    .Join(cabODbContext.RideStatus, r => r.RideStatusId, rs => rs.Id, (r, rs) => new { r, rs.StatusName })
                    .Join(cabODbContext.RideRequestor, r => r.r.RideRequestorId, rr => rr.Id, (r, rr) => new { r, rr.UserName, rr.UserId })
                    .Select(x => new RidesDto
                    {
                        Id = x.r.r.Id,
                        RideYear = getIst(x.r.r.RideDate).Year.ToString(),
                        RideMonth = getIstMonth(x.r.r.RideDate),
                        RideDay = getIst(x.r.r.RideDate).Day.ToString(),
                        RideHour = getIst(x.r.r.RideTime).Hour,
                        RideMinutes = getIst(x.r.r.RideTime).Minute,
                        TimePeriod = getTimePeriod(x.r.r.RideTime),
                        RideRequestorId = x.r.r.RideRequestorId,
                        UserId = x.UserId,
                        UserName = x.UserName,
                        From = x.r.r.From,
                        To = x.r.r.To,
                        ContactNo = x.r.r.ContactNo,
                        PassengerCount = x.r.r.PassengerCount,
                        Purpose = x.r.r.Purpose,
                        ProjectCode = x.r.r.ProjectCode,
                        RideStatusName = x.r.StatusName,
                        InitialReading = Convert.ToInt64(x.r.r.InitialReading),
                        FinalReading = Convert.ToInt64(x.r.r.FinalReading),
                    }).FirstOrDefault();
                return ride;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public RidesDto GetRideById(string guid)
        {
            try
            {
                return GetRideByIdStatic(guid);
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}
