using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Experion.CabO.Data.Entities
{
    public class RideRequestor
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public virtual ICollection<Ride> Ride { get; set; }
    }
}
