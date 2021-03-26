using System.Runtime.InteropServices;
using SharpDX;

namespace NuulEngine.Graphics.Infrastructure.Light
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct LightSource
    {
        public LightSourceType lightSourceType;

        public Vector3 color;

        public Vector3 position;

        public float _padding0;

        public Vector3 direction;

        public float _padding1;

        public float spotAngle;

        public Vector3 attenuation;

        public float ConstantAttenuation { get => attenuation.X; set => attenuation.X = value; }

        public float LinearAttenuation { get => attenuation.Y; set => attenuation.Y = value; }

        public float QuadraticAttenuation { get => attenuation.Z; set => attenuation.Z = value; }
    }
}
