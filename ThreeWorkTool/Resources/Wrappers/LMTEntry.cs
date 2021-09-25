using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Utility;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class LMTEntry
    {
        public string EntryName;
        public string TypeHash;
        public string TempStr;
        public string FileExt;
        public string TrueName;
        public string TempFolder;
        public int AOffset;
        public int CSize;
        public int DSize;
        public int OffsetTemp;
        public byte[] TBFlag;
        public byte[] CompressedData;
        public byte[] UncompressedData;
        public static StringBuilder SBname;
        public string[] EntryDirs;
        public int EntryID;
        public int SomeNumber;
        public int OffsetList;
        public int SecondOffsetList;
        public int RowCount;
        public int SomeValue1;
        public int OffsetOfInterest;
        public int Length;

        public static LMTEntry FillLMTEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
        {

            LMTEntry lmtentry = new LMTEntry();
            List<byte> BTemp = new List<byte>();

            //This block gets the name of the entry.
            lmtentry.OffsetTemp = c;
            lmtentry.EntryID = ID;
            br.BaseStream.Position = lmtentry.OffsetTemp;
            var TempName = Encoding.ASCII.GetString(br.ReadBytes(64)).Trim('\0');

            //This is for the bytes that have the typehash, the thing that dictates the type of file stored.
            BTemp = new List<byte>();
            c = c + 64;
            br.BaseStream.Position = c;
            lmtentry.TypeHash = ByteUtilitarian.BytesToStringL2R(br.ReadBytes(4).ToList(), lmtentry.TypeHash);

            //Compressed Data size.
            lmtentry.CSize = br.ReadInt32();

            //Uncompressed Data size.
            lmtentry.DSize = br.ReadInt32() - 1073741824;

            //Data Offset.
            lmtentry.AOffset = br.ReadInt32();

            //Compressed Data.
            BTemp = new List<byte>();
            br.BaseStream.Position = lmtentry.AOffset;
            lmtentry.CompressedData = br.ReadBytes(lmtentry.CSize);

            //Namestuff.
            lmtentry.EntryName = TempName;

            //Ensures existing subdirectories are cleared so the directories for files are displayed correctly.
            if (subnames != null)
            {
                if (subnames.Count > 0)
                {
                    subnames.Clear();
                }
            }

            //Gets the filename without subdirectories.
            if (lmtentry.EntryName.Contains("\\"))
            {
                string[] splstr = lmtentry.EntryName.Split('\\');

                //foreach (string v in splstr)
                for (int v = 0; v < (splstr.Length - 1); v++)
                {
                    if (!subnames.Contains(splstr[v]))
                    {
                        subnames.Add(splstr[v]);
                    }
                }


                lmtentry.TrueName = lmtentry.EntryName.Substring(lmtentry.EntryName.IndexOf("\\") + 1);
                Array.Clear(splstr, 0, splstr.Length);

                while (lmtentry.TrueName.Contains("\\"))
                {
                    lmtentry.TrueName = lmtentry.TrueName.Substring(lmtentry.TrueName.IndexOf("\\") + 1);
                }
            }
            else
            {
                lmtentry.TrueName = lmtentry.EntryName;
            }


            lmtentry.EntryDirs = subnames.ToArray();


            //Looks through the archive_filetypes.cfg file to find the extension associated with the typehash.
            try
            {
                using (var sr = new StreamReader("archive_filetypes.cfg"))
                {
                    while (!sr.EndOfStream)
                    {
                        var keyword = Console.ReadLine() ?? lmtentry.TypeHash;
                        var line = sr.ReadLine();
                        if (String.IsNullOrEmpty(line)) continue;
                        if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                        {
                            lmtentry.FileExt = line;
                            lmtentry.FileExt = lmtentry.FileExt.Split(' ')[1];
                            lmtentry.EntryName = lmtentry.EntryName + lmtentry.FileExt;
                            lmtentry._FileType = lmtentry.FileExt;
                            break;
                        }
                    }
                }

            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("I cannot find archive_filetypes.cfg so I cannot finish parsing the arc.", "Oh Boy");
                using (StreamWriter sw = File.AppendText("Log.txt"))
                {
                    sw.WriteLine("Cannot find archive_filetypes.cfg so I cannot continue parsing the file.");
                }
                return null;
            }

            lmtentry._FileName = lmtentry.TrueName + lmtentry.FileExt;

            //Decompression Time.
            lmtentry.UncompressedData = ZlibStream.UncompressBuffer(lmtentry.CompressedData);
            lmtentry._DecompressedFileLength = lmtentry.UncompressedData.Length;
            lmtentry._CompressedFileLength = lmtentry.CompressedData.Length;

            int count = 1;
            byte[] STemp = new byte[2];
            byte[] OTemp = new byte[4];

            /*
            lmtentry.OffsetList = new List<int>();
            lmtentry.SomeValue1 = new List<int>();
            lmtentry.RowCount = new List<int>();
            lmtentry.OffsetOfInterest = new List<int>();
            lmtentry.Length = new List<int>();
            lmtentry.SecondOffsetList = new List<int>();
            */

            using (MemoryStream LmtStream = new MemoryStream(lmtentry.UncompressedData))

            {
                LmtStream.Position = 6;
                LmtStream.Read(STemp, 0,2);

                lmtentry.SomeNumber = BitConverter.ToInt16(STemp,0);

                /*
                //Cribbed notes from Lean's lmt_extract tool to get the individual animations.
                while (count < (lmtentry.SomeNumber + 1))
                {
                    LmtStream.Position = count * 8;
                    LmtStream.Read(OTemp,0,4);
                    lmtentry.OffsetTemp = BitConverter.ToInt32(OTemp,0);
                    if (lmtentry.OffsetTemp == 0)
                    {
                        count = count + 1;
                    }
                    else
                    {
                        LmtStream.Position = Convert.ToInt64(lmtentry.OffsetTemp);
                        LmtStream.Read(OTemp, 0, 4);
                        lmtentry.SecondOffsetList = BitConverter.ToInt32(OTemp, 0);
                        LmtStream.Position = LmtStream.Position + 4;
                        LmtStream.Read(OTemp, 0, 4);
                        lmtentry.RowCount = BitConverter.ToInt32(OTemp, 0);
                        LmtStream.Read(OTemp, 0, 4);
                        lmtentry.SomeValue1 = BitConverter.ToInt32(OTemp, 0);
                        LmtStream.Position = LmtStream.Position + 56;
                        LmtStream.Read(OTemp, 0, 4);
                        lmtentry.OffsetOfInterest = BitConverter.ToInt32(OTemp, 0);
                        lmtentry.Length = lmtentry.OffsetOfInterest - lmtentry.SecondOffsetList + 352;

                        LmtStream.Position = lmtentry.SecondOffsetList;

                        LMTM3AEntry aEntry = new LMTM3AEntry();

                        //Finish this....
                        aEntry.FillM3AProprties(aEntry,LmtStream,lmtentry.Length,count,lmtentry.RowCount);



                        count = count + 1;
                    }


                }


                */

            }



            return lmtentry;


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

        #endregion

    }
}
