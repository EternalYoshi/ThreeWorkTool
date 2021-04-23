using System;
using System.Windows.Forms;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class ArcEntryWrapper : ThreeSourceNodeBase
    {
        //public ArcEntry entryData;
        public object entryData;

        //Makes a new node from scratch for the imported file.
        public static ArcEntryWrapper EntryCreator(ArcEntryWrapper node, TreeView Tree, string filename, Type filetype = null)
        {
 //           node.entryData = ArcEntry.InsertEntry(Tree,node,filename);

                return node;



        }
    }
}
