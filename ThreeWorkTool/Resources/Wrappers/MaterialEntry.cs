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
using System.Collections;
using Force.Crc32;
using static ThreeWorkTool.Resources.Utility.ByteUtilitarian;
using System.Buffers.Binary;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class MaterialEntry : DefaultWrapper
    {
        public const int SIZE = 0x28;
        public const int MATSIZE = 0x48;
        public const int MAX_NAME_LENGTH = 64;
        public const int TEXENTRYSIZE = 0x58;
        public string Magic;
        public string Constant;
        public int EntryCount;
        public byte[] WTemp;
        public int SomethingCount;
        public int TextureCount;
        public int MaterialCount;
        public long TextureOffset;
        public long MaterialOffset;
        public int UnknownField;
        public string WeirdHash;
        public int Field14;
        public List<MaterialTextureReference> Textures;
        public List<MaterialMaterialEntry> Materials;
        public string YMLText;
        public MTMaterial YMLMat { get; set; }
        public List<MaterialAnimEntry> MatAnims;

        public static MaterialEntry FillMatEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
        {
            var MATEntry = new MaterialEntry();
            List<byte> BTemp = new List<byte>();

            FillEntry(filename, subnames, tree, br, c, ID, MATEntry);

            //Decompression Time.
            MATEntry.UncompressedData = ZlibStream.UncompressBuffer(MATEntry.CompressedData);

            //Material specific work here.
            using (MemoryStream MatStream = new MemoryStream(MATEntry.UncompressedData))
            {
                using (BinaryReader MBR = new BinaryReader(MatStream))
                {
                    BuildMatEntry(MBR, MATEntry);
                }
            }

            return MATEntry;

        }

        public static MaterialEntry ReplaceMat(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {

            MaterialEntry matentry = new MaterialEntry();
            MaterialEntry oldentry = new MaterialEntry();

            tree.BeginUpdate();

            ReplaceEntry(tree, node, filename, matentry, oldentry);
            matentry.FileName = matentry.TrueName;
            //Type Specific Work Here.
            using (MemoryStream LmtStream = new MemoryStream(matentry.UncompressedData))
            {
                using (BinaryReader bnr = new BinaryReader(LmtStream))
                {
                    BuildMatEntry(bnr, matentry);
                }
            }

            return node.entryfile as MaterialEntry;

        }

        public static MaterialEntry InsertEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            MaterialEntry matentry = new MaterialEntry();

            try
            {
                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
                {
                    InsertKnownEntry(tree, node, filename, matentry, bnr);
                }
            }
            catch (Exception ex)
            {
                string ProperPath = "";
                ProperPath = Globals.ToolPath + "Log.txt"; using (StreamWriter sw = File.AppendText(ProperPath))
                {
                    sw.WriteLine("Caught an exception using the BinaryReader. Here's the details:\n" + ex);
                }
            }

            //Decompression Time.
            matentry.UncompressedData = ZlibStream.UncompressBuffer(matentry.CompressedData);

            try
            {
                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
                {
                    //Material specific work here.
                    using (MemoryStream MatStream = new MemoryStream(matentry.UncompressedData))
                    {
                        using (BinaryReader MBR = new BinaryReader(MatStream))
                        {
                            BuildMatEntry(MBR, matentry);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string ProperPath = "";
                ProperPath = Globals.ToolPath + "Log.txt"; using (StreamWriter sw = File.AppendText(ProperPath))
                {
                    sw.WriteLine("Caught an exception using the BinaryReader. Here's the details:\n" + ex);
                }
            }

            return matentry;
        }

        public static MaterialEntry BuildMatEntry(BinaryReader MBR, MaterialEntry MATEntry)
        {

            //Header variables.
            MATEntry.Magic = ByteUtilitarian.BytesToStringL2R(MBR.ReadBytes(4).ToList(), MATEntry.Magic);
            MATEntry.SomethingCount = MBR.ReadInt32();
            MATEntry.MaterialCount = MBR.ReadInt32();
            MATEntry.TextureCount = MBR.ReadInt32();
            MATEntry.WeirdHash = ByteUtilitarian.BytesToStringL2R(MBR.ReadBytes(4).ToList(), MATEntry.Magic);
            MATEntry.Field14 = MBR.ReadInt32();
            MATEntry.TextureOffset = MBR.ReadInt64();
            MATEntry.MaterialOffset = MBR.ReadInt64();

            //For the Texture References.
            MATEntry.Textures = new List<MaterialTextureReference>();
            MBR.BaseStream.Position = MATEntry.TextureOffset;
            for (int i = 0; i < MATEntry.TextureCount; i++)
            {
                MaterialTextureReference TexTemp = new MaterialTextureReference();
                TexTemp = TexTemp.FillMaterialTexReference(MATEntry, i, MBR, TexTemp);
                MATEntry.Textures.Add(TexTemp);
            }

            //Now for the Materials themselves.
            MATEntry.Materials = new List<MaterialMaterialEntry>();
            byte[] ShadeTemp = new byte[4];
            int PrevOffset = Convert.ToInt32(MBR.BaseStream.Position);
            MATEntry.MatAnims = new List<MaterialAnimEntry>();
            //Materials.
            for (int i = 0; i < MATEntry.MaterialCount; i++)
            {

                MaterialMaterialEntry MMEntry = new MaterialMaterialEntry();
                MMEntry = MMEntry.FIllMatMatEntryPropertiesPart1(MMEntry, MATEntry, MBR, PrevOffset, i);
                MMEntry = MMEntry.FIllMatMatEntryPropertiesPart2(MMEntry, MATEntry, MBR, PrevOffset, i);

                MATEntry.Materials.Add(MMEntry);
                PrevOffset = PrevOffset + 72;

                //Experimental. Gets the animation data.
                if (MMEntry.AnimDataSize > 0)
                {
                    MBR.BaseStream.Position = MMEntry.AnimDataOffset;
                    MaterialAnimEntry Manim = new MaterialAnimEntry();
                    Manim.RawData = MBR.ReadBytes(MMEntry.AnimDataSize);
                    Manim.MaterialIndex = i;
                    Manim.AnimSize = Manim.RawData.Count();
                    Manim.AnimOffset = MMEntry.AnimDataOffset;
                    Manim.IsNew = false;
                    MATEntry.MatAnims.Add(Manim);
                }

                MBR.BaseStream.Position = PrevOffset;

            }


            MATEntry = BuildYML(MATEntry, MATEntry.YMLText);

            return MATEntry;

        }

        //This function based off code TGE Wrote for Material support in the importer.
        public static MaterialEntry BuildYML(MaterialEntry MATEntry, string YML)
        {

            YML = "";
            //YML = YML + "version: 1\n";
            YML = YML + "version: 2\n";
            YML = YML + "materials:\n";
            var encoding = Encoding.ASCII;

            //Materials.
            for (int y = 0; y < MATEntry.Materials.Count; y++)
            {
                if (MATEntry.Materials[y].MatName == "" || MATEntry.Materials[y].MatName == null)
                {
                    YML = YML + "    - _0x" + MATEntry.Materials[y].NameHash + ":\n";
                }
                else
                {
                    YML = YML + "    - " + MATEntry.Materials[y].MatName + ":\n";
                }
                YML = YML + "        type: " + MATEntry.Materials[y].MatType + "\n";
                YML = YML + "        blendState: " + MATEntry.Materials[y].BlendStateType + "\n";
                YML = YML + "        depthStencilState: " + MATEntry.Materials[y].DepthStencilStateType + "\n";
                YML = YML + "        rasterizerState: " + MATEntry.Materials[y].RasterizerStateType + "\n";
                YML = YML + "        cmdListFlags: 0x" + MATEntry.Materials[y].cmdListFlags.ToString("X") + "\n";
                YML = YML + "        matFlags: 0x" + MATEntry.Materials[y].matFlags + "\n";
                YML = YML + "        cmds:\n";


                //Commands.
                for (int z = 0; z < MATEntry.Materials[y].MaterialCommands.Count; z++)
                {
                    if (MATEntry.Materials[y].MaterialCommands[z].MCInfo.CmdFlag == "cbuffer")
                    {
                        if (MATEntry.Materials[y].MaterialCommands[z].RawFloats.Count <= 1)
                        {
                            YML = YML + "            - [ " + MATEntry.Materials[y].MaterialCommands[z].MCInfo.CmdFlag + ", " + MATEntry.Materials[y].MaterialCommands[z].MaterialCommandData.VShaderObjectID.Hash + ", " + MATEntry.Materials[y].MaterialCommands[z].FloatStr + " ]\n";
                        }
                        else
                        {
                            YML = YML + "            - [ " + MATEntry.Materials[y].MaterialCommands[z].MCInfo.CmdFlag + ", " + MATEntry.Materials[y].MaterialCommands[z].MaterialCommandData.VShaderObjectID.Hash + ", " + MATEntry.Materials[y].MaterialCommands[z].FloatStr + "]\n";
                        }
                    }
                    else if (MATEntry.Materials[y].MaterialCommands[z].MCInfo.CmdFlag == "flag" || MATEntry.Materials[y].MaterialCommands[z].MCInfo.CmdFlag == "samplerstate")
                    {
                        YML = YML + "            - [ " + MATEntry.Materials[y].MaterialCommands[z].MCInfo.CmdFlag + ", " + MATEntry.Materials[y].MaterialCommands[z].MaterialCommandData.VShaderObjectID.Hash + ", " + MATEntry.Materials[y].MaterialCommands[z].CmdName + " ]\n";

                    }
                    else if (MATEntry.Materials[y].MaterialCommands[z].MCInfo.CmdFlag == "texture")
                    {
                        YML = YML + "            - [ " + MATEntry.Materials[y].MaterialCommands[z].MCInfo.CmdFlag + ", " + MATEntry.Materials[y].MaterialCommands[z].MaterialCommandData.VShaderObjectID.Hash + ", " + MATEntry.Materials[y].MaterialCommands[z].DataStr + " ]\n";

                    }
                    //YML = YML + " ";

                }

                //Material Animations.
                if (MATEntry.MatAnims != null || MATEntry.MatAnims.Count > 0)
                {
                    if (MATEntry.Materials[y].AnimDataSize > 0)
                    {
                        for (int v = 0; v < MATEntry.MatAnims.Count; v++)
                        {
                            if (MATEntry.MatAnims[v].MaterialIndex == y)
                            {
                                string MatDat = Convert.ToBase64String(MATEntry.MatAnims[v].RawData);
                                YML = YML + "        animData: " + MatDat + "\n";
                            }
                        }
                    }
                }

            }

            MATEntry.YMLText = YML;

            return MATEntry;
        }

        //This function also based off code TGE wrote for Material support in the importer.
        public static MaterialEntry ReplaceYMLToMRL(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {

            MaterialEntry matentry = new MaterialEntry();
            MaterialEntry oldentry = new MaterialEntry();
            MTMaterial newmatA = new MTMaterial();
            string strtemp = "";
            MTMaterial MatList = new MTMaterial();
            List<byte> NewUncompressedData = new List<byte>();
            int ITextureSize = 0;
            int IMaterialSize = 0;
            int IAnimDataSize = 0;
            int ProjectedFinalMatSize = 0;
            int MatBufferSize = 0;
            List<int> AnimIndexCounter = new List<int>();
            List<string> AnimDataStr = new List<string>();
            List<byte> AnimDataRaw = new List<byte>();
            List<byte> CmdDataRaw = new List<byte>();
            List<byte> ExtraDataRaw = new List<byte>();
            List<byte> TempDataRaw = new List<byte>();            

            //Time to check the yml file.
            using (var input = File.OpenText(filename))
            {
                string ymlfile = File.ReadAllText(filename);
                //var yamlYMLMRLGraph = SerializeAndDeserialize.Deserialize(File.ReadAllLines(filename));
                MatList = SerializeAndDeserialize.Deserialize<MTMaterial>(ymlfile);
                //var deserializer = new DeserializerBuilder().WithTagMapping("", typeof(ThreeWorkTool.Resources.Wrappers.LMTM3AEntry)).Build();
                //newmatA = deserializer.Deserialize<MTMaterial>(input);
            }

            //Now to build the material file from what we got.
            if (MatList != null)
            {
                matentry.YMLMat = MatList;

                //First we're getting the material & texture count so we can build the header.
                List<string> MaterialNames = new List<string>();
                List<string> TextureNames = new List<string>();
                //List<byte[]> RawAnimData = new List<byte[]>();

                int counter = 0;
                foreach (var Mitem in MatList.materials)
                {
                    MaterialNames.AddRange(Mitem.Keys.ToList());
                    foreach (var Command in Mitem.First().Value.cmds)
                    {
                        string[] TempArr = ((IEnumerable)Command).Cast<object>().Select(x => x.ToString()).ToArray();
                        switch (TempArr[0])
                        {
                            case "flag":
                                break;

                            case "cbuffer":
                                break;

                            case "samplerstate":
                                break;

                            case "texture":
                                TextureNames.Add(TempArr[2]);
                                break;

                            default:
                                break;

                        }


                    }

                    //Checks for animation data.
                    if (Mitem.First().Value.animData != null)
                    {
                        AnimDataStr.Add(Mitem.First().Value.animData);
                        AnimIndexCounter.Add(counter);
                    }
                    counter++;
                }

                List<string> DistinctTextureNames = TextureNames.Distinct().ToList();

                //Now to rebuild from scratch.

                //The Header, Part 1.
                byte[] HeaderPart1 = { 0x4D, 0x52, 0x4C, 0x00, 0x22, 0x00, 0x00, 0x00 };
                byte[] PlaceHolderHeaderPartA = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                byte[] HeaderHash = { 0x0A, 0x94, 0x88, 0xE5 };
                byte[] Field14 = { 0x00, 0x00, 0x00, 0x00 };
                int InterpTextureOffset = 40;
                int InterpMaterialOffset = InterpTextureOffset + (DistinctTextureNames.Count * 88);
                byte[] PlaceHolderHeaderPartB = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                byte[] DefaultTextureHash = { 0xEB, 0x5D, 0x1F, 0x24 };
                byte[] TextureFiller = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };


                NewUncompressedData.AddRange(HeaderPart1);
                NewUncompressedData.AddRange(PlaceHolderHeaderPartA);
                NewUncompressedData.AddRange(HeaderHash);
                NewUncompressedData.AddRange(Field14);
                NewUncompressedData.AddRange(PlaceHolderHeaderPartB);

                //Gets Estimated size of texture section.
                ITextureSize = DistinctTextureNames.Count() * 88;

                //Gets estimated size of material section.
                IMaterialSize = MaterialNames.Count * 72;

                //Gets estimated size of anim section, if applicable.
                foreach (string anim in AnimDataStr)
                {
                    AnimDataRaw.AddRange(Convert.FromBase64String(anim));
                }

                IAnimDataSize = AnimDataRaw.Count;

                #region Textures Chunk
                //Texture Names part.
                foreach (string texname in DistinctTextureNames)
                {
                    //First the hash.
                    NewUncompressedData.AddRange(DefaultTextureHash);
                    NewUncompressedData.AddRange(TextureFiller);

                    //Now the actual name.
                    int NumberChars = texname.Length;
                    byte[] namebuffer = Encoding.ASCII.GetBytes(texname);
                    int nblength = namebuffer.Length;

                    //Space for name is 64 bytes so we make a byte array with that size and then inject the name data in it.
                    byte[] writenamedata = new byte[64];
                    Array.Clear(writenamedata, 0, writenamedata.Length);


                    for (int i = 0; i < namebuffer.Length; ++i)
                    {
                        writenamedata[i] = namebuffer[i];
                    }

                    NewUncompressedData.AddRange(writenamedata);

                }          
                #endregion

#if DEBUG
                File.WriteAllBytes("D:\\Workshop\\MaterialWorkshop\\Ryu_PART1TEST.mrl", NewUncompressedData.ToArray());
#endif

                byte[] HashBytes = new byte[4];
                
                //Materials Part.
                for (int i = 0; i < MatList.materials.Count; ++i)
                {
                    List<byte> MatChunkRaw = new List<byte>();
                    List<byte> CmdChunkRaw = new List<byte>();

                    //Checks for unique/undocumented Material Types.
                    if (MatList.materials[i].First().Value.type.Contains("0x"))
                    {
                        //Gotta convert this to hex and remove the 0x.
                        HashBytes = ByteUtilitarian.StringToByteArray(MatList.materials[i].First().Value.type.Substring(2));
                        int Convvalue = Convert.ToInt32(MatList.materials[i].First().Value.type, 16);
                    }
                    else
                    {
                        //Gotta figure out how the hash is computed.
                        //First the hashcode for the Material.
                        Ionic.Zlib.CRC32 crc32 = new Ionic.Zlib.CRC32();
                        //uint TypeTocrc = JamcrcHelper.ComputeJamcrc(MatList.materials[i].First().Value.type);
                        byte[] StringBytes = new byte[4];
                        StringBytes = System.Text.Encoding.ASCII.GetBytes(MatList.materials[i].First().Value.type);
                        uint CRCMatType = ByteUtilitarian.ComputeHash(MatList.materials[i].First().Value.type);
                        HashBytes = BitConverter.GetBytes(CRCMatType);

                        MatChunkRaw.AddRange(HashBytes);
                    }

                    //PlaceHolder for Field04.
                    MatChunkRaw.AddRange(Field14);

                    //Namehash.
                    //StringBytes = System.Text.Encoding.ASCII.GetBytes(MatList.materials[i].First().Key);
                    uint CRCMatName = ByteUtilitarian.ComputeHash(MatList.materials[i].First().Key);
                    HashBytes = BitConverter.GetBytes(CRCMatName);
                    MatChunkRaw.AddRange(HashBytes);

                    //PlaceHolder for CmdBuffersize.
                    MatChunkRaw.AddRange(Field14);

                    //Shaders. BlendState.
                    #region Shaders
                    int TempNum = 0;
                    string Tempstr = "";
                    string NewShadRaw = CFGHandler.ShaderNameToHash(Tempstr, MatList.materials[i].First().Value.blendState);
                    TempNum = CFGHandler.GetShaderNameIndex(i, MatList.materials[i].First().Value.blendState);

                    string binarystringhash = String.Join(String.Empty, NewShadRaw.Select
                        (c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));

                    string binarystringIndHex = TempNum.ToString("X3");

                    string binarystringInd = String.Join(String.Empty, binarystringIndHex.Select
                        (c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));

                    string binarystringcombined = binarystringhash + binarystringInd;

                    int FinalShaderValue = Convert.ToInt32(binarystringcombined, 2);

                    MatChunkRaw.AddRange(BitConverter.GetBytes(FinalShaderValue));

                    //Shaders. DepthStencilState.
                    NewShadRaw = CFGHandler.ShaderNameToHash(Tempstr, MatList.materials[i].First().Value.depthStencilState);
                    TempNum = CFGHandler.GetShaderNameIndex(i, MatList.materials[i].First().Value.depthStencilState);

                    binarystringhash = String.Join(String.Empty, NewShadRaw.Select
                       (c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));

                    binarystringIndHex = TempNum.ToString("X3");

                    binarystringInd = String.Join(String.Empty, binarystringIndHex.Select
                       (c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));

                    binarystringcombined = binarystringhash + binarystringInd;

                    FinalShaderValue = Convert.ToInt32(binarystringcombined, 2);

                    MatChunkRaw.AddRange(BitConverter.GetBytes(FinalShaderValue));

                    //Shaders. RasterizeState.
                    NewShadRaw = CFGHandler.ShaderNameToHash(Tempstr, MatList.materials[i].First().Value.rasterizerState);
                    TempNum = CFGHandler.GetShaderNameIndex(i, MatList.materials[i].First().Value.rasterizerState);

                    binarystringhash = String.Join(String.Empty, NewShadRaw.Select
                        (c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));

                    binarystringIndHex = TempNum.ToString("X3");

                    binarystringInd = String.Join(String.Empty, binarystringIndHex.Select
                       (c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));

                    binarystringcombined = binarystringhash + binarystringInd;

                    FinalShaderValue = Convert.ToInt32(binarystringcombined, 2);

                    MatChunkRaw.AddRange(BitConverter.GetBytes(FinalShaderValue));
                    #endregion

                    //CommandListInfo.
                    string TempCMDFlags = MatList.materials[i].First().Value.cmdListFlags.Substring(2);
                    //int Num = int.Parse(TempCMDFlags);
                    int Num = Convert.ToInt32(TempCMDFlags, 16);
                    binarystringhash = Convert.ToString(Num, 2).PadLeft(20, '0');


                    binarystringIndHex = MatList.materials[i].First().Value.cmds.Count.ToString("X2");

                    binarystringInd = String.Join(String.Empty, binarystringIndHex.Select
                        (c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));

                    binarystringcombined = binarystringhash + binarystringInd;

                    FinalShaderValue = Convert.ToInt32(binarystringcombined, 2);

                    MatChunkRaw.AddRange(BitConverter.GetBytes(FinalShaderValue));

                    //Mat Flags.
                    TempCMDFlags = MatList.materials[i].First().Value.matFlags.Substring(2);
                    HashBytes = ByteUtilitarian.StringToByteArray(TempCMDFlags);
                    MatChunkRaw.AddRange(HashBytes);

                    //Filler/Placeholder for undocumented fields.
                    MatChunkRaw.AddRange(Field14);
                    MatChunkRaw.AddRange(Field14);
                    MatChunkRaw.AddRange(Field14);
                    MatChunkRaw.AddRange(Field14);

                    //AnimDataSize.
                    MatChunkRaw.AddRange(Field14);

                    //Pointers for CmdLists and AnimData.
                    MatChunkRaw.AddRange(Field14);
                    MatChunkRaw.AddRange(Field14);
                    MatChunkRaw.AddRange(Field14);
                    MatChunkRaw.AddRange(Field14);

                    //Now the commands. Are all the commands 24/0x18 bytes?
                    for (int u =0; u < MatList.materials[i].First().Value.cmds.Count; u++)
                    {
                        //First the Command info.

                        //Gotta do the inverse of this block....
                        //Command.MCInfo = new MatCmdInfo();
                        //Command.MCInfo.CmdFlag = ((ENumerators.IMatType)Convert.ToInt32(CmdInfoTemp & 0x1F)).ToString();
                        //Command.MCInfo.SomeValue = Convert.ToInt32(CmdInfoTemp & 0x0000FFF0);
                        //Command.MCInfo.ShaderObjectIndex = Convert.ToInt32((CmdInfoTemp >> 20) & 0x1fff);
                        //Command.MCInfo.TypeInt = Convert.ToInt32(CmdInfoTemp & 0x1F);
                        //Command.SomeField04 = bnr.ReadInt32();

                        

                        //Type.
                        List<object> CmdObj = MatList.materials[i].First().Value.cmds[u] as List<object>;
                        List<float> TempFloats = new List<float>();
                        string CmdType = (CmdObj[0]).ToString();
                        string SecondCmd = "";
                        string ThirdCmd = "";
                        int Hash = 0;
                        int FlagValue = 0;
                        switch (CmdType)
                        {
                            case "flag":
                            case "Flag":
                            case "samplerstate":
                            case "Samplerstate":
                                SecondCmd = (CmdObj[1]).ToString();
                                ThirdCmd = (CmdObj[2]).ToString();

                                if (SecondCmd.StartsWith("0x"))
                                {

                                }
                                else
                                {
                                    Hash = CFGHandler.GetShaderNameIndex(Hash, SecondCmd);
                                    //Gets the hex value to insert together through building the binary.
                                    Tempstr = Convert.ToString(Hash, 2).PadLeft(12,'0');
                                    binarystringhash = Tempstr.PadRight(28,'0');

                                    if(CmdType == "flag" || CmdType == "Flag")
                                    {
                                        binarystringhash = binarystringhash + "0000";
                                    }
                                    else
                                    {
                                        binarystringhash = binarystringhash + "0000";
                                    }

                                    FlagValue = Convert.ToInt32(binarystringhash, 2);
                                    CmdChunkRaw.AddRange(BitConverter.GetBytes(FlagValue));

                                    //Placeholder for Field04.
                                    CmdChunkRaw.AddRange(Field14);

                                    //Placeholder for Rest of the data.
                                    CmdChunkRaw.AddRange(Field14);
                                    CmdChunkRaw.AddRange(Field14);
                                    CmdChunkRaw.AddRange(Field14);
                                    CmdChunkRaw.AddRange(Field14);


                                }

                                break;

                            case "cbuffer":
                            case "Cbuffer":
                                SecondCmd = (CmdObj[1]).ToString();

                                //cbuffer type has an array of floating points.
                                List<object> TempValues = (List<object>)CmdObj[2];
                                float TFloat = 0.0f;
                                foreach(var obj in TempValues)
                                {
                                    string TempStr = obj.ToString();
                                    if (float.TryParse(TempStr, out float value))
                                    {
                                        TFloat = value;
                                        TempFloats.Add(TFloat);
                                        CmdChunkRaw.AddRange(BitConverter.GetBytes(TFloat));
                                    }                                                                        
                                }

                                CmdChunkRaw.AddRange(TempDataRaw);
                                int ArrSize = TempDataRaw.Count();

                                //Gotta update the data chunk of the Material before adding it to NewUncompressedData.

                                break;

                            case "Texture":
                            case "texture":
                                SecondCmd = (CmdObj[1]).ToString();
                                ThirdCmd = (CmdObj[2]).ToString();

                                if(SecondCmd != "" && ThirdCmd != "")
                                {
                                    int Tindex = DistinctTextureNames.IndexOf(ThirdCmd);
                                }

                                //Gotta update the data chunk of the Material before adding it to NewUncompressedData.
                                if (SecondCmd != "")
                                {

                                }

                                break;

                            default:
                                //For undocumented stuff.
                                break;
                        }



                    }



                    File.WriteAllBytes("D:\\Workshop\\MaterialWorkshop\\Ryu_PART2TEST1.mrl", MatChunkRaw.ToArray());


                }



                //Anims Part.



            }

            /*
            tree.BeginUpdate();
            
            ReplaceEntry(tree, node, filename, matentry, oldentry);

            //Type Specific Work Here.
            using (MemoryStream LmtStream = new MemoryStream(matentry.UncompressedData))
            {
                using (BinaryReader bnr = new BinaryReader(LmtStream))
                {
                    BuildMatEntry(bnr, matentry);
                }
            }
            */


            return node.entryfile as MaterialEntry;

        }

        #region Material Properties

        [Category("Material Data"), ReadOnlyAttribute(true)]
        public string FileName
        {

            get
            {
                return TrueName;
            }
            set
            {
                TrueName = value;
            }
        }

        [Category("Material Data"), ReadOnlyAttribute(true)]
        public string FileType
        {

            get
            {
                return FileExt;
            }
            set
            {
                FileExt = value;
            }
        }

        [Category("Material Data"), ReadOnlyAttribute(true)]
        public int FileLength
        {

            get
            {
                return DSize;
            }
            set
            {
                DSize = value;
            }
        }

        [Category("Material Data"), ReadOnlyAttribute(true)]
        public int TextureTotal
        {
            get
            {
                return TextureCount;
            }
            set
            {
                TextureCount = value;
            }
        }

        [Category("Material Data"), ReadOnlyAttribute(true)]
        public int MaterialTotal
        {
            get
            {
                return MaterialCount;
            }
            set
            {
                MaterialCount = value;
            }
        }

        [Category("Material Data"), ReadOnlyAttribute(true)]
        public long TextureStartingOffset
        {
            get
            {
                return TextureOffset;
            }
            set
            {
                TextureOffset = value;
            }
        }

        [Category("Material Data"), ReadOnlyAttribute(true)]
        public long MaterialStartingOffset
        {
            get
            {
                return MaterialOffset;
            }
            set
            {
                MaterialOffset = value;
            }
        }

        #endregion

    }
}

