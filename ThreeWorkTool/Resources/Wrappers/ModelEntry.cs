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
        public int BoneCount;
        public int BoneLocalMatrixOffset;
        public int BoneInvBindMatrixOffset;
        public int PrimitiveCount;
        public int MaterialCount;
        public int VertexCount;
        public int IndexCount;
        public int PolygonCount;
        public int VertexBufferSize;
        public int VertexBufferSecondSize;
        public long GroupCount;
        public int BonesOffset;
        public int GroupOffset;
        public int MaterialsOffset;
        public int PrimitveOffset;
        public int VertexBufferOffset;
        public int IndexBufferOffset;
        public int ExtraDataOffset;
        public Vector3 BoundingSphere;
        public float BoundingSphereRadius;
        public Vector4 BoundingBoxCenter;
        public Vector4 BoundingBoxRadius;
        public new static string TypeHash = "58A15856";
        public int Field90;
        public int Field94;
        public int Field98;
        public int Field9C;
        public int PrimitiveJointLinkCount;
        public List<string> MaterialNames;
        public List<ModelBoneEntry> Bones;
        public List<ModelGroupEntry> Groups;
        public List<ModelPrimitiveEntry> Primitives;
        public List<ModelPrimitiveJointLinkEntry> PLJs;
        public byte[] VertexBuffer;
        public byte[] IndexBuffer;
        public byte[] ExtraDataBuffer;

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
                    BuildModelEntry(bnr, modentry);
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
            mdlentry.FileName = mdlentry.TrueName;

            mdlentry.DecompressedFileLength = mdlentry.UncompressedData.Length;
            mdlentry.CompressedFileLength = mdlentry.CompressedData.Length;

            //Type Specific Work Here.
            using (MemoryStream LmtStream = new MemoryStream(mdlentry.UncompressedData))
            {
                using (BinaryReader bnr = new BinaryReader(LmtStream))
                {
                    BuildModelEntry(bnr, mdlentry);
                }
            }

            return node.entryfile as ModelEntry;
        }

        public static ModelEntry InsertModelEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {

            ModelEntry model = new ModelEntry();

            InsertEntry(tree, node, filename, model);

            //Decompression Time.
            model.UncompressedData = ZlibStream.UncompressBuffer(model.CompressedData);

            try
            {
                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
                {
                    BuildModelEntry(bnr, model);
                }
            }
            catch (Exception ex)
            {
                string ProperPath = "";
                ProperPath = Globals.ToolPath + "Log.txt"; using (StreamWriter sw = File.AppendText(ProperPath))
                {
                    sw.WriteLine("Caught an exception using the BinaryReader. Here's the details:\n" + ex);
                }
            }

            return model;

        }

        public static ModelEntry BuildModelEntry(BinaryReader bnr, ModelEntry modentry)
        {

            //Header Stuff. So huge.
            modentry.Magic = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), modentry.Magic);
            modentry.Version = bnr.ReadInt16();
            modentry.BoneCount = bnr.ReadInt16();
            modentry.PrimitiveCount = bnr.ReadInt16();
            modentry.MaterialCount = bnr.ReadInt16();
            modentry.VertexCount = bnr.ReadInt32();
            modentry.IndexCount = bnr.ReadInt32();
            modentry.PolygonCount = bnr.ReadInt32();
            modentry.VertexBufferSizeA = bnr.ReadInt32();
            modentry.VertexBufferSizeB = bnr.ReadInt32();
            modentry.GroupCount = bnr.ReadInt64();
            modentry.BonesOffset = Convert.ToInt32(bnr.ReadInt64());
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
            modentry.BoundingBoxCenter = new Vector4();
            modentry.BoundingBoxRadius = new Vector4();

            modentry.BoundingBoxCenter.X = bnr.ReadSingle();
            modentry.BoundingBoxCenter.Y = bnr.ReadSingle();
            modentry.BoundingBoxCenter.Z = bnr.ReadSingle();
            modentry.BoundingBoxCenter.W = bnr.ReadSingle();

            modentry.BoundingBoxRadius.X = bnr.ReadSingle();
            modentry.BoundingBoxRadius.Y = bnr.ReadSingle();
            modentry.BoundingBoxRadius.Z = bnr.ReadSingle();
            modentry.BoundingBoxRadius.W = bnr.ReadSingle();

            modentry.Field90 = bnr.ReadInt32();
            modentry.Field94 = bnr.ReadInt32();
            modentry.Field98 = bnr.ReadInt32();
            modentry.Field9C = bnr.ReadInt32();
            modentry.PrimitiveJointLinkCount = bnr.ReadInt32();

            //Bones.
            modentry.BoneLocalMatrixOffset = modentry.BonesOffset + (24 * modentry.BoneCount);
            modentry.BoneInvBindMatrixOffset = modentry.BoneLocalMatrixOffset + (modentry.BoneCount * 64);
            bnr.BaseStream.Position = modentry.BonesOffset;
            modentry.Bones = new List<ModelBoneEntry>();
            int PrevOffset = modentry.BonesOffset;

            for (int n = 0; n < modentry.BoneCount; n++)
            {
                ModelBoneEntry Bone = new ModelBoneEntry();
                Bone = Bone.FillModelBoneEntry(Bone, modentry, bnr, PrevOffset, n);
                modentry.Bones.Add(Bone);
                PrevOffset = PrevOffset + 24;
            }

            //Material Names.
            bnr.BaseStream.Position = modentry.MaterialsOffset;
            modentry.MaterialNames = new List<string>();
            string Stringtemp;
            for (int m = 0; m < modentry.MaterialCount; m++)
            {
                Stringtemp = Encoding.ASCII.GetString(bnr.ReadBytes(128)).Trim('\0');
                modentry.MaterialNames.Add(Stringtemp);
            }


            //Groups.
            bnr.BaseStream.Position = modentry.GroupOffset;
            modentry.Groups = new List<ModelGroupEntry>();
            PrevOffset = Convert.ToInt32(bnr.BaseStream.Position);

            for (int o = 0; o < modentry.GroupCount; o++)
            {
                ModelGroupEntry Group = new ModelGroupEntry();
                Group = Group.FillModelGroupEntry(Group, modentry, bnr, PrevOffset, o);
                modentry.Groups.Add(Group);

                PrevOffset = PrevOffset + 32;
            }

            //Primitives. Still Under Construction.
            uint PrimIndTemp, ShaderTemp;
            int PrevAddr, IndexBufferByteCount, IndexBufferOffset, VertBufferOffset, CurrentIndexBufferPosition;
            bnr.BaseStream.Position = modentry.PrimitveOffset;
            IndexBufferOffset = modentry.IndexBufferOffset;
            CurrentIndexBufferPosition = IndexBufferOffset;
            VertBufferOffset = modentry.VertexBufferOffset;
            modentry.Primitives = new List<ModelPrimitiveEntry>();
            for (int v = 0; v < modentry.PrimitiveCount; v++)
            {
                ModelPrimitiveEntry Prim = new ModelPrimitiveEntry();
                Prim.PrimOffset = Convert.ToInt32(bnr.BaseStream.Position);
                Prim.Flags = bnr.ReadInt16();
                Prim.VerticeCount = bnr.ReadInt16();
                PrimIndTemp = bnr.ReadUInt32();

                //For the Indice.
                Prim.Indice = new ModelPrimitiveEntry.Indices();
                Prim.Indice.GroupID = Convert.ToInt32(PrimIndTemp & 0xFFF);
                Prim.Indice.LODIndex = Convert.ToInt32((PrimIndTemp >> (8 * 3)) & 0xFF);
                Prim.Indice.MaterialIndex = Convert.ToInt32((PrimIndTemp & 0xFFF000) >> 12);

                Prim.VertexFlags = bnr.ReadInt16();
                Prim.VertexStride = bnr.ReadByte();
                Prim.RenderMode = bnr.ReadByte();
                Prim.VertexStartIndex = bnr.ReadInt32();
                Prim.VertexBufferOffset = bnr.ReadInt32();
                ShaderTemp = bnr.ReadUInt32();

                ///For the Shader.
                ModelPrimitiveEntry.MTShader shader = new ModelPrimitiveEntry.MTShader();
                shader.Index = Convert.ToInt32(ShaderTemp & 0x00000FFF);
                shader.ShaderObjectHash = CFGHandler.ShaderHashToName(shader.ShaderObjectHash, Convert.ToInt32(shader.Index));
                Prim.Shaders = shader;

                Prim.IndexBufferOffset = bnr.ReadInt32();
                Prim.IndexCount = bnr.ReadInt32();
                Prim.IndexStartIndex = bnr.ReadInt32();
                Prim.BoneMapStartIndex = bnr.ReadByte();
                Prim.PrimitiveJointLinkCount = bnr.ReadByte();
                Prim.ID = bnr.ReadInt16();
                Prim.MinVertexindex = bnr.ReadInt16();
                Prim.MaxVertexIndex = bnr.ReadInt16();
                Prim.Unknown2C = bnr.ReadInt32();
                Prim.PrimitiveJointLinkPtr = bnr.ReadInt64();


                //Validates the Primitive's Index count.
                if (Prim.IndexCount > 0)
                {
                    //Saves the current position then gets the Indexbuffer.
                    PrevAddr = Convert.ToInt32(bnr.BaseStream.Position);
                    bnr.BaseStream.Position = CurrentIndexBufferPosition;
                    Prim.IndexBuffer = new List<short>();
                    for (int e = 0; e < Prim.IndexCount; e++)
                        Prim.IndexBuffer.Add(bnr.ReadInt16());

                    //Adds the finished Primitive data to the list of Primitives.
                    IndexBufferOffset = Convert.ToInt32(bnr.BaseStream.Position);
                    CurrentIndexBufferPosition = Convert.ToInt32(bnr.BaseStream.Position);
                    bnr.BaseStream.Position = PrevAddr;
                }



                modentry.Primitives.Add(Prim);


            }

            int OffsetSaverA = Convert.ToInt32(bnr.BaseStream.Position);
            modentry.PLJs = new List<ModelPrimitiveJointLinkEntry>();

            //Primitive Joint Links.
            for (int v = 0; v < modentry.PrimitiveJointLinkCount; v++)
            {
                ModelPrimitiveJointLinkEntry plj = new ModelPrimitiveJointLinkEntry();

                plj.JointIndex = bnr.ReadInt32();
                plj.Unknwon04 = bnr.ReadInt32();
                plj.Unknown08 = bnr.ReadInt32();
                plj.Unknwon0C = bnr.ReadInt32();

                plj.BoundingSphere = new Vector4();
                plj.BoundingSphere.X = bnr.ReadSingle();
                plj.BoundingSphere.Y = bnr.ReadSingle();
                plj.BoundingSphere.Z = bnr.ReadSingle();
                plj.BoundingSphere.W = bnr.ReadSingle();

                plj.PLJMin = new Vector4();
                plj.PLJMin.X = bnr.ReadSingle();
                plj.PLJMin.Y = bnr.ReadSingle();
                plj.PLJMin.Z = bnr.ReadSingle();
                plj.PLJMin.W = bnr.ReadSingle();

                plj.PLJMax = new Vector4();
                plj.PLJMax.X = bnr.ReadSingle();
                plj.PLJMax.Y = bnr.ReadSingle();
                plj.PLJMax.Z = bnr.ReadSingle();
                plj.PLJMax.W = bnr.ReadSingle();

                plj.LocalMtx = new ModelPrimitiveJointLinkEntry.MTMatrix();
                plj.LocalMtx.Rows = new List<Vector4>();

                Vector4 VexM1 = new Vector4();
                VexM1.X = bnr.ReadSingle();
                VexM1.Y = bnr.ReadSingle();
                VexM1.Z = bnr.ReadSingle();
                VexM1.W = bnr.ReadSingle();

                plj.LocalMtx.Rows.Add(VexM1);

                Vector4 VexM2 = new Vector4();
                VexM2.X = bnr.ReadSingle();
                VexM2.Y = bnr.ReadSingle();
                VexM2.Z = bnr.ReadSingle();
                VexM2.W = bnr.ReadSingle();

                plj.LocalMtx.Rows.Add(VexM2);

                Vector4 VexM3 = new Vector4();
                VexM3.X = bnr.ReadSingle();
                VexM3.Y = bnr.ReadSingle();
                VexM3.Z = bnr.ReadSingle();
                VexM3.W = bnr.ReadSingle();

                plj.LocalMtx.Rows.Add(VexM3);

                Vector4 VexM4 = new Vector4();
                VexM4.X = bnr.ReadSingle();
                VexM4.Y = bnr.ReadSingle();
                VexM4.Z = bnr.ReadSingle();
                VexM4.W = bnr.ReadSingle();

                plj.LocalMtx.Rows.Add(VexM4);

                plj.UnknownVec80 = new Vector4();
                plj.UnknownVec80.X = bnr.ReadSingle();
                plj.UnknownVec80.Y = bnr.ReadSingle();
                plj.UnknownVec80.Z = bnr.ReadSingle();
                plj.UnknownVec80.W = bnr.ReadSingle();

                modentry.PLJs.Add(plj);


            }

            //Now for the Vertex Buffer.
            bnr.BaseStream.Position = modentry.VertexBufferOffset;
            modentry.VertexBuffer = bnr.ReadBytes(modentry.VertexBufferSize);

            //And The Indice Buffer.
            bnr.BaseStream.Position = modentry.IndexBufferOffset;
            modentry.IndexBuffer = bnr.ReadBytes(modentry.IndexCount);



            return modentry;

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
                return BoneCount;
            }
            set
            {
                BoneCount = value;
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
                return BoundingBoxCenter;
            }
            set
            {
                BoundingBoxCenter = value;
            }
        }

        [Category("Model Data"), ReadOnlyAttribute(true)]
        public Vector4 BoxMax
        {
            get
            {
                return BoundingBoxRadius;
            }
            set
            {
                BoundingBoxRadius = value;
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
