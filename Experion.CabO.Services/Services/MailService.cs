using Experion.CabO.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using Experion.CabO.Data.Constant;
using Experion.CabO.Data.Enums;

namespace Experion.CabO.Services.Services
{
    public class MailService
    {
        public MailService(string when)
        {
            sendMailCompletedRides(when);
        }
        public void sendMailCompletedRides(string when)
        {
            var cabODbContext = new CabODbContext();
            if (when == "evening")
            {
                var completedRides = cabODbContext.Ride
                               .Join(cabODbContext.RideStatus, r => r.RideStatusId, rs => rs.Id, (r, rs) => new { r, rs.StatusName })
                               .Where(x => ((x.StatusName == RideStatuses.Completed.ToString())
                               || (x.StatusName == RideStatuses.Approved.ToString() && x.r.CabType == CabTypes.External.ToString()))
                               && x.r.RideDate.Date >= DateTime.UtcNow.AddDays(-1).Date).ToList();
                foreach (var ride in completedRides)
                {
                    if (ride.r.RideDate.Date == DateTime.UtcNow.Date
                                && ride.r.RideTime.TimeOfDay <= DateTime.UtcNow.TimeOfDay.Subtract(new TimeSpan(00, 15, 0))
                                && ride.r.RideTime.TimeOfDay >= new DateTime(0001, 01, 01, 02, 15, 00).TimeOfDay)
                    {
                        sendMail(ride.r.Id);
                    }
                }
            }
            else
            {
                var completedRides = cabODbContext.Ride
                                .Join(cabODbContext.RideStatus, r => r.RideStatusId, rs => rs.Id, (r, rs) => new { r, rs.StatusName })
                                .Where(x => ((x.StatusName == RideStatuses.Completed.ToString())
                                || (x.StatusName == RideStatuses.Approved.ToString() && x.r.CabType == CabTypes.External.ToString()))
                                && x.r.RideDate.Date >= DateTime.UtcNow.AddDays(-2).Date).ToList();
                foreach (var ride in completedRides)
                {
                    if ((ride.r.RideDate.Date == DateTime.UtcNow.Date
                && ride.r.RideTime.TimeOfDay < DateTime.UtcNow.TimeOfDay.Subtract(new TimeSpan(00, 15, 0)))
                || (ride.r.RideDate.Date == DateTime.UtcNow.AddDays(-1).Date
                && ride.r.RideTime.TimeOfDay >= new DateTime(0001, 01, 01, 14, 15, 00).TimeOfDay))
                    {
                        sendMail(ride.r.Id);
                    }
                }
            }
            cabODbContext.Dispose();
        }
        public void sendMail(int rideId)
        {
            var cabODbContext = new CabODbContext();
            var rideDetails = cabODbContext.Ride.Where(r => r.Id == rideId)
                                .Join(cabODbContext.RideRequestor, r => r.RideRequestorId, rr => rr.Id, (r, rr) => new { r, rr })
                                .FirstOrDefault();
                string when;
                if (TimeZoneInfo.ConvertTimeFromUtc(rideDetails.r.RideDate,
                    TimeZoneInfo.FindSystemTimeZoneById(Constant.TimeZone.India)).Date < DateTime.UtcNow.Date)
                {
                    when = "last day";
                }
                else
                {
                    when = "today";
                }
                MailMessage mailMsg = new MailMessage();
                mailMsg.From = new MailAddress(Constant.MailCredentials.MailId);
                mailMsg.To.Add(rideDetails.rr.Email);
                mailMsg.Body = "Hi " + rideDetails.rr.UserName + ", <br>";
                mailMsg.Body += "How was your ride with Cab'O " + when + ", from " + rideDetails.r.From.Split(",")[0];
                mailMsg.Body += " to " + rideDetails.r.To.Split(",")[0] + " at " + TimeZoneInfo.ConvertTimeFromUtc(rideDetails.r.RideTime,
                    TimeZoneInfo.FindSystemTimeZoneById(Constant.TimeZone.India)).ToString("hh:mm tt");
                mailMsg.Body += "? Please rate your experience at " + "http://cabo-portal.s3-website.us-east-2.amazonaws.com/rating/" + rideDetails.r.Guid + ".";
                mailMsg.IsBodyHtml = true;
                mailMsg.Subject = "How was your ride with Cab'O " + when + "?";
                SmtpClient smt = new SmtpClient("smtp.gmail.com");
                smt.Port = 587;
                smt.EnableSsl = true;
                smt.UseDefaultCredentials = false;
                smt.Credentials = new NetworkCredential(Constant.MailCredentials.MailId, Constant.MailCredentials.MailPassword);
                smt.Send(mailMsg);
                cabODbContext.Dispose();
            }
    }
}
