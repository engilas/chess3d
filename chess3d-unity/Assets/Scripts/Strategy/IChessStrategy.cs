using ChessEngine.Engine;

namespace Assets.Scripts.Strategy
{
    public interface IChessStrategy
    {
        void Init(ChessState state);
        void Move(MoveContent move);
        void StopGame();
        void OnDestroy();
        bool IsRestartAllowed();
        bool IsGameOverControl();
    }
}
