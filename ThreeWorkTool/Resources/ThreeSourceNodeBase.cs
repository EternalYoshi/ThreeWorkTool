using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Archives;

namespace ThreeWorkTool.Resources
{
    public abstract class ThreeSourceNodeBase : TreeNode
    {
        public ArcEntry entryfile;
        public string FileExt;
    }
}
