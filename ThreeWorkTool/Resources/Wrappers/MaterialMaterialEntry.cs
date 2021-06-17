using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class MaterialMaterialEntry
    {
        public string MatName;
        public string UnknownField04;
        public string TypeHash;
        public string UnknownField;
        public string NameHash;
        public uint CmdBufferSize;
        public string MateialinfoFlags;
        public uint UnknownField24;
        public uint UnknownField28;
        public uint UnknownField2C;
        public uint UnknownField30;
        public uint AnimDataSize;
        public uint CmdListOffset;
        public uint AnimDataOffset;
        public uint SomethingLabeledP;
        public uint Index;
        public MatShaderObject BlendState;
        public MatShaderObject DepthStencilState;
        public MatShaderObject RasterizerState;
        public List<MaterialCmd> MaterialCommands;
        public List<MatCmd> matCmds;
        public List<byte> ConstantBufferData;

        public struct MatShaderObject
        {
            public int Index;
            public string Hash;
        }

        public struct MaterialCmd
        {

        }

        public struct MatCmd
        {

        }


    }
}
