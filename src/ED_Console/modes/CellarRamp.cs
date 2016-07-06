using NetProcgame.Display.Layers;
using NetProcgame.Game;
using NetProcgame.Services;
using NetProcgame.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ED_Console.Modes
{
    public class CellarRamp : Mode
    {
        private int _rampValue = 250000;
        private int _count;
        private int _lockedBalls = 0;
        private bool _mBallReady = false;
        private bool _hatchOpen = false;
        private object _hit;
        private bool _videosRunning = false;

        SdlTextLayer Text1;
        SdlTextLayer Text2;
        SdlTextLayer Text3;
        SdlTextLayer Text4;

        VideoLayer CellarRampVid;
        VideoLayer CellarOpen;

        AnonDelayedHandler ClearDmdHandler;
        AnonDelayedHandler KickSuperJackpot;
        AnonDelayedHandler CheckCellarDelay;

        Game _game;

        public CellarRamp(Game game, int priority) : base(game, priority)
        {            
            Text1 = new SdlTextLayer(15, 250, "pinregsmall", FontJustify.Left, FontJustify.Center);
            Text2 = new SdlTextLayer(500, 250, "pinregsmall", FontJustify.Left, FontJustify.Center);
            Text3 = new SdlTextLayer(15, 200, "pinregsmall", FontJustify.Left, FontJustify.Center);
            Text4 = new SdlTextLayer(15, 250, "pinregsmall", FontJustify.Left, FontJustify.Center);

            CellarRampVid = AssetService.Videos["CellarRamp"];
            CellarOpen = AssetService.Videos["CellarOpen"];

            ClearDmdHandler = new AnonDelayedHandler(this.ClearDmd);
            KickSuperJackpot = new AnonDelayedHandler(this.KickSuperJack);
            CheckCellarDelay = new AnonDelayedHandler(this.CheckCellarDone);

            _game = game;
        }        

        private void KickSuperJack()
        {
            throw new NotImplementedException();
        }

        private void ClearDmd()
        {
            layer = null;
        }

        public override void mode_started()
        {
            var edPlayer = _game.GetCurrentPlayer();
            _count = edPlayer.CellarRampCount;
            _lockedBalls = edPlayer.CellarLocked;

            if (edPlayer.CellarMultiBallReady)
                _game.CellarHatch(true);

            _videosRunning = false;
        }

        public override void mode_stopped()
        {
            var edPlayer = _game.GetCurrentPlayer();
            edPlayer.CellarRampCount = _count;
        }

        public bool sw_cellarRamp_active(Switch sw)
        {
            //self.game.game_data['Audits']['Cellar Ramps'] += 1
            //self.game.user_profiles['HouseRamp']['Ramps Made'] += 1
            //         if self.game.trophyMode.checkTrophyComplete('Cabin fever'):
            //if self.game.user_profiles['HouseRamp']['Ramps Made'] >= 100:
            //	self.game.trophyMode.layer = self.game.trophyMode.buildLayerScript('Cabin fever')

            //       self.game.trophyMode.updateTrophy('Cabin fever')
            //     if self.game.wizardMode.is_started():
            //return

            if (_lockedBalls == 0)
            {
                if (_game.GetCurrentPlayer().CellarMultiBallReady)
                {
                    if (_count < 6)
                        _count++;

                    _game.score(450000);

                    _game.Coils["flasherHouse"].Schedule(0x000000CC, 4, false);

                    if (Enumerable.Range(1, 6).Contains(_count))
                    {
                        var group = GetCellarLayers();

                        var script = new List<Pair<double, Layer>>();
                        script.Add(new Pair<double, Layer>(1.0, group[0]));
                        script.Add(new Pair<double, Layer>(1.0, group[1]));

                        var scriptLayer = new ScriptedLayer(_game.Width, _game.Height, script);

                        _game._sound.PlaySound("soundsCellar");

                        var grouped = new GroupedLayer(_game.Width, _game.Height, new List<Layer>()
                    {
                        CellarRampVid, scriptLayer
                    });

                        CellarRampVid.reset();

                        layer = grouped;

                        delay("stop", NetProcgame.NetPinproc.EventType.None, 2, ClearDmdHandler);
                    }
                    else if (_count == 6)
                        OpenFirstLock();

                    //                if not self.game.modeEnd.is_started() and not self.game.getPlayerState('mode_active') and not self.game.base_game_mode.any_mb_active2():
                    //self.game.lampctrl.play_show('BridgeRamp', False, self.game.update_lamps)
                }
            }


            return SWITCH_CONTINUE;
        }

        private void OpenFirstLock()
        {
            cancel_delayed("stop");

            Text1.SetText("cellar open", 2);

            var group = new GroupedLayer(_game.Width, _game.Height, new List<Layer>() { CellarOpen, Text1 });

            layer = group;

            _game.GetCurrentPlayer().CellarMultiBallReady = true;

            delay("stop", NetProcgame.NetPinproc.EventType.None, 2, ClearDmdHandler);

            _hatchOpen = true;
            _game.Coils["cellarHatch"].Enable();

        }

        private GroupedLayer[] GetCellarLayers()
        {

            Text1.SetText("cellar ramps");
            Text2.SetText(" " + _count);
            Text3.SetText("complete to open");
            Text4.SetText("cellar locks");

            GroupedLayer[] group = new GroupedLayer[2];

            var group1 = new GroupedLayer(_game.Width, _game.Height, new List<Layer>(2) { Text1, Text2 });

            var group2 = new GroupedLayer(_game.Width, _game.Height, new List<Layer>(2) { Text3, Text4 });

            group[0] = group1;
            group[1] = group2;

            return group;

        }

        public override void update_lamps()
        {
            if (!_game.GetCurrentPlayer().ModeActive)
            {
                if (_count == 0)
                    _game.Lamps["Cellar"].Schedule(0x0F0F0F0F);
                else if (_count == 1)
                {
                    _game.Lamps["Cellar"].Enable();
                    _game.Lamps["cEllar"].Schedule(0x0F0F0F0F);
                }
                else if (_count == 2)
                {
                    _game.Lamps["Cellar"].Enable();
                    _game.Lamps["cEllar"].Enable();
                    _game.Lamps["ceLlar"].Schedule(0x0F0F0F0F);
                }
                else if (_count == 3)
                {
                    _game.Lamps["Cellar"].Enable();
                    _game.Lamps["cEllar"].Enable();
                    _game.Lamps["ceLlar"].Enable();
                    _game.Lamps["celLar"].Schedule(0x0F0F0F0F);
                }
                else if (_count == 4)
                {
                    _game.Lamps["Cellar"].Enable();
                    _game.Lamps["cEllar"].Enable();
                    _game.Lamps["ceLlar"].Enable();
                    _game.Lamps["celLar"].Enable();
                    _game.Lamps["cellAr"].Schedule(0x0F0F0F0F);
                }
                else if (_count == 5)
                {
                    _game.Lamps["Cellar"].Enable();
                    _game.Lamps["cEllar"].Enable();
                    _game.Lamps["ceLlar"].Enable();
                    _game.Lamps["celLar"].Enable();
                    _game.Lamps["cellAr"].Enable();
                    _game.Lamps["cellaR"].Schedule(0x0F0F0F0F);
                }
                else if (_count == 6)
                {
                    _game.Lamps["Cellar"].Enable();
                    _game.Lamps["cEllar"].Enable();
                    _game.Lamps["ceLlar"].Enable();
                    _game.Lamps["celLar"].Enable();
                    _game.Lamps["cellAr"].Enable();
                    _game.Lamps["cellaR"].Enable();
                    _game.Lamps["ArrowCellar"].Schedule(0x0F0F0F0F);
                }
            }

        }

        public bool CellarIsReady()
        {
            if (_lockedBalls == 2)
            {
                _game._sound.FadeOutMusic(50);
                _game._sound.PlayMusic("MockingBird", -1);
                return true;
            }
            else
                return false;        
        }
        private void CheckCellarDone()
        {
            layer = null;
            _videosRunning = false;
            _game.Coils["vuk"].Pulse();

            if (_lockedBalls == 3)
            {
                _lockedBalls = 0;
                _count = 0;
                _game.Modes.Add(_game.CellarMultiBall);
                _game.GetCurrentPlayer().CellarMultiBallReady = false;
            }
            else
            {
                _hatchOpen = true;
                _game.CellarHatch(true);
                _game._sound.UnPauseMusic();
            }

            //PausePages(false);

        }

        private bool sw_vukswitch_active(Switch sw)
        {
            //if self.game.getPlayerState('wizardReady'):
			//self.game.modes.add(self.game.wizardMode)

            if (_game.CellarMultiBall.IsStarted())
            {
                _game.Coils["cellarHatch"].Disable();
                _game.CellarMultiBall.layer =
                    AssetService.Videos["superJackpot"];

                //self.game.user_profiles['CellarMultiball']['Jackpots Complete'] += 1

                delay("superJack", NetProcgame.NetPinproc.EventType.None, 2, KickSuperJackpot);
            }
            else
            {
                CheckCellar();
            }

            return SWITCH_STOP;
        }
        private void sw_vukswitch_inactive_for_10ms(Switch sw)
        {
            //self.game.game_data['Audits']['Cellar VUK'] += 1
            //self.game.base_game_mode.drive_mode_flasher("flasherEye", "medfast", True, 2.0)

            if (_game.CellarMultiBall.IsStarted())
            {
                layer = AssetService.Videos["EyeBall"];
                _game._sound.PlaySound("EyeBall");
            }
            else
            {
                if (_lockedBalls == 2)
                {
                    CellarIsReady();
                    _game._sound.PlaySound("fruitCellar");
                }
                else
                    _game._sound.PlaySound("henriettaRReverb");                
            }

            _game.update_lamps();

        }
        private void sw_flipperLwL_active_for_1s(Switch sw)
        {
            if (_videosRunning)
            {
                _game.LampCtrl.stop_show();
                _game.update_lamps();
                cancel_delayed("cellarAnims");
                layer = null;

                switch (_lockedBalls)
                {
                    case 1:
                        _game._sound.StopSound("Cellar1-SonOfABitch");
                        _game._sound.PlaySound("Cellar1-Short");
                        break;
                    case 2:
                        _game._sound.StopSound("Cellar2-LetMeOut");
                        _game._sound.PlaySound("Cellar2-Short");
                        break;
                    case 3:
                        _game._sound.StopSound("Cellar3-Henrietta");
                        _game._sound.PlaySound("Cellar3-Short");
                        break;
                    default:
                        break;
                }

                CheckCellarDone();
            }
        }

        private void CheckCellar()
        {
            //PausePages();

            cancel_delayed("stop");

            if (!_game.ScottyMultiBall.IsStarted()
                || !_game.ShedMultiball.IsStarted())
                _game._sound.PauseMusic();

            if (_lockedBalls < 3)
                _lockedBalls++;

            //self.game.user_profiles['CellarMultiball']['Locks Completed'] += 1
            //self.game.setPlayerState('cellarLocked', self.locked_balls)

            if (_game.ScottyMultiBall.IsStarted() || _game.ShedMultiball.IsStarted())
                delay("checkCellar", NetProcgame.NetPinproc.EventType.None, 1, CheckCellarDelay);
            else
                PlayMultiBallScenes();
        }

        private void PlayMultiBallScenes()
        {
            _videosRunning = true;

            var skipInfo = new SdlTextLayer(_game.Width, _game.Height, "med",
                FontJustify.Left, FontJustify.Bottom, AssetService.Styles["redYellow"]);

            skipInfo.SetText("hold flipper to skip",blink_frames:2);

            _game.DisableAllLamps();

            _game.LampCtrl.stop_show();

            VideoLayer anim = new VideoLayer();
            ScriptedLayer textLockScript;

            if (_lockedBalls != 0)
            {
                _game.LampCtrl.play_show("cellarLock1");
                _game.CellarHatch(false);

                var strtext = string.Format("Cellar Multiball | Ball {0} Locked", _lockedBalls);
                textLockScript = _game.DisplayHelper.GenerateScriptedTextLayer(
                    strtext, _game.Width, _game.Height, 2.0, "ed_targets", AssetService.Styles["redYellow"]);
                textLockScript.set_target_position(36, 100);

                var time = 0.0;
                switch (_lockedBalls)
                {
                    
                    case 1:
                        anim = AssetService.Videos["Cellar1-SonOfABitch"];
                        _game._sound.PlaySound("Cellar1-SonOfABitch");
                        time = 7.5;
                        break;
                    case 2:
                        anim = AssetService.Videos["Cellar2-LetMeOut"];
                        _game._sound.PlaySound("Cellar2-LetMeOut");
                        time = 3.5;
                        break;
                    case 3:
                        anim = AssetService.Videos["Cellar3-Henrietta"];
                        _game.LampCtrl.play_show("CellarMultiball");
                        _game._sound.PlaySound("Cellar3-Henrietta");
                        time = 9.0;
                        break;
                    default:
                        break;
                }

                delay("cellarAnims", NetProcgame.NetPinproc.EventType.None, time, CheckCellarDelay);

                var grouped = new GroupedLayer(_game.Width, _game.Height, new List<Layer>() { anim, textLockScript });
                layer = grouped;
                anim.reset();                                
            }

        }
    }
}
