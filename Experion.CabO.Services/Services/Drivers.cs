using Experion.CabO.Data.Entities;
using Experion.CabO.Services.DTOs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Experion.CabO.Services.Services
{
    public class Drivers : IDrivers
    {
        CabODbContext cabODbContext;
        private ILogger<IDrivers> _logger;

        public Drivers (CabODbContext cabODbContext)
        {
            this.cabODbContext = cabODbContext;
        }

        public string PostDriverDetails(DriverDetails dtls)
        {
            try
            {
                
                if (!cabODbContext.Driver.Any(r => r.PhoneNo == dtls.PhoneNo && r.IsDeleted == false))                  
                {
                    var user = new Driver
                    {
                        Name = dtls.Name,
                        PhoneNo = dtls.PhoneNo,
                        UserName = dtls.UserName,
                        Password = dtls.Password,
                        IsDeleted = dtls.IsDeleted
                    };
                    cabODbContext.Driver.Add(user);
                    cabODbContext.SaveChanges();
                    return "Posted";
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<Driver> GetDriverDetails()
        
        {
            try
            {
                var detail = cabODbContext.Driver.Where(x => x.IsDeleted == false).ToList();

                return detail;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Driver GetDriver(int id)
        {
            try
            {
                var detail = cabODbContext.Driver.Where(x => x.Id == id).FirstOrDefault();
                return detail;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public string UpdateDriverDetails(int id ,DriverDetails chng)
        {
            try
            {
                if (!cabODbContext.Driver.Any(r => r.PhoneNo == chng.PhoneNo && r.IsDeleted == false))        
                {

                    var update = cabODbContext.Driver.Where(x => x.Id == id).SingleOrDefault();
                    update.Name = chng.Name;
                    update.PhoneNo = chng.PhoneNo;
                    update.UserName = chng.UserName;
                    update.Password = chng.Password;

                    cabODbContext.SaveChanges();
                    return "Updated";
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string DeleteDriver(int id )
        {
            try
            {
                var change = cabODbContext.Driver.Where(x => x.Id == id).FirstOrDefault();
                change.IsDeleted = true;

                cabODbContext.SaveChanges();
                return "1";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
    }
}
