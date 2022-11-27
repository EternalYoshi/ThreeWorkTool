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
        public int JointIndex, Unknwon04, Unknown08, Unknwon0C;
        public Vector4 Vectors10, Vectors20, Vectors30, Vectors80; 
        public struct MTMatrix
        {
            public List<Vector4> Rows;
        }


    }
}
