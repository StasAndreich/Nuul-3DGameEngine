using NullEngine.Graphics.Infrastructure.Structs;
using SharpDX;

namespace NullEngine.Graphics.Infrastructure
{
    internal class Material
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
                specularPower = specularPower
            };
            Texture = texture;
        }

        public MaterialProperties MaterialProperties { get; }

        public Texture Texture { get; }
    }
}
