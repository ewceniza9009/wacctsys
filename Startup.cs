using System.IO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webfmis.Data;
using Telerik.Reporting.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Telerik.Reporting.Cache.File;

namespace webfmis
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Config.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMvc();

            services.TryAddSingleton<IReportServiceConfiguration>(sp => new ReportServiceConfiguration
            {
                ReportingEngineConfiguration = sp.GetService<IConfiguration>(),
                HostAppId = "Net5RestServiceWithCors",
                Storage = new FileStorage(),
                ReportSourceResolver = new UriReportSourceResolver(
                    System.IO.Path.Combine(GetReportsDir(sp)))
            });
            // Add framework services.
            services
                .AddControllersWithViews()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                // Maintain property names during serialization. See:
                // https://github.com/aspnet/Announcements/issues/194
                .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver())
                .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            // Add Kendo UI services to the services container
            services.AddKendo();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddRazorPages();

            services.AddControllers().AddNewtonsoftJson();

            services.TryAddSingleton<IReportServiceConfiguration>(sp =>
            new ReportServiceConfiguration
            {
                ReportingEngineConfiguration = ResolveConfiguration(sp.GetService<IWebHostEnvironment>()),
                HostAppId = "ReportingCore3App",
                Storage = new FileStorage(),
                ReportSourceResolver = new UriReportSourceResolver(
                    System.IO.Path.Combine(sp.GetService<IWebHostEnvironment>().ContentRootPath, "Reports"))
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }

        public static IConfiguration ResolveConfiguration(IWebHostEnvironment environment)
        {
            var wasAppSettings = System.IO.Path.Combine(environment.ContentRootPath, "wasAppSettings.json");

            return new ConfigurationBuilder()
                .AddJsonFile(wasAppSettings, true)
                .Build();
        }

        static string GetReportsDir(IServiceProvider sp)
        {
            return Path.Combine(sp.GetService<IWebHostEnvironment>().ContentRootPath, "Reports");
        }
    }
}
