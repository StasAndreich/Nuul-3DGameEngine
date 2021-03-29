using System;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;
using NuulEngine.Graphics.Infrastructure.Light;

namespace NuulEngine.Graphics
{
    internal sealed partial class GraphicsRenderer
    {
        private const int MaxLightsCount = 8;

        private readonly int _lightSourceTypeCount = Enum
            .GetNames(typeof(LightSourceType)).Length;

        private readonly string[] lightClassVariableNames = new []
        {
            "baseLight",
            "directinalLight",
            "pointLight",
            "spotLight",
        };

        private int[] _lightVariableOffsets = new int[MaxLightsCount];

        private SharpDX.Direct3D11.Buffer _illuminationPropertiesBufferObject;

        private ClassInstance[] _lightInterfaces;

        private ClassInstance[] _lightInstances;

        private void InitializeIllumination(CompilationResult pixelShaderByteCode, ClassLinkage pixelShaderClassLinkage)
        {
            var pixelShaderReflection = new ShaderReflection(pixelShaderByteCode);
            var lightInterfaceCount = pixelShaderReflection.InterfaceSlotCount;
            
            if (lightInterfaceCount != MaxLightsCount)
            {
                throw new IndexOutOfRangeException("Light interfaces count is bigger than MaxLightCount.");
            }

            _lightInterfaces = new ClassInstance[lightInterfaceCount];
            ShaderReflectionVariable shaderVariableLights = pixelShaderReflection.GetVariable("lights");

            for (int i = 0; i <= MaxLightsCount - 1; ++i)
            {
                // i - array element index. For non-array = 0.
                _lightVariableOffsets[i] = shaderVariableLights
                    .GetInterfaceSlot(i);
            }

            _lightInstances = new ClassInstance[_lightSourceTypeCount];
            for (int i = 0; i <= _lightSourceTypeCount - 1; ++i)
            {
                _lightInstances[i] = pixelShaderClassLinkage
                    .GetClassInstance(lightClassVariableNames[i], 0);
            }

            Utilities.Dispose(ref pixelShaderByteCode);
            Utilities.Dispose(ref shaderVariableLights);
            Utilities.Dispose(ref pixelShaderReflection);
        }

        private void CreateIlluminationConstantBuffer()
        {
            _illuminationPropertiesBufferObject = new SharpDX.Direct3D11.Buffer(
                _directX3DGraphicsContext.Device,
                Utilities.SizeOf<IlluminationProperties>(),
                ResourceUsage.Dynamic,
                BindFlags.ConstantBuffer,
                CpuAccessFlags.Write,
                ResourceOptionFlags.None,
                0);
        }

        public void UpdateIllumination(IlluminationProperties illuminationProperties)
        {
            _directX3DGraphicsContext.DeviceContext.MapSubresource(
                _illuminationPropertiesBufferObject,
                MapMode.WriteDiscard,
                MapFlags.None,
                out DataStream dataStream);
            dataStream.Write(illuminationProperties);

            _directX3DGraphicsContext.DeviceContext
                .UnmapSubresource(_illuminationPropertiesBufferObject, 0);
            _directX3DGraphicsContext.DeviceContext.PixelShader
                .SetConstantBuffer(1, _illuminationPropertiesBufferObject);

            for (int i = 0; i < MaxLightsCount; ++i)
            {
                _lightInterfaces[_lightVariableOffsets[i]] = _lightInstances[(int)illuminationProperties[i].lightSourceType];
            }
        }

        public void DisposeIllumination()
        {
            Utilities.Dispose(ref _illuminationPropertiesBufferObject);
            for (int i = 0; i <= _lightSourceTypeCount - 1; ++i)
                Utilities.Dispose(ref _lightInstances[i]);
            _lightInterfaces = null;
        }
    }
}
