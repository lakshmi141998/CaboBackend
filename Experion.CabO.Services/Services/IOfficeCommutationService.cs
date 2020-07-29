using Experion.CabO.Services.DTOs;
using System.Collections.Generic;

namespace Experion.CabO.Services.Services
{
    public interface IOfficeCommutationService
    {
        int AddOfficeCommutation(OfficeCommutationDto officeDto);
        IEnumerable<OfficeDisplayDto> ViewOfficeCommutation();
        int UpdateCommutation(int updateId, OfficeCommutationDto update);
        int DeleteCommutation(int id);
        int AddTimings(AvailableTimeDto timing);
        IEnumerable<AvailableTimeDto> ViewTiming(int Id);
        int DeleteTiming(int id);
    }
}
