using System;
using System.Threading.Tasks;
using NetProcgame.Logging;
using NetProcgame.Display.Sdl;

namespace ED_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(() =>
            {
                try
                {
                    Game game = new Game(NetProcgame.NetPinproc.MachineType.WPC95, new ConsoleLogger());

                    game.run_loop();

                }

                catch (System.Exception ex)
                {
                    if (DisplayManager.SdlWindow != null && DisplayManager.SdlWindow.SDL_Window != IntPtr.Zero)
                        DisplayManager.QuitSdl();

                    Console.WriteLine(ex.Message + "   " + ex.InnerException);
                    Console.WriteLine(ex.StackTrace);

                    Console.ReadLine();

                    Environment.Exit(-1);
                }
            });

            string line = "";
            while (true)
            {
                line = Console.ReadLine();

                if (line == "q")
                    break;
            }
        }
    }
}
