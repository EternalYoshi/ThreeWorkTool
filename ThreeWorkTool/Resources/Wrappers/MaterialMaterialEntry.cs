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
        public int CmdBufferSize;
        public string MateialinfoFlags;
        public string UnknownField24;
        public string UnknownField28;
        public string UnknownField2C;
        public string UnknownField30;
        public int AnimDataSize;
        public int CmdListOffset;
        public int AnimDataOffset;
        public uint SomethingLabeledP;
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
            public uint SomeField04;
            public Value MaterialCommandValue;
            public MatShaderObject CmdShaderObject;
            public uint SomeField14;
        }

        public struct MatCmdInfo
        {
            public uint SomeValue;
            public int SetFlag;
            public int ShaderObjectIndex;
        }

        public struct Value
        {
            public UInt64 ConstantBufferDataOffset;
            public MatShaderObject VShaderObjectID;
            public uint TextureIndex;
        }

    }
}
