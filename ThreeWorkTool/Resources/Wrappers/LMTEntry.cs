using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Archives;
using ThreeWorkTool.Resources.Utility;
using ThreeWorkTool.Resources.Wrappers;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class LMTEntry : DefaultWrapper
    {
        public int SomeNumber;
        //public int OffsetList;
        public int SecondOffsetList;
        public int RowCount;
        public int SomeValue1;
        public int OffsetOfInterest;
        public int Length;
        public List<int> OffsetList;
        public List<LMTM3AEntry> LstM3A;

        public static LMTEntry FillLMTEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
        {

            LMTEntry lmtentry = new LMTEntry();
            List<byte> BTemp = new List<byte>();

            FillEntry(filename, subnames, tree, br, c, ID, lmtentry, filetype);

            lmtentry._FileName = lmtentry.TrueName + lmtentry.FileExt;

            //Decompression Time.
            lmtentry.UncompressedData = ZlibStream.UncompressBuffer(lmtentry.CompressedData);
            lmtentry._DecompressedFileLength = lmtentry.UncompressedData.Length;
            lmtentry._CompressedFileLength = lmtentry.CompressedData.Length;

            int count = 1;
            byte[] STemp = new byte[2];
            byte[] OTemp = new byte[4];

            lmtentry.LstM3A = new List<LMTM3AEntry>();
            lmtentry.OffsetList = new List<int>();
            int ProjectedSize = 0;
            int SecondaryCount = 0;

            using (MemoryStream LmtStream = new MemoryStream(lmtentry.UncompressedData))
            {
                using (BinaryReader bnr = new BinaryReader(LmtStream))
                {
                    bnr.BaseStream.Position = 6;
                    lmtentry.SomeNumber = bnr.ReadInt16();
                    lmtentry.EntryCount = lmtentry.SomeNumber;

                    //Gets all the offsets. ALL OF THEM.
                    while (count < (lmtentry.SomeNumber))
                    {
                        lmtentry.OffsetList.Add(bnr.ReadInt32());
                        bnr.BaseStream.Position = bnr.BaseStream.Position + 4;
                        count++;

                    }

                    count = 0;
                    //Goes through the offsets to get the data. Ignores offsets of 0.
                    for(int i =0;i< lmtentry.OffsetList.Count; i++)
                    {
                        if(lmtentry.OffsetList[i] != 0)
                        {

                            LMTM3AEntry aEntry = new LMTM3AEntry();
                            aEntry = aEntry.FillM3AProprties(aEntry, LmtStream, lmtentry.Length, i, lmtentry.RowCount, lmtentry.SecondOffsetList, bnr, SecondaryCount, lmtentry);
                            lmtentry.LstM3A.Add(aEntry);


                        }


                    }

                        //Cribbed notes from Lean's lmt_extract tool to get the individual animations. For Reference Only rn.
                        /*
                        while (count < (lmtentry.SomeNumber + 1))
                        {
                            bnr.BaseStream.Position = count * 8;
                            lmtentry.OffsetTemp = bnr.ReadInt32();
                            if (lmtentry.OffsetTemp == 0)
                            {
                                count = count + 1;
                            }
                            else
                            {
                                bnr.BaseStream.Position = Convert.ToInt64(lmtentry.OffsetTemp);
                                lmtentry.SecondOffsetList = bnr.ReadInt32();
                                bnr.BaseStream.Position = bnr.BaseStream.Position + 4;
                                lmtentry.RowCount = bnr.ReadInt32();
                                lmtentry.SomeValue1 = bnr.ReadInt32();
                                bnr.BaseStream.Position = bnr.BaseStream.Position + 56;
                                lmtentry.OffsetOfInterest = bnr.ReadInt32();
                                lmtentry.Length = lmtentry.OffsetOfInterest - lmtentry.SecondOffsetList + 352;

                                bnr.BaseStream.Position = lmtentry.SecondOffsetList;

                                LMTM3AEntry aEntry = new LMTM3AEntry();

                                aEntry = aEntry.FillM3AProprties(aEntry, LmtStream, lmtentry.Length, count, lmtentry.RowCount, lmtentry.SecondOffsetList, bnr, SecondaryCount);
                                lmtentry.LstM3A.Add(aEntry);
                                count = count + 1;
                            }


                        }
                        */

                    }

                return lmtentry;

            }
        }

        public static LMTEntry ReplaceLMTEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            LMTEntry lmtentry = new LMTEntry();
            LMTEntry oldentry = new LMTEntry();

            tree.BeginUpdate();
            
            try
            {
                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
                {
                    //We build the lmtentry starting from the uncompressed data.
                    lmtentry.UncompressedData = System.IO.File.ReadAllBytes(filename);
                    lmtentry.DecompressedFileLength = lmtentry.UncompressedData.Length;
                    lmtentry._DecompressedFileLength = lmtentry.UncompressedData.Length;

                    //Then Compress.
                    lmtentry.CompressedData = Zlibber.Compressor(lmtentry.UncompressedData);
                    lmtentry.CompressedFileLength = lmtentry.CompressedData.Length;
                    lmtentry._CompressedFileLength = lmtentry.CompressedData.Length;

                    //Gets the filename of the file to inject without the directory.
                    string trname = filename;
                    while (trname.Contains("\\"))
                    {
                        trname = trname.Substring(trname.IndexOf("\\") + 1);
                    }

                    //Enters name related parameters of the lmtentry.
                    lmtentry.TrueName = trname;
                    lmtentry._FileName = lmtentry.TrueName;
                    lmtentry.TrueName = Path.GetFileNameWithoutExtension(trname);
                    lmtentry.FileExt = trname.Substring(trname.LastIndexOf("."));
                    lmtentry._FileType = lmtentry.FileExt;

                    string TypeHash = "";

                    //Looks through the archive_filetypes.cfg file to find the typehash associated with the extension.
                    try
                    {
                        using (var sr2 = new StreamReader("archive_filetypes.cfg"))
                        {
                            while (!sr2.EndOfStream)
                            {
                                var keyword = Console.ReadLine() ?? lmtentry.FileExt;
                                var line = sr2.ReadLine();
                                if (String.IsNullOrEmpty(line)) continue;
                                if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                                {
                                    TypeHash = line;
                                    TypeHash = TypeHash.Split(' ')[0];
                                    lmtentry.TypeHash = TypeHash;
                                    break;
                                }
                            }
                        }
                    }
                    catch (FileNotFoundException)
                    {
                        MessageBox.Show("I cannot find and/or access archive_filetypes.cfg so I cannot finish parsing the arc.", "Oh Boy");
                        using (StreamWriter sw = File.AppendText("Log.txt"))
                        {
                            sw.WriteLine("Cannot find archive_filetypes.cfg so I cannot continue parsing the file.");
                        }
                        return null;
                    }

                    var tag = node.Tag;
                    if (tag is LMTEntry)
                    {
                        oldentry = tag as LMTEntry;
                    }
                    string path = "";
                    int index = oldentry.EntryName.LastIndexOf("\\");
                    if (index > 0)
                    {
                        path = oldentry.EntryName.Substring(0, index);
                    }

                    lmtentry.EntryName = path + "\\" + lmtentry.TrueName;

                    tag = lmtentry;

                    if (node.Tag is LMTEntry)
                    {
                        node.Tag = lmtentry;
                        node.Name = Path.GetFileNameWithoutExtension(lmtentry.EntryName);
                        node.Text = Path.GetFileNameWithoutExtension(lmtentry.EntryName);

                    }

                    var aew = node as ArcEntryWrapper;

                    string type = node.GetType().ToString();
                    if (type == "ThreeWorkTool.Resources.Wrappers.ArcEntryWrapper")
                    {
                        aew.entryfile = lmtentry;
                    }

                    node = aew;
                    node.entryfile = lmtentry;
                    /*
                    //ArcEntryWrapper aew = new ArcEntryWrapper();
                    if (node is ArcEntryWrapper)
                    {
                        node.entryfile as ArcEntryWrapper = node.Tag;
                    }
                    */
                    tree.EndUpdate();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Read error. Is the file readable?");
                using (StreamWriter sw = File.AppendText("Log.txt"))
                {
                    sw.WriteLine("Read Error! Here's the exception info:\n" + ex);
                }
            }



            return node.entryfile as LMTEntry;
        }

        public static LMTEntry InsertLMTEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            LMTEntry lmtentry = new LMTEntry();

            try
            {
                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
                {
                    //We build the lmtentry starting from the uncompressed data.
                    lmtentry.UncompressedData = System.IO.File.ReadAllBytes(filename);
                    lmtentry.DecompressedFileLength = lmtentry.UncompressedData.Length;
                    lmtentry._DecompressedFileLength = lmtentry.UncompressedData.Length;
                    lmtentry.DSize = lmtentry.UncompressedData.Length;

                    //Then Compress.
                    lmtentry.CompressedData = Zlibber.Compressor(lmtentry.UncompressedData);
                    lmtentry.CompressedFileLength = lmtentry.CompressedData.Length;
                    lmtentry._CompressedFileLength = lmtentry.CompressedData.Length;
                    lmtentry.CSize = lmtentry.CompressedData.Length;

                    //Gets the filename of the file to inject without the directory.
                    string trname = filename;
                    while (trname.Contains("\\"))
                    {
                        trname = trname.Substring(trname.IndexOf("\\") + 1);
                    }

                    lmtentry.TrueName = trname;
                    lmtentry._FileName = lmtentry.TrueName;
                    lmtentry.TrueName = Path.GetFileNameWithoutExtension(trname);
                    lmtentry.FileExt = trname.Substring(trname.LastIndexOf("."));
                    lmtentry._FileType = lmtentry.FileExt;

                    //Gets the path of the selected node to inject here.
                    string nodepath = tree.SelectedNode.FullPath;
                    nodepath = nodepath.Substring(nodepath.IndexOf("\\") + 1);

                    string[] sepstr = { "\\" };
                    lmtentry.EntryDirs = nodepath.Split(sepstr, StringSplitOptions.RemoveEmptyEntries);
                    lmtentry.EntryName = lmtentry.FileName;

                    //Looks through the archive_filetypes.cfg file to find the typehash associated with the extension.
                    try
                    {
                        using (var sr2 = new StreamReader("archive_filetypes.cfg"))
                        {
                            while (!sr2.EndOfStream)
                            {
                                var keyword = Console.ReadLine() ?? lmtentry.FileExt;
                                var line = sr2.ReadLine();
                                if (String.IsNullOrEmpty(line)) continue;
                                if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                                {
                                    lmtentry.TypeHash = line;
                                    lmtentry.TypeHash = lmtentry.TypeHash.Split(' ')[0];

                                    break;
                                }
                            }
                        }
                    }
                    catch (FileNotFoundException)
                    {
                        MessageBox.Show("I cannot find and/or access archive_filetypes.cfg so I cannot finish parsing the arc.", "Oh Boy");
                        using (StreamWriter sw = File.AppendText("Log.txt"))
                        {
                            sw.WriteLine("Cannot find archive_filetypes.cfg so I cannot continue parsing the file.");
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText("Log.txt"))
                {
                    sw.WriteLine("Caught an exception using the BinaryReader. Here's the details:\n" + ex);
                }
            }



            return lmtentry;
        }


        #region LMTEntry Properties
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

        private long _EntryCount;
        [Category("MT ARC Entry"), ReadOnlyAttribute(true)]
        public long EntryCount
        {

            get
            {
                return _EntryCount;
            }
            set
            {
                _EntryCount = value;
            }
        }

        #endregion

    }
}
