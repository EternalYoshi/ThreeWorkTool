using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Archives;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class SoundRequestEntry : DefaultWrapper
    {

        public static SoundRequestEntry FillSoundRequestEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
        {

            SoundRequestEntry srqrentry = new SoundRequestEntry();

            FillEntry(filename, subnames, tree, br, c, ID, srqrentry, filetype);

            srqrentry._FileType = srqrentry.FileExt;
            srqrentry._FileName = srqrentry.TrueName;
            srqrentry._DecompressedFileLength = srqrentry.UncompressedData.Length;
            srqrentry._CompressedFileLength = srqrentry.CompressedData.Length;

            return srqrentry;

        }

        public static SoundRequestEntry ReplaceSoundRequestEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            SoundRequestEntry srqrentry = new SoundRequestEntry();
            SoundRequestEntry oldentry = new SoundRequestEntry();

            tree.BeginUpdate();

            ReplaceEntry(tree, node, filename, srqrentry, oldentry);
            srqrentry.DecompressedFileLength = srqrentry.UncompressedData.Length;
            srqrentry._DecompressedFileLength = srqrentry.UncompressedData.Length;
            srqrentry.CompressedFileLength = srqrentry.CompressedData.Length;
            srqrentry._CompressedFileLength = srqrentry.CompressedData.Length;
            srqrentry._FileName = srqrentry.TrueName;
            srqrentry._FileType = srqrentry.FileExt;
            srqrentry.FileName = srqrentry.TrueName;

            return node.entryfile as SoundRequestEntry;
        }

        public static SoundRequestEntry InsertSoundRequestEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            SoundRequestEntry srqrentry = new SoundRequestEntry();

            InsertEntry(tree, node, filename, srqrentry);

            srqrentry.DecompressedFileLength = srqrentry.UncompressedData.Length;
            srqrentry._DecompressedFileLength = srqrentry.UncompressedData.Length;
            srqrentry.CompressedFileLength = srqrentry.CompressedData.Length;
            srqrentry._CompressedFileLength = srqrentry.CompressedData.Length;
            srqrentry._FileName = srqrentry.TrueName;
            srqrentry._FileType = srqrentry.FileExt;
            srqrentry.EntryName = srqrentry.FileName;



            return srqrentry;
        }

        #region SoundRequest Properties
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
