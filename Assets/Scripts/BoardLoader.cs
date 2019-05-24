using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Models;

public class BoardLoader {

    public static Piece[] FillBoard(Board board)
    {
        var result = new List<Piece>();
        foreach (var color in new [] {PieceColor.White, PieceColor.Black})
        {
            var row = color == PieceColor.Black ? 6 : 1;
            Piece p;
            for (int i = 0; i < 8; i++)
            {
                p = new Piece(PieceType.Pawn, color);
                board.Create(p, new Position(i, row));
                result.Add(p);
            }
            row = color == PieceColor.Black ? 7 : 0;

            p = new Piece(PieceType.Rook, color);
            board.Create(p, new Position(0, row));
            result.Add(p);

            p = new Piece(PieceType.Rook, color);
            board.Create(p, new Position(7, row));
            result.Add(p);

            p = new Piece(PieceType.Knight, color);
            board.Create(p, new Position(1, row));
            result.Add(p);

            p = new Piece(PieceType.Knight, color);
            board.Create(p, new Position(6, row));
            result.Add(p);

            p = new Piece(PieceType.Bishop, color);
            board.Create(p, new Position(2, row));
            result.Add(p);

            p = new Piece(PieceType.Bishop, color);
            board.Create(p, new Position(5, row));
            result.Add(p);

            p = new Piece(PieceType.Queen, color);
            board.Create(p, new Position(3, row));
            result.Add(p);

            p = new Piece(PieceType.King, color);
            board.Create(p, new Position(4, row));
            result.Add(p);
        }

        return result.ToArray();
    }
}
