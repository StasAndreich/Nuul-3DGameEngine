using System;
using System.Linq;
using System.Diagnostics;
using NuulEngine.Graphics;
using SharpDX.Windows;
using System.Collections.Generic;
using NuulEngine.Core.Utils;

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

        private string _activeSceneName;

        private long _lastTickCount;

        //private InputProvider _inputProvider;

        private readonly prototypeRenderer _renderer;

        //private readonly List<Scene> _scenes;

        private readonly GameObjectCollection _scene;

        public EngineCore(ISceneInitializer sceneInitializer)
        {
            sceneInitializer.Initialize(_scene);

            var renderForm = CreateConfiguredRenderForm((form) =>
                    form.Text = "App");
            _renderer = new prototypeRenderer(renderForm);

            Instance = this;
            
            //_inputProvider = new InputProvider(renderForm);
            
        }

        public static EngineCore Instance { get; private set; }

        internal prototypeRenderer Renderer { get; }

        //public Scene ActiveScene { get => _scene.Find(scene => scene.Name == _activeSceneName); }

        /// <summary>
        /// Starts main game rendering loop.
        /// </summary>
        public void Run()
        {
            _renderer.RunRenderLoop(RenderLoopCallback);
        }

        //public void SetSceneActive(string sceneName)
        //{
        //    if (!_scenes.Exists(scene => scene.Name == sceneName))
        //    {
        //        throw new ArgumentOutOfRangeException(nameof(sceneName));
        //    }

        //    _activeSceneName = sceneName;
        //}

        private void RenderLoopCallback()
        {
            var currentTickCount = _stopwatch.ElapsedTicks;
            var deltaTime = (currentTickCount - _lastTickCount)
                / (double)TimeSpan.TicksPerSecond;
            _lastTickCount = currentTickCount;

            foreach (var gameObject in )
            {
                // Put there Update code.
            }

            _renderer.RenderScene();
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
