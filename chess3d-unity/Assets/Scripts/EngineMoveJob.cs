using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChessEngine.Engine;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class EngineMoveJob
    {
        public struct EngineUnityJob : IJob
        {
            public void Execute()
            {
                _engine.AiPonderMove();
            }
        }

        public enum EngineMoveMode {Job, Thread}

        private static Engine _engine;
        private readonly EngineMoveMode _mode;

        private Semaphore _signal;
        private volatile bool _aiThinking;
        private Thread _aiThread;
        private JobHandle _jobHandle;

        public EngineMoveJob(Engine engine)
        {
            _engine = engine;
            _mode = Application.platform == RuntimePlatform.WebGLPlayer ? EngineMoveMode.Job : EngineMoveMode.Thread;

            if (_mode == EngineMoveMode.Thread)
            {
                _signal = new Semaphore(0, 1);
                _aiThread = new Thread(AiAction);
                _aiThread.Start();
            }
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
            if (_mode == EngineMoveMode.Thread)
            {
                _aiThinking = true;
                _signal.Release();
            }

            if (_mode == EngineMoveMode.Job)
            {
                var job = new EngineUnityJob();
                _jobHandle = job.Schedule();
            }
        }

        public bool IsCompleted => _mode == EngineMoveMode.Thread ? !_aiThinking : _jobHandle.IsCompleted;
    }
}
