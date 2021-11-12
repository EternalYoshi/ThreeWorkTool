using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Archives;
using ThreeWorkTool.Resources.Utility;
using ThreeWorkTool.Resources.Wrappers;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class ChainEntry : DefaultWrapper
    {

        public static ChainEntry FillChainEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
        {

            ChainEntry chnentry = new ChainEntry();

            FillEntry(filename, subnames, tree, br, c, ID, chnentry, filetype);

            chnentry._FileType = chnentry.FileExt;
            chnentry._FileName = chnentry.TrueName + chnentry.FileExt;
            chnentry._DecompressedFileLength = chnentry.UncompressedData.Length;
            chnentry._CompressedFileLength = chnentry.CompressedData.Length;

            return chnentry;

        }

        public static ChainEntry ReplaceChainEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            ChainEntry chnentry = new ChainEntry();
            ChainEntry oldentry = new ChainEntry();

            tree.BeginUpdate();

            ReplaceEntry(tree, node, filename, chnentry, oldentry);

            chnentry.DecompressedFileLength = chnentry.UncompressedData.Length;
            chnentry._DecompressedFileLength = chnentry.UncompressedData.Length;
            chnentry.CompressedFileLength = chnentry.CompressedData.Length;
            chnentry._CompressedFileLength = chnentry.CompressedData.Length;
            chnentry._FileName = chnentry.TrueName;
            chnentry._FileType = chnentry.FileExt;

            return node.entryfile as ChainEntry;
        }

        public static ChainEntry InsertChainEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            ChainEntry chnentry = new ChainEntry();

            InsertEntry(tree, node, filename, chnentry);

            chnentry.DecompressedFileLength = chnentry.UncompressedData.Length;
            chnentry._DecompressedFileLength = chnentry.UncompressedData.Length;
            chnentry.CompressedFileLength = chnentry.CompressedData.Length;
            chnentry._CompressedFileLength = chnentry.CompressedData.Length;
            chnentry._FileName = chnentry.TrueName;
            chnentry._FileType = chnentry.FileExt;
            chnentry.EntryName = chnentry.FileName;



            return chnentry;
        }

        #region Chain Collision Properties
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
