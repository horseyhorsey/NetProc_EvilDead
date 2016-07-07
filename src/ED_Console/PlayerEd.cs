using NetProcgame.Game;
using System.Collections.Generic;

namespace ED_Console
{
    public class EdPlayer : Player
    {
        public int SkillLevel { get; set; } = 1;
        public bool ScottyOpen { get; set; } = false;
        public int BumpersHit { get; set; }
        public int BumpersLevel { get; set; }
        public int DeadCount { get; set; }
        public Dictionary<int, bool> EvilDeadTargets { get; set; }
        public Dictionary<string, bool> CompletedMultiBalls { get; set; }        
        public Dictionary<string, bool []> BookModesDone { get; set; }
        public bool ModeActive { get; set; }
        public bool DbdComplete { get; set; }
        public Dictionary<int, bool> Multiballs { get; set; } 
        public int RampCount { get; set; }
        public int MultiballScoring { get; internal set; }
        public bool LockReady { get; internal set; }
        public bool[] WorkshedsLocked = new bool[3];
        public bool[] TargetBankLeft = new bool[3];
        public bool[] TargetBankRight = new bool[3];
        public bool SawReady { get; internal set; }
        public bool GunReady { get; internal set; }
        public int CellarRampCount { get; internal set; }
        public int CellarLocked { get; internal set; }
        public bool CellarMultiBallReady { get; internal set; }

        public EdPlayer(string name) : base(name)
        {
            AddBookModes();            

            Multiballs = new Dictionary<int, bool>();
            for (int i = 0; i < 3; i++)
            {
                Multiballs.Add(i, false);
            }

            CompletedMultiBalls = new Dictionary<string, bool>();
            CompletedMultiBalls.Add("s_multiball", false);
            CompletedMultiBalls.Add("w_multiball", false);
            CompletedMultiBalls.Add("c_multiball", false);
        }

        /// <summary>
        /// Array holds complete and fully completed
        /// </summary>
        private void AddBookModes()
        {
            BookModesDone = new Dictionary<string, bool[]>();
            BookModesDone.Add("linda",new bool[2] { false, false });
            BookModesDone.Add("shelly", new bool[2] { false, false });
            BookModesDone.Add("badhand", new bool[2] { false, false });
            BookModesDone.Add("escape", new bool[2] { false, false });
            BookModesDone.Add("cheryl", new bool[2] { false, false });
        }
    }
}
