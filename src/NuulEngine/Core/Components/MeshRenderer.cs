using NuulEngine.Graphics.GraphicsUtilities;
using NuulEngine.Graphics.Infrastructure;

namespace NuulEngine.Core.Components
{
    public sealed class MeshRenderer : Component
    {
        private Mesh _mesh;

        public MeshRenderer(string meshPath)
            : base(null)
        {
            //TODO: Make static load method
            //_mesh = Loader.LoadMesh(meshPath);
        }

        public override void CallComponent(double deltaTime)
        {
            EngineCore.Instance.Renderer.AddToRenderQueue(_mesh);
        }
    }
}