using NuulEngine.Graphics.Infrastructure.Structs;
using SharpDX;

namespace NuulEngine.Graphics.Infrastructure
{
    public sealed class Material
    {
        public Material(Texture texture, Vector3 emmisiveK,
            Vector3 ambientK, Vector3 diffuseK,
            Vector3 specularK, float specularPower)
        {
            MaterialProperties = new MaterialProperties
            {
                emmisiveK = emmisiveK,
                ambientK = ambientK,
                diffuseK = diffuseK,
                specularK = specularK,
                specularPower = specularPower,
            };
            Texture = texture;
        }

        internal MaterialProperties MaterialProperties { get; }

        public Texture Texture { get; }
    }
}
