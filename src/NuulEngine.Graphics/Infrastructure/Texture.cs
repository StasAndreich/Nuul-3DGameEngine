using System;
using SharpDX;
using SharpDX.Direct3D11;

namespace NuulEngine.Graphics.Infrastructure
{
    internal class Texture : IDisposable
    {
        private bool _isDisposed;

        private SamplerState _samplerState;
        private ShaderResourceView _shaderResourceView;

        public Texture(Texture2D textureData, int height, int width,
            ShaderResourceView shaderResourceView, SamplerState samplerState)
        {
            TextureData = textureData;
            Height = height;
            Width = width;
            _shaderResourceView = shaderResourceView;
            _samplerState = samplerState;
        }

        public int Height { get; }

        public int Width { get; }

        public Texture2D TextureData { get; }

        public SamplerState SamplerState { get => _samplerState; }

        public ShaderResourceView ShaderResourceView { get => _shaderResourceView; }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    Utilities.Dispose(ref _samplerState);
                    Utilities.Dispose(ref _shaderResourceView);
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
