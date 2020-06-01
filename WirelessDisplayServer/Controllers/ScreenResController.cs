using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using WirelessDisplayServer.Services;


namespace WirelessDisplayServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScreenResController : ControllerBase
    {
        protected ILogger<ScreenResController> logger { get;  }
        protected IScreenResolutionService screenResolutionService { get; }

        public ScreenResController(ILogger<ScreenResController> logger, IScreenResolutionService screenResolutionService)
        {
            // logger and screenResolutionService are injected by Dependcy-Injection 
            // by "the runtime" (Program.cs and Startup.cs).
            this.logger = logger;
            this.screenResolutionService = screenResolutionService;
        }

        // GET: api/ScreenRes/AvailableScreenResolutions
        // from Linux (assuming IP-address of this server is 192.168.1.119): 
        // curl  http://192.168.1.119/api/ScreenRes/AvailableScreenResolutions
        [HttpGet("AvailableScreenResolutions")]
        public IEnumerable<string> Get_AvailableScreenResolutions()
        {
            List<string> resolution = screenResolutionService.AvailableScreenResolutions;
            logger?.LogInformation($"GET: api/ScreenRes/AvailableScreenResolutions. Returning: '{System.Text.Json.JsonSerializer.Serialize(resolution)}'");
            return resolution;
        }

        // GET: api/ScreenRes/InitialScreenResolution
        // from Linux (assuming IP-address of this server is 192.168.1.119): 
        // curl  http://192.168.1.119/api/ScreenRes/InitialScreenResolution
        [HttpGet("InitialScreenResolution")]
        public string Get_InitialScreenResolution()
        {
            // Don't know, why the extra double-quotes must be addes manually, but
            // without them, the fetch-calls in site.js don't work correctly.
            string resolution = $"\"{screenResolutionService.InitialScreenResolution}\"";
            logger?.LogInformation($"GET: api/ScreenRes/InitialScreenResolution. Returning: '{resolution}'");
            return resolution;
        }

        // GET: api/ScreenRes/CurrentScreenResolution
        // from Linux (assuming IP-address of this server is 192.168.1.119): 
        // curl  http://192.168.1.119/api/ScreenRes/CurrentScreenResolution
        [HttpGet("CurrentScreenResolution")]
        public string Get_CurrentScreenResolution()
        {
            // Don't know, why the extra double-quotes must be addes manually, but
            // without them, the fetch-calls in site.js don't work correctly.
            string resolution = $"\"{screenResolutionService.CurrentScreenResolution}\"";
             logger?.LogInformation($"GET: api/ScreenRes/CurrentScreenResolution. Returning: '{resolution}'");
            return resolution;
        }

        // POST: api/ScreenRes/SetScreenResolution
        // The string to post is a screen-resolution like "1024x768"
        // from Linux (assuming IP-address of this server is 192.168.1.119): 
        // curl -X POST -H "Accept: application/json" -H "Content-Type: application/json" --data '"1024x768"' http://192.168.1.119/api/ScreenRes/SetScreenResolution
        [HttpPost("SetScreenResolution")]
        public void Post_SetScreenResolution([FromBody] string postDataString)
        {
            logger?.LogInformation($"POST: api/ScreenRes/SetScreenResolution. Data: '{postDataString}'");
            screenResolutionService.SetScreenResolution(postDataString);
        }

    }
}
