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
            public ulong Index;
            public string Hash;
        }

        public struct MaterialCmdListInfo
        {
            public int Count;
            public int CmdListFlags;
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public struct MatCmd
        {
            public const int SIZE = 0x18;
            public MatCmdInfo MCInfo;
            public int SomeField04;
            public MatCmdData MaterialCommandValue;
            public MatShaderObject CmdShaderObject;
            public int SomeField14;
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public struct MatCmdInfo
        {
            public int SomeValue;
            public string CmdFlag;
            public int ShaderObjectIndex;
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public struct MatCmdData
        {
            public ulong ConstantBufferDataOffset;
            public MatShaderObject VShaderObjectID;
            public int TextureIndex;
        }

        public MaterialMaterialEntry FIllMatMatEntryPropertiesPart1(MaterialMaterialEntry MME, MaterialEntry ParentMat ,BinaryReader bnr, int OffsetToStart, int ID)
        {

            //Experimental.
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
            MME.BlendState.Index = ShadeUInt & 0x00000FFF;
            MME.BlendState.Hash = "";
            MME.BlendState.Hash = CFGHandler.ShaderHashToName(MME.BlendState.Hash, Convert.ToInt32(MME.BlendState.Index));

            ShadeTemp = bnr.ReadBytes(4);
            ShadeUInt = BitConverter.ToUInt32(ShadeTemp, 0);
            MME.DepthStencilState.Index = ShadeUInt & 0x00000FFF;
            MME.DepthStencilState.Hash = "";
            MME.DepthStencilState.Hash = CFGHandler.ShaderHashToName(MME.DepthStencilState.Hash, Convert.ToInt32(MME.DepthStencilState.Index));

            ShadeTemp = bnr.ReadBytes(4);
            ShadeUInt = BitConverter.ToUInt32(ShadeTemp, 0);
            MME.RasterizerState.Index = (ShadeUInt & 0x00000FFF);
            MME.RasterizerState.Hash = "";
            MME.RasterizerState.Hash = CFGHandler.ShaderHashToName(MME.RasterizerState.Hash, Convert.ToInt32(MME.RasterizerState.Index));
            MME.MaterialCommandListInfo = new MaterialCmdListInfo();

            //The Material Command List Info.
            ShadeTemp = bnr.ReadBytes(4);
            ShadeUInt = BitConverter.ToUInt32(ShadeTemp, 0);
            MME.MaterialCommandListInfo.Count = Convert.ToInt32(ShadeUInt & 0xFFF);
            MME.MaterialCommandListInfo.CmdListFlags = Convert.ToInt32(ShadeUInt >> 0xC);
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

        public MaterialMaterialEntry FIllMatMatEntryPropertiesPart2(MaterialMaterialEntry MME, MaterialEntry ParentMat, BinaryReader bnr, int OffsetToStart, int ID)
        {

            bnr.BaseStream.Position = MME.CmdListOffset;
            MME.MaterialCommands = new List<MatCmd>();
            ulong ShadeUInt = 0;
            long UnionUTemp = 0;
            byte[] ShadeTemp = new byte[4];
            byte[] UnionTemp = new byte[8];

            
            for (int i = 0; i < MME.MaterialCommandListInfo.Count; i++)
            {
                
                MatCmd Command = new MatCmd();

                //For the Command Info.
                ShadeTemp = bnr.ReadBytes(4);
                uint CmdInfoTemp = BitConverter.ToUInt32(ShadeTemp, 0);
                Command.MCInfo = new MatCmdInfo();
                Command.MCInfo.CmdFlag = ((ENumerators.IMatType)Convert.ToInt32(ShadeUInt & 0x1f)).ToString();
                Command.MCInfo.SomeValue = Convert.ToInt32(ShadeUInt & 0x0000FFF0);
                Command.MCInfo.ShaderObjectIndex = Convert.ToInt32((ShadeUInt >> 20) & 0x1fff);

                Command.SomeField04 = bnr.ReadInt32();

                //For the Union. Uggggh.
                //0x0000000F
                UnionTemp = bnr.ReadBytes(8);
                UnionUTemp = BitConverter.ToInt64(UnionTemp, 0);
                Command.MaterialCommandValue = new MatCmdData();
                Command.MaterialCommandValue.ConstantBufferDataOffset = Convert.ToUInt64(UnionUTemp);
                Command.MaterialCommandValue.VShaderObjectID = new MatShaderObject();
                Command.MaterialCommandValue.VShaderObjectID.Index = (BitConverter.ToUInt64(UnionTemp, 0) & 0x00000FFF);
                Command.MaterialCommandValue.VShaderObjectID.Hash = "";
                Command.MaterialCommandValue.VShaderObjectID.Hash = CFGHandler.ShaderHashToName(Command.MaterialCommandValue.VShaderObjectID.Hash, Convert.ToInt32(Command.MaterialCommandValue.VShaderObjectID.Index));
                Command.MaterialCommandValue.TextureIndex = BitConverter.ToInt32(UnionTemp, 0);

                Command.MaterialCommandValue.VShaderObjectID = new MatShaderObject();
                ShadeTemp = bnr.ReadBytes(4);
                Command.MaterialCommandValue.VShaderObjectID.Index = (BitConverter.ToUInt32(ShadeTemp, 0) & 0x00000FFF);
                Command.MaterialCommandValue.VShaderObjectID.Hash = "";
                Command.MaterialCommandValue.VShaderObjectID.Hash = CFGHandler.ShaderHashToName(MME.DepthStencilState.Hash, Convert.ToInt32(MME.DepthStencilState.Index));

                Command.SomeField14 = bnr.ReadInt32();

                MME.MaterialCommands.Add(Command);
                
            }
            
            MME.ConstantBufferData = bnr.ReadBytes(MME.CmdBufferSize);

            return MME;

        }

#region MaterialSubEntry Properties
        [Category("Material Data"), ReadOnlyAttribute(false)]
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

        [Category("Material Data"), ReadOnlyAttribute(false)]
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

        [Category("Material Data"), ReadOnlyAttribute(false)]
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

        [Category("Material Data"), ReadOnlyAttribute(false)]
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

        [Category("Material Data"), ReadOnlyAttribute(false)]
        public string matFlags
        {

            get
            {
                return MaterialinfoFlags;
            }
            set
            {
                MaterialinfoFlags = value;
            }
        }

        [Category("Material Data"), ReadOnlyAttribute(false)]
        public int cmdListFlags
        {

            get
            {
                return MaterialCommandListInfo.CmdListFlags;
            }
            set
            {
                MaterialCommandListInfo.CmdListFlags = value;
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
