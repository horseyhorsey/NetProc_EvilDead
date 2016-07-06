using NetProcgame.Display.Layers;
using NetProcgame.Display.Sdl;
using NetProcgame.Game;
using NetProcgame.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ED_Console.Modes
{
    public class ScottyMBall : Mode
    {
        private int _numTimesPlayed;

        private List<VideoLayer> VideoLayers;
        private List<string> Sounds;

        private Game _game;

        public ScottyMBall(Game game, int priority) : base(game, priority)
        {
            _game = game;

            VideoLayers = new List<VideoLayer>()
            {
                AssetService.Videos["ScottyAllDie"],
                AssetService.Videos["ScottyBridge"],
                AssetService.Videos["ScottyLeave"],
                AssetService.Videos["ScottyWereNot"],
                AssetService.Videos["ScottyScotty"],
                AssetService.Videos["ScottyCherylWasRight"]
            };

            Sounds = new List<string>(6)
            {
                "ScottyAllDie",
                "ScottyBridge",
                "ScottyLeave",
                "ScottyWereNot",
                "ScottyScotty",
                "ScottyCherylWasRight"
            };
        }

        public override void mode_started()
        {
            //PausePages();

            if (_game.TroughMode.num_balls_in_play == 1)
            {
                _game.BaseMode.MultiBallActive = true;
                _game._sound.StopMusic();
                _game.LampCtrl.stop_show();
                _game.Coils["ledRainbow"].Disable();
                _game.DisableAllLamps();
                _game.LampCtrl.play_show("scottyStart", false, new AnonDelayedHandler(this.update_lamps));
                //var time = _game._sound.PlaySound("ScottyCherylWasRight");
                _game._sound.PlaySound("thunder");
                _game._sound.PlaySound("clockTick");
                delay("started", NetProcgame.NetPinproc.EventType.None, 6, new AnonDelayedHandler(this.StartMusic));
                layer = VideoLayers[5];
                VideoLayers[0].reset();
            }
            else
                StartMusic();

            //         if self.game.trophyMode.checkTrophyComplete('Scotty'):			
            //self.game.trophyMode.layer = self.game.trophyMode.buildLayerScript('Scotty')
            //         self.game.trophyMode.updateTrophy('Scotty')
        }

        public override void mode_stopped()
        {
            _game.BaseMode.MultiBallActive = false;
            _game.GetCurrentPlayer().CompletedMultiBalls["s_multiball"]=true;

            cancel_delayed("dmd"); 
        }

        private void StartMusic()
        {
            //PausePages();
            for (int i = 0; i < 3; i++)
            {
                _game.Lamps["gi0" + i + 1].Pulse();
            }
            _game.Lamps["Scotty2Ball"].Schedule(0x0F0F0F0F);
            _game.Coils["ScottyKick"].Pulse();

            if (_game.CellarMultiBall.IsStarted() && _game.ShedMultiball.IsStarted())
            {
                BallLauncher();
                _game.ball_save.start(4, 20, true, true);
            }
            else if (_game.CellarMultiBall.IsStarted() || _game.ShedMultiball.IsStarted())
            {
                BallLauncher();
                _game.ball_save.start(4, 20, true, true);
            }
            else
            {
                _game.TroughMode.launch_balls(1);
                _game.ball_save.start(2, 20, true, true);
                _game._sound.PlayMusic("scottyMusic", -1);
                DmdLayer();
            }

   //         if not self.game.MultiScoring.is_started():
			//self.game.modes.add(self.game.MultiScoring)

        }

        private void DmdLayer()
        {
            Random r = new Random();
            var random = r.Next(0, 5);

            _game._sound.PlaySound(Sounds[random]);
            layer = VideoLayers[random];

            delay("dmd", NetProcgame.NetPinproc.EventType.None, 12, new AnonDelayedHandler(this.DmdLayer));
            layer.reset();

        }

        public override void update_lamps()
        {
            _game.BaseMode.DriveModeLamp("Scotty2Ball", "medfast");
        }

        private void BallLauncher()
        {
            _game._sound.PlaySound("ScottyScotty");

            if (_game.TroughMode.num_balls_in_play == 3)
                _game.TroughMode.launch_balls(1);
            else if (_game.TroughMode.num_balls_in_play == 2)
                _game.TroughMode.launch_balls(2);
            else if (_game.TroughMode.num_balls_in_play == 1)
                _game.TroughMode.launch_balls(3);
        }

        private bool sw_shooter_active_for_700ms(Switch sw)
        {
            _game.Coils["plunger"].Pulse();

            return SWITCH_STOP;
        }
    }
}
