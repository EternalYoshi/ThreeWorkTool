using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeWorkTool.Resources.Wrappers.ExtraNodes
{
    public class ChainSegmentRecord
    {
        public int Index;
        public int Offset;
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public uint Flags { get; set; }
        //public uint MoreFlags { get; set; }
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public int Joint;
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public byte LinkCount;
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public byte unk07;
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public float SomeFloat08 { get; set; }
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public float SomeFloat0C { get; set; }
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public float SomeFloat10 { get; set; }
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public float SomeFloat14 { get; set; }
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public float SomeFloat18 { get; set; }
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public float SomeFloat1C { get; set; }
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public float SomeFloat20 { get; set; }
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public float SomeFloat24 { get; set; }
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public float SomeFloat28 { get; set; }
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public float SomeFloat2C { get; set; }
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public float SomeFloat30 { get; set; }
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public byte SomeByte34 { get; set; }
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public byte SomeByte35 { get; set; }
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public byte SomeByte36 { get; set; }
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public byte SomeByte37 { get; set; }
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public float SomeFloat38 { get; set; }
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public float SomeFloat3C { get; set; }
        public long AbsoluteLocation;
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public byte JointLinkIndex1 { get; set; }
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public byte JointLinkIndex2 { get; set; }
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public byte JointLinkIndex3 { get; set; }
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public byte JointLinkIndex4 { get; set; }
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public byte JointLinkIndex5 { get; set; }
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public byte JointLinkIndex6 { get; set; }
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public byte JointLinkIndex7 { get; set; }
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public byte JointLinkIndex8 { get; set; }
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public float SomeFloat48 { get; set; }
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public float SomeFloat4C { get; set; }
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public float SomeFloat50 { get; set; }
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public float SomeFloat54 { get; set; }
        [Category("Chain Segment Record"), ReadOnlyAttribute(false)]
        public float SomeFloat58 { get; set; }
    }
}
