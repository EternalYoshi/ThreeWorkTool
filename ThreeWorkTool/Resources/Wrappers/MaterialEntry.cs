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

            ReplaceKnownEntry(tree, node, filename, matentry, oldentry);

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
                using (StreamWriter sw = File.AppendText("Log.txt"))
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
                using (StreamWriter sw = File.AppendText("Log.txt"))
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
                TexTemp = TexTemp.FillMaterialTexReference(MATEntry, i, MBR, TexTemp);
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
                MMEntry = MMEntry.FIllMatMatEntryPropertiesPart1(MMEntry, MATEntry, MBR, PrevOffset, i);
                MMEntry = MMEntry.FIllMatMatEntryPropertiesPart2(MMEntry, MATEntry, MBR, PrevOffset, i);

                MATEntry.Materials.Add(MMEntry);
                PrevOffset = PrevOffset + 72;
                MBR.BaseStream.Position = PrevOffset;

            }

            return MATEntry;

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
