using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Archives;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class TexWrapper:ArcEntryWrapper
    {
        public string Magic;
        public int XSize;
        public int YSize;
        public int ZSize;
        public int version;
        public string TexType;
        public bool HasTransparency;
        public bool HasMips;
        public int MipCount;




    }
}
