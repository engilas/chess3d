using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChessEngine.Engine;
using Unity.Jobs;
using Unity.Collections;

namespace Assets.Scripts
{
    public class EngineMoveJob
    {
        private readonly Engine _engine;
        private Semaphore _signal = new Semaphore(0, 1);
        private volatile bool _aiThinking;
        private Thread _aiThread;

        public EngineMoveJob(Engine engine)
        {
            _engine = engine;
            _aiThread = new Thread(AiAction);
            _aiThread.Start();
        }

        private void AiAction()
        {
            while (_signal.WaitOne())
            {
                _engine.AiPonderMove();
                _aiThinking = false;
            }
        }

        public void Start()
        {
            _aiThinking = true;
            _signal.Release();
        }

        public bool IsCompleted => !_aiThinking;
    }
}
