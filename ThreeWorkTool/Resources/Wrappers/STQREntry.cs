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
    public class STQREntry : DefaultWrapper
    {
        public string Magic;
        public int Format;
        public int EntryCount;
        public int MetadataEntryCount;
        public long SomeOffset01;
        public long SomeOffset02;
        public long EntryDataOffset;
        public long DataBlocksOffset;
        public long FilePathBlockOffsetA;
        public long FilePathBlockOffsetB;
        public long FilePathBlockOffsetC;
        public long FilePathBlockOffsetD;

        public List<STQRNode> EntryList;
        public List<STQREventData> Events;

        public static STQREntry FillSTQREntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
        {
            STQREntry stqrentry = new STQREntry();
            List<byte> BTemp = new List<byte>();

            FillEntry(filename, subnames, tree, br, c, ID, stqrentry, filetype);

            //Decompression Time.
            stqrentry.UncompressedData = ZlibStream.UncompressBuffer(stqrentry.CompressedData);

            //Type Specific Work Here.
            using (MemoryStream LmtStream = new MemoryStream(stqrentry.UncompressedData))
            {
                using (BinaryReader bnr = new BinaryReader(LmtStream))
                {
                    BuildSTQRentry(bnr, stqrentry);
                }
            }

            return stqrentry;
        }

        public static STQREntry BuildSTQRentry(BinaryReader bnr, STQREntry stqrentry)
        {
            ASCIIEncoding ascii = new ASCIIEncoding();

            //Header.
            stqrentry.Magic = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), stqrentry.Magic);
            stqrentry.Format = bnr.ReadInt32();
            stqrentry.EntryCount = bnr.ReadInt32();
            stqrentry.MetadataEntryCount = bnr.ReadInt32();

            stqrentry.SomeOffset01 = bnr.ReadInt64();
            //bnr.BaseStream.Position = bnr.BaseStream.Position + 4;
            stqrentry.SomeOffset02 = bnr.ReadInt64();
            //bnr.BaseStream.Position = bnr.BaseStream.Position + 4;
            stqrentry.EntryDataOffset = bnr.ReadInt64();
            //bnr.BaseStream.Position = bnr.BaseStream.Position + 4;
            stqrentry.DataBlocksOffset = bnr.ReadInt64();
            //bnr.BaseStream.Position = bnr.BaseStream.Position + 4;
            stqrentry.FilePathBlockOffsetA = bnr.ReadInt64();
            //bnr.BaseStream.Position = bnr.BaseStream.Position + 4;
            stqrentry.FilePathBlockOffsetB = bnr.ReadInt64();
            //bnr.BaseStream.Position = bnr.BaseStream.Position + 4;
            stqrentry.FilePathBlockOffsetC = bnr.ReadInt64();
            //bnr.BaseStream.Position = bnr.BaseStream.Position + 4;
            stqrentry.FilePathBlockOffsetD = bnr.ReadInt64();
            //bnr.BaseStream.Position = bnr.BaseStream.Position + 4;

            stqrentry.EntryList = new List<STQRNode>();

            //GroupBox Entries.
            for (int i = 0; i < stqrentry.EntryCount; i++)
            {
                STQRNode stqr = new STQRNode();
                //stqr = STQRNode.BuildSTQNode(stqrentry, i, bnr, stqr);

                stqr.FileNameOffset = bnr.ReadInt32();
                stqr.UnknownDataA = bnr.ReadInt32();
                stqr.FileSize = bnr.ReadInt32();
                stqr.Duration = bnr.ReadInt32();
                stqr.Channels = bnr.ReadInt32();
                stqr.SampleRate = bnr.ReadInt32();
                stqr.LoopStart = bnr.ReadInt32();
                stqr.LoopEnd = bnr.ReadInt32();
                stqr.EntryTypeHash = ByteUtilitarian.HashBytesToString(bnr.ReadBytes(4));
                stqr.UnknownDataB = bnr.ReadInt32();

                try
                {
                    //Undocumented Data
                    //int UndocumentedDataSize = (int)(stqrentry.EntryList[0].FileNameOffset - bnr.BaseStream.Position);
                    //stqr.UnknownDataB = new byte[UndocumentedDataSize];
                    //stqr.UnknownDataB = bnr.ReadBytes(UndocumentedDataSize);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("This array size is bogus.\n" + ex);
                }

                stqrentry.EntryList.Add(stqr);

            }

            long PreviousPosition = bnr.BaseStream.Position;
            string Teme;

            //Now For the FileNames.
            for (int j = 0; j < stqrentry.EntryList.Count; j++)
            {
                bnr.BaseStream.Position = stqrentry.EntryList[j].FileNameOffset;
                //Gets the name length to avoid going out of bounds.
                int namelength = 0;
                if ((j + 1) != stqrentry.EntryList.Count)
                {
                    namelength = stqrentry.EntryList[j + 1].FileNameOffset - stqrentry.EntryList[j].FileNameOffset;
                }
                else
                {
                    namelength = stqrentry.UncompressedData.Length - stqrentry.EntryList[j].FileNameOffset;
                }
                byte[] PLName = new byte[] { };
                PLName = stqrentry.UncompressedData.Skip(stqrentry.EntryList[j].FileNameOffset).Take(namelength).Where(x => x != 0x00).ToArray();
                Teme = ascii.GetString(PLName);
                stqrentry.EntryList[j].FilePath = Teme;

            }

            //For The Event data.
            stqrentry.Events = new List<STQREventData>();
            bnr.BaseStream.Position = stqrentry.DataBlocksOffset;

            for (int j = 0; j < stqrentry.MetadataEntryCount; j++)
            {
                STQREventData sTQREvent = new STQREventData();
                sTQREvent.EventEntryID = bnr.ReadInt32();

                sTQREvent.UnknownValue04 = bnr.ReadInt32();
                sTQREvent.UnknownValue08 = bnr.ReadInt32();
                sTQREvent.UnknownValue0C = bnr.ReadInt32();

                sTQREvent.UnknownValue10 = bnr.ReadInt32();
                sTQREvent.UnknownValue14 = bnr.ReadInt32();
                sTQREvent.UnknownValue18 = bnr.ReadInt32();
                sTQREvent.UnknownValue1C = bnr.ReadInt32();

                sTQREvent.UnknownValue20 = bnr.ReadInt32();
                sTQREvent.UnknownValue24 = bnr.ReadInt32();
                sTQREvent.UnknownValue28 = bnr.ReadInt32();
                sTQREvent.UnknownValue2C = bnr.ReadInt32();

                sTQREvent.UnknownValue30 = bnr.ReadInt32();
                sTQREvent.UnknownValue34 = bnr.ReadInt32();
                sTQREvent.UnknownValue38 = bnr.ReadInt32();
                sTQREvent.UnknownValue3C = bnr.ReadInt32();

                sTQREvent.UnknownValue40 = bnr.ReadInt32();
                sTQREvent.UnknownValue44 = bnr.ReadInt32();
                sTQREvent.UnknownValue48 = bnr.ReadInt32();
                sTQREvent.UnknownValue4C = bnr.ReadInt32();

                sTQREvent.UnknownValue50 = bnr.ReadInt32();
                sTQREvent.UnknownValue54 = bnr.ReadInt32();
                sTQREvent.UnknownValue58 = bnr.ReadInt32();
                sTQREvent.UnknownValue5C = bnr.ReadInt32();

                sTQREvent.UnknownValue60 = bnr.ReadInt32();
                sTQREvent.UnknownValue64 = bnr.ReadInt32();
                sTQREvent.UnknownValue68 = bnr.ReadInt32();
                sTQREvent.UnknownValue6C = bnr.ReadInt32();

                sTQREvent.UnknownValue70 = bnr.ReadInt32();
                sTQREvent.UnknownValue74 = bnr.ReadInt32();
                sTQREvent.UnknownValue78 = bnr.ReadInt32();
                sTQREvent.UnknownValue7C = bnr.ReadInt32();

                sTQREvent.UnknownValue80 = bnr.ReadInt32();
                sTQREvent.UnknownValue84 = bnr.ReadInt32();
                sTQREvent.UnknownValue88 = bnr.ReadInt32();
                sTQREvent.UnknownValue8C = bnr.ReadInt32();

                sTQREvent.UnknownValue90 = bnr.ReadInt32();
                sTQREvent.UnknownValue94 = bnr.ReadInt32();

                stqrentry.Events.Add(sTQREvent);

            }


            bnr.BaseStream.Position = PreviousPosition;

            return stqrentry;

        }

        public static STQREntry ReplaceSTQREntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            STQREntry stqrentry = new STQREntry();
            STQREntry oldentry = new STQREntry();

            tree.BeginUpdate();

            ReplaceEntry(tree, node, filename, stqrentry, oldentry);

            //Type Specific Work Here.
            using (MemoryStream LmtStream = new MemoryStream(stqrentry.UncompressedData))
            {
                using (BinaryReader bnr = new BinaryReader(LmtStream))
                {
                    BuildSTQRentry(bnr, stqrentry);
                }
            }

            return node.entryfile as STQREntry;
        }

        public static STQREntry InsertSTQREntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            STQREntry stqr = new STQREntry();

            InsertEntry(tree, node, filename, stqr);

            //Decompression Time.
            stqr.UncompressedData = ZlibStream.UncompressBuffer(stqr.CompressedData);

            try
            {
                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
                {
                    BuildSTQRentry(bnr, stqr);
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

            return stqr;
        }

        public static STQREntry RebuildSTQREntry(ArcEntryWrapper node, STQREntry stqrentry)
        {



            return stqrentry;

        }

        public static STQREntry SaveSTQREntry(STQREntry stqrentry, TreeNode node)
        {

            using (MemoryStream STQStream = new MemoryStream(stqrentry.UncompressedData))
            {
                using (BinaryWriter bwr = new BinaryWriter(STQStream))
                {

                }
            }


            stqrentry.CompressedData = Zlibber.Compressor(stqrentry.UncompressedData);


            return stqrentry;

        }

        #region STQR
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

        [Category("STQR"), ReadOnlyAttribute(true)]
        public int NumEntries
        {

            get
            {
                return EntryCount;
            }
            set
            {
                EntryCount = value;
            }
        }

        [Category("STQR"), ReadOnlyAttribute(true)]
        public int EventCount
        {

            get
            {
                return MetadataEntryCount;
            }
            set
            {
                MetadataEntryCount = value;
            }
        }

        #endregion

    }
}
