using NuulEngine.Graphics.Infrastructure.Structs;
using SharpDX.Direct3D11;

namespace NuulEngine.Graphics.Infrastructure.Shaders
{
    internal sealed class PerObjectDataConstantBufferBinder : ConstantBufferBinder<PerObjectData>
    {
        public PerObjectDataConstantBufferBinder(Device device, DeviceContext deviceContext,
            CommonShaderStage commonShaderStage, int subresource, int slot)
            : base(device, deviceContext, commonShaderStage, subresource, slot)
        {
        }
    }
}
