using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Archives;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class AtkInfoEntry : DefaultWrapper
    {

        public static AtkInfoEntry FillAtkInfoEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
        {

            AtkInfoEntry atientry = new AtkInfoEntry();

            FillEntry(filename, subnames, tree, br, c, ID, atientry, filetype);

            atientry._FileType = atientry.FileExt;
            atientry._FileName = atientry.TrueName;
            atientry._DecompressedFileLength = atientry.UncompressedData.Length;
            atientry._CompressedFileLength = atientry.CompressedData.Length;

            return atientry;

        }

        public static AtkInfoEntry ReplaceAtkInfoEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            AtkInfoEntry atientry = new AtkInfoEntry();
            AtkInfoEntry oldentry = new AtkInfoEntry();

            tree.BeginUpdate();

            ReplaceEntry(tree, node, filename, atientry, oldentry);
            atientry.DecompressedFileLength = atientry.UncompressedData.Length;
            atientry._DecompressedFileLength = atientry.UncompressedData.Length;
            atientry.CompressedFileLength = atientry.CompressedData.Length;
            atientry._CompressedFileLength = atientry.CompressedData.Length;
            atientry._FileName = atientry.TrueName;
            atientry._FileType = atientry.FileExt;
            atientry.FileName = atientry.TrueName;

            return node.entryfile as AtkInfoEntry;
        }

        public static AtkInfoEntry InsertAtkInfoEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            AtkInfoEntry atientry = new AtkInfoEntry();

            InsertEntry(tree, node, filename, atientry);

            atientry.DecompressedFileLength = atientry.UncompressedData.Length;
            atientry._DecompressedFileLength = atientry.UncompressedData.Length;
            atientry.CompressedFileLength = atientry.CompressedData.Length;
            atientry._CompressedFileLength = atientry.CompressedData.Length;
            atientry._FileName = atientry.TrueName;
            atientry._FileType = atientry.FileExt;
            atientry.EntryName = atientry.FileName;



            return atientry;
        }

        #region AtkInfo Properties
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
