using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Archives;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class ShotEntry : DefaultWrapper
    {

        public static ShotEntry FillShotEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
        {

            ShotEntry shtentry = new ShotEntry();

            FillEntry(filename, subnames, tree, br, c, ID, shtentry, filetype);

            shtentry._FileType = shtentry.FileExt;
            shtentry._FileName = shtentry.TrueName;
            shtentry._DecompressedFileLength = shtentry.UncompressedData.Length;
            shtentry._CompressedFileLength = shtentry.CompressedData.Length;

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



            return shtentry;
        }

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
