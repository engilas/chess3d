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
    public class EngineMoveJob : IDisposable
    {
        private enum EngineMoveMode {UiThread, WorkerThread}

        private Engine _engine;
        private readonly EngineMoveMode _mode;

        private Semaphore _signal;
        private volatile bool _aiThinking;
        private Thread _aiThread;
        private JobHandle _jobHandle;

        private CancellationTokenSource _ctsMove;
        private CancellationTokenSource _ctsThread = new CancellationTokenSource();

        public bool IsCompleted => !_aiThinking;
        public bool Aborted { get; private set; }

        public EngineMoveJob(Engine engine)
        {
            _engine = engine;
            _mode = Application.platform == RuntimePlatform.WebGLPlayer ? EngineMoveMode.UiThread : EngineMoveMode.WorkerThread;

            if (_mode == EngineMoveMode.WorkerThread)
            {
                _signal = new Semaphore(0, 1);
                _aiThread = new Thread(WorkerThreadAction);
                _aiThread.Start();
            }
        }

        private void AiAction()
        {
            try
            {
                _engine.AiPonderMove(_ctsMove.Token);
            }
            catch (OperationCanceledException)
            {

            }
            finally
            {
                _aiThinking = false;
            }
        }

        private void WorkerThreadAction()
        {
            while (!_ctsThread.IsCancellationRequested && _signal.WaitOne())
            {
                if (_ctsThread.IsCancellationRequested)
                    break;

                AiAction();
            }
        }

        public void Start()
        {
            if (_aiThinking)
                throw new InvalidOperationException("Ai move already started");

            Aborted = false;
            _aiThinking = true;
            
            if (_mode == EngineMoveMode.WorkerThread)
            {
                _ctsMove = new CancellationTokenSource();
                _signal.Release();
            }

            if (_mode == EngineMoveMode.UiThread)
            {
                AiAction();
            }
        }

        public void Abort()
        {
            if (_mode == EngineMoveMode.UiThread) return;
            if (!_aiThinking) return;

            Aborted = true;
            _ctsMove.Cancel();
        }

        public void Dispose()
        {
            if (_mode == EngineMoveMode.UiThread) return;

            Abort();
            _ctsMove?.Dispose();
            _ctsThread.Cancel();
            _signal.Release();
            _signal.Dispose();
        }
    }
}
