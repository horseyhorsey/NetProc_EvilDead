
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
                _game._sound.StopSound("");

                _game.AddEdPlayer();

                _game.Modes.Add(_game.BumpersMode);

                return SWITCH_CONTINUE;
            }

            if (_game.ball == 1)
                if (_game.Players.Count < 4)
                {
                    _game.AddEdPlayer();
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

        public bool sw_slingL_active(Switch sw)
        {
            _game.score(1337);
            _game._sound.PlaySound("sling");
            DriveModeFlasher("slingL", "slow", false, 1);

            return SWITCH_CONTINUE;
        }
        public bool sw_slingR_active(Switch sw)
        {
            _game.score(1337);
            _game._sound.PlaySound("sling");
            DriveModeFlasher("slingR", "slow", false, 1);

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
        public bool sw_shooter_active_for_8s(Switch sw)
        {
            PlayIdleSound();

            return SWITCH_STOP;
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

        public bool sw_leftOutLane_active(Switch sw)
        {
            //self.game.game_data['Audits']['Left Outlane'] += 1
            //self.game.user_profiles['Audits']['Left Outlane'] += 1
            _game.bonus(5000);
            PlayDrainLayersIfReady();

            return SWITCH_STOP;
        }
        public bool sw_leftReturn_active(Switch sw)
        {
            //self.game.game_data['Audits']['Left Inlane'] += 1
            //self.game.user_profiles['Audits']['Left Inlane'] += 1

            ReturnLaneHelper("outThereRVerb");

            return SWITCH_CONTINUE;
        }
        public bool sw_rightReturn_active(Switch sw)
        {
            //self.game.game_data['Audits']['Right Inlane'] += 1
            //self.game.user_profiles['Audits']['Right Inlane'] += 1

            ReturnLaneHelper("shutItVerb");

            return SWITCH_CONTINUE;
        }
        public bool sw_rightOutlane_active(Switch sw)
        {
            //self.game.game_data['Audits']['Right Outlane'] += 1
            //self.game.user_profiles['Audits']['Right Outlane'] += 1

            _game.bonus(5000);
            PlayDrainLayersIfReady();

            return SWITCH_CONTINUE;
        }

        public bool sw_topDropTarget_active(Switch sw)
        {
            //   self.game.game_data['Audits']['Scotty Drop'] += 1
            //self.game.user_profiles['Audits']['Scotty Drop'] += 1

            _game.score(2500);
            _game.bonus(1000);
            _game.update_lamps();

            return SWITCH_STOP;
        }

        public bool sw_scottyKicker_active(Switch sw)
        {
            //self.game.game_data['Audits']['Scotty Saucer'] += 1
            //self.game.user_profiles['Audits']['Scotty Saucer'] += 1
            _game.score(10000);
            _game.bonus(5000);

            var player = _game.GetCurrentPlayer();

            if (!player.ModeActive)
            {
                if (player.ScottyOpen)
                {
                    if (!_game.ScottyMultiBall.IsStarted())
                    {
                        _game.Modes.Add(_game.ScottyMultiBall);
                        player.ScottyOpen = false;
                    }
                }
                else if (player.CardsEnabled && !MultiBallActive)
                {
                    _game._sound.StopMusic();
                    player.CardCount = 0;
                    //_game.CardsMode.GuessCards();
                }
                else
                {
                    EjectScottyKicker();
                }
            }
            else
            {
                EjectScottyKicker();
            }

            return SWITCH_STOP;
        }
        public bool sw_scottyKicker_active_for_4s(Switch sw)
        {
            if (!_game.GetCurrentPlayer().CardsEnabled)
            {
                EjectScottyKicker();
            }


            return SWITCH_CONTINUE;
        }
        public bool sw_scottyKicker_inactive_for_2s(Switch sw)
        {
            if (!_game.GetCurrentPlayer().ScottyOpen)
                _game.Coils["topDropTarget"].Enable();
            else
                _game.Coils["topDropTarget"].Disable();

            return SWITCH_STOP;
        }

        public bool sw_kickOut_active(Switch sw)
        {
            _game.Coils["kickback"].Pulse(150);
            //self.game.game_data['Audits']['Tree Kick'] += 1

   //         if self.game.trophyMode.checkTrophyComplete('Tree Love'):
			//total = self.game.user_profiles['Audits']['Tree Kick']

   //         if total >= 100:
			//	self.game.trophyMode.layer = self.game.trophyMode.buildLayerScript('Tree Love')

   //             self.game.trophyMode.updateTrophy('Tree Love')

            if (_game.TroughMode.num_balls_in_play == 1)
            {
                _game.DisableAllLamps();
                _game.LampCtrl.play_show("woodsKickout", false, new AnonDelayedHandler(_game.update_lamps));
            }

            return SWITCH_STOP;
        }
        public bool sw_leftEject_active_for_1s(Switch sw)
        {            
            var player = _game.GetCurrentPlayer();
            if (player.ModeActive || MultiBallActive)
                _game.Coils["leftEject"].Pulse();
            else
            {
                _game.TargetsMode.layer = null;
                _game.bonus(5000);

                if (player.BookModesAttempted.Values.Contains(false))
                    _game.Modes.Add(_game.SelectMode);
                else if (!player.DeadByDawnComplete)
                {
                    _game.Modes.Add(_game.DeadByDawnMode);

                    var completeCountBonus = player.BookModesComplete
                        .Where(x => x.Value == true).Count();

                    _game.DeadByDawnMode._multiplier = completeCountBonus;
                }
                else
                {
                    _game._sound.PlaySound("NoiseWheel");
                    _game.Coils["leftEject"].Pulse();
                    _game.update_lamps();
                }                                    
            }

            return SWITCH_CONTINUE;
        }
        public bool sw_vukswitch_active(Switch sw)
        {
            //TODO - Turn off common  layers

            return SWITCH_STOP;
        }

        /// <summary>
        /// Shed Active
        /// </summary>
        /// <param name="sw"></param>
        /// <returns></returns>
        public bool sw_rightE_active(Switch sw)
        {
            //self.game.game_data['Audits']['Shed Scoop'] += 1
            //self.game.user_profiles['Audits']['Shed Scoop'] += 1

            //if self.game.wizardMode.is_started():
            //return

            //if self.game.modeEnd.is_started():               
            //self.game.modes.remove(self.modeEnd)

            var modeActive = _game.GetCurrentPlayer().ModeActive;

            if (_game.TroughMode.num_balls_in_play == 1 && !modeActive && !_game.PagesMode.IsStarted())
                _game._sound.PauseMusic();

            if (modeActive)
            {
                _game.score(50000);
                _game.bonus(1000);
            }
            else
            {                
                var shedOpen = AssetService.Videos["ShedOpen"];
                _game.DisableAllLamps();
                _game._sound.PlaySound("ShedOpen");
                layer = shedOpen;
                shedOpen.reset();
            }

            if (_game.PagesMode.IsStarted() && !MultiBallActive && !modeActive)
                //self.game.pausePages(True)
                _game._sound.StopMusic();

            return SWITCH_CONTINUE;
        }
        public bool sw_rightE_active_for_1500ms(Switch sw)
        {
            //if self.game.wizardMode.is_started() or self.game.ShedMultiball.is_started():            
            //{
            //    DriveModeFlasher("rightFlash", "medfast", false, 1);
            //    delay("shedKickDelay", NetProcgame.NetPinproc.EventType.None, 1, new AnonDelayedHandler(ShedKickout));
            //}

            var currentPlayer = _game.GetCurrentPlayer();

            if (!currentPlayer.ModeActive)
            {
                if (!currentPlayer.LockReady)
                {
                    // Add a hit to the shed gun targets                 
                    var letter = AwardShedHit(currentPlayer);
                    _game.WorkshedMode.GenerateShedLockLayer(letter.ToString());
                    _game.TargetsMode.CheckMultiballReady();


                    EnableShedKickout();
                    _game._sound.PlaySound("shedExit");
                }
                else
                {
                    layer = null;
                    _game.score(500000);
                    currentPlayer.ResetGunSawVars();
                    _game.WorkshedMode.ShedKickerCheck();
                }

                return SWITCH_STOP;
            }
            else
            {
                EnableShedKickout();
            }

            return SWITCH_CONTINUE;
        }

        public bool sw_rightE_inactive_for_50ms(Switch sw)
        {
            if (!MultiBallActive && _game.PagesMode.IsStarted())
                _game.PagesMode.PausePages(true);
            else if (_game.TroughMode.num_balls_in_play == 1
                && !_game.GetCurrentPlayer().ModeActive)
                _game._sound.UnPauseMusic();

            return SWITCH_CONTINUE;
        }
        public bool sw_topRolloverL_active(Switch sw)
        {
            //self.game.game_data['Audits']['Ash'] += 1
            //self.game.user_profiles['Audits']['Ash'] += 1
            _game.update_lamps();
            _game.bonus(1000);
            _game.score(10000);

            return SWITCH_CONTINUE;
        }
        public bool sw_topRolloverM_active(Switch sw)
        {
  //          self.game.game_data['Audits']['aSh'] += 1

//            self.game.user_profiles['Audits']['aSh'] += 1
            _game.update_lamps();
            _game.bonus(1000);
            _game.score(10000);

            return SWITCH_CONTINUE;
        }
        public bool sw_topRolloverR_active(Switch sw)
        {
            // self.game.game_data['Audits']['asH'] += 1
            //self.game.user_profiles['Audits']['asH'] += 1

            _game.update_lamps();
            _game.bonus(1000);
            _game.score(10000);

            return SWITCH_CONTINUE;
        }
        public bool sw_rightOrbit_active(Switch sw)
        {
            // self.game.game_data['Audits']['Right Orbit'] += 1
            //self.game.user_profiles['Audits']['Right Orbit'] += 1

            _game.bonus(2500);
            _game.score(50000);
            _game.update_lamps();

            return SWITCH_CONTINUE;
        }
        public bool sw_Target1_active(Switch sw)
        {
            _game.TargetsScoreHit();

            return SWITCH_CONTINUE;
        }
        public bool sw_Target2_active(Switch sw)
        {
            _game.TargetsScoreHit();

            return SWITCH_CONTINUE;
        }
        public bool sw_Target3_active(Switch sw)
        {
            _game.TargetsScoreHit();

            return SWITCH_CONTINUE;
        }
        public bool sw_Target4_active(Switch sw)
        {
            _game.TargetsScoreHit();

            return SWITCH_CONTINUE;
        }
        public bool sw_Target5_active(Switch sw)
        {
            _game.TargetsScoreHit();

            return SWITCH_CONTINUE;
        }
        public bool sw_Target6_active(Switch sw)
        {
            _game.TargetsScoreHit();

            return SWITCH_CONTINUE;
        }
        #endregion

        private void StartingWizard()
        {
            throw new NotImplementedException();
        }

        private void CheckForFreeScotty()
        {
            
        }

        private void BallSaved()
        {
            //     self.game.log("BaseGameMode: BALL SAVED from Trough callback")
            //     if self.game.wizardMode.is_started() or self.game.deadByDawn.is_started() or self.game.getPlayerState('mode_active'):
            //self.game.coils.plunger.pulse(50)

            if (!MultiBallActive)
            {
                var txtScriptLayer =
                _game.DisplayHelper.GenerateScriptedMultiTextLayer("WE|LIVE|STILL", _game.Width, _game.Height, 0.7, "EDLarge", "redYellow");

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

        public void RemoveModeInfoLayer()
        {

        }

        private void PlayIdleSound()
        {
            throw new NotImplementedException();
        }

        private void ShedKickout()
        {
            _game.Coils["rightEject"].Pulse();
            _game.update_lamps();
        }

        private void ReturnLaneHelper(string sound)
        {
            _game.bonus(5000);

            if (_game.TroughMode.num_balls_in_play == 1 && !_game.GetCurrentPlayer().ModeActive)
                _game._sound.PlaySound(sound);

            delay("delayed_sound_trigger", NetProcgame.NetPinproc.EventType.None, 7,
                new AnonDelayedHandler(DelayedSoundTrigger));

            _game.update_lamps();
        }

        private void PlayDrainLayersIfReady()
        {
            if (!MultiBallActive && !_game.ball_save.is_active())
            {
                var intoStairs = AssetService.Videos["intoStairs"];
                layer = intoStairs;
                _game._sound.PlaySound("OutLane");
                _game._sound.PlaySound("ASH_STRUGGLE");
                intoStairs.reset();
            }

        }
        private void DelayedSoundTrigger()
        {
            cancel_delayed("delayed_sound_trigger");

            _game._sound.PlaySound("random_voice");
        }

        private char AwardShedHit(EdPlayer currentPlayer)
        {
            char[] letters = new char[] { 'S', 'A', 'W', 'G', 'U', 'N' };

            var lTargets = currentPlayer.TargetBankLeft;
            var rTargets = currentPlayer.TargetBankRight;
            char letter = 'S';

            if (!lTargets[0])
            { lTargets[0] = true; }
            else if (!lTargets[2])
            { lTargets[2] = true; letter = letters[2]; }
            else if (!lTargets[1])
            {
                lTargets[1] = true;
                letter = letters[1];
                currentPlayer.SawReady = true;
            }
            else if (!rTargets[0])
            { rTargets[0] = true; letter = letters[3]; }
            else if (!rTargets[2])
            { rTargets[2] = true; letter = letters[5]; }
            else if (!rTargets[1])
            {
                rTargets[1] = true;
                letter = letters[4];
                currentPlayer.GunReady = true;
            }

            currentPlayer.TargetBankLeft = lTargets;
            currentPlayer.TargetBankRight = rTargets;

            return letter;

        }

        private void EnableShedKickout(double delayTime = 1.0)
        {
            DriveModeFlasher("rightFlash", "medium", false, (int)delayTime);

            delay("shedKickout", NetProcgame.NetPinproc.EventType.None, delayTime, new AnonDelayedHandler(ShedKickout));
        }
        private void EjectScottyKicker()
        {
            _game.Coils["topDropTarget"].Disable();
            _game.Coils["scottyKick"].Pulse();
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

        public GroupedLayer SetStatus(string text1, string text2,
            int seconds = 2,
            string font = "default",
            string style1 = "redYelThin", string style2 = "yelRedThin",
            int topTextX = 558 / 2, int topTextY = 90,
            int bottomTextX = 558 / 2, int bottomTextY = 220, bool composite = true
            )
        {

            var topText1 = new SdlTextLayer(topTextX, topTextY, font, FontJustify.Center, FontJustify.Center, style1);
            var topText2 = new SdlTextLayer(topTextX, topTextY, font, FontJustify.Center, FontJustify.Center, style2);
            var botText1 = new SdlTextLayer(bottomTextX, bottomTextX, font, FontJustify.Center, FontJustify.Center, style1);
            var botText2 = new SdlTextLayer(bottomTextX, bottomTextX, font, FontJustify.Center, FontJustify.Center, style2);

            topText1.SetText(text1, seconds);
            topText2.SetText(text1, seconds, 2);
            botText1.SetText(text2, seconds);
            botText2.SetText(text2, seconds, 2);

            var layers = new List<Layer>() { topText1, topText2, botText1, botText2 };

            GroupedLayer group;
            if (!composite)
                group = new GroupedLayer(_game.Width, _game.Height, layers);
            else
            {
                group = new GroupedLayer(_game.Width, _game.Height, layers, true);
            }

            return group;
        }

        //  def sw_up_active(self, sw):

        //      volume = self.game.sound.volume_up()
        //self.game.set_status("Volume Up : " + str(volume))
        //return True
    }
}
