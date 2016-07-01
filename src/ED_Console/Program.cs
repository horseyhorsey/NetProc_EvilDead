using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetProcgame;
using NetProcgame.Game;
using NetProcgame.Logging;

namespace ED_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            HorseGame game = new HorseGame(NetProcgame.NetPinproc.MachineType.WPC95, new ConsoleLogger());

        }
    }
}
