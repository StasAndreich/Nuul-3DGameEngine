using System;
using NullEngine.Graphics.Infrastructure.Structs;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace NullEngine.Graphics.Infrastructure
{
    internal class MeshObject : Object3D, IDisposable
    {
        private bool _isDisposed;

        private uint[] _indices;

        private VertexDataStruct[] _vertices;

        // ???.
        private DirectX3DGraphics _directX3DGraphics;

        private SharpDX.Direct3D11.Buffer _indicesBufferObject;

        private SharpDX.Direct3D11.Buffer _vertexBufferObject;

        public MeshObject(DirectX3DGraphics directX3DGraphics, Object3D object3D)
        {

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
