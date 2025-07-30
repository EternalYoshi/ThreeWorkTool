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
            List<int> AnimIndexCounter = new List<int>();
            List<string> AnimDataStr = new List<string>();
            List<byte> AnimDataRaw = new List<byte>();

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
                    if(Mitem.First().Value.animData != null)
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

                //Texture Names part.
                foreach(string texname in DistinctTextureNames)
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

                //Materials Part.
                for (int i = 0; i < newmatA.materials.Count; ++i)
                {
                    


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

//using Ionic.Zlib;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Numerics;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Windows.Forms;
//using ThreeWorkTool.Resources.Archives;
//using ThreeWorkTool.Resources.Utility;
//using YamlDotNet.Core;
//using YamlDotNet.Serialization;
//using YamlDotNet.Serialization.NamingConventions;
//using ThreeWorkTool.Resources.Wrappers.ExtraNodes;
//using static ThreeWorkTool.Resources.Wrappers.MaterialMaterialEntry;

//namespace ThreeWorkTool.Resources.Wrappers
//{
//    public class MaterialEntry : DefaultWrapper
//    {
//        public const int SIZE = 0x28;
//        public const int MATSIZE = 0x48;
//        public const int MAX_NAME_LENGTH = 64;
//        public const int TEXENTRYSIZE = 0x58;
//        public string Magic;
//        public string Constant;
//        public int EntryCount;
//        public byte[] WTemp;
//        public int SomethingCount;
//        public int TextureCount;
//        public int MaterialCount;
//        public long TextureOffset;
//        public long MaterialOffset;
//        public int UnknownField;
//        public string WeirdHash;
//        public int Field14;
//        public List<MaterialTextureReference> Textures;
//        public List<MaterialMaterialEntry> Materials;
//        public List<MaterialAnimEntry> MatAnims;
//        public string YMLText;
//        public MTMaterial YMLMat { get; set; }

//        public static MaterialEntry FillMatEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
//        {
//            var MATEntry = new MaterialEntry();
//            List<byte> BTemp = new List<byte>();

//            FillEntry(filename, subnames, tree, br, c, ID, MATEntry);

//            //Decompression Time.
//            MATEntry.UncompressedData = ZlibStream.UncompressBuffer(MATEntry.CompressedData);

//            //Material specific work here.
//            using (MemoryStream MatStream = new MemoryStream(MATEntry.UncompressedData))
//            {
//                using (BinaryReader MBR = new BinaryReader(MatStream))
//                {
//                    BuildMatEntry(MBR, MATEntry);
//                }
//            }

//            return MATEntry;

//        }

//        public static MaterialEntry ReplaceMat(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
//        {

//            MaterialEntry matentry = new MaterialEntry();
//            MaterialEntry oldentry = new MaterialEntry();

//            tree.BeginUpdate();

//            ReplaceEntry(tree, node, filename, matentry, oldentry);
//            matentry.FileName = matentry.TrueName;
//            //Type Specific Work Here.
//            using (MemoryStream LmtStream = new MemoryStream(matentry.UncompressedData))
//            {
//                using (BinaryReader bnr = new BinaryReader(LmtStream))
//                {
//                    BuildMatEntry(bnr, matentry);
//                }
//            }

//            return node.entryfile as MaterialEntry;

//        }

//        public static MaterialEntry InsertEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
//        {
//            MaterialEntry matentry = new MaterialEntry();

//            try
//            {
//                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
//                {
//                    InsertKnownEntry(tree, node, filename, matentry, bnr);
//                }
//            }
//            catch (Exception ex)
//            {
//                string ProperPath = "";
//                ProperPath = Globals.ToolPath + "Log.txt"; using (StreamWriter sw = File.AppendText(ProperPath))
//                {
//                    sw.WriteLine("Caught an exception using the BinaryReader. Here's the details:\n" + ex);
//                }
//            }

//            //Decompression Time.
//            matentry.UncompressedData = ZlibStream.UncompressBuffer(matentry.CompressedData);

//            try
//            {
//                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
//                {
//                    //Material specific work here.
//                    using (MemoryStream MatStream = new MemoryStream(matentry.UncompressedData))
//                    {
//                        using (BinaryReader MBR = new BinaryReader(MatStream))
//                        {
//                            BuildMatEntry(MBR, matentry);
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                string ProperPath = "";
//                ProperPath = Globals.ToolPath + "Log.txt"; using (StreamWriter sw = File.AppendText(ProperPath))
//                {
//                    sw.WriteLine("Caught an exception using the BinaryReader. Here's the details:\n" + ex);
//                }
//            }

//            return matentry;
//        }

//        public static MaterialEntry BuildMatEntry(BinaryReader MBR, MaterialEntry MATEntry)
//        {

//            //Experimental.

//            //Header variables.
//            MATEntry.Magic = ByteUtilitarian.BytesToStringL2R(MBR.ReadBytes(4).ToList(), MATEntry.Magic);
//            MATEntry.SomethingCount = MBR.ReadInt32();
//            MATEntry.MaterialCount = MBR.ReadInt32();
//            MATEntry.TextureCount = MBR.ReadInt32();
//            MATEntry.WeirdHash = ByteUtilitarian.BytesToStringL2R(MBR.ReadBytes(4).ToList(), MATEntry.Magic);
//            MATEntry.Field14 = MBR.ReadInt32();
//            MATEntry.TextureOffset = MBR.ReadInt64();
//            MATEntry.MaterialOffset = MBR.ReadInt64();

//            //For the Texture References.
//            MATEntry.Textures = new List<MaterialTextureReference>();
//            MBR.BaseStream.Position = MATEntry.TextureOffset;
//            for (int i = 0; i < MATEntry.TextureCount; i++)
//            {
//                MaterialTextureReference TexTemp = new MaterialTextureReference();
//                TexTemp = TexTemp.FillMaterialTexReference(MATEntry, i, MBR, TexTemp);
//                MATEntry.Textures.Add(TexTemp);
//            }

//            //Now for the Materials themselves.
//            MATEntry.Materials = new List<MaterialMaterialEntry>();
//            byte[] ShadeTemp = new byte[4];
//            int PrevOffset = Convert.ToInt32(MBR.BaseStream.Position);

//            //Material Anims.
//            MATEntry.MatAnims = new List<MaterialAnimEntry>();

//            //Materials.
//            for (int i = 0; i < MATEntry.MaterialCount; i++)
//            {

//                MaterialMaterialEntry MMEntry = new MaterialMaterialEntry();
//                MMEntry = MMEntry.FIllMatMatEntryPropertiesPart1(MMEntry, MATEntry, MBR, PrevOffset, i);
//                MMEntry = MMEntry.FIllMatMatEntryPropertiesPart2(MMEntry, MATEntry, MBR, PrevOffset, i);

//                //Material Animation Work.

//                if (MMEntry.AnimDataSize > 0 && MMEntry.AnimDataOffset > 0)
//                {

//                    MBR.BaseStream.Position = MMEntry.AnimDataOffset;

//                    MaterialAnimEntry Manim = new MaterialAnimEntry();
//                    Manim.RawData = MBR.ReadBytes(MMEntry.AnimDataSize);
//                    Manim.MaterialIndex = i;
//                    Manim.IsNew = false;


//                    MATEntry.MatAnims.Add(Manim);
//                }

//                //--------------------------------------

//                MATEntry.Materials.Add(MMEntry);
//                PrevOffset = PrevOffset + 72;
//                MBR.BaseStream.Position = PrevOffset;

//            }

//            MATEntry = BuildYML(MATEntry, MATEntry.YMLText);

//            return MATEntry;

//        }

//        //This function based off code TGE Wrote for Material support in the importer.
//        public static MaterialEntry BuildYML(MaterialEntry MATEntry, string YML)
//        {

//            YML = "";
//            YML = YML + "version: 1\n";
//            YML = YML + "materials:\n";

//            //Materials.
//            for (int y = 0; y < MATEntry.Materials.Count; y++)
//            {
//                if (MATEntry.Materials[y].MatName == "" || MATEntry.Materials[y].MatName == null)
//                {
//                    YML = YML + "    - _0x" + MATEntry.Materials[y].NameHash + ":\n";
//                }
//                else
//                {
//                    YML = YML + "    - " + MATEntry.Materials[y].MatName + ":\n";
//                }
//                YML = YML + "        type: " + MATEntry.Materials[y].MatType + "\n";
//                YML = YML + "        blendState: " + MATEntry.Materials[y].BlendStateType + "\n";
//                YML = YML + "        depthStencilState: " + MATEntry.Materials[y].DepthStencilStateType + "\n";
//                YML = YML + "        rasterizerState: " + MATEntry.Materials[y].RasterizerStateType + "\n";
//                YML = YML + "        cmdListFlags: 0x" + MATEntry.Materials[y].cmdListFlags.ToString("X") + "\n";
//                YML = YML + "        matFlags: 0x" + MATEntry.Materials[y].matFlags + "\n";
//                YML = YML + "        cmds:\n";


//                //Commands.
//                for (int z = 0; z < MATEntry.Materials[y].MaterialCommands.Count; z++)
//                {
//                    if (MATEntry.Materials[y].MaterialCommands[z].MCInfo.CmdFlag == "cbuffer")
//                    {
//                        if (MATEntry.Materials[y].MaterialCommands[z].RawFloats.Count <= 1)
//                        {
//                            YML = YML + "            - [ " + MATEntry.Materials[y].MaterialCommands[z].MCInfo.CmdFlag + ", " + MATEntry.Materials[y].MaterialCommands[z].MaterialCommandData.VShaderObjectID.Hash + ", " + MATEntry.Materials[y].MaterialCommands[z].FloatStr + " ]\n";
//                        }
//                        else
//                        {
//                            YML = YML + "            - [ " + MATEntry.Materials[y].MaterialCommands[z].MCInfo.CmdFlag + ", " + MATEntry.Materials[y].MaterialCommands[z].MaterialCommandData.VShaderObjectID.Hash + ", " + MATEntry.Materials[y].MaterialCommands[z].FloatStr + "]\n";
//                        }
//                    }
//                    else if (MATEntry.Materials[y].MaterialCommands[z].MCInfo.CmdFlag == "flag" || MATEntry.Materials[y].MaterialCommands[z].MCInfo.CmdFlag == "samplerstate")
//                    {
//                        YML = YML + "            - [ " + MATEntry.Materials[y].MaterialCommands[z].MCInfo.CmdFlag + ", " + MATEntry.Materials[y].MaterialCommands[z].MaterialCommandData.VShaderObjectID.Hash + ", " + MATEntry.Materials[y].MaterialCommands[z].CmdName + " ]\n";

//                    }
//                    else if (MATEntry.Materials[y].MaterialCommands[z].MCInfo.CmdFlag == "texture")
//                    {
//                        YML = YML + "            - [ " + MATEntry.Materials[y].MaterialCommands[z].MCInfo.CmdFlag + ", " + MATEntry.Materials[y].MaterialCommands[z].MaterialCommandData.VShaderObjectID.Hash + ", " + MATEntry.Materials[y].MaterialCommands[z].DataStr + " ]\n";

//                    }
//                    //YML = YML + " ";

//                }

//            }

//            MATEntry.YMLText = YML;

//            return MATEntry;
//        }

//        //This function also based off code TGE wrote for Material support in the importer.
//        public static MaterialEntry ReplaceYMLToMRL(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
//        {

//            MaterialEntry matentry = new MaterialEntry();
//            MaterialEntry oldentry = new MaterialEntry();
//            string strtemp = "";

//            //Time to check the yml file.
//            var deserializer = new YamlDotNet.Serialization.DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
//            strtemp = File.ReadAllText(filename);
//            var p = deserializer.Deserialize<MTMaterial>(strtemp);

//            //var myConfig = deserializer.Deserialize<Configuration>(File.ReadAllText(filename));


//            using (StreamReader sr = new StreamReader(filename))
//            {
//                strtemp = sr.ReadLine();

//                //Checks the yml version.
//                if (strtemp == "version: 1")
//                {

//                }
//                else
//                {
//                    MessageBox.Show("The selected yml file is not a supported material library version. Apologies.");
//                    return null;
//                }

//            }

//            /*
//            tree.BeginUpdate();

//            ReplaceEntry(tree, node, filename, matentry, oldentry);

//            //Type Specific Work Here.
//            using (MemoryStream LmtStream = new MemoryStream(matentry.UncompressedData))
//            {
//                using (BinaryReader bnr = new BinaryReader(LmtStream))
//                {
//                    BuildMatEntry(bnr, matentry);
//                }
//            }
//            */


//            return node.entryfile as MaterialEntry;

//        }

//        #region Material Properties

//        [Category("Material Data"), ReadOnlyAttribute(true)]
//        public string FileName
//        {

//            get
//            {
//                return TrueName;
//            }
//            set
//            {
//                TrueName = value;
//            }
//        }

//        [Category("Material Data"), ReadOnlyAttribute(true)]
//        public string FileType
//        {

//            get
//            {
//                return FileExt;
//            }
//            set
//            {
//                FileExt = value;
//            }
//        }

//        [Category("Material Data"), ReadOnlyAttribute(true)]
//        public int FileLength
//        {

//            get
//            {
//                return DSize;
//            }
//            set
//            {
//                DSize = value;
//            }
//        }

//        [Category("Material Data"), ReadOnlyAttribute(true)]
//        public int TextureTotal
//        {
//            get
//            {
//                return TextureCount;
//            }
//            set
//            {
//                TextureCount = value;
//            }
//        }

//        [Category("Material Data"), ReadOnlyAttribute(true)]
//        public int MaterialTotal
//        {
//            get
//            {
//                return MaterialCount;
//            }
//            set
//            {
//                MaterialCount = value;
//            }
//        }

//        [Category("Material Data"), ReadOnlyAttribute(true)]
//        public long TextureStartingOffset
//        {
//            get
//            {
//                return TextureOffset;
//            }
//            set
//            {
//                TextureOffset = value;
//            }
//        }

//        [Category("Material Data"), ReadOnlyAttribute(true)]
//        public long MaterialStartingOffset
//        {
//            get
//            {
//                return MaterialOffset;
//            }
//            set
//            {
//                MaterialOffset = value;
//            }
//        }

//        #endregion

//    }
//}
