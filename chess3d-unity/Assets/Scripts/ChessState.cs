using Assets.Scripts.Models;
using ChessEngine.Engine;
using UnityEngine;

public class ChessState
{
    public ChessPieceColor PlayerColor = ChessPieceColor.White;
    public Engine Engine;
    public Board Board;
    public MonoBehaviour MonoBehaviour;
}
