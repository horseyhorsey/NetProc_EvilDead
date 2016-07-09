using System;
using NetProcgame.Display.Layers;
using NetProcgame.Services;
using System.Collections.Generic;
using System.Linq;

namespace ED_Console.Modes
{
    public class ModeSelect : NetProcgame.Game.Mode
    {
        private Game _game;
        private List<string> _modeList;
        private int _selectedIndex;
        private VideoLayer _tapeRecorder;
        private bool _userSelect;
        private string _selectedMode;
        private SdlTextLayer _modeNameText;
        private SdlTextLayer _selectInfoText;

        public ModeSelect(Game game, int priority) : base(game, priority)
        {            
            _userSelect = true;
            _selectedIndex = 0;
            _selectedMode = "";
            _game = game;            
            _tapeRecorder = AssetService.Videos["TapeRecorder"];
        }

        public override void mode_started()
        {
            _game.BaseMode.RemoveModeInfoLayer();
            //_game.Modes.Remove(_game.TimerMode);
            _game.PagesMode.PausePages(true);
            //if self.game.trophyMode.checkTrophyComplete('Start A Mode'):
            //self.game.trophyMode.layer = self.game.trophyMode.buildLayerScript('Start A Mode')
            //self.game.trophyMode.updateTrophy('Start A Mode')
            _game._sound.StopSound("TheOthers");
            //_game.Modes.Remove(_game.ModeEnd);
            
            _game.DisableAllLamps();
            _game.LampCtrl.play_show("Round16", true);
            _game.Coils["topDropTarget"].Enable();
            _game._sound.StopMusic();
            _game.CellarHatch(false);

            AdjustModeList();

            ModePicker();

        }

        private void AdjustModeList()
        {
            var bookModes = _game.GetCurrentPlayer().BookModesAttempted;

            _modeList = new List<string>();

            foreach (var mode in bookModes.Where(x => x.Value == false))
            {
                _modeList.Add(mode.Key);
            }
        }

        public override void mode_stopped()
        {
            layer = null;
            _game.Logger.Log("ModeSelect stopped");
        }

        private void ModePicker()
        {
            _selectedMode = RandomModeName();

            _modeNameText = new SdlTextLayer(_game.Width / 2, 150, "ed_LeftRamp1", FontJustify.Center, FontJustify.Center,
                "redYelThin");
            _selectInfoText = new SdlTextLayer(_game.Width, _game.Height, "default_msg", FontJustify.Right, FontJustify.Bottom,
                "yelRedThin");

            _modeNameText.SetText(_selectedMode);

            if (_userSelect)
            {
                _selectInfoText.SetText(_game.GetCurrentPlayer().name + " Select mode with flippers", blink_frames: 2);
                _selectedIndex = 0;
            }
            else            
                _selectInfoText.SetText(" ");            

            var layers = new List<Layer>() { _tapeRecorder, _modeNameText, _selectInfoText };
            var group = new GroupedLayer(_game.Width, _game.Height, layers);

            _game._sound.PlaySound("Passage");

            layer = group;

            delay("modeStart",
                NetProcgame.NetPinproc.EventType.None, 2.5, 
                new NetProcgame.Game.AnonDelayedHandler(StartSelectedMode));

            if (!_userSelect)
                delay("choice",
                    NetProcgame.NetPinproc.EventType.None, 0.25,
                    new NetProcgame.Game.AnonDelayedHandler(StartSelectedMode));
        }

        private string RandomModeName()
        {
            var random = new Random();
            
            return _modeList[random.Next(0, 5)];
        }

        private void RandomChoiceUpdate()
        {

        }

        private void RandomPassage()
        {

        }

        public void StartSelectedMode()
        {
            _game.GetCurrentPlayer().ModeActive = true;
            layer = null;
            cancel_delayed("choice");
            cancel_delayed("modeStart");
            _game.LampCtrl.stop_show();

            switch (_selectedMode)
            {
                case "linda":
                    _game.Modes.Add(_game.Lindamode);
                    break;
                case "shelly":
                    _game.Modes.Add(_game.CherylMode);
                    break;
                case "escape":
                    _game.Modes.Add(_game.EscapeMode);
                    break;
                case "badhand":
                    _game.Modes.Add(_game.BadHandMode);
                    break;
                case "cheryl":
                    _game.Modes.Add(_game.CherylMode);
                    break;
                default:
                    break;
            }

            _game.update_lamps();
            _game.Modes.Remove(this);            
        }

        public bool sw_flipperLwL_active(NetProcgame.Game.Switch sw)
        {
            if (_userSelect)
            {
                if (_selectedIndex == 0)
                    _selectedIndex = _modeList.Count - 1;
                else
                    _selectedIndex--;
                        
                _selectedMode = _modeList[_selectedIndex];

                _modeNameText.SetText(_selectedMode);
            }

            return SWITCH_STOP;
        }
        public bool sw_flipperLwR_active(NetProcgame.Game.Switch sw)
        {
            if (_userSelect)
            {
                if (_selectedIndex == _modeList.Count - 1)                
                    _selectedIndex = 0;                    
                else                
                    _selectedIndex++;                

                _selectedMode = _modeList[_selectedIndex];

                _modeNameText.SetText(_selectedMode);
            }

            return SWITCH_STOP;
        }
    }
}
