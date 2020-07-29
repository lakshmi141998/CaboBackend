using Experion.CabO.Services.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Experion.CabO.ioc
{
    public class DependencyConfig
    {
        public static void RegisterConfig(IServiceCollection services)
        {
            services.AddTransient<IOfficeLocationService, OfficeLocationService>();
            services.AddTransient<IRideService, RideService>();
            services.AddTransient<IRideAssignmentService, RideAssignmentService>();
            services.AddTransient<ILoginService, LoginService>();
            services.AddTransient<ICabService, CabService>();
            services.AddTransient<IRideListingService, RideListingService>();
            services.AddTransient<IReportService, ReportService>();
            services.AddTransient<IDrivers, Drivers>();
            services.AddTransient<IOfficeCommutationService, OfficeCommutationService>();
            services.AddTransient<TtsApiService, TtsApiService>();
            services.AddTransient<IRatingService, RatingService>();
            services.AddTransient<IDashboardService, DashboardService>();
        }
    }
}
