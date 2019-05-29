using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Models;
using ChessEngine.Engine;
using UnityEditor;
using UnityEngine;

public class BoardLoader {

    private static Piece Create(ChessPieceType type, ChessPieceColor color)
    {
        var path = $"Prefabs/{type.ToString()}{(color == ChessPieceColor.Black ? "Dark" : "Light")}";
        var prefab = Resources.Load(path);
        var obj = Object.Instantiate(prefab) as GameObject;
        var piece = obj.GetComponent<Piece>();
        piece.SetType(type, color);
        return piece;
    }

    public static Piece[] FillBoard(Engine engine)
    {
        var result = new List<Piece>();

        for (byte col = 0; col < 8; col++)
        for (byte row = 0; row < 8; row++)
        {
            var type = engine.GetPieceTypeAt(col, row);
            var color = engine.GetPieceColorAt(col, row);
            
            if (type == ChessPieceType.None) continue;

            var p = Create(type, color);
            p.SetPosition(new Position(col, row));
            result.Add(p);
        }   

        return result.ToArray();
    }
}
