using System;
using NuulEngine.Graphics.Infrastructure;
using NuulEngine.Graphics.Infrastructure.Structs;
using SharpDX;
using SharpDX.DXGI;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;

namespace NuulEngine.Graphics
{
    internal class GraphicsRenderer : IDisposable
    {
        private bool _isDisposed;

        private SharpDX.Direct3D11.Buffer _perFrameConstantBufferObject;

        private SharpDX.Direct3D11.Buffer _perObjectConstantBufferObject;

        private SharpDX.Direct3D11.Buffer _materialPropertiesConstantBufferObject;

        
        private SharpDX.Direct3D11.Device _device;

        private DeviceContext _deviceContext;

        private DirectX3DGraphics _directX3DGraphics;


        private InputLayout _inputLayout;

        private PerFrameData _perFrameConstantBuffer;

        private PerObjectData _perObjectConstantBuffer;

        private VertexShader _vertexShader;

        private PixelShader _pixelShader;

        private Material _currentMaterial;
        private Texture _currentTexture;

        private ShaderSignature _shaderSignature;

        private SamplerState _anisotropicSampler;
        private SamplerState _linearSampler;
        private SamplerState _pointSampler;

        private ClassLinkage _pixelShaderClassLinkage;
        private ClassLinkage _vertexShaderClassLinkage;

        private RasterizerState _solidRasterizerState;
        private RasterizerState _wireframeRasterizerState;

        public GraphicsRenderer(DirectX3DGraphics directX3DGraphics)
        {
            _directX3DGraphics = directX3DGraphics;
            _device = _directX3DGraphics.Device;
            _deviceContext = _directX3DGraphics.DeviceContext;
        }

        public SamplerState AnisotropicSampler { get => _anisotropicSampler; }

        public SamplerState LinearSampler { get => _linearSampler; }

        public SamplerState PointSampler { get => _pointSampler; }

        public void BeginRender()
        {
            _directX3DGraphics.ClearBuffers(Color.Black);
        }

        public void CreateConstantBuffers()
        {
            _perFrameConstantBufferObject = new SharpDX.Direct3D11.Buffer(
                _device,
                Utilities.SizeOf<PerFrameData>(),
                ResourceUsage.Dynamic,
                BindFlags.ConstantBuffer,
                CpuAccessFlags.Write,
                ResourceOptionFlags.None,
                0);

            _perObjectConstantBufferObject = new SharpDX.Direct3D11.Buffer(
                _device,
                Utilities.SizeOf<PerObjectData>(),
                ResourceUsage.Dynamic,
                BindFlags.ConstantBuffer,
                CpuAccessFlags.Write,
                ResourceOptionFlags.None,
                0);

            _materialPropertiesConstantBufferObject = new SharpDX.Direct3D11.Buffer(
                _device,
                Utilities.SizeOf<MaterialProperties>(),
                ResourceUsage.Dynamic,
                BindFlags.ConstantBuffer,
                CpuAccessFlags.Write,
                ResourceOptionFlags.None,
                0);
        }

        public void EndRender()
        {
            _directX3DGraphics.SwapChain.Present(1, PresentFlags.Restart);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    Utilities.Dispose(ref _materialPropertiesConstantBufferObject);
                    Utilities.Dispose(ref _perObjectConstantBufferObject);
                    Utilities.Dispose(ref _perFrameConstantBufferObject);
                    Utilities.Dispose(ref _linearSampler);
                    Utilities.Dispose(ref _anisotropicSampler);
                    Utilities.Dispose(ref _inputLayout);
                    Utilities.Dispose(ref _shaderSignature);
                    //DisposeIllumination();
                    Utilities.Dispose(ref _pixelShader);
                    Utilities.Dispose(ref _vertexShader);
                    Utilities.Dispose(ref _pixelShaderClassLinkage);
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
