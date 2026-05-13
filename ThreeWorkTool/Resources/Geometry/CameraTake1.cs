using System;
using System.Collections.Generic;
using System.Linq;
//using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
//using SharpDX;
//using SharpDX.Mathematics.Interop;

namespace ThreeWorkTool.Resources.Geometry
{
    public class CameraTake1
    {
        public Vector3 Position = new Vector3(0, 25, 15);
        public float Yaw { get; set; } = -90f; //This is the lateral angle/
        public float Pitch { get; set; } = -20f;
        public float SpeedMultiplier = 1.0f;
        public float MouseSensitivity { get; set; } = .10f;
        public float PanSensitivity { get; set; } = 0.35f;

        public float MoveSpeed { get; set; } = 100f;
        public float ZoomSpeed { get; set; } = 20f;
        public float RotateSpeed { get; set; } = 60f;
        public float PanSpeed { get; set; } = 100f;

        //Directional Vectors. Meant to be automatically updated.
        public Vector3 Forward { get; private set; }
        public Vector3 Right { get; private set; }
        public Vector3 Up => Vector3.UnitY;

        //Boundaries for Camera movement. Unsure if I will use these.
        public float MinDistance = 1f;
        public float MaxDistance = 10000f;

        public CameraTake1()
        {
            VectorUpdate();
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + Forward, Up);
        }

        public void UpdateCameraPosition(HashSet<Keys> HeldKeys, float deltaTime)
        {

            //Checks for Shift Key.
            if (HeldKeys.Contains(Keys.ShiftKey))
            {
                SpeedMultiplier = 7.0f;
            }
            else
            {
                SpeedMultiplier = 1.0f;
            }


            //For WASD key support and movmement.
            if (HeldKeys.Contains(Keys.W))
            {
                Position += Forward * MoveSpeed * deltaTime * SpeedMultiplier;
            }
            if (HeldKeys.Contains(Keys.S))
            {
                Position -= Forward * MoveSpeed * deltaTime * SpeedMultiplier;
            }
            if (HeldKeys.Contains(Keys.A))
            {
                Position -= Right * MoveSpeed * deltaTime * SpeedMultiplier;
            }
            if (HeldKeys.Contains(Keys.D))
            {
                Position += Right * MoveSpeed * deltaTime * SpeedMultiplier;
            }

            //For the Arrow Keys rotating the camera.
            if (HeldKeys.Contains(Keys.Left))
            {
                Yaw -= RotateSpeed * deltaTime * SpeedMultiplier;
            }
            if (HeldKeys.Contains(Keys.Right))
            {
                Yaw += RotateSpeed * deltaTime * SpeedMultiplier;
            }
            if (HeldKeys.Contains(Keys.Up))
            {
                Pitch += RotateSpeed * deltaTime * SpeedMultiplier;
            }
            if (HeldKeys.Contains(Keys.Down))
            {
                Pitch -= RotateSpeed * deltaTime * SpeedMultiplier;
            }

            //Up and Down Movement.
            if (HeldKeys.Contains(Keys.Q))
            {
                Position += Vector3.UnitY * MoveSpeed * deltaTime * SpeedMultiplier;
            }
            if (HeldKeys.Contains(Keys.E))
            {
                Position -= Vector3.UnitY * MoveSpeed * deltaTime * SpeedMultiplier;
            }

            //// Spherical to Cartesian
            //float x = Distance * (float)(Math.Cos(Pitch) * Math.Sin(Yaw));
            //float y = Distance * (float)(Math.Sin(Pitch));
            //float z = Distance * (float)(Math.Cos(Pitch) * Math.Cos(Yaw));
            //Position = Target + new Vector3(x, y, z);

            //Clamps pitch to the Camera to prevent camera flipping.
            Pitch = Math.Max(-89f, Math.Min(89f, Pitch));

            VectorUpdate();
        }

        public void Zoom(float delta)
        {
            Position += Forward * delta * ZoomSpeed;
        }

        public void VectorUpdate()
        {
            float yawRad = MathHelper.DegreesToRadians(Yaw);
            float pitchRad = MathHelper.DegreesToRadians(Pitch);

            Forward = Vector3.Normalize(new Vector3(
                (float)(Math.Cos(pitchRad) * Math.Cos(yawRad)),
                (float)Math.Sin(pitchRad),
                (float)(Math.Cos(pitchRad) * Math.Sin(yawRad))
            ));

            Right = Vector3.Normalize(Vector3.Cross(Forward, Vector3.UnitY));
        }

        public void Rotation(float Rx, float Ry)
        {
            Yaw += Rx * MouseSensitivity;
            //Ry is inverted so moving the mouse up will also up the pitch.
            Pitch -= Ry * MouseSensitivity;
            Pitch = Math.Max(-89.0f, Math.Min(89.0f, Pitch));
            VectorUpdate();
        }

        public void Pan(float Px, float Py)
        {

            Position -= Right * Px * PanSensitivity;
            Position += Vector3.UnitY * Py * PanSensitivity;

        }

    }


}


