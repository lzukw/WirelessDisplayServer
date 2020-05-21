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

            // First read configuration-strings from appsettings.json (these settings can be over-
            // written by command-line-arguments or environment-variables).
            string pathToVncViewer = Configuration["PathToTightVncViewer"];
            string vncViewerArgs = Configuration["VncViewerArgs"];
            string pathToFFplay = Configuration["PathToFFplay"];
            string ffplayArgs = Configuration["FFplayArgs"];
            string pathToScreeRes = Configuration["PathToScreenRes"];

            //Check, if executeables are available:
            Console.WriteLine($"Current working-directory is: '{System.IO.Directory.GetCurrentDirectory()}'");
            if (!System.IO.File.Exists(pathToVncViewer))
            {
                throw new ArgumentException($"Can't find VNC-Viewer-Executable at '{pathToVncViewer}'");
            }
            if (!System.IO.File.Exists(pathToFFplay))
            {
                throw new ArgumentException($"Can't find FFplay-Executable at '{pathToFFplay}'");
            }
            if (!System.IO.File.Exists(pathToScreeRes))
            {
                throw new ArgumentException($"Can't find ScreenRes-Executable at '{pathToScreeRes}'");
            }

            // Now create the singletons used for dependency-injection.
            services.AddSingleton<IStreamPlayerService>((s) =>
            {
                var logger = s.GetRequiredService<ILogger<StreamPlayerService>>();
                return new StreamPlayerService(logger, pathToVncViewer, vncViewerArgs, pathToFFplay, ffplayArgs);
            });
            services.AddSingleton<IScreenResolutionService>((s) =>
            {
                var logger = s.GetRequiredService<ILogger<ScreenResolutionService>>();
                return new ScreenResolutionService(logger, pathToScreeRes);
            });

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
