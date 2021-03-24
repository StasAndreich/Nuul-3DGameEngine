using System;
using NuulEngine.Graphics.Infrastructure;
using SharpDX;

namespace NuulEngine.Graphics
{
    internal sealed class Camera : Object3D
    {
        private readonly Matrix _projectionMatrix;

        public Camera(CameraType cameraType, Vector4 position,
            float yaw = 0, float pitch = 0, float roll = 0,
            float fov = MathUtil.PiOverFour, float aspectRatio = 1)
            : base(position, yaw, pitch, roll)
        {
            AspectRatio = aspectRatio;
            Fov = fov;
            _projectionMatrix = CreateProjectionMatrix(cameraType);
        }

        public float AspectRatio { get; set; }

        public float Fov { get; set; }

        private Matrix CreateProjectionMatrix(CameraType cameraType)
        {
            switch (cameraType)
            {
                case CameraType.Perspective:
                    return Matrix.PerspectiveFovLH(
                        fov: Fov,
                        aspect: AspectRatio,
                        znear: 0.1f,
                        zfar: 100.0f);
                default:
                    throw new ArgumentException(nameof(cameraType), "Such type is unsupported.");
            }
        }

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
            return _projectionMatrix;
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
