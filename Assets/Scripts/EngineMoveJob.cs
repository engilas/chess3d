using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Jobs;
using Unity.Collections;

namespace Assets.Scripts
{
    public struct EngineMoveJob : IJob
    {

        public void Execute()
        {
            ChessManager._engine.AiPonderMove();
        }
    }
}
