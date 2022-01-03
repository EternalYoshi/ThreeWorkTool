using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Archives;
using ThreeWorkTool.Resources.Utility;
using ThreeWorkTool.Resources.Wrappers.ModelNodes;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class ModelEntry : DefaultWrapper
    {
        public string Magic;
        public string Constant;
        public byte[] WTemp;
        public int Version;
        public int JointCount;
        public int PrimitiveCount;
        public int MaterialCount;
        public int VertexCount;
        public int IndexCount;
        public int PolygonCount;
        public int VertexBufferSize;
        public int VertexBufferSecondSize;
        public long GroupCount;
        public int JointsOffset;
        public int GroupOffset;
        public int MaterialsOffset;
        public int PrimitveOffset;
        public int VertexBufferOffset;
        public int IndexBufferOffset;
        public int ExtraDataOffset;
        public Vector3 BoundingSphere;
        public float BoundingSphereRadius;
        public Vector4 BoundingBoxMin;
        public Vector4 BoundingBoxMax;
        public new static string TypeHash = "58A15856";
        public int Field90;
        public int Field94;
        public int Field98;
        public int Field9C;
        public int PrimitiveJointLinkCount;
        public List<string> MaterialNames;
        public List<ModelBoneEntry> Bones;

        public static ModelEntry FillModelEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
        {
            ModelEntry modentry = new ModelEntry();
            List<byte> BTemp = new List<byte>();

            FillEntry(filename, subnames, tree, br, c, ID, modentry, filetype);

            //Decompression Time.
            modentry.UncompressedData = ZlibStream.UncompressBuffer(modentry.CompressedData);

            //Type Specific Work Here.

            using (MemoryStream LmtStream = new MemoryStream(modentry.UncompressedData))
            {
                using (BinaryReader bnr = new BinaryReader(LmtStream))
                {

                    //Header Stuff. So huge.
                    modentry.Magic = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), modentry.Magic);
                    modentry.Version = bnr.ReadInt16();
                    modentry.JointCount = bnr.ReadInt16();
                    modentry.PrimitiveCount = bnr.ReadInt16();
                    modentry.MaterialCount = bnr.ReadInt16();
                    modentry.VertexCount = bnr.ReadInt32();
                    modentry.IndexCount = bnr.ReadInt32();
                    modentry.PolygonCount = bnr.ReadInt32();
                    modentry.VertexBufferSizeA = bnr.ReadInt32();
                    modentry.VertexBufferSizeB = bnr.ReadInt32();
                    modentry.GroupCount = bnr.ReadInt64();
                    modentry.JointsOffset = Convert.ToInt32(bnr.ReadInt64());
                    modentry.GroupOffset = Convert.ToInt32(bnr.ReadInt64());
                    modentry.MaterialsOffset = Convert.ToInt32(bnr.ReadInt64());
                    modentry.PrimitveOffset = Convert.ToInt32(bnr.ReadInt64());
                    modentry.VertexBufferOffset = Convert.ToInt32(bnr.ReadInt64());
                    modentry.IndexBufferOffset = Convert.ToInt32(bnr.ReadInt64());
                    modentry.ExtraDataOffset = Convert.ToInt32(bnr.ReadInt64());

                    //Bounding Sphere.
                    modentry.BoundingSphere = new Vector3();
                    modentry.BoundingSphere.X = bnr.ReadSingle();
                    modentry.BoundingSphere.Y = bnr.ReadSingle();
                    modentry.BoundingSphere.Z = bnr.ReadSingle();
                    modentry.BoundingSphereRadius = bnr.ReadSingle();

                    //Bounding Boxes.
                    modentry.BoundingBoxMin = new Vector4();
                    modentry.BoundingBoxMax = new Vector4();

                    modentry.BoundingBoxMin.X = bnr.ReadSingle();
                    modentry.BoundingBoxMin.Y = bnr.ReadSingle();
                    modentry.BoundingBoxMin.Z = bnr.ReadSingle();
                    modentry.BoundingBoxMin.W = bnr.ReadSingle();

                    modentry.BoundingBoxMax.X = bnr.ReadSingle();
                    modentry.BoundingBoxMax.Y = bnr.ReadSingle();
                    modentry.BoundingBoxMax.Z = bnr.ReadSingle();
                    modentry.BoundingBoxMax.W = bnr.ReadSingle();

                    modentry.Field90 = bnr.ReadInt32();
                    modentry.Field94 = bnr.ReadInt32();
                    modentry.Field98 = bnr.ReadInt32();
                    modentry.Field9C = bnr.ReadInt32();
                    modentry.PrimitiveJointLinkCount = bnr.ReadInt32();

                    //Material Names.
                    bnr.BaseStream.Position = modentry.MaterialsOffset;
                    modentry.MaterialNames = new List<string>();
                    string Stringtemp;
                    for (int m = 0; m < modentry.MaterialCount; m++)
                    {
                        Stringtemp = Encoding.ASCII.GetString(bnr.ReadBytes(128)).Trim('\0');
                        modentry.MaterialNames.Add(Stringtemp);
                    }

                }
            }

            return modentry;
        }

        public static ModelEntry ReplaceModelEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            ModelEntry mdlentry = new ModelEntry();
            ModelEntry oldentry = new ModelEntry();

            tree.BeginUpdate();

            ReplaceEntry(tree, node, filename, mdlentry, oldentry);

            mdlentry.DecompressedFileLength = mdlentry.UncompressedData.Length;
            mdlentry.CompressedFileLength = mdlentry.CompressedData.Length;

            return node.entryfile as ModelEntry;
        }


        #region Model Entry Properties
        [Category("Filename"), ReadOnlyAttribute(true)]
        public string FileName
        {

            get
            {
                return TrueName;
            }
            set
            {
                TrueName = value;
            }
        }

        [Category("Model Data"), ReadOnlyAttribute(true)]
        public int CompressedFileLength
        {

            get
            {
                return CSize;
            }
            set
            {
                CSize = value;
            }
        }

        [Category("Model Data"), ReadOnlyAttribute(true)]
        public int DecompressedFileLength
        {

            get
            {
                return DSize;
            }
            set
            {
                DSize = value;
            }
        }

        [Category("Filename"), ReadOnlyAttribute(true)]
        public string FileType
        {

            get
            {
                return FileExt;
            }
            set
            {
                FileExt = value;
            }
        }

        [Category("Model Data"), ReadOnlyAttribute(true)]
        public int FileVersion
        {
            get
            {
                return Version;
            }
            set
            {
                Version = value;
            }
        }

        [Category("Model Data"), ReadOnlyAttribute(true)]
        public int JointTotal
        {
            get
            {
                return JointCount;
            }
            set
            {
                JointCount = value;
            }
        }

        [Category("Model Data"), ReadOnlyAttribute(true)]
        public int PrimitiveTotal
        {
            get
            {
                return PrimitiveCount;
            }
            set
            {
                PrimitiveCount = value;
            }
        }

        [Category("Model Data"), ReadOnlyAttribute(true)]
        public int MaterialTotal
        {
            get
            {
                return MaterialCount;
            }
            set
            {
                MaterialCount = value;
            }
        }

        [Category("Model Data"), ReadOnlyAttribute(true)]
        public int VertexTotal
        {
            get
            {
                return VertexCount;
            }
            set
            {
                VertexCount = value;
            }
        }

        [Category("Model Data"), ReadOnlyAttribute(true)]
        public int IndexTotal
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

        [Category("Model Data"), ReadOnlyAttribute(true)]
        public int PolygonTotal
        {
            get
            {
                return PolygonCount;
            }
            set
            {
                PolygonCount = value;
            }
        }

        [Category("Model Data"), ReadOnlyAttribute(true)]
        public long GroupTotal
        {
            get
            {
                return GroupCount;
            }
            set
            {
                GroupCount = value;
            }
        }

        [Category("Model Data"), ReadOnlyAttribute(true)]
        public int VertexBufferSizeA
        {
            get
            {
                return VertexBufferSize;
            }
            set
            {
                VertexBufferSize = value;
            }
        }

        [Category("Model Data"), ReadOnlyAttribute(true)]
        public int VertexBufferSizeB
        {
            get
            {
                return VertexBufferSecondSize;
            }
            set
            {
                VertexBufferSecondSize = value;
            }
        }

        [Category("Model Data"), ReadOnlyAttribute(true)]
        public Vector3 BoundSphere
        {
            get
            {
                return BoundingSphere;
            }
            set
            {
                BoundingSphere = value;
            }
        }

        [Category("Model Data"), ReadOnlyAttribute(true)]
        public Vector4 BoxMin
        {
            get
            {
                return BoundingBoxMin;
            }
            set
            {
                BoundingBoxMin = value;
            }
        }

        [Category("Model Data"), ReadOnlyAttribute(true)]
        public Vector4 BoxMax
        {
            get
            {
                return BoundingBoxMax;
            }
            set
            {
                BoundingBoxMax = value;
            }
        }

        [Category("Model Data"), ReadOnlyAttribute(true)]
        public float BoundSphereRadius
        {
            get
            {
                return BoundingSphereRadius;
            }
            set
            {
                BoundingSphereRadius = value;
            }
        }

        [Category("Model Data"), ReadOnlyAttribute(true)]
        public List<string> Materials
        {
            get
            {
                return MaterialNames;
            }
            set
            {
                MaterialNames = value;
            }
        }


        #endregion

    }
}
