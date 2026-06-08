using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeWorkTool.Resources.Utility;
using ThreeWorkTool.Resources.Wrappers;
using ThreeWorkTool.Resources.Wrappers.ModelNodes;

namespace ThreeWorkTool.Resources.Geometry
{
    public class Skeleton : IDisposable
    {
        public Joint Root;

        //Intended to be an indexed list for the GPU.
        public List<Joint> Joints = new List<Joint>();

        private BoneRectangle Connector;

        public void Load()
        {
            Connector = new BoneRectangle();
            Connector.Load();

            //Loads the bone handles.
            foreach (var joint in Joints)
            {
                joint.Load();
            }

            //Caputres the bind pose using the identity matrix.
            UpdateWorldMatrices(Root, Matrix4.Identity);
            CaptureBindPose(Root);
        }

        //This is intended to be call every frame before rendering.
        public void Update()
        {
            UpdateWorldMatrices(Root, Matrix4.Identity);
        }

        public void UpdateWorldMatrices(Joint joint, Matrix4 parentWorld)
        {
            //This builds the Local transformation Matrix.
            //joint.CalculateLocalMatrix();

            //Parent joint stuff.
            joint.WorldMatrix = joint.LocalMatrix * parentWorld;

            foreach (var child in joint.Children)
                UpdateWorldMatrices(child, joint.WorldMatrix);
        }

        //For Rendering.
        public void Render(Vector3 cameraPos, Matrix4 view, Matrix4 projection, Skeleton skeleton, int SelectedJointIndex)
        {
            //So we draw our joint connectors first so our joint handles are always rendered on top.
            foreach (var joint in skeleton.Joints)
            {
                if (joint.Parent == null) continue;

                Vector3 start = joint.Parent.WorldMatrix.ExtractTranslation();
                Vector3 end = joint.WorldMatrix.ExtractTranslation();

                Connector.Render(start, end, cameraPos, view, projection);
            }

            //And here we draw the joint handles themselves.
            foreach (var joint in skeleton.Joints)
            {
                Matrix4 model = joint.WorldMatrix;
                joint.Handle.Render(model, view, projection, SelectedJointIndex);
            }
        }
       
        //This gets the inverse joint matrix. Meant to only be called once.
        public void CaptureBindPose(Joint joint)
        {
            joint.InverseBindMatrix = Matrix4.Invert(joint.WorldMatrix);
            foreach (var child in joint.Children)
                CaptureBindPose(child);
        }

        //For later use intended for shaders & mesh rigging.
        public Matrix4[] GetSkinningMatrices()
        {
            var result = new Matrix4[Joints.Count];
            for (int i = 0; i < Joints.Count; i++)
                result[i] = Joints[i].InverseBindMatrix * Joints[i].WorldMatrix;
            return result;
        }

        public void Dispose()
        {
            Connector?.Dispose();
            foreach (var joints in Joints)
                joints.Dispose();
        }

        private void DecomposeMatrix(Matrix4 m, out OpenTK.Vector3 position, out OpenTK.Quaternion rotation, out OpenTK.Vector3 scale)
        {
            position = m.ExtractTranslation();
            scale = m.ExtractScale();

            var rotMatrix = new Matrix4(
                m.Column0 / (scale.X == 0 ? 1 : scale.X),
                m.Column1 / (scale.Y == 0 ? 1 : scale.Y),
                m.Column2 / (scale.Z == 0 ? 1 : scale.Z),
                m.Column3
            );

            rotation = rotMatrix.ExtractRotation();
        }

        //This builds the skeleton based on the specified variables.
        public Skeleton BuildSkeletonFromModelEntry(ModelEntry model)
        {
            Skeleton skeleton = new Skeleton();

            //This is how I'm iterating through the list of bones from Marvel 3's model file. I have to do it twice unfortunately.
            for (int v = 0; v < model.Bones.Count; v++)
            {

                Joint joint = new Joint
                {
                    ID = model.Bones[v].ID,
                    Name = model.Bones[v].JointName
                };

                //Need to convert the Marvel 3 matrices to OpenTK matrices.
                Matrix4 LocalMatrix = ByteUtilitarian.FromSystemNumericsMatrixToOpenTKMatrix(model.Bones[v].LocalMatrix);
                Matrix4 InvBindMatrix = ByteUtilitarian.FromSystemNumericsMatrixToOpenTKMatrix(model.Bones[v].InvBindMatrix);

                //DecomposeMatrix(LocalMatrix, out joint.Location,out joint.Rotation,out joint.Scale);
                joint.LocalMatrix = LocalMatrix;
                joint.InverseBindMatrix = InvBindMatrix;

                skeleton.Joints.Add(joint);

            }

            for (int w = 0; w < model.Bones.Count; w++)
            {
                int ParentIndex = model.Bones[w].Parent;

                if (ParentIndex != 255 && ParentIndex != -1)
                {
                    Joint Child = skeleton.Joints[w];
                    Joint Parent = skeleton.Joints[ParentIndex];
                    Child.Parent = Parent;
                    Parent.Children.Add(Child);
                }
                else
                {
                    skeleton.Root = skeleton.Joints[w];
                }


            }


            skeleton.Load();
            return skeleton;
        }


    }




}

