using System.Runtime.InteropServices;
using SharpDX;

namespace NuulEngine.Graphics.Infrastructure.Light
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct IlluminationProperties
    {
        public Vector4 eyePosition;

        public Vector3 globalAmbient;

        public float _padding;

        public LightSource lightSource0;
        public LightSource lightSource1;
        public LightSource lightSource2;
        public LightSource lightSource3;
        public LightSource lightSource4;
        public LightSource lightSource5;
        public LightSource lightSource6;
        public LightSource lightSource7;

        public LightSource this[int index]
        {
            get
            {
                LightSource l = new LightSource();
                switch (index)
                {
                    case 0:
                        l = lightSource0;
                        break;
                    case 1:
                        l = lightSource1;
                        break;
                    case 2:
                        l = lightSource2;
                        break;
                    case 3:
                        l = lightSource3;
                        break;
                    case 4:
                        l = lightSource4;
                        break;
                    case 5:
                        l = lightSource5;
                        break;
                    case 6:
                        l = lightSource6;
                        break;
                    case 7:
                        l = lightSource7;
                        break;
                }
                return l;
            }
            set
            {
                switch (index)
                {
                    case 0:
                        lightSource0 = value;
                        break;
                    case 1:
                        lightSource1 = value;
                        break;
                    case 2:
                        lightSource2 = value;
                        break;
                    case 3:
                        lightSource3 = value;
                        break;
                    case 4:
                        lightSource4 = value;
                        break;
                    case 5:
                        lightSource5 = value;
                        break;
                    case 6:
                        lightSource6 = value;
                        break;
                    case 7:
                        lightSource7 = value;
                        break;
                }
            }
        }
    }
}
