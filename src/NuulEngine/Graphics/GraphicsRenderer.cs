using System;
using NuulEngine.Graphics.Infrastructure;
using NuulEngine.Graphics.Infrastructure.Shaders;
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

        private Direct3DGraphicsContext _directX3DGraphics;

        private PerFrameDataConstantBufferBinder _perFrameConstantBufferObject;

        private PerObjectDataConstantBufferBinder _perObjectConstantBufferObject;

        private MaterialPropertiesConstantBufferBinder _materialPropertiesConstantBufferObject;
        

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

        public GraphicsRenderer(Direct3DGraphicsContext directX3DGraphics)
        {
            _directX3DGraphics = directX3DGraphics;
            
            MarkUpInputLayout();

        }

        public SamplerState AnisotropicSampler { get => _anisotropicSampler; }

        public SamplerState LinearSampler { get => _linearSampler; }

        public SamplerState PointSampler { get => _pointSampler; }

        internal void BeginRender()
        {
            _directX3DGraphics.ClearBuffers(Color.Black);
        }

        internal void CreateConstantBuffers()
        {
            _perFrameConstantBufferObject = 
                new PerFrameDataConstantBufferBinder(_directX3DGraphics.Device,
                    _directX3DGraphics.DeviceContext, _directX3DGraphics.DeviceContext.VertexShader, 0, 0);

            _perObjectConstantBufferObject =
                new PerObjectDataConstantBufferBinder(_directX3DGraphics.Device,
                _directX3DGraphics.DeviceContext, _directX3DGraphics.DeviceContext.VertexShader, 0, 0);

            _materialPropertiesConstantBufferObject =
                new MaterialPropertiesConstantBufferBinder(_directX3DGraphics.Device,
                _directX3DGraphics.DeviceContext, _directX3DGraphics.DeviceContext.PixelShader, 0, 0);
        }

        internal void EndRender()
        {
            _directX3DGraphics.SwapChain.Present(1, PresentFlags.Restart);
        }

        internal void RenderMeshObject(MeshObject meshObject)
        {
            SetMaterial(meshObject.Material);

            _directX3DGraphics.DeviceContext.InputAssembler.PrimitiveTopology = 
                meshObject.Mesh.PrimitiveTopology;
            // Check were _solidRasterizerState goes to.
            _directX3DGraphics.DeviceContext.Rasterizer.State = _solidRasterizerState;

            _directX3DGraphics.DeviceContext.InputAssembler
                .SetVertexBuffers(0, meshObject.VertexBufferBinding);
            _directX3DGraphics.DeviceContext.InputAssembler
                .SetIndexBuffer(meshObject.IndicesBufferObject, Format.R32_UInt, 0);
            _directX3DGraphics.DeviceContext.VertexShader.Set(_vertexShader);
            //
            // Inimplemented due to lack of lightning implementation.
            //
            //_directX3DGraphics.DeviceContext.PixelShader.Set(_pixelShader, _lightInterfaces);
            _directX3DGraphics.DeviceContext.DrawIndexed(meshObject.Mesh.IndicesCount, 0, 0);
        }

        private void SetMaterial(Material material)
        {
            if (_currentMaterial != material)
            {
                SetTexture(material.Texture);
                _currentMaterial = material;
                _materialPropertiesConstantBufferObject
                    .Update(material.MaterialProperties);
            }
        }

        private void SetTexture(Texture texture)
        {
            if (_currentTexture != texture && texture != null)
            {
                _directX3DGraphics.DeviceContext.PixelShader
                    .SetShaderResource(0, texture.ShaderResourceView);

                _directX3DGraphics.DeviceContext.PixelShader
                    .SetSampler(0, texture.SamplerState);

                _currentTexture = texture;
            }
        }

        /// <summary>
        /// Sets an input-layout object to describe 
        /// the input-buffer data for the input-assembler stage.
        /// </summary>
        private void MarkUpInputLayout()
        {
            InputElement[] inputElements = new[]
            {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("NORMAL", 0, Format.R32G32B32A32_Float, 16, 0),
                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 32, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float, 48, 0)
            };
            _directX3DGraphics.DeviceContext.InputAssembler.InputLayout =
                new InputLayout(_directX3DGraphics.Device, _shaderSignature, inputElements);
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
