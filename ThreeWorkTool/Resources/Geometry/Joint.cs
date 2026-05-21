using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace ThreeWorkTool.Resources.Geometry
{
    public class Joint
    {
        public string Name;
        public int ID;

        //For the Hiearchy.
        public Joint Parent;
        public List<Joint> Children = new List<Joint>();

        //Transformations and Matrices.
        public Vector3 Location;
        public Quaternion Rotation;
        public Vector3 Scale;

        public Matrix4 LocalMatrix;
        public Matrix4 WorldMatrix;

        //Marvel 3 bones have this. Allegedly this is the Bind Pose inverted, used for skinning. I'm unsure how it's calculated.
        public Matrix4 InverseBindMatrix;

        //To link a visual handle to a bone.
        public Sphere Handle;

        public void Load()
        {
            Handle = new Sphere();
            Handle.Radius = 1.0f;
            Handle.Stacks = 10;
            Handle.Slices = 10;
            Handle.Load();
        }

        //This builds the Local transformation Matrix.
        public void CalculateLocalMatrix()
        {
            LocalMatrix =
                Matrix4.CreateScale(Scale) *
                Matrix4.CreateFromQuaternion(Rotation) *
                Matrix4.CreateTranslation(Location);
        }

        //Disposes the handle when done.
        public void Dispose()
        {
            Handle?.Dispose();
        }

    }
}
