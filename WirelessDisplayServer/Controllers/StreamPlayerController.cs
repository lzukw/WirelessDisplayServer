using System;
using Microsoft.AspNetCore.Mvc;

using WirelessDisplayServer.Services;
using Microsoft.Extensions.Logging;

namespace WirelessDisplayServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StreamPlayerController : ControllerBase
    {
        protected ILogger<StreamPlayerController> _logger { get; }
        protected IStreamPlayerService _streamPlayerService { get; }

        public StreamPlayerController(ILogger<StreamPlayerController> logger, IStreamPlayerService streamPlayerService)
        {
            // logger and streamPlayerService are injected by Dependcy-Injection 
            // by "the runtime" (Program.cs and Startup.cs).
            _logger = logger;
            _streamPlayerService = streamPlayerService;
        }

        // GET: api/StreamPlayer/FfplayStarted
        // from Linux (assuming IP-address of this server is 192.168.1.119): 
        // curl  http://192.168.1.119/api/StreamPlayer/FfplayStarted
        [HttpGet("FfplayStarted")]
        public bool Get_FfplayStarted()
        {
            bool started = _streamPlayerService.FfplayStarted;
            _logger.LogInformation($"GET: api/StreamPlayer/FfplayStarted. Returning '{started}'");
            return started;
        }

        // GET: api/StreamPlayer/VncViewerStarted
        // from Linux (assuming IP-address of this server is 192.168.1.119): 
        // curl  http://192.168.1.119/api/StreamPlayer/VncViewerStarted
        [HttpGet("VncViewerStarted")]
        public bool Get_VncViewerStarted()
        {
            bool started = _streamPlayerService.VncViewerStarted;
            _logger.LogInformation($"GET: api/StreamPlayer/VncViewerStarted. Returning '{started}'");
            return started;
        }

        // POST: api/StreamPlayer/StartFfplay
        // The integer (UInt16) to post is the port-Number, which FFplay listens on
        // from Linux (assuming IP-address of this server is 192.168.1.119) and used port-Number is 12345: 
        // curl -X POST -H "Accept: application/json" -H "Content-Type: application/json" --data '12345' http://192.168.1.119/api/StreamPlayer/StartFfplay
        [HttpPost("StartFfplay")]
        public void Post_StartFfplay([FromBody] UInt16 portNo)
        {
            _logger.LogInformation($"POST: api/StreamPlayer/StartFfplay. Posted port-number: {portNo}");
            _streamPlayerService.StartFfplay(portNo);
        }

        // POST: api/StreamPlayer/StartVncViewerReverse
        // The posted integer (UInt16) is the port-Number, which the VNC-Viewer listens on for reverse-connections.
        // from Linux (assuming IP-address of this server is 192.168.1.119) and used port-Number is 5500: 
        // curl -X POST -H "Accept: application/json" -H "Content-Type: application/json" --data '5500' http://192.168.1.119/api/StreamPlayer/StartVncViewerReverse
        [HttpPost("StartVncViewerReverse")]
        public void Post_StartVncViewer([FromBody] UInt16 portNo)
        {
            _logger.LogInformation($"POST: api/StreamPlayer/StartVncViewerReverse. Posted port-number: {portNo}");
            _streamPlayerService.StartVncViewerReverse(portNo);
        }

        // POST: api/StreamPlayer/StopAllStreamPlayers
        // from Linux (assuming IP-address of this server is 192.168.1.119): 
        // curl -X POST -H "Accept: application/json" -H "Content-Type: application/json" http://192.168.1.119/api/StreamPlayer/StopAllStreamPlayers
        [HttpPost("StopAllStreamPlayers")]
        public void Post_StopAllStreamPlayers()
        {
            _logger.LogInformation("POST: api/StreamPlayer/StopAllStreamPlayers");
            _streamPlayerService.StopAllStreamPlayers();
        }


    }
}
