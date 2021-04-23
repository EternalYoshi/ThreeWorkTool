using System.Windows.Forms;
using ThreeWorkTool.Resources.Archives;
using ThreeWorkTool.Resources.Wrappers;

namespace ThreeWorkTool.Resources
{
    public abstract class ThreeSourceNodeBase : TreeNode
    {
        public object entryfile;
        public ArcEntry aentryfile;
        public TextureEntry textfile;
        public string FileExt;
    }
}
