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

            cslentry._FileName = cslentry.TrueName + cslentry.FileExt;
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
                            using (var sr = new StreamReader("archive_filetypes.cfg"))
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
                            using (StreamWriter sw = File.AppendText("Log.txt"))
                            {
                                sw.WriteLine("Cannot find archive_filetypes.cfg and thus cannot continue parsing.");
                            }
                            return null;
                        }

                        cslentry.ChainEntries.Add(cHN);
                        bnr.BaseStream.Position = bnr.BaseStream.Position = 16;
                    }

                    for (int h = 0; h < cslentry.CCLEntryCount; h++)
                    {
                        CCLEntry cCL = new CCLEntry();
                        cCL.FullPath = Encoding.ASCII.GetString(bnr.ReadBytes(64)).Trim('\0');
                        cCL.TypeHash = ByteUtilitarian.BytesToStringL2R(bnr.ReadBytes(4).ToList(), cCL.TypeHash);

                        try
                        {
                            using (var sr = new StreamReader("archive_filetypes.cfg"))
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
                            using (StreamWriter sw = File.AppendText("Log.txt"))
                            {
                                sw.WriteLine("Cannot find archive_filetypes.cfg and thus cannot continue parsing.");
                            }
                            return null;
                        }

                        cslentry.ChainCollEntries.Add(cCL);
                    }



                }
            }







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
                    texbox.Text = texbox.Text + chlste.ChainEntries[t].TotalName + System.Environment.NewLine;
                    chlste.TextBackup.Add(chlste.ChainEntries[t].TotalName + System.Environment.NewLine);
                }
            }
            else
            {

                for (int t = 0; t < chlste.ChainEntries.Count; t++)
                {
                    texbox.Text = texbox.Text + chlste.ChainEntries[t].TotalName + System.Environment.NewLine;
                }
            }

            //Inserts ChainCollEntries.
            if (isEmpty)
            {
                for (int t = 0; t < chlste.ChainCollEntries.Count; t++)
                {
                    texbox.Text = texbox.Text + chlste.ChainCollEntries[t].TotalName + System.Environment.NewLine;
                    chlste.TextBackup.Add(chlste.ChainCollEntries[t].TotalName + System.Environment.NewLine);
                }
            }
            else
            {

                for (int t = 0; t < chlste.ChainCollEntries.Count; t++)
                {
                    texbox.Text = texbox.Text + chlste.ChainCollEntries[t].TotalName + System.Environment.NewLine;
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

            chlste.ChainEntries = new List<CHNEntry>();
            chlste.ChainCollEntries = new List<CCLEntry>();

            for (int i = 0; i < SPLT.Length; i++)
            {
                index = SPLT[i].LastIndexOf(".");
                ExtTemp = SPLT[i].Substring(index);

                if (ExtTemp == ".chn" || ExtTemp == "chn")
                {
                    CHNEntry cHN = new CHNEntry();
                    cHN.TotalName = ExtTemp;
                    cHN.FullPath = SPLT[i].Substring(0, index);
                    cHN.TypeHash = "3E363245";
                    cHN.FileExt = SPLT[i].Substring(index);
                    chlste.ChainEntries.Add(cHN);

                }
                else if (ExtTemp == ".ccl" || ExtTemp == "ccl")
                {
                    CCLEntry cCL = new CCLEntry();
                    cCL.TotalName = ExtTemp;
                    cCL.FullPath = SPLT[i].Substring(0, index);
                    cCL.TypeHash = "0026E7FF";
                    cCL.FileExt = SPLT[i].Substring(index);
                    chlste.ChainCollEntries.Add(cCL);
                }

                chlste.CHNEntryCount = chlste.ChainEntries.Count;
                chlste.CCLEntryCount = chlste.ChainCollEntries.Count;

                //Rebuilds the raw file itself.
                List<byte> NEWCST = new List<byte>();
                byte[] HeaderCST = { 0x43, 0x53, 0x54, 0x00, 0x00, 0x01, 0xFE, 0xFF, 0x00, 0x00, 0x00, 0x00 };
                NEWCST.AddRange(HeaderCST);

                //Converts the CHN and CCL counts and puts them in the array.
                byte[] BufferA = BitConverter.GetBytes(chlste.ChainEntries.Count);
                NEWCST.AddRange(BufferA);

                //Converts the CHN and CCL counts and puts them in the array.
                byte[] BufferB = BitConverter.GetBytes(chlste.ChainCollEntries.Count);
                NEWCST.AddRange(BufferA);

                //Inserts the CHN data.
                int NewEntryCount = chlste.CHNEntryCount;
                if (string.IsNullOrWhiteSpace(chlste.ChainEntries[(chlste.CHNEntryCount - 1)].TotalName))
                {
                    NewEntryCount--;
                }
                int ProjectedSize = NewEntryCount * 84;
                int EstimatedSizeCHN = ((int)Math.Round(ProjectedSize / 16.0, MidpointRounding.AwayFromZero) * 16);

                for (int k = 0; k < NewEntryCount; k++)
                {

                    int NumberChars = chlste.ChainEntries[k].FullPath.Length;
                    byte[] namebuffer = Encoding.ASCII.GetBytes(chlste.ChainEntries[k].FullPath);
                    int nblength = namebuffer.Length;

                }




            }

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
            string ExtTemp = "";

            for (int i = 0; i < lines.Length; i++)
            {





            }

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

        private string _FileType;
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
