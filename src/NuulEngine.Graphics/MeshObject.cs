using System;
using NullEngine.Graphics.Infrastructure.Structs;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace NuulEngine.Graphics.Infrastructure
{
    public class MeshObject : Object3D, IDisposable
    {
        private bool _isDisposed;

        private readonly uint[] _indices;

        private readonly VertexDataStruct[] _vertices;

        private readonly DirectX3DGraphics _directX3DGraphics;

        private SharpDX.Direct3D11.Buffer _indicesBufferObject;

        private SharpDX.Direct3D11.Buffer _vertexBufferObject;

        public MeshObject(
            DirectX3DGraphics directX3DGraphics, Vector4 position,
            float yaw, float pitch, float roll,
            VertexDataStruct[] vertices, uint[] indices,
            PrimitiveTopology primitiveTopology, Material material)
            : base(position, yaw, pitch, roll)
        {
            _directX3DGraphics = directX3DGraphics;
            _vertices = vertices;
            _indices = indices;
            PrimitiveTopology = primitiveTopology;
            Material = material;
            IsVisible = true;

            _vertexBufferObject = SharpDX.Direct3D11.Buffer
                .Create(
                    device: _directX3DGraphics.Device,
                    bindFlags: BindFlags.VertexBuffer,
                    data: _vertices,
                    sizeInBytes: Utilities.SizeOf<VertexDataStruct>() * VerticesCount);

            _indicesBufferObject = SharpDX.Direct3D11.Buffer
                .Create(
                    device: _directX3DGraphics.Device,
                    bindFlags: BindFlags.IndexBuffer,
                    data: _indices,
                    sizeInBytes: Utilities.SizeOf<uint>() * IndicesCount);

            VertexBufferBinding = new VertexBufferBinding(
                buffer: _vertexBufferObject,
                stride: Utilities.SizeOf<VertexDataStruct>(),
                offset: 0);
        }

        public bool IsVisible { get; set; }

        public int VerticesCount { get => _vertices.Length; }

        public int IndicesCount { get => _indices.Length; }

        public SharpDX.Direct3D11.Buffer IndicesBufferObject { get => _indicesBufferObject; }

        public SharpDX.Direct3D11.Buffer VertexBufferObject { get => _vertexBufferObject; }

        public VertexBufferBinding VertexBufferBinding { get; private set; }

        public Material Material { get; private set; }

        public PrimitiveTopology PrimitiveTopology { get; private set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    Utilities.Dispose(ref _indicesBufferObject);
                    Utilities.Dispose(ref _vertexBufferObject);
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
