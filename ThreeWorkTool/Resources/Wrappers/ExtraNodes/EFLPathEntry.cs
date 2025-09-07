using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeWorkTool.Resources.Utility;


namespace ThreeWorkTool.Resources.Wrappers.ExtraNodes
{
    public class EFLPathEntry
    {
        public string FullTexName;
        public int Offset;
        public int Index;

        [Category("EFL File Path"), ReadOnlyAttribute(true)]
        public int PathOffset
        {

            get
            {
                return Offset;
            }
            set
            {
                Offset = value;
            }
        }

    }



}
