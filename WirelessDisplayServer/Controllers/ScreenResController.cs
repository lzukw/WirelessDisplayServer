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
        protected ILogger<ScreenResController> _logger { get;  }
        protected IScreenResolutionService _screenResolutionService { get; }

        public ScreenResController(ILogger<ScreenResController> logger, IScreenResolutionService screenResolutionService)
        {
            // logger and screenResolutionService are injected by Dependcy-Injection 
            // by "the runtime" (Program.cs and Startup.cs).
            _logger = logger;
            _screenResolutionService = screenResolutionService;
        }

        // GET: api/ScreenRes/AvailableScreenResolutions
        // from Linux (assuming IP-address of this server is 192.168.1.119): 
        // curl  http://192.168.1.119/api/ScreenRes/AvailableScreenResolutions
        [HttpGet("AvailableScreenResolutions")]
        public IEnumerable<string> Get_AvailableScreenResolutions()
        {
            return _screenResolutionService.AvailableScreenResolutions;
        }

        // GET: api/ScreenRes/InitialScreenResolution
        // from Linux (assuming IP-address of this server is 192.168.1.119): 
        // curl  http://192.168.1.119/api/ScreenRes/InitialScreenResolution
        [HttpGet("InitialScreenResolution")]
        public string Get_InitialScreenResolution()
        {
            return $"\"{_screenResolutionService.InitialScreenResolution}\"";
        }

        // GET: api/ScreenRes/CurrentScreenResolution
        // from Linux (assuming IP-address of this server is 192.168.1.119): 
        // curl  http://192.168.1.119/api/ScreenRes/CurrentScreenResolution
        [HttpGet("CurrentScreenResolution")]
        public string Get_CurrentScreenResolution()
        {
            return $"\"{_screenResolutionService.CurrentScreenResolution}\"";
        }

        // POST: api/ScreenRes/SetScreenResolution
        // The string to post is a screen-resolution like "1024x768"
        // from Linux (assuming IP-address of this server is 192.168.1.119): 
        // curl -X POST -H "Accept: application/json" -H "Content-Type: application/json" --data '"1024x768"' http://192.168.1.119/api/ScreenRes/SetScreenResolution
        [HttpPost("SetScreenResolution")]
        public void Post_SetScreenResolution([FromBody] string postDataString)
        {
            _screenResolutionService.SetScreenResolution(postDataString);
        }

    }
}
