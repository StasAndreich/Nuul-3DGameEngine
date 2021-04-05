using System;
using NuulEngine.Graphics.Infrastructure.Structs;
using SharpDX;
using SharpDX.Direct3D11;

namespace NuulEngine.Graphics.Infrastructure
{
    internal class MeshObject : Object3D, IDisposable
    {
        private bool _isDisposed;

        private SharpDX.Direct3D11.Buffer _indicesBufferObject;

        private SharpDX.Direct3D11.Buffer _vertexBufferObject;

        public MeshObject(
            Device device, Vector4 position,
            float yaw, float pitch, float roll,
            Mesh mesh, Material material)
            : base(position, yaw, pitch, roll)
        {
            Material = material;
            Mesh = mesh;
            IsVisible = true;

            _vertexBufferObject = SharpDX.Direct3D11.Buffer
                .Create(
                    device: device,
                    bindFlags: BindFlags.VertexBuffer,
                    data: mesh.Vertices,
                    sizeInBytes: Utilities.SizeOf<VertexData>() * mesh.Vertices.Length);

            _indicesBufferObject = SharpDX.Direct3D11.Buffer
                .Create(
                    device: device,
                    bindFlags: BindFlags.IndexBuffer,
                    data: mesh.Indices,
                    sizeInBytes: Utilities.SizeOf<uint>() * mesh.Indices.Length);

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
