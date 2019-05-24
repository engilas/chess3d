using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Models
{
    public class Board
    {
        private Piece[,] _board;
        private int _destroyXOffset = 0;

        public Board()
        {
            _board = new Piece[8, 8];
        }

        public void Move(Piece p, Position pos)
        {
            var old = p.Position;
            if (_board[old.Col, old.Row] != p)
                throw new ArgumentException("Position mismatch");
            if (_board[pos.Col, pos.Row] != null)
                throw new ArgumentException(string.Format("Position {0} occupied", pos.ToString()));
            _board[old.Col, old.Row] = null;
            _board[pos.Col, pos.Row] = p;
            p.SetPosition(pos);
        }

        public void Move(Position src, Position dst)
        {
            //Move();
            var p = _board[src.Col, src.Row];
            if (p == null)
            {
                throw new ArgumentException($"Src pos {src} is empty");
            }
            Move(p, dst);
        }

        public void Create(Piece p, Position pos)
        {
            if (_board[pos.Col, pos.Row] != null)
                throw new ArgumentException(string.Format("Position {0} occupied", pos.ToString()));
            _board[pos.Col, pos.Row] = p;
            p.SetPosition(pos);
        }

        public void Destroy(Position pos)
        {
            var p = _board[pos.Col, pos.Row];
            if (p == null)
            {
                throw new ArgumentException($"Src pos {pos} is empty");
            }
            Destroy(p);
        }

        public void Destroy(Piece p)
        {
            var old = p.Position;
            if (_board[old.Col, old.Row] != p)
                throw new ArgumentException("Position mismatch");
            _board[old.Col, old.Row] = null;
            p.SetPosition(new Position(_destroyXOffset++, -1));
        }
    }
}
