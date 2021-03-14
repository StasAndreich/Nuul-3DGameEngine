using System.Runtime.InteropServices;
using SharpDX;

namespace NullEngine.Graphics.Infrastructure.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct PerFrameConstantBuffer
    {
        public float time;

        public Vector3 _padding;
    }
}
