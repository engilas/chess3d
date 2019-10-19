using System;

namespace Assets.Scripts.Strategy
{
    public static class ChessStrategyFactory
    {
        public static IChessStrategy ResolveStrategy()
        {
            switch (Settings.GameMode)
            {
                case GameMode.OfflineAi:
                    return new AiStrategy();
                case GameMode.OfflineMp:
                    return new OfflineMpStrategy();
                case GameMode.OnlineQuick:
                    return new OnlineMpStrategy();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}