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
using ThreeWorkTool.Resources.Utility;
using System.Windows;

namespace ThreeWorkTool.Resources.Wrappers.ModelNodes
{
    public class ModelPrimitiveEntry : DefaultWrapper
    {
        //Primitive Joint Link = Envelopes
        public int ID, VerticeCount, VertexFlags, VertexStartIndex, VertexBufferOffset, IndexBufferOffset,
            IndexCount, IndexStartIndex, BoneMapStartIndex, EnvelopeCount, MinVertexindex, MaxVertexIndex, Unknown2C, 
             p, VertexStride, reIndexBufferOffset;

        public short Flags;

        public ushort RenderMode;

        public int PrimOffset { get; set; }

        public long EnvelopesPtr;//AKA Primitive Joint Links.

        [Category("Indices")]
        public ushort GroupID { get; set; }
        [Category("Indices")]
        public ushort MaterialIndex { get; set; }
        [Category("Indices")]
        public byte LODIndex { get; set; }

        [Category("UVs"), ReadOnlyAttribute(true)]
        public List<Vector3> UVPrimary { get; set; }

        [Category("UVs"), ReadOnlyAttribute(true)]
        public List<Vector3> UVSecondary { get; set; }

        [Category("UVs"), ReadOnlyAttribute(true)]
        public List<Vector3> UVUnique { get; set; }

        [Category("UVs"), ReadOnlyAttribute(true)]
        public List<Vector3> UVExtend { get; set; }

        public struct Vertex
        {
            public Vector3 Coordinate;
            public List<float> Weights;
            public Vector4 Normals;
            public List<int> Joints;
        }

        public struct MTShader
        {
            public int Index;
            public string ShaderObjectHash;
        }
        public MTShader Shaders;

        public List<short> IndexBuffer { get; set; }

        public List<ModelEnvelopeEntry> PJLs;

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
        public short PrimFlag
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

        [Category("Primitive"), ReadOnlyAttribute(false)]
        public ushort RenderFlag
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

        [Category("Primitive"), ReadOnlyAttribute(false)]
        [DisplayName("MT Shader Type")]
        [DefaultValue("")]
        [TypeConverter(typeof(FormatStringConverter))]
        public string MTShaderType
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

        //Gets the ShaderList from the main form.
        public class FormatStringConverter : StringConverter
        {
            public override Boolean GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }
            public override Boolean GetStandardValuesExclusive(ITypeDescriptorContext context) { return true; }
            public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                List<String> list = new List<String>();
                list = GetShaderMatList(list);
                return new StandardValuesCollection(list);
            }

            private List<string> GetShaderMatList(List<string> sList)
            {

                FrmMainThree frmthree = System.Windows.Forms.Application.OpenForms.OfType<FrmMainThree>().FirstOrDefault();
                sList = frmthree.ShaderList;

                return sList;
            }
        }

        //[Category("Primitive"), ReadOnlyAttribute(true)]
        //public Indices IndiceList
        //{
        //    get { return Indice; }
        //}

        [Category("Primitive"), ReadOnlyAttribute(true)]
        public int PrimitiveJointLinkTotal
        {

            get
            {
                return EnvelopeCount;
            }
            set
            {
                EnvelopeCount = value;
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
                return EnvelopesPtr;
            }
            set
            {
                EnvelopesPtr = value;
            }
        }


        #endregion

    }
}
