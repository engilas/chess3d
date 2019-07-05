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

        public float SelectOffset = 0.5f;
        
        private readonly float _highlightIntensity = 60/255f;
        private Color _highlightColor;

        private bool _selected;

        private void Start()
        {
            _highlightColor = new Color(_highlightIntensity,_highlightIntensity,_highlightIntensity);
        }

        public void SetType(ChessPieceType type, ChessPieceColor color)
        {
            Type = type;
            Color = color;
        }

        public void SetPosition(Position p, bool taken = false)
        {
            Position = p;

            Deselect();
            transform.position = new Vector3(7 - p.Col, taken ? -0.5f : 0f, p.Row);

            OnPositionChange?.Invoke(p);
        }

        public void Select()
        {
            if (!_selected)
                transform.position += new Vector3(0, SelectOffset, 0);
            _selected = true;
        }

        public void Deselect()
        {
            if (_selected)
                transform.position -= new Vector3(0, SelectOffset, 0);
            _selected = false;
        }

        public event Action<Position?> OnPositionChange;

        private void OnMouseEnter()
        {
            gameObject.GetComponent<Renderer>().material.color += _highlightColor;
        }

        private void OnMouseExit()
        {
            gameObject.GetComponent<Renderer>().material.color -= _highlightColor;
        }
    }
}
