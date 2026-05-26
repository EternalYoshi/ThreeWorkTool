using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeWorkTool.Resources.Wrappers.ExtraNodes
{
    public class ChainSegment
    {
        public int Index;
        public int Offset;
        public byte NodeCount;
        public byte Flags { get; set; }
        public uint RecordOffset;
        public uint RecordAbsOffset;
        public int RecordBytes;
        //public List<uint> ParametersU;
        //public List<float> ParamtersF;

        public float UnkParam1 { get; set; }
        public float UnkParam2 { get; set; }
        public float UnkParam3 { get; set; }
        public float UnkParam4 { get; set; }
        public float UnkParam5 { get; set; }
        public float UnkParam6 { get; set; }

        public List<ChainSegmentRecord> Records;
    }
}
