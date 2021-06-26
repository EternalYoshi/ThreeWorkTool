using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class ModelEntry
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
        public static string TypeHash = "58A15856";

    }
}
