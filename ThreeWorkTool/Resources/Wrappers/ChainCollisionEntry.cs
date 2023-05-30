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
    public class ChainCollisionEntry : DefaultWrapper
    {

        public static ChainCollisionEntry FillChainCollEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
        {

            ChainCollisionEntry ccolentry = new ChainCollisionEntry();

            FillEntry(filename, subnames, tree, br, c, ID, ccolentry, filetype);

            ccolentry._FileType = ccolentry.FileExt;
            ccolentry._FileName = ccolentry.TrueName;
            ccolentry._DecompressedFileLength = ccolentry.UncompressedData.Length;
            ccolentry._CompressedFileLength = ccolentry.CompressedData.Length;

            return ccolentry;

        }

        public static ChainCollisionEntry ReplaceChainCollEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            ChainCollisionEntry ccolentry = new ChainCollisionEntry();
            ChainCollisionEntry oldentry = new ChainCollisionEntry();

            tree.BeginUpdate();

            ReplaceEntry(tree, node, filename, ccolentry, oldentry);
            ccolentry.DecompressedFileLength = ccolentry.UncompressedData.Length;
            ccolentry._DecompressedFileLength = ccolentry.UncompressedData.Length;
            ccolentry.CompressedFileLength = ccolentry.CompressedData.Length;
            ccolentry._CompressedFileLength = ccolentry.CompressedData.Length;
            ccolentry._FileName = ccolentry.TrueName;
            ccolentry._FileType = ccolentry.FileExt;
            ccolentry.FileName = ccolentry.TrueName;

            return node.entryfile as ChainCollisionEntry;
        }

        public static ChainCollisionEntry InsertChainCollEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            ChainCollisionEntry ccolentry = new ChainCollisionEntry();

            InsertEntry(tree, node, filename, ccolentry);

            ccolentry.DecompressedFileLength = ccolentry.UncompressedData.Length;
            ccolentry._DecompressedFileLength = ccolentry.UncompressedData.Length;
            ccolentry.CompressedFileLength = ccolentry.CompressedData.Length;
            ccolentry._CompressedFileLength = ccolentry.CompressedData.Length;
            ccolentry._FileName = ccolentry.TrueName;
            ccolentry._FileType = ccolentry.FileExt;
            ccolentry.EntryName = ccolentry.FileName;



            return ccolentry;
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
