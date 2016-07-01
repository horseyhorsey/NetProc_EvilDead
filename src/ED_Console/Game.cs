using NetProcgame.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetProcgame.NetPinproc;
using NetProcgame.Tools;
using NetProcgame.Modes;

namespace ED_Console
{
    public class Game : HorseGame
    {
        public Game(MachineType machine_type, ILogger logger) :
            base(machine_type, logger)
        {
            var attract = new Attract(this, 20);

            this.Modes.Add(attract);

            this._game_data = new GameData();

            
        }
    }
}
