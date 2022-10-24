using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Archives;
using ThreeWorkTool.Resources.Utility;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class ShotListEntry : DefaultWrapper
    {
        //Header Stuff.
        public string Magic;
        public int Unknown04;
        public int InterpolatedFileSize;
        public int EntryCount;
        public List<string> TextBackup;
        public List<SHTEntry> Shots;

        //Entry Stuff.
        public struct SHTEntry
        {
            public string FullPath;
        }

        public static ShotListEntry FillShotListEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
        {
            ShotListEntry LSHentry = new ShotListEntry();

            FillEntry(filename, subnames, tree, br, c, ID, LSHentry, filetype);

            ASCIIEncoding ascii = new ASCIIEncoding();

            LSHentry.UncompressedData = ZlibStream.UncompressBuffer(LSHentry.CompressedData);

            //Type Specific Work Here.
            using (MemoryStream LmtStream = new MemoryStream(LSHentry.UncompressedData))
            {
                using (BinaryReader bnr = new BinaryReader(LmtStream))
                {
                    BuildShotListEntry(bnr, LSHentry, ascii);
                }
            }

            return LSHentry;
        }

        public static ShotListEntry BuildShotListEntry(BinaryReader bnr, ShotListEntry shl, ASCIIEncoding ascii)
        {
            //Getting Header Data.
            shl.TypeHash = "141D851F";
            shl.Magic = BitConverter.ToString(shl.UncompressedData, 0, 4).Replace("-", string.Empty);
            bnr.BaseStream.Position = bnr.BaseStream.Position + 4;
            shl.Unknown04 = bnr.ReadInt32();
            shl.InterpolatedFileSize = bnr.ReadInt32();
            shl.EntryCount = bnr.ReadInt32();

            //For the entries themselves.
            shl.Shots = new List<SHTEntry>();
            byte[] PLName = new byte[] { };
            int p = 24;

            for (int g = 0; g < shl.EntryCount; g++)
            {
                bnr.BaseStream.Position = bnr.BaseStream.Position + 8;
                SHTEntry sht = new SHTEntry();
                PLName = shl.UncompressedData.Skip(p).Take(64).Where(x => x != 0x00).ToArray();
                sht.FullPath = ascii.GetString(PLName);
                shl.Shots.Add(sht);
                bnr.BaseStream.Position = bnr.BaseStream.Position + 8;
                p = p + 80;
            }

            shl.TextBackup = new List<string>();

            return shl;

        }

        public static TextBox LoadLSHInTextBox(TextBox texbox, ShotListEntry lshe)
        {

            texbox.Text = "";

            bool isEmpty = !lshe.TextBackup.Any();
            if (isEmpty)
            {
                for (int t = 0; t < lshe.Shots.Count; t++)
                {
                    texbox.Text = texbox.Text + lshe.Shots[t].FullPath + System.Environment.NewLine;
                    lshe.TextBackup.Add(lshe.Shots[t].FullPath + System.Environment.NewLine);
                }
            }
            else
            {

                for (int t = 0; t < lshe.TextBackup.Count; t++)
                {
                    texbox.Text = texbox.Text + lshe.TextBackup[t];
                }

            }



            return texbox;
        }

        public static ShotListEntry RenewShotListEntry(TextBox texbox, ShotListEntry lshe)
        {

            //Reconstructs the Entry List.
            string txbtxt = texbox.Text;
            string[] SPLT = new string[] { };

            SPLT = txbtxt.Split('\n');

            lshe.EntryCount = SPLT.Length;
            lshe.Shots = new List<SHTEntry>();

            for (int g = 0; g < lshe.EntryCount; g++)
            {
                SHTEntry se = new SHTEntry();
                se.FullPath = SPLT[g];
                lshe.Shots.Add(se);
            }

            //Rebuilds the Decompressed data Array.
            List<byte> NEWSHT = new List<byte>();
            byte[] HeaderSHT = { 0x4C, 0x53, 0x48, 0x00, 0x02, 0x01, 0xFE, 0xFF };
            NEWSHT.AddRange(HeaderSHT);
            int NewEntryCount = lshe.EntryCount;

            if (string.IsNullOrWhiteSpace(lshe.Shots[(lshe.EntryCount - 1)].FullPath))
            {
                NewEntryCount--;
            }

            //File Size.
            int ProjectedSize = NewEntryCount * 80;
            lshe.InterpolatedFileSize = ProjectedSize;
            //Converts an integer to 4 bytes in a roundabout way.
            string SHTSize = ProjectedSize.ToString("X8");
            byte[] SHTProjSize = new byte[4];
            SHTProjSize = ByteUtilitarian.StringToByteArray(SHTSize);
            Array.Reverse(SHTProjSize);
            NEWSHT.AddRange(SHTProjSize);

            //Entry Count.
            byte[] SHTEntryTotal = new byte[4];
            SHTEntryTotal[3] = Convert.ToByte(NewEntryCount);
            Array.Reverse(SHTEntryTotal);
            NEWSHT.AddRange(SHTEntryTotal);

            string ENTemp = "";
            byte[] HashTempDX = new byte[4];
            byte[] Padding = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            //Starts building the entries.
            for (int k = 0; k < NewEntryCount; k++)
            {
                NEWSHT.AddRange(Padding);

                ENTemp = lshe.Shots[k].FullPath;
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

                NEWSHT.AddRange(writenamedata);

                NEWSHT.AddRange(Padding);

            }

            lshe.UncompressedData = NEWSHT.ToArray();
            lshe.DSize = lshe.UncompressedData.Length;
            lshe.CompressedData = Zlibber.Compressor(lshe.UncompressedData);
            lshe.CSize = lshe.CompressedData.Length;

            lshe.TextBackup = new List<string>();

            return lshe;

        }

        public static void UpdateShotList(TextBox texbox, ShotListEntry lshe)
        {
            string txbtxt = texbox.Text;
            string[] SPLT = new string[] { };
            if (texbox.Text != " ")
            {
                RefreshShotList(texbox, lshe);
            }
        }

        public static void RefreshShotList(TextBox texbox, ShotListEntry lshe)
        {

            //Reconstructs the Entry List.
            lshe.EntryCount = lshe.TextBackup.Count;
            lshe.Shots = new List<SHTEntry>();

            for (int g = 0; g < lshe.EntryCount; g++)
            {
                SHTEntry pe = new SHTEntry();
                pe.FullPath = lshe.TextBackup[g];
                lshe.Shots.Add(pe);
            }
            LoadLSHInTextBox(texbox, lshe);

        }

        public static ShotListEntry InsertShotListEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {

            ShotListEntry lsh = new ShotListEntry();

            InsertEntry(tree, node, filename, lsh);

            //Decompression Time.
            lsh.UncompressedData = ZlibStream.UncompressBuffer(lsh.CompressedData);
            ASCIIEncoding ascii = new ASCIIEncoding();

            try
            {
                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
                {
                    BuildShotListEntry(bnr, lsh, ascii);
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText("Log.txt"))
                {
                    sw.WriteLine("Caught an exception using the BinaryReader. Here's the details:\n" + ex);
                }
            }

            return lsh;
            
        }

        public static ShotListEntry ReplaceShotListEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {

            ShotListEntry gementry = new ShotListEntry();
            ShotListEntry oldentry = new ShotListEntry();

            tree.BeginUpdate();

            ReplaceEntry(tree, node, filename, gementry, oldentry);

            ASCIIEncoding ascii = new ASCIIEncoding();

            //Type Specific Work Here.
            using (MemoryStream LmtStream = new MemoryStream(gementry.UncompressedData))
            {
                using (BinaryReader bnr = new BinaryReader(LmtStream))
                {
                    BuildShotListEntry(bnr, gementry, ascii);
                }
            }

            return node.entryfile as ShotListEntry;

        }

        #region ShotList Properties

        [Category("Shot List"), ReadOnlyAttribute(true)]
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

        [Category("Shot List"), ReadOnlyAttribute(true)]
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

        [Category("Resource Path List"), ReadOnlyAttribute(true)]
        public int FileLength
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

        [Category("Shot List"), ReadOnlyAttribute(true)]
        public int EntryTotal
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

        #endregion

    }
}
