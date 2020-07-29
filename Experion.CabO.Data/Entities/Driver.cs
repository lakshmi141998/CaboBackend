using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Experion.CabO.Data.Entities
{
    public class Driver
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNo { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<RideAssignment> RideAssignment { get; set; }
    }
}
