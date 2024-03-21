using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace SB.BackgroundGen
{
  internal static class Win32
  {
    internal const int SPI_SETDESKWALLPAPER = 20;
    internal const int SPIF_UPDATEINIFILE = 0x01;
    internal const int SPIF_SENDWININICHANGE = 0x02;

    [Flags]
    internal enum Style : int
    {
      Tiled,
      Centered,
      Stretched
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

  }
}
