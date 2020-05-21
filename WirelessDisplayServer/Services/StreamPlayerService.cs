using System;
using System.Diagnostics;

using Microsoft.Extensions.Logging;

namespace WirelessDisplayServer.Services
{
    public class StreamPlayerService : IStreamPlayerService
    {
        //
        // Readonly properties set by the constructor (dependency injection)
        //
        protected readonly ILogger<StreamPlayerService> _logger;
        protected readonly string _pathToVncViewer;
        protected readonly string _vncViewerArgs;
        protected readonly string _pathToFfplay;
        protected readonly string _ffplayArgs;

        //
        // Constructor
        //
        public StreamPlayerService( ILogger<StreamPlayerService> logger, string pathToVncViewer, 
                                    string vncViewerArgs, string pathToFfplay, string ffplayArgs)
        {
            // The parameters of this constructor are injected by Dependcy-Injection 
            // by "the runtime" (Program.cs and Startup.cs).
            _logger = logger;
            _pathToVncViewer = pathToVncViewer;
            _vncViewerArgs = vncViewerArgs;
            _pathToFfplay = pathToFfplay;
            _ffplayArgs = ffplayArgs;
        }

        //
        // Process-Objects representing the external processes (VNC-viewer and FFplay)
        //
        protected Process _vncViewerProcess = null;
        protected Process _ffplayProcess = null;

        //
        // Properties for getting the state of the processes.
        //
        public bool FfplayStarted { get { return _ffplayProcess != null && _ffplayProcess.HasExited == false; }}
        public bool VncViewerStarted { get { return _vncViewerProcess != null && _vncViewerProcess.HasExited == false; } }

        //
        // Methods for starting and stopping the processes.
        //
        public bool StartFfplay( UInt16 port )
        {
            StopAllStreamPlayers();

            string arguments = _ffplayArgs.Replace("ppppp", port.ToString());

            _ffplayProcess = new Process();
            
            _ffplayProcess.StartInfo.Arguments = arguments;
            _ffplayProcess.StartInfo.FileName = _pathToFfplay;
            _ffplayProcess.StartInfo.UseShellExecute = true;

            _logger.LogInformation($"Starting Process '{_pathToFfplay} {arguments}'");

            try
            {
                _ffplayProcess.Start();
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception occurred while starting process: {e}");
                _ffplayProcess.Dispose();
                _ffplayProcess = null;
                return false;
            }

            return true;
        }

        public bool StartVncViewerReverse( UInt16 port )
        {
            StopAllStreamPlayers();

            string arguments = _vncViewerArgs.Replace("ppppp", port.ToString());

            _vncViewerProcess = new Process();
            
            _vncViewerProcess.StartInfo.Arguments = arguments;
            _vncViewerProcess.StartInfo.FileName = _pathToVncViewer;
            _vncViewerProcess.StartInfo.UseShellExecute = true;

            _logger.LogInformation($"Starting Process '{_pathToVncViewer} {arguments}'");

            try
            {
                _vncViewerProcess.Start();
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception occurred while starting process: {e}");
                _vncViewerProcess.Dispose();
                _vncViewerProcess = null;
                return false;
            }
            
            return true;
        }


        public void StopAllStreamPlayers()
        {
            if ( _vncViewerProcess != null )
            {
                _logger.LogInformation("Stopping VNC-viewer process.");

                try
                {
                    _vncViewerProcess.Kill();
                   
                }
                catch (Exception e)
                {
                    _logger.LogError($"Could not kill VNC-Client: {e}");
                }
                finally
                {
                    while( ! _vncViewerProcess.HasExited) { /*empty*/ }
                    _vncViewerProcess.Dispose();
                    _vncViewerProcess = null;
                }
            }

            if ( _ffplayProcess != null )
            {
                _logger.LogInformation("Stopping FFplay process.");
                
                try
                {
                    _ffplayProcess.Kill();
                   
                }
               catch (Exception e)
               {
                   _logger.LogError($"Could not kill FFplay: {e}");
               }
               finally
               {
                   while( ! _ffplayProcess.HasExited) { /*empty*/ }
                   _ffplayProcess.Dispose();
                   _ffplayProcess = null;
               }
           }

        }
    }
}