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
        private int _destroyXOffset = 7;

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
            piece.SetPosition(new Position(_destroyXOffset++, 8));
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
