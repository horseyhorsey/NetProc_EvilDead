
using NetProcgame.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ED_Console.Modes
{
    public class BaseMode : Mode
    {
        Game _game;
        bool _ballStarting = true;
        public bool MultiBallActive = false;

        public BaseMode(Game game, int priority) :
            base(game, priority)
        {
            _game = game;
        }

        public bool sw_startButton_active(Switch sw)
        {
            if (_game.Players.Count == 0)
            {
                addEdPlayer();

                return SWITCH_CONTINUE;
            }

            if (_game.ball == 1)
                if (_game.Players.Count < 4)
                {
                    addEdPlayer();
                }


            return SWITCH_CONTINUE;
        }

        private void addEdPlayer()
        {
            _game.AddPlayer();
            _game.Logger.Log("Player added");
        }

        public bool sw_down_active(Switch sw)
        {
            _game._sound.VolumeDown();

            return SWITCH_CONTINUE;
        }

        public bool sw_up_active(Switch sw)
        {
            _game._sound.VolumeUp();

            return SWITCH_CONTINUE;
        }

        public bool sw_shooter_active(Switch sw)
        {
            //_game.Modes.Add(_game.)

            //if (!_game.Modes.Modes.Contains(_game.SkillshotMode))
            //    _game.Modes.Add(_game.SkillshotMode);

            return SWITCH_CONTINUE;
        }

        internal void DriveModeLamp(string v1, string v2)
        {
            throw new NotImplementedException();
        }

        //  def sw_up_active(self, sw):

        //      volume = self.game.sound.volume_up()
        //self.game.set_status("Volume Up : " + str(volume))
        //return True
    }
}
