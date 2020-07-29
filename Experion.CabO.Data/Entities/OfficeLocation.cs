using System.ComponentModel.DataAnnotations;

namespace Experion.CabO.Data.Entities
{
    public class OfficeLocation
    {
        [Key]
        public int Id { get; set; }
        public string Address { get; set; }
        public bool IsDeleted { get; set; }
    }
}
