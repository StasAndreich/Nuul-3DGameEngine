using System;
using System.Diagnostics;
using NuulEngine.Graphics;
using NuulEngine.Core.Utilities;
using SharpDX.Windows;

namespace NuulEngine.Core
{
    /// <summary>
    /// Core class that contains logic for running a game.
    /// </summary>
    public sealed class EngineCore : IDisposable
    {
        private const double FixedUpdateDeltaTime = 0.01;

        private readonly Stopwatch _stopwatch = new Stopwatch();

        private bool _isDisposed;

        private long _lastTickCount;

        private double _timeElapsed;

        private InputProvider _inputProvider;

        private prototypeRenderer _renderer;

        public EngineCore()
        {
            var renderForm = CreateConfiguredRenderForm((form) =>
                    form.Text = "App");
            _renderer = new prototypeRenderer(renderForm);
            _inputProvider = new InputProvider(renderForm);
        }

        /// <summary>
        /// Starts main game rendering loop.
        /// </summary>
        public void Run()
        {
            _renderer.RunRenderLoop(RenderLoopCallback);
        }

        private void RenderLoopCallback()
        {
            var currentTickCount = _stopwatch.ElapsedTicks;
            var deltaTime = (currentTickCount - _lastTickCount)
                / (double)TimeSpan.TicksPerSecond;

            _lastTickCount = currentTickCount;

            _timeElapsed += deltaTime;
            if (_timeElapsed >= FixedUpdateDeltaTime)
            {
                // Put there Fixed update code.
            }

            foreach (var item in )
            {
                // Put there Update code.
            }

            //_renderer.RenderScene();
        }

        internal RenderForm CreateConfiguredRenderForm(Action<RenderForm> configurateRenderForm)
        {
            var renderForm = new RenderForm();
            configurateRenderForm(renderForm);
            return renderForm;
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                _isDisposed = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~EngineCore()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
