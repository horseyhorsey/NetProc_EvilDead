using NetProcgame.Game;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ED_Console.Modes
{
    public class SpellED : NetProcgame.Game.Mode
    {
        public Dictionary<string, bool> deadDict;

        Game _game;

        public SpellED(Game game, int priority) : base(game, priority)
        {
            _game = game;

            deadDict = new Dictionary<string, bool>();
            deadDict.Add("dead1", false);
            deadDict.Add("dead2", false);
            deadDict.Add("dead3", false);
            deadDict.Add("dead4", false);
        }

        private void TargetHitHelper(string targetName)
        {
            _game.bonus(1000);
            //# return if wizard
            //if self.game.wizardMode.is_started():
            //return

            if (!deadDict[targetName])
            {
                deadDict[targetName] = true;

                LaneCheck();

                //# play sounds
                _game._sound.PlaySound("kickNoise2");
            }
            else
            {
                _game._sound.PlaySound("laughs_short");
    
                //self.dmd_status()
            }

            if (_game.TroughMode.num_balls_in_play == 1)
                _game.LampCtrl.play_show("shellyStart", false);
        }

        private void LaneCheck()
        {
            
        }

        //# Flippers Lane Change
        public bool sw_flipperLwL_active(Switch sw)
        {
            //if self.game.wizardMode.is_started() or self.game.SkillShotMode.is_started():
            //return            

            switch (DeadLaneCount())
            {
                case 1:
                    LaneChange1Left();
                    update_lamps();
                    break;
                case 2:
                    //LaneChange2Right();
                    update_lamps();
                    break;
                case 3:
                    //LaneChange3Right();
                    update_lamps();
                    break;
                default:
                    break;
            }

            return SWITCH_CONTINUE;
        }
        public bool sw_flipperLwR_active(Switch sw)
        {
            //if self.game.wizardMode.is_started() or self.game.SkillShotMode.is_started():
            //return            

            switch (DeadLaneCount())
            {
                case 1:
                    LaneChange1Right();
                    update_lamps();
                    break;
                case 2:
                    LaneChange2Right();
                    update_lamps();
                    break;
                case 3:
                    LaneChange3Right();
                    update_lamps();
                    break;
                default:
                    break;
            }

            return SWITCH_CONTINUE;
        }

        public bool sw_leftOutLane_active(Switch sw)
        {
            TargetHitHelper("dead1");

            return SWITCH_CONTINUE;
        }
        public bool sw_leftReturn_active(Switch sw)
        {
            TargetHitHelper("dead2");
            return SWITCH_CONTINUE;
        }
        public bool sw_rightReturn_active(Switch sw)
        {
            TargetHitHelper("dead3");
            return SWITCH_CONTINUE;
        }
        public bool sw_righOutlane_active(Switch sw)
        {
            TargetHitHelper("dead4"); return SWITCH_CONTINUE;
        }
        ////# Evil Hitt
        public bool sw_Evil_active(Switch sw)
        {
            TargetHitHelper("evil1");
            return SWITCH_CONTINUE;
        }
        public bool sw_eVil_active(Switch sw)
        {
            TargetHitHelper("evil2"); return SWITCH_CONTINUE;
        }
        public bool sw_evIl_active(Switch sw)
        {

            TargetHitHelper("evil3"); return SWITCH_CONTINUE;
        }
        public bool sw_eviL_active(Switch sw)
        {
            TargetHitHelper("evil4");
            return SWITCH_CONTINUE;
        }

        public int DeadLaneCount()
        {
            return deadDict.Where(x => x.Value == true).Count();
        }

        public override void update_lamps()
        {
            foreach (var item in deadDict)
            {
                if (item.Value)
                    _game.Lamps[item.Key].Enable();
                else
                    _game.Lamps[item.Key].Disable();
            }

        }

        private void LaneChange1Right()
        {
            if (deadDict["dead1"])
            { deadDict["dead1"] = false; deadDict["dead2"] = true; }
            else if (deadDict["dead2"])
            { deadDict["dead2"] = false; deadDict["dead3"] = true; }
            else if (deadDict["dead3"])
            { deadDict["dead3"] = false; deadDict["dead4"] = true; }
            else if (deadDict["dead4"])
            { deadDict["dead4"] = false; deadDict["dead1"] = true; }
        }
        private void LaneChange2Right()
        {
            if (deadDict["dead1"] && deadDict["dead2"])
            { deadDict["dead2"] = true; deadDict["dead3"] = true; }
            else if (deadDict["dead1"] && deadDict["dead3"])
            { deadDict["dead2"] = true; deadDict["dead4"] = true; }
            else if (deadDict["dead1"] && deadDict["dead4"])
            { deadDict["dead2"] = true; deadDict["dead4"] = true; }
            else if (deadDict["dead2"] && deadDict["dead3"])
            { deadDict["dead3"] = true; deadDict["dead4"] = true; }
            else if (deadDict["dead2"] && deadDict["dead4"])
            { deadDict["dead1"] = true; deadDict["dead3"] = true; }
            else if (deadDict["dead3"] && deadDict["dead4"])
            { deadDict["dead1"] = true; deadDict["dead4"] = true; }
        }
        private void LaneChange3Right()
        {
            if (!deadDict["dead4"])
            {
                deadDict["dead1"] = false; deadDict["dead3"] = true;
                deadDict["dead2"] = true; deadDict["dead4"] = true;
            }
            else if (!deadDict["dead3"])
            {
                deadDict["dead1"] = true; deadDict["dead3"] = true;
                deadDict["dead2"] = true; deadDict["dead4"] = false;
            }
            else if (!deadDict["dead2"])
            {
                deadDict["dead1"] = true; deadDict["dead3"] = false;
                deadDict["dead2"] = true; deadDict["dead4"] = true;
            }
            else if (!deadDict["dead1"])
            {
                deadDict["dead1"] = true; deadDict["dead3"] = true;
                deadDict["dead2"] = false; deadDict["dead4"] = true;
            }

        }
        private void LaneChange1Left()
        {
            if (deadDict["dead1"])
            { deadDict["dead1"] = false; deadDict["dead4"] = true; }
            else if (deadDict["dead4"])
            { deadDict["dead4"] = false; deadDict["dead3"] = true; }
            else if (deadDict["dead3"])
            { deadDict["dead3"] = false; deadDict["dead2"] = true; }
            else if (deadDict["dead2"])
            { deadDict["dead2"] = false; deadDict["dead1"] = true; }
        }
        private void LaneChange2Left()
        {
            if (deadDict["dead1"] && deadDict["dead2"])
            { deadDict["dead2"] = false; deadDict["dead3"] = false; deadDict["dead4"] = true; }
            else if (deadDict["dead1"] && deadDict["dead3"])
            { deadDict["dead1"] = false; deadDict["dead3"] = false; deadDict["dead2"] = true; }
            else if (deadDict["dead1"] && deadDict["dead4"])
            { deadDict["dead1"] = false; deadDict["dead2"] = false; deadDict["dead3"] = true; }
            else if (deadDict["dead2"] && deadDict["dead3"])
            { deadDict["dead3"] = false; deadDict["dead4"] = false; deadDict["dead1"] = true; }
            else if (deadDict["dead2"] && deadDict["dead4"])
            {
                deadDict["dead2"] = false; deadDict["dead4"] = false; deadDict["dead3"] = true;
                deadDict["dead1"] = true;
            }
            else if (deadDict["dead3"] && deadDict["dead4"])
            { deadDict["dead4"] = false; deadDict["dead1"] = false; deadDict["dead2"] = true; }
        }
        private void LaneChange3Left()
        {
            if (!deadDict["dead4"])
            {
                deadDict["dead1"] = true; deadDict["dead3"] = false;
                deadDict["dead2"] = true; deadDict["dead4"] = true;
            }
            else if (!deadDict["dead3"])
            {
                deadDict["dead1"] = true; deadDict["dead3"] = true;
                deadDict["dead2"] = false; deadDict["dead4"] = true;
            }
            else if (!deadDict["dead2"])
            {
                deadDict["dead1"] = false; deadDict["dead3"] = true;
                deadDict["dead2"] = true; deadDict["dead4"] = true;
            }
            else if (!deadDict["dead1"])
            {
                deadDict["dead1"] = true; deadDict["dead3"] = true;
                deadDict["dead2"] = true; deadDict["dead4"] = false;
            }

        }
    }
}


