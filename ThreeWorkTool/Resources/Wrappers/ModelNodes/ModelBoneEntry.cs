using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ThreeWorkTool.Resources.Archives;
using ThreeWorkTool.Resources.Wrappers;

namespace ThreeWorkTool.Resources.Wrappers.ModelNodes
{
    public class ModelBoneEntry : DefaultWrapper
    {
        public int ID;
        public string JointName;
        public int Parent;
        public int SymmetryIndex;
        public int Field3;
        public float Field4;
        public float Length;
        public Vector3 Offset;
        public struct LocalMatrix
        {
            public Vector4 RowA;
            public Vector4 RowB;
            public Vector4 RowC;
            public Vector4 RowD;
        }
        public struct InvBindMatrix
        {
            public Vector4 RowA;
            public Vector4 RowB;
            public Vector4 RowC;
            public Vector4 RowD;
        }
        public BoundingSphere SphereBound;
        public struct BoundingSphere
        {
            public Vector3 Center;
            public float Radius;
        }

        public PrimitiveJointLinks JointLinks;        
        public struct PrimitiveJointLinks
        {
            public int curJointIndex;
            public List<PrimitiveJLink> Links;
        }

        public struct PrimitiveJLink
        {
            public int JointIndex;
            public int Field04;
            public int Field08;
            public int Field0C;
            public Vector4 Vec10;
            public Vector4 Vec20;
            public Vector4 Vec30;
            public Matrix4x4 Mtx40;
            public Vector4 Vec80;



        }

        public ModelBoneEntry FillModelBoneEntry(ModelBoneEntry MBoneE, ModelEntry ParentMod, BinaryReader bnr, int OffsetToStart, int ID)
        {
            bnr.BaseStream.Position = OffsetToStart;
            MBoneE.ID = Convert.ToInt32(bnr.ReadByte());
            MBoneE.Parent = Convert.ToInt32(bnr.ReadByte());
            MBoneE.SymmetryIndex = Convert.ToInt32(bnr.ReadByte());
            MBoneE.Field3 = Convert.ToInt32(bnr.ReadByte());
            MBoneE.Field4 = bnr.ReadSingle();
            MBoneE.Length = bnr.ReadSingle();
            MBoneE.Offset.X = bnr.ReadSingle();
            MBoneE.Offset.Y = bnr.ReadSingle();
            MBoneE.Offset.Z = bnr.ReadSingle();

            MBoneE.JointLinks.Links = new List<PrimitiveJLink>();

            ///*
            //Primitive Joint Links.
            for(int i = 0; i < ParentMod.PrimitiveJointLinkCount; i++)
            {
                bnr.BaseStream.Position = ((ParentMod.PrimitveOffset + ParentMod.PrimitiveCount * 56) + i * 144);
                MBoneE.JointLinks.curJointIndex = Convert.ToInt32(bnr.BaseStream.Position);
                if (MBoneE.JointLinks.curJointIndex == MBoneE.ID)
                {


                    
                    PrimitiveJLink PLink = new PrimitiveJLink();
                    PLink.JointIndex = bnr.ReadInt32();
                    PLink.Field04 = bnr.ReadInt32();
                    PLink.Field08 = bnr.ReadInt32();
                    PLink.Field0C = bnr.ReadInt32();
                    PLink.Vec10.X = bnr.ReadSingle();
                    PLink.Vec10.Y = bnr.ReadSingle();
                    PLink.Vec10.Z = bnr.ReadSingle();
                    PLink.Vec10.W = bnr.ReadSingle();
                    PLink.Vec20.X = bnr.ReadSingle();
                    PLink.Vec20.Y = bnr.ReadSingle();
                    PLink.Vec20.Z = bnr.ReadSingle();
                    PLink.Vec20.W = bnr.ReadSingle();
                    PLink.Vec30.X = bnr.ReadSingle();
                    PLink.Vec30.Y = bnr.ReadSingle();
                    PLink.Vec30.Z = bnr.ReadSingle();
                    PLink.Vec30.W = bnr.ReadSingle();

                    PLink.Mtx40.M11 = bnr.ReadSingle();
                    PLink.Mtx40.M12 = bnr.ReadSingle();
                    PLink.Mtx40.M13 = bnr.ReadSingle();
                    PLink.Mtx40.M14 = bnr.ReadSingle();

                    PLink.Mtx40.M21 = bnr.ReadSingle();
                    PLink.Mtx40.M22 = bnr.ReadSingle();
                    PLink.Mtx40.M23 = bnr.ReadSingle();
                    PLink.Mtx40.M24 = bnr.ReadSingle();

                    PLink.Mtx40.M31 = bnr.ReadSingle();
                    PLink.Mtx40.M32 = bnr.ReadSingle();
                    PLink.Mtx40.M33 = bnr.ReadSingle();
                    PLink.Mtx40.M34 = bnr.ReadSingle();

                    PLink.Mtx40.M41 = bnr.ReadSingle();
                    PLink.Mtx40.M42 = bnr.ReadSingle();
                    PLink.Mtx40.M43 = bnr.ReadSingle();
                    PLink.Mtx40.M44 = bnr.ReadSingle();

                    PLink.Vec80.X = bnr.ReadSingle();
                    PLink.Vec80.Y = bnr.ReadSingle();
                    PLink.Vec80.Z = bnr.ReadSingle();
                    PLink.Vec80.W = bnr.ReadSingle();
                    
                }
            }

            //*/

            return MBoneE;

        }

        #region Bone Properties

        [Category("Bone Data"), ReadOnlyAttribute(true)]
        public int BoneID
        {
            get
            {
                return ID;
            }
            set
            {
                ID = value;
            }
        }

        [Category("Bone Data"), ReadOnlyAttribute(true)]
        public int ParentBone
        {
            get
            {
                return Parent;
            }
            set
            {
                Parent = value;
            }
        }

        [Category("Bone Data"), ReadOnlyAttribute(true)]
        public int BoneSymmetryIndex
        {
            get
            {
                return SymmetryIndex;
            }
            set
            {
                SymmetryIndex = value;
            }
        }

        [Category("Bone Data"), ReadOnlyAttribute(true)]
        public float BoneLength
        {
            get
            {
                return Length;
            }
            set
            {
                Length = value;
            }
        }

        [Category("Bone Data"), ReadOnlyAttribute(true)]
        public Vector3 BoneOffsets
        {
            get
            {
                return Offset;
            }
            set
            {
                Offset = value;
            }
        }

        #endregion

    }

}
