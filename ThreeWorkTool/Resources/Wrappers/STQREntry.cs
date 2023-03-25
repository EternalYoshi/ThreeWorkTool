using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
                byte[] BTemp = new byte[] { };
                stqr.index = i;
                stqr.FileNameOffset = bnr.ReadInt32();
                stqr.UnknownDataA = bnr.ReadInt32();
                stqr.FileSize = bnr.ReadInt32();
                stqr.Duration = bnr.ReadInt32();
                stqr.Channels = bnr.ReadInt32();
                stqr.SampleRate = bnr.ReadInt32();
                stqr.LoopStart = bnr.ReadInt32();
                stqr.LoopEnd = bnr.ReadInt32();
                BTemp = bnr.ReadBytes(4);
                Array.Reverse(BTemp);
                stqr.EntryTypeHash = ByteUtilitarian.HashBytesToString(BTemp);
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
                sTQREvent.index = j;
                sTQREvent.EventEntryID = bnr.ReadInt16();
                sTQREvent.UnknownValue02 = bnr.ReadInt16();
                sTQREvent.UnknownValue04 = bnr.ReadInt32();
                sTQREvent.UnknownValue08 = bnr.ReadInt32();
                sTQREvent.UnknownValue0C = bnr.ReadInt32();
                sTQREvent.UnknownValue10 = bnr.ReadSByte();
                sTQREvent.UnknownValue11 = bnr.ReadSByte();
                sTQREvent.UnknownValue12 = bnr.ReadInt16();
                sTQREvent.UnknownValue14 = bnr.ReadInt16();
                sTQREvent.UnknownValue16 = bnr.ReadInt16();
                sTQREvent.UnknownValue18 = bnr.ReadSByte();
                sTQREvent.UnknownValue19 = bnr.ReadSByte();
                sTQREvent.UnknownValue1A = bnr.ReadSByte();
                sTQREvent.UnknownValue1B = bnr.ReadSByte();
                sTQREvent.UnknownValue1C = bnr.ReadSByte();
                sTQREvent.UnknownValue1D = bnr.ReadInt16();
                sTQREvent.UnknownValue1F = bnr.ReadSByte();
                sTQREvent.UnknownValue20 = bnr.ReadInt16();
                sTQREvent.UnknownValue22 = bnr.ReadInt16();
                sTQREvent.UnknownValue24 = bnr.ReadSingle();
                sTQREvent.UnknownValue28 = bnr.ReadSingle();
                sTQREvent.UnknownValue2C = bnr.ReadInt32();
                sTQREvent.UnknownValue30 = bnr.ReadInt32();
                sTQREvent.UnknownValue34 = bnr.ReadInt16();
                sTQREvent.UnknownValue34 = bnr.ReadInt16();
                sTQREvent.UnknownValue38 = bnr.ReadInt32();
                sTQREvent.UnknownValue3C = bnr.ReadInt32();
                sTQREvent.UnknownValue40 = bnr.ReadInt32();
                sTQREvent.UnknownValue44 = bnr.ReadInt32();
                sTQREvent.UnknownValue48 = bnr.ReadInt32();
                sTQREvent.UnknownValue4C = bnr.ReadInt32();
                sTQREvent.UnknownValue50 = bnr.ReadInt32();
                sTQREvent.UnknownValue54 = bnr.ReadInt32();
                sTQREvent.UnknownValue58 = bnr.ReadInt32();
                sTQREvent.PossibleEntryID = bnr.ReadInt32();
                sTQREvent.UnknownValue60 = bnr.ReadInt32();
                sTQREvent.UnknownValue64 = bnr.ReadSingle();
                sTQREvent.UnknownValue68 = bnr.ReadInt32();
                sTQREvent.UnknownValue6C = bnr.ReadInt32();
                sTQREvent.UnknownValue70 = bnr.ReadInt16();
                sTQREvent.UnknownValue72 = bnr.ReadInt16();
                sTQREvent.UnknownValue74 = bnr.ReadInt16();
                sTQREvent.UnknownValue76 = bnr.ReadInt16();
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

            //Builds a new STQR File to replace the uncompressed and compressed data variables.
            List<byte> newSTQRData = new List<byte>();
            List<byte> TempSTQRData = new List<byte>();
            List<string> FileNames = new List<string>();
            int MiscTemp = 0;
            long StartOfEvents;
            long StartOfFilenames;
            byte[] TerTemp = { 0x00 };

            //Hash.
            byte[] SQTRTemp = new byte[4] { 0x53, 0x54, 0x51, 0x52 };
            newSTQRData.AddRange(SQTRTemp);

            //Version.
            MiscTemp = 4;
            newSTQRData.AddRange(BitConverter.GetBytes(4));

            string STemp = "";
            string HexTemp = "";

            //Entry Count.
            int counta = stqrentry.EntryList.Count;
            newSTQRData.AddRange(BitConverter.GetBytes(counta));

            //Event Count.
            counta = stqrentry.Events.Count;
            newSTQRData.AddRange(BitConverter.GetBytes(counta));

            //Filler for pointer table that points to start of data sections.
            SQTRTemp = new byte[16];
            for (int v = 0; v < SQTRTemp.Length; v++)
            {
                SQTRTemp[v] = 0x00;
            }
            newSTQRData.AddRange(SQTRTemp);

            //Start of Entries.
            long PTemp = 80;
            newSTQRData.AddRange(BitConverter.GetBytes(PTemp));

            //Projected Start of Events.
            PTemp = (96 * stqrentry.EntryList.Count);
            StartOfEvents = PTemp;
            newSTQRData.AddRange(BitConverter.GetBytes(PTemp));

            //Projected Start of Filenames.
            PTemp = (50 + (96 * stqrentry.EntryList.Count) + (stqrentry.Events.Count * 152));
            PTemp = ((int)Math.Round(PTemp / 16.0, MidpointRounding.AwayFromZero) * 16);
            StartOfFilenames = PTemp;

            //This is stored 4 times for whatever reason so... yeah.
            newSTQRData.AddRange(BitConverter.GetBytes(PTemp));
            newSTQRData.AddRange(BitConverter.GetBytes(PTemp));
            newSTQRData.AddRange(BitConverter.GetBytes(PTemp));
            newSTQRData.AddRange(BitConverter.GetBytes(PTemp));

            long PreviousLengths = 0;

            //Entries.
            for (int w = 0; w < stqrentry.EntryList.Count; w++)
            {
                //Filename Position.
                PTemp = (StartOfFilenames + (PreviousLengths));
                newSTQRData.AddRange(BitConverter.GetBytes(PTemp));

                //Filler for unknown parameter.
                SQTRTemp = new byte[4];
                for (int v = 0; v < SQTRTemp.Length; v++)
                {
                    SQTRTemp[v] = 0x00;
                }
                newSTQRData.AddRange(SQTRTemp);

                //File Size of file referenced in Entry and Event.
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.EntryList[w].FileSize));

                //Duration in Hz.
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.EntryList[w].Duration));

                //Channel Count.
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.EntryList[w].Channels));

                //Sample Rate.
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.EntryList[w].SampleRate));

                //Loop Start.
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.EntryList[w].LoopStart));

                //Loop End.
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.EntryList[w].LoopEnd));

                //For the typehash.
                byte[] SQTRTempDX = ByteUtilitarian.StringToByteArray(stqrentry.EntryList[w].EntryTypeHash);
                Array.Reverse(SQTRTempDX);
                if (SQTRTempDX.Length < 4)
                {
                    byte[] PartHash = new byte[] { };
                    PartHash = SQTRTempDX;
                    Array.Resize(ref SQTRTempDX, 4);
                }

                newSTQRData.AddRange(SQTRTempDX);

                //SpecialFlag.
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.EntryList[w].UnknownDataB));

                PreviousLengths = PreviousLengths + (stqrentry.EntryList[w].EntryFileName.Length + 1);
                FileNames.Add(stqrentry.EntryList[w].EntryFileName);


            }

            //Events. Hoo boy.
            for (int w = 0; w < stqrentry.Events.Count; w++)
            {
                //All the variables.
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].EventEntryID));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue02));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue04));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue08));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue0C));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue10));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue11));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue12));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue14));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue16));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue18));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue19));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue1A));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue1B));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue1C));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue1D));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue1F));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue20));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue22));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue24));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue28));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue2C));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue30));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue34));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue36));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue38));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue3C));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue40));

                //Visual Break for my eyes. 5C is an entry that likely corresponds to the entry this event applies to.
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue44));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue48));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue4C));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue50));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue54));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue58));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].PossibleEntryID));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue60));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue64));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue68));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue6C));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue70));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue72));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue74));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue76));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue78));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue7C));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue80));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue84));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue88));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue8C));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue90));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue94));

            }

            //And Finally The Filenames.
            for (int x = 0; x < FileNames.Count; x++)
            {
                List<byte> writenamedata = new List<byte>();
                byte[] namebuffer = Encoding.ASCII.GetBytes(FileNames[x]);

                for (int l = 0; l < namebuffer.Length; ++l)
                {
                    writenamedata.Add(namebuffer[l]);
                }

                writenamedata.AddRange(TerTemp);

            }


            

            stqrentry.CompressedData = Zlibber.Compressor(stqrentry.UncompressedData);
            

            return stqrentry;

        }

        public static STQREntry SaveSTQREntry(STQREntry stqrentry, TreeNode node, ThreeSourceTree TreeSource)
        {


            //Builds a new STQR File to replace the uncompressed and compressed data variables.
            List<byte> newSTQRData = new List<byte>();
            List<byte> TempSTQRData = new List<byte>();
            List<string> FileNames = new List<string>();
            int MiscTemp = 0;
            long StartOfEvents;
            long StartOfFilenames;
            byte[] TerTemp = { 0x00 };

            //Hash.
            byte[] SQTRTemp = new byte[4] { 0x53, 0x54, 0x51, 0x52 };
            newSTQRData.AddRange(SQTRTemp);

            //Version.
            MiscTemp = 4;
            newSTQRData.AddRange(BitConverter.GetBytes(4));

            string STemp = "";
            string HexTemp = "";

            //Entry Count.
            int counta = stqrentry.EntryList.Count;
            newSTQRData.AddRange(BitConverter.GetBytes(counta));

            //Event Count.
            counta = stqrentry.Events.Count;
            newSTQRData.AddRange(BitConverter.GetBytes(counta));

            //Filler for pointer table that points to start of data sections.
            SQTRTemp = new byte[16];
            for (int v = 0; v < SQTRTemp.Length; v++)
            {
                SQTRTemp[v] = 0x00;
            }
            newSTQRData.AddRange(SQTRTemp);

            //Start of Entries.
            long PTemp = 80;
            newSTQRData.AddRange(BitConverter.GetBytes(PTemp));

            //Projected Start of Events.
            PTemp = (96 * stqrentry.EntryList.Count);
            StartOfEvents = PTemp;
            newSTQRData.AddRange(BitConverter.GetBytes(PTemp));

            //Projected Start of Filenames.
            PTemp = (50 + (96 * stqrentry.EntryList.Count) + (stqrentry.Events.Count * 152));
            PTemp = ((int)Math.Round(PTemp / 16.0, MidpointRounding.AwayFromZero) * 16);
            StartOfFilenames = PTemp;

            //This is stored 4 times for whatever reason so... yeah.
            newSTQRData.AddRange(BitConverter.GetBytes(PTemp));
            newSTQRData.AddRange(BitConverter.GetBytes(PTemp));
            newSTQRData.AddRange(BitConverter.GetBytes(PTemp));
            newSTQRData.AddRange(BitConverter.GetBytes(PTemp));

            long PreviousLengths = 0;

            //Entries.
            for (int w = 0; w < stqrentry.EntryList.Count; w++)
            {
                //Filename Position.
                PTemp = (StartOfFilenames + (PreviousLengths));
                newSTQRData.AddRange(BitConverter.GetBytes(PTemp));

                //Filler for unknown parameter.
                SQTRTemp = new byte[4];
                for (int v = 0; v < SQTRTemp.Length; v++)
                {
                    SQTRTemp[v] = 0x00;
                }
                newSTQRData.AddRange(SQTRTemp);

                //File Size of file referenced in Entry and Event.
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.EntryList[w].FileSize));

                //Duration in Hz.
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.EntryList[w].Duration));

                //Channel Count.
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.EntryList[w].Channels));

                //Sample Rate.
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.EntryList[w].SampleRate));

                //Loop Start.
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.EntryList[w].LoopStart));

                //Loop End.
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.EntryList[w].LoopEnd));

                //For the typehash.
                byte[] SQTRTempDX = ByteUtilitarian.StringToByteArray(stqrentry.EntryList[w].EntryTypeHash);
                Array.Reverse(SQTRTempDX);
                if (SQTRTempDX.Length < 4)
                {
                    byte[] PartHash = new byte[] { };
                    PartHash = SQTRTempDX;
                    Array.Resize(ref SQTRTempDX, 4);
                }

                newSTQRData.AddRange(SQTRTempDX);

                //SpecialFlag.
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.EntryList[w].UnknownDataB));

                PreviousLengths = PreviousLengths + (stqrentry.EntryList[w].EntryFileName.Length + 1);
                FileNames.Add(stqrentry.EntryList[w].EntryFileName);


            }

            //Events. Hoo boy.
            for (int w = 0; w < stqrentry.Events.Count; w++)
            {
                //All the variables.
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].EventEntryID));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue02));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue04));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue08));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue0C));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue10));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue11));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue12));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue14));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue16));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue18));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue19));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue1A));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue1B));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue1C));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue1D));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue1F));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue20));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue22));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue24));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue28));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue2C));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue30));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue34));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue36));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue38));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue3C));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue40));

                //Visual Break for my eyes. 5C is an entry that likely corresponds to the entry this event applies to.
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue44));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue48));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue4C));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue50));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue54));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue58));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].PossibleEntryID));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue60));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue64));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue68));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue6C));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue70));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue72));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue74));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue76));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue78));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue7C));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue80));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue84));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue88));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue8C));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue90));
                newSTQRData.AddRange(BitConverter.GetBytes(stqrentry.Events[w].UnknownValue94));

            }

            //And Finally The Filenames.
            for (int x = 0; x < FileNames.Count; x++)
            {
                List<byte> writenamedata = new List<byte>();
                byte[] namebuffer = Encoding.ASCII.GetBytes(FileNames[x]);

                for (int l = 0; l < namebuffer.Length; ++l)
                {
                    writenamedata.Add(namebuffer[l]);
                }

                writenamedata.AddRange(TerTemp);

            }




            stqrentry.CompressedData = Zlibber.Compressor(stqrentry.UncompressedData);


            return stqrentry;

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
