using System.Collections.Generic;

namespace WirelessDisplayServer.Services
{
    public interface IScreenResolutionService
    {
        List<string> AvailableScreenResolutions { get; }

        string InitialScreenResolution { get; }

        string CurrentScreenResolution { get; }
        void SetScreenResolution(string screenResolution );
    }

}