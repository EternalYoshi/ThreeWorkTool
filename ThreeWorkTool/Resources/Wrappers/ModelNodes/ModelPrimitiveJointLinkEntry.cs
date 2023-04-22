using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ThreeWorkTool.Resources.Archives;
using ThreeWorkTool.Resources.Wrappers;

namespace ThreeWorkTool.Resources.Wrappers.ModelNodes
{
    public class ModelPrimitiveJointLinkEntry : DefaultWrapper
    {
        private static readonly int ENTRY_SIZE = 0x90;

        public int JointIndex { get; set; }
        public int Unknwon04 { get; set; }
        public int Unknown08 { get; set; }
        public int Unknwon0C { get; set; }
        public Vector4 BoundingSphere;
        public Vector4 PLJMin;
        public Vector4 PLJMax;

        public struct MTMatrix
        {
            public List<Vector4> Rows;
        }

        public MTMatrix LocalMtx;
        public Vector4 UnknownVec80;

    }
}
