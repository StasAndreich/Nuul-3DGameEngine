using SharpDX;
using SharpDX.DirectInput;
using SharpDX.Windows;
using System;

namespace NuulEngine.Core.Utilities
{
    internal class InputProvider : IDisposable
    {
        private bool _isDisposed;

        private bool _isKeyboardAcquired;

        private bool _isMouseAcquired;

        private bool _isKeyboardUpdated;

        private bool _isMouseUpdated;

        // Don't know if the same instance needed
        // for keyboard and mouse.
        private DirectInput _directInput;

        private Keyboard _keyboard;

        private KeyboardState _keyboardState;

        private Mouse _mouse;

        private MouseState _mouseState;

        public InputProvider(RenderForm renderForm)
        {
            _directInput = new DirectInput();
            _keyboard = new Keyboard(_directInput);
            _mouse = new Mouse(_directInput);

            // Configure input devices.
            _keyboard.SetCooperativeLevel(renderForm.Handle,
                CooperativeLevel.Foreground | CooperativeLevel.NonExclusive);
            TryAcquireKeyboard();
            _keyboardState = new KeyboardState();

            _mouse.SetCooperativeLevel(renderForm.Handle,
                CooperativeLevel.Foreground | CooperativeLevel.Exclusive);           
            TryAcquireMouse();
            _mouseState = new MouseState();
        }

        private void TryAcquireKeyboard()
        {
            try
            {
                _keyboard.Acquire();
                _isKeyboardAcquired = true;
            }
            catch (SharpDXException e)
            {
                if (e.ResultCode.Failure)
                    _isKeyboardAcquired = false;
            }
        }

        private void TryAcquireMouse()
        {
            try
            {
                _mouse.Acquire();
                _isMouseAcquired = true;
            }
            catch (SharpDXException e)
            {
                if (e.ResultCode.Failure)
                    _isMouseAcquired = false;
            }
        }

        private void ProcessKeyboardState()
        {

        }

        private void ProcessMouseState()
        {
            for (int i = 0; i <= 7; ++i)
                _mouseButtons[i] = _mouseState.Buttons[i];
            _mouseRelativePositionX = _mouseState.X;
            _mouseRelativePositionY = _mouseState.Y;
            _mouseRelativePositionZ = _mouseState.Z;
        }

        public void UpdateKeyboardState()
        {
            if (!_isKeyboardAcquired)
            {
                TryAcquireKeyboard();
            }

            ResultDescriptor resultCode = ResultCode.Ok;
            try
            {
                _keyboard.GetCurrentState(ref _keyboardState);
                ProcessKeyboardState();
                _isKeyboardUpdated = true;
            }
            catch (SharpDXException e)
            {
                resultCode = e.Descriptor;
                _isKeyboardUpdated = false;
            }

            if (resultCode == ResultCode.InputLost || resultCode == ResultCode.NotAcquired)
            {
                _isKeyboardAcquired = false;
            }
        }

        public void UpdateMouseState()
        {
            if (!_isMouseAcquired)
            {
                TryAcquireMouse();
            }

            ResultDescriptor resultCode = ResultCode.Ok;
            try
            {
                _mouse.GetCurrentState(ref _mouseState);
                ProcessMouseState();
                _isMouseUpdated = true;
            }
            catch (SharpDXException e)
            {
                resultCode = e.Descriptor;
                _isMouseUpdated = false;
            }

            if (resultCode == ResultCode.InputLost || resultCode == ResultCode.NotAcquired)
            {
                _isMouseAcquired = false;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _mouse.Unacquire();
                    SharpDX.Utilities.Dispose(ref _mouse);
                    _keyboard.Unacquire();
                    SharpDX.Utilities.Dispose(ref _keyboard);
                    SharpDX.Utilities.Dispose(ref _directInput);
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
