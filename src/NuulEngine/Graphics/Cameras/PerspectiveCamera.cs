using SharpDX;

namespace NuulEngine.Graphics.Cameras
{
    internal sealed class PerspectiveCamera : Camera
    {
        private float _aspectRatio;
        
        private float _fov;

        public PerspectiveCamera(Vector4 position,
            float fov = MathUtil.PiOverFour, float aspectRatio = 1,
            float yaw = 0, float pitch = 0, float roll = 0)
            : base(position, yaw, pitch, roll)
        {
            _aspectRatio = aspectRatio;
            _fov = fov;
        }

        public float AspectRatio
        {
            get => _aspectRatio;
            set
            {
                _aspectRatio = value;
                projectionMatrix = CreateProjectionMatrix();
            }
        }

        public float Fov
        { 
            get => _fov;
            set
            {
                _fov = value;
                projectionMatrix = CreateProjectionMatrix();
            }
        }

        protected override Matrix CreateProjectionMatrix()
        {
            return Matrix.PerspectiveFovLH(
                fov: Fov,
                aspect: AspectRatio,
                znear: 0.1f,
                zfar: 100.0f);
        }
    }
}
