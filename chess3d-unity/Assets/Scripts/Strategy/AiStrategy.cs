using Assets.Scripts;
using System.Collections;
using ChessEngine.Engine;

public class AiStrategy : IChessStrategy
{
    private ChessState _chessState;
    private EngineMoveJob _engineMoveJob;

    public void Init(ChessState state)
    {
        _chessState = state;

        _engineMoveJob = new EngineMoveJob(_chessState.Engine);
    }

    public void Move(MoveContent move)
    {
        if (_chessState.Engine.CheckEndGame() != null) return;

        PlayerLock.GameLock = true;
        _chessState.MonoBehaviour.StartCoroutine(EngineMove());
    }

    public void OnDestroy()
    {
        _engineMoveJob.Abort();
        _engineMoveJob.Dispose();
    }

    public bool IsRestartAllowed()
    {
        return true;
    }

    public void StopGame()
    {
        _engineMoveJob.Abort();
    }

    private IEnumerator EngineMove()
    {
        yield return null;
        _engineMoveJob.Start();

        while (!_engineMoveJob.IsCompleted)
            yield return null;

        if (!_engineMoveJob.Aborted)
        {
            _chessState.Board.Move(_chessState.Engine.LastMove);
        }

        PlayerLock.GameLock = false;
    }

    public bool IsGameOverControl()
    {
        return false;
    }
}
