using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Experion.CabO.Data.Entities
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("RideId")]
        public virtual Ride Ride { get; set; }
        public int? RideId { get; set; }
        public float? Timing { get; set; }
        public float? Behaviour { get; set; }
        public float? Overall { get; set; }
        public string Comments { get; set; }
    }
}
