using Experion.CabO.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Experion.CabO.Services.Services
{
    public interface IRideService
    {
        bool updateInitialReading(int id , long initial); 
        bool updateFinalReading(int id , long final); 
        bool ApproveRide(int rideId, int cabId, string cabType, string externalCabName);
        bool CompleteRide(int rideId);
        bool ApproveRideByDriver(int rideId);
        bool RejectRide(int rideId, string cancelReason);
        bool RejectRideByDriver(int rideId);
        ICollection<CabListDto> GetCabList(DateTime rideDate, DateTime rideTime);
        int AddRide(EmployeeRideEditDto ride);
        Task<Object> ProjectsGet(int userId);
        bool CancelRide(int rideId);
        int RideAssignmenId(int driverId);
        ICollection<AssignedOffices> GetAssignedOffices();
        ICollection<AssignedOffices> GetCorrespondingOffices(int officeId);
        OfficeRideDriverInfo CheckTime(CheckTimeInfo rideInfo);
        ICollection<AvailableTimeCab> GetAvailableTimes(int location1, int location2);
        bool sendMailRideAdded(int rideId);
        bool sendMail(int rideId, string status);
        CompletedRideInfo GetCompletedRideInfo(int id);
        OfficeRideDriverInfo getCabDriverInfo(int rideId);
    }
}
