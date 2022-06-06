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
    public class EffectFieldTextureRefernce
    {

        public string TextureName;
        public int Offset;


        [Category("Effect Texture Reference"), ReadOnlyAttribute(true)]
        public int TextureNameOffset
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

        [Category("Effect Texture Reference"), ReadOnlyAttribute(true)]
        public string TextureNameReference
        {

            get
            {
                return TextureName;
            }
            set
            {
                TextureName = value;
            }
        }

    }
}
