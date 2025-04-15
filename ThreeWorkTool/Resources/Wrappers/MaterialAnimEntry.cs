using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Archives;
using ThreeWorkTool.Resources.Utility;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using ThreeWorkTool.Resources.Wrappers.ExtraNodes;
using static ThreeWorkTool.Resources.Wrappers.MaterialMaterialEntry;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class MaterialAnimEntry 
    {
        public int AnimSize;
        public int AnimOffset;
        public byte[] RawData;
        public int MaterialIndex;
        public bool IsNew;


        [Category("Material Animation Data"), ReadOnlyAttribute(true)]
        public int AnimSizeT
        {

            get
            {
                return AnimSize;
            }
            set
            {
                AnimSize = value;
            }
        }

        [Category("Material Animation Data"), ReadOnlyAttribute(false)]
        public int MatIndex
        {

            get
            {
                return MaterialIndex;
            }
            set
            {
                MaterialIndex = value;
            }
        }

    }
}
