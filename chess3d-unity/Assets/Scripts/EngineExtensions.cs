using System;
using ChessEngine.Engine;
using ChessServer.Common;
using ChessServer.Common.Types;
using Microsoft.FSharp.Core;

namespace Assets.Scripts
{
    public static class EngineExtensions
    {
        public static string CheckEndGame(this Engine engine)
        {
            var reason = ChessHelper.checkEndGame(engine);
            if (reason == FSharpOption<Tuple<Command.SessionResult, string>>.None)
            {
                return null;
            }
            else
            {
                return reason.Value.Item2;
            }
        }
    }
}
