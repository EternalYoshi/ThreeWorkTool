using Ionic.Zlib;
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
using ThreeWorkTool.Resources.Wrappers;
using System.Media;
using NAudio.Wave;


namespace ThreeWorkTool.Resources.Wrappers
{
    //For .xsew files as well because they have the same hash for some reason......
    public class RIFFEntry : DefaultWrapper
    {
        public int Version;
        public int FileLength;
        public double SoundLength;
        //MemoryStream Mestream;

        public static RIFFEntry FillRIFFEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
        {

            RIFFEntry Rifentry = new RIFFEntry();
            List<byte> BTemp = new List<byte>();

            FillEntry(filename, subnames, tree, br, c, ID, Rifentry, filetype);

            Rifentry._FileName = Rifentry.TrueName;
            Rifentry._DecompressedFileLength = Rifentry.UncompressedData.Length;
            Rifentry._CompressedFileLength = Rifentry.CompressedData.Length;

            return Rifentry;

        }

        public static RIFFEntry ReplaceRIFFEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            RIFFEntry rifentry = new RIFFEntry();
            RIFFEntry oldentry = new RIFFEntry();

            tree.BeginUpdate();

            ReplaceEntry(tree, node, filename, rifentry, oldentry);

            rifentry.DecompressedFileLength = rifentry.UncompressedData.Length;
            rifentry._DecompressedFileLength = rifentry.UncompressedData.Length;
            rifentry.CompressedFileLength = rifentry.CompressedData.Length;
            rifentry._CompressedFileLength = rifentry.CompressedData.Length;
            rifentry._FileName = rifentry.TrueName;
            rifentry.FileName = rifentry.TrueName;

            return node.entryfile as RIFFEntry;
        }

        public static RIFFEntry InsertRIFFEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            RIFFEntry rifentry = new RIFFEntry();

            //We build the rifentry starting from the uncompressed data.
            rifentry.UncompressedData = System.IO.File.ReadAllBytes(filename);
            rifentry.DecompressedFileLength = rifentry.UncompressedData.Length;
            rifentry._DecompressedFileLength = rifentry.UncompressedData.Length;
            rifentry.DSize = rifentry.UncompressedData.Length;

            //Then Compress.
            rifentry.CompressedData = Zlibber.Compressor(rifentry.UncompressedData);
            rifentry.CompressedFileLength = rifentry.CompressedData.Length;
            rifentry._CompressedFileLength = rifentry.CompressedData.Length;
            rifentry.CSize = rifentry.CompressedData.Length;

            //Gets the filename of the file to inject without the directory.
            string trname = filename;
            while (trname.Contains("\\"))
            {
                trname = trname.Substring(trname.IndexOf("\\") + 1);
            }

            rifentry.TrueName = trname;
            rifentry._FileName = rifentry.TrueName;
            rifentry.TrueName = Path.GetFileNameWithoutExtension(trname);
            rifentry.FileExt = trname.Substring(trname.LastIndexOf("."));

            return rifentry;
        }

        public static void RefreshAudioPlayer(SoundPlayer SPlayer, RIFFEntry riff, WaveFileReader WFReader, MemoryStream MSound)
        {

            //This is just to get the sound length in seconds. Has to be visible before play is pressed.
            MSound = new MemoryStream(riff.UncompressedData);

            WFReader = new WaveFileReader(MSound);
            SPlayer = new SoundPlayer(MSound);
            riff.SoundLength = WFReader.TotalTime.TotalSeconds;


        }

        #region RIFF/XSEW Properties

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

        [Category("MT Sound Entry"), ReadOnlyAttribute(true)]
        public double AudioLength
        {
            get
            {
                return SoundLength;
            }
            set
            {
                SoundLength = value;
            }
        }

        #endregion


    }
}
