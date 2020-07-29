using Experion.CabO.Data.Entities;
using Experion.CabO.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Experion.CabO.Services.Services
{
   public class RideAssignmentService: IRideAssignmentService
    {
        private CabODbContext cabODbContext;
        public RideAssignmentService(CabODbContext _cabODbContext)
        {
            cabODbContext = _cabODbContext;
        }
        public int AddRide(RideAssignmentDto rideDto)
        {
            try
            {
                int value = 0;
                var days = rideDto.Days;
                int i = 0;
                var rideList = cabODbContext.RideAssignment;
                List<RideAssignment> store = new List<RideAssignment>();
                while (i<days)
                {
                    
                    var ddate = rideDto.DailyDate.AddDays(i);
                    
                    if ((!cabODbContext.RideAssignment.Any(x => x.ShiftId == Convert.ToInt32(rideDto.ShiftId) &&
                               x.DailyDate.Date == ddate.Date
                               && x.IsDeleted == false && (x.DriverId == Convert.ToInt32(rideDto.DriverId) || x.CabId == Convert.ToInt32(rideDto.CabId)))))
                    {
                        var rides = new RideAssignment
                        {
                            DailyDate = rideDto.DailyDate.AddDays(i),
                            DriverId = Int32.Parse(rideDto.DriverId),
                            CabId = Int32.Parse(rideDto.CabId),
                            ShiftId = Int32.Parse(rideDto.ShiftId),
                            IsDeleted = false
                        };
                        store.Add(rides);
                        value = 1;
                    }
                    else
                    {
                        value = 0;
                    }
                    i++;
                }
                cabODbContext.RideAssignment.AddRange(store);
                cabODbContext.SaveChanges();
                return value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<RideDisplayDto> ViewAssignment()
        {
            try
            {
                var rideList = cabODbContext.RideAssignment.Where(x => x.IsDeleted == false && x.DailyDate >= DateTime.UtcNow.Date)
                    .Join(cabODbContext.Driver, r => r.DriverId, d => d.Id, (r,d) => new {r, d.Name})
                    .Join(cabODbContext.Cab, rd => rd.r.CabId, c => c.Id, (rd,c)=> new { rd, c.Model })
                    .Join(cabODbContext.Shift, rdc=> rdc.rd.r.ShiftId, s => s.Id, (rdc,s)=> new { rdc, s.ShiftName})
                    .Select(x => new RideDisplayDto {
                        RideId = x.rdc.rd.r.Id,
                        DriverId = x.rdc.rd.r.DriverId,
                        CabId = x.rdc.rd.r.CabId,
                        ShiftId = x.rdc.rd.r.ShiftId,
                        Date = x.rdc.rd.r.DailyDate,
                        DriverName = x.rdc.rd.Name,
                        CabName = x.rdc.Model,
                        ShiftName = x.ShiftName
                    }).OrderBy(x=> x.Date).ToList();
                return rideList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public int UpdateRide(int updateId, RideAssignmentDto updateRide)
        {
            try
            {
                var result = cabODbContext.RideAssignment.FirstOrDefault(x => x.Id == updateId);
                result.CabId = Int32.Parse(updateRide.CabId);
                result.DriverId = Int32.Parse(updateRide.DriverId);
                cabODbContext.SaveChanges();
                return 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int DeleteRide(int id)
        {
            try
            {
                var row = cabODbContext.RideAssignment.FirstOrDefault(x => x.Id == id);
                row.IsDeleted = true;
                cabODbContext.SaveChanges();
                return 1;
            }
            catch(Exception ex)
            { 
                throw ex;
            }
        }

        public IEnumerable<DriverDto> GetDrivers()
        {
            try
            {
                var drivers = cabODbContext.Driver.Where(x => x.IsDeleted == false)
                    .Select(x =>
                new DriverDto
                {
                    Id = x.Id,
                    Name = x.Name,
                })
                    .ToList();
                return drivers;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public IEnumerable<CabDetailDto> GetCabs()
        {
            try
            {
                var cabList = cabODbContext.Cab.Where(x => x.IsDeleted == false)
                    .Select(x =>
                   new CabDetailDto
                   {
                       Id = x.Id,
                       Model = x.Model,
                   }).ToList();
                    return cabList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public IEnumerable<ShiftDto> GetShifts()
        {
            try
            {
                var shiftList = cabODbContext.Shift
                    .Select(x =>
                new ShiftDto
                {
                    Id = x.Id,
                    ShiftName = x.ShiftName,
                }).ToList();
                return shiftList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
