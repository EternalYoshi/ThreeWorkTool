using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Archives;
using ThreeWorkTool.Resources.Utility;
using ThreeWorkTool.Resources.Wrappers.ExtraNodes;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class SoundBankEntry : DefaultWrapper
    {

        //Header Stuff. Thanks to Rootz for this.
        public string Magic;
        public int FileVersion;
        public int SectionACount;
        public int SectionBCount;
        public int SectionCCount;
        public string FullPath;
        public List<SoundBankEntryPath> SoundFilePaths;
        //public List<string> SoundFilePaths;
        //public List<string> TypeHashes;
        //public List<List<byte>> OtherData;
        public List<long> Offsets;

        //.sbkr Footer: 0x00 0x00 0x80 0x3F 0x01 0x00 0x00 0x00

        public static SoundBankEntry FillSoundBankEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
        {

            SoundBankEntry sbkrentry = new SoundBankEntry();

            FillEntry(filename, subnames, tree, br, c, ID, sbkrentry, filetype);

            ASCIIEncoding ascii = new ASCIIEncoding();

            //Type Specific Work Here.
            using (MemoryStream LmtStream = new MemoryStream(sbkrentry.UncompressedData))
            {
                using (BinaryReader bnr = new BinaryReader(LmtStream))
                {
                    BuildSoundBankEntry(bnr, sbkrentry, ascii);
                }
            }

            //sbkrentry._FileType = sbkrentry.FileExt;
            //sbkrentry._FileName = sbkrentry.TrueName;
            //sbkrentry._DecompressedFileLength = sbkrentry.UncompressedData.Length;
            //sbkrentry._CompressedFileLength = sbkrentry.CompressedData.Length;

            return sbkrentry;

        }

        public static SoundBankEntry BuildSoundBankEntry(BinaryReader bnr, SoundBankEntry sbkr, ASCIIEncoding ascii)
        {
            sbkr.Magic = BitConverter.ToString(sbkr.UncompressedData, 0, 4).Replace("-", string.Empty);
            bnr.BaseStream.Position = bnr.BaseStream.Position + 4;
            sbkr.FileVersion = bnr.ReadInt32();
            sbkr.SectionACount = bnr.ReadInt32();
            sbkr.SectionBCount = bnr.ReadInt32();
            sbkr.SectionCCount = bnr.ReadInt32();

            sbkr.SoundFilePaths = new List<SoundBankEntryPath>();

            //Time to get the names.
            bnr.BaseStream.Position = 0x14 + sbkr.SectionACount * 0x18;

            //sbkr.SoundFilePaths = new List<string>();
            //sbkr.TypeHashes = new List<string>();
            //sbkr.OtherData = new List<List<byte>>();
            string Test = "";
            List<byte> PLName = new List<byte>();
            List<byte> PLHash = new List<byte>();
            sbkr.Offsets = new List<long>();
            byte B;
            long p = 0x14 + sbkr.SectionACount * 0x18;

            //for (int g = 0; g < sbkr.SectionBCount; g++)
            //{
            //    SoundBankEntryPath soundBankEntryPath = new SoundBankEntryPath();
            //    soundBankEntryPath.index = g;
            //    //bnr.BaseStream.Position = p;
            //    PLName.Clear();
            //    PLHash.Clear();
            //    sbkr.Offsets.Add(bnr.BaseStream.Position);
            //    //Going to try and read bytes until I see a period. 
            //    //Assuming that 0x00 is the terminating character.
            //    while ((B = bnr.ReadByte()) != 0x00)
            //    {
            //        PLName.Add(B);
            //    }

            //    //Converts to ASCII.
            //    Test = Encoding.ASCII.GetString(PLName.ToArray());
            //    //sbkr.SoundFilePaths.Add(Test);
            //    soundBankEntryPath.SoundFilePath = Test;
            //    //For the Typehash just in case there's other formats than .xsew.
            //    PLHash.AddRange(bnr.ReadBytes(4));
            //    PLHash.Reverse();
            //    //sbkr.TypeHashes.Add(ByteUtilitarian.ByteArrayToString(PLHash.ToArray()));
            //    soundBankEntryPath.TypeHash = ByteUtilitarian.ByteArrayToString(PLHash.ToArray());

            //    byte[] Tempbytes = bnr.ReadBytes(0x4C);
            //    soundBankEntryPath.OtherData.AddRange(Tempbytes.ToList());
            //    //sbkr.OtherData.Add(Tempbytes.ToList());

            //    soundBankEntryPath.OriginalStringLength = soundBankEntryPath.SoundFilePath.Length;

            //    //p = bnr.BaseStream.Position + 0x4C;
            //    sbkr.SoundFilePaths.Add(soundBankEntryPath);
            //}

            sbkr._FileType = sbkr.FileExt;
            sbkr._FileName = sbkr.TrueName;
            sbkr._DecompressedFileLength = sbkr.UncompressedData.Length;
            sbkr._CompressedFileLength = sbkr.CompressedData.Length;

            return sbkr;
        }

        public static SoundBankEntry ReplaceSoundBankEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            SoundBankEntry sbkrentry = new SoundBankEntry();
            SoundBankEntry oldentry = new SoundBankEntry();

            tree.BeginUpdate();

            ReplaceEntry(tree, node, filename, sbkrentry, oldentry);
            sbkrentry.DecompressedFileLength = sbkrentry.UncompressedData.Length;
            sbkrentry._DecompressedFileLength = sbkrentry.UncompressedData.Length;
            sbkrentry.CompressedFileLength = sbkrentry.CompressedData.Length;
            sbkrentry._CompressedFileLength = sbkrentry.CompressedData.Length;
            sbkrentry._FileName = sbkrentry.TrueName;
            sbkrentry._FileType = sbkrentry.FileExt;
            sbkrentry.FileName = sbkrentry.TrueName;

            return node.entryfile as SoundBankEntry;
        }

        public static SoundBankEntry InsertSoundBankEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            SoundBankEntry sbkrentry = new SoundBankEntry();

            InsertEntry(tree, node, filename, sbkrentry);

            sbkrentry.DecompressedFileLength = sbkrentry.UncompressedData.Length;
            sbkrentry._DecompressedFileLength = sbkrentry.UncompressedData.Length;
            sbkrentry.CompressedFileLength = sbkrentry.CompressedData.Length;
            sbkrentry._CompressedFileLength = sbkrentry.CompressedData.Length;
            sbkrentry._FileName = sbkrentry.TrueName;
            sbkrentry._FileType = sbkrentry.FileExt;
            sbkrentry.EntryName = sbkrentry.FileName;



            return sbkrentry;
        }

        #region SoundBank Properties
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
