using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts
{
    public class PieceComponent : MonoBehaviour
    {
        public Piece Piece { get; private set; }

        public void SetPiece(Piece p)
        {
            Piece = p;
            transform.position = new Vector3(p.Position.Value.Col, 0, p.Position.Value.Row);
            Piece.OnPositionChange += pos =>
            {
                if (!pos.HasValue) transform.position = new Vector3(-1, 0, -1);
                else transform.position = new Vector3(pos.Value.Col, 0, pos.Value.Row);
            };
        }
    }
}
