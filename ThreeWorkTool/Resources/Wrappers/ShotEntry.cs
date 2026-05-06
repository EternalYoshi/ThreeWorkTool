using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Archives;
using ThreeWorkTool.Resources.Utility;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class ShotEntry : DefaultWrapper
    {
        public string Magic;

        [Category("Shot File"), ReadOnlyAttribute(false)]
        public string InternalShotName { get; set; } //0x0C

        [Category("Shot File"), ReadOnlyAttribute(false)]
        public string InternalShotPath { get; set; } //0x300

        [Category("Shot File"), ReadOnlyAttribute(false)]
        public string InternalAnmPath { get; set; } //0x214

        [Category("Shot File"), ReadOnlyAttribute(false)]
        public string Internal2ndShotPath { get; set; } //0x13C

        [Category("Shot File"), ReadOnlyAttribute(false)]
        public string Internal3rdShotPath { get; set; } //0x17C

        [Category("Shot File"), ReadOnlyAttribute(false)]
        public string Internal4thShotPath { get; set; } //0x1BC

        public static ShotEntry FillShotEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
        {

            ShotEntry shtentry = new ShotEntry();
            List<byte> BTemp = new List<byte>();

            FillEntry(filename, subnames, tree, br, c, ID, shtentry, filetype);

            //Decompression Time.
            shtentry.UncompressedData = ZlibStream.UncompressBuffer(shtentry.CompressedData);

            //Type Specific Work Here.
            using (MemoryStream LmtStream = new MemoryStream(shtentry.UncompressedData))
            {
                using (BinaryReader bnr = new BinaryReader(LmtStream))
                {
                    BuildShotEntry(bnr, shtentry);
                }
            }

            shtentry._FileType = shtentry.FileExt;
            shtentry._FileName = shtentry.TrueName;
            shtentry._DecompressedFileLength = shtentry.UncompressedData.Length;
            shtentry._CompressedFileLength = shtentry.CompressedData.Length;

            return shtentry;

        }

        public static ShotEntry BuildShotEntry(BinaryReader bnr, ShotEntry shtentry)
        {
            //Header Magic.
            shtentry.Magic = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), shtentry.Magic);

            //Now for the strings.
            bnr.BaseStream.Position = 0xC;
            shtentry.InternalShotName = Encoding.ASCII.GetString(bnr.ReadBytes(32)).Trim('\0');

            bnr.BaseStream.Position = 0x13C;
            shtentry.Internal2ndShotPath = Encoding.ASCII.GetString(bnr.ReadBytes(64)).Trim('\0');

            bnr.BaseStream.Position = 0x17C;
            shtentry.Internal3rdShotPath = Encoding.ASCII.GetString(bnr.ReadBytes(64)).Trim('\0');

            bnr.BaseStream.Position = 0x1BC;
            shtentry.Internal4thShotPath = Encoding.ASCII.GetString(bnr.ReadBytes(64)).Trim('\0');

            bnr.BaseStream.Position = 0x214;
            shtentry.InternalAnmPath = Encoding.ASCII.GetString(bnr.ReadBytes(64)).Trim('\0');

            bnr.BaseStream.Position = 0x300;
            shtentry.InternalShotPath = Encoding.ASCII.GetString(bnr.ReadBytes(64)).Trim('\0');

            return shtentry;

        }

        public static ShotEntry ReplaceShotEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            ShotEntry shtentry = new ShotEntry();
            ShotEntry oldentry = new ShotEntry();

            tree.BeginUpdate();

            ReplaceEntry(tree, node, filename, shtentry, oldentry);
            shtentry.DecompressedFileLength = shtentry.UncompressedData.Length;
            shtentry._DecompressedFileLength = shtentry.UncompressedData.Length;
            shtentry.CompressedFileLength = shtentry.CompressedData.Length;
            shtentry._CompressedFileLength = shtentry.CompressedData.Length;
            shtentry._FileName = shtentry.TrueName;
            shtentry._FileType = shtentry.FileExt;
            shtentry.FileName = shtentry.TrueName;

            //Type Specific Work Here.
            using (MemoryStream LmtStream = new MemoryStream(shtentry.UncompressedData))
            {
                using (BinaryReader bnr = new BinaryReader(LmtStream))
                {
                    BuildShotEntry(bnr, shtentry);
                }
            }

            return node.entryfile as ShotEntry;
        }

        public static ShotEntry InsertShotEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            ShotEntry shtentry = new ShotEntry();

            InsertEntry(tree, node, filename, shtentry);

            shtentry.DecompressedFileLength = shtentry.UncompressedData.Length;
            shtentry._DecompressedFileLength = shtentry.UncompressedData.Length;
            shtentry.CompressedFileLength = shtentry.CompressedData.Length;
            shtentry._CompressedFileLength = shtentry.CompressedData.Length;
            shtentry._FileName = shtentry.TrueName;
            shtentry._FileType = shtentry.FileExt;
            shtentry.EntryName = shtentry.FileName;

            //Type Specific Work Here.
            try
            {
                using (MemoryStream LmtStream = new MemoryStream(shtentry.UncompressedData))
                {
                    using (BinaryReader bnr = new BinaryReader(LmtStream))
                    {
                        BuildShotEntry(bnr, shtentry);
                    }
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
            return shtentry;
        }

        public static ShotEntry SaveShotEntry(ShotEntry shtentry, TreeNode node)
        {

            using (MemoryStream ShtStream = new MemoryStream(shtentry.UncompressedData))
            {
                using (BinaryWriter bwr = new BinaryWriter(ShtStream))
                {

                    //Now for the strings.
                    bwr.BaseStream.Position = 0xC;
                    //32 characters.
                    int NumberChars = shtentry.InternalShotName.Length;
                    byte[] namebuffer = Encoding.ASCII.GetBytes(shtentry.InternalShotName);
                    int nblength = namebuffer.Length;
                    //Space for this name is 32 bytes so we make a byte array with that size and then inject the name data in it.
                    byte[] writenamedata = new byte[32];
                    Array.Clear(writenamedata, 0, writenamedata.Length);
                    for (int i = 0; i < namebuffer.Length; ++i)
                    {
                        writenamedata[i] = namebuffer[i];
                    }
                    bwr.Write(writenamedata, 0, writenamedata.Length);

                    bwr.BaseStream.Position = 0x13C;
                    NumberChars = shtentry.Internal2ndShotPath.Length;
                    namebuffer = Encoding.ASCII.GetBytes(shtentry.Internal2ndShotPath);
                    nblength = namebuffer.Length;
                    //Space for this name is 64 bytes so we make a byte array with that size and then inject the name data in it.
                    writenamedata = new byte[64];
                    Array.Clear(writenamedata, 0, writenamedata.Length);
                    for (int i = 0; i < namebuffer.Length; ++i)
                    {
                        writenamedata[i] = namebuffer[i];
                    }
                    bwr.Write(writenamedata, 0, writenamedata.Length);

                    bwr.BaseStream.Position = 0x17C;
                    NumberChars = shtentry.Internal3rdShotPath.Length;
                    namebuffer = Encoding.ASCII.GetBytes(shtentry.Internal3rdShotPath);
                    nblength = namebuffer.Length;
                    //Space for this name is 64 bytes so we make a byte array with that size and then inject the name data in it.
                    writenamedata = new byte[64];
                    Array.Clear(writenamedata, 0, writenamedata.Length);
                    for (int i = 0; i < namebuffer.Length; ++i)
                    {
                        writenamedata[i] = namebuffer[i];
                    }
                    bwr.Write(writenamedata, 0, writenamedata.Length);

                    bwr.BaseStream.Position = 0x1BC;
                    NumberChars = shtentry.Internal4thShotPath.Length;
                    namebuffer = Encoding.ASCII.GetBytes(shtentry.Internal4thShotPath);
                    nblength = namebuffer.Length;
                    //Space for this name is 64 bytes so we make a byte array with that size and then inject the name data in it.
                    writenamedata = new byte[64];
                    Array.Clear(writenamedata, 0, writenamedata.Length);
                    for (int i = 0; i < namebuffer.Length; ++i)
                    {
                        writenamedata[i] = namebuffer[i];
                    }
                    bwr.Write(writenamedata, 0, writenamedata.Length);

                    bwr.BaseStream.Position = 0x214;
                    NumberChars = shtentry.InternalAnmPath.Length;
                    namebuffer = Encoding.ASCII.GetBytes(shtentry.InternalAnmPath);
                    nblength = namebuffer.Length;
                    //Space for this name is 64 bytes so we make a byte array with that size and then inject the name data in it.
                    writenamedata = new byte[64];
                    Array.Clear(writenamedata, 0, writenamedata.Length);
                    for (int i = 0; i < namebuffer.Length; ++i)
                    {
                        writenamedata[i] = namebuffer[i];
                    }
                    bwr.Write(writenamedata, 0, writenamedata.Length);

                    bwr.BaseStream.Position = 0x300;
                    NumberChars = shtentry.InternalShotPath.Length;
                    namebuffer = Encoding.ASCII.GetBytes(shtentry.InternalShotPath);
                    nblength = namebuffer.Length;
                    //Space for this name is 64 bytes so we make a byte array with that size and then inject the name data in it.
                    writenamedata = new byte[64];
                    Array.Clear(writenamedata, 0, writenamedata.Length);
                    for (int i = 0; i < namebuffer.Length; ++i)
                    {
                        writenamedata[i] = namebuffer[i];
                    }
                    bwr.Write(writenamedata, 0, writenamedata.Length);

                }
            }


            shtentry.CompressedData = Zlibber.Compressor(shtentry.UncompressedData);


            return shtentry;

        }


        //public static ShotEntry RebuildShotEntry(ShotEntry shtentry)
        //{


        //}

        #region Shot Properties
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
