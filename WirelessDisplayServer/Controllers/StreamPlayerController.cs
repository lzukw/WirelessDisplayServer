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
        protected ILogger<StreamPlayerController> logger { get; }
        protected IStreamSinkService streamSinkService { get; }

        public StreamPlayerController(ILogger<StreamPlayerController> logger, IStreamSinkService streamSinkService)
        {
            // logger and streamSinkService are injected by Dependcy-Injection 
            // by "the runtime" (Program.cs and Startup.cs).
            this.logger = logger;
            this.streamSinkService = streamSinkService;
        }

        // GET: api/StreamPlayer/FfplayStarted
        // from Linux (assuming IP-address of this server is 192.168.1.119): 
        // curl  http://192.168.1.119/api/StreamPlayer/FfplayStarted
        [HttpGet("FfplayStarted")]
        public bool Get_FfplayStarted()
        {
            bool started = (streamSinkService.StartedStream == StreamType.FFmpeg);
            logger?.LogInformation($"GET: api/StreamPlayer/FfplayStarted. Returning '{started}'");
            return started;
        }

        // GET: api/StreamPlayer/VncViewerStarted
        // from Linux (assuming IP-address of this server is 192.168.1.119): 
        // curl  http://192.168.1.119/api/StreamPlayer/VncViewerStarted
        [HttpGet("VncViewerStarted")]
        public bool Get_VncViewerStarted()
        {
            bool started = (streamSinkService.StartedStream == StreamType.VNC);
            logger?.LogInformation($"GET: api/StreamPlayer/VncViewerStarted. Returning '{started}'");
            return started;
        }

        // POST: api/StreamPlayer/StartFfplay
        // The integer (UInt16) to post is the port-Number, which FFplay listens on
        // from Linux (assuming IP-address of this server is 192.168.1.119) and used port-Number is 12345: 
        // curl -X POST -H "Accept: application/json" -H "Content-Type: application/json" --data '12345' http://192.168.1.119/api/StreamPlayer/StartFfplay
        [HttpPost("StartFfplay")]
        public void Post_StartFfplay([FromBody] UInt16 portNo)
        {
            logger?.LogInformation($"POST: api/StreamPlayer/StartFfplay. Posted port-number: {portNo}");
            streamSinkService.StartStreaming(StreamType.FFmpeg, portNo);
        }

        // POST: api/StreamPlayer/StartVncViewerReverse
        // The posted integer (UInt16) is the port-Number, which the VNC-Viewer listens on for reverse-connections.
        // from Linux (assuming IP-address of this server is 192.168.1.119) and used port-Number is 5500: 
        // curl -X POST -H "Accept: application/json" -H "Content-Type: application/json" --data '5500' http://192.168.1.119/api/StreamPlayer/StartVncViewerReverse
        [HttpPost("StartVncViewerReverse")]
        public void Post_StartVncViewer([FromBody] UInt16 portNo)
        {
            logger?.LogInformation($"POST: api/StreamPlayer/StartVncViewerReverse. Posted port-number: {portNo}");
            streamSinkService.StartStreaming(StreamType.VNC, portNo);
        }

        // POST: api/StreamPlayer/StopAllStreamPlayers
        // from Linux (assuming IP-address of this server is 192.168.1.119): 
        // curl -X POST -H "Accept: application/json" -H "Content-Type: application/json" http://192.168.1.119/api/StreamPlayer/StopAllStreamPlayers
        [HttpPost("StopAllStreamPlayers")]
        public void Post_StopAllStreamPlayers()
        {
            logger?.LogInformation("POST: api/StreamPlayer/StopAllStreamPlayers");
            streamSinkService.StopAllStreamPlayers();
        }


    }
}
