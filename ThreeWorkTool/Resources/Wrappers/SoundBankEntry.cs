using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Archives;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class SoundBankEntry : DefaultWrapper
    {

        public static SoundBankEntry FillSoundBankEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
        {

            SoundBankEntry sbkrentry = new SoundBankEntry();

            FillEntry(filename, subnames, tree, br, c, ID, sbkrentry, filetype);

            sbkrentry._FileType = sbkrentry.FileExt;
            sbkrentry._FileName = sbkrentry.TrueName;
            sbkrentry._DecompressedFileLength = sbkrentry.UncompressedData.Length;
            sbkrentry._CompressedFileLength = sbkrentry.CompressedData.Length;

            return sbkrentry;

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
