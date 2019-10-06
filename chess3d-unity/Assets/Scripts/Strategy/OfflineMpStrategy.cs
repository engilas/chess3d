using Assets.Scripts;
using UnityEngine;
using ChessEngine.Engine;

public class OfflineMpStrategy : IChessStrategy
{
    private ChessState _chessState;
    private CameraManager _cameraManager;
     
    public void Init(ChessState state)
    {
        _cameraManager = Object.FindObjectOfType<CameraManager>();
        _chessState = state;
    }

    public void Move(MoveContent move)
    {
        if (_chessState.Engine.CheckEndGame() != null) return;

        //swap active color
        if (_chessState.PlayerColor == ChessPieceColor.Black)
            _chessState.PlayerColor = ChessPieceColor.White;
        else if (_chessState.PlayerColor == ChessPieceColor.White)
            _chessState.PlayerColor = ChessPieceColor.Black;

        _cameraManager.MoveOtherSide();
    }

    public void OnDestroy()
    {
    }

    public bool IsRestartAllowed()
    {
        return true;
    }

    public void StopGame()
    {
        if (_chessState.PlayerColor == ChessPieceColor.Black)
        {
            _cameraManager.MoveOtherSide();
            _chessState.PlayerColor = ChessPieceColor.White;
        }
    }
}
