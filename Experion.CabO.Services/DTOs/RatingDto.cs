using System.Collections.Generic;

namespace Experion.CabO.Services.DTOs
{
    public class RatingDto
    {
        public IEnumerable<float> Timing { get; set; }
        public float TimingTotal { get; set; }
        public IEnumerable<float> Behaviour { get; set; }
        public float BehaviourTotal { get; set; }
        public IEnumerable<float> Overall { get; set; }
        public float OverallTotal { get; set; }
        public int rideCount { get; set; }
    }
}
