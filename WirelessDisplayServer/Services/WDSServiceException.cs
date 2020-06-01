using System;

namespace WirelessDisplayServer.Services
{
    public class WDSServiceException : Exception
    {
        public WDSServiceException(string msg) :base (msg)
        {
        }
    }
}