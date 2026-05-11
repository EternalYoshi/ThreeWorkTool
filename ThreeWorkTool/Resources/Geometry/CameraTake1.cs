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
        public float Yaw { get; set; }   = -90f; //This is the lateral angle/
        public float Pitch { get; set; } = -20f;

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

            //For WASD key support and movmement.
            if (HeldKeys.Contains(Keys.W))
            {
                Position += Forward * MoveSpeed * deltaTime;
            }
            if (HeldKeys.Contains(Keys.S))
            {
                Position -= Forward * MoveSpeed * deltaTime;
            }
            if (HeldKeys.Contains(Keys.A))
            {
                Position -= Right * MoveSpeed * deltaTime;
            }
            if (HeldKeys.Contains(Keys.D))
            {
                Position += Right * MoveSpeed * deltaTime;
            }

            //For the Arrow Keys rotating the camera.
            if (HeldKeys.Contains(Keys.Left)) Yaw -= RotateSpeed * deltaTime;
            if (HeldKeys.Contains(Keys.Right)) Yaw += RotateSpeed * deltaTime;
            if (HeldKeys.Contains(Keys.Up)) Pitch += RotateSpeed * deltaTime;
            if (HeldKeys.Contains(Keys.Down)) Pitch -= RotateSpeed * deltaTime;

            //Up and Down Movement.
            if (HeldKeys.Contains(Keys.Q)) Position += Vector3.UnitY * MoveSpeed * deltaTime;
            if (HeldKeys.Contains(Keys.E)) Position -= Vector3.UnitY * MoveSpeed * deltaTime;

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


    }


}


