using System;
using System.ComponentModel;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Archives;

namespace ThreeWorkTool
{
    public class ThreeSourceTree : TreeView
    {

        public ArcFile archivefile { get; set; }

        public event EventHandler SelectionChanged;

        private bool _EnableContextMenus = true;
        [DefaultValue(true)]
        public bool EnableContextMenus
        {
            get { return _EnableContextMenus; }
            set { _EnableContextMenus = value; }
        }


        private TreeNode _selected;
        new public TreeNode SelectedNode
        {
            get { return base.SelectedNode; }

            set
            {
                if (_selected == value) return;

                _selected = base.SelectedNode = value;
                SelectionChanged?.Invoke(this, null);

            }
        }

        /*
         
        private TreeNode _selected;
        new public TreeNode SelectedNode
        {
            get { return base.SelectedNode; }

            set
            {
                if (_selected == value) return;

                _selected = base.SelectedNode = value;
                SelectionChanged?.Invoke(this, null);

            }
        }
          
        */

        public ThreeSourceTree()
        {
            this.SetStyle(ControlStyles.UserMouse, true);
        }

    }
}
