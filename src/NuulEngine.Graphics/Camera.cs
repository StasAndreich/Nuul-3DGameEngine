using NullEngine.Graphics.Infrastructure;
using SharpDX;

namespace NullEngine.Graphics
{
    internal class Camera : Object3D
    {
        public Camera(Vector4 position,
            float yaw = 0, float pitch = 0, float roll = 0,
            float fov = MathUtil.PiOverFour, float aspectRatio = 1)
            : base(position, yaw, pitch, roll)
        {
            AspectRatio = aspectRatio;
            Fov = fov;
        }

        public float AspectRatio { get; set; }

        public float Fov { get; set; }

        public void MoveForZ(float direction)
        {
            Matrix rotation = Matrix.RotationYawPitchRoll(Yaw, Pitch, Roll);
            var viewTo = (Vector3) Vector4.Transform(Vector4.UnitZ * direction, rotation);
            MoveBy(viewTo.X, viewTo.Y, viewTo.Z);
        }

        public void MoveForX(float direction)
        {
            Matrix rotation = Matrix.RotationYawPitchRoll(Yaw, Pitch, Roll);
            var viewTo = (Vector3) Vector4.Transform(Vector4.UnitX * direction, rotation);
            MoveBy(viewTo.X, viewTo.Y, viewTo.Z);
        }

        public Matrix GetPojectionMatrix()
        {
            return Matrix.PerspectiveFovLH(
                fov: Fov,
                aspect: AspectRatio,
                znear: 0.1f,
                zfar: 100.0f);
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
