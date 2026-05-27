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
    public class ChainEntry : DefaultWrapper
    {
        //Header Stuff.
        //Thanks to Rootz for helping with updating the info.
        public string Magic;
        public int FileVersion;
        public int ProjectedFileSize; //Is this what is called Payload?
        public int SegmentCount;

        public static int CHAINSEGMENTSIZE = 0x20;
        public static int CHAINSEGMENTRECORDSIZE = 0x5C;
        public static int CHAINRECORDLINKTABLEOFFSET = 0x40;

        //public struct ChainSegmentRecord
        //{
        //    public int Index;
        //    public int Offset;
        //    public uint Flags;
        //    public uint MoreFlags;
        //    public int Joint;
        //    public byte LinkCount;
        //    public byte unk07;
        //    public float SomeFloat08 { get; set; }
        //    public float SomeFloat0C { get; set; }
        //    public float SomeFloat10 { get; set; }
        //    public float SomeFloat14 { get; set; }
        //    public float SomeFloat18 { get; set; }
        //    public float SomeFloat1C { get; set; }
        //    public float SomeFloat20 { get; set; }
        //    public float SomeFloat24 { get; set; }
        //    public float SomeFloat28 { get; set; }
        //    public float SomeFloat2C { get; set; }
        //    public float SomeFloat30 { get; set; }
        //    public float SomeFloat34 { get; set; }
        //    public float SomeFloat38 { get; set; }
        //    public float SomeFloat3C { get; set; }
        //    public long AbsoluteLocation;
        //    public byte JointLinkIndex1 { get; set; }
        //    public byte JointLinkIndex2 { get; set; }
        //    public byte JointLinkIndex3 { get; set; }
        //    public byte JointLinkIndex4 { get; set; }
        //    public byte JointLinkIndex5 { get; set; }
        //    public byte JointLinkIndex6 { get; set; }
        //    public byte JointLinkIndex7 { get; set; }
        //    public byte JointLinkIndex8 { get; set; }
        //    public float SomeFloat48 { get; set; }
        //    public float SomeFloat4C { get; set; }
        //    public float SomeFloat50 { get; set; }
        //    public float SomeFloat54 { get; set; }
        //    public float SomeFloat58 { get; set; }
        //    //public List<int> JointLinkIndices;
        //    //public List<int> LinkTable;
        //    //public List<uint> SolverParams;
        //    //public List<float> SolverFloatParams;
        //    //public List<uint> RawData;
        //    //public List<float> rawFloats;
        //    //public List<byte> TailBytes;
        //};

        //public struct ChainSegment
        //{
        //    public int Index;
        //    public int Offset;
        //    public byte NodeCount;
        //    public byte Flags { get; set; }
        //    public uint RecordOffset;
        //    public uint RecordAbsOffset;
        //    public int RecordBytes;
        //    //public List<uint> ParametersU;
        //    //public List<float> ParamtersF;

        //    public float UnkParam1 { get; set; }
        //    public float UnkParam2 { get; set; }
        //    public float UnkParam3 { get; set; }
        //    public float UnkParam4 { get; set; }
        //    public float UnkParam5 { get; set; }
        //    public float UnkParam6 { get; set; }

        //    public List<ChainSegmentRecord> Records;
        //};

        public List<ChainSegment> Segments;

        public static ChainEntry FillChainEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
        {

            ChainEntry chnentry = new ChainEntry();

            FillEntry(filename, subnames, tree, br, c, ID, chnentry, filetype);

            //Type Specific Work Here.
            using (MemoryStream LmtStream = new MemoryStream(chnentry.UncompressedData))
            {
                using (BinaryReader bnr = new BinaryReader(LmtStream))
                {
                    BuildChainEntry(bnr, chnentry);
                }
            }

            return chnentry;

        }

        public static ChainEntry BuildChainEntry(BinaryReader bnr, ChainEntry chnentry)
        {

            //Header.
            chnentry.Magic = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), chnentry.Magic);
            chnentry.FileVersion = bnr.ReadInt32();
            chnentry.ProjectedFileSize = bnr.ReadInt32();
            chnentry.SegmentCount = bnr.ReadInt32();

            //Going to validate. 
            if (bnr.BaseStream.Length != (chnentry.ProjectedFileSize + 0x10))
            {
                MessageBox.Show("Something funky is up with the chain file:\n" + chnentry.TrueName + "The projected file size is incorrect and I can't read this further.");

                chnentry._FileType = chnentry.FileExt;
                chnentry._FileName = chnentry.TrueName;
                chnentry._DecompressedFileLength = chnentry.UncompressedData.Length;
                chnentry._CompressedFileLength = chnentry.CompressedData.Length;

                return chnentry;
            }

            chnentry.Segments = new List<ChainSegment>();

            for (int j = 0; j < chnentry.SegmentCount; j++)
            {
                bnr.BaseStream.Position = 0x10 + j * CHAINSEGMENTSIZE;
                ChainSegment seg = new ChainSegment();
                seg.Index = j;
                seg.NodeCount = bnr.ReadByte();
                seg.Flags = bnr.ReadByte();
                bnr.BaseStream.Position = bnr.BaseStream.Position + 2;
                seg.RecordOffset = bnr.ReadUInt32();
                seg.RecordAbsOffset = Convert.ToUInt32((j * CHAINSEGMENTSIZE) + (seg.RecordOffset));
                seg.RecordBytes = seg.NodeCount * CHAINSEGMENTRECORDSIZE;

                //Dunno what these params are but hey, may as well get them down.
                seg.UnkParam1 = bnr.ReadSingle();
                seg.UnkParam2 = bnr.ReadSingle();
                seg.UnkParam3 = bnr.ReadSingle();
                seg.UnkParam4 = bnr.ReadSingle();
                seg.UnkParam5 = bnr.ReadSingle();
                seg.UnkParam6 = bnr.ReadSingle();

                //Now for the Records.
                seg.Records = new List<ChainSegmentRecord>();

                for (int k = 0; k < seg.NodeCount; k++)
                {
                    ChainSegmentRecord Rec = new ChainSegmentRecord();
                    Rec.AbsoluteLocation = 0x10 + seg.RecordAbsOffset + (0x5C * k);

                    //bnr.BaseStream.Position = Rec.AbsoluteLocation;
                    //Rec.TailBytes = new List<byte>();
                    //Rec.RawData = new List<uint>();
                    //Rec.rawFloats = new List<float>();

                    ////Filling the raw data in. The number used to check is ChainSegmentRecordsize(92) / 4.
                    //for (int d = 0; d < 23; d++)
                    //{
                    //    Rec.RawData.Add(bnr.ReadUInt32());
                    //}

                    //bnr.BaseStream.Position = Rec.AbsoluteLocation;

                    ////Filling the raw data in. The number used to check is ChainSegmentRecordsize(92) / 4.
                    //for (int d = 0; d < 23; d++)
                    //{
                    //    Rec.rawFloats.Add(bnr.ReadSingle());
                    //}

                    //bnr.BaseStream.Position = Rec.AbsoluteLocation + 6;
                    //Rec.LinkCount = bnr.ReadInt16();

                    //bnr.BaseStream.Position = Rec.AbsoluteLocation + CHAINRECORDLINKTABLEOFFSET;
                    //long TempL = (Rec.AbsoluteLocation + CHAINSEGMENTRECORDSIZE) - (Rec.AbsoluteLocation + CHAINRECORDLINKTABLEOFFSET);
                    //Rec.TailBytes.AddRange(bnr.ReadBytes(Convert.ToInt32(TempL)));
                    Rec.Index = k;
                    bnr.BaseStream.Position = Rec.AbsoluteLocation;
                    Rec.Flags = bnr.ReadUInt32();

                    Rec.Joint = bnr.ReadByte();

                    bnr.BaseStream.Position = Rec.AbsoluteLocation + 6;
                    Rec.LinkCount = bnr.ReadByte();
                    Rec.unk07 = bnr.ReadByte();

                    Rec.SomeFloat08 = bnr.ReadSingle();
                    Rec.SomeFloat0C = bnr.ReadSingle();

                    Rec.SomeFloat10 = bnr.ReadSingle();
                    Rec.SomeFloat14 = bnr.ReadSingle();
                    Rec.SomeFloat18 = bnr.ReadSingle();
                    Rec.SomeFloat1C = bnr.ReadSingle();

                    Rec.SomeFloat20 = bnr.ReadSingle();
                    Rec.SomeFloat24 = bnr.ReadSingle();
                    Rec.SomeFloat28 = bnr.ReadSingle();
                    Rec.SomeFloat2C = bnr.ReadSingle();

                    Rec.SomeFloat30 = bnr.ReadSingle();
                    Rec.SomeByte34 = bnr.ReadByte();
                    Rec.SomeByte35 = bnr.ReadByte();
                    Rec.SomeByte36 = bnr.ReadByte();
                    Rec.SomeByte37 = bnr.ReadByte();
                    Rec.SomeFloat38 = bnr.ReadSingle();
                    Rec.SomeFloat3C = bnr.ReadSingle();

                    Rec.JointLinkIndex1 = bnr.ReadByte();
                    Rec.JointLinkIndex2 = bnr.ReadByte();
                    Rec.JointLinkIndex3 = bnr.ReadByte();
                    Rec.JointLinkIndex4 = bnr.ReadByte();
                    Rec.JointLinkIndex5 = bnr.ReadByte();
                    Rec.JointLinkIndex6 = bnr.ReadByte();
                    Rec.JointLinkIndex7 = bnr.ReadByte();
                    Rec.JointLinkIndex8 = bnr.ReadByte();

                    Rec.SomeFloat48 = bnr.ReadSingle();
                    Rec.SomeFloat4C = bnr.ReadSingle();
                    Rec.SomeFloat50 = bnr.ReadSingle();
                    Rec.SomeFloat54 = bnr.ReadSingle();
                    Rec.SomeFloat58 = bnr.ReadSingle();
                    seg.Records.Add(Rec);
                }
                chnentry.Segments.Add(seg);

            }



            chnentry._FileType = chnentry.FileExt;
            chnentry._FileName = chnentry.TrueName;
            chnentry._DecompressedFileLength = chnentry.UncompressedData.Length;
            chnentry._CompressedFileLength = chnentry.CompressedData.Length;

            return chnentry;
        }

        public static ChainEntry ReplaceChainEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            ChainEntry chnentry = new ChainEntry();
            ChainEntry oldentry = new ChainEntry();

            tree.BeginUpdate();

            ReplaceEntry(tree, node, filename, chnentry, oldentry);
            chnentry.FileName = chnentry.TrueName;
            chnentry.DecompressedFileLength = chnentry.UncompressedData.Length;
            chnentry._DecompressedFileLength = chnentry.UncompressedData.Length;
            chnentry.CompressedFileLength = chnentry.CompressedData.Length;
            chnentry._CompressedFileLength = chnentry.CompressedData.Length;
            chnentry._FileName = chnentry.TrueName;
            chnentry._FileType = chnentry.FileExt;

            return node.entryfile as ChainEntry;
        }

        public static ChainEntry InsertChainEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            ChainEntry chnentry = new ChainEntry();

            InsertEntry(tree, node, filename, chnentry);

            chnentry.DecompressedFileLength = chnentry.UncompressedData.Length;
            chnentry._DecompressedFileLength = chnentry.UncompressedData.Length;
            chnentry.CompressedFileLength = chnentry.CompressedData.Length;
            chnentry._CompressedFileLength = chnentry.CompressedData.Length;
            chnentry._FileName = chnentry.TrueName;
            chnentry._FileType = chnentry.FileExt;
            chnentry.EntryName = chnentry.FileName;



            return chnentry;
        }

        //Rebuilds using the new data.
        public static ChainEntry RebuildChainEntry(ChainEntry chnentry)
        {



            return chnentry;
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
