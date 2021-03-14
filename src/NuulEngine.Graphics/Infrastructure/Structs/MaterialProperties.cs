using System.Runtime.InteropServices;
using SharpDX;

namespace NullEngine.Graphics.Infrastructure.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MaterialProperties
    {
        public Vector3 emmisiveK;

        public float _padding0;

        public Vector3 ambientK;

        public float _padding1;

        public Vector3 diffuseK;

        public float _padding2;

        public Vector3 specularK;

        public float specularPower;
    }
}
