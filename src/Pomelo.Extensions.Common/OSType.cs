using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#if !NET451
using System.Runtime.InteropServices;
#endif

namespace System
{
    public enum OSType
    {
        Windows,
        OSX,
        Linux
    }

    public static class Platform
    {
        public static OSType OS
        {
            get
            {
#if !NET451
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    return OSType.Windows;
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    return OSType.OSX;
                else
                    return OSType.Linux;
#else
                if (Environment.OSVersion.Platform == PlatformID.Win32NT || Environment.OSVersion.Platform == PlatformID.Win32S || Environment.OSVersion.Platform == PlatformID.Win32Windows || Environment.OSVersion.Platform == PlatformID.WinCE)
                    return OSType.Windows;
                else if (Environment.OSVersion.Platform == PlatformID.MacOSX)
                    return OSType.OSX;
                else
                    return OSType.Linux;
#endif
            }
        }
    }
}
