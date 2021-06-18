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
using ThreeWorkTool.Resources.Utility;
using static ThreeWorkTool.Resources.Wrappers.MaterialMaterialEntry;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class MaterialEntry
    {
        public string Magic;
        public string Constant;
        public int CSize;
        public int DSize;
        public int EntryCount;
        public int OffsetTemp;
        public string EntryName;
        public int AOffset;
        public int EntryID;
        public byte[] WTemp;
        public byte[] CompressedData;
        public byte[] UncompressedData;
        public string[] EntryDirs;
        public string TrueName;
        public string FileExt;
        public static StringBuilder SBname;
        public int TextureCount;
        public int MaterialCount;
        public Int64 TextureOffset;
        public Int64 MaterialOffset;
        public int UnknownField;
        public string WeirdHash;
        public static string TypeHash = "2749C8A8";
        public List<MaterialTextureReference> TexEntries;
        public int CommandBufferIndex;

        //Well then.... gotta construct these classes before I put in the code that fills that data in the FillMatEntry function.

        public struct MaterialEntries
        {
            public string MatName;
            public string UnknownField04;
            public string TypeHash;
            public string UnknownField;
            public string NameHash;
            public int CmdBufferSize;
            public string MateialinfoFlags;
            public int UnknownField24;
            public int UnknownField28;
            public int UnknownField2C;
            public int UnknownField30;
            public int AnimDataSize;
            public int CmdListOffset;
            public int AnimDataOffset;
            public int SomethingLabeledP;
        }

        public struct MatShaderObject
        {
            public int Index;
            public int Hash;
        }

        public struct ConstantBufferData
        {
            public byte[] BufferRawData;
        }

        public static MaterialEntry FillMatEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
        {
            MaterialEntry MATEntry = new MaterialEntry();

            //This block gets the name of the entry.
            MATEntry.OffsetTemp = c;
            MATEntry.EntryID = ID;
            List<byte> BTemp = new List<byte>();
            br.BaseStream.Position = MATEntry.OffsetTemp;
            BTemp.AddRange(br.ReadBytes(64));
            BTemp.RemoveAll(ByteUtilitarian.IsZeroByte);

            if (SBname == null)
            {
                SBname = new StringBuilder();
            }
            else
            {
                SBname.Clear();
            }

            string Tempname;
            ASCIIEncoding ascii = new ASCIIEncoding();
            Tempname = ascii.GetString(BTemp.ToArray());

            //Compressed Data size.
            BTemp = new List<byte>();
            c = c + 68;
            br.BaseStream.Position = c;
            BTemp.AddRange(br.ReadBytes(4));
            MATEntry.CSize = BitConverter.ToInt32(BTemp.ToArray(), 0);

            //Uncompressed Data size.
            BTemp = new List<byte>();
            c = c + 4;
            br.BaseStream.Position = c;
            BTemp.AddRange(br.ReadBytes(4));
            BTemp.Reverse();
            string TempStr = "";
            TempStr = ByteUtilitarian.BytesToStringL2(BTemp, TempStr);
            BigInteger BN1, BN2, DIFF;
            BN2 = BigInteger.Parse("40000000", NumberStyles.HexNumber);
            BN1 = BigInteger.Parse(TempStr, NumberStyles.HexNumber);
            DIFF = BN1 - BN2;
            MATEntry.DSize = (int)DIFF;

            //Data Offset.
            BTemp = new List<byte>();
            c = c + 4;
            br.BaseStream.Position = c;
            BTemp.AddRange(br.ReadBytes(4));
            MATEntry.AOffset = BitConverter.ToInt32(BTemp.ToArray(), 0);

            //Compressed Data.
            BTemp = new List<byte>();
            c = MATEntry.AOffset;
            br.BaseStream.Position = c;
            BTemp.AddRange(br.ReadBytes(MATEntry.CSize));
            MATEntry.CompressedData = BTemp.ToArray();

            //Namestuff.
            MATEntry.EntryName = Tempname;
            MATEntry._FileType = ".mrl";

            //Ensures existing subdirectories are cleared so the directories for files are displayed correctly.
            if (subnames != null)
            {
                if (subnames.Count > 0)
                {
                    subnames.Clear();
                }
            }

            //Gets the filename without subdirectories.
            if (MATEntry.EntryName.Contains("\\"))
            {
                string[] splstr = MATEntry.EntryName.Split('\\');

                //foreach (string v in splstr)
                for (int v = 0; v < (splstr.Length - 1); v++)
                {
                    if (!subnames.Contains(splstr[v]))
                    {
                        subnames.Add(splstr[v]);
                    }
                }


                MATEntry.TrueName = MATEntry.EntryName.Substring(MATEntry.EntryName.IndexOf("\\") + 1);
                Array.Clear(splstr, 0, splstr.Length);

                while (MATEntry.TrueName.Contains("\\"))
                {
                    MATEntry.TrueName = MATEntry.TrueName.Substring(MATEntry.TrueName.IndexOf("\\") + 1);
                }
            }
            else
            {
                MATEntry.TrueName = MATEntry.EntryName;
            }

            MATEntry._FileName = MATEntry.TrueName;

            MATEntry.EntryDirs = subnames.ToArray();
            MATEntry.FileExt = ".mrl";
            MATEntry.EntryName = MATEntry.EntryName + MATEntry.FileExt;

            //Decompression Time.
            MATEntry.UncompressedData = ZlibStream.UncompressBuffer(MATEntry.CompressedData);
            MATEntry._FileLength = MATEntry.UncompressedData.Length;


            //Material specific work here.
            MATEntry.WTemp = new byte[5];
            Array.Copy(MATEntry.UncompressedData, 0, MATEntry.WTemp, 0, 5);
            MATEntry.Magic = ByteUtilitarian.BytesToString(MATEntry.WTemp,MATEntry.Magic);

            byte[] MTemp = new byte[4];
            Array.Copy(MATEntry.UncompressedData, 8, MTemp, 0, 4);
            MATEntry.MaterialCount = BitConverter.ToInt32(MTemp,0);
            MATEntry._MaterialTotal = MATEntry.MaterialCount;

            Array.Copy(MATEntry.UncompressedData, 12, MTemp, 0, 4);
            MATEntry.TextureCount = BitConverter.ToInt32(MTemp,0);
            MATEntry._TextureTotal = MATEntry.TextureCount;

            Array.Copy(MATEntry.UncompressedData, 16, MTemp, 0, 4);
            MATEntry.WeirdHash = ByteUtilitarian.BytesToString(MTemp, MATEntry.WeirdHash);

            byte[] SixFourTemp = new byte[8];
            Array.Copy(MATEntry.UncompressedData, 24, SixFourTemp, 0, 8);
            MATEntry.TextureOffset = BitConverter.ToInt64(SixFourTemp,0);
            MATEntry._TextureStartingOffset = Convert.ToInt32(MATEntry.TextureOffset);

            Array.Copy(MATEntry.UncompressedData, 32, SixFourTemp, 0, 8);
            MATEntry.MaterialOffset = BitConverter.ToInt64(SixFourTemp, 0);
            MATEntry._MaterialStartingOffset = Convert.ToInt32(MATEntry.MaterialOffset);

            MATEntry.TexEntries = new List<MaterialTextureReference>();

            int j = Convert.ToInt32(MATEntry.TextureOffset);
            int k,l;
            byte[] MENTemp = new byte[64];
            //Fills in(or at least tries to) fill in each Texture and Material entry.
            for (int i = 0; i < MATEntry.TextureCount; i++)
            {
                j = (Convert.ToInt32(MATEntry.TextureOffset) + i * 88);
                MaterialTextureReference TexTemp = new MaterialTextureReference();
                Array.Copy(MATEntry.UncompressedData, j, MTemp, 0, 4);
                TexTemp.TypeHash = ByteUtilitarian.BytesToString(MTemp, TexTemp.TypeHash);
                j = j + 24;
                BTemp.Clear();
                Array.Copy(MATEntry.UncompressedData, j, MENTemp, 0, 64);
                BTemp.AddRange(MENTemp);
                BTemp.RemoveAll(ByteUtilitarian.IsZeroByte);
                ASCIIEncoding asciime = new ASCIIEncoding();
                Tempname = asciime.GetString(BTemp.ToArray());
                TexTemp.FullTexName = Tempname;
                TexTemp.Index = i;
                TexTemp._Index = TexTemp.Index;
                MATEntry.TexEntries.Add(TexTemp);
            }

            j = Convert.ToInt32(MATEntry.MaterialOffset);

            byte[] XTemp = new byte[4];
            byte[] XXTemp = new byte[8];
            int ITemp;
            string HashTemp = "";
            string BinTempA = "";
            string BinTempB = "";
            
            for (int i=0; i< MATEntry.MaterialCount; i++)
            {
                MaterialMaterialEntry MMEntry = new MaterialMaterialEntry();
                Array.Copy(MATEntry.UncompressedData, j, XTemp, 0, 4);
                MMEntry.TypeHash = ByteUtilitarian.HashBytesToString(XTemp);
                j = j + 4;
                Array.Copy(MATEntry.UncompressedData, j, XTemp, 0, 4);
                ITemp = BitConverter.ToInt32(XTemp,0);
                MMEntry.UnknownField04 = ITemp.ToString();
                j = j + 4;
                Array.Copy(MATEntry.UncompressedData, j, XTemp, 0, 4);
                MMEntry.NameHash = ByteUtilitarian.HashBytesToString(XTemp);
                j = j + 4;
                Array.Copy(MATEntry.UncompressedData, j, XTemp, 0, 4);
                MMEntry.CmdBufferSize = BitConverter.ToInt32(XTemp,0);
                j = j + 4;

                //This part is for ShaderObjectID Stuff.

                Array.Copy(MATEntry.UncompressedData, j, XTemp, 0, 4);
                Array.Reverse(XTemp);
                HashTemp = ByteUtilitarian.BytesToString(XTemp, HashTemp);
                BinTempA = HashTemp.Substring(5,3);
                BinTempB = HashTemp.Substring(0,5);
                MMEntry.BlendState = new MaterialMaterialEntry.MatShaderObject();
                MMEntry.BlendState.Index = int.Parse(BinTempA, System.Globalization.NumberStyles.HexNumber);
                int Num = MMEntry.BlendState.Index;
                string line = "";
                try
                {
                    line = File.ReadLines("mvc3shadertypes.cfg").Skip(MMEntry.BlendState.Index).Take(1).First();
                }
                catch (Exception xx)
                {
                    MessageBox.Show("mvc3shadertypes.cfg is missing or not read. Can't continue parsing materials.", "Uh-Oh");
                    return null;
                }

                MMEntry.BlendState.Hash = line;
                j = j + 4;

                Array.Copy(MATEntry.UncompressedData, j, XTemp, 0, 4);
                Array.Reverse(XTemp);
                HashTemp = ByteUtilitarian.BytesToString(XTemp, HashTemp);
                BinTempA = HashTemp.Substring(5, 3);
                BinTempB = HashTemp.Substring(0, 5);
                MMEntry.DepthStencilState = new MaterialMaterialEntry.MatShaderObject();
                MMEntry.DepthStencilState.Index = int.Parse(BinTempA, System.Globalization.NumberStyles.HexNumber);
                int Num2 = MMEntry.DepthStencilState.Index;
                string line2 = "";
                try
                {
                    line2 = File.ReadLines("mvc3shadertypes.cfg").Skip(MMEntry.DepthStencilState.Index).Take(1).First();
                }
                catch (Exception xx)
                {
                    MessageBox.Show("mvc3shadertypes.cfg is missing or not read. Can't continue parsing materials.", "Uh-Oh");
                    return null;
                }

                MMEntry.DepthStencilState.Hash = line2;
                j = j + 4;

                Array.Copy(MATEntry.UncompressedData, j, XTemp, 0, 4);
                Array.Reverse(XTemp);
                HashTemp = ByteUtilitarian.BytesToString(XTemp, HashTemp);
                BinTempA = HashTemp.Substring(5, 3);
                BinTempB = HashTemp.Substring(0, 5);
                MMEntry.RasterizerState = new MaterialMaterialEntry.MatShaderObject();
                MMEntry.RasterizerState.Index = int.Parse(BinTempA, System.Globalization.NumberStyles.HexNumber);
                int Num3 = MMEntry.RasterizerState.Index;
                string line3 = "";
                try
                {
                    line3 = File.ReadLines("mvc3shadertypes.cfg").Skip(MMEntry.RasterizerState.Index).Take(1).First();
                }
                catch (Exception xx)
                {
                    MessageBox.Show("mvc3shadertypes.cfg is missing or not read. Can't continue parsing materials.", "Uh-Oh");
                    return null;
                }

                MMEntry.RasterizerState.Hash = line3;
                j = j + 4;

                //Material Command List Info.
                Array.Copy(MATEntry.UncompressedData, j, XTemp, 0, 4);
                Array.Reverse(XTemp);
                HashTemp = ByteUtilitarian.BytesToString(XTemp, HashTemp);
                BinTempA = HashTemp.Substring(5, 3);
                BinTempB = HashTemp.Substring(0, 5);
                MMEntry.MaterialCommandListInfo = new MaterialMaterialEntry.MaterialCmdListInfo();
                MMEntry.MaterialCommandListInfo.Count = int.Parse(BinTempA, System.Globalization.NumberStyles.HexNumber);
                MMEntry.MaterialCommandListInfo.Unknown = int.Parse(BinTempB, System.Globalization.NumberStyles.HexNumber); ;
                j = j + 4;

                Array.Copy(MATEntry.UncompressedData, j, XTemp, 0, 4);
                MMEntry.MateialinfoFlags = ByteUtilitarian.BytesToString(XTemp, MMEntry.MateialinfoFlags);

                //Time for the weird unknown fields.
                j = j + 4;
                Array.Copy(MATEntry.UncompressedData, j, XTemp, 0, 4);
                MMEntry.UnknownField24 = ByteUtilitarian.BytesToString(XTemp, MMEntry.UnknownField24);
                j = j + 4;
                Array.Copy(MATEntry.UncompressedData, j, XTemp, 0, 4);
                MMEntry.UnknownField28 = ByteUtilitarian.BytesToString(XTemp, MMEntry.UnknownField28);
                j = j + 4;
                Array.Copy(MATEntry.UncompressedData, j, XTemp, 0, 4);
                MMEntry.UnknownField2C = ByteUtilitarian.BytesToString(XTemp, MMEntry.UnknownField2C);
                j = j + 4;
                Array.Copy(MATEntry.UncompressedData, j, XTemp, 0, 4);
                MMEntry.UnknownField30 = ByteUtilitarian.BytesToString(XTemp, MMEntry.UnknownField30);

                //More Parameters yay!
                j = j + 4;
                Array.Copy(MATEntry.UncompressedData, j, XTemp, 0, 4);
                Array.Reverse(XTemp);
                MMEntry.AnimDataSize = BitConverter.ToInt32(XTemp,0);

                j = j + 4;
                Array.Copy(MATEntry.UncompressedData, j, XXTemp, 0, 8);
                //Array.Reverse(XXTemp);
                MMEntry.CmdListOffset = BitConverter.ToInt32(XXTemp, 0);

                j = j + 8;
                Array.Copy(MATEntry.UncompressedData, j, XXTemp, 0, 8);
                Array.Reverse(XXTemp);
                MMEntry.AnimDataOffset = BitConverter.ToInt32(XXTemp, 0);
                MMEntry.SomethingLabeledP = Convert.ToUInt32(j);
                j = j + 8;
                j = MMEntry.CmdListOffset;

                //If there's no animdata.
                if (MMEntry.AnimDataSize == 0)
                {
                    //Command List Info.
                    for (int p = 0; i < MMEntry.MaterialCommandListInfo.Count; i++)
                    {
                        MatCmd cmd = new MatCmd();
                        MatCmdInfo cmdInfo = new MatCmdInfo();
                        Array.Copy(MATEntry.UncompressedData, j, XTemp, 0, 4);
                        Array.Reverse(XTemp);
                        HashTemp = ByteUtilitarian.BytesToString(XTemp, HashTemp);
                        BinTempA = HashTemp.Substring(5, 3);
                        BinTempB = HashTemp.Substring(0, 5);
                        cmdInfo.SetFlag = 0;
                        cmdInfo.SomeValue = 0;
                        cmdInfo.ShaderObjectIndex = int.Parse(BinTempA, System.Globalization.NumberStyles.HexNumber);
                        cmd.MCInfo = cmdInfo;
                        j = j + 4;
                        Array.Copy(MATEntry.UncompressedData, j, XTemp, 0, 4);
                        MMEntry.UnknownField04 = BitConverter.ToString(XTemp, 0);
                        j = j + 4;
                            
                        //Union Value stuff.
                        Value val = new Value();
                        Array.Copy(MATEntry.UncompressedData, j, XXTemp, 0, 8);
                        j = j + 4;
                        Array.Copy(MATEntry.UncompressedData, j, XTemp, 0, 4);
                        Array.Reverse(XXTemp);
                        Array.Reverse(XTemp);
                        val.ConstantBufferDataOffset = BitConverter.ToUInt64(XXTemp, 0);
                        val.TextureIndex = BitConverter.ToUInt16(XTemp,0);
                        HashTemp = ByteUtilitarian.BytesToString(XTemp, HashTemp);
                        BinTempA = HashTemp.Substring(5, 3);
                        BinTempB = HashTemp.Substring(0, 5);
                        val.VShaderObjectID.Index = int.Parse(BinTempA, System.Globalization.NumberStyles.HexNumber);
                        val.VShaderObjectID.Hash = BinTempB;
                        cmd.MaterialCommandValue = val;

                        j = j + 8;

                        Array.Copy(MATEntry.UncompressedData, j, XTemp, 0, 4);
                        Array.Reverse(XTemp);
                        HashTemp = ByteUtilitarian.BytesToString(XTemp, HashTemp);
                        BinTempA = HashTemp.Substring(5, 3);
                        BinTempB = HashTemp.Substring(0, 5);
                        cmd.CmdShaderObject.Index = int.Parse(BinTempA, System.Globalization.NumberStyles.HexNumber);
                        cmd.CmdShaderObject.Hash = BinTempB;

                        j = j + 4;
                        Array.Copy(MATEntry.UncompressedData, j, XTemp, 0, 4);
                        Array.Reverse(XTemp);
                        HashTemp = ByteUtilitarian.BytesToString(XTemp, HashTemp);
                        cmd.SomeField14 = Convert.ToUInt32(HashTemp);

                        MMEntry.MaterialCommands = new List<MatCmd>();
                        MMEntry.MaterialCommands.Add(cmd);
                    }

                   //Now for the CommandBuffer.
                    MMEntry.CommandBufferIndex = MMEntry.CmdBufferSize - (MMEntry.MaterialCommandListInfo.Count*24);
                    MMEntry.ConstantBufferData = new byte[MMEntry.CommandBufferIndex];

                    

                }


            }


            return MATEntry;

        }

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

        private int _TextureTotal;
        [Category("Material Data"), ReadOnlyAttribute(true)]
        public int TextureTotal
        {
            get
            {
                return _TextureTotal;
            }
            set
            {
                _TextureTotal = value;
            }
        }

        private int _MaterialTotal;
        [Category("Material Data"), ReadOnlyAttribute(true)]
        public int MaterialTotal
        {
            get
            {
                return _MaterialTotal;
            }
            set
            {
                _MaterialTotal = value;
            }
        }

        private int _TextureStartingOffset;
        [Category("Material Data"), ReadOnlyAttribute(true)]
        public int TextureStartingOffset
        {
            get
            {
                return _TextureStartingOffset;
            }
            set
            {
                _TextureStartingOffset = value;
            }
        }

        private int _MaterialStartingOffset;
        [Category("Material Data"), ReadOnlyAttribute(true)]
        public int MaterialStartingOffset
        {
            get
            {
                return _MaterialStartingOffset;
            }
            set
            {
                _MaterialStartingOffset = value;
            }
        }

        #endregion

    }
}
