using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class MaterialTextureReference
    {
        public string FullTexName;
        public string TypeHash;
        public int UnknownParam04;
        public int UnknownParam08;
        public int UnknownParam0C;
        public int UnknownParam10;
        public int UnknownParam14;
        public int Index;

        public int _Index;
        [Category("Material Texture Reference"), ReadOnlyAttribute(true)]
        public int TextureReferenceIndex
        {

            get
            {
                return _Index;
            }
            set
            {
                _Index = value;
            }
        }

    }
}
