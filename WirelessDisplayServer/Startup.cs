using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.IO;

using WirelessDisplayServer.Services;

namespace WirelessDisplayServer
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
            services.AddControllers();

            // ZUKW: added Dependency-Injection: Here a Singleton-Instane of StreamPlayerService and
            // another Singleton-Instance of ScreenResolutionServie are created. They will be passed
            // to the constructor of the webapi-Controllers.

            // First read configuration-strings from appsettings.json and put them into a 
            // NameValueCollection specificConfig, which is passed to the constructors of 
            // StreamPlayerService and ScreenResolutionServie.
            NameValueCollection specificConfig = new NameValueCollection();
            string usedOperatingSystem;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                usedOperatingSystem = "Linux";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                usedOperatingSystem = "macOS";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                usedOperatingSystem = "Windows";
            }
            else
            {
                throw new Exception("Operating System not supported");
            }

            var osSection = Configuration.GetSection(usedOperatingSystem);
            var osSectionChildren = osSection.Get<Dictionary<string,string>>();

            foreach (var keyValuePair in osSectionChildren)
            { 
                specificConfig[keyValuePair.Key] = keyValuePair.Value;
            }

            // Now create the singletons used for dependency-injection.
            services.AddSingleton<IStreamSinkService>((s) =>
            {
                var logger = s.GetRequiredService<ILogger<StreamSinkService>>();
                return new StreamSinkService(logger, specificConfig );
            });
            services.AddSingleton<IScreenResolutionService>((s) =>
            {
                var logger = s.GetRequiredService<ILogger<ScreenResolutionService>>();
                return new ScreenResolutionService(logger, specificConfig);
            });

            // For debugging purposes: Show current working directory, since 
            // script-paths are relative
            Console.WriteLine($"Current working-directory is: '{System.IO.Directory.GetCurrentDirectory()}'");

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
