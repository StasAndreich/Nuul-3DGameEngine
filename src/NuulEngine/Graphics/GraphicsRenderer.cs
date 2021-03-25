using System;
using NuulEngine.Graphics.Infrastructure;
using NuulEngine.Graphics.Infrastructure.Shaders;
using NuulEngine.Graphics.Infrastructure.Structs;
using SharpDX;
using SharpDX.DXGI;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SharpDX.Mathematics.Interop;

namespace NuulEngine.Graphics
{
    internal sealed class GraphicsRenderer : IDisposable
    {
        private bool _isDisposed;

        private readonly Direct3DGraphicsContext _directX3DGraphicsContext;

        private PerFrameDataConstantBufferBinder _perFrameConstantBufferBinder;

        private PerObjectDataConstantBufferBinder _perObjectConstantBufferBinder;

        private MaterialPropertiesConstantBufferBinder _materialPropertiesConstantBufferBinder;
        

        private PerFrameData _perFrameConstantBuffer;
        private PerObjectData _perObjectConstantBuffer;

        private VertexShader _vertexShader;
        private PixelShader _pixelShader;

        private Material _currentMaterial;
        private Texture _currentTexture;

        private SamplerState _anisotropicSampler;
        private SamplerState _linearSampler;
        private SamplerState _pointSampler;

        private RasterizerState _rasterizerState;

        public GraphicsRenderer(Direct3DGraphicsContext directX3DGraphics)
        {
            _directX3DGraphicsContext = directX3DGraphics;
            
            InitializeVertexShader("shader", out ShaderSignature shaderSignature);
            InitializePixelShader("shader");
            MarkUpInputLayout(shaderSignature);
            SetRasterizerState();
            InitializeSamplers();
        }

        [Obsolete("Use carefully", false)]
        public SamplerState AnisotropicSampler { get => _anisotropicSampler; }

        [Obsolete("Use carefully", false)]
        public SamplerState LinearSampler { get => _linearSampler; }

        [Obsolete("Use carefully", false)]
        public SamplerState PointSampler { get => _pointSampler; }

        internal void BeginRender()
        {
            _directX3DGraphicsContext.ClearBuffers(Color.Black);
        }

        internal void CreateConstantBuffers()
        {
            _perFrameConstantBufferBinder = 
                new PerFrameDataConstantBufferBinder(_directX3DGraphicsContext.Device,
                    _directX3DGraphicsContext.DeviceContext, _directX3DGraphicsContext.DeviceContext.VertexShader, 0, 0);

            _perObjectConstantBufferBinder =
                new PerObjectDataConstantBufferBinder(_directX3DGraphicsContext.Device,
                _directX3DGraphicsContext.DeviceContext, _directX3DGraphicsContext.DeviceContext.VertexShader, 0, 0);

            _materialPropertiesConstantBufferBinder =
                new MaterialPropertiesConstantBufferBinder(_directX3DGraphicsContext.Device,
                _directX3DGraphicsContext.DeviceContext, _directX3DGraphicsContext.DeviceContext.PixelShader, 0, 0);
        }

        internal void EndRender()
        {
            _directX3DGraphicsContext.SwapChain.Present(1, PresentFlags.Restart);
        }

        internal void RenderMeshObject(MeshObject meshObject)
        {
            SetMaterial(meshObject.Material);

            _directX3DGraphicsContext.DeviceContext.InputAssembler.PrimitiveTopology = 
                meshObject.Mesh.PrimitiveTopology;

            // Check were _rasterizerState goes to.
            _directX3DGraphicsContext.DeviceContext.Rasterizer.State = _rasterizerState;

            _directX3DGraphicsContext.DeviceContext.InputAssembler
                .SetVertexBuffers(0, meshObject.VertexBufferBinding);
            _directX3DGraphicsContext.DeviceContext.InputAssembler
                .SetIndexBuffer(meshObject.IndicesBufferObject, Format.R32_UInt, 0);
            _directX3DGraphicsContext.DeviceContext.VertexShader.Set(_vertexShader);

            //
            // Inimplemented due to lack of lightning implementation.
            //
            //_directX3DGraphics.DeviceContext.PixelShader.Set(_pixelShader, _lightInterfaces);
            _directX3DGraphicsContext.DeviceContext.DrawIndexed(meshObject.Mesh.IndicesCount, 0, 0);
        }

        internal void UpdatePerFrameConstantBuffers(float deltaTime)
        {
            var data = new PerFrameData { time = deltaTime };
            _perFrameConstantBufferBinder.Update(data);
        }

        internal void UpdatePerObjectConstantBuffers(Matrix world, Matrix view,
            Matrix projection, int timeScaling)
        {
            Matrix worldViewProjection = Matrix
                    .Multiply(Matrix.Multiply(world, view), projection);
            worldViewProjection.Transpose();
            Matrix worldMatrix = world;
            worldMatrix.Transpose();

            var data = new PerObjectData
            { 
                worldViewProjectionMatrix = worldViewProjection,
                worldMatrix = worldMatrix,
                inverseTransposeWorldMatrix = Matrix.Invert(world),
                timeScaling = timeScaling,
            };
            _perObjectConstantBufferBinder.Update(data);
        }

        private void SetMaterial(Material material)
        {
            if (_currentMaterial != material)
            {
                SetTexture(material.Texture);
                _currentMaterial = material;
                _materialPropertiesConstantBufferBinder
                    .Update(material.MaterialProperties);
            }
        }

        private void SetTexture(Texture texture)
        {
            if (_currentTexture != texture && texture != null)
            {
                _directX3DGraphicsContext.DeviceContext.PixelShader
                    .SetShaderResource(0, texture.ShaderResourceView);

                _directX3DGraphicsContext.DeviceContext.PixelShader
                    .SetSampler(0, texture.SamplerState);

                _currentTexture = texture;
            }
        }

        private void SetRasterizerState()
        {
            var _rasterizerStateDescription = new RasterizerStateDescription
            {
                FillMode = FillMode.Solid,
                CullMode = CullMode.Front,
                IsFrontCounterClockwise = true,
                IsMultisampleEnabled = true,
                IsAntialiasedLineEnabled = true,
                IsDepthClipEnabled = true,
            };
            _rasterizerState = new RasterizerState(_directX3DGraphicsContext.Device,
                _rasterizerStateDescription);
        }

        private void InitializeVertexShader(string fileName, out ShaderSignature shaderSignature)
        {
            CompilationResult vertexShaderByteCode = ShaderBytecode
                .CompileFromFile(fileName, "vertexShader", "vs_5_0",
                ShaderFlags.None, EffectFlags.None, null, new IncludeHandler());

            _vertexShader = new VertexShader(_directX3DGraphicsContext.Device,
                vertexShaderByteCode, new ClassLinkage(_directX3DGraphicsContext.Device));
            shaderSignature = ShaderSignature.GetInputSignature(vertexShaderByteCode);
            Utilities.Dispose(ref vertexShaderByteCode);
        }

        private void InitializePixelShader(string fileName)
        {
            CompilationResult pixelShaderByteCode =
                ShaderBytecode.CompileFromFile(fileName, "pixelShader", "ps_5_0",
                ShaderFlags.EnableStrictness | ShaderFlags.SkipOptimization | ShaderFlags.Debug,
                EffectFlags.None, null, new IncludeHandler());

            _pixelShader = new PixelShader(_directX3DGraphicsContext.Device,
                pixelShaderByteCode, new ClassLinkage(_directX3DGraphicsContext.Device));
            Utilities.Dispose(ref pixelShaderByteCode);
        }

        private void InitializeSamplers()
        {
            var samplerStateDescription = new SamplerStateDescription
                {
                    Filter = Filter.Anisotropic,
                    AddressU = TextureAddressMode.Clamp,
                    AddressV = TextureAddressMode.Clamp,
                    AddressW = TextureAddressMode.Clamp,
                    MipLodBias = 0,
                    MaximumAnisotropy = 16,
                    ComparisonFunction = Comparison.Never,
                    BorderColor = new RawColor4(1.0f, 1.0f, 1.0f, 1.0f),
                    MinimumLod = 0,
                    MaximumLod = float.MaxValue,
                };

            _anisotropicSampler = new SamplerState(_directX3DGraphicsContext.Device, samplerStateDescription);
            samplerStateDescription.Filter = Filter.MinMagMipLinear;
            _linearSampler = new SamplerState(_directX3DGraphicsContext.Device, samplerStateDescription);
            _pointSampler = new SamplerState(_directX3DGraphicsContext.Device, samplerStateDescription);
        }

        /// <summary>
        /// Sets an input-layout object to describe
        /// the input-buffer data for the input-assembler stage.
        /// </summary>
        private void MarkUpInputLayout(ShaderSignature shaderSignature)
        {
            InputElement[] inputElements = new[]
            {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("NORMAL", 0, Format.R32G32B32A32_Float, 16, 0),
                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 32, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float, 48, 0)
            };
            _directX3DGraphicsContext.DeviceContext.InputAssembler.InputLayout =
                new InputLayout(_directX3DGraphicsContext.Device, shaderSignature, inputElements);
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    Utilities.Dispose(ref _materialPropertiesConstantBufferBinder);
                    Utilities.Dispose(ref _perObjectConstantBufferBinder);
                    Utilities.Dispose(ref _perFrameConstantBufferBinder);
                    Utilities.Dispose(ref _linearSampler);
                    Utilities.Dispose(ref _anisotropicSampler);
                    Utilities.Dispose(ref _pointSampler);
                    //DisposeIllumination();
                    Utilities.Dispose(ref _pixelShader);
                    Utilities.Dispose(ref _vertexShader);
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
