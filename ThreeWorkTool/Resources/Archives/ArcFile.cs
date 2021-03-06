using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThreeWorkTool.Resources.Archives
{
    public class ArcFile
    {
        public List<ArcEntry> arctable;
        public List<ArcEntry> arctableBACKUP;
        public List<String> FileList;
        public int FLOffset;
        public static StringBuilder SBname;
        public byte[] HeaderMagic;
        public byte[] HeaderB;
        public byte FileCount;
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

            using (FileStream fs = File.OpenRead(filename))
            {
                byte[] Bytes = File.ReadAllBytes(filename);

                arcsize = Bytes.Length;
                int Totalsize = arcsize;
                arcfile.FileLength = arcsize;

                fs.Read(Bytes, 0, Convert.ToInt32(fs.Length));

                arcfile.Tempname = filename;

                //Checks file signature/Endianess.
                if (Bytes[0] != 0x41 && Bytes[1] != 0x52 && Bytes[1] != 0x43)
                {
                    MessageBox.Show("This .arc file is not in the kind of endian I can deal with right now, so I'm closing it.", "Ummm");
                    fs.Close();
                }

                arcfile.arctable = new List<ArcEntry>();
                arcfile.FileList = new List<string>();

                arcfile.FileCount = Bytes[6];
                arcfile.FileAmount = Bytes[6];
                arcfile.UnknownFlag = Bytes[4];
                arcfile.Version = arcfile.UnknownFlag;
                arcfile.HeaderMagic = Bytes.Take(4).ToArray();

                List<String> filenames = new List<String>();

                List<string> subdref = new List<string>();
                foldernames = subdref;

                byte[] BytesTemp;
                BytesTemp = new byte[] { };
                int j = 8;
                //int k = 0;
                int l = 64;
                int m = 80;
                //Iterates through the header/first part of the arc to get all the filenames and occupy the filename list.
                for (int i = 0; i < arcfile.FileCount; i++)
                {
                    Array.Clear(BytesTemp, 0, BytesTemp.Length);
                    j = 8 + (m * i);
                    //Copies the specified range to isolate the bytes containing a filename.
                    BytesTemp = Bytes.Skip(j).Take(l).Where(x => x != 0x00).ToArray();
                    //Array.Copy(Bytes, j, BytesTemp,k,l - j);
                    filenames.Add(BytesToString(BytesTemp));
                }



                //Fills in each file as an ArcEntry.
                j = 8;
                int IDCounter = 0;
                for (int i = 0; i < arcfile.FileCount; i++)
                {
                    j = 8 + (80 * i);
                    ArcEntry newentry = ArcEntry.FillEntry(filename, foldernames, tree, Bytes, j, IDCounter);                    
                    arcfile.arctable.Add(newentry);
                    arcfile.FileList.Add(newentry.EntryName);
                    foldernames.Clear();
                    IDCounter++;
                }



                fs.Close();
                //byte[] HeaderMagic = new List<byte>().GetRange(0,3).ToArray();

                for (int i = 0; i > arcfile.FileCount; i++)
                {


                }

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
