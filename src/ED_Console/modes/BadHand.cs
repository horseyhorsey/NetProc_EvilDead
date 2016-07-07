namespace ED_Console.Modes
{
    public class BadHand : NetProcgame.Game.Mode
    {
        private NetProcgame.Game.Driver[] _handCoils;
        private byte _count = 0;
        Game _game;

        public BadHand(Game game, int priority) : base(game, priority)
        {
            _game = game;

            _handCoils = new NetProcgame.Game.Driver[4];
            for (int i = 0; i < 4; i++)
            {
                _handCoils[i] = game.Coils["handDrop" + (i + 1)];
            }
        }

        public override void mode_started()
        {
            _game.GetCurrentPlayer().ModeActive = true;
            _game.LampCtrl.stop_show();
            ModeStart();
        }

        private void ModeStart()
        {
            
        }
    }
}
