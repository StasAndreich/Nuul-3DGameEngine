using NuulEngine.Graphics.GraphicsUtilities;
using NuulEngine.Graphics.Infrastructure;
using SharpDX.Direct3D11;

namespace NuulEngine.Core.Components
{
    public sealed class MeshRenderer : Component
    {
        private readonly Mesh _mesh;

        private VertexShader _vertexShader;

        private PixelShader _pixelShader;

        public MeshRenderer(string meshPath)
            : base(null)
        {
            //TODO: Make static load method
            //_mesh = Loader.LoadMesh(meshPath);
        }

        public override void CallComponent(double deltaTime)
        {
            var M = new MeshObject()
            EngineCore.Instance.Renderer.AddToRenderQueue(_mesh);
        }
    }
}