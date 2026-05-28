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
        public static ChainEntry RebuildChainEntry(TreeView tree, ArcEntryWrapper node)
        {

            ChainEntry oldChain = tree.SelectedNode.Tag as ChainEntry;

            //Gotta start from scratch.
            ChainEntry newChain = new ChainEntry();
            newChain.Segments = new List<ChainSegment>();
            newChain.TrueName = oldChain.TrueName;

            int SegmentCount = 0;
            int RecordCount = 0;
            int CurrentRecordCounter = 0;
            int l = 0;
            //We need to get all the Segment TreeNodes, then the Segment Record TreeNodes.
            List<TreeNode> Children = new List<TreeNode>();
            List<TreeNode> GrandChildren = new List<TreeNode>();

            foreach (TreeNode thisNode in tree.SelectedNode.Nodes)
            {
                //Children.Add(thisNode);
                //SegmentCount++;

                //Let's get the Segment' Records.
                ChainSegment segment = thisNode.Tag as ChainSegment;
                segment.Index = l;
                if (segment != null)
                {
                    segment.Records = new List<ChainSegmentRecord>();
                    segment.NodeCount = Convert.ToByte(thisNode.Nodes.Count);

                    //foreach (TreeNode RecNode in tree.SelectedNode.Nodes)
                    for (int m = 0; m < thisNode.Nodes.Count; m++)
                    {
                        ChainSegmentRecord record = thisNode.Nodes[m].Tag as ChainSegmentRecord;
                        if (record != null)
                        {
                            record.Index = m;
                            segment.Records.Add(record);
                            GrandChildren.Add(thisNode.Nodes[m]);
                        }
                    }
                    //
                    newChain.Segments.Add(segment);
                }

                Children.Add(thisNode);
                SegmentCount++;

                l++;
            }

            //Now to rebuild from scratch using the data from above.
            List<byte> NewUncompressedData = new List<byte>();

            //Header is a size of 0x10.
            byte[] HeaderAndVersion = { 0x43, 0x48, 0x4E, 0x00, 0x18, 0x06, 0x08, 0x00 };
            byte[] PlaceholderRestOfHeader = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            //Segments are always 0x20.
            byte[] PlaceholderSegment = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                          0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};

            byte[] FillerForSegment = { 0x00, 0x00 };

            //Records are always 0x5C.
            byte[] PlaceholderRecord = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};

            byte FillerForRecord = 0x00;

            //Gonna fill in the params as I go.
            newChain.SegmentCount = Children.Count;
            newChain.ProjectedFileSize = (Children.Count * 0x20) + (GrandChildren.Count * 0x5C);

            NewUncompressedData.AddRange(HeaderAndVersion);
            NewUncompressedData.AddRange(BitConverter.GetBytes(newChain.ProjectedFileSize));
            NewUncompressedData.AddRange(BitConverter.GetBytes(newChain.SegmentCount));

            int RecordCounter = 0;
            long OffsetForRecords = (Children.Count * 0x20) + 0x10;

            //Segment data.
            for (int p = 0; p < Children.Count; p++)
            {
                ChainSegment segm = Children[p].Tag as ChainSegment;
                if (segm != null)
                {
                    NewUncompressedData.Add(segm.NodeCount);
                    NewUncompressedData.Add(segm.Flags);
                    NewUncompressedData.AddRange(FillerForSegment);


                    //Calculate the offset for the start of the records.
                    int OffsetForThisSegment = (RecordCounter * 0x5C) + ((newChain.SegmentCount - p) * 0x20);
                    NewUncompressedData.AddRange(BitConverter.GetBytes(OffsetForThisSegment));

                    NewUncompressedData.AddRange(BitConverter.GetBytes(segm.UnkParam1));
                    NewUncompressedData.AddRange(BitConverter.GetBytes(segm.UnkParam2));
                    NewUncompressedData.AddRange(BitConverter.GetBytes(segm.UnkParam3));
                    NewUncompressedData.AddRange(BitConverter.GetBytes(segm.UnkParam4));
                    NewUncompressedData.AddRange(BitConverter.GetBytes(segm.UnkParam5));
                    NewUncompressedData.AddRange(BitConverter.GetBytes(segm.UnkParam6));
                    RecordCounter = RecordCounter + segm.NodeCount;

                }
            }

            //Records data.
            for (int q = 0; q < GrandChildren.Count; q++)
            {
                ChainSegmentRecord Srec = GrandChildren[q].Tag as ChainSegmentRecord;
                if (Srec != null)
                {
                    NewUncompressedData.AddRange(BitConverter.GetBytes(Srec.Flags));
                    NewUncompressedData.Add(Convert.ToByte(Srec.Joint));
                    NewUncompressedData.Add(FillerForRecord);
                    NewUncompressedData.Add(Srec.LinkCount);
                    NewUncompressedData.Add(Srec.unk07);

                    NewUncompressedData.AddRange(BitConverter.GetBytes(Srec.SomeFloat08));
                    NewUncompressedData.AddRange(BitConverter.GetBytes(Srec.SomeFloat0C));

                    NewUncompressedData.AddRange(BitConverter.GetBytes(Srec.SomeFloat10));
                    NewUncompressedData.AddRange(BitConverter.GetBytes(Srec.SomeFloat14));
                    NewUncompressedData.AddRange(BitConverter.GetBytes(Srec.SomeFloat18));
                    NewUncompressedData.AddRange(BitConverter.GetBytes(Srec.SomeFloat1C));

                    NewUncompressedData.AddRange(BitConverter.GetBytes(Srec.SomeFloat20));
                    NewUncompressedData.AddRange(BitConverter.GetBytes(Srec.SomeFloat24));
                    NewUncompressedData.AddRange(BitConverter.GetBytes(Srec.SomeFloat28));
                    NewUncompressedData.AddRange(BitConverter.GetBytes(Srec.SomeFloat2C));

                    NewUncompressedData.AddRange(BitConverter.GetBytes(Srec.SomeFloat30));
                    NewUncompressedData.Add(Srec.SomeByte34);
                    NewUncompressedData.Add(Srec.SomeByte35);
                    NewUncompressedData.Add(Srec.SomeByte36);
                    NewUncompressedData.Add(Srec.SomeByte37);
                    NewUncompressedData.AddRange(BitConverter.GetBytes(Srec.SomeFloat38));
                    NewUncompressedData.AddRange(BitConverter.GetBytes(Srec.SomeFloat3C));

                    NewUncompressedData.Add(Srec.JointLinkIndex1);
                    NewUncompressedData.Add(Srec.JointLinkIndex2);
                    NewUncompressedData.Add(Srec.JointLinkIndex3);
                    NewUncompressedData.Add(Srec.JointLinkIndex4);
                    NewUncompressedData.Add(Srec.JointLinkIndex5);
                    NewUncompressedData.Add(Srec.JointLinkIndex6);
                    NewUncompressedData.Add(Srec.JointLinkIndex7);
                    NewUncompressedData.Add(Srec.JointLinkIndex8);

                    NewUncompressedData.AddRange(BitConverter.GetBytes(Srec.SomeFloat48));
                    NewUncompressedData.AddRange(BitConverter.GetBytes(Srec.SomeFloat4C));
                    NewUncompressedData.AddRange(BitConverter.GetBytes(Srec.SomeFloat50));
                    NewUncompressedData.AddRange(BitConverter.GetBytes(Srec.SomeFloat54));
                    NewUncompressedData.AddRange(BitConverter.GetBytes(Srec.SomeFloat58));

                }

            }

#if DEBUG
            File.WriteAllBytes("D:\\Workshop\\MessHall\\Etc\\ChainTestSamples\\" + newChain.TrueName  + "_ChainHeaderSampleTest.bin", NewUncompressedData.ToArray());
#endif

            newChain.UncompressedData = NewUncompressedData.ToArray();
            newChain.CompressedData = Zlibber.Compressor(newChain.UncompressedData);

            newChain._FileType = ".chn";
            newChain.FileExt = ".chn";

            return newChain;
        }

        public static ChainEntry TransferCHNEntryProperties(ChainEntry OldCHN, ChainEntry NewCHN, Type filetype = null)
        {

            NewCHN._FileName = OldCHN._FileName;
            NewCHN._CompressedFileLength = NewCHN.CompressedData.Length;
            NewCHN._DecompressedFileLength = NewCHN.UncompressedData.Length;
            NewCHN.FileExt = OldCHN.FileExt;
            NewCHN.FileName = OldCHN.FileName;
            NewCHN.TrueName = OldCHN.TrueName;
            NewCHN.EntryDirs = OldCHN.EntryDirs;
            NewCHN.EntryID = OldCHN.EntryID;
            NewCHN.TypeHash = OldCHN.TypeHash;
            NewCHN.EntryName = OldCHN.EntryName;

            return NewCHN;
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
