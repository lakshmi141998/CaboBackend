using System.ComponentModel.DataAnnotations;

namespace Experion.CabO.Data.Enums
{
    public enum CabTypes { Internal, External };
    public enum RideStatuses { Approved, Pending, Rejected, Completed, Cancelled };
    public enum RideTypes {[Display(Name = "CABO-OFFICE")] RideTypeOffice, [Display(Name = "CABO-CLIENT")] RideTypeClient, [Display(Name = "CABO-OTHERS")] RideTypeOthers, [Display(Name = "CABO-ADMIN")] RideTypeAdmin };
}
