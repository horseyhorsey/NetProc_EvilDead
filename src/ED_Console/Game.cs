using NetProcgame.Game;
using NetProcgame.NetPinproc;
using NetProcgame.Tools;
using NetProcgame.Modes;
using ED_Console.Modes;
using System.Collections.Generic;
using System;

namespace ED_Console
{
    public class Game : HorseGame
    {
        #region Modes
        public BaseMode BaseMode;
        public Skillshot SkillshotMode;
        public PageMode PagesMode;
        public CellarMBall CellarMultiBall;
        public ScottyMBall ScottyMultiBall;
        public ShedMball ShedMultiball;
        public CellarRamp CellarRampMode;
        #endregion

        public List<EdPlayer> EdPlayers;
        
        public Game(MachineType machine_type, ILogger logger) :
            base(machine_type, logger)
        {

            this._game_data = new GameData();
            EdPlayers = new List<EdPlayer>();

            ScoreDisplay = new ScoreDisplay(this, 1);
            ScoreDisplay.layer.enabled = false;
            BaseMode = new BaseMode(this, 2);
            AttractMode = new Attract(this, 20);
            SkillshotMode = new Skillshot(this, 3);
            PagesMode = new PageMode(this,5);
            CellarRampMode = new CellarRamp(this, 6);

            Modes.Add(ScoreDisplay);
            Modes.Add(BaseMode);
            Modes.Add(AttractMode);

        }

        internal void CellarHatch(bool enable)
        {
            if (enable)
                this.Coils["cellarHatch"].Enable();
            else
                this.Coils["cellarHatch"].Disable();
        }

        public EdPlayer AddEdPlayer()
        {
            base.AddPlayer();

            var edPlayer = CreateEdPlayer("Player " + (_players.Count).ToString());

            EdPlayers.Add(edPlayer);            

            return edPlayer;
        }

        public EdPlayer CreateEdPlayer(string name)
        {
            return new EdPlayer(name);
        }

        public EdPlayer GetCurrentPlayer()
        {
            return this.EdPlayers[this.current_player_index];
        }

        internal void DisableAllLamps()
        {
            foreach (var lamp in this.Lamps)
            {
                lamp.Value.Disable();
            }
        }
    }
}
