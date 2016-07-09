using NetProcgame.Display.Layers;
using NetProcgame.Display.Transitions;
using NetProcgame.Services;
using NetProcgame.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ED_Console.Modes
{
    public class Bumpers : NetProcgame.Game.Mode
    {
        private int _bumperHits;
        private int _bumperHitsRound;
        private int _bumperLevel;
        private int _bumperSounds;
        private Game _game;
        Layer bubbaLayer;
        Layer TextBubbaLabel;
        Layer TextBumperLabel;
        Layer TextBumperLevelsLabel;
        Layer TextBumperCount;
        Layer TextLevels;
        AnimatedLayer BloodSplat;
        MoveLayer MoveBlood;  
        int[] _bumperAwardRange1;
        int[] _bumperAwardRange2to4;
        int[] _bumperAwardRange5;

        public Bumpers(Game game, int priority) : base(game, priority)
        {
            _game = game;
            _bumperHits = 0;
            _bumperSounds = 1;
            _bumperLevel = 1;
            _bumperAwardRange1 = Range.GetRange(10, 500, 10);
            _bumperAwardRange2to4 = Range.GetRange(25, 500, 25);
            _bumperAwardRange5 = Range.GetRange(100, 500, 25);

            bubbaLayer = AssetService.Animations["bubbaJoe"];
            BloodSplat = AssetService.Animations["BloodSplat"];
            BloodSplat.repeat = false;
            BloodSplat.hold = false;            

            MoveBlood = new MoveLayer(BloodSplat, targetY: 300,framesLength:BloodSplat.frames.Length);

            TextBubbaLabel = _game.BaseMode.SetStatus("bubba", "", 2, "spellED", composite: false);
            TextBumperLabel = _game.BaseMode.SetStatus("bumpers", "", 2, "ed_common", composite: false);
            TextBumperLevelsLabel = _game.BaseMode.SetStatus("level", "", 2, "ed_common", composite: false);
            
        }

        public override void mode_started()
        {
            var player = _game.GetCurrentPlayer();
            _bumperHits = player.BumpersHit;
            _bumperLevel = player.BumpersLevel;
            _bumperHitsRound = 0;        

            layer = new GroupedLayer(_game.Width, _game.Height, new List<Layer>()
            {
               MoveBlood
            });

            BloodSplat.enabled = false;
            MoveBlood.enabled = false;
        }

        #region Switches
        public bool sw_bumperL_active(NetProcgame.Game.Switch sw)
        {
            MoveAndEnableBlood(50, 50);
            BumperHandler();

            return SWITCH_STOP;
        }
        public bool sw_bumperM_active(NetProcgame.Game.Switch sw)
        {
            MoveAndEnableBlood(200, 50);
            BumperHandler();

            return SWITCH_STOP;
        }
        public bool sw_bumperR_active(NetProcgame.Game.Switch sw)
        {
            MoveAndEnableBlood(400, 50);
            BumperHandler();

            return SWITCH_STOP;
        }
        #endregion

        private void BumperHandler()
        {
            BumperScore();
            BumperAwards();
            ResetFrames();
        }

        private void BumperScore()
        {
            _bumperHits++;
            _bumperHitsRound++;
            _game.score(1337 * _bumperLevel);
            _game.bonus(137 * _bumperLevel);
        }

        private void MoveAndEnableBlood(int x, int y)
        {
            BloodSplat.set_target_position(x, y);
            MoveBlood._startX = x;            

        }

        private void ResetFrames()
        {
            BloodSplat.reset();
            BloodSplat.enabled = true;
            MoveBlood.reset();
            MoveBlood.enabled = true;
        }

        private void BumperAwards()
        {
            if (_game.Switches["bumperL"].TimeSinceChange() > 0.0)
                if (_game.Switches["bumperM"].TimeSinceChange() > 0.0)
                    if (_game.Switches["bumperR"].TimeSinceChange() > 0.0)
                    {
                        cancel_delayed("resetSound");
                        PlayBumperSound(_bumperSounds);
                    }

            _game.Coils["flasherHouse"].Schedule(0x0000000CC, 1, false);

            switch (_bumperLevel)
            {
                case 1:
                    BumperAwardDisplay(ref _bumperAwardRange1,_bumperHits);
                    break;
                case 2:
                case 3:
                case 4:
                    BumperAwardDisplay(ref _bumperAwardRange2to4, _bumperHits);
                    break;
                case 5:
                    BumperAwardDisplay(ref _bumperAwardRange5, _bumperHits);
                    break;
                default:
                    break;
            }


        }

        private void BumperAwardDisplay(ref int[] range, int hits)
        {
            if (range.Contains(hits))
             {
                _game._sound.PlaySound("BubbaJoe");
                layer = CreateDisplayLayers();
            }
        }

        private Layer CreateDisplayLayers()
        {
            TextBumperCount = _game.BaseMode.SetStatus(_bumperHits.ToString(), "", 2, "spellED", composite: false);
            TextLevels = _game.BaseMode.SetStatus(_bumperLevel.ToString(), "", 2, "spellED", composite: false);

            _bumperLevel++;

            //var group = new GroupedLayer(_game.Width,_game.Height,)

            return null;
        }

        private void PlayBumperSound(int soundNumber)
        {
            //if (WizardMode)

            cancel_delayed("resetSound");
            //for (int i = 0; i < 8; i++)
            //{
            //    _game._sound.StopSound("bump" + i);
            //}

            _game._sound.PlaySound("bump" + soundNumber);
            _bumperSounds++;
            if (soundNumber >= 8)
                _bumperSounds = 1;

            delay("resetSound", NetProcgame.NetPinproc.EventType.None, 3, 
                new NetProcgame.Game.AnonDelayedHandler(ResetBumperNumber));
        }

        private void ResetBumperNumber()
        {
            _bumperSounds = 1;
        }
    }
}
