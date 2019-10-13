using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine.Engine;

public interface IChessStrategy
{
    void Init(ChessState state);
    void Move(MoveContent move);
    void StopGame();
    void OnDestroy();
    bool IsRestartAllowed();
    bool IsGameOverControl();
}
