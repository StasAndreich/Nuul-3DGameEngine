using System;
using SharpDX;
using SharpDX.Direct3D11;

namespace NuulEngine.Graphics.Infrastructure.Shaders
{
    internal abstract class ConstantBuffer<TStruct> : IDisposable
        where TStruct : struct
    {
        private bool _isDisposed;

        private readonly int _subresourse;
        
        private readonly int _slot;

        private readonly CommonShaderStage _commonShaderStage;

        private readonly DeviceContext _deviceContext;

        private SharpDX.Direct3D11.Buffer _buffer;

        public ConstantBuffer(Device device, DeviceContext deviceContext,
            CommonShaderStage commonShaderStage, int subresourse, int slot)
        {
            _deviceContext = deviceContext;
            _commonShaderStage = commonShaderStage;
            _subresourse = subresourse;
            _slot = slot;
            _buffer = new SharpDX.Direct3D11.Buffer(
                device: device,
                sizeInBytes: Utilities.SizeOf<TStruct>(),
                usage: ResourceUsage.Dynamic,
                bindFlags: BindFlags.ConstantBuffer,
                accessFlags: CpuAccessFlags.Write,
                optionFlags: ResourceOptionFlags.None,
                structureByteStride: 0);
        }

        public void Update(TStruct data)
        {
            _deviceContext.MapSubresource(_buffer, MapMode.WriteDiscard,
                MapFlags.None, out DataStream dataStream);

            dataStream.Write(data);
            _deviceContext.UnmapSubresource(_buffer, _subresourse);
            _commonShaderStage.SetConstantBuffer(_slot, _buffer);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    Utilities.Dispose(ref _buffer);
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
