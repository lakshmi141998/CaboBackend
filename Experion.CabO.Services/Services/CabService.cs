using Experion.CabO.Data.Entities;
using System;
using Experion.CabO.Services.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Experion.CabO.Services.Services
{
    public class CabService : ICabService
    {      
        CabODbContext _context;            
        public CabService(CabODbContext _context)
        {

            this._context = _context;
        }

        public int AddCab(cabdto cabdetails)
        {                            
            try
            {
                if (!_context.Cab.Any(r => r.VehicleNo == cabdetails.VehicleNo && r.IsDeleted == false))
                {
                     var CabDetail = new Cab
            {
                Capacity = Int32.Parse(cabdetails.Capacity),
                Model = cabdetails.Model,
                VehicleNo = cabdetails.VehicleNo
            };
                    _context.Cab.Add(CabDetail);
                    _context.SaveChanges();
                    
                }
                else
                {
                    throw new Exception();
                }

                return 1;

            }
            catch(Exception e)
            {
                throw  e;
            }
        }

        public IEnumerable<Cab> GetCabs()
        {
            try
            {
                var result = _context.Cab.Where(x => x.IsDeleted == false);
   
                return result;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public cabdto GetCabById(int id)
        {
            try
            {
                var result = (from c in _context.Cab
                              where c.Id == id
                              select c).SingleOrDefault();
                cabdto cab = new cabdto {
                    Model = result.Model,
                    VehicleNo = result.VehicleNo,
                    Capacity = result.Capacity.ToString()

                };
                return cab;

            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public int UpdateCab(int id, cabdto cabdetails)
        {
            try
            {
                if (!_context.Cab.Any(r => r.VehicleNo == cabdetails.VehicleNo && r.IsDeleted == false))
                {
                    var result = (from c in _context.Cab
                                  where c.Id == id
                                  select c).SingleOrDefault();
                    result.Model = cabdetails.Model;
                    result.VehicleNo = cabdetails.VehicleNo;
                    result.Capacity = Int32.Parse(cabdetails.Capacity);
                    _context.SaveChanges();
                    return 1;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }

            public int DeleteCab(int id)
        { 
                try
                {
                var result = _context.Cab.Where(x => x.Id == id).SingleOrDefault();

                result.IsDeleted = true;
                _context.SaveChanges();
                return 1;
                }
                catch(Exception e)
                {
                    throw e;
                }
            }
        }    
}




