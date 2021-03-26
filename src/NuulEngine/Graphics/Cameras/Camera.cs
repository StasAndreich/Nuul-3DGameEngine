using NuulEngine.Graphics.Infrastructure;
using SharpDX;

namespace NuulEngine.Graphics.Cameras
{
    internal abstract class Camera : Object3D
    {
        protected Matrix projectionMatrix;

        public Camera(Vector4 position,
            float yaw = 0, float pitch = 0, float roll = 0)
            : base(position, yaw, pitch, roll)
        {
            projectionMatrix = CreateProjectionMatrix();
        }

        protected abstract Matrix CreateProjectionMatrix();

        public Matrix GetPojectionMatrix()
        {
            return projectionMatrix;
        }

        public Matrix GetViewMatrix()
        {
            Matrix rotation = Matrix.RotationYawPitchRoll(Yaw, Pitch, Roll);
            var viewTo = (Vector3)Vector4.Transform(Vector4.UnitZ, rotation);
            var viewUp = (Vector3)Vector4.Transform(Vector4.UnitY, rotation);

            return Matrix.LookAtLH((Vector3)Position, (Vector3)Position + viewTo, viewUp);
        }
    }
}
