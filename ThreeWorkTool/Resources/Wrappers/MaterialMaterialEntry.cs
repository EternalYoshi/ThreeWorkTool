using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class MaterialMaterialEntry
    {
        public string MatName;
        public int UnknownField04;
        public string TypeHash;
        public string UnknownField;
        public string NameHash;
        public int CmdBufferSize;
        public string MaterialinfoFlags;
        public int UnknownField24;
        public int UnknownField28;
        public int UnknownField2C;
        public int UnknownField30;
        public int AnimDataSize;
        public int CmdListOffset;
        public int AnimDataOffset;
        public int SomethingLabeledP;
        public uint Index;
        public MatShaderObject BlendState;
        public MatShaderObject DepthStencilState;
        public MatShaderObject RasterizerState;
        public MaterialCmdListInfo MaterialCommandListInfo;
        public List<MatCmd> MaterialCommands;
        public byte[] ConstantBufferData;
        public int CommandBufferIndex;

        public struct MatShaderObject
        {
            public int Index;
            public string Hash;
        }

        public struct MaterialCmdListInfo
        {
            public int Count;
            public int Unknown;
        }

        public struct MatCmd
        {
            public MatCmdInfo MCInfo;
            public int SomeField04;
            public Value MaterialCommandValue;
            public MatShaderObject CmdShaderObject;
            public int SomeField14;
        }

        public struct MatCmdInfo
        {
            public int SomeValue;
            public int SetFlag;
            public int ShaderObjectIndex;
        }

        public struct Value
        {
            public int ConstantBufferDataOffset;
            public MatShaderObject VShaderObjectID;
            public int TextureIndex;
        }

        public MaterialMaterialEntry FIllProperties(MaterialMaterialEntry MME)
        {


            string TypeHash = "";
            string line = "";
            //Looks through the archive_filetypes.cfg file to find the typehash associated with the extension.
            try
            {
                using (var sr2 = new StreamReader("archive_filetypes.cfg"))
                {
                    while (!sr2.EndOfStream)
                    {
                        var keyword = Console.ReadLine() ?? MME.TypeHash;
                        line = sr2.ReadLine();
                        if (String.IsNullOrEmpty(line)) continue;
                        if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                        {
                            TypeHash = line;
                            TypeHash = TypeHash.Split(' ')[2];
                            MME._MaterialType = TypeHash;
                            break;
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("I cannot find and/or access archive_filetypes.cfg so I cannot finish parsing the arc.", "Oh Boy");
                using (StreamWriter sw = File.AppendText("Log.txt"))
                {
                    sw.WriteLine("Cannot find archive_filetypes.cfg so I cannot continue parsing the file.");
                }
                return null;
            }

            return MME;
        }

        #region MaterialMaterialEntry Properties
        
        private string _MaterialType;
        [Category("Material Data"), ReadOnlyAttribute(true)]
        public string MaterialType
        {

            get
            {
                return _MaterialType;
            }
            set
            {
                _MaterialType = value;
            }
        }

        #endregion


    }
}
