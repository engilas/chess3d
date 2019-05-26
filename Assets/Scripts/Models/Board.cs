using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChessEngine.Engine;

namespace Assets.Scripts.Models
{
    public class Board
    {
        private Piece[] _board = new Piece[64];
        private int _blackTaken = 0;
        private int _whiteTaken = 0;

        public Board(IEnumerable<Piece> pieces)
        {
            foreach (var piece in pieces)
            {
                _board[piece.Position.Pos] = piece;
            }
        }

        public void Move(MoveContent move)
        {
            if (move.TakenPiece.PieceType != ChessPieceType.None)
            {
                Destroy(move.TakenPiece.Position);
            }
            Move(move.MovingPiecePrimary);
            if (move.MovingPieceSecondary.PieceType != ChessPieceType.None)
            {
                Move(move.MovingPieceSecondary);
            }
        }

        public void Move(PieceMoving pieceMoving)
        {
            Move(pieceMoving.SrcPosition, pieceMoving.DstPosition);
        }

        public void Move(byte src, byte dst)
        {
            var piece = GetPiece(src);
            _board[src] = null;
            _board[dst] = piece;
            piece.SetPosition(new Position(dst));
        }

        public void Destroy(byte src)
        {
            var piece = GetPiece(src);
            _board[src] = null;
            ref var count = ref piece.Color == ChessPieceColor.White ? ref _whiteTaken : ref _blackTaken;
            var col = count / 8;
            var row = count % 8;
            if (piece.Color == ChessPieceColor.Black)
                col = - 2 - col;
            else
                col = 9 + col;
            piece.SetPosition(new Position(col, row), true);
            ++count;
        }

        private Piece GetPiece(byte pos)
        {
            var piece = _board[pos];
            if (piece == null)
                throw new ArgumentException($"Piece {pos} not found");
            return piece;
        }
    }
}
