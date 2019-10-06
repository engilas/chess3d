using System;
using ChessEngine.Engine;
using Microsoft.FSharp.Core;
using Newtonsoft.Json;
using Types;
using UnityEngine;

public class OnlineMpStrategy : IChessStrategy
{
    private ChessState _chessState;

    public void Init(ChessState state)
    {
        _chessState = state;

        _chessState.Engine.HumanPlayer = OnlineManager.PlayerColor;
        _chessState.PlayerColor = OnlineManager.PlayerColor;
        OnlineManager.OnOpponentMove += OnlineManagerOnOnOpponentMove;
        OnlineManager.OnEndGame += OnlineManagerOnEndGame;
        OnlineManager.OnSessionClosed += OnlineManagerOnSessionClosed;
        if (_chessState.PlayerColor == ChessPieceColor.Black)
        {
            PlayerLock.GameLock = true;
        }
    }

    private void OnlineManagerOnSessionClosed(Command.SessionCloseReason reason)
    {
        var reasonString = "Session closed";
        if (reason == Command.SessionCloseReason.OpponentDisconnected)
            reasonString = "Opponent disconnected";
        _chessState.ChessManager.StopGame(reasonString);
    }

    private void OnlineManagerOnEndGame(Command.EndGameNotify engGame)
    {
        Debug.Log("End game event from server");
        // пока завершать игру будет ChessMgr
    }

    private void OnlineManagerOnOnOpponentMove(Domain.MoveDescription move)
    {
        if (_chessState.Engine.WhoseMove == _chessState.PlayerColor)
        {
            Debug.LogError("Player move, but received event from server of opponent move");
            return;
        }

        if (_chessState.Engine.IsValidMove(move.Primary.Src, move.Primary.Dst)
                &&
            _chessState.Engine.MovePiece(move.Primary.Src, move.Primary.Dst))
        {
            var pawnPromotion = move.PawnPromotion == FSharpOption<Domain.PieceType>.None
                ? ChessPieceType.None
                : EngineMappers.toEngineType.Invoke(move.PawnPromotion.Value);

            var secondaryMove = new PieceMoving{PieceType = ChessPieceType.None};
            if (move.Secondary != FSharpOption<Domain.Move>.None)
            {
                var secondary = move.Secondary.Value;
                secondaryMove = new PieceMoving
                    {PieceType = ChessPieceType.Bishop, SrcPosition = secondary.Src, DstPosition = secondary.Dst};
            }

            var takenPiece = new PieceTaken {PieceType = ChessPieceType.None};
            if (move.TakenPiecePos != FSharpOption<byte>.None)
            {
                var taken = move.TakenPiecePos.Value;
                takenPiece = new PieceTaken {PieceType = ChessPieceType.Bishop, Position = taken};
            }

            var moveContent = new MoveContent
            {
                MovingPiecePrimary = new PieceMoving {SrcPosition = move.Primary.Src, DstPosition = move.Primary.Dst},
                PawnPromotedTo = pawnPromotion, MovingPieceSecondary = secondaryMove, TakenPiece = takenPiece
            };
            _chessState.Board.Move(moveContent);
            PlayerLock.GameLock = false;
        }
        else
        {
            Debug.LogError("Invalid move from server: " + JsonConvert.SerializeObject(move));
        }
    }

    //todo добавить на сервер ValidateMove, чтобы сначала вызвать его, потом клиентский Move, потом серверный Move

    public void Move(MoveContent move)
    {
        PlayerLock.GameLock = true;
        var primary = move.MovingPiecePrimary;
        OnlineManager.Move(primary.SrcPosition, primary.DstPosition, move.PawnPromotedTo);
    }

    public void StopGame()
    {
        throw new NotImplementedException();
    }

    public void OnDestroy()
    {
        OnlineManager.OnOpponentMove -= OnlineManagerOnOnOpponentMove;
        OnlineManager.StopConnection();
    }

    public bool IsRestartAllowed()
    {
        return false;
    }
}
