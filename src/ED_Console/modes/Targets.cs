using System;
using NetProcgame.Display.Layers;
using NetProcgame.Services;

namespace ED_Console.Modes
{
    public class Targets : NetProcgame.Game.Mode
    {
        bool _gunReady, _sawReady, _lockReady;
        VideoLayer zoomSaw, loadGun, gunNSaw;
        AnimatedLayer shedPic, shedLock;
        SdlTextLayer _text1, _text2;
        Game _game;
        NetProcgame.Game.AnonDelayedHandler ClearDmdHandler;

        public Targets(Game game, int priority) : base(game, priority)
        {
            _game = game;

            ClearDmdHandler = new NetProcgame.Game.AnonDelayedHandler(ClearDmd);

            InitDisplayLayers();
        }

        private void ClearDmd()
        {
            layer = null;
        }

        public override void mode_started()
        {

        }

        private void InitDisplayLayers()
        {
            zoomSaw = AssetService.Videos["zoom_saw"];
            zoomSaw.hold = true;
            loadGun = AssetService.Videos["load_gun"];
            loadGun.hold = true;
            gunNSaw = AssetService.Videos["gunNsaw"];
            gunNSaw.hold = true;

            shedPic = AssetService.Animations["light_texture"];
            shedLock = AssetService.Animations["shedLock"];

            _text1 = new SdlTextLayer(_game.Width/2, 10, "ed_targets", FontJustify.Center, FontJustify.Top);
            _text2 = new SdlTextLayer(0, _game.Height / 2, "ed_targets", FontJustify.Center, FontJustify.Bottom);
        }              

        public void CheckMultiballReady()
        {
            var player = _game.GetCurrentPlayer();

            if (player.GunReady && player.SawReady)
            {
                cancel_delayed("clearDmd");
                _game.GetCurrentPlayer().LockReady = true;

                var text = _game.DisplayHelper.GenerateMultiTextLayer("shed open|lock the ball", "ed_common","redYellow");

                var group = new GroupedLayer(_game.Width, _game.Height, new System.Collections.Generic.List<Layer>()
                            {
                                gunNSaw, text
                            });

                layer = group;
                loadGun.reset();

                _game.Coils["rightFlash"].Pulse(125);
                
            }

            delay("clearDMD", NetProcgame.NetPinproc.EventType.None, 2, ClearDmdHandler);
        }

        private Layer GenerateShedLayer(string letter)
        {
            _text1.SetText(letter,2);
            _text2.SetText("Complete Targets",2);

            var group1 = new GroupedLayer(_game.Width, _game.Height, new System.Collections.Generic.List<Layer>()
            {
                shedPic, _text1
            });

            var group2 = new GroupedLayer(_game.Width, _game.Height, new System.Collections.Generic.List<Layer>()
            {
                shedLock, _text2
            });

            var trans = new TransitionLayer(group1, group2);
            
            return trans;
        }

        public bool sw_Target1_active(NetProcgame.Game.Switch sw)
        {
            SawTargetCheck(0);

            return SWITCH_CONTINUE;
        }
        public bool sw_Target2_active(NetProcgame.Game.Switch sw)
        {
            SawTargetCheck(1);

            return SWITCH_CONTINUE;
        }
        public bool sw_Target3_active(NetProcgame.Game.Switch sw)
        {
            SawTargetCheck(2);

            return SWITCH_CONTINUE;
        }
        public bool sw_Target4_active(NetProcgame.Game.Switch sw)
        {
            GunTargetBankCheck(0);

            return SWITCH_CONTINUE;
        }
        public bool sw_Target5_active(NetProcgame.Game.Switch sw)
        {
            GunTargetBankCheck(1);

            return SWITCH_CONTINUE;
        }
        public bool sw_Target6_active(NetProcgame.Game.Switch sw)
        {
            GunTargetBankCheck(2);

            return SWITCH_CONTINUE;
        }

        private void SawTargetCheck(int targetNum)
        {
            //if self.game.wizardMode.is_started():
            //return
            _game._sound.PlaySound("TargetHit");
            _sawReady = _game.GetCurrentPlayer().SawReady;
            var targets = _game.GetCurrentPlayer().TargetBankLeft;

            if (!_sawReady)
            {
                switch (targetNum)
                {
                    case 0:
                        if (!targets[targetNum])
                        {
                            cancel_delayed("clearDMD");
                            targets[targetNum] = true;
                            layer = GenerateShedLayer("S");                           
                            update_lamps();
                        }
                        break;
                    case 1:
                        if (targets[0] && targets[2])
                        {
                            cancel_delayed("clearDMD");
                            targets[targetNum] = true;
                            _sawReady = true;
                            _game.GetCurrentPlayer().SawReady = true;
                            _game._sound.PlaySound("sawComplete");
                            var text = _game.DisplayHelper.GenerateMultiTextLayer("saw|targets completed", "ed_common", "redYellow");
                            var group = new GroupedLayer(_game.Width, _game.Height, new System.Collections.Generic.List<Layer>()
                            {
                                zoomSaw, text
                            });

                            layer = group;
                            zoomSaw.reset();
                            CheckMultiballReady();
                            update_lamps();

                        }
                        break;
                    case 2:
                        if (!targets[targetNum])
                        {
                            cancel_delayed("clearDMD");
                            targets[targetNum] = true;
                            layer = GenerateShedLayer("W");
                            update_lamps();
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                cancel_delayed("clearDMD");
                layer = _game.DisplayHelper.GenerateMultiTextLayer(
                    "Saw Completed|Completing gun and saw|unlocks shed","ed_common","redYellow",bgLayer:zoomSaw);

                zoomSaw.reset();
            }

            delay("clearDMD", NetProcgame.NetPinproc.EventType.None, 3, ClearDmdHandler);

        }
        private void GunTargetBankCheck(int targetNum)
        {
            _game._sound.PlaySound("TargetHit");
            _gunReady = _game.GetCurrentPlayer().GunReady;
            var targets = _game.GetCurrentPlayer().TargetBankRight;

            if (!_gunReady)
            {
                switch (targetNum)
                {
                    case 0:
                        if (!targets[targetNum])
                        {
                            cancel_delayed("clearDMD");
                            targets[targetNum] = true;
                            layer = GenerateShedLayer("G");
                            update_lamps();
                        }
                        break;
                    case 1:
                        if (targets[0] && targets[2])
                        {
                            cancel_delayed("clearDMD");
                            targets[targetNum] = true;
                            _gunReady = true;
                            _game.GetCurrentPlayer().GunReady = true;
                            _game._sound.PlaySound("ChungChing");
                            var text = _game.DisplayHelper.GenerateMultiTextLayer("gun|targets completed", "ed_common", "redYellow");
                            var group = new GroupedLayer(_game.Width, _game.Height, new System.Collections.Generic.List<Layer>()
                            {
                                loadGun, text
                            });

                            layer = group;
                            loadGun.reset();
                            CheckMultiballReady();
                            update_lamps();

                        }
                        break;
                    case 2:
                        if (!targets[targetNum])
                        {
                            cancel_delayed("clearDMD");
                            targets[targetNum] = true;
                            layer = GenerateShedLayer("N");
                            update_lamps();
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                cancel_delayed("clearDMD");
                loadGun.hold = true;
                layer = _game.DisplayHelper.GenerateMultiTextLayer(
                    "Gun Completed|Completing gun and saw|unlocks shed", "ed_common", "redYellow", bgLayer: loadGun);

                loadGun.reset();
            }

            delay("clearDMD", NetProcgame.NetPinproc.EventType.None, 3, ClearDmdHandler);

        }

        public override void update_lamps()
        {
            SawLamps();

            GunLamps();

            var shedLocks = _game.GetCurrentPlayer().WorkshedsLocked;
            if (shedLocks[0])
                _game.Lamps["Shed1Lock"].Enable();
            if (shedLocks[1])
                _game.Lamps["Shed2Lock"].Enable();

        }
        private void SawLamps()
        {
            var lTargets = _game.GetCurrentPlayer().TargetBankLeft;

            for (int i = 0; i < lTargets.Length; i++)
            {
                if (lTargets[i])
                    _game.Lamps["T" + (i + 1) + "_Lamp"].Enable();
                else
                {
                    if (lTargets[1])
                    {
                        if (lTargets[0] && lTargets[2])
                            _game.Lamps["T" + (i + 1) + "_Lamp"].Schedule(0x0F0F0F0F);
                    }
                    else
                        _game.Lamps["T" + (i + 1) + "_Lamp"].Schedule(0x0F0F0F0F);
                }
            }
        }
        private void GunLamps()
        {
            var rTargets = _game.GetCurrentPlayer().TargetBankRight;

            for (int i = 0; i < rTargets.Length; i++)
            {
                if (rTargets[i])
                    _game.Lamps["T" + (i + 4) + "_Lamp"].Enable();
                else
                {
                    if (rTargets[1])
                    {
                        if (rTargets[0] && rTargets[2])
                            _game.Lamps["T" + (i + 4) + "_Lamp"].Schedule(0x0F0F0F0F);
                    }
                    else
                        _game.Lamps["T" + (i + 4) + "_Lamp"].Schedule(0x0F0F0F0F);
                }
            }
        }
    }
}


