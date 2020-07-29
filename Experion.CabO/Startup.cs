using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Experion.CabO.Data.Entities;
using Experion.CabO.ioc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.IO;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;

namespace Experion.CabO
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Services.Services.TaskScheduler.Instance.ScheduleTask(02, 30, 24,
            () =>
                {
                    new Services.Services.MailService("morning");
                });
            Services.Services.TaskScheduler.Instance.ScheduleTask(14, 30, 24,
            () =>
            {
                new Services.Services.MailService("evening");
            });
            services.AddControllers();
            DependencyConfig.RegisterConfig(services);
            services.AddCors(o => o.AddPolicy("CabOPolicy", builder =>
            {
                builder.AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowAnyOrigin();
            }));
            services.AddDbContext<CabODbContext>(options =>
                
            options.UseSqlServer(Configuration.GetConnectionString("SqlServerConnection")), ServiceLifetime.Transient);

            var key = "Experion.CabO,CabBookingSystemDevelepedByFreshersBatch";
            var symmetricToken = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidIssuer = "CabOAdmin",
                        ValidAudience = "CabOUser",
                        IssuerSigningKey = symmetricToken


                    };
                });
           

            services.AddSwaggerGen(c => {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "CabO",
                Description = "Cab Booking System"
            });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                 Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                 Enter 'Bearer' [space] and then your token in the text input below.
                 \r\n\r\nExample: 'Bearer ANCDDDDDDDDDDDDKDKJDKWJ'",
                 Name = "Authorization",
                 In = ParameterLocation.Header,
                 Type = SecuritySchemeType.ApiKey,
                 Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,

                    },
                    new List<string>()
                }
            });
         });
         services.AddControllers();
           
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            //var logPath = Configuration.GetValue<string>("LogPath");
            //loggerFactory.AddFile($"{logPath}/mylog-{{Date}}.txt", LogLevel.Information);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseRouting();
            app.UseCors("CabOPolicy");
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CabO API");
            });
        }

    }
}
