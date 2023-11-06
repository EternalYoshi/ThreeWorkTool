using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Utility;
using ThreeWorkTool.Resources.Archives;
using ThreeWorkTool.Resources.Wrappers.ExtraNodes;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class StageObjLayoutEntry : DefaultWrapper
    {
        public string Magic;
        public int unknown04;
        public int EntryCount;
        public int InterpolatedFileSize;
        public List<StageObjLayoutGroup> Groups;

        public static StageObjLayoutEntry FillSLOEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
        {
            StageObjLayoutEntry sloentry = new StageObjLayoutEntry();
            List<byte> BTemp = new List<byte>();

            FillEntry(filename, subnames, tree, br, c, ID, sloentry, filetype);

            //Decompression Time.
            sloentry.UncompressedData = ZlibStream.UncompressBuffer(sloentry.CompressedData);

            //Type Specific Work Here.
            using (MemoryStream LmtStream = new MemoryStream(sloentry.UncompressedData))
            {
                using (BinaryReader bnr = new BinaryReader(LmtStream))
                {
                    BuildSLOEntry(bnr, sloentry);
                }
            }

            return sloentry;
        }

        public static StageObjLayoutEntry BuildSLOEntry(BinaryReader bnr, StageObjLayoutEntry sloentry)
        {

            //Header.
            sloentry.Magic = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), sloentry.Magic);
            sloentry.unknown04 = bnr.ReadInt32();
            sloentry.EntryCount = bnr.ReadInt32();
            sloentry.InterpolatedFileSize = bnr.ReadInt32();

            sloentry.Groups = new List<StageObjLayoutGroup>();

            //GroupBox Entries.
            for (int i = 0; i < sloentry.EntryCount; i++)
            {
                StageObjLayoutGroup slg = new StageObjLayoutGroup();
                slg = StageObjLayoutGroup.BuildSLOGroup(sloentry, i, bnr, slg);
                sloentry.Groups.Add(slg);
            }

            return sloentry;

        }

        public static StageObjLayoutEntry ReplaceSLOEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            StageObjLayoutEntry sloentry = new StageObjLayoutEntry();
            StageObjLayoutEntry oldentry = new StageObjLayoutEntry();

            tree.BeginUpdate();

            ReplaceEntry(tree, node, filename, sloentry, oldentry);
            sloentry.FileName = sloentry.TrueName;

            //Type Specific Work Here.
            using (MemoryStream LmtStream = new MemoryStream(sloentry.UncompressedData))
            {
                using (BinaryReader bnr = new BinaryReader(LmtStream))
                {
                    BuildSLOEntry(bnr, sloentry);
                }
            }

            return node.entryfile as StageObjLayoutEntry;
        }

        public static StageObjLayoutEntry InsertSLOEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            StageObjLayoutEntry slo = new StageObjLayoutEntry();

            InsertEntry(tree, node, filename, slo);

            //Decompression Time.
            slo.UncompressedData = ZlibStream.UncompressBuffer(slo.CompressedData);

            try
            {
                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
                {
                    BuildSLOEntry(bnr, slo);
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

            return slo;
        }

        public static StageObjLayoutEntry RebuildSLOEntry(StageObjLayoutEntry sloentry)
        {

            List<byte> NewBuffer = new List<byte>();
            byte[] HeaderThing = {0x53, 0x4C, 0x4F, 0x00, 0x08, 0x01, 0xFE, 0xFF};
            NewBuffer.AddRange(HeaderThing);
            NewBuffer.AddRange(BitConverter.GetBytes(sloentry.EntryCount));
            NewBuffer.AddRange(BitConverter.GetBytes(sloentry.InterpolatedFileSize));

            //Now for the subentries. At offset 0x10.
            for(int i = 0; i < sloentry.EntryCount; i++)
            {
                NewBuffer.AddRange(BitConverter.GetBytes(sloentry.Groups[i].UnknownFlags));
                //32-Bit string.
                //Space for name is 32 bytes so we make a byte array with that size and then inject the name data in it.
                byte[] namebuffer = Encoding.ASCII.GetBytes(sloentry.Groups[i].PossibleGroupName);
                byte[] writenamedata = new byte[32];
                Array.Clear(writenamedata, 0, writenamedata.Length);

                for (int j = 0; j < namebuffer.Length; ++j)
                {
                    writenamedata[j] = namebuffer[j];
                }

                NewBuffer.AddRange(writenamedata);

                NewBuffer.AddRange(sloentry.Groups[i].BufferA);

                NewBuffer.AddRange(BitConverter.GetBytes(sloentry.Groups[i].VectorA.X));
                NewBuffer.AddRange(BitConverter.GetBytes(sloentry.Groups[i].VectorA.Y));
                NewBuffer.AddRange(BitConverter.GetBytes(sloentry.Groups[i].VectorA.Z));
                NewBuffer.AddRange(BitConverter.GetBytes(sloentry.Groups[i].SomeFloat1));

                NewBuffer.AddRange(BitConverter.GetBytes(sloentry.Groups[i].VectorB.X));
                NewBuffer.AddRange(BitConverter.GetBytes(sloentry.Groups[i].VectorB.Y));
                NewBuffer.AddRange(BitConverter.GetBytes(sloentry.Groups[i].VectorB.Z));
                NewBuffer.AddRange(BitConverter.GetBytes(sloentry.Groups[i].SomeFloat2));

                NewBuffer.AddRange(BitConverter.GetBytes(sloentry.Groups[i].VectorC.X));
                NewBuffer.AddRange(BitConverter.GetBytes(sloentry.Groups[i].VectorC.Y));
                NewBuffer.AddRange(BitConverter.GetBytes(sloentry.Groups[i].VectorC.Z));
                NewBuffer.AddRange(BitConverter.GetBytes(sloentry.Groups[i].SomeFloat3));

                //To The FileReferences.
                //Space for name is 64 bytes so we make a byte array with that size and then inject the name data in it.
                byte[] namebuffer1 = Encoding.ASCII.GetBytes(sloentry.Groups[i].FileReference1);
                byte[] writenamedata1 = new byte[64];
                Array.Clear(writenamedata1, 0, writenamedata1.Length);

                for (int k = 0; k < namebuffer1.Length; ++k)
                {
                    writenamedata1[k] = namebuffer1[k];
                }

                NewBuffer.AddRange(writenamedata1);

                //Space for name is 64 bytes so we make a byte array with that size and then inject the name data in it.
                byte[] namebuffer2 = Encoding.ASCII.GetBytes(sloentry.Groups[i].FileReference2);
                byte[] writenamedata2 = new byte[64];
                Array.Clear(writenamedata2, 0, writenamedata2.Length);

                for (int k = 0; k < namebuffer2.Length; ++k)
                {
                    writenamedata2[k] = namebuffer2[k];
                }

                NewBuffer.AddRange(writenamedata2);

                //Space for name is 64 bytes so we make a byte array with that size and then inject the name data in it.
                byte[] namebuffer3 = Encoding.ASCII.GetBytes(sloentry.Groups[i].FileReference3);
                byte[] writenamedata3 = new byte[64];
                Array.Clear(writenamedata3, 0, writenamedata3.Length);

                for (int k = 0; k < namebuffer3.Length; ++k)
                {
                    writenamedata3[k] = namebuffer3[k];
                }

                NewBuffer.AddRange(writenamedata3);

                //Space for name is 64 bytes so we make a byte array with that size and then inject the name data in it.
                byte[] namebuffer4 = Encoding.ASCII.GetBytes(sloentry.Groups[i].FileReference4);
                byte[] writenamedata4 = new byte[64];
                Array.Clear(writenamedata4, 0, writenamedata4.Length);

                for (int k = 0; k < namebuffer4.Length; ++k)
                {
                    writenamedata4[k] = namebuffer4[k];
                }

                NewBuffer.AddRange(writenamedata4);

                //Space for name is 64 bytes so we make a byte array with that size and then inject the name data in it.
                byte[] namebuffer5 = Encoding.ASCII.GetBytes(sloentry.Groups[i].FileReference5);
                byte[] writenamedata5 = new byte[64];
                Array.Clear(writenamedata5, 0, writenamedata5.Length);

                for (int k = 0; k < namebuffer5.Length; ++k)
                {
                    writenamedata5[k] = namebuffer5[k];
                }

                NewBuffer.AddRange(writenamedata5);

                //Space for name is 64 bytes so we make a byte array with that size and then inject the name data in it.
                byte[] namebuffer6 = Encoding.ASCII.GetBytes(sloentry.Groups[i].FileReference6);
                byte[] writenamedata6 = new byte[64];
                Array.Clear(writenamedata6, 0, writenamedata6.Length);

                for (int k = 0; k < namebuffer6.Length; ++k)
                {
                    writenamedata6[k] = namebuffer6[k];
                }

                NewBuffer.AddRange(writenamedata6);

                //Space for name is 64 bytes so we make a byte array with that size and then inject the name data in it.
                byte[] namebuffer7 = Encoding.ASCII.GetBytes(sloentry.Groups[i].FileReference7);
                byte[] writenamedata7 = new byte[64];
                Array.Clear(writenamedata7, 0, writenamedata7.Length);

                for (int k = 0; k < namebuffer7.Length; ++k)
                {
                    writenamedata7[k] = namebuffer7[k];
                }

                NewBuffer.AddRange(writenamedata7);

                NewBuffer.AddRange(sloentry.Groups[i].BufferFooter);


            }


            sloentry.UncompressedData = NewBuffer.ToArray();
            sloentry.CompressedData = Zlibber.Compressor(sloentry.UncompressedData);
            return sloentry;

        }

        public static StageObjLayoutEntry SaveSLOEntry(StageObjLayoutEntry sloentry, TreeNode node)
        {

            using (MemoryStream SloStream = new MemoryStream(sloentry.UncompressedData))
            {
                using (BinaryWriter bwr = new BinaryWriter(SloStream))
                {
                    //Gets All The StageObjLayoutGroup Files From Child Nodes variables and writes to the main StageObjLayout file.
                    foreach (ArcEntryWrapper youngn in node.Nodes)
                    {
                        StageObjLayoutGroup slog = youngn.Tag as StageObjLayoutGroup;
                        bwr.BaseStream.Position = slog.DataOffset;
                        bwr.BaseStream.Position = bwr.BaseStream.Position + 48;
                        bwr.Write(slog.VectorA.X);
                        bwr.Write(slog.VectorA.Y);
                        bwr.Write(slog.VectorA.Z);
                        bwr.Write(slog.SomeFloat1);
                        bwr.Write(slog.VectorB.X);
                        bwr.Write(slog.VectorB.Y);
                        bwr.Write(slog.VectorB.Z);
                        bwr.Write(slog.SomeFloat2);
                        bwr.Write(slog.VectorC.X);
                        bwr.Write(slog.VectorC.Y);
                        bwr.Write(slog.VectorC.Z);
                        bwr.Write(slog.SomeFloat3);

                        //Now for the file references.
                        int NumberChars = slog.FileReference1.Length;
                        byte[] namebuffer = Encoding.ASCII.GetBytes(slog.FileReference1);
                        int nblength = namebuffer.Length;
                        //Space for name is 64 bytes so we make a byte array with that size and then inject the name data in it.
                        byte[] writenamedata = new byte[64];
                        Array.Clear(writenamedata, 0, writenamedata.Length);
                        for (int i = 0; i < namebuffer.Length; ++i)
                        {
                            writenamedata[i] = namebuffer[i];
                        }
                        bwr.Write(writenamedata, 0, writenamedata.Length);

                        namebuffer = Encoding.ASCII.GetBytes(slog.FileReference2);
                        nblength = namebuffer.Length;
                        //Space for name is 64 bytes so we make a byte array with that size and then inject the name data in it.
                        writenamedata = new byte[64];
                        Array.Clear(writenamedata, 0, writenamedata.Length);

                        for (int i = 0; i < namebuffer.Length; ++i)
                        {
                            writenamedata[i] = namebuffer[i];
                        }

                        bwr.Write(writenamedata, 0, writenamedata.Length);

                        namebuffer = Encoding.ASCII.GetBytes(slog.FileReference3);
                        nblength = namebuffer.Length;
                        //Space for name is 64 bytes so we make a byte array with that size and then inject the name data in it.
                        writenamedata = new byte[64];
                        Array.Clear(writenamedata, 0, writenamedata.Length);

                        for (int i = 0; i < namebuffer.Length; ++i)
                        {
                            writenamedata[i] = namebuffer[i];
                        }

                        bwr.Write(writenamedata, 0, writenamedata.Length);

                        namebuffer = Encoding.ASCII.GetBytes(slog.FileReference4);
                        nblength = namebuffer.Length;
                        //Space for name is 64 bytes so we make a byte array with that size and then inject the name data in it.
                        writenamedata = new byte[64];
                        Array.Clear(writenamedata, 0, writenamedata.Length);

                        for (int i = 0; i < namebuffer.Length; ++i)
                        {
                            writenamedata[i] = namebuffer[i];
                        }

                        bwr.Write(writenamedata, 0, writenamedata.Length);

                        namebuffer = Encoding.ASCII.GetBytes(slog.FileReference5);
                        nblength = namebuffer.Length;
                        //Space for name is 64 bytes so we make a byte array with that size and then inject the name data in it.
                        writenamedata = new byte[64];
                        Array.Clear(writenamedata, 0, writenamedata.Length);

                        for (int i = 0; i < namebuffer.Length; ++i)
                        {
                            writenamedata[i] = namebuffer[i];
                        }

                        bwr.Write(writenamedata, 0, writenamedata.Length);

                        namebuffer = Encoding.ASCII.GetBytes(slog.FileReference6);
                        nblength = namebuffer.Length;
                        //Space for name is 64 bytes so we make a byte array with that size and then inject the name data in it.
                        writenamedata = new byte[64];
                        Array.Clear(writenamedata, 0, writenamedata.Length);

                        for (int i = 0; i < namebuffer.Length; ++i)
                        {
                            writenamedata[i] = namebuffer[i];
                        }

                        bwr.Write(writenamedata, 0, writenamedata.Length);

                        namebuffer = Encoding.ASCII.GetBytes(slog.FileReference7);
                        nblength = namebuffer.Length;
                        //Space for name is 64 bytes so we make a byte array with that size and then inject the name data in it.
                        writenamedata = new byte[64];
                        Array.Clear(writenamedata, 0, writenamedata.Length);

                        for (int i = 0; i < namebuffer.Length; ++i)
                        {
                            writenamedata[i] = namebuffer[i];
                        }

                        bwr.Write(writenamedata, 0, writenamedata.Length);




                    }


                }
            }


            sloentry.CompressedData = Zlibber.Compressor(sloentry.UncompressedData);


            return sloentry;

        }

        #region Stage Object Layout Properties

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

        [Category("Data"), ReadOnlyAttribute(true)]
        public int GroupCount
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

        [Category("Data"), ReadOnlyAttribute(true)]
        public int FileSize
        {

            get
            {
                return InterpolatedFileSize;
            }
            set
            {
                InterpolatedFileSize = value;
            }
        }

        #endregion

    }
}
