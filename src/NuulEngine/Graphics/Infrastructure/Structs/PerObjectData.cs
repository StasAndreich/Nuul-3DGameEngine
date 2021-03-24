using System.Runtime.InteropServices;
using SharpDX;

namespace NuulEngine.Graphics.Infrastructure.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct PerObjectData
    {
        public Matrix worldViewProjectionMatrix;

        public Matrix worldMatrix;

        public Matrix inverseTransposeWorldMatrix;

        public int timeScaling;

        public Vector3 _padding;
    }
}
