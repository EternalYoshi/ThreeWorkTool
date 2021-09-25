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
using ThreeWorkTool.Resources.Wrappers;

namespace ThreeWorkTool.Resources.Archives
{
    public abstract class DefaultWrapper
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



    }
}
