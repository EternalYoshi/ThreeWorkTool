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
