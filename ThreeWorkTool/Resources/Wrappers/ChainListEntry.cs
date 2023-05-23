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

namespace ThreeWorkTool.Resources.Wrappers
{
    public class ChainListEntry : DefaultWrapper
    {

        public int Unknown04;
        public int TotalEntrySize;
        public int CHNEntryCount;
        public int CCLEntryCount;
        public List<CHNEntry> ChainEntries;
        public List<CCLEntry> ChainCollEntries;
        public List<string> TextBackup;

        public struct CHNEntry
        {
            public string FullPath;
            public string TypeHash;
            public string FileExt;
            public string TotalName;
            public int Flag1;
            public int Flag2;
            public int Flag3;
            public int Flag4;
        }

        public struct CCLEntry
        {
            public string FullPath;
            public string TypeHash;
            public string FileExt;
            public string TotalName;
        }

        public static ChainListEntry FillCSTEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
        {
            ChainListEntry cslentry = new ChainListEntry();
            List<byte> BTemp = new List<byte>();

            FillEntry(filename, subnames, tree, br, c, ID, cslentry, filetype);

            cslentry._FileName = cslentry.TrueName;
            cslentry._DecompressedFileLength = cslentry.UncompressedData.Length;
            cslentry._CompressedFileLength = cslentry.CompressedData.Length;

            cslentry.TypeHash = "326F732E";

            //Type specific work here.
            using (MemoryStream CslStream = new MemoryStream(cslentry.UncompressedData))
            {
                using (BinaryReader bnr = new BinaryReader(CslStream))
                {

                    bnr.BaseStream.Position = 4;
                    cslentry.Unknown04 = bnr.ReadInt32();
                    cslentry.TotalEntrySize = bnr.ReadInt32();
                    cslentry.CHNEntryCount = bnr.ReadInt32();
                    cslentry.CCLEntryCount = bnr.ReadInt32();

                    cslentry.ChainEntries = new List<CHNEntry>();
                    cslentry.ChainCollEntries = new List<CCLEntry>();

                    for (int g = 0; g < cslentry.CHNEntryCount; g++)
                    {
                        CHNEntry cHN = new CHNEntry();
                        cHN.FullPath = Encoding.ASCII.GetString(bnr.ReadBytes(64)).Trim('\0');
                        cHN.TypeHash = ByteUtilitarian.BytesToStringL2R(bnr.ReadBytes(4).ToList(), cHN.TypeHash);

                        try
                        {
                            //Gets the Corrected path for the cfg.
                            string ProperPath = "";
                            ProperPath = Globals.ToolPath + "archive_filetypes.cfg";
                            using (var sr = new StreamReader(ProperPath))
                            {
                                while (!sr.EndOfStream)
                                {
                                    var keyword = Console.ReadLine() ?? cHN.TypeHash;
                                    var line = sr.ReadLine();
                                    if (String.IsNullOrEmpty(line)) continue;
                                    if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                                    {
                                        cHN.FileExt = line;
                                        cHN.FileExt = cHN.FileExt.Split(' ')[1];
                                        cHN.TotalName = cHN.FullPath + cHN.FileExt;
                                        break;
                                    }
                                }
                            }

                        }
                        catch (FileNotFoundException)
                        {
                            MessageBox.Show("I cannot find archive_filetypes.cfg so I cannot finish parsing the arc.", "Oh Boy");
                            string ProperPath = "";
                            ProperPath = Globals.ToolPath + "Log.txt";
                            using (StreamWriter sw = File.AppendText(ProperPath))
                            {
                                sw.WriteLine("Cannot find archive_filetypes.cfg and thus cannot continue parsing.");
                            }
                            return null;
                        }

                        cHN.Flag1 = bnr.ReadInt32();
                        cHN.Flag2 = bnr.ReadInt32();
                        cHN.Flag3 = bnr.ReadInt32();
                        cHN.Flag4 = bnr.ReadInt32();


                        cslentry.ChainEntries.Add(cHN);
                        //bnr.BaseStream.Position = bnr.BaseStream.Position + 16;
                    }

                    for (int h = 0; h < cslentry.CCLEntryCount; h++)
                    {
                        CCLEntry cCL = new CCLEntry();
                        cCL.FullPath = Encoding.ASCII.GetString(bnr.ReadBytes(64)).Trim('\0');
                        cCL.TypeHash = ByteUtilitarian.BytesToStringL2R(bnr.ReadBytes(4).ToList(), cCL.TypeHash);

                        try
                        {
                            //Gets the Corrected path for the cfg.
                            string ProperPath = "";
                            ProperPath = Globals.ToolPath + "archive_filetypes.cfg";
                            using (var sr = new StreamReader(ProperPath))
                            {
                                while (!sr.EndOfStream)
                                {
                                    var keyword = Console.ReadLine() ?? cCL.TypeHash;
                                    var line = sr.ReadLine();
                                    if (String.IsNullOrEmpty(line)) continue;
                                    if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                                    {
                                        cCL.FileExt = line;
                                        cCL.FileExt = cCL.FileExt.Split(' ')[1];
                                        cCL.TotalName = cCL.FullPath + cCL.FileExt;
                                        break;
                                    }
                                }
                            }

                        }
                        catch (FileNotFoundException)
                        {
                            MessageBox.Show("I cannot find archive_filetypes.cfg so I cannot finish parsing the arc.", "Oh Boy");
                            string ProperPath = "";
                            ProperPath = Globals.ToolPath + "Log.txt";
                            using (StreamWriter sw = File.AppendText(ProperPath))
                            {
                                sw.WriteLine("Cannot find archive_filetypes.cfg and thus cannot continue parsing.");
                            }
                            return null;
                        }

                        cslentry.ChainCollEntries.Add(cCL);
                    }

                }
            }

            cslentry.TextBackup = new List<string>();





            return cslentry;
        }

        public static TextBox LoadCSTInTextBox(TextBox texbox, ChainListEntry chlste)
        {

            texbox.Text = "";

            //Inserts Chain phsyics entries.
            bool isEmpty = !chlste.TextBackup.Any();
            if (isEmpty)
            {
                for (int t = 0; t < chlste.ChainEntries.Count; t++)
                {
                    texbox.Text = texbox.Text + chlste.ChainEntries[t].TotalName + " [" + chlste.ChainEntries[t].Flag1 + "," + chlste.ChainEntries[t].Flag2 + "," + chlste.ChainEntries[t].Flag3 + "," + chlste.ChainEntries[t].Flag4 + "]" + Environment.NewLine;
                    chlste.TextBackup.Add(chlste.ChainEntries[t].TotalName + " [" + chlste.ChainEntries[t].Flag1 + "," + chlste.ChainEntries[t].Flag2 + "," + chlste.ChainEntries[t].Flag3 + "," + chlste.ChainEntries[t].Flag4 + "]" + Environment.NewLine);
                }
            }
            else
            {

                for (int t = 0; t < chlste.ChainEntries.Count; t++)
                {
                    texbox.Text = texbox.Text + chlste.ChainEntries[t].TotalName + " [" + chlste.ChainEntries[t].Flag1 + "," + chlste.ChainEntries[t].Flag2 + "," + chlste.ChainEntries[t].Flag3 + "," + chlste.ChainEntries[t].Flag4 + "]" + Environment.NewLine;
                }
            }

            //Inserts ChainCollEntries.
            if (isEmpty)
            {
                for (int t = 0; t < chlste.ChainCollEntries.Count; t++)
                {
                    texbox.Text = texbox.Text + chlste.ChainCollEntries[t].TotalName + Environment.NewLine;
                    chlste.TextBackup.Add(chlste.ChainCollEntries[t].TotalName + Environment.NewLine);
                }
            }
            else
            {

                for (int t = 0; t < chlste.ChainCollEntries.Count; t++)
                {
                    texbox.Text = texbox.Text + chlste.ChainCollEntries[t].TotalName + Environment.NewLine;
                }
            }

            return texbox;
        }

        public static void UpdateCSTList(TextBox texbox, ChainListEntry chlste)
        {
            string txbtxt = texbox.Text;
            string[] SPLT = new string[] { };
            if (texbox.Text != " ")
            {
                RefreshCSTList(texbox, chlste);
            }

        }

        public static ChainListEntry RenewCSTList(TextBox texbox, ChainListEntry chlste)
        {

            //Reconstructs the Lists.
            string txbtxt = texbox.Text;
            string[] SPLT = new string[] { };
            string ExtTemp = "";

            SPLT = txbtxt.Split('\n');
            int index = 0;
            int brindex = 0;
            int secondbrindex = 0;
            int thirdbrindex = 0;
            int fourthbrindex = 0;
            int endbrindex = 0;
            string strtemp = "";
            chlste.ChainEntries = new List<CHNEntry>();
            chlste.ChainCollEntries = new List<CCLEntry>();

            for (int i = 0; i < SPLT.Length; i++)
            {
                bool Isvalidline = SPLT[i].Contains(".");

                if (Isvalidline == true)
                {
                    index = SPLT[i].LastIndexOf(".");
                    brindex = (SPLT[i].IndexOf("[") + 1);
                    secondbrindex = (ByteUtilitarian.GetNthIndex(SPLT[i], ',', 1));
                    thirdbrindex = (ByteUtilitarian.GetNthIndex(SPLT[i],',',2));
                    fourthbrindex = (ByteUtilitarian.GetNthIndex(SPLT[i], ',', 3));
                    endbrindex = SPLT[i].LastIndexOf("]");
                    ExtTemp = SPLT[i].Substring(index, 4);

                    if (ExtTemp == ".chn" || ExtTemp == "chn")
                    {
                        CHNEntry cHN = new CHNEntry();
                        cHN.FullPath = SPLT[i].Substring(0, index);
                        cHN.TypeHash = "3E363245";
                        cHN.TotalName = cHN.FullPath + ExtTemp;
                        cHN.FileExt = SPLT[i].Substring((index), 4);

                        strtemp = SPLT[i].Substring(brindex,(secondbrindex - brindex));
                        cHN.Flag1 = Convert.ToInt32(strtemp);

                        strtemp = SPLT[i].Substring((secondbrindex+1), ((thirdbrindex-1) - secondbrindex));
                        cHN.Flag2 = Convert.ToInt32(strtemp);

                        strtemp = SPLT[i].Substring((thirdbrindex+1), ((fourthbrindex-1) - thirdbrindex));
                        cHN.Flag3 = Convert.ToInt32(strtemp);

                        strtemp = SPLT[i].Substring((fourthbrindex+1), ((endbrindex-1) - fourthbrindex));
                        cHN.Flag4 = Convert.ToInt32(strtemp);

                        chlste.ChainEntries.Add(cHN);
                    }
                    else if (ExtTemp == ".ccl" || ExtTemp == "ccl")
                    {
                        CCLEntry cCL = new CCLEntry();
                        cCL.FullPath = SPLT[i].Substring(0, index);
                        cCL.TypeHash = "0026E7FF";
                        cCL.TotalName = cCL.FullPath + ExtTemp;
                        cCL.FileExt = SPLT[i].Substring((index), 4);
                        chlste.ChainCollEntries.Add(cCL);
                    }
                }
            }

            chlste.CHNEntryCount = chlste.ChainEntries.Count;
            chlste.CCLEntryCount = chlste.ChainCollEntries.Count;

            //Rebuilds the raw file itself.
            List<byte> NEWCST = new List<byte>();
            byte[] HeaderCST = { 0x43, 0x53, 0x54, 0x00, 0x00, 0x01, 0xFE, 0xFF, 0x00, 0x00, 0x00, 0x00 };
            byte[] CHNHash = { 0x45, 0x32, 0x36, 0x3E };
            byte[] CCLHash = { 0xFF, 0xE7, 0x26, 0x00 };
            byte[] FillerLine = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, };
            byte[] EndingFiller = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};


            NEWCST.AddRange(HeaderCST);

            //Converts the CHN and CCL counts and puts them in the array.
            byte[] BufferA = BitConverter.GetBytes(chlste.ChainEntries.Count);
            NEWCST.AddRange(BufferA);

            byte[] BufferB = BitConverter.GetBytes(chlste.ChainCollEntries.Count);
            NEWCST.AddRange(BufferA);

            //Inserts the CHN data.
            int NewEntryCount = chlste.CHNEntryCount;
            int ChainCount = 0;
            int ChainColCount = 0;
            if (chlste.CHNEntryCount > 0)
            {
                if (string.IsNullOrWhiteSpace(chlste.ChainEntries[(chlste.CHNEntryCount - 1)].TotalName))
                {
                    NewEntryCount--;
                }
                //int ProjectedSize = NewEntryCount * 84;
                //int EstimatedSizeCHN = ((int)Math.Round(ProjectedSize / 16.0, MidpointRounding.AwayFromZero) * 16);

                for (int k = 0; k < NewEntryCount; k++)
                {

                    int NumberChars = chlste.ChainEntries[k].FullPath.Length;
                    byte[] namebuffer = Encoding.ASCII.GetBytes(chlste.ChainEntries[k].FullPath);
                    int nblength = namebuffer.Length;
                    byte[] writenamedata = new byte[64];
                    Array.Clear(writenamedata, 0, writenamedata.Length);

                    for (int l = 0; l < namebuffer.Length; ++l)
                    {
                        writenamedata[l] = namebuffer[l];
                    }

                    NEWCST.AddRange(writenamedata);
                    NEWCST.AddRange(CHNHash);
                    NEWCST.AddRange(BitConverter.GetBytes(chlste.ChainEntries[k].Flag1));
                    NEWCST.AddRange(BitConverter.GetBytes(chlste.ChainEntries[k].Flag2));
                    NEWCST.AddRange(BitConverter.GetBytes(chlste.ChainEntries[k].Flag3));
                    NEWCST.AddRange(BitConverter.GetBytes(chlste.ChainEntries[k].Flag4));
                    ChainCount++;
                }
            }


            //Inserts the CCL data.
            NewEntryCount = chlste.CCLEntryCount;

            if (chlste.CCLEntryCount > 0)
            {
                if (string.IsNullOrWhiteSpace(chlste.ChainCollEntries[(chlste.CCLEntryCount - 1)].TotalName))
                {
                    NewEntryCount--;
                }

                for (int m = 0; m < NewEntryCount; m++)
                {

                    int NumberChars = chlste.ChainCollEntries[m].FullPath.Length;
                    byte[] namebuffer = Encoding.ASCII.GetBytes(chlste.ChainCollEntries[m].FullPath);
                    int nblength = namebuffer.Length;
                    byte[] writenamedata = new byte[64];
                    Array.Clear(writenamedata, 0, writenamedata.Length);

                    for (int l = 0; l < namebuffer.Length; ++l)
                    {
                        writenamedata[l] = namebuffer[l];
                    }

                    NEWCST.AddRange(writenamedata);
                    NEWCST.AddRange(CCLHash);
                    ChainColCount++;
                }
            }
            //Fills in 48 blank bytes at the end.
            NEWCST.AddRange(EndingFiller);

            //Updates the parameters of the ChainListEntry.
            chlste.UncompressedData = NEWCST.ToArray();

            //Updates the counts in the byte array.
            int entrysize = (NEWCST.Count - 20);
            byte[] ETemp = new byte[4];
            ETemp = BitConverter.GetBytes(entrysize);
            Buffer.BlockCopy(ETemp, 0, chlste.UncompressedData, 8, 4);

            ETemp = BitConverter.GetBytes(ChainCount);
            Buffer.BlockCopy(ETemp, 0, chlste.UncompressedData, 12, 4);

            ETemp = BitConverter.GetBytes(ChainColCount);
            Buffer.BlockCopy(ETemp, 0, chlste.UncompressedData, 16, 4);


            chlste.DSize = chlste.UncompressedData.Length;

            chlste.CompressedData = Zlibber.Compressor(chlste.UncompressedData);
            chlste.CSize = chlste.CompressedData.Length;

            chlste.CCLEntryCount = ChainColCount;
            chlste.CHNEntryCount = ChainCount;

            return chlste;

        }

        public static void RefreshCSTList(TextBox texbox, ChainListEntry chlste)
        {
            //Reconstructs the Entry List.
            /*
            rple.EntryCount = rple.TextBackup.Count;
            rple.EntryList = new List<PathEntries>();

            for (int g = 0; g < rple.EntryCount; g++)
            {
                PathEntries pe = new PathEntries();
                pe.TotalName = rple.TextBackup[g];
                rple.EntryList.Add(pe);
            }
            LoadRPLInTextBox(texbox, rple);
            */

            //Reconstructs the Chain List.

            chlste.ChainEntries = new List<CHNEntry>();
            chlste.ChainCollEntries = new List<CCLEntry>();

            string[] lines = texbox.Lines;

            for (int i = 0; i < lines.Length; i++)
            {





            }

        }

        public static ChainListEntry ReplaceCST(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            ChainListEntry cstnentry = new ChainListEntry();
            ChainListEntry cstoldentry = new ChainListEntry();

            tree.BeginUpdate();

            //Gotta Fix this up then test insert and replacing.
            try
            {
                using (BinaryReader br = new BinaryReader(File.OpenRead(filename)))
                {

                    ReplaceKnownEntry(tree, node, filename, cstnentry, cstoldentry);

                    cstnentry._FileName = cstnentry.TrueName;
                    cstnentry._DecompressedFileLength = cstnentry.UncompressedData.Length;
                    cstnentry._CompressedFileLength = cstnentry.CompressedData.Length;

                    cstnentry.TypeHash = "326F732E";

                    //Type specific work here.
                    using (MemoryStream CslStream = new MemoryStream(cstnentry.UncompressedData))
                    {
                        using (BinaryReader bnr = new BinaryReader(CslStream))
                        {

                            bnr.BaseStream.Position = 4;
                            cstnentry.Unknown04 = bnr.ReadInt32();
                            cstnentry.TotalEntrySize = bnr.ReadInt32();
                            cstnentry.CHNEntryCount = bnr.ReadInt32();
                            cstnentry.CCLEntryCount = bnr.ReadInt32();

                            cstnentry.ChainEntries = new List<CHNEntry>();
                            cstnentry.ChainCollEntries = new List<CCLEntry>();

                            for (int g = 0; g < cstnentry.CHNEntryCount; g++)
                            {
                                CHNEntry cHN = new CHNEntry();
                                cHN.FullPath = Encoding.ASCII.GetString(bnr.ReadBytes(64)).Trim('\0');
                                cHN.TypeHash = ByteUtilitarian.BytesToStringL2R(bnr.ReadBytes(4).ToList(), cHN.TypeHash);

                                try
                                {
                                    //Gets the Corrected path for the cfg.
                                    string ProperPath = "";
                                    ProperPath = Globals.ToolPath + "archive_filetypes.cfg";
                                    using (var sr = new StreamReader(ProperPath))
                                    {
                                        while (!sr.EndOfStream)
                                        {
                                            var keyword = Console.ReadLine() ?? cHN.TypeHash;
                                            var line = sr.ReadLine();
                                            if (String.IsNullOrEmpty(line)) continue;
                                            if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                                            {
                                                cHN.FileExt = line;
                                                cHN.FileExt = cHN.FileExt.Split(' ')[1];
                                                cHN.TotalName = cHN.FullPath + cHN.FileExt;
                                                break;
                                            }
                                        }
                                    }

                                }
                                catch (FileNotFoundException)
                                {
                                    MessageBox.Show("I cannot find archive_filetypes.cfg so I cannot finish parsing the arc.", "Oh Boy");
                                    string ProperPath = "";
                                    ProperPath = Globals.ToolPath + "Log.txt";
                                    using (StreamWriter sw = File.AppendText(ProperPath))
                                    {
                                        sw.WriteLine("Cannot find archive_filetypes.cfg and thus cannot continue parsing.");
                                    }
                                    return null;
                                }

                                cstnentry.ChainEntries.Add(cHN);
                                bnr.BaseStream.Position = bnr.BaseStream.Position + 16;
                            }

                            for (int h = 0; h < cstnentry.CCLEntryCount; h++)
                            {
                                CCLEntry cCL = new CCLEntry();
                                cCL.FullPath = Encoding.ASCII.GetString(bnr.ReadBytes(64)).Trim('\0');
                                cCL.TypeHash = ByteUtilitarian.BytesToStringL2R(bnr.ReadBytes(4).ToList(), cCL.TypeHash);

                                try
                                {
                                    string ProperPath = "";
                                    ProperPath = Globals.ToolPath + "archive_filetypes.cfg";
                                    using (var sr = new StreamReader(ProperPath))
                                    {
                                        while (!sr.EndOfStream)
                                        {
                                            var keyword = Console.ReadLine() ?? cCL.TypeHash;
                                            var line = sr.ReadLine();
                                            if (String.IsNullOrEmpty(line)) continue;
                                            if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                                            {
                                                cCL.FileExt = line;
                                                cCL.FileExt = cCL.FileExt.Split(' ')[1];
                                                cCL.TotalName = cCL.FullPath + cCL.FileExt;
                                                break;
                                            }
                                        }
                                    }

                                }
                                catch (FileNotFoundException)
                                {
                                    MessageBox.Show("I cannot find archive_filetypes.cfg so I cannot finish parsing the arc.", "Oh Boy");
                                    string ProperPath = "";
                                    ProperPath = Globals.ToolPath + "Log.txt";
                                    using (StreamWriter sw = File.AppendText(ProperPath))
                                    {
                                        sw.WriteLine("Cannot find archive_filetypes.cfg and thus cannot continue parsing.");
                                    }
                                    return null;
                                }

                                cstnentry.ChainCollEntries.Add(cCL);
                            }

                        }
                    }

                    cstnentry.TextBackup = new List<string>();


                    //Hmmm.

                    var tag = node.Tag;
                    if (tag is ChainListEntry)
                    {
                        cstoldentry = tag as ChainListEntry;
                    }
                    string path = "";
                    int index = cstoldentry.EntryName.LastIndexOf("\\");
                    if (index > 0)
                    {
                        path = cstoldentry.EntryName.Substring(0, index);
                    }

                    cstnentry.EntryName = path + "\\" + cstnentry.TrueName;

                    tag = cstnentry;

                    if (node.Tag is ChainListEntry)
                    {
                        node.Tag = cstnentry;
                        node.Name = Path.GetFileNameWithoutExtension(cstnentry.EntryName);
                        node.Text = Path.GetFileNameWithoutExtension(cstnentry.EntryName);

                    }

                    var aew = node as ArcEntryWrapper;

                    string type = node.GetType().ToString();
                    if (type == "ThreeWorkTool.Resources.Wrappers.ArcEntryWrapper")
                    {
                        aew.entryfile = cstnentry;
                    }

                    node = aew;
                    node.entryfile = cstnentry;
                    tree.EndUpdate();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Read error. Is the file readable?");
                string ProperPath = "";
                ProperPath = Globals.ToolPath + "Log.txt";
                using (StreamWriter sw = File.AppendText(ProperPath))
                {
                    sw.WriteLine("Read error. Cannot access the file:" + filename + "\n" + ex);
                }
            }



            return node.entryfile as ChainListEntry;
        }

        public static ChainListEntry InsertChainListEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            ChainListEntry clstentry = new ChainListEntry();

            InsertEntry(tree, node, filename, clstentry);

            clstentry.DecompressedFileLength = clstentry.UncompressedData.Length;
            clstentry._DecompressedFileLength = clstentry.UncompressedData.Length;
            clstentry.CompressedFileLength = clstentry.CompressedData.Length;
            clstentry._CompressedFileLength = clstentry.CompressedData.Length;
            clstentry._FileName = clstentry.TrueName;
            clstentry.EntryName = clstentry.FileName;

            clstentry.TypeHash = "326F732E";

            //Type specific work here.
            using (MemoryStream CslStream = new MemoryStream(clstentry.UncompressedData))
            {
                using (BinaryReader bnr = new BinaryReader(CslStream))
                {

                    bnr.BaseStream.Position = 4;
                    clstentry.Unknown04 = bnr.ReadInt32();
                    clstentry.TotalEntrySize = bnr.ReadInt32();
                    clstentry.CHNEntryCount = bnr.ReadInt32();
                    clstentry.CCLEntryCount = bnr.ReadInt32();

                    clstentry.ChainEntries = new List<CHNEntry>();
                    clstentry.ChainCollEntries = new List<CCLEntry>();

                    for (int g = 0; g < clstentry.CHNEntryCount; g++)
                    {
                        CHNEntry cHN = new CHNEntry();
                        cHN.FullPath = Encoding.ASCII.GetString(bnr.ReadBytes(64)).Trim('\0');
                        cHN.TypeHash = ByteUtilitarian.BytesToStringL2R(bnr.ReadBytes(4).ToList(), cHN.TypeHash);

                        try
                        {
                            string ProperPath = "";
                            ProperPath = Globals.ToolPath + "archive_filetypes.cfg";
                            using (var sr = new StreamReader(ProperPath))
                            {
                                while (!sr.EndOfStream)
                                {
                                    var keyword = Console.ReadLine() ?? cHN.TypeHash;
                                    var line = sr.ReadLine();
                                    if (String.IsNullOrEmpty(line)) continue;
                                    if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                                    {
                                        cHN.FileExt = line;
                                        cHN.FileExt = cHN.FileExt.Split(' ')[1];
                                        cHN.TotalName = cHN.FullPath + cHN.FileExt;
                                        break;
                                    }
                                }
                            }

                        }
                        catch (FileNotFoundException)
                        {
                            MessageBox.Show("I cannot find archive_filetypes.cfg so I cannot finish parsing the arc.", "Oh Boy");
                            string ProperPath = "";
                            ProperPath = Globals.ToolPath + "Log.txt";
                            using (StreamWriter sw = File.AppendText(ProperPath))
                            {
                                sw.WriteLine("Cannot find archive_filetypes.cfg and thus cannot continue parsing.");
                            }
                            return null;
                        }

                        clstentry.ChainEntries.Add(cHN);
                        bnr.BaseStream.Position = bnr.BaseStream.Position + 16;
                    }

                    for (int h = 0; h < clstentry.CCLEntryCount; h++)
                    {
                        CCLEntry cCL = new CCLEntry();
                        cCL.FullPath = Encoding.ASCII.GetString(bnr.ReadBytes(64)).Trim('\0');
                        cCL.TypeHash = ByteUtilitarian.BytesToStringL2R(bnr.ReadBytes(4).ToList(), cCL.TypeHash);

                        try
                        {
                            string ProperPath = "";
                            ProperPath = Globals.ToolPath + "archive_filetypes.cfg";
                            using (var sr = new StreamReader(ProperPath))
                            {
                                while (!sr.EndOfStream)
                                {
                                    var keyword = Console.ReadLine() ?? cCL.TypeHash;
                                    var line = sr.ReadLine();
                                    if (String.IsNullOrEmpty(line)) continue;
                                    if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                                    {
                                        cCL.FileExt = line;
                                        cCL.FileExt = cCL.FileExt.Split(' ')[1];
                                        cCL.TotalName = cCL.FullPath + cCL.FileExt;
                                        break;
                                    }
                                }
                            }

                        }
                        catch (FileNotFoundException)
                        {
                            MessageBox.Show("I cannot find archive_filetypes.cfg so I cannot finish parsing the arc.", "Oh Boy");
                            string ProperPath = "";
                            ProperPath = Globals.ToolPath + "Log.txt";
                            using (StreamWriter sw = File.AppendText(ProperPath))
                            {
                                sw.WriteLine("Cannot find archive_filetypes.cfg and thus cannot continue parsing.");
                            }
                            return null;
                        }

                        clstentry.ChainCollEntries.Add(cCL);
                    }

                }
            }

            clstentry.TextBackup = new List<string>();

            return clstentry;
        }

        #region CST Properties
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

        [Category("Filename"), ReadOnlyAttribute(true)]
        public string FileType
        {

            get
            {
                return FileExt;
            }
            set
            {
                FileExt = value;
            }
        }

        [Category("ChainListData"), ReadOnlyAttribute(true)]
        public int ChainCount
        {

            get
            {
                return CHNEntryCount;
            }
            set
            {
                CHNEntryCount = value;
            }

        }

        [Category("ChainListData"), ReadOnlyAttribute(true)]
        public int ChainCollisionCount
        {

            get
            {
                return CCLEntryCount;
            }
            set
            {
                CCLEntryCount = value;
            }

        }

        #endregion
    }
}
