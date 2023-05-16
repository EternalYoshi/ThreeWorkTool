using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;
using System.Runtime.InteropServices;
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
        public List<MatCmd> MaterialCommands { get; set; }
        public byte[] ConstantBufferData;
        public int CommandBufferIndex;
        public string SubMaterialYMLData;
        public byte[] NameHashBytes;

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

        [TypeConverter(typeof(CollectionConverter))]
        public class MatCmd
        {
            public const int SIZE = 0x18;
            public int cmdInt;
            public string CmdType;
            public string CmdName;
            public MatCmdInfo MCInfo;
            public int SomeField04;
            public MatCmdData MaterialCommandData;
            public MatShaderObject CmdShaderObject;
            public int SomeField14;
            public string DataStr;
            public List<float> RawFloats { get; set; }
            public string FloatStr;
            public string FinalData;
        }

        /*        
        [TypeConverter(typeof(CollectionConverter))]
        public class MatCmd
        {
            public const int SIZE = 0x18;
            public int cmdInt;
            public string CmdType;
            public string CmdName;
            public MatCmdInfo MCInfo;
            public int SomeField04;
            public MatCmdData MaterialCommandData;
            public MatShaderObject CmdShaderObject;
            public int SomeField14;
            public string DataStr;
            public List<float> RawFloats { get; set; }
            public string FloatStr;
            public string FinalData;
        }
        */

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public struct MatCmdInfo
        {
            public int SomeValue;
            public string CmdFlag;
            public int ShaderObjectIndex;
            public int TypeInt;
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public struct MatCmdData
        {

            public long ConstantBufferDataOffset;
            public MatShaderObject VShaderObjectID;
            public long TextureIndex;
            public string FileRef;
            public List<float> RawFloats;

        }

        public MaterialMaterialEntry FIllMatMatEntryPropertiesPart1(MaterialMaterialEntry MME, MaterialEntry ParentMat, BinaryReader bnr, int OffsetToStart, int ID)
        {
            MME.SubMaterialYMLData = "";
            //Experimental.
            MME.Index = ID;
            MME.TypeHash = ByteUtilitarian.BytesToStringL2R(bnr.ReadBytes(4).ToList(), MME.TypeHash);

            //Gets the Material type.
            MME.MatType = CFGHandler.ArchiveHashToName(MME.MatType, MME.TypeHash);

            MME.UnknownField04 = bnr.ReadInt32();
            MME.NameHashBytes = bnr.ReadBytes(4);
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
            ulong TempUintTwo = 0;
            long UnionUTemp = 0;
            long OffTemp = 0;
            byte[] ShadeTemp = new byte[4];
            byte[] UnionTemp = new byte[8];


            for (int i = 0; i < MME.MaterialCommandListInfo.Count; i++)
            {

                MatCmd Command = new MatCmd();

                //For the Command Info.
                ShadeTemp = bnr.ReadBytes(4);
                uint CmdInfoTemp = BitConverter.ToUInt32(ShadeTemp, 0);

                //First Param.
                Command.MCInfo = new MatCmdInfo();
                Command.MCInfo.CmdFlag = ((ENumerators.IMatType)Convert.ToInt32(CmdInfoTemp & 0x1F)).ToString();
                Command.MCInfo.SomeValue = Convert.ToInt32(CmdInfoTemp & 0x0000FFF0);
                Command.MCInfo.ShaderObjectIndex = Convert.ToInt32((CmdInfoTemp >> 20) & 0x1fff);
                Command.MCInfo.TypeInt = Convert.ToInt32(CmdInfoTemp & 0x1F);
                Command.SomeField04 = bnr.ReadInt32();

                //For the Union.
                UnionTemp = bnr.ReadBytes(8);
                UnionUTemp = BitConverter.ToInt64(UnionTemp, 0);

                ShadeTemp = bnr.ReadBytes(4);
                uint ShObIDTemp = BitConverter.ToUInt32(ShadeTemp, 0);
                ShObIDTemp = (ShObIDTemp & 0xFFFFF000) >> 12;
                Command.SomeField14 = bnr.ReadInt32();
                Command.cmdInt = Convert.ToInt32(ShadeUInt & 0x1f);
                Command.CmdType = ((ENumerators.IMatType)Convert.ToInt32(ShadeUInt & 0x1f)).ToString();
                Command.CmdName = CFGHandler.ShaderHashToName(Command.CmdName, Convert.ToInt32(BitConverter.ToUInt64(UnionTemp, 0) & 0x00000FFF));

                OffTemp = bnr.BaseStream.Position;

                //Second Param.
                Command.MaterialCommandData = new MatCmdData();
                Command.MaterialCommandData.ConstantBufferDataOffset = Convert.ToInt64(UnionUTemp);

                Command.MaterialCommandData = GetMaterialCmdData(MME, Command, Command.MaterialCommandData, ShadeTemp, UnionTemp, Command.cmdInt, bnr, ShObIDTemp);
                
                if (Command.MaterialCommandData.RawFloats != null)
                {
                    Command.RawFloats = new List<float>();
                    Command.RawFloats = Command.MaterialCommandData.RawFloats;
                }

                if (Command.MCInfo.CmdFlag == "texture")
                {
                    if (Command.MaterialCommandData.TextureIndex > 0)
                    {
                        Command.DataStr = ParentMat.Textures[Convert.ToInt32(Command.MaterialCommandData.TextureIndex - 1)].FullTexName;
                    }
                    else
                    {
                        Command.FinalData = Command.FinalData + "";
                    }
                }
                else if (Command.MCInfo.CmdFlag == "flag" || Command.MCInfo.CmdFlag == "samplerstate")
                {
                    //Command.DataStr = CFGHandler.ShaderHashToName(Command.DataStr, Convert.ToInt32(BitConverter.ToUInt64(UnionTemp, 0) & 0x00000FFF));
                    Command.DataStr = Command.MCInfo.CmdFlag;
                }
                else if (Command.MCInfo.CmdFlag == "cbuffer")
                {
                    Command.DataStr = string.Join(",", Command.RawFloats);

                    Command.FloatStr = "";
                    if(Command.MaterialCommandData.RawFloats.Count <= 4)
                    {
                        Command.FloatStr = Command.FloatStr + "[";

                        for (int f = 0; f < Command.MaterialCommandData.RawFloats.Count; f++)
                        {

                            if (((f + 0) % 4) == 0 && f > 3)
                            {
                                Command.FloatStr = Command.FloatStr + "\n                " + String.Format("{0:0.0###############}", Command.MaterialCommandData.RawFloats[f]) + ", ";
                            }
                            else if (f == 3)
                            {
                                Command.FloatStr = Command.FloatStr + String.Format("{0:0.0###############}", Command.MaterialCommandData.RawFloats[f]);
                            }
                            else
                            {
                                Command.FloatStr = Command.FloatStr + String.Format("{0:0.0###############}", Command.MaterialCommandData.RawFloats[f]) + ", ";
                            }

                        }
                        Command.FloatStr = Command.FloatStr + "]";
                    }
                    else
                    {
                        Command.FloatStr = Command.FloatStr + "[\n                ";

                        for (int f = 0; f < Command.MaterialCommandData.RawFloats.Count; f++)
                        {

                            if (((f + 0) % 4) == 0 && f > 3)
                            {
                                Command.FloatStr = Command.FloatStr + "\n                " + String.Format("{0:0.0###############}", Command.MaterialCommandData.RawFloats[f]) + ", ";
                            }
                            else
                            {
                                Command.FloatStr = Command.FloatStr + String.Format("{0:0.0###############}", Command.MaterialCommandData.RawFloats[f]) + ", ";
                            }

                        }
                        Command.FloatStr = Command.FloatStr + "\n              ]";
                    }


                }
                else
                {
                    Command.DataStr = Command.MCInfo.CmdFlag;
                }

                MME.MaterialCommands.Add(Command);
                bnr.BaseStream.Position = OffTemp;
            }

            MME.ConstantBufferData = bnr.ReadBytes(MME.CmdBufferSize);

            //int inttemp = int.Parse(MME.NameHash, System.Globalization.NumberStyles.HexNumber);
            //string DerpTemp = Convert.ToString(inttemp);

            long intValue = Convert.ToInt64(MME.NameHash, 16);
            string IntHash = Convert.ToString(intValue);

            MME.MatName = CFGHandler.MaterialHashToName(MME.MatName, IntHash);




            return MME;

        }

        //Gets all the needed data associated with the material command. Based off the code TGE wrote for the original model importer.
        public static MatCmdData GetMaterialCmdData(MaterialMaterialEntry MME, MatCmd Command, MatCmdData cmd, byte[] ShadeTemp, byte[] UnionTemp, int cmdType, BinaryReader bnr, uint WeirdUint)
        {

            switch (Command.MCInfo.TypeInt)
            {
                //SetFlag
                case 0:
                    cmd.VShaderObjectID = new MatShaderObject();
                    uint ShadeUInt = BitConverter.ToUInt32(ShadeTemp, 0);
                    cmd.VShaderObjectID.Index = ShadeUInt & 0x00000FFF;
                    cmd.VShaderObjectID.Hash = "";
                    cmd.VShaderObjectID.Hash = CFGHandler.ShaderHashToName(cmd.VShaderObjectID.Hash, Convert.ToInt32(cmd.VShaderObjectID.Index));

                    break;

                //SetConstantBuffer
                case 1:
                    long temp = bnr.BaseStream.Position;

                    //byte[] BTemp = new byte[] { };
                    //BTemp = bnr.ReadBytes(4);
                    //ulong BUInt = BitConverter.ToUInt32(BTemp, 0);
                    //ulong Bindex = (BUInt & 0x00000FFF);

                    bnr.BaseStream.Position = (long)MME.CmdListOffset + Command.MaterialCommandData.ConstantBufferDataOffset;
                    string ShashT = WeirdUint.ToString("X");
                    string HashTemp = "";

                    HashTemp = CFGHandler.ShaderHashToNameTwo(HashTemp, ShashT);

                    cmd.VShaderObjectID = new MatShaderObject();
                    ShadeUInt = BitConverter.ToUInt32(ShadeTemp, 0);
                    cmd.VShaderObjectID.Index = ShadeUInt & 0x00000FFF;
                    cmd.VShaderObjectID.Hash = "";
                    cmd.VShaderObjectID.Hash = CFGHandler.ShaderHashToName(cmd.VShaderObjectID.Hash, Convert.ToInt32(cmd.VShaderObjectID.Index));

                    cmd.RawFloats = new List<float>();

                    switch (HashTemp)
                    {
                        case "CBMaterial":

                            for (int r = 0; r < 32; r++)
                            {
                                cmd.RawFloats.Add(bnr.ReadSingle());
                            }

                            break;

                        case "$Globals":

                            for (int r = 0; r < 76; r++)
                            {
                                cmd.RawFloats.Add(bnr.ReadSingle());
                            }
                            break;

                        case "CBDiffuseColorCorect":

                            for (int r = 0; r < 4; r++)
                            {
                                cmd.RawFloats.Add(bnr.ReadSingle());
                            }
                            break;

                        case "CBHalfLambert":

                            for (int r = 0; r < 4; r++)
                            {
                                cmd.RawFloats.Add(bnr.ReadSingle());
                            }
                            break;

                        case "CBToon2":

                            for (int r = 0; r < 4; r++)
                            {
                                cmd.RawFloats.Add(bnr.ReadSingle());
                            }
                            break;

                        case "CBIndirectUser":

                            for (int r = 0; r < 12; r++)
                            {
                                cmd.RawFloats.Add(bnr.ReadSingle());
                            }
                            break;

                        default:
                            MessageBox.Show("A constant buffer hasn't been handled properly.");
                            break;
                    }

                    bnr.BaseStream.Position = temp;


                    break;

                //SetSamplerState
                case 2:

                    cmd.VShaderObjectID = new MatShaderObject();
                    uint ShadeUIntTWO = BitConverter.ToUInt32(ShadeTemp, 0);
                    cmd.VShaderObjectID.Index = ShadeUIntTWO & 0x00000FFF;
                    cmd.VShaderObjectID.Hash = "";
                    cmd.VShaderObjectID.Hash = CFGHandler.ShaderHashToName(cmd.VShaderObjectID.Hash, Convert.ToInt32(cmd.VShaderObjectID.Index));

                    break;

                //SetTexture
                case 3:

                    uint ShadeUIntST = BitConverter.ToUInt32(UnionTemp, 0);
                    cmd.TextureIndex = ShadeUIntST;
                    uint ShadeUIntTHREE = BitConverter.ToUInt32(ShadeTemp, 0);
                    cmd.VShaderObjectID = new MatShaderObject();
                    cmd.VShaderObjectID.Index = ShadeUIntTHREE & 0x00000FFF;
                    cmd.VShaderObjectID.Hash = "";
                    cmd.VShaderObjectID.Hash = CFGHandler.ShaderHashToName(cmd.VShaderObjectID.Hash, Convert.ToInt32(cmd.VShaderObjectID.Index));

                    break;

                default:
                    break;



            }











            return cmd;

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
