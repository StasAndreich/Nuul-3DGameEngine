using NuulEngine.Graphics.Infrastructure.Structs;
using SharpDX.Direct3D;

namespace NuulEngine.Graphics.Infrastructure
{
    internal sealed class Mesh
    {
        public Mesh(VertexData[] vertices, uint[] indices,
            PrimitiveTopology primitiveTopology = PrimitiveTopology.TriangleList)
        {
            Vertices = vertices;
            Indices = indices;
            PrimitiveTopology = primitiveTopology;
        }

        public uint[] Indices { get; private set; }

        public VertexData[] Vertices { get; private set; }

        public PrimitiveTopology PrimitiveTopology { get; private set; }
    }
}
