using System;
using System.Linq;

namespace ED_Console.Modes
{
    public class WorkShed : NetProcgame.Game.Mode
    {        
        Game _game;

        public WorkShed(Game game, int priority) : base(game, priority)
        {
            _game = game;
        }

        public override void mode_started()
        {
            var s = _game.GetCurrentPlayer().WorkshedsLocked.Where(x => x == true).Count();
        }

        private void ShedKickerCheck()
        {
            if (!_game.ShedMultiball.IsStarted())
            {
                if (!_game.GetCurrentPlayer().WorkshedsLocked[0])
                {
                    _game.GetCurrentPlayer().WorkshedsLocked[0] = true;
                    _game.score(250000);

                    if (!_game.BaseMode.MultiBallActive)
                    {
                        //var time
                    }
                    else
                        MultiBallReady();
                }
                else if (!_game.GetCurrentPlayer().WorkshedsLocked[1])
                {
                    _game.GetCurrentPlayer().WorkshedsLocked[1] = true;
                    _game.score(500000);

                    if (!_game.BaseMode.MultiBallActive)
                    {
                        //var time
                    }
                    else
                        MultiBallReady();


                }
                else if (!_game.GetCurrentPlayer().WorkshedsLocked[2])
                {
                    _game.GetCurrentPlayer().WorkshedsLocked[2] = true;
                    _game.score(750000);

                    if (!_game.BaseMode.MultiBallActive)
                    {
                        //var time
                    }
                    else
                        MultiBallReady();
                }
            }
            else
            {
                _game.BaseMode.DriveModeLamp("rightFlash", "medium");

                delay("", NetProcgame.NetPinproc.EventType.None, 1, new NetProcgame.Game.AnonDelayedHandler(KickOutFromShed));
            }
        }

        private void KickOutFromShed()
        {
            _game.Coils["rightEject"].Pulse();
            _game.update_lamps();
        }

        private void MultiBallReady()
        {
            cancel_delayed("multiballReady");


        }

        private void GenerateShedLockLayer(string letter)
        {

        }
    }
}




