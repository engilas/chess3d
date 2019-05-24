using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class Piece
    {
        public PieceType Type { get; private set; }
        public PieceColor Color { get; private set; }
        public Position? Position { get; private set; }

        public Piece(PieceType type, PieceColor color)
        {
            Type = type;
            Color = color;
        }

        public void SetPosition(Position? p)
        {
            Position = p;
            if (OnPositionChange != null) OnPositionChange(p);
        }

        public event Action<Position?> OnPositionChange;
    }
}
