using System;
using SharpDX;
using SharpDX.DirectInput;
using SharpDX.Windows;

namespace NuulEngine.Core.Input
{
    public abstract class Input : IDisposable
    {
        private bool _isDisposed;

        private bool _isKeyboardAcquired;

        private bool _isMouseAcquired;

        private DirectInput _directInput;

        private Keyboard _keyboard;

        private KeyboardState _keyboardState;

        private Mouse _mouse;

        private MouseState _mouseState;

        public Input(RenderForm renderForm)
        {
            _directInput = new DirectInput();
            _keyboard = new Keyboard(_directInput);
            _keyboard.SetCooperativeLevel(renderForm.Handle,
                CooperativeLevel.Foreground | CooperativeLevel.NonExclusive);      // __ NonExclusive. With excluzive Alt-F4 don't work.
            AcquireKeyboard();
            _keyboardState = new KeyboardState();

            _mouse = new Mouse(_directInput);
            _mouse.SetCooperativeLevel(renderForm.Handle,
                CooperativeLevel.Foreground | CooperativeLevel.Exclusive);      // __ NonExclusive (after add FXX frizes). Cursor hide with Excluzive.
            AcquireMouse();
            _mouseState = new MouseState();
        }

        public bool[] MouseButtons { get => _mouseState.Buttons; }

        public int MouseRelativePositionX { get => _mouseState.X; }

        public int MouseRelativePositionY { get => _mouseState.Y; }

        public int MouseRelativePositionZ { get => _mouseState.Z; }

        protected KeyboardState KeyboardState { get => _keyboardState; }

        public void UpdateKeyboardState()
        {
            if (!_isKeyboardAcquired)
            {
                AcquireKeyboard();
            }

            ResultDescriptor resultCode = ResultCode.Ok;
            try
            {
                _keyboard.GetCurrentState(ref _keyboardState);
            }
            catch (SharpDXException e)
            {
                resultCode = e.Descriptor;
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
                AcquireMouse();
            }

            ResultDescriptor resultCode = ResultCode.Ok;
            try
            {
                _mouse.GetCurrentState(ref _mouseState);
            }
            catch (SharpDXException e)
            {
                resultCode = e.Descriptor;
            }

            if (resultCode == ResultCode.InputLost || resultCode == ResultCode.NotAcquired)
            {
                _isMouseAcquired = false;
            }
        }

        private void AcquireKeyboard()
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

        private void AcquireMouse()
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

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _mouse.Unacquire();
                    Utilities.Dispose(ref _mouse);
                    _keyboard.Unacquire();
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
