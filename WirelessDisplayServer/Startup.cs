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

            // First read configuration-strings from appsettings.json (these settings can be over-
            // written by command-line-arguments or environment-variables).
            string pathToVncViewer = Configuration["PathToTightVncViewer"];
            string vncViewerArgs = Configuration["VncViewerArgs"];
            string pathToFFplay = Configuration["PathToFFplay"];
            string ffplayArgs = Configuration["FFplayArgs"];
            string pathToScreeRes = Configuration["PathToScreenRes"];

            //Check, if executeables are available:
            Console.WriteLine($"Current working-directory is: '{System.IO.Directory.GetCurrentDirectory()}'");

            FileInfo vncViewerExecutable = new FileInfo(pathToVncViewer);
            if (!vncViewerExecutable.Exists)
            {
                throw new FileNotFoundException($"Can't find VNC-Viewer-Executable at '{vncViewerExecutable.FullName}'. Consider changing appsettings.json.");
            }

            FileInfo ffplayExecutable = new FileInfo(pathToFFplay);
            if (!ffplayExecutable.Exists)
            {
                throw new FileNotFoundException($"Can't find FFplay-Executable at '{ffplayExecutable.FullName}'. Consider changing appsettings.json.");
            }

            FileInfo screenresExecutable = new FileInfo(pathToScreeRes);
            if (!screenresExecutable.Exists)
            {
                throw new ArgumentException($"Can't find ScreenRes-Executable at '{screenresExecutable.FullName}'. Consider changing appsettings.json.");
            }

            // Now create the singletons used for dependency-injection.
            services.AddSingleton<IStreamPlayerService>((s) =>
            {
                var logger = s.GetRequiredService<ILogger<StreamPlayerService>>();
                return new StreamPlayerService(logger, 
                                               vncViewerExecutable.FullName, vncViewerArgs, 
                                               ffplayExecutable.FullName, ffplayArgs);
            });
            services.AddSingleton<IScreenResolutionService>((s) =>
            {
                var logger = s.GetRequiredService<ILogger<ScreenResolutionService>>();
                return new ScreenResolutionService(logger, screenresExecutable.FullName);
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
