using SharpDX;

namespace NullEngine.Graphics.Infrastructure
{
    internal class Object3D
    {
        private float _yaw;

        private float _pitch;

        private float _roll;

        private Vector4 _position;

        public Object3D(Vector4 position, float yaw = 0, float pitch = 0, float roll = 0)
        {
            _position = position;
            _yaw = yaw;
            _pitch = pitch;
            _roll = roll;
        }

        public Vector4 Position { get => _position; }

        public float Yaw { get => _yaw; }

        public float Pitch { get => _pitch; }

        public float Roll { get => _roll; }

        public virtual void YawByAngle(float deltaYaw)
        {
            _yaw += deltaYaw;
            LimitAngleByPlusMinusPi(ref _yaw);
        }

        public virtual void PitchByAngle(float deltaPitch)
        {
            _pitch += deltaPitch;
            LimitAngleByPlusMinusPi(ref _pitch);
        }

        public virtual void RollByAngle(float deltaRoll)
        {
            _roll += deltaRoll;
            LimitAngleByPlusMinusPi(ref _roll);
        }

        public virtual void MoveBy(float deltaX, float deltaY, float deltaZ)
        {
            _position.X += deltaX;
            _position.Y += deltaY;
            _position.Z += deltaZ;
        }
        public virtual void MoveBy(Vector4 delta)
        {
            _position += delta;
            _position.W = 1.0f;
        }

        public virtual void MoveTo(float x, float y, float z)
        {
            _position.X = x;
            _position.Y = y;
            _position.Z = z;
        }

        public Matrix GetWorldMatrix()
        {
            return Matrix.Multiply(Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll), Matrix.Translation((Vector3)_position));
        }

        private void LimitAngleByPlusMinusPi(ref float angle)
        {
            if (angle > MathUtil.Pi)
            {
                angle -= MathUtil.TwoPi;
            }
            else if (angle < -MathUtil.Pi)
            {
                angle += MathUtil.TwoPi;
            }
        }
    }
}
