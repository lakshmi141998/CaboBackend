using Experion.CabO.Data.Entities;
using Experion.CabO.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Experion.CabO.Data.Enums;

namespace Experion.CabO.Services.Services
{
    public class RideListingService : IRideListingService
    {
        private CabODbContext _context;
        TtsApiService apiService;
        public RideListingService(CabODbContext context, TtsApiService _apiService)
        { 

            _context = context;
            apiService = _apiService;
        }
        public IEnumerable<RidesDto> GetLatestRideStatic()
        {
             try
             {
                var rides = _context.Ride.Where(x => x.Id == _context.Ride.Max(y=>y.Id))
                            .Join(_context.RideStatus, r => r.RideStatusId, s => s.Id, (r, s) => new { r, s.StatusName })
                            .Join(_context.RideRequestor, res => res.r.RideRequestorId, req => req.Id, (res, req) => new { res, req })
                            .Join(_context.RideType, rr => rr.res.r.RideTypeId, rt => rt.Id, (rr,rt) => new { rr, rt.rideType})
                            .Select(i => new RidesDto
                            {
                                Id = i.rr.res.r.Id,
                                RideYear = getIst(i.rr.res.r.RideDate).Year.ToString(),
                                RideMonth = getIstMonth(i.rr.res.r.RideDate),
                                RideDay = getIst(i.rr.res.r.RideDate).Day.ToString(),
                                RideHour = getIst(i.rr.res.r.RideTime).Hour,
                                RideDate = getIst(i.rr.res.r.RideDate),
                                RideTime = getIst(i.rr.res.r.RideTime),
                                RideMinutes = getIst(i.rr.res.r.RideTime).Minute,
                                TimePeriod = getTimePeriod(i.rr.res.r.RideTime),
                                RideRequestorId = i.rr.res.r.RideRequestorId,
                                UserId = i.rr.req.UserId,
                                UserName = i.rr.req.UserName,
                                From = i.rr.res.r.From,
                                To = i.rr.res.r.To,
                                ContactNo = i.rr.res.r.ContactNo,
                                PassengerCount = i.rr.res.r.PassengerCount,
                                Purpose = i.rr.res.r.Purpose,
                                ProjectCode = i.rr.res.r.ProjectCode,
                                RideStatusName = i.rr.res.StatusName,
                                InitialReading = Convert.ToInt32(i.rr.res.r.InitialReading),
                                FinalReading = Convert.ToInt32(i.rr.res.r.FinalReading),
                                OnTime = checkOnTime(i.rr.res.r.RideDate, i.rr.res.r.RideTime),
                                RideType = i.rideType
                            }
                    ).ToList();

                /*var originalGuid = Guid.NewGuid();
                string[] stringGuids = { originalGuid.ToString("B"),
                         originalGuid.ToString("D"),
                         originalGuid.ToString("X") };
                foreach (var stringGuid in stringGuids)
                {
                        Guid newGuid = Guid.Parse(stringGuid);
                }*/
                

                var sorted = rides.OrderBy(x => x.RideDate).ThenBy(x => x.RideTime.TimeOfDay).ToList();

                return sorted; 
             }
            catch(Exception e)
             {
                throw e;
             }
        }
        
        public ICollection<RidesDto> GetRidesDateWiseStatic(DateTime start_date, DateTime end_date, string status)
        {
            try
            {
                var rides = _context.RideStatus.Where(x => x.StatusName == status)
                    .Join(_context.Ride, s => s.Id, r => r.RideStatusId, (s, r) => new { s.Id,s.StatusName, r })
                    .Where(y => y.r.RideStatusId == y.Id && y.r.RideDate >= start_date && y.r.RideDate <= end_date)
                    .Join(_context.RideRequestor, res => res.r.RideRequestorId, req => req.Id, (res, req) => new { res, req })
                    .Join(_context.RideType, rr => rr.res.r.RideTypeId, rt => rt.Id, (rr,rt) => new { rr, rt.rideType})
                    .Select(i => new RidesDto
                    {
                        Id = i.rr.res.r.Id,
                        RideYear = getIst(i.rr.res.r.RideDate).Year.ToString(),
                        RideMonth = getIstMonth(i.rr.res.r.RideDate),
                        RideDay = getIst(i.rr.res.r.RideDate).Day.ToString(),
                        RideHour = getIst(i.rr.res.r.RideTime).Hour,
                        RideDate = getIst(i.rr.res.r.RideDate),
                        RideTime = getIst(i.rr.res.r.RideTime),
                        RideMinutes = getIst(i.rr.res.r.RideTime).Minute,
                        TimePeriod = getTimePeriod(i.rr.res.r.RideTime),
                        RideRequestorId = i.rr.res.r.RideRequestorId,
                        UserId = i.rr.req.UserId,
                        UserName = i.rr.req.UserName,
                        From = i.rr.res.r.From,
                        To = i.rr.res.r.To,
                        ContactNo = i.rr.res.r.ContactNo,
                        PassengerCount = i.rr.res.r.PassengerCount,
                        Purpose = i.rr.res.r.Purpose,
                        ProjectCode = i.rr.res.r.ProjectCode,
                        RideStatusName = i.rr.res.StatusName,
                        InitialReading = Convert.ToInt32(i.rr.res.r.InitialReading),
                        FinalReading = Convert.ToInt32(i.rr.res.r.FinalReading),
                        OnTime = checkOnTime(i.rr.res.r.RideDate, i.rr.res.r.RideTime),
                        RideType = i.rideType
                    }
                    ).ToList();
                var sorted = rides.OrderBy(x => x.RideDate).ThenBy(x => x.RideTime.TimeOfDay).ToList();
                return sorted;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public IEnumerable<RidesDto> GetRidesStatic (string status)

        {          
            try
            {
                DateTime current_date =  DateTime.UtcNow;
                DateTime end = current_date.AddDays(-15);
                
                if (status == RideStatuses.Completed.ToString())
                {
                    end = current_date.AddDays(-30);
                }
                else if (status == RideStatuses.Rejected.ToString() || status == RideStatuses.Cancelled.ToString())
                {
                    end = current_date.AddDays(-15);
                    current_date = current_date.AddDays(30);
                }
                else if (status == RideStatuses.Pending.ToString() || status == RideStatuses.Approved.ToString())
                {
                    end = current_date.AddDays(-1);
                    current_date = current_date.AddDays(30);
                }
                var rides = _context.RideStatus.Where(x => x.StatusName == status)
               .Join(_context.Ride, s => s.Id, r => r.RideStatusId, (s, r) => new { s.Id,s.StatusName, r })
               .Where(y => y.r.RideStatusId == y.Id && y.r.RideDate <= current_date && y.r.RideDate >= end)
               .Join(_context.RideRequestor, res => res.r.RideRequestorId, req => req.Id, (res, req) => new { res, req })
               .Join(_context.RideType, rr => rr.res.r.RideTypeId, rt => rt.Id, (rr, rt) => new { rr, rt.rideType })

               .Select(i => new RidesDto
               {
                   Id = i.rr.res.r.Id,
                   RideYear = getIst(i.rr.res.r.RideDate).Year.ToString(),
                   RideMonth = getIstMonth(i.rr.res.r.RideDate),
                   RideDay = getIst(i.rr.res.r.RideDate).Day.ToString(),
                   RideHour = getIst(i.rr.res.r.RideTime).Hour,
                   RideDate = getIst(i.rr.res.r.RideDate),
                   RideTime = getIst(i.rr.res.r.RideTime),
                   RideMinutes = getIst(i.rr.res.r.RideTime).Minute,
                   TimePeriod = getTimePeriod(i.rr.res.r.RideTime),
                   RideRequestorId = i.rr.res.r.RideRequestorId,
                   UserId = i.rr.req.UserId,
                   UserName = i.rr.req.UserName,
                   From = i.rr.res.r.From,
                   To = i.rr.res.r.To,
                   ContactNo = i.rr.res.r.ContactNo,
                   PassengerCount = i.rr.res.r.PassengerCount,
                   Purpose = i.rr.res.r.Purpose,
                   ProjectCode = i.rr.res.r.ProjectCode,
                   RideStatusName = i.rr.res.StatusName,
                   InitialReading = Convert.ToInt32(i.rr.res.r.InitialReading),
                   FinalReading = Convert.ToInt32(i.rr.res.r.FinalReading),
                   OnTime = checkOnTime(i.rr.res.r.RideDate, i.rr.res.r.RideTime),
                   RideType = i.rideType
               }
               ).ToList();

                var sorted = rides.OrderBy(x => x.RideDate).ThenBy(x => x.RideTime.TimeOfDay).ToList();
                return sorted;
            }
           
            catch(Exception e)
            {
                throw e;
            }

        }
        public static DateTime getIst(DateTime date)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(date, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
        }
        public static string getIstMonth(DateTime date)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(TimeZoneInfo.ConvertTimeFromUtc(date, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")).Month).ToString();
        }
        public static string getTimePeriod(DateTime time)
        {
            time = getIst(time);
            var timePeriod = time.Hour >= 12 ? "PM" : "AM";
            if (time.Hour == 24)
                timePeriod = "AM";
            return timePeriod;
        }
        public IEnumerable<RideStatus>GetOptions()
        {
            try
            {
                var result = _context.RideStatus.ToList();
                return result;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public async Task<ICollection<TTSDetailById>> ViewUserDetails(int id)
        {
            try
            {
                var userDetails = new List<TTSDetailById>();
                //var userId = _context.RideRequestor.FirstOrDefault(x => x.UserId == id).UserId;
                var user = await apiService.GetTTSUserDetailsById(id);
                var details = new TTSDetailById
                {
                   userName = user.firstName + " " + user.lastName,
                   designation = new DesignationDto{ 
                        name = user.designation.name
                   },
                   email = user.email,
                   managerId = "",
                   mobileNo = user.mobileNo,
                   employeeId = user.employeeId
                };
                userDetails.Add(details);
                return userDetails;
            }
            catch(Exception e)
            {
                throw e;
            }
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

        public IEnumerable<RidesDto> GetRides(string status)
        {
            try
            {
                return (GetRidesStatic(status));
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public IEnumerable<RidesDto> GetLatestRide()
        {
            try
            {
                return (GetLatestRideStatic());
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ICollection<RidesDto> GetRidesDateWise(DateTime start_date, DateTime end_date, string status)
        {
            try
            {
                return (GetRidesDateWiseStatic(start_date, end_date, status));
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}

