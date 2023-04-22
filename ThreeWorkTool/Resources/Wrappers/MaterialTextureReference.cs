using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeWorkTool.Resources.Utility;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class MaterialTextureReference
    {
        public string FullTexName;
        public string TypeHash;
        public string TypeName;
        public int UnknownParam04;
        public int UnknownParam08;
        public int UnknownParam0C;
        public int UnknownParam10;
        public int UnknownParam14;
        public int Index;
        public const int ENTRYSIZE = 0x58;


        public MaterialTextureReference FillMaterialTexReference(MaterialEntry Mat, int ID, BinaryReader bnr, MaterialTextureReference texref)
        {
            //Typehash.
            texref.TypeHash = ByteUtilitarian.BytesToStringL2R(bnr.ReadBytes(4).ToList(), texref.TypeHash);
            texref.UnknownParam04 = bnr.ReadInt32();
            texref.UnknownParam08 = bnr.ReadInt32();
            texref.UnknownParam0C = bnr.ReadInt32();
            texref.UnknownParam10 = bnr.ReadInt32();
            texref.UnknownParam14 = bnr.ReadInt32();
            //Name.
            texref.FullTexName = Encoding.ASCII.GetString(bnr.ReadBytes(64)).Trim('\0');
            texref.Index = ID + 1;

            return texref;
        }

        [Category("Material Texture Reference"), ReadOnlyAttribute(true)]
        public int TextureReferenceIndex
        {

            get
            {
                return Index;
            }
            set
            {
                Index = value;
            }
        }

    }
}
