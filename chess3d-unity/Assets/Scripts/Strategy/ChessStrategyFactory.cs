using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            default:
                throw new NotImplementedException();
        }
    }
}