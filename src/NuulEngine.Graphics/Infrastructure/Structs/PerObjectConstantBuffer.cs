using System.Runtime.InteropServices;
using SharpDX;

namespace NullEngine.Graphics.Infrastructure.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct PerObjectConstantBuffer
    {
        public Matrix worldViewProjectionMatrix;

        public Matrix worldMatrix;

        public Matrix inverseTransposeWorldMatrix;

        public int timeScaling;

        public Vector3 _padding;
    }
}
