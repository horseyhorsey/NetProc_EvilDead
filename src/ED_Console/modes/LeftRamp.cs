using NetProcgame.Display.Layers;
using NetProcgame.Game;
using NetProcgame.NetPinproc;
using NetProcgame.Services;
using NetProcgame.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ED_Console.modes
{
    public class LeftRamp : Mode
    {
        public int RampValue { get; set; }

        private int[] _pageRange;
        private int[] _scottyRange;
        private int[] _bonusRange;
        private int[] _extraBallRange;

        public int RampCount;

        VideoLayer CarPortal;
        VideoLayer BridgeDivert;
        VideoLayer BookOpen;

        SdlTextLayer _Text1;
        SdlTextLayer _Text2;
        SdlTextLayer ComboText;

        AnonDelayedHandler ClearDmdHandler;

        Game _game;

        public LeftRamp(Game game, int priority) : base(game, priority)
        {
            _game = game;

            ClearDmdHandler = new AnonDelayedHandler(this.ClearDmd);

            CarPortal = AssetService.Videos["CarPortal"];
            BridgeDivert = AssetService.Videos["BridgeDivert"];
            BookOpen = AssetService.Videos["BookOpen"];

            PopulateTextLayers(ref game);

            _pageRange = Range.GetRange(3, 533, 10);
            _scottyRange = Range.GetRange(5, 535, 10);
            _bonusRange = Range.GetRange(6, 536, 10);
            _extraBallRange = Range.GetRange(10, 540, 10);

        }

        #region SwitchEvents
        private bool sw_rampDiverted_active()
        {
            //if self.game.wizardMode.is_started():
            //    ;

            _game._sound.PlaySound("CarTires");
            cancel_delayed("clearDmd");

            var edPlayer = _game.GetCurrentPlayer();
            if (!edPlayer.ModeActive)
            {
                edPlayer.RampCount++;
                //self.game.game_data['Audits']['Wireform Diverted'] += 1
                _Text1.SetText(edPlayer.RampCount.ToString(), 2);
                _Text2.SetText("Bridge Ramps", 2);

                _Text1.set_target_position(_game.game_config.DotsW / 2, 215);
                _Text2.set_target_position(_game.game_config.DotsW / 2, 100);

                _game.LampCtrl.play_show("BridgeDivert", false);

                RampAwards(_game.Switches["rampDiverted"]);

                return SWITCH_STOP;
            }
            else return SWITCH_CONTINUE;

        }
        private bool sw_leftRampFull_active()
        {
            var edPlayer = _game.GetCurrentPlayer();
            if (edPlayer.ModeActive)
            {
                cancel_delayed("clearDmd");
                _game._sound.PlaySound("TruckEngine");
                edPlayer.RampCount++;

                _Text1.SetText(edPlayer.RampCount.ToString(), 2);
                _Text1.set_target_position(_game.game_config.DotsW / 2, 215);

                _Text2.SetText("bridge ramps", 2);
                _Text2.set_target_position(_game.game_config.DotsW / 2, 100);

                delay("clearDMD", EventType.None, 2, ClearDmdHandler);

                RampAwards(_game.Switches["leftRampFull"]);

                return SWITCH_STOP;
            }
            else


                //if (!_game.ModeEnd.IsStarted() && !edPlayer.ModeActive && !_game.BaseMode.MultiBallActive)
                //    _game.LampCtrl.play_show("BridgeRamp");


                return SWITCH_CONTINUE;

        }
        private bool sw_leftRamp_active()
        {
            RampEntryHit();
            _game.Coils["flasherCar"].Schedule(0xFFF0FFF0, 2);

            return SWITCH_CONTINUE;
        }
        #endregion

        #region Awards
        private void RampAwards(Switch sw)
        {
            CheckCombo();

            layer = ChangeBackGround(BridgeDivert);

            var edPlayer = _game.GetCurrentPlayer();
            var rampCount = edPlayer.RampCount;

            //         if self.game.trophyMode.checkTrophyComplete('Toll Bridge'):
            //total = self.game.user_profiles['Audits']['Bridge Ramp Full'] + self.game.user_profiles['Audits']['Wireform Diverted']

            //         if total >= 100:
            //	self.game.trophyMode.layer = self.game.trophyMode.buildLayerScript('Toll Bridge')

            //             self.game.trophyMode.updateTrophy('Toll Bridge')


            if (_game.BaseMode.MultiBallActive)
                _game.score(500000 * edPlayer.MultiballScoring);
            else
                _game.score(250000);

            if (_pageRange.Contains(rampCount))
            {
                if (_game.PagesMode.IsStarted())
                    AwardPages();
             }

            else if (_scottyRange.Contains(rampCount))
                CheckScotty();
            else if (_bonusRange.Contains(rampCount))
            {
                if (edPlayer.LockReady)
                    AwardShed();
            }
            else if (_extraBallRange.Contains(rampCount))
                AwardExtraBall();
            else
            {
                if (sw.Name == "leftRampFull")
                    _game._sound.PlaySound("goodShot");
                else
                    _game._sound.PlaySound("Broken");
            }

            delay("clearDMD", EventType.None, 2, ClearDmdHandler);

        }

        private void AwardPages()
        {
            throw new NotImplementedException();
        }

        private void AwardExtraBall()
        {
            _Text1.SetText(RampCount.ToString(), 2);
            _Text1.set_target_position(_game.Width, 215);

            _Text2.SetText("EXTRA BALL LIT", 2);
            _Text2.set_target_position(_game.Width, 100);

            delay("clearDMD", EventType.None, 2, ClearDmdHandler);

            //self.game.base_game_mode.light_extra_ball()
        }
        private void AwardShed()
        {
            var edPlayer = _game.GetCurrentPlayer();

            var shedLocks = edPlayer
                .WorkshedsLocked;

            if (shedLocks.Contains(false))
            {
                _Text1.SetText(RampCount.ToString(), 2);
                _Text1.set_target_position(_game.Width, 215);

                _Text2.SetText("SHED OPEN", 2);
                _Text2.set_target_position(_game.Width, 100);

                ResetShedLocks();

                if (!shedLocks[0])
                    edPlayer.WorkshedsLocked[0] = true;
                else if (!shedLocks[1])
                    edPlayer.WorkshedsLocked[1] = true;
                else if (!shedLocks[2])
                    edPlayer.WorkshedsLocked[2] = true;
                else
                    _Text1.SetText("Spot Cellar", 2);

                delay("clearDMD", EventType.None, 2, ClearDmdHandler);

            }

        }
        private void AwardScotty()
        {
            _Text1.SetText(RampCount.ToString(), 2);
            _Text1.set_target_position(_game.Width, 215);
            _Text2.SetText("SCOTTY OPEN", 2);
            _Text2.set_target_position(_game.Width, 100);

            var group = ChangeBackGround(AssetService.Animations["scottyStill"]);

            layer = group;

            _game.Coils["topDropTarget"].Disable();
            _game.GetCurrentPlayer().ScottyOpen = true;
            _game.Lamps["ScottyLamp"].Enable();
        }
        #endregion

        public override void mode_stopped()
        {
            layer = null;

            base.mode_stopped();
        }

        private bool CheckDiverter()
        {
            var count = RampCount + 1;

            if (_pageRange.Contains(count))
                return true;
            else return false;
        }

        private void RampEntryHit()
        {
            if (CheckDiverter())
                _game.Coils["diverter"].Enable();
            else
                _game.Coils["diverter"].Disable();

            //if (deadByDawnIsStarted)
            //_game._sound.PlaySound("DownBuild");

            if (_game.BaseMode.MultiBallActive)
            {
                if (_game.Switches["leftRamp"].TimeSinceChange() > 2.3)
                {
                    cancel_delayed("clearLayers");
                    _game._sound.PlaySound("DownBuild");
                    _Text1.SetText("");
                    _Text2.SetText("Bridge Ramp");
                    _Text2.set_target_position(_game.Width / 2, 250);

                    var group = ChangeBackGround(CarPortal);

                    layer = group;

                    CarPortal.reset();

                    delay("clearDMD", EventType.None, 3, ClearDmdHandler);
                }
                else
                {
                    if (_game.TroughMode.num_balls_in_play == 1)
                        _game._sound.PlaySound("ASH_Incoming");
                }
            }

            //self.game.game_data['Audits']['Bridge Ramp Entry'] += 1

        }

        private GroupedLayer ChangeBackGround(Layer layer)
        {
            return new GroupedLayer(_game.game_config.DotsW,
                _game.game_config.DotsH, new List<Layer>()
                { layer, _Text1, _Text2, ComboText });
        }

        private void ResetShedLocks()
        {
            var edPlayer = _game.GetCurrentPlayer();

            for (int i = 0; i < edPlayer.WorkshedsLocked.Length; i++)
            {
                edPlayer.WorkshedsLocked[i] = false;
            }
        }
        private void SetShedStatus()
        {
            for (int i = 0; i < 3; i++)
            {
                _game.GetCurrentPlayer().TargetBankLeft[0] = true;
                _game.GetCurrentPlayer().TargetBankRight[0] = true;
            };

            _game.GetCurrentPlayer().SawReady = true;
            _game.GetCurrentPlayer().GunReady = true;
            _game.GetCurrentPlayer().LockReady = true;

            //_game.TargetsMode.CheckMultiBallReady();

            _game.update_lamps();
        }
        private void CheckScotty()
        {
            var edPlayer = _game.GetCurrentPlayer();

            if (edPlayer.RampCount == 5 && edPlayer.Multiballs[0])
                delay("clearDmd", EventType.None, 2, ClearDmdHandler);
            else if (!edPlayer.ScottyOpen)
                AwardScotty();
            else
                delay("clearDmd", EventType.None, 2, ClearDmdHandler);
        }
        private void CheckCombo()
        {

            if (_game.Switches["rampDiverted"].TimeSinceChange() < 3
                 || _game.Switches["leftRampFull"].TimeSinceChange() < 3)
            {
                ComboText.SetText("ramp combo", 2);

                ComboText.set_target_position(_game.game_config.DotsW / 2, _game.game_config.DotsH);
            }
            else
            {
                ComboText.SetText("", 2);
                ComboText.set_target_position(_game.game_config.DotsW, _game.game_config.DotsH);
            }
        }

        private void PopulateTextLayers(ref Game game)
        {
            _Text1 = new SdlTextLayer(
                _game.game_config.DotsW / 2,
                15, "ed_LeftRamp1",
                FontJustify.Center, FontJustify.Bottom,
                AssetService.Styles["redYellow"]);

            _Text2 = new SdlTextLayer(
                _game.game_config.DotsW / 2,
                15, "ed_LeftRamp1",
                FontJustify.Center, FontJustify.Bottom,
                AssetService.Styles["redYellow"]);

            ComboText = new SdlTextLayer(
                _game.game_config.DotsW / 2,
                _game.game_config.DotsH, "ed_LeftRamp1",
                FontJustify.Center, FontJustify.Bottom,
                AssetService.Styles["redYellow"]);

            resetText();

        }
        private void ClearDmd()
        {
            layer = null;
        }

        private void resetText()
        {
            _Text1.SetText("");
            _Text2.SetText("");
            ComboText.SetText("");
        }
    }
}
