using NetProcgame.Display.Layers;
using NetProcgame.Services;
using NetProcgame.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ED_Console.Modes
{
    public class WorkShed : NetProcgame.Game.Mode
    {        
        Game _game;
        AnimatedLayer ChainsawWipe;

        public WorkShed(Game game, int priority) : base(game, priority)
        {
            _game = game;

            ChainsawWipe = AssetService.Animations["chainsawWipe"];
        }

        public override void mode_started()
        {
            try
            {
                var s = _game.GetCurrentPlayer().WorkshedsLocked
                .Where(x => x == true).Count();
            }
            catch (Exception)
            {                
            }
            
        }

        public void ShedKickerCheck()
        {
            var player = _game.GetCurrentPlayer();

            if (!_game.ShedMultiball.IsStarted())
            {
                if (!player.WorkshedsLocked[0])
                {
                    player.WorkshedsLocked[0] = true;
                    _game.score(250000);

                    if (!_game.BaseMode.MultiBallActive)
                    {
                        //var time
                    }
                    else
                        MultiBallReady();
                }
                else if (!player.WorkshedsLocked[1])
                {
                    player.WorkshedsLocked[1] = true;
                    _game.score(500000);

                    if (!_game.BaseMode.MultiBallActive)
                    {
                        //var time
                    }
                    else
                        MultiBallReady();


                }
                else if (!player.WorkshedsLocked[2])
                {
                    player.WorkshedsLocked[2] = true;
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

                delay("shedKickoutAfterCheck", NetProcgame.NetPinproc.EventType.None, 1, new NetProcgame.Game.AnonDelayedHandler(KickOutFromShed));
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

        public Layer GenerateShedLockLayer(string letter)
        {            
            var shedStatus =
                _game.BaseMode.SetStatus(letter, "Spotted", 3, "ed_targets", composite: false);
            //trans = procgame.dmd.TransitionLayer(None, self.HDTextLayer1, transitionType = procgame.dmd.Transition.TYPE_FADE)            

            var script = new List<Pair<double, Layer>>();
            var layerEmpty = new Layer();
            script.Add(new Pair<double, Layer>( 1.0, layerEmpty));
            script.Add(new Pair<double, Layer>(2, shedStatus));
            var sl = new ScriptedLayer(_game.Width, _game.Height, script);
            sl.opaque = true;

            var layers = new List<Layer>() { ChainsawWipe, sl };
            var group = new GroupedLayer(_game.Width, _game.Height, layers);

            delay("clearDMD", NetProcgame.NetPinproc.EventType.None, 4,
                new NetProcgame.Game.AnonDelayedHandler(() => {
                    layer = null;
                    ChainsawWipe.reset();
                }));

            layer = group;

            return group;
        }

    }
}




