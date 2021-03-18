using System.Runtime.InteropServices;
using SharpDX;

namespace NuulEngine.Graphics.Infrastructure.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct VertexDataStruct
    {
        public Vector4 position;

        public Vector4 normal;

        public Vector4 color;

        public Vector2 texCoord;
    }
}
