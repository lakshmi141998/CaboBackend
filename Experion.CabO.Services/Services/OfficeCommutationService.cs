using Experion.CabO.Data.Entities;
using Experion.CabO.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Experion.CabO.Services.Services
{
    public class OfficeCommutationService:IOfficeCommutationService
    {
        private CabODbContext cabODbContext;

        public OfficeCommutationService(CabODbContext _cabODbContext)
        {
            this.cabODbContext = _cabODbContext;
        }
        public int AddOfficeCommutation(OfficeCommutationDto officeDto)
        {
            try
            {
                int insertedId = 0;
                if(!cabODbContext.OfficeCommutation.Any(x => x.Location1 == Int32.Parse(officeDto.Location1) && x.Location2 == Int32.Parse(officeDto.Location2) && x.IsDeleted == false))
                {
                    var commutation = new OfficeCommutation
                    {
                        Location1 = Int32.Parse(officeDto.Location1),
                        Location2 = Int32.Parse(officeDto.Location2),
                        CabId = Int32.Parse(officeDto.CabId),
                        IsDeleted = false
                    };
                    cabODbContext.OfficeCommutation.Add(commutation);
                    cabODbContext.SaveChanges();
                    insertedId = commutation.Id;
                }
                    return insertedId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<OfficeDisplayDto> ViewOfficeCommutation()
        {
            try
            {
                var display = new List<OfficeDisplayDto>();
                var displayList = cabODbContext.OfficeCommutation.Where(x => x.IsDeleted == false).ToList();
                foreach (OfficeCommutation value in displayList)
                {
                    var displayTime = new List<AvailableTimeDto>();
                    var timings = cabODbContext.AvailableTime.Where(x => x.OfficeCommutationId == value.Id).ToList();
                    var cab = cabODbContext.Cab.Where(x => x.Id == value.CabId).FirstOrDefault();
                    var location1 = cabODbContext.OfficeLocation.Where(x => x.Id == value.Location1).FirstOrDefault();
                    var location2 = cabODbContext.OfficeLocation.Where(x => x.Id == value.Location2).FirstOrDefault();
                    foreach (AvailableTime val in timings)
                    {
                        var t = new AvailableTimeDto
                        {
                            StartTime = getIst(val.StartTime),
                            EndTime = getIst(val.EndTime)
                        };
                        displayTime.Add(t);
                    }
                    var commutationDetail = new OfficeDisplayDto
                    {
                        Location1 = location1.Address,
                        Location2 = location2.Address,
                        Location1Id = value.Location1,
                        Location2Id = value.Location2,
                        Id = value.Id,
                        CabName = cab.Model,
                        CabId = value.CabId,
                        Times = displayTime
                    };
                        display.Add(commutationDetail);
                }
                return display;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static DateTime getIst(DateTime date)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(date, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
        }
        public int UpdateCommutation(int updateId, OfficeCommutationDto update)
        {
            try
            {
                var result = cabODbContext.OfficeCommutation.Where(x => x.Id == updateId).FirstOrDefault();
                result.CabId = Int32.Parse(update.CabId);
                cabODbContext.SaveChanges();
                return 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int DeleteCommutation(int id)
        {
            try
            {
                var row = cabODbContext.OfficeCommutation.Where(x => x.Id == id).FirstOrDefault();
                row.IsDeleted = true;
                cabODbContext.SaveChanges();
                return id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int AddTimings(AvailableTimeDto timing)
        {
            try
            { 
            int insertedId = 0;
            var stime = new DateTime(01, 01, 01, timing.StartTime.Hour, timing.StartTime.Minute, timing.StartTime.Second);
            var etime = new DateTime(01, 01, 01, timing.EndTime.Hour, timing.EndTime.Minute, timing.EndTime.Second);
            if (!cabODbContext.AvailableTime.Any(x => (x.OfficeCommutationId == timing.OfficeCommutationId) && (x.IsDeleted == false) && (x.StartTime == timing.StartTime) && (x.EndTime == timing.EndTime)))
            {
                var commutation = new AvailableTime
                {
                    OfficeCommutationId = timing.OfficeCommutationId,
                    StartTime = stime,
                    EndTime = etime,
                    IsDeleted = false
                };
                cabODbContext.AvailableTime.Add(commutation);
                cabODbContext.SaveChanges();
                insertedId = commutation.Id;
            }
            return insertedId;
        }
            catch (Exception ex)
            {
                throw ex;
            }
        }
                                                 
        public IEnumerable<AvailableTimeDto> ViewTiming(int Id)
        {
            try
            {
                var display = new List<AvailableTimeDto>();

                var displayList = cabODbContext.AvailableTime.Where(x => x.OfficeCommutationId == Id && x.IsDeleted == false).ToList();

                foreach (AvailableTime value in displayList)
                {
                        var timingDetail = new AvailableTimeDto
                        {
                            Id = value.Id,
                            StartTime = getIst(value.StartTime),
                            EndTime = getIst(value.EndTime)

                        };

                        display.Add(timingDetail);

                }
                return display;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public int DeleteTiming(int id)
        {
            try
            {
                var row = cabODbContext.AvailableTime.Where(x => x.OfficeCommutationId == id).ToList();
                foreach(AvailableTime value in row)
                {
                    value.IsDeleted = true; 
                }
                cabODbContext.SaveChanges();
                return id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        }
    }

