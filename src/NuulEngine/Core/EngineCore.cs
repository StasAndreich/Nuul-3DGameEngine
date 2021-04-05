using System;
using System.Diagnostics;
using NuulEngine.Core.Input;
using NuulEngine.Graphics;
using SharpDX.Windows;

namespace NuulEngine.Core
{
    /// <summary>
    /// Core class that contains logic for running a game.
    /// </summary>
    public class EngineCore : IDisposable
    {
        private const double FixedUpdateDeltaTime = 0.01;

        private readonly Stopwatch _stopwatch = new Stopwatch();

        private bool _isDisposed;

        private long _lastTickCount;

        public EngineCore(ISceneInitializer sceneInitializer)
        {
            sceneInitializer.Initialize(Scene);

            var renderForm = CreateConfiguredRenderForm((form) =>
                    form.Text = "App");
            Renderer = new prototypeRenderer(renderForm);

            Instance = this;
            
            InputProvider = new InputProvider(renderForm);
        }

        public static EngineCore Instance { get; private set; }

        internal GameObjectCollection Scene { get; }

        internal prototypeRenderer Renderer { get; }

        public InputProvider InputProvider { get; }

        /// <summary>
        /// Starts main game rendering loop.
        /// </summary>
        public void Run()
        {
            Renderer.RunRenderLoop(RenderLoopCallback);
        }

        private void RenderLoopCallback()
        {
            // Remove all the game objects that been deleted.
            Scene.ConfirmRemove();

            var currentTickCount = _stopwatch.ElapsedTicks;
            var deltaTime = (currentTickCount - _lastTickCount)
                / (double)TimeSpan.TicksPerSecond;
            _lastTickCount = currentTickCount;

            // Update user's input.
            InputProvider.Update();

            foreach (var gameObject in Scene)
            {
                if (gameObject.IsActive)
                {
                    gameObject.Update((float)deltaTime);
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
