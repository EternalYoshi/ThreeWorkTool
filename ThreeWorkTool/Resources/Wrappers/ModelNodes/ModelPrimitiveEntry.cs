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
    public class ModelPrimitiveEntry : DefaultWrapper
    {
        public int ID, Flags, VerticeCount, VertexFlags, RenderMode, VertexStartIndex, VertexBufferOffset, IndexBufferOffset, 
            IndexCount, IndexStartIndex, BoneMapStartIndex, PrimitiveJointLinkCount, MinVertexindex, MaxVertexIndex, Unknown2C,
             p, VertexStride, reIndexBufferOffset;

        public long PrimitiveJointLinkPtr;

        public struct Indices
        {
            public int GroupID, MaterialIndex, LODIndex;            
        }
        public Indices Indice;

        public struct MTShader
        {
            public int Index;
            public string ShaderObjectHash;
        }
        public MTShader Shaders;

        public byte[] IndexBuffer;

        public List<ModelPrimitiveJointLinkEntry> PJLs;

        public struct VertexShaderInputLayout
        {
            public int p;
            public List<int> Joints, Normal;
            public struct Postiion
            {
                public float PX, PY, PZ;
            }
            public float Weight;
        }

        #region Model Primitive Entry Properties
        [Category("Primitive"), ReadOnlyAttribute(true)]
        public int PrimitiveID
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

        [Category("Primitive"), ReadOnlyAttribute(true)]
        public int Vertices
        {

            get
            {
                return VerticeCount;
            }
            set
            {
                VerticeCount = value;
            }
        }

        [Category("Primitive"), ReadOnlyAttribute(true)]
        public int PrimFlag
        {

            get
            {
                return Flags;
            }
            set
            {
                Flags = value;
            }
        }

        [Category("Primitive"), ReadOnlyAttribute(true)]
        public int RenderFlag
        {

            get
            {
                return RenderMode;
            }
            set
            {
                RenderMode = value;
            }
        }

        [Category("Primitive"), ReadOnlyAttribute(true)]
        public int NumberOfIndices
        {

            get
            {
                return IndexCount;
            }
            set
            {
                IndexCount = value;
            }
        }

        [Category("Primitive"), ReadOnlyAttribute(true)]
        public string MTSHaderType
        {

            get
            {
                return Shaders.ShaderObjectHash;
            }
            set
            {
                Shaders.ShaderObjectHash = value;
            }
        }

        [Category("Primitive"), ReadOnlyAttribute(true)]
        public Indices IndiceList
        {

            get
            {
                return Indice;
            }
            set
            {
                Indice = value;
            }
        }


        [Category("Primitive"), ReadOnlyAttribute(true)]
        public int PrimitiveJointLinkTotal
        {

            get
            {
                return PrimitiveJointLinkCount;
            }
            set
            {
                PrimitiveJointLinkCount = value;
            }
        }

        [Category("Primitive"), ReadOnlyAttribute(true)]
        public int VertStride
        {

            get
            {
                return VertexStride;
            }
            set
            {
                VertexStride = value;
            }
        }

        [Category("Primitive"), ReadOnlyAttribute(true)]
        public int MinimumVertexIndex
        {

            get
            {
                return MinVertexindex;
            }
            set
            {
                MinVertexindex = value;
            }
        }

        [Category("Primitive"), ReadOnlyAttribute(true)]
        public int MaximumVertexIndex
        {

            get
            {
                return MaxVertexIndex;
            }
            set
            {
                MaxVertexIndex = value;
            }
        }

        [Category("Primitive"), ReadOnlyAttribute(true)]
        public long PrimJointLinkPointer 
        {

            get
            {
                return PrimitiveJointLinkPtr;
            }
            set
            {
                PrimitiveJointLinkPtr = value;
            }
        }

        #endregion

    }
}
