using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Wrappers;
using ThreeWorkTool.Resources.Utility;


namespace ThreeWorkTool.Resources.Archives
{
    public class ArcFile
    {
        public List<ArcEntry> arctable;
        public List<ArcEntry> arctableBACKUP;
        public List<object> arcfiles;
        public List<String> FileList;
        public List<String> TypeHashes;
        public int FLOffset;
        public static StringBuilder SBname;
        public byte[] HeaderMagic;
        public byte[] HeaderB;
        public int FileCount;
        public Type FileType;
        public byte UnknownFlag;
        public string Tempname;
        public string th;
        public string tn;
        public int k;
        public int IDCounter;
        public int Totalsize;
        public List<string> subdref;
        public static int EntryStart = 0x08;
        public static int EntrySize = 0x50;

        public static string[] RecogExtensions = { ".TEX", ".MOD", ".MRL", ".CHN", ".CCL", ".CST", ".LMT" };

        public static ArcFile LoadArc(TreeView tree, string filename, List<string> foldernames, bool Verifier = false, Type filetype = null, int arcsize = -1)
        {
            
            ArcFile arcfile = new ArcFile();
            byte[] Bytes = File.ReadAllBytes(filename);

            using (BinaryReader br = new BinaryReader(File.Open(filename, FileMode.Open)))
            {

                arcsize = Bytes.Length;
                int Totalsize = arcsize;
                arcfile.FileLength = arcsize;
                arcfile.Tempname = filename;

                br.BaseStream.Position = 0;
                byte[] HeaderMagic = br.ReadBytes(4);
                //Checks file signature/Endianess.
                if (HeaderMagic[0] == 0x00 && HeaderMagic[1] == 0x43 && HeaderMagic[2] == 0x52 && HeaderMagic[3] == 0x41)
                {
                    MessageBox.Show("This .arc file is not in the kind of endian I can deal with right now, so I'm closing it.", "Ummm");
                    br.Close();
                    return null;
                }

                if (HeaderMagic[0] != 0x41 && HeaderMagic[1] != 0x52 && HeaderMagic[2] != 0x43 && HeaderMagic[3] != 0x00)
                {
                    MessageBox.Show("This .arc file is either not the correct kind or is not properly extracted, so I'm closing it.", "Oh dear");
                    br.Close();
                    return null;
                }

                arcfile.HeaderMagic = HeaderMagic;

                arcfile.arctable = new List<ArcEntry>();
                arcfile.arcfiles = new List<object>();
                arcfile.FileList = new List<string>();
                arcfile.TypeHashes = new List<string>();

                br.BaseStream.Position = 4;
                arcfile.UnknownFlag = br.ReadByte();

                br.BaseStream.Position = 6;
                arcfile.FileCount = BitConverter.ToInt16((br.ReadBytes(2)), 0);
                arcfile.Version = arcfile.UnknownFlag;

                List<String> filenames = new List<String>();

                List<string> subdref = new List<string>();
                foldernames = subdref;


                //byte[] BytesTemp;
                //BytesTemp = new byte[] { };
                List<byte> BytesTemp = new List<byte>();
                byte[] HTTemp = new byte[] { };
                int j = 8;
                int l = 64;
                int m = 80;
                int n = 4;

                //Iterates through the header/first part of the arc to get all the filenames and occupy the filename list.
                for (int i = 0; i < arcfile.FileCount; i++)
                {
                    BytesTemp.Clear();
                    BytesTemp.TrimExcess();
                    j = 8 + (m * i);
                    //Copies the specified range to isolate the bytes containing a filename.
                    br.BaseStream.Position = j;
                    BytesTemp.AddRange(br.ReadBytes(l));
                    BytesTemp.RemoveAll(ByteUtilitarian.IsZeroByte);
                    filenames.Add(ByteUtilitarian.BytesToStringL(BytesTemp));
                    //For The Typehashes.
                    n = 72 + (m * i);

                    br.BaseStream.Position = n;
                    HTTemp = br.ReadBytes(4);
                    Array.Reverse(HTTemp);
                    arcfile.TypeHashes.Add(ByteUtilitarian.HashBytesToString(HTTemp));

                }

                //Fills in each file as an ArcEntry or TextureEntry as needed. 
                j = 8;
                int IDCounter = 0;
                for (int i = 0; i < arcfile.FileCount; i++)
                {
                    j = 8 + (80 * i);
                    switch (arcfile.TypeHashes[i])
                    {
                        //Texture Files.
                        case "241F5DEB":
                            TextureEntry newtexen = TextureEntry.FillTexEntry(filename, foldernames, tree, br, j, IDCounter);
                            arcfile.arcfiles.Add(newtexen);
                            arcfile.FileList.Add(newtexen.EntryName);
                            foldernames.Clear();
                            IDCounter++;
                            break;

                        //Resource Path Lists.
                        case "357EF6D4":
                            ResourcePathListEntry newplen = ResourcePathListEntry.FillRPLEntry(filename, foldernames, tree, br, j, IDCounter);
                            arcfile.arcfiles.Add(newplen);
                            arcfile.FileList.Add(newplen.EntryName);
                            foldernames.Clear();
                            IDCounter++;
                            break;

                        //Materials.
                        case "2749C8A8":
                            MaterialEntry Maten = MaterialEntry.FillMatEntry(filename, foldernames, tree, br, j, IDCounter);
                            arcfile.arcfiles.Add(Maten);
                            arcfile.FileList.Add(Maten.EntryName);
                            foldernames.Clear();
                            IDCounter++;
                            break;

                        //LMT Files.
                        case "76820D81":
                            LMTEntry LMTen = LMTEntry.FillLMTEntry(filename, foldernames, tree, br, j, IDCounter);
                            arcfile.arcfiles.Add(LMTen);
                            arcfile.FileList.Add(LMTen.EntryName);
                            foldernames.Clear();
                            IDCounter++;
                            break;

                        //MSD Files. Commented out until a future release.
                        /*
                        case "5B55F5B1":
                            MSDEntry newmsden = MSDEntry.FillMSDEntry(filename, foldernames, tree, Bytes, j, IDCounter);
                            arcfile.arcfiles.Add(newmsden);
                            arcfile.FileList.Add(newmsden.EntryName);
                            foldernames.Clear();
                            IDCounter++;
                            break;
                        */

                        default:
                            //Everything not listed above.
                            ArcEntry newentry = ArcEntry.FillEntry(filename, foldernames, tree, br, j, IDCounter);
                            arcfile.arcfiles.Add(newentry);
                            arcfile.FileList.Add(newentry.EntryName);
                            foldernames.Clear();
                            IDCounter++;
                            break;
                    }
                }



                br.Close();
            }

            return arcfile;
        }

        public static string BytesToString(byte[] bytes)
        {
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
            Tempname = ascii.GetString(bytes);
            //Tempname = Tempname.Replace(@"\\", @"\");
            return Tempname;
        }

        public static string HashBytesToString(byte[] bytes)
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

        #region Arc Class
        //Affects what you see in the Property Grid, grdMain.

        private UInt16 _FileAmount;
        [Category("MT ARC Entry"), ReadOnlyAttribute(true)]
        public UInt16 FileAmount
        {

            get
            {
                return _FileAmount;
            }
            set
            {
                _FileAmount = value;
            }
        }

        private UInt16 _Version;
        [Category("MT ARC Entry"), ReadOnlyAttribute(true)]
        public UInt16 Version
        {

            get
            {
                return _Version;
            }
            set
            {
                _Version = value;
            }
        }

        private long _FileLength;
        [Category("MT ARC Entry"), ReadOnlyAttribute(true)]
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

        #endregion

    }
}
