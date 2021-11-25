using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Utility;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class MaterialMaterialEntry
    {
        public string MatName;
        public string MatType;
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
        public int Index;
        public uint NameTemp;
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

        public MaterialMaterialEntry FIllMatMatEntryPropertiesPart1(MaterialMaterialEntry MME, MaterialEntry ParentMat ,BinaryReader bnr, MemoryStream MatStrim, int OffsetToStart, int ID)
        {
            MME.Index = ID;
            MME.TypeHash = ByteUtilitarian.BytesToStringL2R(bnr.ReadBytes(4).ToList(), MME.TypeHash);

            //Gets the Material type.
            MME.MatType = CFGHandler.ArchiveHashToName(MME.MatType, MME.TypeHash);

            MME.UnknownField04 = bnr.ReadInt32();
            byte[] NameHashBytes = bnr.ReadBytes(4);
            MME.NameHash = ByteUtilitarian.BytesToStringL2R(NameHashBytes.ToList(), MME.TypeHash);
            NameTemp = BitConverter.ToUInt32(NameHashBytes, 0);

            //ShaderObjects.
            MME.BlendState = new MatShaderObject();
            MME.DepthStencilState = new MatShaderObject();
            MME.RasterizerState = new MatShaderObject();
            MME.CmdBufferSize = bnr.ReadInt32();

            byte[] ShadeTemp = new byte[4];
            ShadeTemp = bnr.ReadBytes(4);
            uint ShadeUInt = BitConverter.ToUInt32(ShadeTemp, 0);
            MME.BlendState.Index = Convert.ToInt32(ShadeUInt & 0x00000FFF);
            MME.BlendState.Hash = "";
            MME.BlendState.Hash = CFGHandler.ShaderHashToName(MME.BlendState.Hash, MME.BlendState.Index);

            ShadeTemp = bnr.ReadBytes(4);
            ShadeUInt = BitConverter.ToUInt32(ShadeTemp, 0);
            MME.DepthStencilState.Index = Convert.ToInt32(ShadeUInt & 0x00000FFF);
            MME.DepthStencilState.Hash = "";
            MME.DepthStencilState.Hash = CFGHandler.ShaderHashToName(MME.DepthStencilState.Hash, MME.DepthStencilState.Index);


            ShadeTemp = bnr.ReadBytes(4);
            ShadeUInt = BitConverter.ToUInt32(ShadeTemp, 0);
            MME.RasterizerState.Index = Convert.ToInt32(ShadeUInt & 0x00000FFF);
            MME.RasterizerState.Hash = "";
            MME.RasterizerState.Hash = CFGHandler.ShaderHashToName(MME.RasterizerState.Hash, MME.RasterizerState.Index);
            MME.MaterialCommandListInfo = new MaterialCmdListInfo();

            //The Material Command List Info.
            ShadeTemp = bnr.ReadBytes(4);
            ShadeUInt = BitConverter.ToUInt32(ShadeTemp, 0);
            MME.MaterialCommandListInfo.Count = Convert.ToInt32(ShadeUInt & 0xFFF);
            MME.MaterialCommandListInfo.Unknown = Convert.ToInt32(ShadeUInt & 0xFFFF000);
            MME.MaterialinfoFlags = ByteUtilitarian.BytesToStringL2R(bnr.ReadBytes(4).ToList(), MME.MaterialinfoFlags);
            MME.UnknownField24 = bnr.ReadInt32();
            MME.UnknownField28 = bnr.ReadInt32();
            MME.UnknownField2C = bnr.ReadInt32();
            MME.UnknownField30 = bnr.ReadInt32();
            MME.AnimDataSize = bnr.ReadInt32();
            MME.CmdListOffset = Convert.ToInt32(bnr.ReadInt64());
            MME.AnimDataOffset = Convert.ToInt32(bnr.ReadInt64());
            OffsetToStart = Convert.ToInt32(bnr.BaseStream.Position);

            return MME;
        }

        #region MaterialSubEntry Properties
        [Category("Material Data"), ReadOnlyAttribute(true)]
        public string MaterialType
        {

            get
            {
                return MatType;
            }
            set
            {
                MatType = value;
            }

        }

        [Category("Material Data"), ReadOnlyAttribute(true)]
        public int BlendStateIndex
        {

            get
            {
                return BlendState.Index;
            }
            set
            {
                BlendState.Index = value;
            }

        }

        [Category("Material Data"), ReadOnlyAttribute(true)]
        public string BlendStateType
        {

            get
            {
                return BlendState.Hash;
            }
            set
            {
                BlendState.Hash = value;
            }

        }

        [Category("Material Data"), ReadOnlyAttribute(true)]
        public int DepthStencilStateIndex
        {

            get
            {
                return DepthStencilState.Index;
            }
            set
            {
                DepthStencilState.Index = value;
            }

        }

        [Category("Material Data"), ReadOnlyAttribute(true)]
        public string DepthStencilStateType
        {

            get
            {
                return DepthStencilState.Hash;
            }
            set
            {
                DepthStencilState.Hash = value;
            }

        }

        [Category("Material Data"), ReadOnlyAttribute(true)]
        public int RasterizerStateIndex
        {

            get
            {
                return RasterizerState.Index;
            }
            set
            {
                RasterizerState.Index = value;
            }

        }

        [Category("Material Data"), ReadOnlyAttribute(true)]
        public string RasterizerStateType
        {

            get
            {
                return RasterizerState.Hash;
            }
            set
            {
                RasterizerState.Hash = value;
            }

        }

        private string _Name;
        [Category("Misc"), ReadOnlyAttribute(true)]
        public string Name
        {

            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        [Category("Misc"), ReadOnlyAttribute(true)]
        public int CommandListOffset
        {

            get
            {
                return CmdListOffset;
            }
            set
            {
                CmdListOffset = value;
            }
        }

        [Category("Misc"), ReadOnlyAttribute(true)]
        public int CommandListBufferSize
        {

            get
            {
                return CmdBufferSize;
            }
            set
            {
                CmdBufferSize = value;
            }
        }

        [Category("Index"), ReadOnlyAttribute(true)]
        public int SubIndex
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

        #endregion


    }
}
