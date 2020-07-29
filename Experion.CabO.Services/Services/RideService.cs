using Experion.CabO.Data.Entities;
using Experion.CabO.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Microsoft.Extensions.Configuration;
using Twilio.Types;
using System.Threading.Tasks;
using Experion.CabO.Data.Constant;
using Experion.CabO.Data.Enums;

namespace Experion.CabO.Services.Services
{
    public class RideService : IRideService
    {
        private CabODbContext cabODbContext;
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        private IConfiguration Configuration { get; }
        private PhoneNumber twilioNo;
        private string accountSid, authToken;
        private TtsApiService apiService;
        private string FEHostedAddress;
        private string CabOMailId;
        private string CabOMailPassword;
        public RideService(CabODbContext _cabODbContext, IConfiguration _configuration, TtsApiService _apiService)
        {
            cabODbContext = _cabODbContext;
            Configuration = _configuration;
            apiService = _apiService;
            accountSid = Configuration.GetValue<string>(Constant.Default.TwilioSID);
            authToken = Configuration.GetValue<string>(Constant.Default.TwilioAuthToken);
            twilioNo = Configuration.GetValue<string>(Constant.Default.TwilioWhatsAppNo);
            FEHostedAddress = Configuration.GetValue<string>(Constant.Default.FEHostedAddress);
            TwilioClient.Init(accountSid, authToken);
            CabOMailId = Configuration.GetValue<string>(Constant.Default.CabOMailId);
            CabOMailPassword = Configuration.GetValue<string>(Constant.Default.CabOMailPassword);
        }
        public async Task<ICollection<TTSUserDetailsById>> GetAdminDetails()
        {
            try
            {
                var allAdmins = new List<TTSUserDetailsById>();
                var admins1 = await apiService.GetAdminDetails(Constant.AdminRoles.AdminRole1);
                var admins2 = await apiService.GetAdminDetails(Constant.AdminRoles.AdminRole2);
                allAdmins = admins1.Union(admins2).ToList();
                return allAdmins;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public bool ApproveRideByDriver(int rideId)
        {
            try
            {
                int statusId = getStatusId(RideStatuses.Approved.ToString());
                var result = (from r in cabODbContext.Ride
                              where r.Id == rideId
                              select r).SingleOrDefault();
                result.RideStatusId = statusId; 
                cabODbContext.SaveChanges();
                return true;
            }
            catch(Exception e)
            {
                throw e;
            }
         }
        public bool ApproveRide(int rideId, int cabId, string cabType, string externalCabName)
        {
            try
            {
                int statusId = getStatusId(RideStatuses.Approved.ToString());
                var result = (from r in cabODbContext.Ride
                               where r.Id == rideId
                               select r).SingleOrDefault();
                result.RideStatusId = statusId;
                result.CabType = cabType;
                int rideAssignmentId = 0;
                if (cabType == CabTypes.Internal.ToString())
                {
                    rideAssignmentId = getAssignmentId(rideId, cabId);
                    result.RideAssignmentId = rideAssignmentId;
                }
                else
                {
                    result.ExternalCabName = externalCabName;
                }
                cabODbContext.SaveChanges();
                return true;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public bool RejectRideByDriver (int rideId)
        {
            try
            {
                int statusId = getStatusId(RideStatuses.Rejected.ToString());
                Ride result = (from r in cabODbContext.Ride
                               where r.Id == rideId
                               select r).SingleOrDefault();
                result.RideStatusId = statusId;
                cabODbContext.SaveChanges();
                return true;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public bool RejectRide(int rideId, string cancelReason)
        {
            try
            {
                int statusId = getStatusId(RideStatuses.Rejected.ToString());
                Ride result = (from r in cabODbContext.Ride
                               where r.Id == rideId
                               select r).SingleOrDefault();
                result.CancelReason = cancelReason;
                result.RideStatusId = statusId;
                cabODbContext.SaveChanges();
                return true;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public ICollection<CabListDto> GetCabList(DateTime rideDate, DateTime rideTime)
        {
            try
            {
                var rDate = TimeZoneInfo.ConvertTimeFromUtc(rideDate,
              TimeZoneInfo.FindSystemTimeZoneById(Constant.TimeZone.India));

                rDate = new DateTime(rDate.Year, rDate.Month, rDate.Day, 12, 30, 00);
                var rTime = rideTime;
                var result = new List<CabListDto>();
                var shiftId = getShiftId(rTime);
                var cabIdList = (from a in cabODbContext.RideAssignment
                                 where a.DailyDate == rDate && a.ShiftId == shiftId && 
                                 a.IsDeleted == false
                                 select a.CabId).Distinct().ToArray();
                var cabs = (from c in cabODbContext.Cab
                            where c.IsDeleted == false
                            && cabIdList.Contains(c.Id)
                            select new
                            {
                                c.Id,
                                c.Model
                            });
                foreach(var cab in cabs)
                {
                    var tempCab = new CabListDto
                    {
                        cabId = cab.Id,
                        cabName=cab.Model
                    };
                    result.Add(tempCab);
                }
                return result;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public int getStatusId(string statusName)
        {
            try
            {
                var statusId = (from s in cabODbContext.RideStatus
                                where s.StatusName == statusName
                                select s.Id).FirstOrDefault();
                return statusId;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public DateTime getRideDate(int rideId)
        {
            try
            {
                var rideDate = (from r in cabODbContext.Ride
                                where r.Id == rideId
                                select r.RideDate).First();
                return rideDate;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public DateTime getRideTime(int rideId)
        {
            try
            {
                var rideTime = (from r in cabODbContext.Ride
                                where r.Id == rideId
                                select r.RideTime).First();
                return rideTime;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public int getShiftId(DateTime rTime)
        {
            try
            {
                var shifts = cabODbContext.Shift.ToList();
                int shiftId = 0;
                foreach (Shift shiftInfo in shifts)
                {
                    var shiftStart = shiftInfo.ShiftStart.TimeOfDay;
                    var shiftEnd = shiftInfo.ShiftEnd.TimeOfDay;
                    var rideTime = TimeZoneInfo.ConvertTimeFromUtc(rTime, TimeZoneInfo.FindSystemTimeZoneById(Constant.TimeZone.India)).TimeOfDay;
                    if (shiftStart < shiftEnd && rideTime >= shiftStart && rideTime <= shiftEnd)
                    {

                        shiftId = shiftInfo.Id;
                        break;
                    }
                    else if (shiftStart > shiftEnd)
                    {
                        if (rideTime > shiftEnd && rideTime > shiftStart)
                        {
                            shiftId = shiftInfo.Id;
                            break;
                        }
                        else if (rideTime < shiftStart && rideTime < shiftEnd)
                        {

                            shiftId = shiftInfo.Id;
                            break;
                        }
                    }
                }
                return shiftId;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public int getAssignmentId(int rideId, int cabId)
        {
            try
            {
                var rideTime = getRideTime(rideId);
                var rideDate = getRideDate(rideId);
                rideDate = new DateTime(rideDate.Year, rideDate.Month, rideDate.Day, 12, 30, 00);
                var shiftId = getShiftId(rideTime);
                var rideAssignmentId = (from a in cabODbContext.RideAssignment
                                        where a.CabId == cabId && a.ShiftId == shiftId && a.DailyDate == rideDate && a.IsDeleted == false
                                        select a.Id).FirstOrDefault();
                return rideAssignmentId;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public int AddRide(EmployeeRideEditDto ride)
        {
            try
            {
                var adminDetails = GetAdminDetails().Result;
                var exist = cabODbContext.RideRequestor.Any(x => x.UserId == Int32.Parse(ride.UserId));
                if (!exist)
                {
                    var newRequestor = new RideRequestor
                    {
                        UserId = Int32.Parse(ride.UserId),
                        UserName = ride.UserName,
                        Email = ride.Email
                    };
                    cabODbContext.RideRequestor.Add(newRequestor);
                    cabODbContext.SaveChanges();
                }
                var requestorId = cabODbContext.RideRequestor.Where(x => x.UserId == Int32.Parse(ride.UserId)).Select(x => x.Id).FirstOrDefault();
                int statusId = cabODbContext.RideStatus.Where(x => x.StatusName == RideStatuses.Pending.ToString()).Select(x => x.Id).FirstOrDefault();
                var rideTypeId = cabODbContext.RideType.Where(r => r.rideType == ride.RideType).Select(i => i.Id).FirstOrDefault();
                var date = new DateTime(ride.RideDate.Year, ride.RideDate.Month, ride.RideDate.Day);
                var time = new DateTime(01, 01, 01, ride.RideTime.Hour, ride.RideTime.Minute, ride.RideTime.Second);
                Guid guid = Guid.NewGuid();
                var newRide = new Ride
                {
                    RideRequestorId =requestorId,
                    To = ride.To,
                    From = ride.From,
                    RideDate = date,
                    RideTime = time,
                    ContactNo = ride.ContactNo,
                    PassengerCount = ride.PassengerCount,
                    Purpose = ride.Purpose,
                    ProjectCode = ride.ProjectCode,
                    RideStatusId = statusId,
                    RideTypeId = rideTypeId,
                    Guid = guid.ToString()
                };
                cabODbContext.Ride.Add(newRide);
                cabODbContext.SaveChanges();
                return newRide.Id;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public bool sendMailRideAdded(int rideId) {
            try
            {
                var adminDetails = GetAdminDetails().Result;
                var rideTypeOffice = EnumHelper<RideTypes>.GetDisplayValue(RideTypes.RideTypeOffice);
                var newRide = cabODbContext.Ride.Where(r => r.Id == rideId).Select(x => x)
                                .Join(cabODbContext.RideRequestor, r=> r.RideRequestorId, rr=> rr.Id, (r,rr)=>new { r, rr.UserName, rr.Email}).FirstOrDefault();
                var officeRideId = cabODbContext.RideType.Where(o => o.rideType == RideTypes.RideTypeOffice.ToString()).Select(x => x.Id).FirstOrDefault();
                var fromAddressSplit = newRide.r.From.Split(",");
                var toAddressSplit = newRide.r.To.Split(",");
                var fromAddress = fromAddressSplit[0];
                var toAddress = toAddressSplit[0];
                var msg = newRide.UserName + " has requested a ride starting from " + fromAddress + " to " + toAddress + " on " + TimeZoneInfo.ConvertTimeFromUtc(newRide.r.RideDate,
                TimeZoneInfo.FindSystemTimeZoneById(Constant.TimeZone.India)).ToString("dd-MMM-yyyy") + " at " + TimeZoneInfo.ConvertTimeFromUtc(newRide.r.RideTime,
                TimeZoneInfo.FindSystemTimeZoneById(Constant.TimeZone.India)).ToString("hh:mm tt");
                msg += ". Contact: " + newRide.r.ContactNo;
                if (newRide.r.RideTypeId == officeRideId)
                {
                    var loc1 = cabODbContext.OfficeLocation.Where(o => o.Address == newRide.r.From && o.IsDeleted == false).Select(x => x.Id).FirstOrDefault();
                    var loc2 = cabODbContext.OfficeLocation.Where(o => o.Address == newRide.r.To && o.IsDeleted == false).Select(x => x.Id).FirstOrDefault();
                    var tempRideInfo = new CheckTimeInfo
                    {
                        location1 = loc1,
                        location2 = loc2,
                        rideTime = newRide.r.RideTime,
                        rideDate = newRide.r.RideDate
                    };
                    var driverInfo = CheckTime(tempRideInfo);
                    string mailBody = "<body><div style='font-family: Calibri; border:1px solid #B1B1B1; padding: 20px;'>";
                    mailBody += "<div style='text-align:center;'>";
                    mailBody += "<span style='background-color: #2EB83C; color: white; padding: 5px 10%; width:80%; font-size: 1.5em;'>";
                    mailBody += "Ride Request Submitted Successfully";
                    mailBody += "</span>";
                    mailBody += "</div>";
                    mailBody += "<div style = 'background-color: #FCF3D8; padding:10px; margin-left:auto; margin-right:auto; width: 90%; margin-top: 10px;' >";
                    mailBody += "<table style='margin: 0 auto;'>";
                    mailBody += "<tr><td style = 'padding: 5px; 10px;'> From </td><td style = 'padding: 5px; 10px;'>:</td><td style = 'padding: 5px; 10px;'>" + newRide.r.From + " </td></tr>";
                    mailBody += "<tr><td style = 'padding: 5px; 10px;'> To </td><td style = 'padding: 5px; 10px;'>:</td><td style = 'padding: 5px; 10px;'>" + newRide.r.To + "</td></tr>";
                    mailBody += "<tr><td style = 'padding: 5px; 10px;'> Date </td><td style = 'padding: 5px; 10px;'>:</td><td style = 'padding: 5px; 10px;'>" + TimeZoneInfo.ConvertTimeFromUtc(newRide.r.RideDate,
              TimeZoneInfo.FindSystemTimeZoneById(Constant.TimeZone.India)).ToString("dd-MMM-yyyy") + "</td></tr>";
                    mailBody += "<tr><td style = 'padding: 5px; 10px;'> Time </td><td style = 'padding: 5px; 10px;'>:</td><td style = 'padding: 5px; 10px;'>" + TimeZoneInfo.ConvertTimeFromUtc(newRide.r.RideTime,
              TimeZoneInfo.FindSystemTimeZoneById(Constant.TimeZone.India)).ToString("hh:mm tt") + "</td></tr>";
                    mailBody += "<tr><td style = 'padding: 5px; 10px;'> Assigned To </td><td style = 'padding: 5px; 10px;'>:</td><td style = 'padding: 5px; 10px;'>" + driverInfo.driverName + "</td></tr>";
                    mailBody += "<tr><td style = 'padding: 5px; 10px;'> Cab </td><td style = 'padding: 5px; 10px;'>:</td><td style = 'padding: 5px; 10px;'>" + driverInfo.cabName + "</td></tr>";
                    mailBody += "<tr><td style = 'padding: 5px; 10px;'> Contact </td><td style = 'padding: 5px; 10px;'>:</td><td style = 'padding: 5px; 10px;'>" + driverInfo.driverPhone + "</td></tr>";
                    mailBody += "</table>";
                    mailBody += "</div>";
                    mailBody += "</div></body>";
                    mailBody += "This is a system generated booking. Request to contact the driver in case you haven't received a ride acceptance from the driver via e-mail.";
                    MailMessage mailMsg = new MailMessage();
                    mailMsg.From = new MailAddress(CabOMailId);
                    mailMsg.To.Add(newRide.Email);
                    mailMsg.Body = mailBody;
                    mailMsg.IsBodyHtml = true;
                    mailMsg.Subject = "Ride Request Submitted Successfully!";
                    SmtpClient smt = new SmtpClient("smtp.gmail.com", 587);
                    smt.EnableSsl = true;
                    smt.UseDefaultCredentials = false;
                    smt.Credentials = new NetworkCredential(CabOMailId, CabOMailPassword);
                    smt.Send(mailMsg);
                    int rideAssignmentId = getAssignmentId(newRide.r.Id, driverInfo.cabId);
                    var rideData = cabODbContext.Ride.Where(r => r.Id == newRide.r.Id).FirstOrDefault();
                    rideData.RideAssignmentId = rideAssignmentId;
                    cabODbContext.SaveChanges();
                    if (adminDetails.Count > 0)
                    {
                        var msg2 = newRide.UserName + " has requested a office-to office ride starting from " + fromAddress + " to " + toAddress + " on " + TimeZoneInfo.ConvertTimeFromUtc(newRide.r.RideDate,
                    TimeZoneInfo.FindSystemTimeZoneById(Constant.TimeZone.India)).ToString("dd-MMM-yyyy") + " at " + TimeZoneInfo.ConvertTimeFromUtc(newRide.r.RideTime,
                    TimeZoneInfo.FindSystemTimeZoneById(Constant.TimeZone.India)).ToString("hh:mm tt");
                        msg +=". Contact: " + newRide.r.ContactNo;

                        foreach (var admin in adminDetails)
                        {
                            sendWhatsapp(msg2, admin.mobileNo);
                        }
                    }
                    msg += ". Visit: " + FEHostedAddress + "my-rides/" + newRide.r.Guid;
                    sendWhatsapp(msg, driverInfo.driverPhone);
                }
                else
                {

                    msg += ". Visit: " + FEHostedAddress + "admin-rides/" + newRide.r.Guid;

                    if (adminDetails.Count > 0)
                    {
                        if (adminDetails.Count > 0)
                        {
                            foreach(var admin in adminDetails)
                            {
                                sendWhatsapp(msg, admin.mobileNo);
                            }
                        }
                    }
                    string mailBody = "<body><div style='font-family: Calibri; border:1px solid #B1B1B1; padding: 20px;'>";
                    mailBody += "<div style='text-align:center;'>";
                    mailBody += "<span style='background-color: #2EB83C; color: white; padding: 5px 10%; width:80%; font-size: 1.5em;'>";
                    mailBody += "Ride Request Submitted Successfully";
                    mailBody += "</span>";
                    mailBody += "</div>";
                    mailBody += "<div style = 'background-color: #FCF3D8; padding:10px; margin-left:auto; margin-right:auto; width: 90%; margin-top: 10px;' >";
                    mailBody += "<table style='margin: 0 auto;'>";
                    mailBody += "<tr><td style = 'padding: 5px; 10px;'> From </td><td style = 'padding: 5px; 10px;'>:</td><td style = 'padding: 5px; 10px;'>" + newRide.r.From + " </td></tr>";
                    mailBody += "<tr><td style = 'padding: 5px; 10px;'> To </td><td style = 'padding: 5px; 10px;'>:</td><td style = 'padding: 5px; 10px;'>" + newRide.r.To + "</td></tr>";
                    mailBody += "<tr><td style = 'padding: 5px; 10px;'> Date </td><td style = 'padding: 5px; 10px;'>:</td><td style = 'padding: 5px; 10px;'>" + TimeZoneInfo.ConvertTimeFromUtc(newRide.r.RideDate,
              TimeZoneInfo.FindSystemTimeZoneById(Constant.TimeZone.India)).ToString("dd-MMM-yyyy") + "</td></tr>";
                    mailBody += "<tr><td style = 'padding: 5px; 10px;'> Time </td><td style = 'padding: 5px; 10px;'>:</td><td style = 'padding: 5px; 10px;'>" + TimeZoneInfo.ConvertTimeFromUtc(newRide.r.RideTime,
              TimeZoneInfo.FindSystemTimeZoneById(Constant.TimeZone.India)).ToString("hh:mm tt") + "</td></tr>";
                    mailBody += "</table>";
                    mailBody += "</div>";
                    mailBody += "</div></body>";
                    mailBody += "This is a system generated booking. Request to contact the admin in case you haven't received a ride acceptance from the admin via e-mail.";
                    MailMessage mailMsg = new MailMessage();
                    mailMsg.From = new MailAddress(CabOMailId);
                    mailMsg.To.Add(newRide.Email);
                    mailMsg.Body = mailBody;
                    mailMsg.IsBodyHtml = true;
                    mailMsg.Subject = "Ride Request Submitted Successfully!";
                    SmtpClient smt = new SmtpClient("smtp.gmail.com", 587);
                    smt.EnableSsl = true;
                    smt.UseDefaultCredentials = false;
                    smt.Credentials = new NetworkCredential(CabOMailId, CabOMailPassword);
                    smt.Send(mailMsg);
                }
                return true;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public async Task<Object> ProjectsGet(int userId)
        {
            try
            {
                var projectsList = await apiService.GetTTSProjectsByUserId(userId);
                var orderedProjectsList = projectsList.OrderByDescending(p => p.projectId);
                var projects = new List<String>();
                foreach (TTSProjectsByUserId project in orderedProjectsList)
                {
                    var projectString = project.project.code + "(" + project.project.description + ")";
                    projects.Add(projectString);
                }
                return projects;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public bool CancelRide(int rideId)
        {
            try
            {
                var adminDetails = GetAdminDetails().Result;
                var ride = cabODbContext.Ride.Where(x => x.Id == rideId)
                            .Join(cabODbContext.RideRequestor, r => r.RideRequestorId, rr => rr.Id, (r, rr) => new { r, rr.Email, rr.UserName })
                            .Join(cabODbContext.RideStatus, r => r.r.RideStatusId, rs=> rs.Id, (r,rs)=> new { r,rs.StatusName})
                            .Join(cabODbContext.RideType, r=> r.r.r.RideTypeId, rt=> rt.Id, (r,rt)=> new { r, rt.rideType}).FirstOrDefault();
                var prevStatus = ride.r.StatusName;
                var cancelId = cabODbContext.RideStatus.Where(r => r.StatusName == RideStatuses.Cancelled.ToString()).Select(x => x.Id).FirstOrDefault();
                ride.r.r.r.RideStatusId = cancelId;
                cabODbContext.SaveChanges();
                if (prevStatus == RideStatuses.Approved.ToString())
                {
                    var fromAddressSplit = ride.r.r.r.From.Split(",");
                    var toAddressSplit = ride.r.r.r.To.Split(",");
                    var fromAddress = fromAddressSplit[0];
                    var toAddress = toAddressSplit[0];
                    var msg = "Ride request starting from " + fromAddress + " to " + toAddress + " on " + TimeZoneInfo.ConvertTimeFromUtc(ride.r.r.r.RideDate,
                       TimeZoneInfo.FindSystemTimeZoneById(Constant.TimeZone.India)).ToString("dd-MMM-yyyy") + " at " + TimeZoneInfo.ConvertTimeFromUtc(ride.r.r.r.RideTime,
                       TimeZoneInfo.FindSystemTimeZoneById(Constant.TimeZone.India)).ToString("hh:mm tt");
                    msg += " requested By " + ride.r.r.UserName + " is cancelled by user. ";
                    if (ride.rideType != RideTypes.RideTypeOffice.ToString())
                    {
                        if (adminDetails.Count > 0)
                        {
                            foreach (var admin in adminDetails)
                            {
                                sendWhatsapp(msg, admin.mobileNo);
                            }
                        }
                    }
                    if (ride.rideType == RideTypes.RideTypeOffice.ToString() || ride.r.r.r.CabType == CabTypes.Internal.ToString())
                    {
                        var driverNo = cabODbContext.RideAssignment.Where(ra => ra.Id == ride.r.r.r.RideAssignmentId).Join(cabODbContext.Driver, ra => ra.DriverId, d => d.Id, (ra, d) => new { ra, d }).Select(x => x.d.PhoneNo).FirstOrDefault();
                        sendWhatsapp(msg, driverNo);
                    }
                    
                }
                return true;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public int RideAssignmenId(int driverId)
        {
            try
            {
                DateTime now = DateTime.Now;
                DateTime date = new DateTime(now.Year, now.Month, now.Day, 12, 30, 00);
                var time = new DateTime(01, 01, 01, now.Hour, now.Minute, now.Second);
                var shifts = cabODbContext.Shift.ToList();
                int shiftId = 0;
                foreach (Shift shiftInfo in shifts)
                {
                  if(shiftInfo.ShiftStart< shiftInfo.ShiftEnd && time>= shiftInfo.ShiftStart && time < shiftInfo.ShiftEnd)
                    {
                     
                        shiftId = shiftInfo.Id;
                        break;
                    }
                    else if(shiftInfo.ShiftStart > shiftInfo.ShiftEnd)
                    {
                        if(time> shiftInfo.ShiftEnd && time > shiftInfo.ShiftStart)
                        {
                            shiftId = shiftInfo.Id;
                            break;
                        }
                        else if(time< shiftInfo.ShiftStart && time < shiftInfo.ShiftEnd)
                        {
                            
                            shiftId = shiftInfo.Id;
                            break;
                        }
                    }
                }
                var rideAssignment = cabODbContext.RideAssignment.Where(x => x.DriverId == driverId && x.DailyDate == date && x.ShiftId == shiftId && x.IsDeleted==false).FirstOrDefault();
                return rideAssignment.Id;
            }
            catch
            {
                return 0;
            }
        }
        public bool sendMail(int rideId, string status)
        {
            try
            {
                var adminDetails = GetAdminDetails().Result;
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("rideinfo.cabo@gmail.com");
                var mailDetails = getApprovedRideDetails(rideId);
                msg.To.Add(mailDetails.MailId);
                string cabType = cabODbContext.Ride.Where(r => r.Id == rideId).Select(x => x.CabType).FirstOrDefault();
                if (status == RideStatuses.Approved.ToString())
                {
                    string guid = cabODbContext.Ride.Where(r => r.Id == rideId).Select(x => x.Guid).FirstOrDefault();
                    msg.Body = "<body><div style='font-family: Calibri; border:1px solid #B1B1B1; padding: 20px;'>";
                    msg.Body += "<div style='text-align:center;'>";
                    msg.Body += "<span style='background-color: #2EB83C; color: white; padding: 5px 10%; width:80%; font-size: 1.5em;'>";
                    msg.Body += "Ride Request Approved";
                    msg.Body += "</span>";
                    msg.Body += "</div>";
                    msg.Body += "<div style = 'background-color: #FCF3D8; padding:10px; margin-left:auto; margin-right:auto; width: 90%; margin-top: 10px;' >";
                    msg.Body += "<table style='margin: 0 auto;'>";
                    msg.Body += "<tr><td style = 'padding: 5px; 10px;'> From </td><td style = 'padding: 5px; 10px;'>:</td><td style = 'padding: 5px; 10px;'>" + mailDetails.RideFrom + " </td></tr>";
                    msg.Body += "<tr><td style = 'padding: 5px; 10px;'> To </td><td style = 'padding: 5px; 10px;'>:</td><td style = 'padding: 5px; 10px;'>" + mailDetails.RideTo + "</td></tr>";
                    msg.Body += "<tr><td style = 'padding: 5px; 10px;'> Date </td><td style = 'padding: 5px; 10px;'>:</td><td style = 'padding: 5px; 10px;'>" + mailDetails.RideDate + "</td></tr>";
                    msg.Body += "<tr><td style = 'padding: 5px; 10px;'> Time </td><td style = 'padding: 5px; 10px;'>:</td><td style = 'padding: 5px; 10px;'>" + mailDetails.RideTime + "</td></tr>";
                    if (cabType == CabTypes.Internal.ToString())
                    {
                        msg.Body += "<tr><td style = 'padding: 5px; 10px;'> Driver Name </td><td style = 'padding: 5px; 10px;'>:</td><td style = 'padding: 5px; 10px;'>" + mailDetails.DriverName + "</td></tr>";
                        msg.Body += "<tr><td style = 'padding: 5px; 10px;'> Driver Phone </td><td style = 'padding: 5px; 10px;'>:</td><td style = 'padding: 5px; 10px;'>" + mailDetails.DriverPhone + "</td></tr>";
                        msg.Body += "<tr><td style = 'padding: 5px; 10px;'> Cab Name </td><td style = 'padding: 5px; 10px;'>:</td><td style = 'padding: 5px; 10px;'>" + mailDetails.CabName + "</td></tr>";
                        if (mailDetails.RideType != RideTypes.RideTypeOffice.ToString())
                        {
                            var fromAddressSplit = mailDetails.RideFrom.Split(",");
                            var toAddressSplit = mailDetails.RideTo.Split(",");
                            var fromAddress = fromAddressSplit[0];
                            var toAddress = toAddressSplit[0];
                            var whatsappMsg = mailDetails.FullName + " has requested a ride starting from " + fromAddress + " to " + toAddress + " on " + mailDetails.RideDate + " at " + mailDetails.RideTime;
                            whatsappMsg += ". Contact: " + mailDetails.ContactNo;
                            whatsappMsg += ". Visit: " + FEHostedAddress + "my-rides/" + guid;
                            sendWhatsapp(whatsappMsg, mailDetails.DriverPhone);
                        }
                    }
                    else if(cabType == CabTypes.External.ToString())
                    {
                        msg.Body += "<tr><td style = 'padding: 5px; 10px;'> Assigned to External Cab </td><td style = 'padding: 5px; 10px;'>:</td><td style = 'padding: 5px; 10px;'>" + mailDetails.ExternalCabName + "</td></tr>";
                    }
                    if (mailDetails.RideType == RideTypes.RideTypeOffice.ToString())
                    {
                        msg.Body += "<tr><td style = 'padding: 5px; 10px;'> Driver Name </td><td style = 'padding: 5px; 10px;'>:</td><td style = 'padding: 5px; 10px;'>" + mailDetails.DriverName + "</td></tr>";
                        msg.Body += "<tr><td style = 'padding: 5px; 10px;'> Driver Phone </td><td style = 'padding: 5px; 10px;'>:</td><td style = 'padding: 5px; 10px;'>" + mailDetails.DriverPhone + "</td></tr>";
                        msg.Body += "<tr><td style = 'padding: 5px; 10px;'> Cab Name </td><td style = 'padding: 5px; 10px;'>:</td><td style = 'padding: 5px; 10px;'>" + mailDetails.CabName + "</td></tr>";
                        var fromAddressSplit = mailDetails.RideFrom.Split(",");
                        var toAddressSplit = mailDetails.RideTo.Split(",");
                        var fromAddress = fromAddressSplit[0];
                        var toAddress = toAddressSplit[0];
                        var whatsappMsg = "Ride request starting from " + fromAddress + " to " + toAddress + " on " + mailDetails.RideDate + " at " + mailDetails.RideTime;
                        whatsappMsg += " requested By " + mailDetails.FullName ;
                        whatsappMsg += " is approved by driver " + mailDetails.DriverName + ".";
                        if (adminDetails.Count > 0)
                        {
                            foreach (var admin in adminDetails)
                            {
                                sendWhatsapp(whatsappMsg, admin.mobileNo);
                            }
                        }
                    }
                    msg.Body += "</table>";
                    msg.Body += "</div>";
                    msg.Body += "</div></body>";
                    msg.IsBodyHtml = true;
                    msg.Subject = "Ride Request Approved!";
                }
                else
                {
                    var cancelReason = cabODbContext.Ride.Where(x => x.Id == rideId).FirstOrDefault().CancelReason;
                    msg.Body = "<body><div style='font-family: Calibri; border:1px solid #B1B1B1; padding: 20px;'>";
                    msg.Body += "<div style='text-align:center;'>";
                    msg.Body += "<span style='background-color: #E43E13; color: white; padding: 5px 10%; width:80%; font-size: 1.5em;'>";
                    if (status == RideStatuses.Rejected.ToString())
                    {
                        msg.Body += "Ride Request Rejected";
                        var fromAddressSplit = mailDetails.RideFrom.Split(",");
                        var toAddressSplit = mailDetails.RideTo.Split(",");
                        var fromAddress = fromAddressSplit[0];
                        var toAddress = toAddressSplit[0];
                        if (mailDetails.RideType == RideTypes.RideTypeOffice.ToString())
                        {
                            
                            var whatsappMsg = "Ride request starting from " + fromAddress + " to " + toAddress + " on " + mailDetails.RideDate + " at " + mailDetails.RideTime;
                            whatsappMsg += " requested By " + mailDetails.FullName;
                            whatsappMsg += " is rejected by driver " + mailDetails.DriverName + ".";
                            if (adminDetails.Count > 0)
                            {
                                foreach (var admin in adminDetails)
                                {
                                    sendWhatsapp(whatsappMsg, admin.mobileNo);
                                }
                            }
                        }
                        else if(cabType == CabTypes.Internal.ToString())
                        {
                            var whatsappMsg = "Ride request starting from " + fromAddress + " to " + toAddress + " on " + mailDetails.RideDate + " at " + mailDetails.RideTime;
                            whatsappMsg += " requested By " + mailDetails.FullName;
                            whatsappMsg += " is rejected by admin.";
                            sendWhatsapp(whatsappMsg, mailDetails.DriverPhone);
                        }
                    }
                    else
                    {
                        msg.Body += "Ride Request Cancelled";
                    }
                    msg.Body += "</span>";
                    msg.Body += "</div>";
                    msg.Body += "<div style = 'background-color: #FCF3D8; padding:10px; margin-left:auto; margin-right:auto; width: 90%; margin-top: 10px;' >";
                    msg.Body += "<table style='margin: 0 auto;'>";
                    msg.Body += "<tr><td style = 'padding: 5px; 10px;'> From </td><td style = 'padding: 5px; 10px;'>:</td><td style = 'padding: 5px; 10px;'>" + mailDetails.RideFrom + " </td></tr>";
                    msg.Body += "<tr><td style = 'padding: 5px; 10px;'> To </td><td style = 'padding: 5px; 10px;'>:</td><td style = 'padding: 5px; 10px;'>" + mailDetails.RideTo + "</td></tr>";
                    msg.Body += "<tr><td style = 'padding: 5px; 10px;'> Date </td><td style = 'padding: 5px; 10px;'>:</td><td style = 'padding: 5px; 10px;'>" + mailDetails.RideDate + "</td></tr>";
                    msg.Body += "<tr><td style = 'padding: 5px; 10px;'> Time </td><td style = 'padding: 5px; 10px;'>:</td><td style = 'padding: 5px; 10px;'>" + mailDetails.RideTime + "</td></tr>";
                    if (status == RideStatuses.Rejected.ToString())
                    {
                        msg.Body += "<tr><td style = 'padding: 5px; 10px;'> Reason for reject </td><td style = 'padding: 5px; 10px;'>:</td><td style = 'padding: 5px; 10px;'>" + cancelReason + "</td></tr>";
                    }
                    else
                    {
                        msg.Body += "<tr><td style = 'padding: 5px; 10px;'> Requested By </td><td style = 'padding: 5px; 10px;'>:</td><td style = 'padding: 5px; 10px;'>" + mailDetails.FullName + "</td></tr>";
                    }
                    msg.Body += "</table>";
                    msg.Body += "</div>";
                    msg.Body += "</div></body>";
                    msg.IsBodyHtml = true;
                    if (status == RideStatuses.Rejected.ToString())
                    {
                        msg.Subject = "Ride Request Rejected!";
                    }
                    else
                    {
                        msg.Subject = "Ride Request Cancelled!";
                    }
                }
                SmtpClient smt = new SmtpClient("smtp.gmail.com", 587);
                smt.EnableSsl = true;
                smt.UseDefaultCredentials = false;
                smt.Credentials = new NetworkCredential(CabOMailId, CabOMailPassword);
                smt.Send(msg);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public MailDto getApprovedRideDetails(int rideId)
        {
            try
            {
                var mailDetails = new MailDto();
                var rideDetails = cabODbContext.Ride.Where(r => r.Id == rideId)
                                    .Join(cabODbContext.RideRequestor, r => r.RideRequestorId, rr => rr.Id, (r, rr) => new { r, rr.Email, rr.UserName })
                                    .Join(cabODbContext.RideType, r=> r.r.RideTypeId, rt=> rt.Id, (r,rt)=> new { r.r, r.Email, r.UserName, rt.rideType})
                                    .FirstOrDefault();
                mailDetails.MailId = rideDetails.Email;
                mailDetails.FullName = rideDetails.UserName;
                mailDetails.RideDate = TimeZoneInfo.ConvertTimeFromUtc(rideDetails.r.RideDate,
              TimeZoneInfo.FindSystemTimeZoneById(Constant.TimeZone.India)).ToString("dd-MMM-yyyy");
                mailDetails.RideTime = TimeZoneInfo.ConvertTimeFromUtc(rideDetails.r.RideTime,
              TimeZoneInfo.FindSystemTimeZoneById(Constant.TimeZone.India)).ToString("hh:mm tt");
                mailDetails.RideFrom = rideDetails.r.From;
                mailDetails.RideTo = rideDetails.r.To;
                mailDetails.ExternalCabName = rideDetails.r.ExternalCabName;
                mailDetails.ContactNo = rideDetails.r.ContactNo;
                mailDetails.RideType = rideDetails.rideType;
                if (rideDetails.r.RideAssignmentId != null)
                {
                    var rideAssigmentDetails = (from a in cabODbContext.RideAssignment
                                                join r in cabODbContext.Ride on a.Id equals r.RideAssignmentId
                                                join d in cabODbContext.Driver on a.DriverId equals d.Id
                                                join c in cabODbContext.Cab on a.CabId equals c.Id
                                                where a.Id == rideDetails.r.RideAssignmentId
                                                select new
                                                {
                                                    d.Name,
                                                    d.PhoneNo,
                                                    c.Model
                                                }
                                                ).FirstOrDefault();
                    mailDetails.DriverName = rideAssigmentDetails.Name;
                    mailDetails.DriverPhone = rideAssigmentDetails.PhoneNo;
                    mailDetails.CabName = rideAssigmentDetails.Model;
                }
                return mailDetails;
            }
            catch(Exception e)
            {
                throw e;
            }
        } 
        public bool updateInitialReading(int id , long initial)
        {
            try
            {
                var firstReading = cabODbContext.Ride.Where(x => x.Id == id).FirstOrDefault();
                firstReading.InitialReading = initial;
                cabODbContext.SaveChanges();
                return true;
            }
            catch(Exception e) 
            {
                throw e;
            }
        }
        public bool updateFinalReading(int id ,long final)
        {
            try
            {
                var completedId = cabODbContext.RideStatus.Where(x => x.StatusName == RideStatuses.Completed.ToString()).Select(x => x.Id).FirstOrDefault();
                var ride = cabODbContext.Ride.Where(x => x.Id == id).FirstOrDefault();
                ride.FinalReading = final;
                ride.RideStatusId = completedId;
                cabODbContext.SaveChanges();
                return true;
            }
            catch(Exception e) 
            {
                throw e;
            }
        }
        public ICollection<AssignedOffices> GetAssignedOffices()
        {
            try
            {
                var locations1 = cabODbContext.OfficeCommutation.Where(x => x.IsDeleted == false)
                    .Join(cabODbContext.OfficeLocation.Where(x => x.IsDeleted == false), oc => oc.Location1, ol => ol.Id, (oc, ol) => new AssignedOffices { OfficeId = ol.Id, OfficeAddress = ol.Address });
                var officeIds = locations1.Select(x => x.OfficeId).ToArray();
                var locations2 = cabODbContext.OfficeCommutation.Where(x => x.IsDeleted == false)
                    .Join(cabODbContext.OfficeLocation.Where(x => x.IsDeleted == false), oc => oc.Location2, ol => ol.Id, (oc, ol) => new AssignedOffices { OfficeId = ol.Id, OfficeAddress = ol.Address }).Where(x => !officeIds.Contains(x.OfficeId));
                var assignedOffices = locations1.Union(locations2);
                return assignedOffices.Distinct().ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public ICollection<AssignedOffices> GetCorrespondingOffices(int officeId)
        {
            try
            {
                var locations1 = cabODbContext.OfficeCommutation.Where(x => x.IsDeleted == false && x.Location1 == officeId).Select(x => x.Location2)
                    .Join(cabODbContext.OfficeLocation.Where(x => x.IsDeleted == false), oc => oc, ol => ol.Id, (oc, ol) => new AssignedOffices { OfficeId = ol.Id, OfficeAddress = ol.Address });
                var officeIds = locations1.Select(x => x.OfficeId).ToArray();
                var locations2 = cabODbContext.OfficeCommutation.Where(x => x.IsDeleted == false && x.Location2 == officeId).Select(x => x.Location1)
                    .Join(cabODbContext.OfficeLocation.Where(x => x.IsDeleted == false), oc => oc, ol => ol.Id, (oc, ol) => new AssignedOffices { OfficeId = ol.Id, OfficeAddress = ol.Address }).Where(x => !officeIds.Contains(x.OfficeId));
                var correspondingOffices = locations1.Union(locations2).Distinct().ToList();
                return correspondingOffices;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public OfficeRideDriverInfo CheckTime(CheckTimeInfo rideInfo)
        {
            try
            {
                var tempContext = new CabODbContext();
                var driverInfo = new OfficeRideDriverInfo();
                DateTime rideTime = TimeZoneInfo.ConvertTimeFromUtc(rideInfo.rideTime,
                 TimeZoneInfo.FindSystemTimeZoneById(Constant.TimeZone.India));
                var ride = tempContext.OfficeCommutation.Where(o => o.IsDeleted == false && o.Location1 == rideInfo.location1 && o.Location2 == rideInfo.location2).FirstOrDefault();
                if (ride == null)
                {
                    ride = tempContext.OfficeCommutation.Where(o => o.IsDeleted == false && o.Location2 == rideInfo.location1 && o.Location1 == rideInfo.location2).FirstOrDefault();
                }
                var availableTimes = tempContext.AvailableTime.Where(a => a.IsDeleted == false && a.OfficeCommutationId == ride.Id).OrderBy(x => x.StartTime).ToList();
                int timeOk = 0;
                if (availableTimes.Count != 0)
                {
                    int flag = 0;
                    foreach(var time in availableTimes)
                    {
                        if(rideTime.TimeOfDay >= TimeZoneInfo.ConvertTimeFromUtc(time.StartTime,
                 TimeZoneInfo.FindSystemTimeZoneById(Constant.TimeZone.India)).TimeOfDay && rideTime.TimeOfDay < TimeZoneInfo.ConvertTimeFromUtc(time.EndTime,
                 TimeZoneInfo.FindSystemTimeZoneById(Constant.TimeZone.India)).TimeOfDay)
                        {
                            flag = 1;
                            break;
                        }
                    }
                    if (flag != 1)
                    {
                        timeOk = 1;
                    }
                }
                else
                {
                    timeOk = 1;
                }
                if(timeOk == 1)
                {
                    var shiftId = getShiftId(rideInfo.rideTime);
                    var assignment = tempContext.RideAssignment.Where(r => r.IsDeleted == false && r.DailyDate.Date == rideInfo.rideDate.Date && r.CabId == ride.CabId && r.ShiftId == shiftId).FirstOrDefault();
                    if (assignment != null)
                    {
                        var cabDetails = tempContext.Cab.Where(c => c.Id == ride.CabId).FirstOrDefault();
                        var driverDetails = tempContext.Driver.Where(d => d.Id == assignment.DriverId).FirstOrDefault();
                        driverInfo.driverName = driverDetails.Name;
                        driverInfo.driverPhone = driverDetails.PhoneNo;
                        driverInfo.driverId = driverDetails.Id;
                        driverInfo.cabId = cabDetails.Id;
                        driverInfo.cabName = cabDetails.Model;
                        driverInfo.vehicleNo = cabDetails.VehicleNo;
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                tempContext.Dispose();
                if (driverInfo.cabName == null) {
                    return null;
                }
                else
                {
                    return driverInfo;
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public ICollection<AvailableTimeCab> GetAvailableTimes(int location1, int location2)
        { 
            try
            {
                var availableTimes = new List<AvailableTimeCab>();
                var ride = cabODbContext.OfficeCommutation.Where(o => o.IsDeleted == false && o.Location1 == location1 && o.Location2 == location2).FirstOrDefault();
                if (ride == null)
                {
                    ride = cabODbContext.OfficeCommutation.Where(o => o.IsDeleted == false && o.Location2 == location1 && o.Location1 == location2).FirstOrDefault();
                }
                var times = cabODbContext.AvailableTime.Where(a => a.IsDeleted == false && a.OfficeCommutationId == ride.Id).ToList();
                foreach (var time in times)
                {
                    string temp = "";
                    DateTime sTime = TimeZoneInfo.ConvertTimeFromUtc(time.StartTime,
                 TimeZoneInfo.FindSystemTimeZoneById(Constant.TimeZone.India));
                    temp += sTime.ToString("hh:mm tt");
                    temp += " to ";
                    DateTime eTime = TimeZoneInfo.ConvertTimeFromUtc(time.EndTime,
                 TimeZoneInfo.FindSystemTimeZoneById(Constant.TimeZone.India));
                    temp += eTime.ToString("hh:mm tt");
                    AvailableTimeCab tempObj = new AvailableTimeCab
                    {
                        availableTime = temp
                    };
                    availableTimes.Add(tempObj);
                }
                availableTimes.OrderBy(x => x.availableTime);
                return availableTimes;
            }
            catch(Exception e)
            {
                throw e;
            }
            
        }
        public void sendWhatsapp(string msg, string phoneNumber)
        {
            try
            {
                var twilioWhatsAppNo = Constant.Default.WhatsappPrefix + twilioNo;
                var recipientPhone = Constant.Default.WhatsappPrefix + Constant.PhoneCode.India + phoneNumber;
                var message = MessageResource.Create(
                body: msg,
                from: new Twilio.Types.PhoneNumber(twilioWhatsAppNo),
                to: new Twilio.Types.PhoneNumber(recipientPhone)
                );
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public CompletedRideInfo GetCompletedRideInfo(int id)
        {
            try
            {
                return cabODbContext.Ride.Where(r => r.Id == id)
                            .Join(cabODbContext.RideAssignment, r => r.RideAssignmentId, ra => ra.Id, (r, ra) => new { rideId = r.Id, driverId = ra.DriverId, cabId = ra.CabId })
                            .Join(cabODbContext.Driver, r => r.driverId, d => d.Id, (r, d) => new { r, d.Name })
                            .Join(cabODbContext.Cab, r => r.r.cabId, c => c.Id, (r, c) => new { r.r.rideId, r.Name, c.Model })
                            .GroupJoin(cabODbContext.Rating, r => r.rideId, rat => rat.RideId, (r, rat) => new { r, rat })
                            .SelectMany(r => r.rat.DefaultIfEmpty(), (r, rat) => new { r.r, rat })
                            .Select(x => new CompletedRideInfo
                            {
                                DriverName = x.r.Name,
                                CabName = x.r.Model,
                                Timing = Convert.ToSingle(x.rat.Timing),
                                Behaviour = Convert.ToSingle(x.rat.Behaviour),
                                Overall = Convert.ToSingle(x.rat.Overall),
                                Comments = x.rat.Comments
                            }).FirstOrDefault();
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public bool CompleteRide(int rideId)
        {
            try
            {
                int statusId = getStatusId(RideStatuses.Completed.ToString());
                var result = (from r in cabODbContext.Ride
                              where r.Id == rideId
                              select r).SingleOrDefault();
                result.RideStatusId = statusId;
                cabODbContext.SaveChanges();
                return true;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public OfficeRideDriverInfo getCabDriverInfo(int rideId)
        {
            try
            {
                var tempContext = new CabODbContext();
                var cabDriverInfo = tempContext.Ride
                                       .Join(tempContext.RideAssignment, r => r.RideAssignmentId, ra => ra.Id, (r, ra) => new { ra })
                                       .Join(tempContext.Cab, ra => ra.ra.CabId, c => c.Id, (ra, c) => new { ra.ra, c.VehicleNo, c.Model, cabId = c.Id })
                                       .Join(tempContext.Driver, ra => ra.ra.DriverId, d => d.Id, (ra, d) => new { ra.Model, ra.VehicleNo, d.Name, d.PhoneNo, ra.cabId, driverId = d.Id })
                                       .Select(x => new OfficeRideDriverInfo { 
                                          cabId = x.cabId,
                                          driverId = x.driverId,
                                          cabName = x.Model,
                                          driverName = x.Name,
                                          vehicleNo = x.VehicleNo,
                                          driverPhone = x.PhoneNo
                                       }).FirstOrDefault();
                tempContext.Dispose();
                if(cabDriverInfo.driverName != null)
                {
                    return cabDriverInfo;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}