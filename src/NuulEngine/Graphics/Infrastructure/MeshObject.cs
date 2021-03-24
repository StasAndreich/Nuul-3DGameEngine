using System;
using NuulEngine.Graphics.Infrastructure;
using NuulEngine.Graphics.Infrastructure.Structs;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace NuulEngine.Graphics.Infrastructure
{
    internal class MeshObject : Object3D, IDisposable
    {
        private bool _isDisposed;

        private readonly Direct3DGraphicsContext _directX3DGraphics;

        private SharpDX.Direct3D11.Buffer _indicesBufferObject;

        private SharpDX.Direct3D11.Buffer _vertexBufferObject;

        public MeshObject(
            Direct3DGraphicsContext directX3DGraphics, Vector4 position,
            float yaw, float pitch, float roll,
            Mesh mesh, Material material)
            : base(position, yaw, pitch, roll)
        {
            _directX3DGraphics = directX3DGraphics;
            Material = material;
            Mesh = mesh;
            IsVisible = true;

            _vertexBufferObject = SharpDX.Direct3D11.Buffer
                .Create(
                    device: _directX3DGraphics.Device,
                    bindFlags: BindFlags.VertexBuffer,
                    data: mesh.Vertices,
                    sizeInBytes: Utilities.SizeOf<VertexData>() * mesh.VerticesCount);

            _indicesBufferObject = SharpDX.Direct3D11.Buffer
                .Create(
                    device: _directX3DGraphics.Device,
                    bindFlags: BindFlags.IndexBuffer,
                    data: mesh.Indices,
                    sizeInBytes: Utilities.SizeOf<uint>() * mesh.IndicesCount);

            VertexBufferBinding = new VertexBufferBinding(
                buffer: _vertexBufferObject,
                stride: Utilities.SizeOf<VertexData>(),
                offset: 0);
        }

        public bool IsVisible { get; set; }

        public SharpDX.Direct3D11.Buffer IndicesBufferObject { get => _indicesBufferObject; }

        public SharpDX.Direct3D11.Buffer VertexBufferObject { get => _vertexBufferObject; }

        public VertexBufferBinding VertexBufferBinding { get; private set; }

        public Material Material { get; private set; }

        public Mesh Mesh { get; private set; }

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
