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
                        MMEntry = MMEntry.FIllMatMatEntryPropertiesPart1(MMEntry, MATEntry,MBR, MatStream, PrevOffset, i);
                        MMEntry = MMEntry.FIllMatMatEntryPropertiesPart2(MMEntry, MATEntry, MBR, MatStream, PrevOffset, i);

                        MATEntry.Materials.Add(MMEntry);
                        PrevOffset = PrevOffset + 72;
                        MBR.BaseStream.Position = PrevOffset;

                    }
                    


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

                return node.entryfile as MaterialEntry;

        }


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
