using System;


namespace WirelessDisplayServer.Services
{
    public enum StreamType
    {
        None,
        VNC,
        FFmpeg
    }


    public interface IStreamSinkService
    {
        StreamType StartedStream { get; }
        
        void StartStreaming( StreamType streamType, UInt16 portNo );

        void StopAllStreamPlayers();
    }
}