using Experion.CabO.Data.Entities;
using Experion.CabO.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Experion.CabO.Services.Services
{
    public class OfficeLocationService : IOfficeLocationService
    {
        CabODbContext _context;
        public OfficeLocationService(CabODbContext _context)
        {
            this._context = _context;
        }
        public int AddLocation(OfficeLocationDto location)
        {
            try
            {
                int insertedId = 0;            
                if (!_context.OfficeLocation.Any(r => r.Address == location.Address && r.IsDeleted == false))
                    {
                    var office_location = new OfficeLocation
                    {
                        Address = location.Address
                    };
                    _context.OfficeLocation.Add(office_location);
                    _context.SaveChanges();
                    insertedId = office_location.Id;
                }
                return insertedId;
            }

            catch
            {
             return 0;
            }
        }

        public IEnumerable<OfficeLocation> GetLocations()
        {
            try
            {
               /* var response = (from r in _context.OfficeLocation
                                where r.IsDeleted == false
                                select r.Address).ToList(); */
                var response = _context.OfficeLocation.Where(x => x.IsDeleted == false);
                return response;
            }

            catch
            {
                throw;
            }
        }

        public int DeleteLocation (int id)
        {
            try
            {
                var response = _context.OfficeLocation.Where(x => x.Id == id).SingleOrDefault();
                response.IsDeleted = true;
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
