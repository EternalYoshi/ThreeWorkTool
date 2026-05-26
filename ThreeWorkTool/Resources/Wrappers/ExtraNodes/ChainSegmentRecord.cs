using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeWorkTool.Resources.Wrappers.ExtraNodes
{
    public class ChainSegmentRecord
    {
        public int Index;
        public int Offset;
        public uint Flags { get; set; }
        //public uint MoreFlags { get; set; }
        public int Joint;
        public byte LinkCount;
        public byte unk07;
        public float SomeFloat08 { get; set; }
        public float SomeFloat0C { get; set; }
        public float SomeFloat10 { get; set; }
        public float SomeFloat14 { get; set; }
        public float SomeFloat18 { get; set; }
        public float SomeFloat1C { get; set; }
        public float SomeFloat20 { get; set; }
        public float SomeFloat24 { get; set; }
        public float SomeFloat28 { get; set; }
        public float SomeFloat2C { get; set; }
        public float SomeFloat30 { get; set; }
        public float SomeFloat34 { get; set; }
        public float SomeFloat38 { get; set; }
        public float SomeFloat3C { get; set; }
        public long AbsoluteLocation;
        public byte JointLinkIndex1 { get; set; }
        public byte JointLinkIndex2 { get; set; }
        public byte JointLinkIndex3 { get; set; }
        public byte JointLinkIndex4 { get; set; }
        public byte JointLinkIndex5 { get; set; }
        public byte JointLinkIndex6 { get; set; }
        public byte JointLinkIndex7 { get; set; }
        public byte JointLinkIndex8 { get; set; }
        public float SomeFloat48 { get; set; }
        public float SomeFloat4C { get; set; }
        public float SomeFloat50 { get; set; }
        public float SomeFloat54 { get; set; }
        public float SomeFloat58 { get; set; }
    }
}
