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
    class ResourcePathListEntry
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

                int ECTemp = Convert.ToInt32(STemp);
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

            for (int t = 0; t < rple.EntryList.Count; t++)
            {
                texbox.Text = texbox.Text + rple.EntryList[t].TotalName + System.Environment.NewLine;
            }


            return texbox;
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



                }
            }
            catch (Exception ex)
            {

            }



            return rplentry;
        }


    }
}
