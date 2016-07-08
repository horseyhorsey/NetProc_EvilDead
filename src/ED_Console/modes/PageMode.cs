using NetProcgame.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ED_Console.Modes
{
    public class PageMode : Mode
    {
        public PageMode(Game game, int priority) : base(game, priority)
        {
            
        }

        internal void PausePages(bool v)
        {
            throw new NotImplementedException();
        }
    }
}
