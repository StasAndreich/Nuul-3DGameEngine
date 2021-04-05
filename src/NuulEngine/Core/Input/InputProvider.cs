using SharpDX;
using SharpDX.DirectInput;
using SharpDX.Windows;
using System;
using System.Windows.Forms;

namespace NuulEngine.Core.Input
{
    public sealed class InputProvider : IDisposable
    {
        private bool _isDisposed;

        private RenderForm _renderForm;
        private DirectInput _directInput;

        private Keyboard _keyboard;
        private KeyboardState _keyboardState;
        private KeyboardState _lastKeyboardState;

        private Mouse _mouse;
        private MouseState _mouseState;
        private MouseState _lastMouseState;

        public InputProvider(RenderForm renderForm)
        {
            _renderForm = renderForm;
            _directInput = new DirectInput();

            _keyboard = new Keyboard(_directInput);
            _keyboard.SetCooperativeLevel(_renderForm.Handle,
                CooperativeLevel.Foreground | CooperativeLevel.NonExclusive);
            _keyboard.Acquire();

            _mouse = new Mouse(_directInput);
            _mouse.SetCooperativeLevel(_renderForm.Handle,
                CooperativeLevel.Foreground | CooperativeLevel.Exclusive);
            _mouse.Acquire();
        }

        public void Update()
        {
            _lastKeyboardState = _keyboardState;
            _keyboardState = _keyboard.GetCurrentState();

            _lastMouseState = _mouseState;
            _mouseState = _mouse.GetCurrentState();
        }

        public bool IsKeyPressed(Key key)
        {
            return _keyboardState != null && _keyboardState.IsPressed(key);
        }

        public bool IsKeyPressed(MouseKey mouseKey)
        {
            return _mouseState != null && _mouseState.Buttons[(int)mouseKey];
        }

        public int GetMouseWheelDelta()
        {
            return _mouseState != null ? _mouseState.Z : 0;
        }

        public Vector2 GetMousePosition()
        {
            var mousePosititon = _renderForm.PointToClient(Cursor.Position);
            var positionNormalized = new Vector2(
                x: mousePosititon.X / _renderForm.ClientSize.Width,
                y: mousePosititon.Y / _renderForm.ClientSize.Height);
            return positionNormalized;
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _mouse.Unacquire();
                    _keyboard.Unacquire();
                    Utilities.Dispose(ref _mouse);
                    Utilities.Dispose(ref _keyboard);
                    Utilities.Dispose(ref _directInput);
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
