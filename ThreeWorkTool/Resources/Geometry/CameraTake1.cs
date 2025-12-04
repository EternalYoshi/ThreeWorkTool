using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Mathematics.Interop;

namespace ThreeWorkTool.Resources.Geometry
{
    public class CameraTake1
    {
        public Vector3 Position;
        public Vector3 Target;
        public float Distance;

        public float Yaw;
        public float Pitch;

        public float MoveSpeed = 5f;
        public float ZoomSpeed = 1f;
        public float RotateSpeed = 0.01f;
        public float PanSpeed = 0.01f;

        public float MinDistance = 1f;
        public float MaxDistance = 100f;

        public CameraTake1()
        {
            Reset();
        }

        public Matrix View => Matrix.LookAtLH(Position, Target, Vector3.UnitY);

        public void UpdateCameraPosition()
        {
            // Spherical to Cartesian
            float x = Distance * (float)(Math.Cos(Pitch) * Math.Sin(Yaw));
            float y = Distance * (float)(Math.Sin(Pitch));
            float z = Distance * (float)(Math.Cos(Pitch) * Math.Cos(Yaw));

            Position = Target + new Vector3(x, y, z);
        }

        public void Rotate(float deltaX, float deltaY)
        {
            Yaw += deltaX * RotateSpeed;
            Pitch += deltaY * RotateSpeed;

            Pitch = MathUtil.Clamp(Pitch, -MathUtil.PiOverTwo + 0.01f, MathUtil.PiOverTwo - 0.01f);

            UpdateCameraPosition();
        }

        public void Zoom(float delta)
        {
            Distance = MathUtil.Clamp(Distance - delta * ZoomSpeed, MinDistance, MaxDistance);
            UpdateCameraPosition();
        }

        public void Pan(float deltaX, float deltaY)
        {
            // Calculate right and up vectors
            Vector3 forward = Vector3.Normalize(Target - Position);
            Vector3 right = Vector3.Normalize(Vector3.Cross(Vector3.UnitY, forward));
            Vector3 up = Vector3.Normalize(Vector3.Cross(forward, right));

            Target += -right * deltaX * PanSpeed + up * deltaY * PanSpeed;
            UpdateCameraPosition();
        }

        public void Move(Vector3 direction, float deltaTime)
        {
            Vector3 forward = Vector3.Normalize(Target - Position);
            Vector3 right = Vector3.Normalize(Vector3.Cross(Vector3.UnitY, forward));

            Vector3 moveVec = (forward * direction.Z + right * direction.X + Vector3.UnitY * direction.Y);
            Target += moveVec * MoveSpeed * deltaTime;
            UpdateCameraPosition();
        }

        public void Reset()
        {
            Target = Vector3.Zero;
            Distance = 10f;
            Yaw = MathUtil.PiOverTwo;
            Pitch = 0.0f;
            UpdateCameraPosition();
        }
    }


}


