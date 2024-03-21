using System;
using System.Net.Sockets;
using System.Linq;
using System.Drawing;

namespace SB.BackgroundGen
{
  class Program
  {
    private static readonly Color BgColor = ColorTranslator.FromHtml("#214188");
    private static readonly int Width = 1440;
    private static readonly int Height = 900;
    private static string SaveFileName = System.Environment.ExpandEnvironmentVariables("%USERPROFILE%\\Pictures\\sb-bg-info.png");

    private static bool UseMonospaceFont = true;
    private static bool IncludeUsername = false;
    private static bool Blank = false;

    private static string LocalIpAddress
    {
      get
      {
        using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
        {
          socket.Connect("8.8.8.8", 65530);

          return $"{(socket.LocalEndPoint as System.Net.IPEndPoint).Address}";
        }
      }
    }

    static void Main(string[] args)
    {
      if (args.Any(a => a.ToUpper().Contains("HELP")))
      {
        Console.WriteLine($"SB.BackgroundGen v{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}");
        Console.WriteLine($"  usage: SB.BackgroundGen.exe [options]");
        Console.WriteLine($"  options:");
        Console.WriteLine($"    -USER, -U - Add username to the output image.");
        Console.WriteLine($"    -SEGOEUI  - Use Segoe UI as the font for the output image.");
        Console.WriteLine($"    -BLANK    - Generates, but does not set, a blank image.");
        return;
      }

      if (args.Any(a => a.ToUpper().Contains("-USER") || a.ToUpper() == "-U"))
        Program.IncludeUsername = true;
      if (args.Any(a => a.ToUpper().Contains("-SEGOEUI")))
        Program.UseMonospaceFont = false;
      if (args.Any(a => a.ToUpper().Contains("-BLANK")))
        Program.Blank = true;

      using (var bmp = new Bitmap(Program.Width, Program.Height))
      {
        using (var gfx = Graphics.FromImage(bmp))
        {
          gfx.FillRectangle(new SolidBrush(Program.BgColor), new RectangleF(0, 0, bmp.Width, bmp.Height));

          if (Program.Blank)
          {
            Program.SaveFileName = Program.SaveFileName.Replace(".png", "-blank.png");
          }
          else
          {
            var name = $"{(Program.IncludeUsername ? $"{System.Environment.UserName} @ " : "")}{System.Environment.MachineName}".Trim();
            var addr = $"{Program.LocalIpAddress}".Trim();

            var fName = new Font((Program.UseMonospaceFont ? "Consolas" : "Segoe UI"), 24, FontStyle.Regular);
            var fAddr = new Font((Program.UseMonospaceFont ? "Consolas" : "Segoe UI"), 20, FontStyle.Regular);

            var szName = gfx.MeasureString(name, fName);
            var szAddr = gfx.MeasureString(addr, fAddr);

            gfx.DrawString(name, fName, Brushes.White, new PointF(Program.Width - szName.Width - 24, 24));
            gfx.DrawString(addr, fAddr, Brushes.White, new PointF(Program.Width - szAddr.Width - 24, (float)(24 + szName.Height)));
          }

          if (System.IO.File.Exists(Program.SaveFileName))
            System.IO.File.Delete(Program.SaveFileName);
        }

        bmp.Save(Program.SaveFileName, System.Drawing.Imaging.ImageFormat.Png);
      }

      if (System.Diagnostics.Debugger.IsAttached)
        System.Diagnostics.Process.Start(Program.SaveFileName);
      
      if (!Program.Blank)
        Win32.SystemParametersInfo(Win32.SPI_SETDESKWALLPAPER, 0, Program.SaveFileName, Win32.SPIF_UPDATEINIFILE | Win32.SPIF_SENDWININICHANGE);
    }
  }
}
