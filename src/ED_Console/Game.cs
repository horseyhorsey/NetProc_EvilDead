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
        public Targets TargetsMode;
        public WorkShed WorkshedMode;
        public Bumpers BumpersMode;
        public DeadByDawn DeadByDawnMode;

        public ModeSelect SelectMode;
        public Linda Lindamode;
        public Shelly ShellyMode;
        public BadHand BadHandMode;
        public Escape EscapeMode;
        public Cheryl CherylMode;
        #endregion

        public List<EdPlayer> EdPlayers;

        public bool TiltStatus { get; internal set; }

        public Game(MachineType machine_type, ILogger logger) :
            base(machine_type, logger)
        {

            this._game_data = new GameData();
            EdPlayers = new List<EdPlayer>();

            InitModes();

        }

        private void InitModes()
        {
            ScoreDisplay = new ScoreDisplay(this, 1);
            ScoreDisplay.layer.enabled = false;
            BaseMode = new BaseMode(this, 2);
            AttractMode = new Attract(this, 20);
            SkillshotMode = new Skillshot(this, 3);
            PagesMode = new PageMode(this, 5);
            CellarRampMode = new CellarRamp(this, 6);
            TargetsMode = new Targets(this, 25);
            WorkshedMode = new WorkShed(this, 30);
            ShedMultiball = new ShedMball(this, 35);
            BumpersMode = new Bumpers(this, 40);
            DeadByDawnMode = new DeadByDawn(this, 50);

            SelectMode = new ModeSelect(this, 40);
            Lindamode = new Linda(this, 40);
            CherylMode = new Cheryl(this, 40);
            BadHandMode = new BadHand(this, 40);
            EscapeMode = new Escape(this, 40);
            ShellyMode = new Shelly(this, 40);

            Modes.Add(ScoreDisplay);
            Modes.Add(BaseMode);
            Modes.Add(AttractMode);
            Modes.Add(TargetsMode);
            Modes.Add(WorkshedMode);            
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

        public void TargetsScoreHit()
        {
            bonus(2000);
            score(25000);
            _sound.PlaySound("target_hit");
        }
    }
}
