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
        #endregion

        public AshRollover(GameController game, int priority) : 
            base(game, priority)
        {

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
            return SWITCH_CONTINUE;
        }
    }
}
