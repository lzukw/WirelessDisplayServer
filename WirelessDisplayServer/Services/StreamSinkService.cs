using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Threading;

using Microsoft.Extensions.Logging;

namespace WirelessDisplayServer.Services
{
    //
    // Summary:
    //     A class for starting and stopping a script that starts a
    //     streaming-sink (either VNC-viewer in reverse connection
    //     or ffplay).
    public class StreamSinkService : IStreamSinkService
    {
        //////////////////////////////////////////////////////////////////////
        // Constructor and private fields
        //////////////////////////////////////////////////////////////////////
        #region
        
        protected readonly ILogger<StreamSinkService> logger;

        // The vales from the config passed to the constructor
        private readonly string shell;
        private readonly string shellArgsTemplate;
        private readonly string startStreamingScriptPath;
        private readonly string startStreamingScriptArgsTemplate;

        //
        // Summary:
        //     Constructor. 
        // Parameters:
        //   logger:
        //     A logger. Can be null, if no logging is used.
        //   specificConfig:
        //     A key-value-collection with condiguration-values, that have  
        //     their origin in appsettings.json.
        public StreamSinkService( ILogger<StreamSinkService> logger, NameValueCollection specificConfig)
        {
            // The parameters of this constructor are injected by Dependcy-Injection 
            this.logger = logger;

            // extract the necessary values from the configuration.
            FileInfo scriptPath = new FileInfo(specificConfig["Start_Streaming_Script_Path"]);
            if ( ! scriptPath.Exists )
            {
                logger?.LogCritical($"Not creating inatance of StreamSinkService, because script-file not found: '{scriptPath.FullName}'");
                throw new FileNotFoundException($"Script-file does not exist: '{scriptPath.FullName}'");
            }

            shell = specificConfig["shell"];
            shellArgsTemplate = specificConfig["shell_Args_Template"];
            startStreamingScriptPath = scriptPath.FullName;
            startStreamingScriptArgsTemplate = specificConfig["Start_Streaming_Script_Args_Template"];
        }

        // The process used to execute the start-streaming-script
        private Process streamingSinkProcess = null;

        #endregion

        //////////////////////////////////////////////////////////////////////////
        // pPublic properties and methods for implementation of IStreamSinkService
        //////////////////////////////////////////////////////////////////////////
        #region       

        // The backup-field for the property StartedStream.
        private StreamType startedStream = StreamType.None;
        
        //
        // Summary:
        //     The type of streaming, that has been started, or StreamType.None,
        //     if streaming is stopped.
        StreamType IStreamSinkService.StartedStream { get => startedStream; }

        //
        // Summary:
        //     Starts the streaming sing by executing the script. 
        // Parameters:
        //   streamType:
        //     Must be either StreamType.VNC or StreamType.FFmpeg and indicates
        //     the type of streaming-sink to start (vncviewer in reverse mode or
        //     ffplay).
        //   port:
        //     The Port-Number the streaming-sink-lervice listens on.
        // Exceptions:
        //   T:InvalidArgumentException:
        //     If streamType is StreamType.None
        //   T: FileNotFoundException:
        //     The script-file cannot be found.
        //   T:WirelessDisplayServer.Service.WDSServiceException:
        //     If the streaming could not be started or terminated within very
        //     short time.
        void IStreamSinkService.StartStreaming( StreamType streamType, UInt16 portNo )
        {
            // Before starting, first ensure that everything is stopped
            ((IStreamSinkService) this).StopAllStreamPlayers();

            if (streamType == StreamType.None)
            {
                logger?.LogCritical($"StartStreaming() called with argument {StreamType.None}.");
                throw new ArgumentException($"StartStreaming() called with argument {StreamType.None}.");
            }

            FileInfo scriptPath = new FileInfo(startStreamingScriptPath);
            if (! scriptPath.Exists)
            {
                logger?.LogCritical($"Script-file disapperaed: '{scriptPath.FullName}'");
                throw new FileNotFoundException($"Script-file disapperaed: '{scriptPath.FullName}'"); 
            }

            string scriptArgs = startStreamingScriptArgsTemplate;
            scriptArgs = scriptArgs.Replace("%STREAMING_TYPE", streamType.ToString());
            scriptArgs = scriptArgs.Replace("%PORT_NO", portNo.ToString());

            string argsForProcess = shellArgsTemplate;
            argsForProcess = argsForProcess.Replace("%SCRIPT", scriptPath.FullName);
            argsForProcess = argsForProcess.Replace("%ARGS", scriptArgs);

            // Create new process
            streamingSinkProcess = new Process();
            streamingSinkProcess.StartInfo.FileName = shell;
            streamingSinkProcess.StartInfo.Arguments = argsForProcess;
            streamingSinkProcess.StartInfo.WorkingDirectory = scriptPath.Directory.FullName;
            streamingSinkProcess.StartInfo.UseShellExecute=false;
            streamingSinkProcess.StartInfo.CreateNoWindow = true;

            try
            {
                streamingSinkProcess.Start();
            }
            catch (Exception)
            {
                throw new WDSServiceException($"Could not start Process '{shell} {argsForProcess}'");
            }

            bool exited = streamingSinkProcess.WaitForExit(1000);
            if (exited)
            {
                throw new WDSServiceException($"Process terminated immediately: '{shell} {argsForProcess}'");
            }

            // Finally switch state
            startedStream = streamType;
            logger?.LogInformation($"Successfully started {streamType}-streaming-sink");
        }



        void IStreamSinkService.StopAllStreamPlayers()
        {
            if (streamingSinkProcess != null)
            {
                try
                {
                    if ( ! streamingSinkProcess.HasExited )
                    {
                        streamingSinkProcess.Kill(entireProcessTree:true);   
                    }
                }
                catch (Exception e)
                {
                    logger?.LogCritical($"Could not kill streaming-sink-process: {e.Message}");
                    throw new WDSServiceException($"Could not kill streaming-sink-process: {e.Message}");
                }
                finally
                {
                    streamingSinkProcess.WaitForExit();
                    streamingSinkProcess.Dispose();
                    streamingSinkProcess = null;

                    // finally switch state
                    startedStream = StreamType.None;
                    logger?.LogInformation("Stopped streaming-sink-Process");
                }
            }

        }

        #endregion
    }
}