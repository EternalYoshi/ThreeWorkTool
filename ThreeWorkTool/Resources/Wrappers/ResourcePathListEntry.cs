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
using ThreeWorkTool.Resources.Archives;
using ThreeWorkTool.Resources.Utility;
using ThreeWorkTool.Resources.Wrappers;


namespace ThreeWorkTool.Resources.Wrappers
{
    public class ResourcePathListEntry : DefaultWrapper
    {
        public string Magic;
        public string Constant;
        public int EntryCount;
        public byte[] WTemp;
        public List<string> TextBackup;
        public List<PathEntries> EntryList;

        public struct PathEntries
        {
            public string FullPath;
            public string TypeHash;
            public string FileExt;
            public string TotalName;
        }


        public static ResourcePathListEntry FillRPLEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
        {
            ResourcePathListEntry RPLentry = new ResourcePathListEntry();

            FillEntry(filename, subnames, tree, br, c, ID, RPLentry, filetype);

            RPLentry._FileName = RPLentry.TrueName;
            RPLentry._FileType = RPLentry.FileExt;
            RPLentry._FileLength = RPLentry.DSize;

            ASCIIEncoding ascii = new ASCIIEncoding();

            //Specific file type work goes here!

            //Gets the Magic.
            RPLentry.Magic = BitConverter.ToString(RPLentry.UncompressedData, 0, 4).Replace("-", string.Empty);
            RPLentry._EntryTotal = BitConverter.ToInt32(RPLentry.UncompressedData, 12);
            RPLentry.EntryCount = RPLentry._EntryTotal;

            //If the entry count is too high then it means the lrp file is likely corrupted or malinformed, meaning we put out an error message and force a close lest we hang.
            if (RPLentry.EntryCount > 76310)
            {

                string errorpath = "";
                foreach (string s in subnames) { errorpath = errorpath + s + "\\"; }
                errorpath = errorpath + RPLentry._FileName + ".lrp";
                MessageBox.Show("The file located at \n" + errorpath + "\nis malinformed and has way too many entries.\nFor this reason, I cannot continue reading this arc file and must close. Sorry." ,"ATTENTION", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Process.GetCurrentProcess().Kill();

            }


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
                    //Gets the Corrected path for the cfg.
                    string ProperPath = "";
                    ProperPath = Globals.ToolPath + "archive_filetypes.cfg";
                    using (var sr = new StreamReader(ProperPath))
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

                for (int t = 0; t < rple.TextBackup.Count; t++)
                {
                    texbox.Text = texbox.Text + rple.TextBackup[t];
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
            for (int k = 0; k < NewEntryCount; k++)
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
                        //Gets the Corrected path for the cfg.
                        string ProperPath = "";
                        ProperPath = Globals.ToolPath + "archive_filetypes.cfg";
                        using (var sr = new StreamReader(ProperPath))
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
                if (ExtFound == true)
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

            if (EstimatedSize > (NEWLRP.Count - 16))
            {
                for (int vv = 0; EstimatedSize > (NEWLRP.Count - 16); vv++)
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

            //Updates the TextBackup used for display.
            rple.TextBackup = new List<string>();

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

                    InsertKnownEntry(tree, node, filename, rplentry, bnr);

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
                            //Gets the Corrected path for the cfg.
                            string ProperPath = "";
                            ProperPath = Globals.ToolPath + "archive_filetypes.cfg";
                            using (var sr = new StreamReader(ProperPath))
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
                using (BinaryReader br = new BinaryReader(File.OpenRead(filename)))
                {

                    ReplaceKnownEntry(tree, node, filename, rpathentry, rpoldentry);

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
                            //Gets the Corrected path for the cfg.
                            string ProperPath = "";
                            ProperPath = Globals.ToolPath + "archive_filetypes.cfg";
                            using (var sr = new StreamReader(ProperPath))
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
                            br.Close();
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
