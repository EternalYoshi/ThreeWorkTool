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
        public Vector4 LocalMatrix;
        public Vector4 InvBindMatrix;
        public BoundingSphere SphereBound;
        public struct BoundingSphere
        {
            public Vector3 Center;
            public float Radius;
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
            //MBoneE.SphereBound = new BoundingSphere();



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
