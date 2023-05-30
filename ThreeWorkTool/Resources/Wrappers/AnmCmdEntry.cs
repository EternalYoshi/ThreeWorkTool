using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Archives;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class AnmCmdEntry : DefaultWrapper
    {

        public static AnmCmdEntry FillAnmCmdEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
        {

            AnmCmdEntry anmentry = new AnmCmdEntry();

            FillEntry(filename, subnames, tree, br, c, ID, anmentry, filetype);

            anmentry._FileType = anmentry.FileExt;
            anmentry._FileName = anmentry.TrueName;
            anmentry._DecompressedFileLength = anmentry.UncompressedData.Length;
            anmentry._CompressedFileLength = anmentry.CompressedData.Length;

            return anmentry;

        }

        public static AnmCmdEntry ReplaceAnmCmdEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            AnmCmdEntry anmentry = new AnmCmdEntry();
            AnmCmdEntry oldentry = new AnmCmdEntry();

            tree.BeginUpdate();

            ReplaceEntry(tree, node, filename, anmentry, oldentry);
            anmentry.DecompressedFileLength = anmentry.UncompressedData.Length;
            anmentry._DecompressedFileLength = anmentry.UncompressedData.Length;
            anmentry.CompressedFileLength = anmentry.CompressedData.Length;
            anmentry._CompressedFileLength = anmentry.CompressedData.Length;
            anmentry._FileName = anmentry.TrueName;
            anmentry._FileType = anmentry.FileExt;
            anmentry.FileName = anmentry.TrueName;

            return node.entryfile as AnmCmdEntry;
        }

        public static AnmCmdEntry InsertAnmCmdEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            AnmCmdEntry anmentry = new AnmCmdEntry();

            InsertEntry(tree, node, filename, anmentry);

            anmentry.DecompressedFileLength = anmentry.UncompressedData.Length;
            anmentry._DecompressedFileLength = anmentry.UncompressedData.Length;
            anmentry.CompressedFileLength = anmentry.CompressedData.Length;
            anmentry._CompressedFileLength = anmentry.CompressedData.Length;
            anmentry._FileName = anmentry.TrueName;
            anmentry._FileType = anmentry.FileExt;
            anmentry.EntryName = anmentry.FileName;



            return anmentry;
        }

        #region AnmCmd Properties
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
