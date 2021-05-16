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
using ThreeWorkTool.Resources.Utility;

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

        public static ResourcePathListEntry FillRPLEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
        {
            ResourcePathListEntry RPLentry = new ResourcePathListEntry();


            //This block gets the name of the entry.
            RPLentry.OffsetTemp = c;
            RPLentry.EntryID = ID;
            List<byte> BTemp = new List<byte>();
            br.BaseStream.Position = RPLentry.OffsetTemp;
            BTemp.AddRange(br.ReadBytes(64));
            BTemp.RemoveAll(ByteUtilitarian.IsZeroByte);

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
                Tempname = ascii.GetString(BTemp.ToArray());

            //Compressed Data size.
            BTemp = new List<byte>();
            c = c + 68;
            br.BaseStream.Position = c;
            BTemp.AddRange(br.ReadBytes(4));
            RPLentry.CSize = BitConverter.ToInt32(BTemp.ToArray(), 0);

            //Uncompressed Data size.
            BTemp = new List<byte>();
            c = c + 4;
            br.BaseStream.Position = c;
            BTemp.AddRange(br.ReadBytes(4));
            BTemp.Reverse();
            string TempStr = "";
            TempStr = ByteUtilitarian.BytesToStringL2(BTemp, TempStr);
            BigInteger BN1, BN2, DIFF;
            BN2 = BigInteger.Parse("40000000", NumberStyles.HexNumber);
            BN1 = BigInteger.Parse(TempStr, NumberStyles.HexNumber);
            DIFF = BN1 - BN2;
            RPLentry.DSize = (int)DIFF;

            //Data Offset.
            BTemp = new List<byte>();
            c = c + 4;
            br.BaseStream.Position = c;
            BTemp.AddRange(br.ReadBytes(4));
            RPLentry.AOffset = BitConverter.ToInt32(BTemp.ToArray(), 0);

            //Compressed Data.
            BTemp = new List<byte>();
            c = RPLentry.AOffset;
            br.BaseStream.Position = c;
            BTemp.AddRange(br.ReadBytes(RPLentry.CSize));
            RPLentry.CompressedData = BTemp.ToArray();


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
                RPLentry.Magic = ByteUtilitarian.BytesToString(MTemp, RPLentry.Magic);

                Array.Copy(RPLentry.UncompressedData, 12, MTemp, 0, 4);
                Array.Reverse(MTemp);
                STemp = ByteUtilitarian.BytesToString(MTemp,STemp);

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

                    Teme = ByteUtilitarian.BytesToString(PTHName, Teme);
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
                    using (StreamWriter sw = File.AppendText("Log.txt"))
                    {
                        sw.WriteLine("Cannot find archive_filetypes.cfg and thus cannot continue parsing.");
                    }
                    return null;
                    }

                    RPLentry.EntryList.Add(pe);
                    p = p + 4;

                }

                RPLentry.TextBackup = new List<string>();

                return RPLentry;

            
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

        public static ResourcePathListEntry RenewRPLList(TextBox texbox, ResourcePathListEntry rple)
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

            //Rebuilds the Decompressed data Array.
            //byte[] NewLRP = new byte[] { };
            List<byte> NEWLRP = new List<byte>();
            byte[] HeaderLRP = { 0x4C, 0x52, 0x50, 0x00, 0x00, 0x01, 0xFE, 0xFF };
            NEWLRP.AddRange(HeaderLRP);
            int NewEntryCount = rple.EntryCount;
            //if (rple.EntryList[(rple.EntryCount - 1)].TotalName == "" || rple.EntryList[(rple.EntryCount - 1)].TotalName == " ")
            if (string.IsNullOrWhiteSpace(rple.EntryList[(rple.EntryCount - 1)].TotalName))
            {
                NewEntryCount--;
            }
            int ProjectedSize = NewEntryCount * 68;
            int EstimatedSize = ((int)Math.Round(ProjectedSize / 16.0, MidpointRounding.AwayFromZero) * 16);
            EstimatedSize = EstimatedSize + 48;

            //byte[] LRPEntryTotal = { Convert.ToByte(NewEntryCount), 0x00};
            byte[] LRPEntryTotal = new byte[4];
            LRPEntryTotal[3] = Convert.ToByte(NewEntryCount);
            Array.Reverse(LRPEntryTotal);

            //Converts an integer to 4 bytes in a roundabout way.
            string LRPSize = EstimatedSize.ToString("X8");
            byte[] LRPProjSize = new byte[4];
            LRPProjSize = ByteUtilitarian.StringToByteArray(LRPSize);
            Array.Reverse(LRPProjSize);

            //Finishes the header for the new LRP built from what you see on the textbox.
            NEWLRP.AddRange(LRPProjSize);
            NEWLRP.AddRange(LRPEntryTotal);

            string ENTemp = "";
            string RPTemp = "";
            string HashStr = "";
            byte[] HashTempDX = new byte[4];

            //Starts building the entries.
            for (int k= 0; k< NewEntryCount; k++)
            {
                ENTemp = rple.EntryList[k].TotalName;
                ENTemp = ENTemp.Replace("\r", "");
                int inp = (ENTemp.IndexOf("."));
                string[] SplTemp = ENTemp.Split('.');
                if (inp < 0)
                {
                    RPTemp = "";
                }
                else
                {
                    RPTemp = ENTemp.Substring(inp, ENTemp.Length - inp);
                }
                ENTemp = SplTemp[0];
                bool ExtFound = false;

                int NumberChars = ENTemp.Length;
                byte[] namebuffer = Encoding.ASCII.GetBytes(ENTemp);
                int nblength = namebuffer.Length;

                //Space for name is 64 bytes so we make a byte array with that size and then inject the name data in it.
                byte[] writenamedata = new byte[64];
                Array.Clear(writenamedata, 0, writenamedata.Length);

                for (int i = 0; i < namebuffer.Length; ++i)
                {
                    writenamedata[i] = namebuffer[i];
                }

                NEWLRP.AddRange(writenamedata);

                if (RPTemp == "")
                {
                    HashTempDX[0] = 0xFF;
                    HashTempDX[1] = 0xFF;
                    HashTempDX[2] = 0xFF;
                    HashTempDX[3] = 0xFF;
                }
                else
                {
                    //Typehash stuff.
                    try
                    {
                        using (var sr = new StreamReader("archive_filetypes.cfg"))
                        {
                            while (!sr.EndOfStream)
                            {
                                var keyword = Console.ReadLine() ?? RPTemp;
                                var line = sr.ReadLine();
                                if (String.IsNullOrEmpty(line)) continue;
                                if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                                {
                                    ExtFound = true;
                                    HashStr = line;
                                    HashStr = HashStr.Split(' ')[0];
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
                            sw.WriteLine("Cannot find archive_filetypes.cfg and thus cannot continue parsing.");
                        }
                        return null;
                    }
                }
                if(ExtFound == true)
                {
                    HashTempDX = ByteUtilitarian.StringToByteArray(HashStr);
                    Array.Reverse(HashTempDX);
                }
                else
                {

                    HashTempDX[0] = 0xFF;
                    HashTempDX[1] = 0xFF;
                    HashTempDX[2] = 0xFF;
                    HashTempDX[3] = 0xFF;
                }

                NEWLRP.AddRange(HashTempDX);

            }

            if(EstimatedSize > (NEWLRP.Count - 16))
            {
                for(int vv=0; EstimatedSize > (NEWLRP.Count - 16); vv++)
                {
                    NEWLRP.Add(0x00);
                }
            }

            rple.UncompressedData = NEWLRP.ToArray();
            rple.DSize = rple.UncompressedData.Length;

            rple.CompressedData = Zlibber.Compressor(rple.UncompressedData);
            rple.CSize = rple.CompressedData.Length;

            rple._FileLength = rple.UncompressedData.Length;
            rple._EntryTotal = NewEntryCount;

            return rple;

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
                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
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
                    ASCIIEncoding ascii = new ASCIIEncoding();

                    //Gets the Magic.
                    byte[] MTemp = new byte[4];
                    string STemp = " ";
                    Array.Copy(rplentry.UncompressedData, 0, MTemp, 0, 4);
                    rplentry.Magic = ByteUtilitarian.BytesToString(MTemp, rplentry.Magic);

                    Array.Copy(rplentry.UncompressedData, 12, MTemp, 0, 4);
                    Array.Reverse(MTemp);
                    STemp = ByteUtilitarian.BytesToString(MTemp, STemp);

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

                        Teme = ByteUtilitarian.BytesToString(PTHName, Teme);
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
                            using (StreamWriter sw = File.AppendText("Log.txt"))
                            {
                                sw.WriteLine("Cannot find archive_filetypes.cfg and thus cannot continue parsing.");
                            }
                            return null;
                        }

                        rplentry.EntryList.Add(pe);
                        p = p + 4;

                    }

                    rplentry.TextBackup = new List<string>();

                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText("Log.txt"))
                {
                    sw.WriteLine("Caught the exception:" + ex);
                }
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
                    rpathentry.Magic = ByteUtilitarian.BytesToString(MTemp, rpathentry.Magic);

                    Array.Copy(rpathentry.UncompressedData, 12, MTemp, 0, 4);
                    Array.Reverse(MTemp);
                    STemp = ByteUtilitarian.BytesToString(MTemp, STemp);

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

                        Teme = ByteUtilitarian.BytesToString(PTHName, Teme);
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
                using (StreamWriter sw = File.AppendText("Log.txt"))
                {
                    sw.WriteLine("Read error. Cannot access the file:" + filename + "\n" + ex);
                }
            }



            return node.entryfile as ResourcePathListEntry;
        }

    }
}
