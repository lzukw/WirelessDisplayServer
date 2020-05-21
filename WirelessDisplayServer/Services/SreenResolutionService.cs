using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace WirelessDisplayServer.Services
{
    /**
     * Class to run the external program 'screenres.exe', which is used
     * to obain information about the current screen-resolution, get a 
     * list of all available screen-resolutions and change the screen-resolution.
     */
    public class ScreenResolutionService : IScreenResolutionService
    {
        private readonly ILogger<ScreenResolutionService> _logger;

        /** The filepath to the screenres.exe-file */
        protected string _pathToScreenRes { get; }

        /** Constructor, receives the filepath to the 'screenres.exe' */
        public ScreenResolutionService(ILogger<ScreenResolutionService> logger, string pathToScreenRes)
        {
            // logger and pathToScreenRes are injected by Dependcy-Injection 
            // by "the runtime" (Program.cs and Startup.cs).
            _logger = logger;
            this._pathToScreenRes = pathToScreenRes;
        }


        // Backup-field for InitialScreenResolution
        protected string _initialScreenResolution = null;

        // protected getter for InitialScreenResolution
        protected string _getInitialScreenResolution()
        {
            if (_initialScreenResolution == null)
            {
                List<string> outputLines;
                // run 'screenres.exe /V /S'
                int exitCode = runScreenRes("/V /S", out outputLines);
                if (exitCode != 0)
                {
                    _logger.LogError("Could not get initial screen-resolution");
                }
                Debug.Assert(outputLines.Count == 1);

                MatchCollection mc = Regex.Matches(outputLines[0], @"(\d+)x(\d+).*");
                Debug.Assert(mc.Count==1);
                Debug.Assert(mc[0].Groups.Count==3);

                _initialScreenResolution = $"{mc[0].Groups[1]}x{mc[0].Groups[2]}";
            }

            return _initialScreenResolution;
        }


        /** 
         * The initial screen-resolution, which is the screen-resolution that was
         * set, when first calling 'screenres.exe'. The string is for example "1024x768".
         */
        public string InitialScreenResolution 
        { 
            get 
            { 
                return _getInitialScreenResolution();
            } 
        }


        /** Gets all available screen-resolutions as strings (for example '640x480') */
        public List<string> AvailableScreenResolutions 
        { 
            get 
            {
                // First initialize the value of _initialScreenResolution, if necessary
                if (_initialScreenResolution == null)
                {
                    _getInitialScreenResolution();
                }

                List<string> _availableScreenResolutions = new List<string>();

                List<string> outputLines;
                // run 'screenres.exe /V /L'
                int exitCode = runScreenRes("/V /L", out outputLines);
                if (exitCode != 0)
                {
                    _logger.LogError ("Could not get available screen-resolutions.");
                }
               
                foreach (string outputLine in outputLines)
                {
                    MatchCollection mc = Regex.Matches(outputLine, @"(\d+)x(\d+).*");
                    if (mc.Count != 1)
                    {
                        _logger.LogWarning($"Could not parse line '{outputLine}'");
                        continue;
                    }
                    Debug.Assert(mc[0].Groups.Count==3);
                    string resolution = $"{mc[0].Groups[1]}x{mc[0].Groups[2]}";
                    if ( ! _availableScreenResolutions.Contains(resolution))
                    {
                        _availableScreenResolutions.Add(resolution);
                    }
                }

                return _availableScreenResolutions;
            }
        }

        /**
         * The current screen-resolution (for example "640x480").
         */
        public string CurrentScreenResolution
        {
            get
            {
                // First initialize the value of _initialScreenResolution, if necessary
                if (_initialScreenResolution == null)
                {
                    _getInitialScreenResolution();
                }

                List<string> outputLines;
                // run 'screenres.exe /V /S'
                int exitCode = runScreenRes("/V /S", out outputLines);
                if (exitCode != 0)
                {
                    _logger.LogError ("Could not get current screen-resolution.");
                }
                Debug.Assert(outputLines.Count == 1);

                MatchCollection mc = Regex.Matches(outputLines[0], @"(\d+)x(\d+).*");
                Debug.Assert(mc.Count==1);
                Debug.Assert(mc[0].Groups.Count==3);

               return $"{mc[0].Groups[1]}x{mc[0].Groups[2]}";
            }
            
        }


        /** 
         * Set the screen-resolution. The argument must be string beginning with
         * for example "640x480".
         */
        public void SetScreenResolution(string screenResolution )
        {
            // First initialize the value of _initialScreenResolution, if necessary
            if (_initialScreenResolution == null)
            {
                _getInitialScreenResolution();
            }

            // Retrieve width and heigth information.
            MatchCollection mc = Regex.Matches(screenResolution, @"(\d+)x(\d+).*");
            Debug.Assert(mc.Count==1);
            Debug.Assert(mc[0].Groups.Count==3);

            string arguments = $"/X:{mc[0].Groups[1]} /Y:{mc[0].Groups[2]}";

            // Run 'screenres.exe /X:nnn /y:mmm'
            List<string> outputLines;
            int exitCode = runScreenRes(arguments, out outputLines);
            if (exitCode != 0)
            {
                _logger.LogError ("Could not set screen-resolution.");
            }
        }     


        /**
         * Helper method to call the external program (screenres.exe)
         */
        protected int runScreenRes(string arguments, out List<string> outputLines)
        {
            _logger.LogInformation($"Starting process '{_pathToScreenRes} {arguments}'");

            outputLines = new List<string>();

            using (Process _screenRes = new Process())
            {
                _screenRes.StartInfo.UseShellExecute = false;
                _screenRes.StartInfo.RedirectStandardOutput = true;
                _screenRes.StartInfo.RedirectStandardInput = true;
                _screenRes.StartInfo.FileName = _pathToScreenRes;
                _screenRes.StartInfo.CreateNoWindow = true;
                _screenRes.StartInfo.Arguments = arguments;
                try
                {
                    _screenRes.Start();
                }
                catch (Exception e)
                {
                    _logger.LogError($"Exception occurred while starting process: {e}");
                    return 1;
                }

                //Wait until screenres-process exits.
                while (!_screenRes.HasExited) { System.Threading.Thread.Sleep(10); }

                string line;
                while ((line = _screenRes.StandardOutput.ReadLine()) != null)
                {
                    outputLines.Add(line);
                }

                _logger.LogInformation($"Process returned with exit-code {_screenRes.ExitCode}");

                return _screenRes.ExitCode;
            }
        }


    }
}