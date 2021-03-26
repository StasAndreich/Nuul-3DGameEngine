using SharpDX;

namespace NuulEngine.Graphics.Cameras
{
    internal sealed class OrthographicCamera : Camera
    {
        private float _size;

        public OrthographicCamera(Vector4 position, float size,
            float yaw = 0, float pitch = 0, float roll = 0)
            : base(position, yaw, pitch, roll)
        {
            _size = size;
        }

        public float Size
        {
            get => _size;
            set
            {
                _size = value;
                projectionMatrix = CreateProjectionMatrix();
            }
        }

        protected override Matrix CreateProjectionMatrix()
        {
            return Matrix.OrthoLH(
                width: _size,
                height: _size,
                znear: 0.1f,
                zfar: 100.0f);
        }
    }
}
