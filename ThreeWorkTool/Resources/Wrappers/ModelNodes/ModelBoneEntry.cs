using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ThreeWorkTool.Resources.Archives;
using ThreeWorkTool.Resources.Wrappers;

namespace ThreeWorkTool.Resources.Wrappers.ModelNodes
{
    public class ModelBoneEntry : DefaultWrapper
    {
        public int ID;
        public int Parent;
        public int Field3;
        public float Field4;
        public float Length;
        public Vector3 Offset;
        public Vector4 LocalMatrix;
        public Vector4 WorldMatrix;


    }
}
