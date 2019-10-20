using Assets.Scripts.Models;
using ChessEngine.Engine;
using UnityEngine;

namespace Assets.Scripts
{
    public class ChessState
    {
        public ChessPieceColor PlayerColor = ChessPieceColor.White;
        public Engine Engine;
        public BoardManager Board;
        public MonoBehaviour MonoBehaviour;
        public IChessManager ChessManager;
        public void TryStopByEndGame()
        {
            var endGameReason = Engine.CheckEndGame();
            if (endGameReason != null)
            {
                ChessManager.StopGame(endGameReason);
            }
        }
    }
}
