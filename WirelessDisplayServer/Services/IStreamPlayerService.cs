using System;


namespace WirelessDisplayServer.Services
{
    public interface IStreamPlayerService
    {
        bool FfplayStarted { get; }
        
        bool VncViewerStarted { get; }
        
        bool StartFfplay( UInt16 port );

        bool StartVncViewerReverse( UInt16 port );

        void StopAllStreamPlayers();
    }
}