using System;
using System.Diagnostics;
using NuulEngine.Graphics;
using SharpDX.Windows;
using NuulEngine.Core.Utils;

namespace NuulEngine.Core
{
    /// <summary>
    /// Core class that contains logic for running a game.
    /// </summary>
    public class EngineCore : IDisposable
    {
        private const double FixedUpdateDeltaTime = 0.01;

        private readonly Stopwatch _stopwatch = new Stopwatch();

        private readonly InputProvider _inputProvider;

        private readonly GameObjectCollection _scene;

        private bool _isDisposed;

        private long _lastTickCount;

        public EngineCore(ISceneInitializer sceneInitializer)
        {
            sceneInitializer.Initialize(_scene);

            var renderForm = CreateConfiguredRenderForm((form) =>
                    form.Text = "App");
            Renderer = new prototypeRenderer(renderForm);

            Instance = this;
            
            //_inputProvider = new InputProvider(renderForm);
            
        }

        public static EngineCore Instance { get; private set; }

        internal prototypeRenderer Renderer { get; }

        /// <summary>
        /// Starts main game rendering loop.
        /// </summary>
        public void Run()
        {
            Renderer.RunRenderLoop(RenderLoopCallback);
        }

        private void RenderLoopCallback()
        {
            var currentTickCount = _stopwatch.ElapsedTicks;
            var deltaTime = (currentTickCount - _lastTickCount)
                / (double)TimeSpan.TicksPerSecond;
            _lastTickCount = currentTickCount;

            foreach (var gameObject in _scene)
            {
                if (gameObject.IsActive)
                {
                    gameObject.Update();
                }
            }

            Renderer.RenderScene((float)deltaTime);
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
                    // Add Input disposing.
                    Renderer.Dispose();
                }

                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
