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
    public class ModelEnvelopeEntry : DefaultWrapper
    {
        private static readonly int ENTRY_SIZE = 0x90;

        //public int JointIndex { get; set; }
        //public int Unknwon04 { get; set; }
        //public int Unknown08 { get; set; }
        //public int Unknwon0C { get; set; }
        //public Vector4 BoundingSphere;
        //public Vector4 PLJMin;
        //public Vector4 PLJMax;

        //public struct MTMatrix
        //{
        //    public List<Vector4> Rows;
        //}

        //public MTMatrix LocalMtx;
        //public Vector4 UnknownVec80;

        public int JointIndex, Unk04, Unk08, Unk0C;

        public Vector3 BoundingSphere;
        public float BoundingSphereRadius;
        public Vector4 BoundingBoxMin;
        public Vector4 BoundingBoxMax;

        public struct Pivot
        {
            public Vector4 Row1;
            public Vector4 Row2;
            public Vector4 Row3;
            public Vector4 Row4;
        };
        public Pivot EnvPivot;

        public Vector4 Vec80;

    }
}
