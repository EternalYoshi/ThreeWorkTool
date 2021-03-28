using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Wrappers;

namespace ThreeWorkTool.Resources.Archives
{
    public class ArcEntry
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

        public static ArcEntry FillEntry(string filename, List<string> subnames, TreeView tree, byte[] Bytes, int c, int ID, Type filetype = null)
        {
            ArcEntry arcentry = new ArcEntry();

            using (FileStream fs = File.OpenRead(filename))
            {
                //This block gets the name of the entry.

                arcentry.OffsetTemp = c;
                arcentry.EntryID = ID;
                byte[] BTemp;
                BTemp = new byte[] { };
                BTemp = Bytes.Skip(arcentry.OffsetTemp).Take(64).Where(x => x != 0x00).ToArray();

                if (SBname == null)
                {
                    SBname = new StringBuilder();
                }
                else
                {
                    SBname.Clear();
                }

                string Tempname;
                ASCIIEncoding ascii = new ASCIIEncoding();
                Tempname = ascii.GetString(BTemp);

                //This is for the bytes that have the typehash, the thing that dictates the type of file stored.

                BTemp = new byte[] { };
                c = c + 64;
                BTemp = Bytes.Skip(c).Take(4).Where(x => x != 0x00).ToArray();
                Array.Reverse(BTemp);
                arcentry.TypeHash = BytesToString(BTemp, arcentry.TypeHash);

                //Compressed Data size.

                BTemp = new byte[] { };
                c = c + 4;
                BTemp = Bytes.Skip(c).Take(4).ToArray();
                BTemp.Reverse();
                arcentry.CSize = BitConverter.ToInt32(BTemp, 0);

                //Uncompressed Data size.

                BTemp = new byte[] { };
                c = c + 4;
                BTemp = Bytes.Skip(c).Take(4).ToArray();
                Array.Reverse(BTemp);
                string TempStr = "";
                TempStr = BytesToString(BTemp, TempStr);
                BigInteger BN1, BN2, DIFF;
                BN2 = BigInteger.Parse("40000000", NumberStyles.HexNumber);
                BN1 = BigInteger.Parse(TempStr, NumberStyles.HexNumber);
                DIFF = BN1 - BN2;
                arcentry.DSize = (int)DIFF;

                //Data Offset.
                BTemp = new byte[] { };
                c = c + 4;
                BTemp = Bytes.Skip(c).Take(4).ToArray();
                BTemp.Reverse();
                arcentry.AOffset = BitConverter.ToInt32(BTemp, 0);

                //Compressed Data.
                BTemp = new byte[] { };
                c = arcentry.AOffset;
                BTemp = Bytes.Skip(c).Take(arcentry.CSize).ToArray();
                arcentry.CompressedData = BTemp;

                //Namestuff.
                arcentry.EntryName = Tempname;

                //Ensures existing subdirectories are cleared so the directories for files are displayed correctly.
                if (subnames != null)
                {
                    if (subnames.Count > 0)
                    {
                        subnames.Clear();
                    }
                }

                //Gets the filename without subdirectories.
                if (arcentry.EntryName.Contains("\\"))
                {
                    string[] splstr = arcentry.EntryName.Split('\\');

                    //foreach (string v in splstr)
                    for (int v = 0; v < (splstr.Length - 1); v++)
                    {
                        if (!subnames.Contains(splstr[v]))
                        {
                            subnames.Add(splstr[v]);
                        }
                    }


                    arcentry.TrueName = arcentry.EntryName.Substring(arcentry.EntryName.IndexOf("\\") + 1);
                    Array.Clear(splstr,0,splstr.Length);

                    while (arcentry.TrueName.Contains("\\"))
                    {
                        arcentry.TrueName = arcentry.TrueName.Substring(arcentry.TrueName.IndexOf("\\") + 1);
                    }
                }
                else
                {
                    arcentry.TrueName = arcentry.EntryName;
                }


                arcentry.EntryDirs = subnames.ToArray();
                

                //Looks through the archive_filetypes.cfg file to find the extension associated with the typehash.
                try
                {
                    using (var sr = new StreamReader("archive_filetypes.cfg"))
                    {
                        while (!sr.EndOfStream)
                        {
                            var keyword = Console.ReadLine() ?? arcentry.TypeHash;
                            var line = sr.ReadLine();
                            if (String.IsNullOrEmpty(line)) continue;
                            if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                            {
                                arcentry.FileExt = line;
                                arcentry.FileExt = arcentry.FileExt.Split(' ')[1];
                                arcentry.EntryName = arcentry.EntryName + arcentry.FileExt;
                                arcentry._FileType = arcentry.FileExt;
                                break;
                            }
                        }
                    }

                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show("I cannot find archive_filetypes.cfg so I cannot finish parsing the arc.", "Oh Boy");
                    fs.Close();
                }


            }

            arcentry._FileName = arcentry.TrueName + arcentry.FileExt;

            //Decompression Time.
            arcentry.UncompressedData = ZlibStream.UncompressBuffer(arcentry.CompressedData);
            arcentry._DecompressedFileLength = arcentry.UncompressedData.Length;
            arcentry._CompressedFileLength = arcentry.CompressedData.Length;


            return arcentry;
        }

        public static string BytesToString(byte[] bytes, string s)
        {
            string temps;
            string tru = "";
            //int tempi;
            for (int i = 0; i < bytes.Length; i++)
            {
                temps = bytes[i].ToString("X");
                //Fix this for other single digit numbers!
                if (temps == "0")
                {
                    temps = "00";
                }
                else if (temps == "1")
                {
                    temps = "01";
                }
                else if (temps == "2")
                {
                    temps = "02";
                }
                else if (temps == "3")
                {
                    temps = "03";
                }
                else if (temps == "4")
                {
                    temps = "04";
                }
                else if (temps == "5")
                {
                    temps = "05";
                }
                else if (temps == "6")
                {
                    temps = "06";
                }
                else if (temps == "7")
                {
                    temps = "07";
                }
                else if (temps == "8")
                {
                    temps = "08";
                }
                else if (temps == "9")
                {
                    temps = "09";
                }
                else if (temps == "A")
                {
                    temps = "0A";
                }
                else if (temps == "B")
                {
                    temps = "0B";
                }
                else if (temps == "C")
                {
                    temps = "0C";
                }
                else if (temps == "D")
                {
                    temps = "0D";
                }
                else if (temps == "E")
                {
                    temps = "0E";
                }
                else if (temps == "F")
                {
                    temps = "0F";
                }
                tru += temps;
            }
            return tru;
        }

        public static ArcEntry ReplaceEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            ArcEntry arcentry = new ArcEntry();
            ArcEntry oldentry = new ArcEntry();

            tree.BeginUpdate();

            try
            {
                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
                {
                    //We build the arcentry starting from the uncompressed data.
                    arcentry.UncompressedData = System.IO.File.ReadAllBytes(filename);
                    arcentry.DecompressedFileLength = arcentry.UncompressedData.Length;
                    arcentry._DecompressedFileLength = arcentry.UncompressedData.Length;

                    //Then Compress.
                    arcentry.CompressedData = Zlibber.Compressor(arcentry.UncompressedData);
                    arcentry.CompressedFileLength = arcentry.CompressedData.Length;
                    arcentry._CompressedFileLength = arcentry.CompressedData.Length;

                    //Gets the filename of the file to inject without the directory.
                    string trname = filename;
                    while (trname.Contains("\\"))
                    {
                        trname = trname.Substring(trname.IndexOf("\\") + 1);
                    }

                    //Enters name related parameters of the arcentry.
                    arcentry.TrueName = trname;
                    arcentry._FileName = arcentry.TrueName;
                    arcentry.TrueName = Path.GetFileNameWithoutExtension(trname);
                    arcentry.FileExt = trname.Substring(trname.LastIndexOf("."));
                    arcentry._FileType = arcentry.FileExt;

                    string TypeHash = "";

                    //Looks through the archive_filetypes.cfg file to find the typehash associated with the extension.
                    try
                    {
                        using (var sr2 = new StreamReader("archive_filetypes.cfg"))
                        {
                            while (!sr2.EndOfStream)
                            {
                                var keyword = Console.ReadLine() ?? arcentry.FileExt;
                                var line = sr2.ReadLine();
                                if (String.IsNullOrEmpty(line)) continue;
                                if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                                {
                                    TypeHash = line;
                                    TypeHash = TypeHash.Split(' ')[0];
                                    arcentry.TypeHash = TypeHash;
                                    break;
                                }
                            }
                        }
                    }
                    catch (FileNotFoundException)
                    {
                        MessageBox.Show("I cannot find and/or access archive_filetypes.cfg so I cannot finish parsing the arc.", "Oh Boy");

                    }

                    var tag = node.Tag;
                    if (tag is ArcEntry)
                    {
                        oldentry = tag as ArcEntry;
                    }
                    string path = "";
                    int index = oldentry.EntryName.LastIndexOf("\\");
                    if (index > 0)
                    {
                        path = oldentry.EntryName.Substring(0, index);
                    }

                    arcentry.EntryName = path + "\\" + arcentry.TrueName;
                    
                    tag = arcentry;

                    if (node.Tag is ArcEntry)
                    {
                        node.Tag = arcentry;
                        node.Name = Path.GetFileNameWithoutExtension(arcentry.EntryName);
                        node.Text = Path.GetFileNameWithoutExtension(arcentry.EntryName);
                        
                    }

                    var aew = node as ArcEntryWrapper;

                    string type = node.GetType().ToString();
                    if (type == "ThreeWorkTool.Resources.Wrappers.ArcEntryWrapper")
                    {
                        aew.entryfile = arcentry;
                    }

                    node = aew;
                    node.entryfile = arcentry;
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
            catch(Exception ex)
           {
                MessageBox.Show("Read error. Is the file readable?");
           }



            return node.entryfile as ArcEntry;
        }

        public static ArcEntry InsertEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            ArcEntry arcentry = new ArcEntry();

            try
            {
                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
                {
                    //We build the arcentry starting from the uncompressed data.
                    arcentry.UncompressedData = System.IO.File.ReadAllBytes(filename);
                    arcentry.DecompressedFileLength = arcentry.UncompressedData.Length;
                    arcentry._DecompressedFileLength = arcentry.UncompressedData.Length;
                    arcentry.DSize = arcentry.UncompressedData.Length;

                    //Then Compress.
                    arcentry.CompressedData = Zlibber.Compressor(arcentry.UncompressedData);
                    arcentry.CompressedFileLength = arcentry.CompressedData.Length;
                    arcentry._CompressedFileLength = arcentry.CompressedData.Length;
                    arcentry.CSize = arcentry.CompressedData.Length;

                    //Gets the filename of the file to inject without the directory.
                    string trname = filename;
                    while (trname.Contains("\\"))
                    {
                        trname = trname.Substring(trname.IndexOf("\\") + 1);
                    }

                    arcentry.TrueName = trname;
                    arcentry._FileName = arcentry.TrueName;
                    arcentry.TrueName = Path.GetFileNameWithoutExtension(trname);
                    arcentry.FileExt = trname.Substring(trname.LastIndexOf("."));
                    arcentry._FileType = arcentry.FileExt;

                    //Gets the path of the selected node to inject here.
                    string nodepath = tree.SelectedNode.FullPath;
                    nodepath = nodepath.Substring(nodepath.IndexOf("\\") + 1);

                    string[] sepstr = { "\\"};
                    arcentry.EntryDirs = nodepath.Split(sepstr, StringSplitOptions.RemoveEmptyEntries);
                    arcentry.EntryName = arcentry.FileName;

                    //Looks through the archive_filetypes.cfg file to find the typehash associated with the extension.
                    try
                    {
                        using (var sr2 = new StreamReader("archive_filetypes.cfg"))
                        {
                            while (!sr2.EndOfStream)
                            {
                                var keyword = Console.ReadLine() ?? arcentry.FileExt;
                                var line = sr2.ReadLine();
                                if (String.IsNullOrEmpty(line)) continue;
                                if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                                {
                                    arcentry.TypeHash = line;
                                    arcentry.TypeHash = arcentry.TypeHash.Split(' ')[0];

                                    break;
                                }
                            }
                        }
                    }
                    catch (FileNotFoundException)
                    {
                        MessageBox.Show("I cannot find and/or access archive_filetypes.cfg so I cannot finish parsing the arc.", "Oh Boy");
                        
                    }


                }
            }
            catch(Exception ex)
            {

            }



            return arcentry;
        }

        //Looks through the cfg file to find the Typehash and returns it.
        public static string TypeHashFinder(ArcEntry arctry)
        {
            string TypeHash = "";


            //Looks through the archive_filetypes.cfg file to find the typehash associated with the extension.
            try
            {
                using (var sr2 = new StreamReader("archive_filetypes.cfg"))
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
                MessageBox.Show("I cannot find and/or access archive_filetypes.cfg so I cannot finish parsing the arc.", "Oh Boy");

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
