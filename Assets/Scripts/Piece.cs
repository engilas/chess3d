using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Models;
using ChessEngine.Engine;
using UnityEngine;

namespace Assets.Scripts
{
    public class Piece : MonoBehaviour
    {
        public ChessPieceType Type { get; private set; }
        public ChessPieceColor Color { get; private set; }
        public Position Position { get; private set; }

        public void SetType(ChessPieceType type, ChessPieceColor color)
        {
            Type = type;
            Color = color;
        }

        public void SetPosition(Position p, bool taken = false)
        {
            Position = p;

            transform.position = new Vector3(p.Col, taken ? -0.5f : 0f, p.Row);

            OnPositionChange?.Invoke(p);
        }

        public event Action<Position?> OnPositionChange;
    }
}
