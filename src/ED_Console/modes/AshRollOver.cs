using ED_Console;
using NetProcgame.Display.Layers;
using NetProcgame.Game;
using NetProcgame.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFontTest.EvilDead.Modes
{
    public class AshRollover : Mode
    {
        #region Properties
        private byte _lanesEnabledCount;
        private byte[] _ashLanes = new byte[3];
        private byte _bonusX = 0;
        private Game _game;
        private object _lastSwitch;
        Layer AshBonusLayer;
        #endregion

        public AshRollover(Game game, int priority) : 
            base(game, priority)
        {
            _game = game;

            AshBonusLayer = AssetService.Animations["ashBonus"];

        }

        public override void mode_started()
        {
            _lanesEnabledCount = 0;
            _bonusX = 0;

            var anim = AssetService.Animations["light_texture"];
            //anim.hold = true;

            var text = new SdlTextLayer(0, 0,fontNamed: "default",justify: FontJustify.Center);

            var layers = new List<Layer>();
            layers.Add(anim);
            layers.Add(text);

            var group = new GroupedLayer(558, 300, layers);

            this.layer = anim;
        }

        public override void mode_stopped()
        {
            
        }

        public override void update_lamps()
        {
            
        }

        public bool sw_flipperLwL_active(Switch sw)
        {
            if (_ashLanes[0] == 1)
            { _ashLanes[0] = 0; _ashLanes[2] = 1; }
            else if (_ashLanes[2] == 1)
            { _ashLanes[2] = 0; _ashLanes[1] = 1; }
            else if (_ashLanes[1] == 1)
            { _ashLanes[1] = 0; _ashLanes[0] = 1; }
            else if (_ashLanes[0] == 0)
            { _ashLanes[0] = 1; _ashLanes[0] = 1; }

            return SWITCH_CONTINUE;
        }

        public bool sw_topRolloverL_active(Switch sw)
        {
            AshRolloverHandler(1);

            return SWITCH_CONTINUE;
        }
        public bool sw_topRolloverM_active(Switch sw)
        {
            AshRolloverHandler(2);

            return SWITCH_CONTINUE;
        }
        public bool sw_topRolloverR_active(Switch sw)
        {
            AshRolloverHandler(3);

            return SWITCH_CONTINUE;
        }

        private void AshRolloverHandler(int num)
        {
            _lanesEnabledCount++;
            _lastSwitch = num;
            //if self.game.wizardMode.is_started():
            var player = _game.GetCurrentPlayer();
            if (player.ScottyOpen || player.CardsEnabled
                && !_game.Switches["shooter"].IsActive())
                _game.Coils["topDropTarget"].Disable();

            cancel_delayed("clearDMD");

            switch (num)
            {
                case 1:
                    SetTextLayer("A");
                    _game.Lamps["Top_A"].Enable();
                    break;
                case 2:
                    SetTextLayer("S");
                    _game.Lamps["Top_S"].Enable();
                    break;
                case 3:
                    SetTextLayer("H");
                    _game.Lamps["Top_H"].Enable();
                    break;
                default:
                    break;
            }

            LaneCheck();

        }

        private void LaneCheck()
        {
            cancel_delayed("clearDMD");

            if (_lanesEnabledCount >=3)
            {
                _game.DisableAllLamps();
                _game._sound.PlaySound("AshBonus");
                //self.game.user_profiles['Multipliers']['Complete'] += 1
                if (_bonusX <5)
                {
                    _bonusX++;
                    var bonusStr = (_bonusX + 1) + "X";
                    var textLayer =
                         _game.BaseMode.SetStatus("A S H", bonusStr, 2, "ed_targets");
                    var layers = new List<Layer>() { AshBonusLayer, textLayer };
                    var group = new GroupedLayer(_game.Width, _game.Height, layers);

                    layer = group;
                }
                else
                {

                }
            }
            else
            {
                _game._sound.PlaySound("ChezLaff");
                _game.DisableAllLamps();
                _game.LampCtrl.play_show("attract_show_2");
            }

        }

        private void SetTextLayer(string v)
        {
            throw new NotImplementedException();
        }
    }
}
