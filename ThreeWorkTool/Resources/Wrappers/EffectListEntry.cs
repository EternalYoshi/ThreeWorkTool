using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Utility;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class EffectListEntry
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
        public byte[] WTemp;
        public byte[] CompressedData;
        public byte[] UncompressedData;
        public string[] EntryDirs;
        public string TrueName;
        public string FileExt;
        public static StringBuilder SBname;
        public static string TypeHash = "6D5AE854";
        public int DataSize;
        public string WeirdConstant;
        public string OtherWeirdConstant;
        public int SomeEntryCount;
        public int OtherEntryCount;

        public static EffectListEntry FillEFLEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
        {
            EffectListEntry effectList = new EffectListEntry();

            //This block gets the name of the entry.
            effectList.OffsetTemp = c;
            effectList.EntryID = ID;
            List<byte> BTemp = new List<byte>();
            br.BaseStream.Position = effectList.OffsetTemp;
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
            effectList.CSize = BitConverter.ToInt32(BTemp.ToArray(), 0);

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
            effectList.DSize = (int)DIFF;

            //Data Offset.
            BTemp = new List<byte>();
            c = c + 4;
            br.BaseStream.Position = c;
            BTemp.AddRange(br.ReadBytes(4));
            effectList.AOffset = BitConverter.ToInt32(BTemp.ToArray(), 0);

            //Compressed Data.
            BTemp = new List<byte>();
            c = effectList.AOffset;
            br.BaseStream.Position = c;
            BTemp.AddRange(br.ReadBytes(effectList.CSize));
            effectList.CompressedData = BTemp.ToArray();


            //Namestuff.
            effectList.EntryName = Tempname;

            //Ensures existing subdirectories are cleared so the directories for files are displayed correctly.
            if (subnames != null)
            {
                if (subnames.Count > 0)
                {
                    subnames.Clear();
                }
            }

            //Gets the filename without subdirectories.
            if (effectList.EntryName.Contains("\\"))
            {
                string[] splstr = effectList.EntryName.Split('\\');

                //foreach (string v in splstr)
                for (int v = 0; v < (splstr.Length - 1); v++)
                {
                    if (!subnames.Contains(splstr[v]))
                    {
                        subnames.Add(splstr[v]);
                    }
                }


                effectList.TrueName = effectList.EntryName.Substring(effectList.EntryName.IndexOf("\\") + 1);
                Array.Clear(splstr, 0, splstr.Length);

                while (effectList.TrueName.Contains("\\"))
                {
                    effectList.TrueName = effectList.TrueName.Substring(effectList.TrueName.IndexOf("\\") + 1);
                }
            }
            else
            {
                effectList.TrueName = effectList.EntryName;
            }

            effectList._FileName = effectList.TrueName;

            effectList.EntryDirs = subnames.ToArray();
            effectList.FileExt = ".efl";
            effectList.EntryName = effectList.EntryName + effectList.FileExt;
            effectList._FileName = effectList.TrueName;
            effectList._FileType = effectList.FileExt;
            effectList._FileLength = effectList.DSize;

            //Decompression Time.
            effectList.UncompressedData = ZlibStream.UncompressBuffer(effectList.CompressedData);

            //Specific file type work goes here!

            //Header is 30 bytes. What info is in there?
            byte[] MTemp = new byte[4];
            string STemp = " ";
            Array.Copy(effectList.UncompressedData, 0, MTemp, 0, 4);
            effectList.Magic = ByteUtilitarian.BytesToString(MTemp, effectList.Magic);

            //These values at 0x04 seem identical based on what I've seen so far.
            string SCTemp = " ";
            Array.Copy(effectList.UncompressedData, 4, MTemp, 0, 4);
            effectList.Magic = ByteUtilitarian.BytesToString(MTemp, effectList.WeirdConstant);

            //Gets the Data Size.
            Array.Copy(effectList.UncompressedData, 8, MTemp, 0, 4);
            Array.Reverse(MTemp);
            STemp = ByteUtilitarian.BytesToString(MTemp, STemp);
            int ECTemp = Convert.ToInt32(STemp, 16);
            effectList.DataSize = ECTemp;

            //These values at 0x0C also seem identical based on what I've seen so far.
            Array.Copy(effectList.UncompressedData, 4, MTemp, 0, 4);
            effectList.Magic = ByteUtilitarian.BytesToString(MTemp, effectList.OtherWeirdConstant);

            byte[] TwoTemp = new byte[2];
            Array.Copy(effectList.UncompressedData, 16, TwoTemp, 0, 2);
            Array.Reverse(TwoTemp);
            effectList.SomeEntryCount = BitConverter.ToInt32(TwoTemp,0);

            Array.Copy(effectList.UncompressedData, 18, TwoTemp, 0, 2);
            Array.Reverse(TwoTemp);
            effectList.OtherEntryCount = BitConverter.ToInt32(TwoTemp, 0);

            return effectList;
        }

        #region EffectListEntryProperties

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

    }
}
