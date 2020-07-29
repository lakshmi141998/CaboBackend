using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Experion.CabO.Data.Entities
{
    public class Ride
    {
        [Key]
        public int Id { get; set; }
        public DateTime RideDate { get; set; }
        public DateTime RideTime { get; set; }

        [ForeignKey("RideRequestorId")]
        public virtual RideRequestor RideRequestor { get; set; }
        public int RideRequestorId { get; set; }
        public string From { get; set; }
        public string To { get; set; }

        [ForeignKey("RideAssignmentId")]
        public virtual RideAssignment RideAssignment { get; set; }
        public int? RideAssignmentId { get; set; }
        public long? InitialReading { get; set; }
        public long? FinalReading { get; set; }
        public string ContactNo { get; set; }
        public int PassengerCount { get; set; }
        public string CabType { get; set; }
        public string Purpose { get; set; }
        public string ProjectCode { get; set; }
        public string ExternalCabName { get; set; }
        public string CancelReason  { get; set; }

        [ForeignKey("RideStatusId")]
        public virtual RideStatus RideStatus { get; set; }
        public int? RideStatusId { get; set; }

        [ForeignKey("RideTypeId")]
        public virtual RideType RideType { get; set; }
        public int? RideTypeId { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<Rating> Rating { get; set; }
        public string Guid { get; set; }
    }
}
