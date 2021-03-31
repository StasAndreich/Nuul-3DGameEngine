using NuulEngine.Graphics.GraphicsUtilities;
using NuulEngine.Graphics.Infrastructure;

namespace NuulEngine.Core.Components
{
    public sealed class MeshRenderer : Component
    {
        private Mesh _mesh;
        public MeshRenderer(GameObject owner, string meshPath) 
            : base(owner)
        {
            //TODO: Make static load method
            //_mesh = Loader.LoadMesh(meshPath);
        }

        public override void CallComponent(double deltaTime)
        {
            //TODO: Add mesh object to rendering order
            //if(Owner.Active)
            //{
            //  Add to order
            //}
        }
    }
}