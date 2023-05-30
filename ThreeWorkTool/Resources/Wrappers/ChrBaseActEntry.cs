using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Archives;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class ChrBaseActEntry : DefaultWrapper
    {

        public static ChrBaseActEntry FillChrBaseActEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
        {

            ChrBaseActEntry cbaentry = new ChrBaseActEntry();

            FillEntry(filename, subnames, tree, br, c, ID, cbaentry, filetype);

            cbaentry._FileType = cbaentry.FileExt;
            cbaentry._FileName = cbaentry.TrueName;
            cbaentry._DecompressedFileLength = cbaentry.UncompressedData.Length;
            cbaentry._CompressedFileLength = cbaentry.CompressedData.Length;

            return cbaentry;

        }

        public static ChrBaseActEntry ReplaceChrBaseActEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            ChrBaseActEntry cbaentry = new ChrBaseActEntry();
            ChrBaseActEntry oldentry = new ChrBaseActEntry();

            tree.BeginUpdate();

            ReplaceEntry(tree, node, filename, cbaentry, oldentry);
            cbaentry.DecompressedFileLength = cbaentry.UncompressedData.Length;
            cbaentry._DecompressedFileLength = cbaentry.UncompressedData.Length;
            cbaentry.CompressedFileLength = cbaentry.CompressedData.Length;
            cbaentry._CompressedFileLength = cbaentry.CompressedData.Length;
            cbaentry._FileName = cbaentry.TrueName;
            cbaentry._FileType = cbaentry.FileExt;
            cbaentry.FileName = cbaentry.TrueName;

            return node.entryfile as ChrBaseActEntry;
        }

        public static ChrBaseActEntry InsertChrBaseActEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            ChrBaseActEntry cbaentry = new ChrBaseActEntry();

            InsertEntry(tree, node, filename, cbaentry);

            cbaentry.DecompressedFileLength = cbaentry.UncompressedData.Length;
            cbaentry._DecompressedFileLength = cbaentry.UncompressedData.Length;
            cbaentry.CompressedFileLength = cbaentry.CompressedData.Length;
            cbaentry._CompressedFileLength = cbaentry.CompressedData.Length;
            cbaentry._FileName = cbaentry.TrueName;
            cbaentry._FileType = cbaentry.FileExt;
            cbaentry.EntryName = cbaentry.FileName;



            return cbaentry;
        }

        #region ChrBaseAct Properties
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
