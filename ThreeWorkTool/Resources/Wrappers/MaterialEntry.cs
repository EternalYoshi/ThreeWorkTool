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
using static ThreeWorkTool.Resources.Wrappers.MaterialMaterialEntry;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class MaterialEntry : DefaultWrapper
    {
        public const int SIZE = 0x28;
        public const int MATSIZE = 0x48;
        public const int MAX_NAME_LENGTH = 64;
        public string Magic;
        public string Constant;
        public int EntryCount;
        public byte[] WTemp;
        public int SomethingCount;
        public int TextureCount;
        public int MaterialCount;
        public int TextureOffset;
        public int MaterialOffset;
        public int UnknownField;
        public string WeirdHash;
        public int Field14;
        public List<MaterialTextureReference> Textures;
        public List<MaterialMaterialEntry> Materials;

        public static MaterialEntry FillMatEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
        {
            var MATEntry = new MaterialEntry();
            List<byte> BTemp = new List<byte>();

            FillEntry(filename,subnames,tree,br,c,ID,MATEntry);
            MATEntry._FileType = MATEntry.FileExt;
            MATEntry._FileName = MATEntry.TrueName;

            //Decompression Time.
            MATEntry.UncompressedData = ZlibStream.UncompressBuffer(MATEntry.CompressedData);
            MATEntry._FileLength = MATEntry.UncompressedData.Length;

            //Material specific work here.
            using (MemoryStream MatStream = new MemoryStream(MATEntry.UncompressedData))
            {
                using (BinaryReader MBR = new BinaryReader(MatStream))
                {
                    //Header variables.
                    MATEntry.Magic = ByteUtilitarian.BytesToStringL2R(MBR.ReadBytes(4).ToList(),MATEntry.Magic);
                    MATEntry.SomethingCount = MBR.ReadInt32();
                    MATEntry.MaterialCount = MBR.ReadInt32();
                    MATEntry.TextureCount = MBR.ReadInt32();
                    MATEntry.WeirdHash = ByteUtilitarian.BytesToStringL2R(MBR.ReadBytes(4).ToList(), MATEntry.Magic);
                    MATEntry.Field14 = MBR.ReadInt32();
                    MATEntry.TextureOffset = MBR.ReadInt32();
                    MBR.BaseStream.Position = MBR.BaseStream.Position + 4;
                    MATEntry.MaterialOffset = MBR.ReadInt32();
                    MBR.BaseStream.Position = MBR.BaseStream.Position + 4;

                    //For the Texture References.
                    MATEntry.Textures = new List<MaterialTextureReference>();
                    MBR.BaseStream.Position = MATEntry.TextureOffset;
                    for (int i = 0; i < MATEntry.TextureCount; i++)
                    {
                        MaterialTextureReference TexTemp = new MaterialTextureReference();
                        TexTemp = TexTemp.FillMaterialTexReference(MATEntry,i,MBR,TexTemp);
                        MATEntry.Textures.Add(TexTemp);
                    }

                    //Now for the Materials themselves.
                    
                    MATEntry.Materials = new List<MaterialMaterialEntry>();
                    byte[] ShadeTemp = new byte[4];
                    int PrevOffset = Convert.ToInt32(MBR.BaseStream.Position);
                    //uint ShadeUInt;
                    //byte[] NameHashBytes;
                    //uint NameTemp;

                    //Part 1 of Materials.
                    for (int i = 0; i < MATEntry.MaterialCount; i++)
                    {
                        MaterialMaterialEntry MMEntry = new MaterialMaterialEntry();
                        MMEntry = MMEntry.FIllMatMatEntryPropertiesPart1(MMEntry, MATEntry,MBR, MatStream, PrevOffset,i);
                        MATEntry.Materials.Add(MMEntry);

                        /*
                        MaterialMaterialEntry MMEntry = new MaterialMaterialEntry();
                        MMEntry.TypeHash = ByteUtilitarian.BytesToStringL2R(MBR.ReadBytes(4).ToList(), MMEntry.TypeHash);
                        MMEntry.UnknownField04 = MBR.ReadInt32();
                        NameHashBytes = MBR.ReadBytes(4);
                        MMEntry.NameHash = ByteUtilitarian.BytesToStringL2R(NameHashBytes.ToList(), MMEntry.TypeHash);

                        //Name.
                        NameTemp = BitConverter.ToUInt32(NameHashBytes,0);

                        //ShaderObjects.
                        MMEntry.BlendState = new MatShaderObject();
                        MMEntry.DepthStencilState = new MatShaderObject();
                        MMEntry.RasterizerState = new MatShaderObject();
                        MMEntry.CmdBufferSize = MBR.ReadInt32();
                        ShadeTemp = MBR.ReadBytes(4);
                        ShadeUInt = BitConverter.ToUInt32(ShadeTemp, 0);
                        MMEntry.BlendState.Index = Convert.ToInt32(ShadeUInt & 0x00000FFF);
                        MMEntry.BlendState.Hash = "";

                        //Getting The Hash.
                        string line = "";
                        try
                        {
                            line = File.ReadLines("mvc3shadertypes.cfg").Skip(MMEntry.BlendState.Index).Take(1).First();
                        }
                        catch (Exception xx)
                        {
                            MessageBox.Show("mvc3shadertypes.cfg is missing or not read. Can't continue parsing materials.\n" + xx, "Uh-Oh");
                            return null;
                        }
                        MMEntry.BlendState.Hash = line;

                        ShadeTemp = MBR.ReadBytes(4);
                        ShadeUInt = BitConverter.ToUInt32(ShadeTemp, 0);
                        MMEntry.DepthStencilState.Index = Convert.ToInt32(ShadeUInt & 0x00000FFF);
                        MMEntry.DepthStencilState.Hash = "";

                        //Getting The Hash.
                        try
                        {
                            line = File.ReadLines("mvc3shadertypes.cfg").Skip(MMEntry.DepthStencilState.Index).Take(1).First();
                        }
                        catch (Exception xx)
                        {
                            MessageBox.Show("mvc3shadertypes.cfg is missing or not read. Can't continue parsing materials.\n" + xx, "Uh-Oh");
                            return null;
                        }
                        MMEntry.DepthStencilState.Hash = line;

                        ShadeTemp = MBR.ReadBytes(4);
                        ShadeUInt = BitConverter.ToUInt32(ShadeTemp, 0);
                        MMEntry.RasterizerState.Index = Convert.ToInt32(ShadeUInt & 0x00000FFF);
                        MMEntry.RasterizerState.Hash = "";

                        //Getting The Hash.
                        try
                        {
                            line = File.ReadLines("mvc3shadertypes.cfg").Skip(MMEntry.RasterizerState.Index).Take(1).First();
                        }
                        catch (Exception xx)
                        {
                            MessageBox.Show("mvc3shadertypes.cfg is missing or not read. Can't continue parsing materials.\n" + xx, "Uh-Oh");
                            return null;
                        }
                        MMEntry.RasterizerState.Hash = line;
                        MMEntry.MaterialCommandListInfo = new MaterialCmdListInfo();

                        //The Material Command List Info.
                        ShadeTemp = MBR.ReadBytes(4);
                        ShadeUInt = BitConverter.ToUInt32(ShadeTemp, 0);
                        MMEntry.MaterialCommandListInfo.Count = Convert.ToInt32(ShadeUInt & 0xFFF);
                        MMEntry.MaterialCommandListInfo.Unknown = Convert.ToInt32(ShadeUInt & 0xFFFF000);
                        MMEntry.MaterialinfoFlags = ByteUtilitarian.BytesToStringL2R(MBR.ReadBytes(4).ToList(), MMEntry.MaterialinfoFlags);
                        MMEntry.UnknownField24 = MBR.ReadInt32();
                        MMEntry.UnknownField28 = MBR.ReadInt32();
                        MMEntry.UnknownField2C = MBR.ReadInt32();
                        MMEntry.UnknownField30 = MBR.ReadInt32();
                        MMEntry.AnimDataSize = MBR.ReadInt32();
                        MMEntry.CmdListOffset = Convert.ToInt32(MBR.ReadInt64());
                        MMEntry.AnimDataOffset = Convert.ToInt32(MBR.ReadInt64());
                        MMEntry.SomethingLabeledP = Convert.ToInt32(MBR.BaseStream.Position);

                        #region Commands
                        //Commands.
                        MBR.BaseStream.Position = MMEntry.CmdListOffset;
                        MMEntry.MaterialCommands = new List<MatCmd>();
                        byte[] InfoTemp = new byte[4];
                        uint UInfoTemp;
                        int Uniontemp;
                        for (int r = 0;r < MMEntry.MaterialCommandListInfo.Count; r++)
                        {
                            MatCmd cmd = new MatCmd();
                            //Command Info.
                            cmd.MCInfo = new MatCmdInfo();
                            InfoTemp = MBR.ReadBytes(4);
                            UInfoTemp = BitConverter.ToUInt32(InfoTemp, 0);
                            cmd.MCInfo.SomeValue = Convert.ToInt32(UInfoTemp & 0x0000000F);
                            cmd.MCInfo.SetFlag = Convert.ToInt32(UInfoTemp & 0x000FFFF0);
                            cmd.MCInfo.ShaderObjectIndex = Convert.ToInt32(UInfoTemp >> 20);

                            cmd.SomeField04 = MBR.ReadInt32();
                            InfoTemp = MBR.ReadBytes(4);
                            UInfoTemp = BitConverter.ToUInt32(InfoTemp, 0);
                            Uniontemp = BitConverter.ToInt32(InfoTemp, 0);

                            //Yet another Shader Object ID inside the Value Union.
                            cmd.MaterialCommandValue = new Value();
                            cmd.MaterialCommandValue.ConstantBufferDataOffset = Uniontemp;
                            cmd.MaterialCommandValue.TextureIndex = cmd.MaterialCommandValue.ConstantBufferDataOffset;
                            cmd.MaterialCommandValue.VShaderObjectID = new MatShaderObject();
                            cmd.MaterialCommandValue.VShaderObjectID.Index = Convert.ToInt32(UInfoTemp & 0x00000FFF);
                            cmd.MaterialCommandValue.VShaderObjectID.Hash = "";

                            //Getting The Hash.
                            line = "";
                            try
                            {
                                line = File.ReadLines("mvc3shadertypes.cfg").Skip(cmd.MaterialCommandValue.VShaderObjectID.Index).Take(1).First();
                            }
                            catch (Exception xx)
                            {
                                MessageBox.Show("mvc3shadertypes.cfg is missing or not read. Can't continue parsing materials.\n" + xx, "Uh-Oh");
                                return null;
                            }
                            cmd.MaterialCommandValue.VShaderObjectID.Hash = line;
                            MBR.BaseStream.Position = MBR.BaseStream.Position + 4;
                            //Again, but for the Shader Object OUTISDE the Union.
                            InfoTemp = MBR.ReadBytes(4);
                            UInfoTemp = BitConverter.ToUInt32(InfoTemp, 0);
                            cmd.CmdShaderObject = new MatShaderObject();
                            cmd.CmdShaderObject.Index = Convert.ToInt32(UInfoTemp & 0x00000FFF);
                            //Getting The Hash.
                            line = "";
                            try
                            {
                                line = File.ReadLines("mvc3shadertypes.cfg").Skip(cmd.CmdShaderObject.Index).Take(1).First();
                            }
                            catch (Exception xx)
                            {
                                MessageBox.Show("mvc3shadertypes.cfg is missing or not read. Can't continue parsing materials.\n" + xx, "Uh-Oh");
                                return null;
                            }
                            cmd.CmdShaderObject.Hash = line;
                            cmd.SomeField14 = MBR.ReadInt32();

                            //....Gotta do something about the NameHash. Need to reference the model somehow...

                            MMEntry.MaterialCommands.Add(cmd);

                        }

                        #endregion

                        //Gotta have SOMETHING here to get the ConstantBufferData.... so how would I associate the floating points to the proper command?


                        MMEntry = MMEntry.FIllProperties(MMEntry);

                        MATEntry.Materials.Add(MMEntry);
                        MBR.BaseStream.Position = MMEntry.SomethingLabeledP;
                        */
                    }



                }
            }


            return MATEntry;

        }

        /*
        public static MaterialEntry ReplaceMat(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            MaterialEntry material = new MaterialEntry();
            MaterialEntry oldentry = new MaterialEntry();

            tree.BeginUpdate();

            try
            {
                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
                {
                    //We build the MaterialEntry starting from the uncompressed data.
                    material.UncompressedData = System.IO.File.ReadAllBytes(filename);

                    //Then Compress.
                    material.CompressedData = Zlibber.Compressor(material.UncompressedData);

                    //Gets the filename of the file to inject without the directory.
                    string trname = filename;
                    while (trname.Contains("\\"))
                    {
                        trname = trname.Substring(trname.IndexOf("\\") + 1);
                    }

                    //Reads and inserts Material data. To be expanded upon later.
                    List<byte> BTemp = new List<byte>();
                    string Tempname;

                    material.WTemp = new byte[5];
                    Array.Copy(material.UncompressedData, 0, material.WTemp, 0, 5);
                    material.Magic = ByteUtilitarian.BytesToString(material.WTemp, material.Magic);

                    byte[] MTemp = new byte[4];
                    Array.Copy(material.UncompressedData, 8, MTemp, 0, 4);
                    material.MaterialCount = BitConverter.ToInt32(MTemp, 0);
                    material._MaterialTotal = material.MaterialCount;

                    Array.Copy(material.UncompressedData, 12, MTemp, 0, 4);
                    material.TextureCount = BitConverter.ToInt32(MTemp, 0);
                    material._TextureTotal = material.TextureCount;

                    Array.Copy(material.UncompressedData, 16, MTemp, 0, 4);
                    material.WeirdHash = ByteUtilitarian.BytesToString(MTemp, material.WeirdHash);

                    byte[] SixFourTemp = new byte[8];
                    Array.Copy(material.UncompressedData, 24, SixFourTemp, 0, 8);
                    material.TextureOffset = BitConverter.ToInt64(SixFourTemp, 0);
                    material._TextureStartingOffset = Convert.ToInt32(material.TextureOffset);

                    Array.Copy(material.UncompressedData, 32, SixFourTemp, 0, 8);
                    material.MaterialOffset = BitConverter.ToInt64(SixFourTemp, 0);
                    material._MaterialStartingOffset = Convert.ToInt32(material.MaterialOffset);

                    material.TexEntries = new List<MaterialTextureReference>();

                    int j = Convert.ToInt32(material.TextureOffset);
                    byte[] MENTemp = new byte[64];
                    //Fills in(or at least tries to) fill in each Texture and Material entry.
                    for (int i = 0; i < material.TextureCount; i++)
                    {
                        j = (Convert.ToInt32(material.TextureOffset) + i * 88);
                        MaterialTextureReference TexTemp = new MaterialTextureReference();
                        Array.Copy(material.UncompressedData, j, MTemp, 0, 4);
                        TexTemp.TypeHash = ByteUtilitarian.BytesToString(MTemp, TexTemp.TypeHash);
                        j = j + 24;
                        BTemp.Clear();
                        Array.Copy(material.UncompressedData, j, MENTemp, 0, 64);
                        BTemp.AddRange(MENTemp);
                        BTemp.RemoveAll(ByteUtilitarian.IsZeroByte);
                        ASCIIEncoding asciime = new ASCIIEncoding();
                        Tempname = asciime.GetString(BTemp.ToArray());
                        TexTemp.FullTexName = Tempname;
                        TexTemp.Index = i;
                        TexTemp._Index = TexTemp.Index;
                        material.TexEntries.Add(TexTemp);
                    }

                    //Enters name related parameters of the material.
                    material.TrueName = trname;
                    material._FileName = material.TrueName;
                    material.TrueName = Path.GetFileNameWithoutExtension(trname);
                    material.FileExt = trname.Substring(trname.LastIndexOf("."));
                    material._FileType = material.FileExt;

                    var tag = node.Tag;
                    if (tag is MaterialEntry)
                    {
                        oldentry = tag as MaterialEntry;
                    }
                    string path = "";
                    int index = oldentry.EntryName.LastIndexOf("\\");
                    if (index > 0)
                    {
                        path = oldentry.EntryName.Substring(0, index);
                    }

                    material.EntryName = path + "\\" + material.TrueName;

                    tag = material;

                    if (node.Tag is MaterialEntry)
                    {
                        node.Tag = material;
                        node.Name = Path.GetFileNameWithoutExtension(material.EntryName);
                        node.Text = Path.GetFileNameWithoutExtension(material.EntryName);

                    }

                    var aew = node as ArcEntryWrapper;

                    string type = node.GetType().ToString();
                    if (type == "ThreeWorkTool.Resources.Wrappers.ArcEntryWrapper")
                    {
                        aew.entryfile = material;
                    }

                    node = aew;
                    node.entryfile = material;
                    tree.EndUpdate();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Read error. Is the file readable?");
                using (StreamWriter sw = File.AppendText("Log.txt"))
                {
                    sw.WriteLine("Read Error! Here's the exception info:\n" + ex);
                }
            }



            return node.entryfile as MaterialEntry;
        }
        */

        /*
        public static MaterialEntry InsertEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            MaterialEntry matentry = new MaterialEntry();

            try
            {
                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
                {
                    //We build the arcentry starting from the uncompressed data.
                    matentry.UncompressedData = System.IO.File.ReadAllBytes(filename);
                    matentry.DSize = matentry.UncompressedData.Length;

                    //Then Compress.
                    matentry.CompressedData = Zlibber.Compressor(matentry.UncompressedData);
                    matentry.CSize = matentry.CompressedData.Length;

                    //Reads and inserts Material data. To be expanded upon later.
                    List<byte> BTemp = new List<byte>();
                    string Tempname;

                    matentry.WTemp = new byte[5];
                    Array.Copy(matentry.UncompressedData, 0, matentry.WTemp, 0, 5);
                    matentry.Magic = ByteUtilitarian.BytesToString(matentry.WTemp, matentry.Magic);

                    byte[] MTemp = new byte[4];
                    Array.Copy(matentry.UncompressedData, 8, MTemp, 0, 4);
                    matentry.MaterialCount = BitConverter.ToInt32(MTemp, 0);
                    matentry._MaterialTotal = matentry.MaterialCount;

                    Array.Copy(matentry.UncompressedData, 12, MTemp, 0, 4);
                    matentry.TextureCount = BitConverter.ToInt32(MTemp, 0);
                    matentry._TextureTotal = matentry.TextureCount;

                    Array.Copy(matentry.UncompressedData, 16, MTemp, 0, 4);
                    matentry.WeirdHash = ByteUtilitarian.BytesToString(MTemp, matentry.WeirdHash);

                    byte[] SixFourTemp = new byte[8];
                    Array.Copy(matentry.UncompressedData, 24, SixFourTemp, 0, 8);
                    matentry.TextureOffset = BitConverter.ToInt64(SixFourTemp, 0);
                    matentry._TextureStartingOffset = Convert.ToInt32(matentry.TextureOffset);

                    Array.Copy(matentry.UncompressedData, 32, SixFourTemp, 0, 8);
                    matentry.MaterialOffset = BitConverter.ToInt64(SixFourTemp, 0);
                    matentry._MaterialStartingOffset = Convert.ToInt32(matentry.MaterialOffset);

                    matentry.TexEntries = new List<MaterialTextureReference>();

                    int j = Convert.ToInt32(matentry.TextureOffset);
                    byte[] MENTemp = new byte[64];
                    //Fills in(or at least tries to) fill in each Texture and Material entry.
                    for (int i = 0; i < matentry.TextureCount; i++)
                    {
                        j = (Convert.ToInt32(matentry.TextureOffset) + i * 88);
                        MaterialTextureReference TexTemp = new MaterialTextureReference();
                        Array.Copy(matentry.UncompressedData, j, MTemp, 0, 4);
                        TexTemp.TypeHash = ByteUtilitarian.BytesToString(MTemp, TexTemp.TypeHash);
                        j = j + 24;
                        BTemp.Clear();
                        Array.Copy(matentry.UncompressedData, j, MENTemp, 0, 64);
                        BTemp.AddRange(MENTemp);
                        BTemp.RemoveAll(ByteUtilitarian.IsZeroByte);
                        ASCIIEncoding asciime = new ASCIIEncoding();
                        Tempname = asciime.GetString(BTemp.ToArray());
                        TexTemp.FullTexName = Tempname;
                        TexTemp.Index = i;
                        TexTemp._Index = TexTemp.Index;
                        matentry.TexEntries.Add(TexTemp);
                    }

                    //Gets the filename of the file to inject without the directory.
                    string trname = filename;
                    while (trname.Contains("\\"))
                    {
                        trname = trname.Substring(trname.IndexOf("\\") + 1);
                    }

                    matentry.TrueName = trname;
                    matentry._FileName = matentry.TrueName;
                    matentry.TrueName = Path.GetFileNameWithoutExtension(trname);
                    matentry.FileExt = trname.Substring(trname.LastIndexOf("."));
                    matentry._FileType = matentry.FileExt;

                    //Gets the path of the selected node to inject here.
                    string nodepath = tree.SelectedNode.FullPath;
                    nodepath = nodepath.Substring(nodepath.IndexOf("\\") + 1);

                    string[] sepstr = { "\\" };
                    matentry.EntryDirs = nodepath.Split(sepstr, StringSplitOptions.RemoveEmptyEntries);
                    matentry.EntryName = matentry.FileName;

                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText("Log.txt"))
                {
                    sw.WriteLine("Caught an exception using the BinaryReader. Here's the details:\n" + ex);
                }
            }



            return matentry;
        }
        */

        #region Material Properties

        private string _FileName;
        [Category("Material Data"), ReadOnlyAttribute(true)]
        public string FileName
        {

            get
            {
                return _FileName;
            }
            set
            {
                _FileName = value;
            }
        }

        private string _FileType;
        [Category("Material Data"), ReadOnlyAttribute(true)]
        public string FileType
        {

            get
            {
                return _FileType;
            }
            set
            {
                _FileType = value;
            }
        }

        private long _FileLength;
        [Category("Material Data"), ReadOnlyAttribute(true)]
        public long FileLength
        {

            get
            {
                return _FileLength;
            }
            set
            {
                _FileLength = value;
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
        public int TextureStartingOffset
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
        public int MaterialStartingOffset
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
