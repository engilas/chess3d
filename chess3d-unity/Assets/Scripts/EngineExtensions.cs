using ChessEngine.Engine;
using UnityEngine;

namespace Assets.Scripts
{
    public static class EngineExtensions
    {
        public static string CheckEndGame(this Engine engine)
        {
            string reason = null;
            if (engine.StaleMate)
            {
                if (engine.InsufficientMaterial)
                {
                    reason = "Draw by insufficient material";
                }
                else if (engine.RepeatedMove)
                {
                    reason = "Draw by repetition";
                }
                else if (engine.FiftyMove)
                {
                    reason = "Draw by fifty move rule";
                }
                else
                {
                    reason = "Stalemate";
                }
            }
            else if (engine.GetWhiteMate())
            {
                reason = "Black mates";
            }
            else if (engine.GetBlackMate())
            {
                reason = "White mates";
            }

            if (engine.GetBlackCheck())
            {
                Debug.Log("Black check");
            }

            if (engine.GetWhiteCheck())
            {
                Debug.Log("White check");
            }

            return reason;
            //if (reason != null)
            //{
            //    //_chessState.GameOver = true;
            //    PlayerLock.GameLock = true;
            //    uiManager.ShowGameOver(reason);
            //    return true;
            //}
            //return false;
        }
    }
}
