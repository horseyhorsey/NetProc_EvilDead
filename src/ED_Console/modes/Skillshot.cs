using NetProcgame.Display.Layers;
using NetProcgame.Game;
using NetProcgame.Services;
using NetProcgame.Tools;
using System.Collections.Generic;
using System.Linq;

namespace ED_Console.Modes
{
    public class Skillshot : Mode
    {
        Game _game;

        ScriptedLayer scriptLayer;

        public Skillshot(Game game, int priority) 
            : base(game, priority)
        {
            _game = game;

            scriptLayer = BuilSkillLayers();
        }

        public override void mode_started()
        {
            AssetService.Sounds["AshReady"].PlaySound(0, 0);

            this.layer = scriptLayer;
        }

        private ScriptedLayer BuilSkillLayers()
        {
            var anim = AssetService.Animations["DeerWall"];

            var skillBonusText10K = new SdlTextLayer(150, 210,"ed_common", FontJustify.Center,false);
            skillBonusText10K.SetText("10,000", -1, 2, AssetService.Styles["redYellow"]);

            var skillBonusText1M = _game.DisplayHelper.GenerateLayer("1,000,000", font: "ed_common", style: AssetService.Styles["redYellow"]);
            var skillBonus100K = _game.DisplayHelper.GenerateLayer("100,000", font: "ed_common", style: AssetService.Styles["redYellow"]);
            
            var skillBonusLeft10k = new GroupedLayer(HorseGame.DisplayConfig.DotsW, HorseGame.DisplayConfig.DotsH, new List<Layer>() { anim, skillBonusText10K });

            var skillBonusMid1M = new GroupedLayer(HorseGame.DisplayConfig.DotsW, HorseGame.DisplayConfig.DotsH, new List<Layer>() { anim, skillBonusText1M });

            var skillBonus100KR = new GroupedLayer(HorseGame.DisplayConfig.DotsW, HorseGame.DisplayConfig.DotsH, new List<Layer>() { anim, skillBonus100K });                   

            var script = new List<Pair<double, Layer>>();

            script.Add(new Pair<double, Layer>(1, skillBonusLeft10k));
            script.Add(new Pair<double, Layer>(1, skillBonusMid1M));
            script.Add(new Pair<double, Layer>(1, skillBonus100KR));

            var scriptLayer = new ScriptedLayer(HorseGame.DisplayConfig.DotsW, HorseGame.DisplayConfig.DotsH, script);

            return scriptLayer;
        }

        public override void mode_stopped()
        {
            
        }

        public override void update_lamps()
        {
            
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

        public bool sw_shooter_active(Switch sw)
        {
            _game.Modes.Remove(this);

            return SWITCH_STOP;
        }
    }
}
