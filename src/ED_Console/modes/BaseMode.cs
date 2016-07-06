
using NetProcgame.Display.Layers;
using NetProcgame.Game;
using NetProcgame.Services;
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
        private bool _testModes = false;

        public BaseMode(Game game, int priority) :
            base(game, priority)
        {
            _game = game;
        }
       
        private void addEdPlayer()
        {
            _game.AddPlayer();
            _game.Logger.Log("Player added");
        }

        #region Switch Handlers
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
        
        public bool sw_flipperLwL_active_for_6s(Switch sw)
        {
            return SWITCH_CONTINUE;
        }
        public bool sw_flipperLwR_active_for_6s(Switch sw)
        {
            return SWITCH_CONTINUE;
        }
        public bool sw_outhole_active_for_350ms(Switch sw)
        {
            if (_game.TiltStatus)
            {
                _game.Coils["outhole"].Pulse();

                return SWITCH_STOP;
            }

            if (_game.TroughMode.num_balls_in_play == 1)
            {
                if (_game.ball_save.is_active())
                {                    
                    _game.DisableAllLamps();
                    _game.LampCtrl.play_show("shellyHit");
                    _game._sound.PlaySound("ChungArghh");
                    _game._sound.PlaySound("STDM");
                }
                else
                    _game._sound.PlaySound("Outhole");
            }

            return SWITCH_CONTINUE;
        }
        public bool sw_shooter_active(Switch sw)
        {
            if (_ballStarting)
            {
                //StartGameProfile();
                //self.game.trophyMode.setupPlayer()

                if (!_testModes)
                {
                    //if not self.isWizardReady():
                    if (!_game.SkillshotMode.IsStarted())
                    {
                        _game.Modes.Add(_game.SkillshotMode);
                        CheckForFreeScotty();
                    }
                    else
                        //StartingWizard();
                        ;
                }
                else
                {
                    _game.Lamps["gi01"].Pulse();
                    _game.Lamps["gi02"].Pulse();
                    _game.Lamps["gi03"].Pulse();

                    //if (_game.ModeTest.IsStarted())
                    //    _game.Modes.Add(_game.ModeTest);
                }

            }
            else
                _game.Coils["topDropTarget"].Enable();

            return SWITCH_CONTINUE;
        }    
        public bool sw_shooter_inactive_for_50ms(Switch sw)
        {
            if (_ballStarting)
            {
                _game.LampCtrl.play_show("alternating");

                if (!_game.CellarRampMode.CellarIsReady())
                    _game._sound.PlayMusic("base-music-bgm", -1);
            }

            return SWITCH_CONTINUE;
        }
        public bool sw_shooter_inactive_for_2s(Switch sw)
        {
            if (_ballStarting)
            {
                var ballSaveTime = 15;
                _game.ball_save.num_balls_to_save = 1;
                _game.ball_save.timer = ballSaveTime;
                _game.ball_save.callback = new AnonDelayedHandler(BallSaved);
                _game.ball_save.allow_multiple_saves = true;
                _game.ball_save.trough_enable_ball_save = 
                    new NetProcGame.modes.BallSaveEnable(_game.TroughMode.enable_ball_save);

                _ballStarting = false;
                layer = null;

                _game.LampCtrl.play_show("alternating");

                if (!_game.CellarRampMode.CellarIsReady())
                    _game._sound.PlayMusic("base-music-bgm", -1);
            }

            return SWITCH_STOP;
        }
        public bool sw_ballShooter_active_for_5ms(Switch sw)
        {
            if (_game.Switches["shooter"].IsActive(0.3))
            {
                cancel_delayed("IdleSounds");

                _game._sound.PlaySound("shotGunBlast");

                _game.Coils["plunger"].Pulse();
            }

            return SWITCH_STOP;
        }
        public bool sw_slingL_active(Switch sw)
        {
            _game.score(1337);
            _game._sound.PlaySound("sling");
            DriveModeFlasher("slingL","slow",false,1);

            return SWITCH_CONTINUE;
        }
        public bool sw_slingR_active(Switch sw)
        {
            _game.score(1337);
            _game._sound.PlaySound("sling");
            DriveModeFlasher("slingR", "slow", false, 1);

            return SWITCH_CONTINUE;
        }

        public bool sw_leftOutLane_active(Switch sw)
        {


            return SWITCH_CONTINUE;
        }
        public bool sw_leftReturn_active(Switch sw)
        {


            return SWITCH_CONTINUE;
        }
        public bool sw_rightReturn_active(Switch sw)
        {


            return SWITCH_CONTINUE;
        }
        public bool sw_rightOutlane_active(Switch sw)
        {


            return SWITCH_CONTINUE;
        }

        public bool sw_topDropTarget_active(Switch sw)
        {


            return SWITCH_CONTINUE;
        }

        public bool sw_scottyKicker_active(Switch sw)
        {


            return SWITCH_CONTINUE;
        }
        public bool sw_scottyKicker_active_for_4s(Switch sw)
        {


            return SWITCH_CONTINUE;
        }
        public bool sw_scottyKicker_inactive_for_2s(Switch sw)
        {


            return SWITCH_CONTINUE;
        }

        public bool sw_kickOut_active(Switch sw)
        {


            return SWITCH_CONTINUE;
        }
        public bool sw_leftEject_active_for_1s(Switch sw)
        {


            return SWITCH_CONTINUE;
        }
        public bool sw_vukswitch_active(Switch sw)
        {
            return SWITCH_CONTINUE;
        }   
        public bool sw_rightE_active(Switch sw)
        {
            return SWITCH_CONTINUE;
        }
        public bool sw_rightE_active_for_1500ms(Switch sw)
        {
            return SWITCH_CONTINUE;
        }
        public bool sw_rightE_inactive_for_50ms(Switch sw)
        {
            return SWITCH_CONTINUE;
        }
        public bool sw_topRolloverL_active(Switch sw)
        {
            return SWITCH_CONTINUE;
        }
        public bool sw_topRolloverM_active(Switch sw)
        {
            return SWITCH_CONTINUE;
        }
        public bool sw_topRolloverR_active(Switch sw)
        {
            return SWITCH_CONTINUE;
        }
        public bool sw_rightOrbit_active(Switch sw)
        {
            return SWITCH_CONTINUE;
        }
        public bool sw_Target1_active(Switch sw)
        {
            return SWITCH_CONTINUE;
        }
        public bool sw_Target2_active(Switch sw)
        {
            return SWITCH_CONTINUE;
        }
        public bool sw_Target3_active(Switch sw)
        {
            return SWITCH_CONTINUE;
        }
        public bool sw_Target4_active(Switch sw)
        {
            return SWITCH_CONTINUE;
        }
        public bool sw_Target5_active(Switch sw)
        {
            return SWITCH_CONTINUE;
        }
        public bool sw_Target6_active(Switch sw)
        {
            return SWITCH_CONTINUE;
        }
        #endregion

        private void StartingWizard()
        {
            throw new NotImplementedException();
        }

        private void CheckForFreeScotty()
        {
            throw new NotImplementedException();
        }

        private void BallSaved()
        {
            //     self.game.log("BaseGameMode: BALL SAVED from Trough callback")
            //     if self.game.wizardMode.is_started() or self.game.deadByDawn.is_started() or self.game.getPlayerState('mode_active'):
            //self.game.coils.plunger.pulse(50)

            if (!MultiBallActive)
            {
                var txtScriptLayer =
                _game.DisplayHelper.GenerateScriptedTextLayer("WE|LIVE|STILL", _game.Width, _game.Height, 0.7, "EDLarge", AssetService.Styles["redYellow"]);

                var group = new GroupedLayer(_game.Width, _game.Height, new List<Layer>()
                {
                    AssetService.Videos["WeLiveStill"],
                    txtScriptLayer
                });

                layer = group;

                _game._sound.PlaySound("WeLiveStill");

                delay("clearLayer", NetProcgame.NetPinproc.EventType.None, 2.5, new AnonDelayedHandler(ClearLayer));

                _game.Coils["plunger"].Pulse(50);
                AssetService.Videos["WeLiveStill"].reset();
            }
        }

        private void ClearLayer()
        {
            layer = null;
        }

        public void DriveModeLamp(string lampName, string style = "on", bool delayed = false, int delayTime = 0)
        {
            switch (style)
            {
                case "slow":
                    _game.Lamps[lampName].Schedule(0x00ff00ff, delayTime);
                    break;
                case "medium":
                    _game.Lamps[lampName].Schedule(0x0f0f0f0f, delayTime);
                    break;
                case "medfast":
                    _game.Lamps[lampName].Schedule(0x33333333, delayTime);
                    break;
                case "fast":
                    _game.Lamps[lampName].Schedule(0x55555555, delayTime);
                    break;
                case "on":
                    _game.Lamps[lampName].Pulse(0);
                    break;
                case "off":
                    _game.Lamps[lampName].Disable();
                    break;
                default:
                    break;
            }

            if (delayed)
                this.delay("coilDelay", NetProcgame.NetPinproc.EventType.None, (double)delayTime,
                    new AnonDelayedHandler(_game.Lamps[lampName].Disable));
        }
        private void DriveModeFlasher(string coilName, string style, bool delayed, int delayTime)
        {
            switch (style)
            {
                case "slow":
                    _game.Coils[coilName].Schedule(0x00ff00ff, delayTime);
                    break;
                case "medium":
                    _game.Coils[coilName].Schedule(0x0f0f0f0f, delayTime);
                    break;
                case "medfast":
                    _game.Coils[coilName].Schedule(0x33333333, delayTime);
                    break;
                case "fast":
                    _game.Coils[coilName].Schedule(0x55555555, delayTime);
                    break;
                case "on":
                    _game.Coils[coilName].Pulse(0);
                    break;
                case "off":
                    _game.Coils[coilName].Disable();
                    break;
                default:
                    break;
            }

            if (delayed)
                this.delay("coilDelay", NetProcgame.NetPinproc.EventType.None, (double)delayTime,
                    new AnonDelayedHandler(_game.Coils[coilName].Disable));
        }

        //  def sw_up_active(self, sw):

        //      volume = self.game.sound.volume_up()
        //self.game.set_status("Volume Up : " + str(volume))
        //return True
    }
}
