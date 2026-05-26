using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ThreeWorkTool.Resources.Wrappers.ExtraNodes
{
    public class ChainColNode
    {
        //Thanks to Rootz for the info.
        public int index;
        public int SomeFlags { get; set; }
        public byte BoneID1 { get; set; }
        public byte BoneID2 { get; set; }
        public float Radius { get; set; }
        public int PrimitiveType { get; set; } //0 For sphere, and 1 for Capsule. 2 for Box. 3 for Oriented Box.
        public float CenterPosition_X { get; set; }
        public float CenterPosition_Y { get; set; }
        public float CenterPosition_Z { get; set; }
        public float Rotation_X { get; set; } //Apparently this might be using Euler instead of Quaternion.
        public float Rotation_Y { get; set; }
        public float Rotation_Z { get; set; }
        public int Unknown30;
        public int Unknown34;
        public int Unknown38;
        public int Unknown3C;
    }
}
