using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Utility;
using ThreeWorkTool.Resources.Wrappers;

namespace ThreeWorkTool.Resources.Archives
{
    public class ArcEntry : DefaultWrapper
    {
        public static ArcEntry FillArcEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
        {
            ArcEntry arcentry = new ArcEntry();
            List<byte> BTemp = new List<byte>();

            FillEntry(filename, subnames, tree, br, c, ID, arcentry, filetype);

            arcentry._FileType = arcentry.FileExt;
            arcentry._FileName = arcentry.TrueName + arcentry.FileExt;
            arcentry._DecompressedFileLength = arcentry.UncompressedData.Length;
            arcentry._CompressedFileLength = arcentry.CompressedData.Length;

            return arcentry;
        }

        public static ArcEntry ReplaceArcEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            ArcEntry arcentry = new ArcEntry();
            ArcEntry oldentry = new ArcEntry();

            tree.BeginUpdate();

            ReplaceEntry(tree, node, filename, arcentry, oldentry);

            arcentry.DecompressedFileLength = arcentry.UncompressedData.Length;
            arcentry._DecompressedFileLength = arcentry.UncompressedData.Length;
            arcentry.CompressedFileLength = arcentry.CompressedData.Length;
            arcentry._CompressedFileLength = arcentry.CompressedData.Length;
            arcentry._FileName = arcentry.TrueName;
            arcentry._FileType = arcentry.FileExt;
            arcentry.FileName = arcentry.TrueName;
            return node.entryfile as ArcEntry;
        }

        public static ArcEntry InsertArcEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            ArcEntry arcentry = new ArcEntry();

            InsertEntry(tree, node, filename, arcentry);

            arcentry.DecompressedFileLength = arcentry.UncompressedData.Length;
            arcentry._DecompressedFileLength = arcentry.UncompressedData.Length;
            arcentry.CompressedFileLength = arcentry.CompressedData.Length;
            arcentry._CompressedFileLength = arcentry.CompressedData.Length;
            arcentry._FileName = arcentry.TrueName;
            arcentry._FileType = arcentry.FileExt;
            arcentry.EntryName = arcentry.FileName;



            return arcentry;
        }

        //Looks through the cfg file to find the Typehash and returns it.
        public static string TypeHashFinder(ArcEntry arctry)
        {
            string TypeHash = "";

            if (arctry.FileExt.Length == 9)
            {
                TypeHash = arctry.FileExt;
                TypeHash = TypeHash.Substring(1);
                if (System.Text.RegularExpressions.Regex.IsMatch(TypeHash, @"\A\b[0-9A-F]+\b\Z") == true)
                {
                    return TypeHash;
                }
            }

            //Gets the Corrected path for the cfg.
            string ProperPath = "";
            ProperPath = Globals.ToolPath + "archive_filetypes.cfg";
            //Looks through the archive_filetypes.cfg file to find the typehash associated with the extension.
            try
            {
                using (var sr2 = new StreamReader(ProperPath))
                {
                    while (!sr2.EndOfStream)
                    {
                        var keyword = Console.ReadLine() ?? arctry.FileExt;
                        var line = sr2.ReadLine();
                        if (String.IsNullOrEmpty(line)) continue;
                        if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                        {
                            TypeHash = line;
                            TypeHash = TypeHash.Split(' ')[0];

                            break;
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Cannot find archive_filetypes.cfg so I cannot continue parsing this file.\n Find archive_filetypes.cfg and then restart this program.", " ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Process.GetCurrentProcess().Kill();
            }

            return TypeHash;
        }

        #region ArcEntry Properties
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
