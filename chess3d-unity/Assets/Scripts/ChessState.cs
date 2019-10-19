using Assets.Scripts.Models;
using ChessEngine.Engine;
using UnityEngine;

namespace Assets.Scripts
{
    public class ChessState
    {
        public ChessPieceColor PlayerColor = ChessPieceColor.White;
        public Engine Engine;
        public Board Board;
        public MonoBehaviour MonoBehaviour;
        public IChessManager ChessManager;
    }
}
