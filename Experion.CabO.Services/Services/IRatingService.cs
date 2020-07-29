using Experion.CabO.Services.DTOs;

namespace Experion.CabO.Services.Services
{
    public interface IRatingService
    {
        bool checkRating(string rideId);
        bool addRating(RatingInfo rating);
    }
}
