using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Archives;
using ThreeWorkTool.Resources.Utility;
using ThreeWorkTool.Resources.Wrappers;
using ThreeWorkTool.Resources.Wrappers.ExtraNodes;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class ChainCollisionEntry : DefaultWrapper
    {
        public string Magic;
        public int FileVersion;
        public int TotalEntrySize;
        public int EntryCount;
        public int ProjectedFileSize;
        public List<ChainColNode> Collisions;

        public static ChainCollisionEntry FillChainCollEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
        {

            ChainCollisionEntry ccolentry = new ChainCollisionEntry();

            FillEntry(filename, subnames, tree, br, c, ID, ccolentry, filetype);

            //Decompression Time.
            ccolentry.UncompressedData = ZlibStream.UncompressBuffer(ccolentry.CompressedData);

            //Type Specific Work Here.
            using (MemoryStream LmtStream = new MemoryStream(ccolentry.UncompressedData))
            {
                using (BinaryReader bnr = new BinaryReader(LmtStream))
                {
                    BuildChainCollEntry(bnr, ccolentry);
                }
            }

            return ccolentry;

        }

        public static ChainCollisionEntry BuildChainCollEntry(BinaryReader bnr, ChainCollisionEntry cclentry)
        {
            //Header.
            cclentry.Magic = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), cclentry.Magic);
            cclentry.FileVersion = bnr.ReadInt32();
            cclentry.EntryCount = bnr.ReadInt32();
            cclentry.ProjectedFileSize = bnr.ReadInt32();

            cclentry.Collisions = new List<ChainColNode>();

            for(int i = 0; i < cclentry.EntryCount; i++)
            {
                ChainColNode CNode = new ChainColNode();
                CNode.index = i;
                CNode.SomeFlags = bnr.ReadInt32();
                CNode.BoneID1 = bnr.ReadByte();
                bnr.BaseStream.Position = bnr.BaseStream.Position + 1;
                CNode.BoneID2 = bnr.ReadByte();
                bnr.BaseStream.Position = bnr.BaseStream.Position + 1;
                CNode.PrimitiveType = bnr.ReadInt32();
                bnr.BaseStream.Position = bnr.BaseStream.Position + 4;
                CNode.CenterPosition_X = bnr.ReadSingle();
                CNode.CenterPosition_Y = bnr.ReadSingle();
                CNode.CenterPosition_Z = bnr.ReadSingle();
                bnr.BaseStream.Position = bnr.BaseStream.Position + 4;
                CNode.Rotation_X = bnr.ReadSingle();
                CNode.Rotation_Y = bnr.ReadSingle();
                CNode.Rotation_Z = bnr.ReadSingle();
                CNode.Radius = bnr.ReadSingle();
                CNode.Unknown30 = bnr.ReadInt32();
                CNode.Unknown34 = bnr.ReadInt32();
                CNode.Unknown38 = bnr.ReadInt32();
                CNode.Unknown3C = bnr.ReadInt32();
                cclentry.Collisions.Add(CNode);
            }

            cclentry._FileType = cclentry.FileExt;
            cclentry._FileName = cclentry.TrueName;
            cclentry._DecompressedFileLength = cclentry.UncompressedData.Length;
            cclentry._CompressedFileLength = cclentry.CompressedData.Length;

            return cclentry;
        }

        public static ChainCollisionEntry ReplaceChainCollEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            ChainCollisionEntry ccolentry = new ChainCollisionEntry();
            ChainCollisionEntry oldentry = new ChainCollisionEntry();

            tree.BeginUpdate();

            //Type Specific Work Here.
            using (MemoryStream LmtStream = new MemoryStream(ccolentry.UncompressedData))
            {
                using (BinaryReader bnr = new BinaryReader(LmtStream))
                {
                    BuildChainCollEntry(bnr, ccolentry);
                }
            }

            ReplaceEntry(tree, node, filename, ccolentry, oldentry);
            ccolentry.DecompressedFileLength = ccolentry.UncompressedData.Length;
            ccolentry._DecompressedFileLength = ccolentry.UncompressedData.Length;
            ccolentry.CompressedFileLength = ccolentry.CompressedData.Length;
            ccolentry._CompressedFileLength = ccolentry.CompressedData.Length;
            ccolentry._FileName = ccolentry.TrueName;
            ccolentry._FileType = ccolentry.FileExt;
            ccolentry.FileName = ccolentry.TrueName;

            return node.entryfile as ChainCollisionEntry;
        }

        public static ChainCollisionEntry InsertChainCollEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            ChainCollisionEntry ccolentry = new ChainCollisionEntry();

            InsertEntry(tree, node, filename, ccolentry);

            //Type Specific Work Here.
            using (MemoryStream LmtStream = new MemoryStream(ccolentry.UncompressedData))
            {
                using (BinaryReader bnr = new BinaryReader(LmtStream))
                {
                    BuildChainCollEntry(bnr, ccolentry);
                }
            }

            ccolentry.DecompressedFileLength = ccolentry.UncompressedData.Length;
            ccolentry._DecompressedFileLength = ccolentry.UncompressedData.Length;
            ccolentry.CompressedFileLength = ccolentry.CompressedData.Length;
            ccolentry._CompressedFileLength = ccolentry.CompressedData.Length;
            ccolentry._FileName = ccolentry.TrueName;
            ccolentry._FileType = ccolentry.FileExt;
            ccolentry.EntryName = ccolentry.FileName;



            return ccolentry;
        }

        public static ChainCollisionEntry SaveCCLEntry(ChainCollisionEntry cclentry, TreeNode node)
        {
            using (MemoryStream CCLStream = new MemoryStream(cclentry.UncompressedData))
            {
                using (BinaryWriter bwr = new BinaryWriter(CCLStream))
                {
                    bwr.BaseStream.Position = 0x10;
                    byte B;
                    //Gets All The ChainColNode Files From Child Nodes variables and writes to the main ChainCollisionEntry file.
                    foreach (ArcEntryWrapper youngn in node.Nodes)
                    {
                        ChainColNode ccolnode = youngn.Tag as ChainColNode;
                        if(ccolnode != null)
                        {
                            bwr.Write(ccolnode.SomeFlags);
                            bwr.Write(ccolnode.BoneID1);
                            bwr.BaseStream.Position = bwr.BaseStream.Position + 1;
                            bwr.Write(ccolnode.BoneID2);
                            if(ccolnode.BoneID2 == 0xFF)
                            {
                                B = 0xFF;
                                bwr.Write(B);
                            }
                            else
                            {
                                B = 0x00;
                                bwr.Write(B);
                            }
                            bwr.Write(ccolnode.PrimitiveType);
                            bwr.BaseStream.Position = bwr.BaseStream.Position + 4;
                            bwr.Write(ccolnode.CenterPosition_X);
                            bwr.Write(ccolnode.CenterPosition_Y);
                            bwr.Write(ccolnode.CenterPosition_Z);
                            bwr.BaseStream.Position = bwr.BaseStream.Position + 4;
                            bwr.Write(ccolnode.Rotation_X);
                            bwr.Write(ccolnode.Rotation_Y);
                            bwr.Write(ccolnode.Rotation_Z);
                            bwr.Write(ccolnode.Radius);
                            bwr.Write(ccolnode.Unknown30);
                            bwr.Write(ccolnode.Unknown34);
                            bwr.Write(ccolnode.Unknown38);
                            bwr.Write(ccolnode.Unknown3C);

                        }
                    }
                }
            }

            cclentry.CompressedData = Zlibber.Compressor(cclentry.UncompressedData);

            return cclentry;
        }

        #region Chain Collision Properties
        private string _FileName;
        [Category("Filename"), ReadOnlyAttribute(true)]
        public string FileName
        {

            get
            {
                return _FileName;
            }
            set
            {
                _FileName = value;
            }
        }

        private long _CompressedFileLength;
        [Category("MT ARC Entry"), ReadOnlyAttribute(true)]
        public long CompressedFileLength
        {

            get
            {
                return _CompressedFileLength;
            }
            set
            {
                _CompressedFileLength = value;
            }
        }

        private long _DecompressedFileLength;
        [Category("MT ARC Entry"), ReadOnlyAttribute(true)]
        public long DecompressedFileLength
        {

            get
            {
                return _DecompressedFileLength;
            }
            set
            {
                _DecompressedFileLength = value;
            }
        }

        private string _FileType;
        [Category("Filename"), ReadOnlyAttribute(true)]
        public string FileType
        {

            get
            {
                return _FileType;
            }
            set
            {
                _FileType = value;
            }
        }

        #endregion

    }
}
