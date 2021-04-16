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

namespace ThreeWorkTool.Resources.Wrappers
{
    public class ResourcePathListEntry
    {
        public string Magic;
        public string Constant;
        public int CSize;
        public int DSize;
        public int EntryCount;
        public int OffsetTemp;
        public string EntryName;
        public int AOffset;
        public int EntryID;
        public byte[] CompressedData;
        public byte[] UncompressedData;
        public static StringBuilder SBname;
        public string[] EntryDirs;
        public string TrueName;
        public byte[] WTemp;
        public string FileExt;
        public List<string> TextBackup;

        public struct PathEntries
        {
            public string FullPath;
            public string TypeHash;
            public string FileExt;
            public string TotalName;
        }

        public List<PathEntries> EntryList;

        public static ResourcePathListEntry FillRPLEntry(string filename, List<string> subnames, TreeView tree, byte[] Bytes, int c, int ID, Type filetype = null)
        {
            ResourcePathListEntry RPLentry = new ResourcePathListEntry();

            using (FileStream fs = File.OpenRead(filename))
            {
                //This block gets the name of the entry.

                RPLentry.OffsetTemp = c;
                RPLentry.EntryID = ID;
                byte[] BTemp;
                BTemp = new byte[] { };
                BTemp = Bytes.Skip(RPLentry.OffsetTemp).Take(64).Where(x => x != 0x00).ToArray();

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

                //Compressed Data size.
                BTemp = new byte[] { };
                c = c + 68;
                BTemp = Bytes.Skip(c).Take(4).ToArray();
                BTemp.Reverse();
                RPLentry.CSize = BitConverter.ToInt32(BTemp, 0);

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
                RPLentry.DSize = (int)DIFF;

                //Data Offset.
                BTemp = new byte[] { };
                c = c + 4;
                BTemp = Bytes.Skip(c).Take(4).ToArray();
                BTemp.Reverse();
                RPLentry.AOffset = BitConverter.ToInt32(BTemp, 0);

                //Compressed Data.
                BTemp = new byte[] { };
                c = RPLentry.AOffset;
                BTemp = Bytes.Skip(c).Take(RPLentry.CSize).ToArray();
                RPLentry.CompressedData = BTemp;


                //Namestuff.
                RPLentry.EntryName = Tempname;

                //Ensures existing subdirectories are cleared so the directories for files are displayed correctly.
                if (subnames != null)
                {
                    if (subnames.Count > 0)
                    {
                        subnames.Clear();
                    }
                }

                //Gets the filename without subdirectories.
                if (RPLentry.EntryName.Contains("\\"))
                {
                    string[] splstr = RPLentry.EntryName.Split('\\');

                    //foreach (string v in splstr)
                    for (int v = 0; v < (splstr.Length - 1); v++)
                    {
                        if (!subnames.Contains(splstr[v]))
                        {
                            subnames.Add(splstr[v]);
                        }
                    }


                    RPLentry.TrueName = RPLentry.EntryName.Substring(RPLentry.EntryName.IndexOf("\\") + 1);
                    Array.Clear(splstr, 0, splstr.Length);

                    while (RPLentry.TrueName.Contains("\\"))
                    {
                        RPLentry.TrueName = RPLentry.TrueName.Substring(RPLentry.TrueName.IndexOf("\\") + 1);
                    }
                }
                else
                {
                    RPLentry.TrueName = RPLentry.EntryName;
                }

                RPLentry._FileName = RPLentry.TrueName;

                RPLentry.EntryDirs = subnames.ToArray();
                RPLentry.FileExt = ".lrp";
                RPLentry.EntryName = RPLentry.EntryName + RPLentry.FileExt;
                RPLentry._FileName = RPLentry.TrueName;
                RPLentry._FileType = RPLentry.FileExt;
                RPLentry._FileLength = RPLentry.DSize;

                //Decompression Time.
                RPLentry.UncompressedData = ZlibStream.UncompressBuffer(RPLentry.CompressedData);

                //Specific file type work goes here!

                //Gets the Magic.
                byte[] MTemp = new byte[4];
                string STemp = " ";
                Array.Copy(RPLentry.UncompressedData,0,MTemp,0,4);
                RPLentry.Magic = BytesToString(MTemp, RPLentry.Magic);

                Array.Copy(RPLentry.UncompressedData, 12, MTemp, 0, 4);
                Array.Reverse(MTemp);
                STemp = BytesToString(MTemp,STemp);

                int ECTemp = Convert.ToInt32(STemp, 16);
                RPLentry._EntryTotal = ECTemp;
                RPLentry.EntryCount = ECTemp;

                //Starts occupying the entry list via structs. 
                RPLentry.EntryList = new List<PathEntries>();
                byte[] PLName = new byte[] { };
                byte[] PTHName = new byte[] { };

                int p = 16;
                string Teme;
                string Hame;

                for (int g = 0; g < RPLentry.EntryCount; g++)
                {
                    PathEntries pe = new PathEntries();
                    PLName = RPLentry.UncompressedData.Skip(p).Take(64).Where(x => x != 0x00).ToArray();                
                    Teme = ascii.GetString(PLName);

                    pe.FullPath = Teme;
                    p = p + 64;
                    PTHName = RPLentry.UncompressedData.Skip(p).Take(4).Where(x => x != 0x00).ToArray();
                    Array.Reverse(PTHName);

                    Teme = BytesToString(PTHName, Teme);
                    pe.TypeHash = Teme;

                    try
                    {
                        using (var sr = new StreamReader("archive_filetypes.cfg"))
                        {
                            while (!sr.EndOfStream)
                            {
                                var keyword = Console.ReadLine() ?? Teme;
                                var line = sr.ReadLine();
                                if (String.IsNullOrEmpty(line)) continue;
                                if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                                {
                                    Hame = line;
                                    Hame = Hame.Split(' ')[1];
                                    pe.TotalName = pe.FullPath + Hame;
                                    pe.FileExt = Hame;
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

                    RPLentry.EntryList.Add(pe);
                    p = p + 4;

                }

                RPLentry.TextBackup = new List<string>();

                return RPLentry;

            }
        }

        #region ResourcePathListEntryProperties

        private string _FileName;
        [Category("Resource Path List"), ReadOnlyAttribute(true)]
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

        private string _FileType;
        [Category("Resource Path List"), ReadOnlyAttribute(true)]
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

        private long _FileLength;
        [Category("Resource Path List"), ReadOnlyAttribute(true)]
        public long FileLength
        {

            get
            {
                return _FileLength;
            }
            set
            {
                _FileLength = value;
            }
        }

        private int _EntryTotal;
        [Category("Resource Path List"), ReadOnlyAttribute(true)]
        public int EntryTotal
        {
            get
            {
                return _EntryTotal;
            }
            set
            {
                _EntryTotal = value;
            }
        }

        #endregion

        public static string BytesToString(byte[] bytes, string s)
        {
            string temps;
            string tru = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                temps = bytes[i].ToString("X");
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

        public static byte[] StringToByteArray(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static byte[] BinaryStringToByteArray(string binary)
        {
            int numOfBytes = binary.Length / 8;
            byte[] bytes = new byte[numOfBytes];
            for (int i = 0; i < numOfBytes; ++i)
            {
                bytes[i] = Convert.ToByte(binary.Substring(8 * i, 8), 2);
            }
            return bytes;
        }

        public static TextBox LoadRPLInTextBox(TextBox texbox, ResourcePathListEntry rple)
        {

            texbox.Text = "";

            bool isEmpty = !rple.TextBackup.Any();
            if (isEmpty)
            {
                for (int t = 0; t < rple.EntryList.Count; t++)
                {
                    texbox.Text = texbox.Text + rple.EntryList[t].TotalName + System.Environment.NewLine;
                    rple.TextBackup.Add(rple.EntryList[t].TotalName + System.Environment.NewLine);
                }
            }
            else
            {

                for (int t = 0; t < rple.EntryList.Count; t++)
                {
                    texbox.Text = texbox.Text + rple.EntryList[t].TotalName + System.Environment.NewLine;
                }
                /*
                for (int t = 0; t < rple.TextBackup.Count; t++)
                {
                    texbox.Text = texbox.Text + rple.TextBackup[t];
                }
                */
            }



            return texbox;
        }

        public static void UpdateRPLList(TextBox texbox, ResourcePathListEntry rple)
        {
            string txbtxt = texbox.Text;
            string[] SPLT = new string[] { };
            if (texbox.Text != " ")
            {
                //SPLT = txbtxt.Split('\n');
                //rple.TextBackup = SPLT.ToList();
                RefreshRPLList(texbox, rple);
            }

        }

        public static void RenewRPLList(TextBox texbox, ResourcePathListEntry rple)
        {
            //Reconstructs the Entry List.
            string txbtxt = texbox.Text;
            string[] SPLT = new string[] { };

            SPLT = txbtxt.Split('\n');

            rple.EntryCount = SPLT.Length;
            rple.EntryList = new List<PathEntries>();

            for (int g = 0; g < rple.EntryCount; g++)
            {
                PathEntries pe = new PathEntries();
                pe.TotalName = SPLT[g];
                rple.EntryList.Add(pe);
            }
        }

        public static void RefreshRPLList(TextBox texbox, ResourcePathListEntry rple)
        {
            //Reconstructs the Entry List.
            rple.EntryCount = rple.TextBackup.Count;
            rple.EntryList = new List<PathEntries>();

            for (int g = 0; g < rple.EntryCount; g++)
            {
                PathEntries pe = new PathEntries();
                pe.TotalName = rple.TextBackup[g];
                rple.EntryList.Add(pe);
            }
            LoadRPLInTextBox(texbox, rple);
        }

        public static ResourcePathListEntry InsertRPL(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            ResourcePathListEntry rplentry = new ResourcePathListEntry();

            try
            {
                using (FileStream fs = File.OpenRead(filename))
                {
                    //We build the arcentry starting from the uncompressed data.
                    rplentry.UncompressedData = System.IO.File.ReadAllBytes(filename);
                    rplentry._FileLength = rplentry.UncompressedData.Length;
                    rplentry.DSize = rplentry.UncompressedData.Length;

                    //Then Compress.
                    rplentry.CompressedData = Zlibber.Compressor(rplentry.UncompressedData);
                    rplentry.CSize = rplentry.CompressedData.Length;

                    //Gets the filename of the file to inject without the directory.
                    string trname = filename;
                    while (trname.Contains("\\"))
                    {
                        trname = trname.Substring(trname.IndexOf("\\") + 1);
                    }

                    rplentry.TrueName = trname;
                    rplentry._FileName = rplentry.TrueName;
                    rplentry.TrueName = Path.GetFileNameWithoutExtension(trname);
                    rplentry.FileExt = trname.Substring(trname.LastIndexOf("."));
                    rplentry._FileType = rplentry.FileExt;

                    //Gets the path of the selected node to inject here.
                    string nodepath = tree.SelectedNode.FullPath;
                    nodepath = nodepath.Substring(nodepath.IndexOf("\\") + 1);

                    string[] sepstr = { "\\" };
                    rplentry.EntryDirs = nodepath.Split(sepstr, StringSplitOptions.RemoveEmptyEntries);
                    rplentry.EntryName = rplentry.FileName;

                    //Specific file type work goes here!

                    string Tempname;
                    ASCIIEncoding ascii = new ASCIIEncoding();

                    //Gets the Magic.
                    byte[] MTemp = new byte[4];
                    string STemp = " ";
                    Array.Copy(rplentry.UncompressedData, 0, MTemp, 0, 4);
                    rplentry.Magic = BytesToString(MTemp, rplentry.Magic);

                    Array.Copy(rplentry.UncompressedData, 12, MTemp, 0, 4);
                    Array.Reverse(MTemp);
                    STemp = BytesToString(MTemp, STemp);

                    int ECTemp = Convert.ToInt32(STemp, 16);
                    rplentry._EntryTotal = ECTemp;
                    rplentry.EntryCount = ECTemp;

                    //Starts occupying the entry list via structs. 
                    rplentry.EntryList = new List<PathEntries>();
                    byte[] PLName = new byte[] { };
                    byte[] PTHName = new byte[] { };

                    int p = 16;
                    string Teme;
                    string Hame;

                    for (int g = 0; g < rplentry.EntryCount; g++)
                    {
                        PathEntries pe = new PathEntries();
                        PLName = rplentry.UncompressedData.Skip(p).Take(64).Where(x => x != 0x00).ToArray();
                        Teme = ascii.GetString(PLName);

                        pe.FullPath = Teme;
                        p = p + 64;
                        PTHName = rplentry.UncompressedData.Skip(p).Take(4).Where(x => x != 0x00).ToArray();
                        Array.Reverse(PTHName);

                        Teme = BytesToString(PTHName, Teme);
                        pe.TypeHash = Teme;

                        try
                        {
                            using (var sr = new StreamReader("archive_filetypes.cfg"))
                            {
                                while (!sr.EndOfStream)
                                {
                                    var keyword = Console.ReadLine() ?? Teme;
                                    var line = sr.ReadLine();
                                    if (String.IsNullOrEmpty(line)) continue;
                                    if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                                    {
                                        Hame = line;
                                        Hame = Hame.Split(' ')[1];
                                        pe.TotalName = pe.FullPath + Hame;
                                        pe.FileExt = Hame;
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

                        rplentry.EntryList.Add(pe);
                        p = p + 4;

                    }

                    rplentry.TextBackup = new List<string>();

                }
            }
            catch (Exception ex)
            {

            }



            return rplentry;
        }

        public static ResourcePathListEntry ReplaceRPL(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            ResourcePathListEntry rpathentry = new ResourcePathListEntry();
            ResourcePathListEntry rpoldentry = new ResourcePathListEntry();

            tree.BeginUpdate();

            //Gotta Fix this up then test insert and replacing.
            try
            {
                using (FileStream fs = File.OpenRead(filename))
                {
                    //We build the arcentry starting from the uncompressed data.
                    rpathentry.UncompressedData = System.IO.File.ReadAllBytes(filename);

                    //Then Compress.
                    rpathentry.CompressedData = Zlibber.Compressor(rpathentry.UncompressedData);

                    //Gets the filename of the file to inject without the directory.
                    string trname = filename;
                    while (trname.Contains("\\"))
                    {
                        trname = trname.Substring(trname.IndexOf("\\") + 1);
                    }

                    //Enters name related parameters of the arcentry.
                    rpathentry.TrueName = trname;
                    rpathentry._FileName = rpathentry.TrueName;
                    rpathentry.TrueName = Path.GetFileNameWithoutExtension(trname);
                    rpathentry.FileExt = trname.Substring(trname.LastIndexOf("."));
                    rpathentry._FileType = rpathentry.FileExt;

                    string TypeHash = "";

                    //Looks through the archive_filetypes.cfg file to find the typehash associated with the extension.
                    try
                    {
                        using (var sr2 = new StreamReader("archive_filetypes.cfg"))
                        {
                            while (!sr2.EndOfStream)
                            {
                                var keyword = Console.ReadLine() ?? rpathentry.FileExt;
                                var line = sr2.ReadLine();
                                if (String.IsNullOrEmpty(line)) continue;
                                if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                                {
                                    TypeHash = line;
                                    TypeHash = TypeHash.Split(' ')[0];
                                    //arcentry.TypeHash = TypeHash;
                                    break;
                                }
                            }
                        }
                    }
                    catch (FileNotFoundException)
                    {
                        MessageBox.Show("I cannot find and/or access archive_filetypes.cfg so I cannot finish parsing the arc.", "Oh Boy");

                    }

                    ASCIIEncoding ascii = new ASCIIEncoding();

                    //Gets the Magic.
                    byte[] MTemp = new byte[4];
                    string STemp = " ";
                    Array.Copy(rpathentry.UncompressedData, 0, MTemp, 0, 4);
                    rpathentry.Magic = BytesToString(MTemp, rpathentry.Magic);

                    Array.Copy(rpathentry.UncompressedData, 12, MTemp, 0, 4);
                    Array.Reverse(MTemp);
                    STemp = BytesToString(MTemp, STemp);

                    int ECTemp = Convert.ToInt32(STemp, 16);
                    rpathentry._EntryTotal = ECTemp;
                    rpathentry.EntryCount = ECTemp;

                    //Starts occupying the entry list via structs. 
                    rpathentry.EntryList = new List<PathEntries>();
                    byte[] PLName = new byte[] { };
                    byte[] PTHName = new byte[] { };

                    int p = 16;
                    string Teme;
                    string Hame;

                    for (int g = 0; g < rpathentry.EntryCount; g++)
                    {
                        PathEntries pe = new PathEntries();
                        PLName = rpathentry.UncompressedData.Skip(p).Take(64).Where(x => x != 0x00).ToArray();
                        Teme = ascii.GetString(PLName);

                        pe.FullPath = Teme;
                        p = p + 64;
                        PTHName = rpathentry.UncompressedData.Skip(p).Take(4).Where(x => x != 0x00).ToArray();
                        Array.Reverse(PTHName);

                        Teme = BytesToString(PTHName, Teme);
                        pe.TypeHash = Teme;

                        try
                        {
                            using (var sr = new StreamReader("archive_filetypes.cfg"))
                            {
                                while (!sr.EndOfStream)
                                {
                                    var keyword = Console.ReadLine() ?? Teme;
                                    var line = sr.ReadLine();
                                    if (String.IsNullOrEmpty(line)) continue;
                                    if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                                    {
                                        Hame = line;
                                        Hame = Hame.Split(' ')[1];
                                        pe.TotalName = pe.FullPath + Hame;
                                        pe.FileExt = Hame;
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

                        rpathentry.EntryList.Add(pe);
                        p = p + 4;

                    }

                    rpathentry.TextBackup = new List<string>();
                    rpathentry._FileLength = rpathentry.UncompressedData.Length;

                    //Hmmm.

                    var tag = node.Tag;
                    if (tag is ResourcePathListEntry)
                    {
                        rpoldentry = tag as ResourcePathListEntry;
                    }
                    string path = "";
                    int index = rpoldentry.EntryName.LastIndexOf("\\");
                    if (index > 0)
                    {
                        path = rpoldentry.EntryName.Substring(0, index);
                    }

                    rpathentry.EntryName = path + "\\" + rpathentry.TrueName;

                    tag = rpathentry;

                    if (node.Tag is ResourcePathListEntry)
                    {
                        node.Tag = rpathentry;
                        node.Name = Path.GetFileNameWithoutExtension(rpathentry.EntryName);
                        node.Text = Path.GetFileNameWithoutExtension(rpathentry.EntryName);

                    }

                    var aew = node as ArcEntryWrapper;

                    string type = node.GetType().ToString();
                    if (type == "ThreeWorkTool.Resources.Wrappers.ArcEntryWrapper")
                    {
                        aew.entryfile = rpathentry;
                    }

                    node = aew;
                    node.entryfile = rpathentry;
                    tree.EndUpdate();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Read error. Is the file readable?");
            }



            return node.entryfile as ResourcePathListEntry;
        }

        /*
         
                public static ResourcePathListEntry ReplaceRPL(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            ResourcePathListEntry rpathentry = new ResourcePathListEntry();
            ResourcePathListEntry rpoldentry = new ResourcePathListEntry();

            tree.BeginUpdate();

            //Gotta Fix this up then test insert and replacing.
            try
            {
                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
                {
                    //We build the arcentry starting from the uncompressed data.
                    rpathentry.UncompressedData = System.IO.File.ReadAllBytes(filename);

                    //Then Compress.
                    rpathentry.CompressedData = Zlibber.Compressor(rpathentry.UncompressedData);

                    //Gets the filename of the file to inject without the directory.
                    string trname = filename;
                    while (trname.Contains("\\"))
                    {
                        trname = trname.Substring(trname.IndexOf("\\") + 1);
                    }

                    //Enters name related parameters of the arcentry.
                    rpathentry.TrueName = trname;
                    rpathentry._FileName = rpathentry.TrueName;
                    rpathentry.TrueName = Path.GetFileNameWithoutExtension(trname);
                    rpathentry.FileExt = trname.Substring(trname.LastIndexOf("."));
                    rpathentry._FileType = rpathentry.FileExt;

                    string TypeHash = "";

                    //Looks through the archive_filetypes.cfg file to find the typehash associated with the extension.
                    try
                    {
                        using (var sr2 = new StreamReader("archive_filetypes.cfg"))
                        {
                            while (!sr2.EndOfStream)
                            {
                                var keyword = Console.ReadLine() ?? rpathentry.FileExt;
                                var line = sr2.ReadLine();
                                if (String.IsNullOrEmpty(line)) continue;
                                if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                                {
                                    TypeHash = line;
                                    TypeHash = TypeHash.Split(' ')[0];
                                    //arcentry.TypeHash = TypeHash;
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
                    if (tag is ResourcePathListEntry)
                    {
                        rpoldentry = tag as ResourcePathListEntry;
                    }
                    string path = "";
                    int index = rpoldentry.EntryName.LastIndexOf("\\");
                    if (index > 0)
                    {
                        path = rpoldentry.EntryName.Substring(0, index);
                    }

                    rpathentry.EntryName = path + "\\" + rpathentry.TrueName;

                    tag = rpathentry;

                    if (node.Tag is ResourcePathListEntry)
                    {
                        node.Tag = rpathentry;
                        node.Name = Path.GetFileNameWithoutExtension(rpathentry.EntryName);
                        node.Text = Path.GetFileNameWithoutExtension(rpathentry.EntryName);

                    }

                    var aew = node as ArcEntryWrapper;

                    string type = node.GetType().ToString();
                    if (type == "ThreeWorkTool.Resources.Wrappers.ArcEntryWrapper")
                    {
                        aew.entryfile = rpathentry;
                    }

                    node = aew;
                    node.entryfile = rpathentry;
                    tree.EndUpdate();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Read error. Is the file readable?");
            }



            return node.entryfile as ResourcePathListEntry;
        }

         
         */

    }
}
