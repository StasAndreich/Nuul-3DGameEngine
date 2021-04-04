using NuulEngine.Core;
using SharpDX.Direct3D11;

namespace NuulEngine.Graphics.Infrastructure
{
    /// <summary>
    /// Stores data needed for rendering a mesh.
    /// </summary>
    internal sealed class RenderQueueElement
    {
        public MeshObject MeshObject { get; set; }

        public PixelShader PixelShader { get; set; }

        public VertexShader VertexShader { get; set; }
    }
}
