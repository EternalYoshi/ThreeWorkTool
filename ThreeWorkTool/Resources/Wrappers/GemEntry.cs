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

namespace ThreeWorkTool.Resources.Wrappers
{


    public class GemEntry : DefaultWrapper
    {
        public string Magic;
        public int Constant;
        public int EntryCountA;
        public int EntryCountB;
        public int EntryCountTotal;
        public byte[] WTemp;
        public int InterpolatedFileSize;
        public List<string> TextBackup;
        public List<GEMEntries> EntryList;

        public struct GEMEntries
        {
            public string TotalName;
        }

        public static GemEntry FillGEMEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
        {

            GemEntry GEMentry = new GemEntry();

            FillEntry(filename, subnames, tree, br, c, ID, GEMentry, filetype);

            GEMentry._FileLength = GEMentry.DSize;

            ASCIIEncoding ascii = new ASCIIEncoding();

            GEMentry.UncompressedData = ZlibStream.UncompressBuffer(GEMentry.CompressedData);

            //Type Specific Work Here.
            using (MemoryStream LmtStream = new MemoryStream(GEMentry.UncompressedData))
            {
                using (BinaryReader bnr = new BinaryReader(LmtStream))
                {
                    BuildGemEntry(bnr, GEMentry, ascii);
                }
            }

            return GEMentry;

        }

        public static GemEntry BuildGemEntry(BinaryReader bnr, GemEntry gem, ASCIIEncoding ascii)
        {

            //Specific file type work goes here!
            gem.Magic = BitConverter.ToString(gem.UncompressedData, 0, 4).Replace("-", string.Empty);
            bnr.BaseStream.Position = 4;
            gem.EntryCountA = bnr.ReadByte();
            gem.EntryCountB = bnr.ReadByte();
            gem.Constant = bnr.ReadInt16();
            gem.EntryCountTotal = gem.EntryCountA + gem.EntryCountB + 1;
            gem.InterpolatedFileSize = bnr.ReadInt32();

            //Starts occupying the entry list via structs. 
            gem.EntryList = new List<GEMEntries>();
            byte[] PLName = new byte[] { };
            byte[] PTHName = new byte[] { };
            int p = 24;

            for (int g = 0; g < gem.EntryCountTotal; g++)
            {
                GEMEntries ge = new GEMEntries();
                PLName = gem.UncompressedData.Skip(p).Take(64).Where(x => x != 0x00).ToArray();
                ge.TotalName = ascii.GetString(PLName);
                gem.EntryList.Add(ge);
                p = p + 80;
            }

            gem.TextBackup = new List<string>();
            return gem;

        }

        public static TextBox LoadGEMInTextBox(TextBox texbox, GemEntry geme)
        {

            texbox.Text = "";

            bool isEmpty = !geme.TextBackup.Any();
            if (isEmpty)
            {
                for (int t = 0; t < geme.EntryList.Count; t++)
                {
                    texbox.Text = texbox.Text + geme.EntryList[t].TotalName + System.Environment.NewLine;
                    geme.TextBackup.Add(geme.EntryList[t].TotalName + System.Environment.NewLine);
                }
            }
            else
            {

                for (int t = 0; t < geme.TextBackup.Count; t++)
                {
                    texbox.Text = texbox.Text + geme.TextBackup[t];
                }
                /*
                for (int t = 0; t < rple.TextBackup.Count; t++)
                {
                    texbox.Text = texbox.Text + rple.TextBackup[t];
                }
                */
            }



            return texbox;
        }

        public static GemEntry RenewGemEntry(TextBox texbox, GemEntry gem)
        {

            //Reconstructs the Entry List.
            string txbtxt = texbox.Text;
            string[] SPLT = new string[] { };

            SPLT = txbtxt.Split('\n');

            gem.EntryCountTotal = SPLT.Length;
            gem.InterpolatedFileSize = gem.EntryCountTotal * 64;
            gem.EntryList = new List<GEMEntries>();

            for (int g = 0; g < gem.EntryCountTotal; g++)
            {
                GEMEntries pe = new GEMEntries();
                pe.TotalName = SPLT[g];
                gem.EntryList.Add(pe);
            }

            //Rebuilds the Decompressed data Array.
            List<byte> NEWGEM = new List<byte>();
            byte[] HeaderGEM = { 0x47, 0x45, 0x4D, 0x00, 0x01, 0x01, 0xFE, 0xFF };
            NEWGEM.AddRange(HeaderGEM);
            int NewEntryCount = gem.EntryCountTotal;

            //Converts an integer to 4 bytes in a roundabout way.
            string GEMSize = 192.ToString("X8");
            byte[] GEMProjSize = new byte[4];
            GEMProjSize = ByteUtilitarian.StringToByteArray(GEMSize);
            Array.Reverse(GEMProjSize);

            NEWGEM.AddRange(GEMProjSize);

            string ENTemp = "";
            //string RPTemp = "";
            //string HashStr = "";
            byte[] HashTempDX = new byte[4];
            int counter = gem.EntryCountTotal - 1;

            for (int k = 0; k < counter; k++)
            {

                ENTemp = gem.EntryList[k].TotalName;
                ENTemp = ENTemp.Replace("\r", "");
                int NumberChars = ENTemp.Length;
                byte[] namebuffer = Encoding.ASCII.GetBytes(ENTemp);
                int nblength = namebuffer.Length;

                //Space for name is 64 bytes so we make a byte array with that size and then inject the name data in it.
                byte[] writenamedata = new byte[64];
                Array.Clear(writenamedata, 0, writenamedata.Length);

                for (int i = 0; i < namebuffer.Length; ++i)
                {
                    writenamedata[i] = namebuffer[i];
                }

                NEWGEM.AddRange(writenamedata);
            }

            gem.UncompressedData = NEWGEM.ToArray();
            gem.DSize = gem.UncompressedData.Length;
            gem.CompressedData = Zlibber.Compressor(gem.UncompressedData);
            gem.CSize = gem.CompressedData.Length;
            gem._FileLength = gem.UncompressedData.Length;

            return gem;

        }

        public static void UpdateGEMList(TextBox texbox, GemEntry geme)
        {
            string txbtxt = texbox.Text;
            string[] SPLT = new string[] { };
            if (texbox.Text != " ")
            {
                RefreshGEMList(texbox, geme);
            }

        }

        public static void RefreshGEMList(TextBox texbox, GemEntry geme)
        {
            //Reconstructs the Entry List.
            geme.EntryCountTotal = geme.TextBackup.Count;
            geme.EntryList = new List<GEMEntries>();

            for (int g = 0; g < geme.EntryCountTotal; g++)
            {
                GEMEntries pe = new GEMEntries();
                pe.TotalName = geme.TextBackup[g];
                geme.EntryList.Add(pe);
            }
            LoadGEMInTextBox(texbox, geme);
        }

        public static GemEntry InsertGEM(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {

            GemEntry gem = new GemEntry();

            InsertEntry(tree, node, filename, gem);

            //Decompression Time.
            gem.UncompressedData = ZlibStream.UncompressBuffer(gem.CompressedData);
            ASCIIEncoding ascii = new ASCIIEncoding();

            try
            {
                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
                {
                    BuildGemEntry(bnr, gem, ascii);
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText("Log.txt"))
                {
                    sw.WriteLine("Caught an exception using the BinaryReader. Here's the details:\n" + ex);
                }
            }

            return gem;

        }

        public static GemEntry ReplaceGEM(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {

            GemEntry gementry = new GemEntry();
            GemEntry oldentry = new GemEntry();

            tree.BeginUpdate();

            ReplaceEntry(tree, node, filename, gementry, oldentry);

            ASCIIEncoding ascii = new ASCIIEncoding();

            //Type Specific Work Here.
            using (MemoryStream LmtStream = new MemoryStream(gementry.UncompressedData))
            {
                using (BinaryReader bnr = new BinaryReader(LmtStream))
                {
                    BuildGemEntry(bnr, gementry, ascii);
                }
            }

            return node.entryfile as GemEntry;

        }



        #region Gem Properties

        [Category("Resource Path List"), ReadOnlyAttribute(true)]
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

        [Category("Resource Path List"), ReadOnlyAttribute(true)]
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

        private long _FileLength;
        [Category("Resource Path List"), ReadOnlyAttribute(true)]
        public long FileLength
        {

            get
            {
                return _FileLength;
            }
            set
            {
                _FileLength = value;
            }
        }

        [Category("Resource Path List"), ReadOnlyAttribute(true)]
        public int EntryTotalA
        {
            get
            {
                return EntryCountA;
            }
            set
            {
                EntryCountA = value;
            }
        }

        #endregion


    }


}
