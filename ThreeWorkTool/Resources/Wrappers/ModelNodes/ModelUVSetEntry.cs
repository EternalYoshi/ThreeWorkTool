using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Archives;
using ThreeWorkTool.Resources.Utility;

namespace ThreeWorkTool.Resources.Wrappers.ModelNodes
{
    public class ModelUVSetEntry
    {
        public int VertexCount;
        public List<float> UVPrimary;
        public List<float> UVSecondary;
        public List<float> UVUnique; //For Light Maps.
        public List<float> UVExtend;





    }
}
