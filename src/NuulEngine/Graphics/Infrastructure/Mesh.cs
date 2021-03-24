using NuulEngine.Graphics.Infrastructure.Structs;
using SharpDX.Direct3D;

namespace NuulEngine.Graphics.Infrastructure
{
    public sealed class Mesh
    {
        public Mesh(VertexData[] vertices, uint[] indices,
            PrimitiveTopology primitiveTopology)
        {
            Vertices = vertices;
            Indices = indices;
            PrimitiveTopology = primitiveTopology;
        }

        public uint[] Indices { get; private set; }

        public int IndicesCount { get => Indices.Length; }

        public VertexData[] Vertices { get; private set; }

        public int VerticesCount { get => Vertices.Length; }

        public PrimitiveTopology PrimitiveTopology { get; private set; }
    }
}
