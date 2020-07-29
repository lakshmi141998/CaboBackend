using Experion.CabO.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Experion.CabO.Services.DTOs;

namespace Experion.CabO.Services.Services
{
    public class RatingService : IRatingService
    {
        private CabODbContext cabODbContext;

        public RatingService(CabODbContext cabODbContext)
        {
            this.cabODbContext = cabODbContext;
        }

        public bool addRating(RatingInfo rating)
        {
            try
            {
                var rideId = cabODbContext.Ride.FirstOrDefault(r => r.Guid == rating.rideId).Id;
                Rating ratingData = new Rating {
                    RideId = rideId,
                    Timing = rating.timing,
                    Behaviour = rating.behaviour,
                    Overall = rating.overall,
                    Comments = rating.comments
                };
                cabODbContext.Rating.Add(ratingData);
                cabODbContext.SaveChanges();
                return true;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public bool checkRating(string rideGuid)
        {
            try
            {
                if (!cabODbContext.Rating
                  .Join(cabODbContext.Ride, rt => rt.RideId, r => r.Id, (rt, r) => new { r.Guid })
                  .Any(x => x.Guid == rideGuid))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
