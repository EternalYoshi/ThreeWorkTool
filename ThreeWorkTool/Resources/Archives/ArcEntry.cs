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
                

                //Looks through the archive_filetypes.txt file to find the extension associated with the typehash.
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
                //temps = Convert.ToString(bytes[i]);
                if (temps == "0")
                {
                    temps = "00";
                }
                tru += temps;
            }
            return tru;
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
