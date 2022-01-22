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
using ThreeWorkTool.Resources.Wrappers;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class TextureEntry : DefaultWrapper
    {
        public static string TYPEHASH = "241F5DEB";
        public string Magic;
        public int XSize;
        public int YSize;
        public int ZSize;
        public int Type;
        public int Dimension;
        public int PixelCount;
        public string TexType;
        public bool HasTransparency;
        public bool HasMips;
        public int Mips;
        public int Dimensions;
        public byte[] WTemp;
        public byte[] OutMaps;
        public byte[][] OutMapsB;
        public byte[] OutMapsC;
        public byte[] OutTar;
        public int[] MipOffsets;
        public List<byte> OutTexTest;
        public int Shift;
        public Bitmap Picture;
        public bool IsCubeMap = false;
        public List<Face> CubeFace;
        public struct Face
        {
            public float field00;
            public Vector3 Negative;
            public Vector3 Postiive;
            public Vector2 UV;
        }


        public static TextureEntry FillTexEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
        {
            TextureEntry texentry = new TextureEntry();

            FillEntry(filename, subnames, tree, br, c, ID, texentry, filetype);

            texentry._FileName = texentry.TrueName;

            //Actual Tex Loading work here.

            //Gets the SizeShift.... whatever that is.
            texentry.Shift = texentry.UncompressedData[6];

            //texentry.Type = BitConverter.ToInt16(texentry.UncompressedData, 4);

            //Let's try this MemoryStream Stuff.

            using (MemoryStream TexStream = new MemoryStream(texentry.UncompressedData))
            {
                using (BinaryReader brStream = new BinaryReader(TexStream))
                {
                    brStream.BaseStream.Position = 13;
                    texentry.TexType = brStream.ReadByte().ToString("X2");
                    texentry._TextureType = texentry.TexType;

                    List<byte> PreviewTemp = new List<byte>();

                    texentry = InitializeTexture(tree, texentry, brStream, TexStream, filename);

                }
            }

            return texentry;

        }

        #region TextureEntry Properties
        private string _FileName;
        [Category("Filename"), ReadOnlyAttribute(true)]
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

        private string _TextureType;
        [Category("Filename"), ReadOnlyAttribute(true)]
        public string TextureType
        {

            get
            {
                return _TextureType;
            }
            set
            {
                _TextureType = value;
            }
        }

        private int _X;
        [Category("Filename"), ReadOnlyAttribute(true)]
        public int X
        {

            get
            {
                return _X;
            }
            set
            {
                _X = value;
            }
        }

        private int _Y;
        [Category("Filename"), ReadOnlyAttribute(true)]
        public int Y
        {

            get
            {
                return _Y;
            }
            set
            {
                _Y = value;
            }
        }

        private int _MipMapCount;
        [Category("Filename"), ReadOnlyAttribute(true)]
        public int MipMapCount
        {

            get
            {
                return _MipMapCount;
            }
            set
            {
                _MipMapCount = value;
            }
        }

        private string _Format;
        [Category("Filename"), ReadOnlyAttribute(true)]
        public string Format
        {

            get
            {
                return _Format;
            }
            set
            {
                _Format = value;
            }
        }

        #endregion

        public static Bitmap BitmapBuilder(string filenametest, Stream strim)
        {
            #region PNG Stuffs
            //From the pfim website.
            using (var image = Pfim.Pfim.FromStream(strim))
            {
                PixelFormat format;

                // Convert from Pfim's backend agnostic image format into GDI+'s image format
                switch (image.Format)
                {
                    case Pfim.ImageFormat.Rgba32:
                        format = PixelFormat.Format32bppArgb;
                        break;
                    case Pfim.ImageFormat.Rgb24:
                        format = PixelFormat.Format24bppRgb;
                        break;
                    default:
                        // see the sample for more details
                        throw new NotImplementedException();
                }

                // Pin pfim's data array so that it doesn't get reaped by GC, unnecessary
                // in this snippet but useful technique if the data was going to be used in
                // control like a picture box
                var handle = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
                try
                {
                    var data = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);
                    var bitmap = new Bitmap(image.Width, image.Height, image.Stride, format, data);
                    //Stream strim;
                    //bitmap.Save(strim, System.Drawing.Imaging.ImageFormat.Png);
                    //bitmap.Save(Path.ChangeExtension(filenametest, ".png"), System.Drawing.Imaging.ImageFormat.Png);     
                    bitmap.Save(Path.ChangeExtension(filenametest, ".png"), System.Drawing.Imaging.ImageFormat.Png);
                    //Stream goodmap = new MemoryStream(barry);
                    //bitmap.Save(goodmap, System.Drawing.Imaging.ImageFormat.Bmp);
                    return bitmap;
                }
                finally
                {
                    handle.Free();
                }
            }
            #endregion
        }

        public static TextureEntry InsertTextureEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            TextureEntry teXentry = new TextureEntry();

            try
            {
                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
                {

                    //We build the arcentry starting from the uncompressed data.
                    teXentry.UncompressedData = System.IO.File.ReadAllBytes(filename);
                    teXentry.DSize = teXentry.UncompressedData.Length;

                    //Then Compress.
                    teXentry.CompressedData = Zlibber.Compressor(teXentry.UncompressedData);
                    teXentry.CSize = teXentry.CompressedData.Length;

                    //Gets the filename of the file to inject without the directory.
                    string trname = filename;
                    while (trname.Contains("\\"))
                    {
                        trname = trname.Substring(trname.IndexOf("\\") + 1);
                    }

                    //Gets Dimensions and Tex Type.
                    teXentry.TexType = teXentry.UncompressedData[13].ToString("X2");
                    teXentry._TextureType = teXentry.TexType;

                    teXentry.TrueName = trname;
                    teXentry._FileName = teXentry.TrueName;
                    teXentry.TrueName = Path.GetFileNameWithoutExtension(trname);
                    teXentry.FileExt = trname.Substring(trname.LastIndexOf("."));

                    //Gets the path of the selected node to inject here.
                    string nodepath = tree.SelectedNode.FullPath;
                    nodepath = nodepath.Substring(nodepath.IndexOf("\\") + 1);

                    string[] sepstr = { "\\" };
                    teXentry.EntryDirs = nodepath.Split(sepstr, StringSplitOptions.RemoveEmptyEntries);
                    teXentry.EntryName = teXentry.FileName;

                    teXentry = InitializeTextureI(tree, teXentry, bnr, filename);

                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText("Log.txt"))
                {
                    sw.WriteLine("Caught an exception:" + ex);
                }
            }



            return teXentry;
        }

        public static TextureEntry ReplaceTextureEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            TextureEntry texentry = new TextureEntry();
            TextureEntry oldentry = new TextureEntry();

            tree.BeginUpdate();

            try
            {
                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
                {
                    //We build the arcentry starting from the uncompressed data.
                    texentry.UncompressedData = System.IO.File.ReadAllBytes(filename);
                    texentry.DSize = texentry.UncompressedData.Length;

                    //Then Compress.
                    texentry.CompressedData = Zlibber.Compressor(texentry.UncompressedData);
                    texentry.CSize = texentry.CompressedData.Length;

                    using (MemoryStream TexStream = new MemoryStream(texentry.UncompressedData))
                    {

                        //Gets the filename of the file to inject without the directory.
                        string trname = filename;
                        while (trname.Contains("\\"))
                        {
                            trname = trname.Substring(trname.IndexOf("\\") + 1);
                        }


                        //Gets Dimensions and Tex Type.
                        TexStream.Position = 13;
                        texentry.TexType = TexStream.ReadByte().ToString("X2");
                        texentry._TextureType = texentry.TexType;

                        //Actual Tex Loading work here.
                        byte[] VTemp = new byte[4];
                        uint[] LWData = new uint[3];
                        byte[] DTemp = new byte[4];

                        List<byte> PreviewTemp = new List<byte>();

                        TexStream.Position = 8;

                        //Enters name related parameters of the arcentry.
                        texentry.TrueName = trname;
                        texentry._FileName = texentry.TrueName;
                        texentry.TrueName = Path.GetFileNameWithoutExtension(trname);
                        texentry.FileExt = trname.Substring(trname.LastIndexOf("."));

                        texentry = InitializeTextureR(tree, texentry, bnr, filename);

                        var tag = node.Tag;
                        if (tag is TextureEntry)
                        {
                            oldentry = tag as TextureEntry;
                        }
                        string path = "";
                        int index = oldentry.EntryName.LastIndexOf("\\");
                        if (index > 0)
                        {
                            path = oldentry.EntryName.Substring(0, index);
                        }

                        texentry.EntryName = path + "\\" + texentry.TrueName;

                        tag = texentry;

                        if (node.Tag is TextureEntry)
                        {
                            node.Tag = texentry;
                            node.Name = Path.GetFileNameWithoutExtension(texentry.EntryName);
                            node.Text = Path.GetFileNameWithoutExtension(texentry.EntryName);

                        }

                        var aew = node as ArcEntryWrapper;

                        string type = node.GetType().ToString();
                        if (type == "ThreeWorkTool.Resources.Wrappers.ArcEntryWrapper")
                        {
                            aew.entryfile = texentry;
                        }

                        node = aew;
                        node.entryfile = texentry;
                        tree.EndUpdate();
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Read error. Is the file readable?");
                using (StreamWriter sw = File.AppendText("Log.txt"))
                {
                    sw.WriteLine("Possible read error. Here's details:\n" + ex);
                }
            }



            return node.entryfile as TextureEntry;
        }

        public static TextureEntry InsertTextureFromDDS(TreeView tree, ArcEntryWrapper node, string filename, FrmTexEncodeDialog FTED, byte[] newtex, Type filetype = null)
        {
            //Gotta Finish this to ensure the insertion method is done properly.
            TextureEntry teXentry = new TextureEntry();

            try
            {
                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
                {

                    if (FTED.TXx > FTED.TXy)
                    {
                        double XD = Convert.ToDouble(FTED.TXx);
                        teXentry.MipMapCount = Convert.ToInt32(Math.Log(XD, 2.0));
                    }
                    else
                    {
                        double XD = Convert.ToDouble(FTED.TXy);
                        teXentry.MipMapCount = Convert.ToInt32(Math.Log(XD, 2.0));
                    }

                    FTED.TXfilename = teXentry.EntryName;
                    FTED.TXfilename = teXentry.TrueName;
                    teXentry._FileName = teXentry.TrueName;
                    teXentry.FileExt = ".tex";

                    //Gets Dimensions and Tex Type.                    
                    teXentry.TexType = FTED.TXTextureType;
                    teXentry._TextureType = teXentry.TexType;

                    string FullEightBinary = "00000000000000000000000000000000";

                    byte[] EightTemp = new byte[4];

                    //Fiddles with binary to insert the values in a little endian binary style.
                    string MipBinary = Convert.ToString(teXentry.MipMapCount, 2);
                    if (MipBinary.Length < 8)
                    {
                        MipBinary = MipBinary.PadLeft(8, '0');
                    }

                    //Gotta split these strings in accordance to how they're stored in the image I made yesterday, then insert them in the big binary.
                    string WidthBinary = Convert.ToString(FTED.TXx, 2);
                    if (WidthBinary.Length < 11)
                    {
                        WidthBinary = WidthBinary.PadLeft(11, '0');
                    }

                    if (WidthBinary.Length == 11)
                    {
                        WidthBinary = WidthBinary.Substring(0, WidthBinary.Length - 2);
                        WidthBinary = WidthBinary.PadLeft(11, '0');
                    }

                    string[] WidthParts = new string[2];
                    string[] LengthParts = new string[2];

                    string twp = WidthBinary;
                    WidthParts[0] = WidthBinary.Substring(3, 8);
                    WidthParts[1] = WidthBinary.Substring(0, 3);

                    string LengthBinary = Convert.ToString(FTED.TXy, 2);
                    if (LengthBinary.Length < 13)
                    {
                        LengthBinary = LengthBinary.PadLeft(13, '0');
                    }

                    string tlp = LengthBinary;
                    LengthParts[0] = LengthBinary.Substring(0, 8);
                    LengthParts[1] = tlp.Substring(8, 5);


                    var aStringBuilder = new StringBuilder(FullEightBinary);
                    //Puts the MipMap Count in the primary string Binary.
                    aStringBuilder.Remove(0, 8);
                    aStringBuilder.Insert(0, MipBinary);
                    aStringBuilder.Remove(8, 8);
                    aStringBuilder.Insert(8, WidthParts[0]);

                    aStringBuilder.Remove(16, 5);
                    aStringBuilder.Insert(16, LengthParts[1]);
                    aStringBuilder.Remove(21, 3);
                    aStringBuilder.Insert(21, WidthParts[1]);

                    aStringBuilder.Remove(24, 8);
                    aStringBuilder.Insert(24, LengthParts[0]);

                    string bytesstr = aStringBuilder.ToString();

                    byte[] TexTemp;
                    TexTemp = new byte[] { };

                    //Gets the binary representation into 4 Bytes.
                    TexTemp = ByteUtilitarian.BinaryStringToByteArray(bytesstr);

                    teXentry.Shift = 0;
                    byte[] TEXHeader = { 0x54, 0x45, 0x58, 0x00, 0x9d, 0xa0, 0x00, 0x20 };

                    //Starts Building the tex data in this list.
                    List<byte> TBuffer = new List<byte>();
                    TBuffer.AddRange(TEXHeader);
                    TBuffer.AddRange(TexTemp);

                    //Filling in data for the teXentry.
                    teXentry.TrueName = FTED.ShortName;
                    teXentry._FileName = teXentry.TrueName;
                    teXentry.UncompressedData = newtex;
                    teXentry.CompressedData = Zlibber.Compressor(newtex);
                    teXentry.DSize = newtex.Length;
                    teXentry.CSize = teXentry.CompressedData.Length;
                    teXentry._X = FTED.TXx;
                    teXentry._Y = FTED.TXy;
                    teXentry.XSize = teXentry._X;
                    teXentry.YSize = teXentry._Y;
                    //teXentry.OutMaps = FTED.FirstMip;
                    teXentry.OutMaps = FTED.DDSData;

                    switch (teXentry.TexType)
                    {
                        #region Bitmap Textures
                        case "13":
                            teXentry._Format = "DXT1/BC1";
                            byte[] HeaderTwo = { 0x01, 0x13, 0x01, 0x01 };
                            TBuffer.AddRange(HeaderTwo);

                            for (int mip = 0; mip < teXentry.MipMapCount; mip++)
                            {
                                //Writes a dummy entry for each mipmap.
                                byte[] MipOffsetData = { 0x60, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                            }

                            teXentry = DDSToRGBA(teXentry);

                            break;

                        #endregion

                        #region Bitmap Textures with Transparency
                        case "17":
                            teXentry._Format = "DXT5/BC3";

                            byte[] HeaderTwo17 = { 0x01, 0x17, 0x01, 0x01 };
                            TBuffer.AddRange(HeaderTwo17);

                            for (int mip = 0; mip < teXentry.MipMapCount; mip++)
                            {
                                //Writes a dummy entry for each mipmap.
                                byte[] MipOffsetData = { 0x60, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                            }

                            teXentry = DDSToRGBA(teXentry);

                            break;
                        #endregion

                        #region Specular Tetures
                        case "19":
                            teXentry._Format = "BC4_UNORM/Metalic/Specular Map";

                            byte[] HeaderTwo19 = { 0x01, 0x19, 0x01, 0x01 };
                            TBuffer.AddRange(HeaderTwo19);

                            for (int mip = 0; mip < teXentry.MipMapCount; mip++)
                            {
                                //Writes a dummy entry for each mipmap.
                                byte[] MipOffsetData = { 0x60, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                            }

                            teXentry = DDSToRGBA(teXentry);

                            break;

                        #endregion

                        #region Normal Maps(Incomplete)
                        case "1F":
                            teXentry._Format = "BC5/Normal Map";

                            byte[] HeaderTwo1F = { 0x01, 0x1F, 0x01, 0x01 };
                            TBuffer.AddRange(HeaderTwo1F);

                            for (int mip = 0; mip < teXentry.MipMapCount; mip++)
                            {
                                //Writes a dummy entry for each mipmap.
                                byte[] MipOffsetData = { 0x60, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                            }

                            teXentry = DDSToRGBA(teXentry);

                            break;


                        #endregion

                        #region Weird Toon Shader Textures
                        case "27":
                            teXentry._Format = "????/Problematic Portrait Picture";

                            byte[] HeaderTwo27 = { 0x01, 0x27, 0x01, 0x01 };
                            TBuffer.AddRange(HeaderTwo27);

                            for (int mip = 0; mip < teXentry.MipMapCount; mip++)
                            {
                                //Writes a dummy entry for each mipmap.
                                byte[] MipOffsetData = { 0x60, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                            }

                            teXentry = DDSToRGBA(teXentry);

                            break;

                        #endregion

                        #region Weirdo Problematic Portrait Textures
                        case "2A":
                            teXentry._Format = "????/Problematic Portrait Picture";

                            byte[] HeaderTwo2A = { 0x01, 0x2A, 0x01, 0x01 };
                            TBuffer.AddRange(HeaderTwo2A);

                            for (int mip = 0; mip < teXentry.MipMapCount; mip++)
                            {
                                //Writes a dummy entry for each mipmap.
                                byte[] MipOffsetData = { 0x60, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                            }

                            teXentry = DDSToRGBA(teXentry);

                            break;

                        #endregion

                        default:
                            break;
                    }



                    //Gets the path of the selected node to inject here.
                    string nodepath = tree.SelectedNode.FullPath;
                    nodepath = nodepath.Substring(nodepath.IndexOf("\\") + 1);

                    string[] sepstr = { "\\" };
                    teXentry.EntryDirs = nodepath.Split(sepstr, StringSplitOptions.RemoveEmptyEntries);
                    teXentry.EntryName = teXentry.FileName;

                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText("Log.txt"))
                {
                    sw.WriteLine("Texture insertion from .DDS file failed. Here's details:\n" + ex);
                }
            }


            //teXentry.OutMaps = ;

            return teXentry;
        }

        public static TextureEntry ReplaceTextureFromDDS(TreeView tree, ArcEntryWrapper node, string filename, FrmTexEncodeDialog FTED, byte[] newtex, Type filetype = null)
        {
            //Gotta Finish this to ensure the insertion method is done properly.
            TextureEntry teXentry = new TextureEntry();

            try
            {
                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
                {

                    if (FTED.TXx > FTED.TXy)
                    {
                        double XD = Convert.ToDouble(FTED.TXx);
                        teXentry.MipMapCount = Convert.ToInt32(Math.Log(XD, 2.0));
                    }
                    else
                    {
                        double XD = Convert.ToDouble(FTED.TXy);
                        teXentry.MipMapCount = Convert.ToInt32(Math.Log(XD, 2.0));
                    }

                    FTED.TXfilename = teXentry.EntryName;
                    FTED.TXfilename = teXentry.TrueName;
                    teXentry._FileName = teXentry.TrueName;
                    teXentry.FileExt = ".tex";

                    //Gets Dimensions and Tex Type.                    
                    teXentry.TexType = FTED.TXTextureType;
                    teXentry._TextureType = teXentry.TexType;

                    string FullEightBinary = "00000000000000000000000000000000";

                    byte[] EightTemp = new byte[4];

                    //Fiddles with binary to insert the values in a little endian binary style.
                    string MipBinary = Convert.ToString(teXentry.MipMapCount, 2);
                    if (MipBinary.Length < 8)
                    {
                        MipBinary = MipBinary.PadLeft(8, '0');
                    }

                    //Gotta split these strings in accordance to how they're stored in the image I made yesterday, then insert them in the big binary.
                    string WidthBinary = Convert.ToString(FTED.TXx, 2);
                    if (WidthBinary.Length < 11)
                    {
                        WidthBinary = WidthBinary.PadLeft(11, '0');
                    }

                    if (WidthBinary.Length == 11)
                    {
                        WidthBinary = WidthBinary.Substring(0, WidthBinary.Length - 2);
                        WidthBinary = WidthBinary.PadLeft(11, '0');
                    }

                    string[] WidthParts = new string[2];
                    string[] LengthParts = new string[2];

                    string twp = WidthBinary;
                    WidthParts[0] = WidthBinary.Substring(3, 8);
                    WidthParts[1] = WidthBinary.Substring(0, 3);

                    string LengthBinary = Convert.ToString(FTED.TXy, 2);
                    if (LengthBinary.Length < 13)
                    {
                        LengthBinary = LengthBinary.PadLeft(13, '0');
                    }

                    string tlp = LengthBinary;
                    LengthParts[0] = LengthBinary.Substring(0, 8);
                    LengthParts[1] = tlp.Substring(8, 5);


                    var aStringBuilder = new StringBuilder(FullEightBinary);
                    //Puts the MipMap Count in the primary string Binary.
                    aStringBuilder.Remove(0, 8);
                    aStringBuilder.Insert(0, MipBinary);
                    aStringBuilder.Remove(8, 8);
                    aStringBuilder.Insert(8, WidthParts[0]);

                    aStringBuilder.Remove(16, 5);
                    aStringBuilder.Insert(16, LengthParts[1]);
                    aStringBuilder.Remove(21, 3);
                    aStringBuilder.Insert(21, WidthParts[1]);

                    aStringBuilder.Remove(24, 8);
                    aStringBuilder.Insert(24, LengthParts[0]);

                    string bytesstr = aStringBuilder.ToString();

                    byte[] TexTemp;
                    TexTemp = new byte[] { };

                    //Gets the binary representation into 4 Bytes.
                    TexTemp = ByteUtilitarian.BinaryStringToByteArray(bytesstr);

                    teXentry.Shift = 0;
                    byte[] TEXHeader = { 0x54, 0x45, 0x58, 0x00, 0x9d, 0xa0, 0x00, 0x20 };

                    //Starts Building the tex data in this list.
                    List<byte> TBuffer = new List<byte>();
                    TBuffer.AddRange(TEXHeader);
                    TBuffer.AddRange(TexTemp);

                    TextureEntry OldTex = node.Tag as TextureEntry;

                    //Filling in data for the teXentry.
                    //teXentry.TrueName = FTED.ShortName;
                    //teXentry._FileName = teXentry.TrueName;
                    teXentry.TrueName = OldTex.TrueName;
                    teXentry._FileName = OldTex._FileName;
                    teXentry.EntryName = OldTex.EntryName;

                    teXentry.UncompressedData = newtex;
                    teXentry.CompressedData = Zlibber.Compressor(newtex);
                    teXentry.DSize = newtex.Length;
                    teXentry.CSize = teXentry.CompressedData.Length;
                    teXentry._X = FTED.TXx;
                    teXentry._Y = FTED.TXy;
                    teXentry.XSize = teXentry._X;
                    teXentry.YSize = teXentry._Y;
                    //teXentry.OutMaps = FTED.FirstMip;
                    teXentry.OutMaps = FTED.DDSData;

                    switch (teXentry.TexType)
                    {
                        #region Bitmap Textures
                        case "13":
                            teXentry._Format = "DXT1/BC1";
                            byte[] HeaderTwo = { 0x01, 0x13, 0x01, 0x01 };
                            TBuffer.AddRange(HeaderTwo);

                            for (int mip = 0; mip < teXentry.MipMapCount; mip++)
                            {
                                //Writes a dummy entry for each mipmap.
                                byte[] MipOffsetData = { 0x60, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                            }

                            teXentry = DDSToRGBA(teXentry);


                            break;

                        #endregion

                        #region Bitmap Textures with Transparency
                        case "17":
                            teXentry._Format = "DXT5/BC3";

                            byte[] HeaderTwo17 = { 0x01, 0x17, 0x01, 0x01 };
                            TBuffer.AddRange(HeaderTwo17);

                            for (int mip = 0; mip < teXentry.MipMapCount; mip++)
                            {
                                //Writes a dummy entry for each mipmap.
                                byte[] MipOffsetData = { 0x60, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                            }

                            teXentry = DDSToRGBA(teXentry);

                            break;
                        #endregion

                        #region Specular Tetures
                        case "19":
                            teXentry._Format = "BC4_UNORM/Metalic/Specular Map";

                            byte[] HeaderTwo19 = { 0x01, 0x19, 0x01, 0x01 };
                            TBuffer.AddRange(HeaderTwo19);

                            for (int mip = 0; mip < teXentry.MipMapCount; mip++)
                            {
                                //Writes a dummy entry for each mipmap.
                                byte[] MipOffsetData = { 0x60, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                            }

                            teXentry = DDSToRGBA(teXentry);

                            break;

                        #endregion

                        #region Normal Maps(Incomplete)
                        case "1F":
                            teXentry._Format = "BC5/Normal Map";

                            byte[] HeaderTwo1F = { 0x01, 0x1F, 0x01, 0x01 };
                            TBuffer.AddRange(HeaderTwo1F);

                            for (int mip = 0; mip < teXentry.MipMapCount; mip++)
                            {
                                //Writes a dummy entry for each mipmap.
                                byte[] MipOffsetData = { 0x60, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                            }

                            teXentry = DDSToRGBA(teXentry);

                            break;


                        #endregion

                        #region Weird Toon Shader Textures
                        case "27":
                            teXentry._Format = "????/Problematic Portrait Picture";

                            byte[] HeaderTwo27 = { 0x01, 0x27, 0x01, 0x01 };
                            TBuffer.AddRange(HeaderTwo27);

                            for (int mip = 0; mip < teXentry.MipMapCount; mip++)
                            {
                                //Writes a dummy entry for each mipmap.
                                byte[] MipOffsetData = { 0x60, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                            }

                            teXentry = DDSToRGBA(teXentry);

                            break;

                        #endregion

                        #region Weirdo Problematic Portrait Textures
                        case "2A":
                            teXentry._Format = "????/Problematic Portrait Picture";

                            byte[] HeaderTwo2A = { 0x01, 0x2A, 0x01, 0x01 };
                            TBuffer.AddRange(HeaderTwo2A);

                            for (int mip = 0; mip < teXentry.MipMapCount; mip++)
                            {
                                //Writes a dummy entry for each mipmap.
                                byte[] MipOffsetData = { 0x60, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                            }

                            teXentry = DDSToRGBA(teXentry);

                            break;

                        #endregion

                        default:
                            break;
                    }



                    //Gets the path of the selected node to inject here.
                    string nodepath = tree.SelectedNode.FullPath;
                    nodepath = nodepath.Substring(nodepath.IndexOf("\\") + 1);

                    string[] sepstr = { "\\" };
                    teXentry.EntryDirs = nodepath.Split(sepstr, StringSplitOptions.RemoveEmptyEntries);
                    teXentry.EntryName = teXentry.FileName;

                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText("Log.txt"))
                {
                    sw.WriteLine("Texture replacement from .DDS file failed. Here's details:\n" + ex);
                }
            }


            //teXentry.OutMaps = ;

            return teXentry;
        }

        public static TextureEntry InitializeTexture(TreeView tree, TextureEntry texentry, BinaryReader brStream, MemoryStream TexStream, string filename)
        {
            List<byte> BTemp = new List<byte>();
            byte[] VTemp = new byte[4];
            uint[] LWData = new uint[3];
            byte[] DTemp = new byte[4];

            switch (texentry.TexType)
            {

                #region Bitmap Textures
                case "13":

                    texentry._Format = "DXT1/BC1";
                    //Gets the unsigned integers which hold data on the texture's dimensions.
                    brStream.BaseStream.Position = 4;
                    LWData[0] = BitConverter.ToUInt32(brStream.ReadBytes(4), 0);
                    LWData[1] = BitConverter.ToUInt32(brStream.ReadBytes(4), 0);
                    LWData[2] = BitConverter.ToUInt32(brStream.ReadBytes(4), 0);

                    byte[] bytemp = { 0x09, 0x80, 0x10, 0x01 };
                    uint uinttemp;
                    int inttemp;
                    int inttempw;

                    //X and Y coordinates. This method is borrowed from the old TexCheck.py file.
                    texentry.XSize = Convert.ToInt32(((LWData[1] >> 6) & 0x1fff));

                    uinttemp = BitConverter.ToUInt32(bytemp, 0);
                    inttemp = Convert.ToInt32((uinttemp >> 6) & 0x1fff);
                    inttempw = Convert.ToInt32((uinttemp >> 19) & 0x1fff);
                    texentry._X = texentry.XSize;
                    uinttemp = uinttemp & 0x1fff;

                    texentry.YSize = Convert.ToInt32(((LWData[1] >> 19) & 0x1fff));
                    texentry._Y = texentry.YSize;

                    texentry.Mips = Convert.ToInt32(((LWData[1]) & 0x3f));
                    texentry._MipMapCount = texentry.Mips;

                    texentry.PixelCount = texentry._X * texentry._Y;

                    texentry.CSize = ((texentry._X / 4) * (texentry._Y / 4));

                    texentry.DSize = 8;

                    Array.Clear(DTemp, 0, 4);

                    int v = 0x10;

                    //Misc Properties in the header.
                    texentry.Dimensions = Convert.ToInt32(((LWData[0] >> 28) & 0xf));

                    texentry.MipOffsets = new int[texentry.MipMapCount];

                    #region CubeMaps
                    //For CubeMaps.
                    if (texentry.Dimensions == 6)
                    {
                        texentry._Format = "Cube Map(Unsupported)";
                        texentry.IsCubeMap = true;

                        //Faces.



                        return texentry;
                    }
                    #endregion

                    for (int i = 0; i < texentry._MipMapCount; i++)
                    {
                        //Gets offsets of MipMapData.
                        brStream.BaseStream.Position = v;
                        texentry.MipOffsets[i] = brStream.ReadInt32();
                        v = v + 8;
                    }

                    v = 0x10;

                    int w = 0;
                    int u = 0;

                    //Extracts and separates the Mip Maps.
                    texentry.OutMapsB = new byte[texentry._MipMapCount][];

                    for (int i = 0; i < texentry._MipMapCount; i++)
                    {

                        if ((i) == (texentry.MipOffsets.Length - 1))
                        {
                            texentry.WTemp = new byte[(brStream.BaseStream.Length - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)];

                            //Array.Copy(TexStream.ToArray(), texentry.MipOffsets[i], texentry.WTemp, 0, (Convert.ToInt32(brStream.BaseStream.Length) - texentry.MipOffsets[i]));
                            Array.Copy(TexStream.ToArray(), texentry.MipOffsets[i], texentry.WTemp, 0, (Convert.ToInt32(brStream.BaseStream.Length) - texentry.MipOffsets[i]));

                            w = texentry.WTemp.Length;
                            u = u + texentry.WTemp.Length;

                            texentry.OutMaps = texentry.WTemp;
                            texentry.OutMapsB[i] = texentry.OutMaps;
                        }
                        else
                        {
                            texentry.WTemp = new byte[(texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)];


                            Array.Copy(TexStream.ToArray(), texentry.MipOffsets[i], texentry.WTemp, 0, (texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i]));

                            w = texentry.WTemp.Length;
                            u = u + texentry.WTemp.Length;

                            texentry.OutMaps = texentry.WTemp;
                            texentry.OutMapsB[i] = texentry.OutMaps;
                        }

                    }

                    //Debug Export. Gotta use this for the export as well.
                    texentry.OutTexTest = new List<byte>();


                    byte[] DDSHeader = { 0x44, 0x44, 0x53, 0x20, 0x7c, 0x00, 0x00, 0x00, 0x07, 0x10, 0x08, 0x00, 0x00, 0x01, 0x00, 0x00,
                                         0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                                         0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x31, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                    if (texentry.FileName.Contains("NOMIP"))
                    {
                        DDSHeader[28] = Convert.ToByte(1);
                    }
                    else
                    {
                        DDSHeader[28] = Convert.ToByte(texentry.MipMapCount);
                    }

                    texentry.OutTexTest.AddRange(DDSHeader);

                    foreach (byte[] array in texentry.OutMapsB)
                    {
                        texentry.OutTexTest.AddRange(array);
                    }

                    int findex = filename.LastIndexOf("\\");
                    string outname = (filename.Substring(0, findex) + "\\") + texentry.TrueName;
                    string outpngname = outname + ".png";
                    outname = outname + ".dds";

                    texentry.OutMaps = texentry.OutTexTest.ToArray();
                    uint blargx = Convert.ToUInt32(texentry.XSize);
                    uint blargy = Convert.ToUInt32(texentry.YSize);

                    byte[] Xbytes = BitConverter.GetBytes(blargy);
                    byte[] Ybytes = BitConverter.GetBytes(blargx);

                    Array.Copy(Xbytes, 0, texentry.OutMaps, 12, 4);
                    Array.Copy(Ybytes, 0, texentry.OutMaps, 16, 4);

                    //Test to setup a nomip dds file of read texture for preview reasons.
                    byte[] DHTemp =    { 0x44, 0x44, 0x53, 0x20, 0x7c, 0x00, 0x00, 0x00, 0x07, 0x10, 0x08, 0x00, 0x00, 0x01, 0x00, 0x00,
                                         0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                                         0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x31, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                    byte[] LenTemp = BitConverter.GetBytes(texentry.YSize);
                    byte[] WidTemp = BitConverter.GetBytes(texentry.XSize);

                    Array.Copy(LenTemp, 0, DHTemp, 12, 4);
                    Array.Copy(WidTemp, 0, DHTemp, 16, 4);

                    texentry = DDSToRGBA(texentry);

                    break;

                #endregion

                #region Bitmap Textures with Transparency
                case "17":
                    texentry._Format = "DXT5/BC3";

                    //Gets the unsigned integers which hold data on the texture's dimensions.
                    brStream.BaseStream.Position = 4;
                    LWData[0] = BitConverter.ToUInt32(brStream.ReadBytes(4), 0);
                    LWData[1] = BitConverter.ToUInt32(brStream.ReadBytes(4), 0);
                    LWData[2] = BitConverter.ToUInt32(brStream.ReadBytes(4), 0);

                    //X and Y coordinates. This method is borrowed from the old TexCheck.py file.
                    texentry.XSize = Convert.ToInt32(((LWData[1] >> 6) & 0x1fff));
                    texentry._X = texentry.XSize;

                    texentry.YSize = Convert.ToInt32(((LWData[1] >> 19) & 0x1fff));
                    texentry._Y = texentry.YSize;

                    texentry.Mips = Convert.ToInt32(((LWData[1]) & 0x3f));
                    texentry._MipMapCount = texentry.Mips;

                    texentry.PixelCount = texentry._X * texentry._Y;

                    texentry.CSize = ((texentry._X / 4) * (texentry._Y / 4));

                    texentry.DSize = 16;

                    Array.Clear(DTemp, 0, 4);

                    int v17 = 0x10;

                    texentry.MipOffsets = new int[texentry.MipMapCount];

                    for (int i = 0; i < texentry._MipMapCount; i++)
                    {
                        //Gets offsets of MipMapData.
                        brStream.BaseStream.Position = v17;
                        texentry.MipOffsets[i] = brStream.ReadInt32();
                        v17 = v17 + 8;
                    }
                    v = 0x10;
                    int w17 = 0;
                    int u17 = 0;
                    //Extracts and separates the Mip Maps.
                    texentry.OutMapsB = new byte[texentry._MipMapCount][];
                    for (int i = 0; i < texentry._MipMapCount; i++)
                    {
                        if ((i) == (texentry.MipOffsets.Length - 1))
                        {
                            texentry.WTemp = new byte[(Convert.ToInt32(TexStream.Length) - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)];
                            Array.Copy(TexStream.ToArray(), texentry.MipOffsets[i], texentry.WTemp, 0, (Convert.ToInt32(TexStream.Length) - texentry.MipOffsets[i]));
                            w17 = texentry.WTemp.Length;
                            u17 = u17 + texentry.WTemp.Length;
                            texentry.OutMaps = texentry.WTemp;
                            texentry.OutMapsB[i] = texentry.OutMaps;
                        }
                        else
                        {
                            texentry.WTemp = new byte[(texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)]; Array.Copy(TexStream.ToArray(), texentry.MipOffsets[i], texentry.WTemp, 0, (texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i]));
                            w = texentry.WTemp.Length;
                            u17 = u17 + texentry.WTemp.Length;
                            texentry.OutMaps = texentry.WTemp;
                            texentry.OutMapsB[i] = texentry.OutMaps;
                        }
                    }
                    //Debug Export. Gotta use this for the export as well.
                    texentry.OutTexTest = new List<byte>();



                    byte[] DDSHeader17 = { 0x44, 0x44, 0x53, 0x20, 0x7c, 0x00, 0x00, 0x00, 0x07, 0x10, 0x08, 0x00, 0x00, 0x01, 0x00, 0x00,
                                         0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                                         0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x35, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                    if (texentry.FileName.Contains("NOMIP"))
                    {
                        DDSHeader17[28] = Convert.ToByte(1);
                    }
                    else
                    {
                        DDSHeader17[28] = Convert.ToByte(texentry.MipMapCount);
                    }

                    texentry.OutTexTest.AddRange(DDSHeader17);

                    foreach (byte[] array in texentry.OutMapsB)
                    {
                        texentry.OutTexTest.AddRange(array);
                    }

                    int findex17 = filename.LastIndexOf("\\");
                    string outname17 = (filename.Substring(0, findex17) + "\\") + texentry.TrueName;
                    string outpngname17 = outname17 + ".png";
                    outname17 = outname17 + ".dds";

                    texentry.OutMaps = texentry.OutTexTest.ToArray();

                    uint blargx17 = Convert.ToUInt32(texentry.XSize);
                    uint blargy17 = Convert.ToUInt32(texentry.YSize);

                    byte[] Xbytes17 = BitConverter.GetBytes(blargy17);
                    byte[] Ybytes17 = BitConverter.GetBytes(blargx17);

                    Array.Copy(Xbytes17, 0, texentry.OutMaps, 12, 4);
                    Array.Copy(Ybytes17, 0, texentry.OutMaps, 16, 4);

                    texentry = DDSToRGBA(texentry);

                    break;

                #endregion

                #region Specular Tetures
                case "19":

                    texentry._Format = "BC4_UNORM/Metalic/Specular Map";

                    //Gets the unsigned integers which hold data on the texture's dimensions.
                    brStream.BaseStream.Position = 4;
                    LWData[0] = BitConverter.ToUInt32(brStream.ReadBytes(4), 0);
                    LWData[1] = BitConverter.ToUInt32(brStream.ReadBytes(4), 0);
                    LWData[2] = BitConverter.ToUInt32(brStream.ReadBytes(4), 0);

                    //X and Y coordinates. This method is borrowed from the old TexCheck.py file.
                    texentry.XSize = Convert.ToInt32(((LWData[1] >> 6) & 0x1fff));
                    texentry._X = texentry.XSize;

                    texentry.YSize = Convert.ToInt32(((LWData[1] >> 19) & 0x1fff));
                    texentry._Y = texentry.YSize;

                    texentry.Mips = Convert.ToInt32(((LWData[1]) & 0x3f));
                    texentry._MipMapCount = texentry.Mips;

                    texentry.PixelCount = texentry._X * texentry._Y;

                    texentry.CSize = ((texentry._X / 4) * (texentry._Y / 4));

                    texentry.DSize = 8;

                    Array.Clear(DTemp, 0, 4);

                    int v19 = 0x10;

                    texentry.MipOffsets = new int[texentry.MipMapCount];

                    for (int i = 0; i < texentry._MipMapCount; i++)
                    {
                        //Gets offsets of MipMapData.
                        brStream.BaseStream.Position = v19;
                        texentry.MipOffsets[i] = brStream.ReadInt32();
                        v19 = v19 + 8;
                    }

                    v19 = 0x10;

                    int w19 = 0;
                    int u19 = 0;

                    //Extracts and separates the Mip Maps.
                    texentry.OutMapsB = new byte[texentry._MipMapCount][];

                    for (int i = 0; i < texentry._MipMapCount; i++)
                    {

                        if ((i) == (texentry.MipOffsets.Length - 1))
                        {
                            texentry.WTemp = new byte[(Convert.ToInt32(TexStream.Length) - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)];

                            Array.Copy(TexStream.ToArray(), texentry.MipOffsets[i], texentry.WTemp, 0, (Convert.ToInt32(TexStream.Length) - texentry.MipOffsets[i]));
                            w19 = texentry.WTemp.Length;
                            u19 = u19 + texentry.WTemp.Length;

                            texentry.OutMaps = texentry.WTemp;
                            texentry.OutMapsB[i] = texentry.OutMaps;
                        }
                        else
                        {
                            texentry.WTemp = new byte[(texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)];
                            Array.Copy(TexStream.ToArray(), texentry.MipOffsets[i], texentry.WTemp, 0, (texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i]));
                            w19 = texentry.WTemp.Length;
                            u19 = u19 + texentry.WTemp.Length;

                            texentry.OutMaps = texentry.WTemp;
                            texentry.OutMapsB[i] = texentry.OutMaps;
                        }

                    }

                    //Debug Export. Gotta use this for the export as well.
                    texentry.OutTexTest = new List<byte>();


                    byte[] DDSHeader19 = { 0x44, 0x44, 0x53, 0x20, 0x7c, 0x00, 0x00, 0x00, 0x07, 0x10, 0x08, 0x00, 0x00, 0x01, 0x00, 0x00,
                                         0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                                         0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x31, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    if (texentry.FileName.Contains("NOMIP"))
                    {
                        DDSHeader19[28] = Convert.ToByte(1);
                    }
                    else
                    {
                        DDSHeader19[28] = Convert.ToByte(texentry.MipMapCount);
                    }

                    texentry.OutTexTest.AddRange(DDSHeader19);

                    foreach (byte[] array in texentry.OutMapsB)
                    {
                        texentry.OutTexTest.AddRange(array);
                    }

                    int findex19 = filename.LastIndexOf("\\");
                    string outname19 = (filename.Substring(0, findex19) + "\\") + texentry.TrueName;
                    string outpngname19 = outname19 + ".png";
                    outname19 = outname19 + ".dds";

                    texentry.OutMaps = texentry.OutTexTest.ToArray();
                    uint blargx19 = Convert.ToUInt32(texentry.XSize);
                    uint blargy19 = Convert.ToUInt32(texentry.YSize);

                    byte[] Xbytes19 = BitConverter.GetBytes(blargy19);
                    byte[] Ybytes19 = BitConverter.GetBytes(blargx19);

                    Array.Copy(Xbytes19, 0, texentry.OutMaps, 12, 4);
                    Array.Copy(Ybytes19, 0, texentry.OutMaps, 16, 4);

                    //Test to setup a nomip dds file of read texture for preview reasons.
                    byte[] DHTemp19 =    { 0x44, 0x44, 0x53, 0x20, 0x7c, 0x00, 0x00, 0x00, 0x07, 0x10, 0x08, 0x00, 0x00, 0x01, 0x00, 0x00,
                                         0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                                         0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x31, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                    byte[] LenTemp19 = BitConverter.GetBytes(texentry.YSize);
                    byte[] WidTemp19 = BitConverter.GetBytes(texentry.XSize);

                    Array.Copy(LenTemp19, 0, DHTemp19, 12, 4);
                    Array.Copy(WidTemp19, 0, DHTemp19, 16, 4);

                    texentry = DDSToRGBA(texentry);

                    break;

                #endregion

                #region Unknown Cloth Textures
                case "1E":
                    texentry._Format = "Cloth?";

                    //Gets the unsigned integers which hold data on the texture's dimensions.
                    brStream.BaseStream.Position = 4;
                    LWData[0] = BitConverter.ToUInt32(brStream.ReadBytes(4), 0);
                    LWData[1] = BitConverter.ToUInt32(brStream.ReadBytes(4), 0);
                    LWData[2] = BitConverter.ToUInt32(brStream.ReadBytes(4), 0);

                    //X and Y coordinates. This method is borrowed from the old TexCheck.py file.
                    texentry.XSize = Convert.ToInt32(((LWData[1] >> 6) & 0x1fff));
                    texentry._X = texentry.XSize;

                    texentry.YSize = Convert.ToInt32(((LWData[1] >> 19) & 0x1fff));
                    texentry._Y = texentry.YSize;

                    texentry.Mips = Convert.ToInt32(((LWData[1]) & 0x3f));
                    texentry._MipMapCount = texentry.Mips;

                    texentry.PixelCount = texentry._X * texentry._Y;

                    texentry.CSize = ((texentry._X / 4) * (texentry._Y / 4));

                    texentry.DSize = 16;

                    Array.Clear(DTemp, 0, 4);

                    int v1e = 0x10;

                    texentry.MipOffsets = new int[texentry.MipMapCount];

                    for (int i = 0; i < texentry._MipMapCount; i++)
                    {
                        //Gets offsets of MipMapData.
                        brStream.BaseStream.Position = v1e;
                        texentry.MipOffsets[i] = brStream.ReadInt32();
                        v1e = v1e + 8;
                    }

                    v = 0x10;

                    int w1e = 0;
                    int u1e = 0;

                    //Extracts and separates the Mip Maps.
                    texentry.OutMapsB = new byte[texentry._MipMapCount][];

                    for (int i = 0; i < texentry._MipMapCount; i++)
                    {

                        if ((i) == (texentry.MipOffsets.Length - 1))
                        {
                            texentry.WTemp = new byte[(Convert.ToInt32(TexStream.Length) - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)];

                            Array.Copy(TexStream.ToArray(), texentry.MipOffsets[i], texentry.WTemp, 0, (Convert.ToInt32(TexStream.Length) - texentry.MipOffsets[i]));
                            w1e = texentry.WTemp.Length;
                            u1e = u1e + texentry.WTemp.Length;

                            texentry.OutMaps = texentry.WTemp;
                            texentry.OutMapsB[i] = texentry.OutMaps;
                        }
                        else
                        {
                            texentry.WTemp = new byte[(texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)];
                            Array.Copy(TexStream.ToArray(), texentry.MipOffsets[i], texentry.WTemp, 0, (texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i]));
                            w = texentry.WTemp.Length;
                            u1e = u1e + texentry.WTemp.Length;

                            texentry.OutMaps = texentry.WTemp;
                            texentry.OutMapsB[i] = texentry.OutMaps;
                        }

                    }

                    //Debug Export. Gotta use this for the export as well.
                    texentry.OutTexTest = new List<byte>();


                    byte[] DDSHeader1e = { 0x44, 0x44, 0x53, 0x20, 0x7c, 0x00, 0x00, 0x00, 0x07, 0x10, 0x08, 0x00, 0x00, 0x01, 0x00, 0x00,
                                         0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                                         0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x31, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                    if (texentry.FileName.Contains("NOMIP"))
                    {
                        DDSHeader1e[28] = Convert.ToByte(1);
                    }
                    else
                    {
                        DDSHeader1e[28] = Convert.ToByte(texentry.MipMapCount);
                    }

                    texentry.OutTexTest.AddRange(DDSHeader1e);

                    foreach (byte[] array in texentry.OutMapsB)
                    {
                        texentry.OutTexTest.AddRange(array);
                    }

                    int findex1e = filename.LastIndexOf("\\");
                    string outname1e = (filename.Substring(0, findex1e) + "\\") + texentry.TrueName;
                    string outpngname1e = outname1e + ".png";
                    outname1e = outname1e + ".dds";

                    texentry.OutMaps = texentry.OutTexTest.ToArray();
                    uint blargx1e = Convert.ToUInt32(texentry.XSize);
                    uint blargy1e = Convert.ToUInt32(texentry.YSize);

                    byte[] Xbytes1e = BitConverter.GetBytes(blargy1e);
                    byte[] Ybytes1e = BitConverter.GetBytes(blargx1e);

                    Array.Copy(Xbytes1e, 0, texentry.OutMaps, 12, 4);
                    Array.Copy(Ybytes1e, 0, texentry.OutMaps, 16, 4);

                    texentry = DDSToRGBA(texentry);

                    break;
                #endregion

                #region Normal Maps(Incomplete)
                case "1F":

                    texentry._Format = "BC5/Normal Map";

                    //Gets the unsigned integers which hold data on the texture's dimensions.
                    brStream.BaseStream.Position = 4;
                    LWData[0] = BitConverter.ToUInt32(brStream.ReadBytes(4), 0);
                    LWData[1] = BitConverter.ToUInt32(brStream.ReadBytes(4), 0);
                    LWData[2] = BitConverter.ToUInt32(brStream.ReadBytes(4), 0);

                    //X and Y coordinates. This method is borrowed from the old TexCheck.py file.
                    texentry.XSize = Convert.ToInt32(((LWData[1] >> 6) & 0x1fff));
                    texentry._X = texentry.XSize;

                    texentry.YSize = Convert.ToInt32(((LWData[1] >> 19) & 0x1fff));
                    texentry._Y = texentry.YSize;

                    texentry.Mips = Convert.ToInt32(((LWData[1]) & 0x3f));
                    texentry._MipMapCount = texentry.Mips;

                    texentry.PixelCount = texentry._X * texentry._Y;

                    texentry.CSize = ((texentry._X / 4) * (texentry._Y / 4));

                    texentry.DSize = 8;

                    Array.Clear(DTemp, 0, 4);

                    int v1f = 0x10;

                    texentry.MipOffsets = new int[texentry.MipMapCount];

                    for (int i = 0; i < texentry._MipMapCount; i++)
                    {
                        //Gets offsets of MipMapData.
                        brStream.BaseStream.Position = v1f;
                        texentry.MipOffsets[i] = brStream.ReadInt32();
                        v1f = v1f + 8;
                    }

                    v1f = 0x10;

                    int w1f = 0;
                    int u1f = 0;

                    //Extracts and separates the Mip Maps.
                    texentry.OutMapsB = new byte[texentry._MipMapCount][];

                    for (int i = 0; i < texentry._MipMapCount; i++)
                    {

                        if ((i) == (texentry.MipOffsets.Length - 1))
                        {
                            texentry.WTemp = new byte[(Convert.ToInt32(TexStream.Length) - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)];

                            Array.Copy(TexStream.ToArray(), texentry.MipOffsets[i], texentry.WTemp, 0, (Convert.ToInt32(TexStream.Length) - texentry.MipOffsets[i]));
                            w1f = texentry.WTemp.Length;
                            u1f = u1f + texentry.WTemp.Length;

                            texentry.OutMaps = texentry.WTemp;
                            texentry.OutMapsB[i] = texentry.OutMaps;
                        }
                        else
                        {
                            texentry.WTemp = new byte[(texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)];
                            Array.Copy(TexStream.ToArray(), texentry.MipOffsets[i], texentry.WTemp, 0, (texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i]));
                            w1f = texentry.WTemp.Length;
                            u1f = u1f + texentry.WTemp.Length;

                            texentry.OutMaps = texentry.WTemp;
                            texentry.OutMapsB[i] = texentry.OutMaps;
                        }

                    }

                    //Debug Export. Gotta use this for the export as well.
                    texentry.OutTexTest = new List<byte>();


                    byte[] DDSHeader1f = { 0x44, 0x44, 0x53, 0x20, 0x7c, 0x00, 0x00, 0x00, 0x07, 0x10, 0x08, 0x00, 0x00, 0x01, 0x00, 0x00,
                                         0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                                         0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x35, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                    if (texentry.FileName.Contains("NOMIP"))
                    {
                        DDSHeader1f[28] = Convert.ToByte(1);
                    }
                    else
                    {
                        DDSHeader1f[28] = Convert.ToByte(texentry.MipMapCount);
                    }


                    texentry.OutTexTest.AddRange(DDSHeader1f);

                    foreach (byte[] array in texentry.OutMapsB)
                    {
                        texentry.OutTexTest.AddRange(array);
                    }

                    int findex1f = filename.LastIndexOf("\\");
                    string outname1f = (filename.Substring(0, findex1f) + "\\") + texentry.TrueName;
                    string outpngname1f = outname1f + ".png";
                    outname1f = outname1f + ".dds";

                    texentry.OutMaps = texentry.OutTexTest.ToArray();
                    uint blargx1f = Convert.ToUInt32(texentry.XSize);
                    uint blargy1f = Convert.ToUInt32(texentry.YSize);

                    byte[] Xbytes1a = BitConverter.GetBytes(blargy1f);
                    byte[] Ybytes1a = BitConverter.GetBytes(blargx1f);

                    Array.Copy(Xbytes1a, 0, texentry.OutMaps, 12, 4);
                    Array.Copy(Ybytes1a, 0, texentry.OutMaps, 16, 4);

                    texentry = DDSToRGBA(texentry);

                    break;

                #endregion

                #region Weird Toon Shader Textures
                case "27":
                    texentry._Format = "RGBA Linear Texture";

                    //Gets the unsigned integers which hold data on the texture's dimensions.
                    brStream.BaseStream.Position = 4;
                    LWData[0] = BitConverter.ToUInt32(brStream.ReadBytes(4), 0);
                    LWData[1] = BitConverter.ToUInt32(brStream.ReadBytes(4), 0);
                    LWData[2] = BitConverter.ToUInt32(brStream.ReadBytes(4), 0);

                    //X and Y coordinates. This method is borrowed from the old TexCheck.py file.
                    texentry.XSize = Convert.ToInt32(((LWData[1] >> 6) & 0x1fff));
                    texentry._X = texentry.XSize;

                    texentry.YSize = Convert.ToInt32(((LWData[1] >> 19) & 0x1fff));
                    texentry._Y = texentry.YSize;

                    texentry.Mips = Convert.ToInt32(((LWData[1]) & 0x3f));
                    texentry._MipMapCount = texentry.Mips;

                    texentry.PixelCount = texentry._X * texentry._Y;

                    texentry.CSize = ((texentry._X / 4) * (texentry._Y / 4));

                    texentry.DSize = 16;

                    Array.Clear(DTemp, 0, 4);

                    int v27 = 0x10;

                    texentry.MipOffsets = new int[texentry.MipMapCount];

                    for (int i = 0; i < texentry._MipMapCount; i++)
                    {
                        //Gets offsets of MipMapData.
                        brStream.BaseStream.Position = v27;
                        texentry.MipOffsets[i] = brStream.ReadInt32();
                        v27 = v27 + 8;
                    }

                    v = 0x10;

                    int w27 = 0;
                    int u27 = 0;

                    //Extracts and separates the Mip Maps.
                    texentry.OutMapsB = new byte[texentry._MipMapCount][];

                    for (int i = 0; i < texentry._MipMapCount; i++)
                    {

                        if ((i) == (texentry.MipOffsets.Length - 1))
                        {
                            texentry.WTemp = new byte[(Convert.ToInt32(TexStream.Length) - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)];

                            Array.Copy(TexStream.ToArray(), texentry.MipOffsets[i], texentry.WTemp, 0, (Convert.ToInt32(TexStream.Length) - texentry.MipOffsets[i]));
                            w27 = texentry.WTemp.Length;
                            u27 = u27 + texentry.WTemp.Length;

                            texentry.OutMaps = texentry.WTemp;
                            texentry.OutMapsB[i] = texentry.OutMaps;
                        }
                        else
                        {
                            texentry.WTemp = new byte[(texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)];
                            Array.Copy(TexStream.ToArray(), texentry.MipOffsets[i], texentry.WTemp, 0, (texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i]));
                            w = texentry.WTemp.Length;
                            u27 = u27 + texentry.WTemp.Length;

                            texentry.OutMaps = texentry.WTemp;
                            texentry.OutMapsB[i] = texentry.OutMaps;
                        }

                    }


                    foreach (byte[] barray in texentry.OutMapsB)
                    {
                        for (int i = 0; i < barray.Length; i++)
                        {

                        }
                    }


                    //Debug Export. Gotta use this for the export as well.
                    texentry.OutTexTest = new List<byte>();


                    byte[] DDSHeader27 = { 0x44, 0x44, 0x53, 0x20, 0x7c, 0x00, 0x00, 0x00, 0x07, 0x10, 0x08, 0x00, 0x00, 0x01, 0x00, 0x00,
                                           0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                                           0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x35, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                    if (texentry.FileName.Contains("NOMIP"))
                    {
                        DDSHeader27[28] = Convert.ToByte(1);
                    }
                    else
                    {
                        DDSHeader27[28] = Convert.ToByte(texentry.MipMapCount);
                    }

                    texentry.OutTexTest.AddRange(DDSHeader27);

                    foreach (byte[] array in texentry.OutMapsB)
                    {
                        texentry.OutTexTest.AddRange(array);
                    }

                    int findex27 = filename.LastIndexOf("\\");
                    string outname27 = (filename.Substring(0, findex27) + "\\") + texentry.TrueName;
                    string outpngname27 = outname27 + ".png";
                    outname27 = outname27 + ".dds";

                    texentry.OutMaps = texentry.OutTexTest.ToArray();
                    uint blargx27 = Convert.ToUInt32(texentry.XSize);
                    uint blargy27 = Convert.ToUInt32(texentry.YSize);

                    byte[] Xbytes27 = BitConverter.GetBytes(blargy27);
                    byte[] Ybytes27 = BitConverter.GetBytes(blargx27);

                    Array.Copy(Xbytes27, 0, texentry.OutMaps, 12, 4);
                    Array.Copy(Ybytes27, 0, texentry.OutMaps, 16, 4);

                    //Test to setup a nomip dds file of read texture for preview reasons.
                    byte[] DHTemp27 =    { 0x44, 0x44, 0x53, 0x20, 0x7c, 0x00, 0x00, 0x00, 0x07, 0x10, 0x08, 0x00, 0x00, 0x01, 0x00, 0x00,
                                         0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                                         0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x31, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                    byte[] LenTemp27 = BitConverter.GetBytes(texentry.YSize);
                    byte[] WidTemp27 = BitConverter.GetBytes(texentry.XSize);

                    Array.Copy(LenTemp27, 0, DHTemp27, 12, 4);
                    Array.Copy(WidTemp27, 0, DHTemp27, 16, 4);

                    texentry = DDSToRGBA(texentry);

                    break;

                #endregion

                #region Weirdo Problematic Portrait Textures
                case "2A":

                    texentry._Format = "????/Problematic Portrait Picture";

                    //Gets the unsigned integers which hold data on the texture's dimensions.
                    brStream.BaseStream.Position = 4;
                    LWData[0] = BitConverter.ToUInt32(brStream.ReadBytes(4), 0);
                    LWData[1] = BitConverter.ToUInt32(brStream.ReadBytes(4), 0);
                    LWData[2] = BitConverter.ToUInt32(brStream.ReadBytes(4), 0);

                    //X and Y coordinates. This method is borrowed from the old TexCheck.py file.
                    texentry.XSize = Convert.ToInt32(((LWData[1] >> 6) & 0x1fff));
                    texentry._X = texentry.XSize;

                    texentry.YSize = Convert.ToInt32(((LWData[1] >> 19) & 0x1fff));
                    texentry._Y = texentry.YSize;

                    texentry.Mips = Convert.ToInt32(((LWData[1]) & 0x3f));
                    texentry._MipMapCount = texentry.Mips;

                    texentry.PixelCount = texentry._X * texentry._Y;

                    texentry.CSize = ((texentry._X / 4) * (texentry._Y / 4));

                    texentry.DSize = 16;

                    Array.Clear(DTemp, 0, 4);

                    int v2a = 0x10;

                    texentry.MipOffsets = new int[texentry.MipMapCount];

                    for (int i = 0; i < texentry._MipMapCount; i++)
                    {
                        //Gets offsets of MipMapData.
                        brStream.BaseStream.Position = v2a;
                        texentry.MipOffsets[i] = brStream.ReadInt32();
                        v2a = v2a + 8;
                    }

                    v = 0x10;

                    int w2a = 0;
                    int u2a = 0;

                    //Extracts and separates the Mip Maps.
                    texentry.OutMapsB = new byte[texentry._MipMapCount][];

                    for (int i = 0; i < texentry._MipMapCount; i++)
                    {

                        if ((i) == (texentry.MipOffsets.Length - 1))
                        {
                            texentry.WTemp = new byte[(Convert.ToInt32(TexStream.Length) - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)];

                            Array.Copy(TexStream.ToArray(), texentry.MipOffsets[i], texentry.WTemp, 0, (Convert.ToInt32(TexStream.Length) - texentry.MipOffsets[i]));
                            w2a = texentry.WTemp.Length;
                            u2a = u2a + texentry.WTemp.Length;

                            texentry.OutMaps = texentry.WTemp;
                            texentry.OutMapsB[i] = texentry.OutMaps;
                        }
                        else
                        {
                            texentry.WTemp = new byte[(texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)];
                            Array.Copy(TexStream.ToArray(), texentry.MipOffsets[i], texentry.WTemp, 0, (texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i]));
                            w = texentry.WTemp.Length;
                            u2a = u2a + texentry.WTemp.Length;

                            texentry.OutMaps = texentry.WTemp;
                            texentry.OutMapsB[i] = texentry.OutMaps;
                        }

                    }

                    //Special test for these problematic portraits.

                    List<byte> SpecialSwapper = new List<byte>();

                    //Debug Export. Gotta use this for the export as well.
                    texentry.OutTexTest = new List<byte>();


                    byte[] DDSHeader2a = { 0x44, 0x44, 0x53, 0x20, 0x7c, 0x00, 0x00, 0x00, 0x07, 0x10, 0x08, 0x00, 0x00, 0x04, 0x00, 0x00,
                                           0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                                           0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x35, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                    if (texentry.FileName.Contains("NOMIP"))
                    {
                        DDSHeader2a[28] = Convert.ToByte(1);
                    }
                    else
                    {
                        DDSHeader2a[28] = Convert.ToByte(texentry.MipMapCount);
                    }


                    texentry.OutTexTest.AddRange(DDSHeader2a);

                    foreach (byte[] array in texentry.OutMapsB)
                    {
                        texentry.OutTexTest.AddRange(array);
                    }

                    int findex2a = filename.LastIndexOf("\\");
                    string outname2a = (filename.Substring(0, findex2a) + "\\") + texentry.TrueName;
                    string outpngname2a = outname2a + ".png";
                    outname2a = outname2a + ".dds";

                    texentry.OutMaps = texentry.OutTexTest.ToArray();
                    uint blargx2a = Convert.ToUInt32(texentry.XSize);
                    uint blargy2a = Convert.ToUInt32(texentry.YSize);

                    byte[] Xbytes2a = BitConverter.GetBytes(blargy2a);
                    byte[] Ybytes2a = BitConverter.GetBytes(blargx2a);

                    Array.Copy(Xbytes2a, 0, texentry.OutMaps, 12, 4);
                    Array.Copy(Ybytes2a, 0, texentry.OutMaps, 16, 4);


                    texentry = DDSToRGBA(texentry);

                    break;

                #endregion

                #region Everything Else
                default:
                    texentry._Format = "??????";

                    //Gets the unsigned integers which hold data on the texture's dimensions.
                    brStream.BaseStream.Position = 4;
                    LWData[0] = BitConverter.ToUInt32(brStream.ReadBytes(4), 0);
                    LWData[1] = BitConverter.ToUInt32(brStream.ReadBytes(4), 0);
                    LWData[2] = BitConverter.ToUInt32(brStream.ReadBytes(4), 0);

                    //X and Y coordinates. This method is borrowed from the old TexCheck.py file.
                    texentry.XSize = Convert.ToInt32(((LWData[1] >> 6) & 0x1fff));
                    texentry._X = texentry.XSize;

                    texentry.YSize = Convert.ToInt32(((LWData[1] >> 19) & 0x1fff));
                    texentry._Y = texentry.YSize;

                    texentry.Mips = Convert.ToInt32(((LWData[1]) & 0x3f));
                    texentry._MipMapCount = texentry.Mips;

                    texentry.PixelCount = texentry._X * texentry._Y;

                    texentry.CSize = ((texentry._X / 4) * (texentry._Y / 4));

                    texentry.DSize = 16;

                    Array.Clear(DTemp, 0, 4);

                    texentry.Dimensions = Convert.ToInt32(((LWData[0] >> 28) & 0xf));

                    texentry.MipOffsets = new int[texentry.MipMapCount];

                    //For CubeMaps.
                    if (texentry.Dimensions == 6)
                    {
                        texentry._Format = "Cube Map(Unsupported)";

                        break;
                    }

                    break;

                    #endregion

            }

            return texentry;

        }

        public static TextureEntry InitializeTextureI(TreeView tree, TextureEntry teXentry, BinaryReader bnr, string filename)
        {
            List<byte> BTemp = new List<byte>();
            //Actual Tex Loading work here.
            byte[] VTemp = new byte[4];
            uint[] LWData = new uint[3];
            byte[] DTemp = new byte[4];

            switch (teXentry.TexType)
            {
                #region Bitmap Textures
                case "13":

                    teXentry._Format = "DXT1/BC1";

                    //Gets the unsigned integers which hold data on the texture's dimensions.
                    bnr.BaseStream.Position = 4;
                    LWData[0] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);
                    LWData[1] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);
                    LWData[2] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);

                    //X and Y coordinates. This method is borrowed from the old TexCheck.py file.
                    teXentry.XSize = Convert.ToInt32(((LWData[1] >> 6) & 0x1fff));
                    teXentry._X = teXentry.XSize;

                    teXentry.YSize = Convert.ToInt32(((LWData[1] >> 19) & 0x1fff));
                    teXentry._Y = teXentry.YSize;

                    teXentry.Mips = Convert.ToInt32(((LWData[1]) & 0x3f));
                    teXentry._MipMapCount = teXentry.Mips;

                    teXentry.PixelCount = teXentry._X * teXentry._Y;

                    teXentry.CSize = ((teXentry._X / 4) * (teXentry._Y / 4));

                    teXentry.DSize = 8;

                    Array.Clear(DTemp, 0, 4);

                    int v = 0x10;

                    teXentry.MipOffsets = new int[teXentry.MipMapCount];

                    for (int i = 0; i < teXentry._MipMapCount; i++)
                    {
                        //Gets offsets of MipMapData.
                        bnr.BaseStream.Position = v;
                        teXentry.MipOffsets[i] = bnr.ReadInt32();
                        v = v + 8;
                    }

                    v = 0x10;

                    int w = 0;
                    int u = 0;

                    //Extracts and separates the Mip Maps.
                    teXentry.OutMapsB = new byte[teXentry._MipMapCount][];

                    for (int i = 0; i < teXentry._MipMapCount; i++)
                    {

                        if ((i) == (teXentry.MipOffsets.Length - 1))
                        {
                            teXentry.WTemp = new byte[(teXentry.UncompressedData.Length - teXentry.MipOffsets[i])];
                            teXentry.OutMaps = new byte[(teXentry._MipMapCount)];

                            System.Buffer.BlockCopy(teXentry.UncompressedData, teXentry.MipOffsets[i], teXentry.WTemp, 0, (teXentry.UncompressedData.Length - teXentry.MipOffsets[i]));
                            w = teXentry.WTemp.Length;
                            u = u + teXentry.WTemp.Length;

                            teXentry.OutMaps = teXentry.WTemp;
                            teXentry.OutMapsB[i] = teXentry.OutMaps;
                        }
                        else
                        {
                            teXentry.WTemp = new byte[(teXentry.MipOffsets[(i + 1)] - teXentry.MipOffsets[i])];
                            teXentry.OutMaps = new byte[(teXentry._MipMapCount)];
                            System.Buffer.BlockCopy(teXentry.UncompressedData, teXentry.MipOffsets[i], teXentry.WTemp, 0, (teXentry.MipOffsets[(i + 1)] - teXentry.MipOffsets[i]));
                            w = teXentry.WTemp.Length;
                            u = u + teXentry.WTemp.Length;

                            teXentry.OutMaps = teXentry.WTemp;
                            teXentry.OutMapsB[i] = teXentry.OutMaps;
                        }

                    }

                    //Debug Export. Gotta use this for the export as well.
                    teXentry.OutTexTest = new List<byte>();


                    byte[] DDSHeader = { 0x44, 0x44, 0x53, 0x20, 0x7c, 0x00, 0x00, 0x00, 0x07, 0x10, 0x08, 0x00, 0x00, 0x01, 0x00, 0x00,
                                                0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                                                0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x31, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                                                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                    teXentry.OutTexTest.AddRange(DDSHeader);

                    foreach (byte[] array in teXentry.OutMapsB)
                    {
                        teXentry.OutTexTest.AddRange(array);
                    }

                    teXentry.OutMaps = teXentry.OutTexTest.ToArray();
                    uint blargx = Convert.ToUInt32(teXentry.XSize);
                    uint blargy = Convert.ToUInt32(teXentry.YSize);

                    byte[] Xbytes = BitConverter.GetBytes(blargy);
                    byte[] Ybytes = BitConverter.GetBytes(blargx);

                    Array.Copy(Xbytes, 0, teXentry.OutMaps, 12, 4);
                    Array.Copy(Ybytes, 0, teXentry.OutMaps, 16, 4);

                    #region ToRGBAPNG
                    //Time to convert this to png for RGBA related reasons.
                    byte[] DDSTemp = new byte[] { };
                    byte[] RGBATemp = new byte[] { };
                    DDSTemp = teXentry.OutMaps;

                    using (Stream strim = new MemoryStream(DDSTemp))
                    {
                        using (var image = Pfim.Pfim.FromStream(strim))
                        {
                            PixelFormat format;

                            // Convert from Pfim's backend agnostic image format into GDI+'s image format
                            switch (image.Format)
                            {
                                case Pfim.ImageFormat.Rgba32:
                                    format = PixelFormat.Format32bppArgb;
                                    break;
                                case Pfim.ImageFormat.Rgb24:
                                    format = PixelFormat.Format24bppRgb;
                                    break;
                                default:
                                    // see the sample for more details
                                    throw new NotImplementedException();
                            }

                            // Pin pfim's data array so that it doesn't get reaped by GC, unnecessary
                            // in this snippet but useful technique if the data was going to be used in
                            // control like a picture box
                            var handle = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
                            try
                            {
                                var data = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);
                                var bitmap = new Bitmap(image.Width, image.Height, image.Stride, format, data);


                                using (var stream = new MemoryStream())
                                {
                                    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                                    RGBATemp = stream.ToArray();
                                }


                            }
                            finally
                            {
                                handle.Free();
                            }


                        }
                    }

                    teXentry.OutMaps = RGBATemp;

                    teXentry.Picture = ByteUtilitarian.BitmapBuilderDX(teXentry.OutMaps, teXentry);
                    #endregion

                    break;

                #endregion

                #region Bitmap Textures with Transparency
                case "17":
                    teXentry._Format = "DXT5/BC3";

                    //Gets the unsigned integers which hold data on the texture's dimensions.
                    bnr.BaseStream.Position = 4;
                    LWData[0] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);
                    LWData[1] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);
                    LWData[2] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);

                    //X and Y coordinates. This method is borrowed from the old TexCheck.py file.
                    teXentry.XSize = Convert.ToInt32(((LWData[1] >> 6) & 0x1fff));
                    teXentry._X = teXentry.XSize;

                    teXentry.YSize = Convert.ToInt32(((LWData[1] >> 19) & 0x1fff));
                    teXentry._Y = teXentry.YSize;

                    teXentry.Mips = Convert.ToInt32(((LWData[1]) & 0x3f));
                    teXentry._MipMapCount = teXentry.Mips;

                    teXentry.PixelCount = teXentry._X * teXentry._Y;

                    teXentry.CSize = ((teXentry._X / 4) * (teXentry._Y / 4));

                    teXentry.DSize = 16;

                    Array.Clear(DTemp, 0, 4);

                    int v17 = 0x10;

                    teXentry.MipOffsets = new int[teXentry.MipMapCount];

                    for (int i = 0; i < teXentry._MipMapCount; i++)
                    {
                        //Gets offsets of MipMapData.
                        bnr.BaseStream.Position = v17;
                        teXentry.MipOffsets[i] = bnr.ReadInt32();
                        v17 = v17 + 8;
                    }

                    v = 0x10;

                    int w17 = 0;
                    int u17 = 0;

                    //Extracts and separates the Mip Maps.
                    teXentry.OutMapsB = new byte[teXentry._MipMapCount][];

                    for (int i = 0; i < teXentry._MipMapCount; i++)
                    {

                        if ((i) == (teXentry.MipOffsets.Length - 1))
                        {
                            teXentry.WTemp = new byte[(teXentry.UncompressedData.Length - teXentry.MipOffsets[i])];
                            teXentry.OutMaps = new byte[(teXentry._MipMapCount)];

                            System.Buffer.BlockCopy(teXentry.UncompressedData, teXentry.MipOffsets[i], teXentry.WTemp, 0, (teXentry.UncompressedData.Length - teXentry.MipOffsets[i]));
                            w17 = teXentry.WTemp.Length;
                            u17 = u17 + teXentry.WTemp.Length;

                            teXentry.OutMaps = teXentry.WTemp;
                            teXentry.OutMapsB[i] = teXentry.OutMaps;
                        }
                        else
                        {
                            teXentry.WTemp = new byte[(teXentry.MipOffsets[(i + 1)] - teXentry.MipOffsets[i])];
                            teXentry.OutMaps = new byte[(teXentry._MipMapCount)];
                            System.Buffer.BlockCopy(teXentry.UncompressedData, teXentry.MipOffsets[i], teXentry.WTemp, 0, (teXentry.MipOffsets[(i + 1)] - teXentry.MipOffsets[i]));
                            w = teXentry.WTemp.Length;
                            u17 = u17 + teXentry.WTemp.Length;

                            teXentry.OutMaps = teXentry.WTemp;
                            teXentry.OutMapsB[i] = teXentry.OutMaps;
                        }

                    }

                    teXentry.OutTexTest = new List<byte>();


                    byte[] DDSHeader17 = { 0x44, 0x44, 0x53, 0x20, 0x7c, 0x00, 0x00, 0x00, 0x07, 0x10, 0x08, 0x00, 0x00, 0x01, 0x00, 0x00,
                                         0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                                         0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x35, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                    teXentry.OutTexTest.AddRange(DDSHeader17);

                    foreach (byte[] array in teXentry.OutMapsB)
                    {
                        teXentry.OutTexTest.AddRange(array);
                    }

                    teXentry.OutMaps = teXentry.OutTexTest.ToArray();
                    uint blargx17 = Convert.ToUInt32(teXentry.XSize);
                    uint blargy17 = Convert.ToUInt32(teXentry.YSize);

                    byte[] Xbytes17 = BitConverter.GetBytes(blargy17);
                    byte[] Ybytes17 = BitConverter.GetBytes(blargx17);

                    Array.Copy(Xbytes17, 0, teXentry.OutMaps, 12, 4);
                    Array.Copy(Ybytes17, 0, teXentry.OutMaps, 16, 4);

                    #region ToRGBAPNG
                    //Time to convert this to png for RGBA related reasons.
                    byte[] DDSTemp17 = new byte[] { };
                    byte[] RGBATemp17 = new byte[] { };
                    DDSTemp17 = teXentry.OutMaps;

                    using (Stream strim = new MemoryStream(DDSTemp17))
                    {
                        using (var image = Pfim.Pfim.FromStream(strim))
                        {
                            PixelFormat format;

                            // Convert from Pfim's backend agnostic image format into GDI+'s image format
                            switch (image.Format)
                            {
                                case Pfim.ImageFormat.Rgba32:
                                    format = PixelFormat.Format32bppArgb;
                                    break;
                                case Pfim.ImageFormat.Rgb24:
                                    format = PixelFormat.Format24bppRgb;
                                    break;
                                default:
                                    // see the sample for more details
                                    throw new NotImplementedException();
                            }

                            // Pin pfim's data array so that it doesn't get reaped by GC, unnecessary
                            // in this snippet but useful technique if the data was going to be used in
                            // control like a picture box
                            var handle = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
                            try
                            {
                                var data = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);
                                var bitmap = new Bitmap(image.Width, image.Height, image.Stride, format, data);


                                using (var stream = new MemoryStream())
                                {
                                    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                                    RGBATemp17 = stream.ToArray();
                                }


                            }
                            finally
                            {
                                handle.Free();
                            }


                        }
                    }

                    teXentry.OutMaps = RGBATemp17;

                    teXentry.Picture = ByteUtilitarian.BitmapBuilderDX(teXentry.OutMaps, teXentry);
                    #endregion

                    break;
                #endregion

                #region Specular Tetures
                case "19":

                    teXentry._Format = "Metalic/Specular Map";

                    //Gets the unsigned integers which hold data on the texture's dimensions.
                    bnr.BaseStream.Position = 4;
                    LWData[0] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);
                    LWData[1] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);
                    LWData[2] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);

                    //X and Y coordinates. This method is borrowed from the old TexCheck.py file.
                    teXentry.XSize = Convert.ToInt32(((LWData[1] >> 6) & 0x1fff));
                    teXentry._X = teXentry.XSize;

                    teXentry.YSize = Convert.ToInt32(((LWData[1] >> 19) & 0x1fff));
                    teXentry._Y = teXentry.YSize;

                    teXentry.Mips = Convert.ToInt32(((LWData[1]) & 0x3f));
                    teXentry._MipMapCount = teXentry.Mips;

                    teXentry.PixelCount = teXentry._X * teXentry._Y;

                    teXentry.CSize = ((teXentry._X / 4) * (teXentry._Y / 4));

                    teXentry.DSize = 8;

                    Array.Clear(DTemp, 0, 4);

                    int v19 = 0x10;

                    teXentry.MipOffsets = new int[teXentry.MipMapCount];

                    for (int i = 0; i < teXentry._MipMapCount; i++)
                    {
                        //Gets offsets of MipMapData.
                        bnr.BaseStream.Position = v19;
                        teXentry.MipOffsets[i] = bnr.ReadInt32();
                        v19 = v19 + 8;
                    }

                    v = 0x10;

                    int w19 = 0;
                    int u19 = 0;

                    //Extracts and separates the Mip Maps.
                    teXentry.OutMapsB = new byte[teXentry._MipMapCount][];

                    for (int i = 0; i < teXentry._MipMapCount; i++)
                    {

                        if ((i) == (teXentry.MipOffsets.Length - 1))
                        {
                            teXentry.WTemp = new byte[(teXentry.UncompressedData.Length - teXentry.MipOffsets[i])];
                            teXentry.OutMaps = new byte[(teXentry._MipMapCount)];

                            System.Buffer.BlockCopy(teXentry.UncompressedData, teXentry.MipOffsets[i], teXentry.WTemp, 0, (teXentry.UncompressedData.Length - teXentry.MipOffsets[i]));
                            w19 = teXentry.WTemp.Length;
                            u19 = u19 + teXentry.WTemp.Length;

                            teXentry.OutMaps = teXentry.WTemp;
                            teXentry.OutMapsB[i] = teXentry.OutMaps;
                        }
                        else
                        {
                            teXentry.WTemp = new byte[(teXentry.MipOffsets[(i + 1)] - teXentry.MipOffsets[i])];
                            teXentry.OutMaps = new byte[(teXentry._MipMapCount)];
                            System.Buffer.BlockCopy(teXentry.UncompressedData, teXentry.MipOffsets[i], teXentry.WTemp, 0, (teXentry.MipOffsets[(i + 1)] - teXentry.MipOffsets[i]));
                            w19 = teXentry.WTemp.Length;
                            u19 = u19 + teXentry.WTemp.Length;

                            teXentry.OutMaps = teXentry.WTemp;
                            teXentry.OutMapsB[i] = teXentry.OutMaps;
                        }

                    }

                    teXentry.OutTexTest = new List<byte>();


                    byte[] DDSHeader19 = { 0x44, 0x44, 0x53, 0x20, 0x7c, 0x00, 0x00, 0x00, 0x07, 0x10, 0x08, 0x00, 0x00, 0x01, 0x00, 0x00,
                                         0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                                         0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x31, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                    teXentry.OutTexTest.AddRange(DDSHeader19);

                    foreach (byte[] array in teXentry.OutMapsB)
                    {
                        teXentry.OutTexTest.AddRange(array);
                    }

                    teXentry.OutMaps = teXentry.OutTexTest.ToArray();
                    uint blargx19 = Convert.ToUInt32(teXentry.XSize);
                    uint blargy19 = Convert.ToUInt32(teXentry.YSize);

                    byte[] Xbytes19 = BitConverter.GetBytes(blargy19);
                    byte[] Ybytes19 = BitConverter.GetBytes(blargx19);

                    Array.Copy(Xbytes19, 0, teXentry.OutMaps, 12, 4);
                    Array.Copy(Ybytes19, 0, teXentry.OutMaps, 16, 4);

                    #region ToRGBAPNG
                    //Time to convert this to png for RGBA related reasons.
                    byte[] DDSTemp19 = new byte[] { };
                    byte[] RGBATemp19 = new byte[] { };
                    DDSTemp19 = teXentry.OutMaps;

                    using (Stream strim = new MemoryStream(DDSTemp19))
                    {
                        using (var image = Pfim.Pfim.FromStream(strim))
                        {
                            PixelFormat format;

                            // Convert from Pfim's backend agnostic image format into GDI+'s image format
                            switch (image.Format)
                            {
                                case Pfim.ImageFormat.Rgba32:
                                    format = PixelFormat.Format32bppArgb;
                                    break;
                                case Pfim.ImageFormat.Rgb24:
                                    format = PixelFormat.Format24bppRgb;
                                    break;
                                default:
                                    // see the sample for more details
                                    throw new NotImplementedException();
                            }

                            // Pin pfim's data array so that it doesn't get reaped by GC, unnecessary
                            // in this snippet but useful technique if the data was going to be used in
                            // control like a picture box
                            var handle = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
                            try
                            {
                                var data = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);
                                var bitmap = new Bitmap(image.Width, image.Height, image.Stride, format, data);


                                using (var stream = new MemoryStream())
                                {
                                    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                                    RGBATemp19 = stream.ToArray();
                                }


                            }
                            finally
                            {
                                handle.Free();
                            }


                        }
                    }

                    teXentry.OutMaps = RGBATemp19;

                    teXentry.Picture = ByteUtilitarian.BitmapBuilderDX(teXentry.OutMaps, teXentry);
                    #endregion


                    break;

                #endregion

                #region Normal Maps(Incomplete)
                case "1F":

                    teXentry._Format = "BC5/Normal Map";

                    //Gets the unsigned integers which hold data on the texture's dimensions.
                    bnr.BaseStream.Position = 4;
                    LWData[0] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);
                    LWData[1] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);
                    LWData[2] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);

                    //X and Y coordinates. This method is borrowed from the old TexCheck.py file.
                    teXentry.XSize = Convert.ToInt32(((LWData[1] >> 6) & 0x1fff));
                    teXentry._X = teXentry.XSize;

                    teXentry.YSize = Convert.ToInt32(((LWData[1] >> 19) & 0x1fff));
                    teXentry._Y = teXentry.YSize;

                    teXentry.Mips = Convert.ToInt32(((LWData[1]) & 0x3f));
                    teXentry._MipMapCount = teXentry.Mips;

                    teXentry.PixelCount = teXentry._X * teXentry._Y;

                    teXentry.CSize = ((teXentry._X / 4) * (teXentry._Y / 4));

                    teXentry.DSize = 16;

                    Array.Clear(DTemp, 0, 4);

                    int v1f = 0x10;

                    teXentry.MipOffsets = new int[teXentry.MipMapCount];

                    for (int i = 0; i < teXentry._MipMapCount; i++)
                    {
                        //Gets offsets of MipMapData.
                        bnr.BaseStream.Position = v1f;
                        teXentry.MipOffsets[i] = bnr.ReadInt32();
                        v1f = v1f + 8;
                    }

                    v = 0x10;

                    int w1f = 0;
                    int u1f = 0;

                    //Extracts and separates the Mip Maps.
                    teXentry.OutMapsB = new byte[teXentry._MipMapCount][];

                    for (int i = 0; i < teXentry._MipMapCount; i++)
                    {

                        if ((i) == (teXentry.MipOffsets.Length - 1))
                        {
                            teXentry.WTemp = new byte[(teXentry.UncompressedData.Length - teXentry.MipOffsets[i])];
                            teXentry.OutMaps = new byte[(teXentry._MipMapCount)];

                            System.Buffer.BlockCopy(teXentry.UncompressedData, teXentry.MipOffsets[i], teXentry.WTemp, 0, (teXentry.UncompressedData.Length - teXentry.MipOffsets[i]));
                            w1f = teXentry.WTemp.Length;
                            u1f = u1f + teXentry.WTemp.Length;

                            teXentry.OutMaps = teXentry.WTemp;
                            teXentry.OutMapsB[i] = teXentry.OutMaps;
                        }
                        else
                        {
                            teXentry.WTemp = new byte[(teXentry.MipOffsets[(i + 1)] - teXentry.MipOffsets[i])];
                            teXentry.OutMaps = new byte[(teXentry._MipMapCount)];
                            System.Buffer.BlockCopy(teXentry.UncompressedData, teXentry.MipOffsets[i], teXentry.WTemp, 0, (teXentry.MipOffsets[(i + 1)] - teXentry.MipOffsets[i]));
                            w = teXentry.WTemp.Length;
                            u1f = u1f + teXentry.WTemp.Length;

                            teXentry.OutMaps = teXentry.WTemp;
                            teXentry.OutMapsB[i] = teXentry.OutMaps;
                        }

                    }

                    teXentry.OutTexTest = new List<byte>();


                    byte[] DDSHeader1f = { 0x44, 0x44, 0x53, 0x20, 0x7c, 0x00, 0x00, 0x00, 0x07, 0x10, 0x08, 0x00, 0x00, 0x01, 0x00, 0x00,
                                         0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                                         0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x35, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };


                    teXentry.OutTexTest.AddRange(DDSHeader1f);

                    foreach (byte[] array in teXentry.OutMapsB)
                    {
                        teXentry.OutTexTest.AddRange(array);
                    }

                    teXentry.OutMaps = teXentry.OutTexTest.ToArray();
                    uint blargx1f = Convert.ToUInt32(teXentry.XSize);
                    uint blargy1f = Convert.ToUInt32(teXentry.YSize);

                    byte[] Xbytes1f = BitConverter.GetBytes(blargy1f);
                    byte[] Ybytes1f = BitConverter.GetBytes(blargx1f);

                    Array.Copy(Xbytes1f, 0, teXentry.OutMaps, 12, 4);
                    Array.Copy(Ybytes1f, 0, teXentry.OutMaps, 16, 4);

                    teXentry = DDSToRGBA(teXentry);

                    break;


                #endregion

                #region Weird Toon Shader Textures
                case "27":
                    teXentry._Format = "????/Problematic Portrait Picture";

                    //Gets the unsigned integers which hold data on the texture's dimensions.
                    bnr.BaseStream.Position = 4;
                    LWData[0] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);
                    LWData[1] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);
                    LWData[2] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);

                    //X and Y coordinates. This method is borrowed from the old TexCheck.py file.
                    teXentry.XSize = Convert.ToInt32(((LWData[1] >> 6) & 0x1fff));
                    teXentry._X = teXentry.XSize;

                    teXentry.YSize = Convert.ToInt32(((LWData[1] >> 19) & 0x1fff));
                    teXentry._Y = teXentry.YSize;

                    teXentry.Mips = Convert.ToInt32(((LWData[1]) & 0x3f));
                    teXentry._MipMapCount = teXentry.Mips;

                    teXentry.PixelCount = teXentry._X * teXentry._Y;

                    teXentry.CSize = ((teXentry._X / 4) * (teXentry._Y / 4));

                    teXentry.DSize = 16;

                    Array.Clear(DTemp, 0, 4);

                    int v27 = 0x10;

                    teXentry.MipOffsets = new int[teXentry.MipMapCount];

                    for (int i = 0; i < teXentry._MipMapCount; i++)
                    {
                        //Gets offsets of MipMapData.
                        bnr.BaseStream.Position = v27;
                        teXentry.MipOffsets[i] = bnr.ReadInt32();
                        v27 = v27 + 8;
                    }

                    v = 0x10;

                    int w27 = 0;
                    int u27 = 0;

                    //Extracts and separates the Mip Maps.
                    teXentry.OutMapsB = new byte[teXentry._MipMapCount][];

                    for (int i = 0; i < teXentry._MipMapCount; i++)
                    {

                        if ((i) == (teXentry.MipOffsets.Length - 1))
                        {
                            teXentry.WTemp = new byte[(teXentry.UncompressedData.Length - teXentry.MipOffsets[i])];
                            teXentry.OutMaps = new byte[(teXentry._MipMapCount)];

                            System.Buffer.BlockCopy(teXentry.UncompressedData, teXentry.MipOffsets[i], teXentry.WTemp, 0, (teXentry.UncompressedData.Length - teXentry.MipOffsets[i]));
                            w27 = teXentry.WTemp.Length;
                            u27 = u27 + teXentry.WTemp.Length;

                            teXentry.OutMaps = teXentry.WTemp;
                            teXentry.OutMapsB[i] = teXentry.OutMaps;
                        }
                        else
                        {
                            teXentry.WTemp = new byte[(teXentry.MipOffsets[(i + 1)] - teXentry.MipOffsets[i])];
                            teXentry.OutMaps = new byte[(teXentry._MipMapCount)];
                            System.Buffer.BlockCopy(teXentry.UncompressedData, teXentry.MipOffsets[i], teXentry.WTemp, 0, (teXentry.MipOffsets[(i + 1)] - teXentry.MipOffsets[i]));
                            w = teXentry.WTemp.Length;
                            u27 = u27 + teXentry.WTemp.Length;

                            teXentry.OutMaps = teXentry.WTemp;
                            teXentry.OutMapsB[i] = teXentry.OutMaps;
                        }

                    }

                    teXentry.OutTexTest = new List<byte>();


                    byte[] DDSHeader27 = { 0x44, 0x44, 0x53, 0x20, 0x7c, 0x00, 0x00, 0x00, 0x07, 0x10, 0x08, 0x00, 0x00, 0x01, 0x00, 0x00,
                                           0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                                           0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x35, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                    teXentry.OutTexTest.AddRange(DDSHeader27);

                    foreach (byte[] array in teXentry.OutMapsB)
                    {
                        teXentry.OutTexTest.AddRange(array);
                    }

                    teXentry.OutMaps = teXentry.OutTexTest.ToArray();
                    uint blargx27 = Convert.ToUInt32(teXentry.XSize);
                    uint blargy27 = Convert.ToUInt32(teXentry.YSize);

                    byte[] Xbytes27 = BitConverter.GetBytes(blargy27);
                    byte[] Ybytes27 = BitConverter.GetBytes(blargx27);

                    Array.Copy(Xbytes27, 0, teXentry.OutMaps, 12, 4);
                    Array.Copy(Ybytes27, 0, teXentry.OutMaps, 16, 4);

                    #region ToRGBAPNG
                    //Time to convert this to png for RGBA related reasons.
                    byte[] DDSTemp27 = new byte[] { };
                    byte[] RGBATemp27 = new byte[] { };
                    DDSTemp27 = teXentry.OutMaps;

                    using (Stream strim = new MemoryStream(DDSTemp27))
                    {
                        using (var image = Pfim.Pfim.FromStream(strim))
                        {
                            PixelFormat format;

                            // Convert from Pfim's backend agnostic image format into GDI+'s image format
                            switch (image.Format)
                            {
                                case Pfim.ImageFormat.Rgba32:
                                    format = PixelFormat.Format32bppArgb;
                                    break;
                                case Pfim.ImageFormat.Rgb24:
                                    format = PixelFormat.Format24bppRgb;
                                    break;
                                default:
                                    // see the sample for more details
                                    throw new NotImplementedException();
                            }

                            // Pin pfim's data array so that it doesn't get reaped by GC, unnecessary
                            // in this snippet but useful technique if the data was going to be used in
                            // control like a picture box
                            var handle = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
                            try
                            {
                                var data = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);
                                var bitmap = new Bitmap(image.Width, image.Height, image.Stride, format, data);


                                using (var stream = new MemoryStream())
                                {
                                    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                                    RGBATemp27 = stream.ToArray();
                                }


                            }
                            finally
                            {
                                handle.Free();
                            }


                        }
                    }

                    teXentry.OutMaps = RGBATemp27;

                    teXentry.Picture = ByteUtilitarian.BitmapBuilderDX(teXentry.OutMaps, teXentry);
                    #endregion


                    break;

                #endregion

                #region Weirdo Problematic Portrait Textures
                case "2A":

                    teXentry._Format = "????/Problematic Portrait Picture";

                    //Gets the unsigned integers which hold data on the texture's dimensions.
                    bnr.BaseStream.Position = 4;
                    LWData[0] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);
                    LWData[1] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);
                    LWData[2] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);

                    //X and Y coordinates. This method is borrowed from the old TexCheck.py file.
                    teXentry.XSize = Convert.ToInt32(((LWData[1] >> 6) & 0x1fff));
                    teXentry._X = teXentry.XSize;

                    teXentry.YSize = Convert.ToInt32(((LWData[1] >> 19) & 0x1fff));
                    teXentry._Y = teXentry.YSize;

                    teXentry.Mips = Convert.ToInt32(((LWData[1]) & 0x3f));
                    teXentry._MipMapCount = teXentry.Mips;

                    teXentry.PixelCount = teXentry._X * teXentry._Y;

                    teXentry.CSize = ((teXentry._X / 4) * (teXentry._Y / 4));

                    teXentry.DSize = 16;

                    Array.Clear(DTemp, 0, 4);

                    int v2a = 0x10;

                    teXentry.MipOffsets = new int[teXentry.MipMapCount];

                    for (int i = 0; i < teXentry._MipMapCount; i++)
                    {
                        //Gets offsets of MipMapData.
                        bnr.BaseStream.Position = v2a;
                        teXentry.MipOffsets[i] = bnr.ReadInt32();
                        v2a = v2a + 8;
                    }

                    v = 0x10;

                    int w2a = 0;
                    int u2a = 0;

                    //Extracts and separates the Mip Maps.
                    teXentry.OutMapsB = new byte[teXentry._MipMapCount][];

                    for (int i = 0; i < teXentry._MipMapCount; i++)
                    {

                        if ((i) == (teXentry.MipOffsets.Length - 1))
                        {
                            teXentry.WTemp = new byte[(teXentry.UncompressedData.Length - teXentry.MipOffsets[i])];
                            teXentry.OutMaps = new byte[(teXentry._MipMapCount)];

                            System.Buffer.BlockCopy(teXentry.UncompressedData, teXentry.MipOffsets[i], teXentry.WTemp, 0, (teXentry.UncompressedData.Length - teXentry.MipOffsets[i]));
                            w2a = teXentry.WTemp.Length;
                            u2a = u2a + teXentry.WTemp.Length;

                            teXentry.OutMaps = teXentry.WTemp;
                            teXentry.OutMapsB[i] = teXentry.OutMaps;
                        }
                        else
                        {
                            teXentry.WTemp = new byte[(teXentry.MipOffsets[(i + 1)] - teXentry.MipOffsets[i])];
                            teXentry.OutMaps = new byte[(teXentry._MipMapCount)];
                            System.Buffer.BlockCopy(teXentry.UncompressedData, teXentry.MipOffsets[i], teXentry.WTemp, 0, (teXentry.MipOffsets[(i + 1)] - teXentry.MipOffsets[i]));
                            w = teXentry.WTemp.Length;
                            u2a = u2a + teXentry.WTemp.Length;

                            teXentry.OutMaps = teXentry.WTemp;
                            teXentry.OutMapsB[i] = teXentry.OutMaps;
                        }

                    }


                    teXentry.OutTexTest = new List<byte>();


                    byte[] DDSHeader2a = { 0x44, 0x44, 0x53, 0x20, 0x7c, 0x00, 0x00, 0x00, 0x07, 0x10, 0x08, 0x00, 0x00, 0x01, 0x00, 0x00,
                                           0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                                           0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x35, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                    teXentry.OutTexTest.AddRange(DDSHeader2a);

                    foreach (byte[] array in teXentry.OutMapsB)
                    {
                        teXentry.OutTexTest.AddRange(array);
                    }

                    teXentry.OutMaps = teXentry.OutTexTest.ToArray();
                    uint blargx2a = Convert.ToUInt32(teXentry.XSize);
                    uint blargy2a = Convert.ToUInt32(teXentry.YSize);

                    byte[] Xbytes2a = BitConverter.GetBytes(blargy2a);
                    byte[] Ybytes2a = BitConverter.GetBytes(blargx2a);

                    Array.Copy(Xbytes2a, 0, teXentry.OutMaps, 12, 4);
                    Array.Copy(Ybytes2a, 0, teXentry.OutMaps, 16, 4);

                    #region ToRGBAPNG
                    //Time to convert this to png for RGBA related reasons.
                    byte[] DDSTemp2a = new byte[] { };
                    byte[] RGBATemp2a = new byte[] { };
                    DDSTemp2a = teXentry.OutMaps;

                    using (Stream strim = new MemoryStream(DDSTemp2a))
                    {
                        using (var image = Pfim.Pfim.FromStream(strim))
                        {
                            PixelFormat format;

                            // Convert from Pfim's backend agnostic image format into GDI+'s image format
                            switch (image.Format)
                            {
                                case Pfim.ImageFormat.Rgba32:
                                    format = PixelFormat.Format32bppArgb;
                                    break;
                                case Pfim.ImageFormat.Rgb24:
                                    format = PixelFormat.Format24bppRgb;
                                    break;
                                default:
                                    // see the sample for more details
                                    throw new NotImplementedException();
                            }

                            // Pin pfim's data array so that it doesn't get reaped by GC, unnecessary
                            // in this snippet but useful technique if the data was going to be used in
                            // control like a picture box
                            var handle = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
                            try
                            {
                                var data = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);
                                var bitmap = new Bitmap(image.Width, image.Height, image.Stride, format, data);


                                using (var stream = new MemoryStream())
                                {
                                    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                                    RGBATemp2a = stream.ToArray();
                                }


                            }
                            finally
                            {
                                handle.Free();
                            }


                        }
                    }

                    teXentry.OutMaps = RGBATemp2a;

                    teXentry.Picture = ByteUtilitarian.BitmapBuilderDX(teXentry.OutMaps, teXentry);
                    #endregion


                    break;

                #endregion

                default:
                    MessageBox.Show("Either this texture format doesn't seem to be documented or supported, or the texture file seems to be corrupt.", "Uhhh");
                    break;
            }

            return teXentry;

        }

        public static TextureEntry InitializeTextureR(TreeView tree, TextureEntry texentry, BinaryReader bnr, string filename)
        {

            //Actual Tex Loading work here.
            byte[] VTemp = new byte[4];
            uint[] LWData = new uint[3];
            byte[] DTemp = new byte[4];

            List<byte> PreviewTemp = new List<byte>();

            switch (texentry.TexType)
            {

                #region Bitmap Textures
                case "13":

                    texentry._Format = "DXT1/BC1";
                    //Gets the unsigned integers which hold data on the texture's dimensions.
                    bnr.BaseStream.Position = 4;
                    LWData[0] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);
                    LWData[1] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);
                    LWData[2] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);

                    byte[] bytemp = { 0x09, 0x80, 0x10, 0x01 };
                    uint uinttemp;
                    int inttemp;
                    int inttempw;

                    //X and Y coordinates. This method is borrowed from the old TexCheck.py file.
                    texentry.XSize = Convert.ToInt32(((LWData[1] >> 6) & 0x1fff));
                    texentry._X = texentry.XSize;

                    texentry.YSize = Convert.ToInt32(((LWData[1] >> 19) & 0x1fff));
                    texentry._Y = texentry.YSize;

                    texentry.Mips = Convert.ToInt32(((LWData[1]) & 0x3f));
                    texentry._MipMapCount = texentry.Mips;

                    texentry.PixelCount = texentry._X * texentry._Y;

                    texentry.CSize = ((texentry._X / 4) * (texentry._Y / 4));

                    texentry.DSize = 8;

                    Array.Clear(DTemp, 0, 4);

                    int v = 0x10;

                    texentry.Dimensions = Convert.ToInt32(((LWData[0] >> 28) & 0xf));

                    texentry.MipOffsets = new int[texentry.MipMapCount];

                    //For CubeMaps.
                    if (texentry.Dimensions == 6)
                    {
                        texentry._Format = "Cube Map(Unsupported)";

                        return texentry;
                    }

                    for (int i = 0; i < texentry._MipMapCount; i++)
                    {
                        //Gets offsets of MipMapData.
                        bnr.BaseStream.Position = v;
                        texentry.MipOffsets[i] = bnr.ReadInt32();
                        v = v + 8;
                    }

                    v = 0x10;

                    int w = 0;
                    int u = 0;

                    //Extracts and separates the Mip Maps.
                    texentry.OutMapsB = new byte[texentry._MipMapCount][];

                    for (int i = 0; i < texentry._MipMapCount; i++)
                    {

                        if ((i) == (texentry.MipOffsets.Length - 1))
                        {
                            texentry.WTemp = new byte[(texentry.UncompressedData.Length - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)];

                            System.Buffer.BlockCopy(texentry.UncompressedData, texentry.MipOffsets[i], texentry.WTemp, 0, (texentry.UncompressedData.Length - texentry.MipOffsets[i]));
                            w = texentry.WTemp.Length;
                            u = u + texentry.WTemp.Length;

                            texentry.OutMaps = texentry.WTemp;
                            texentry.OutMapsB[i] = texentry.OutMaps;
                        }
                        else
                        {
                            texentry.WTemp = new byte[(texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)];
                            System.Buffer.BlockCopy(texentry.UncompressedData, texentry.MipOffsets[i], texentry.WTemp, 0, (texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i]));
                            w = texentry.WTemp.Length;
                            u = u + texentry.WTemp.Length;

                            texentry.OutMaps = texentry.WTemp;
                            texentry.OutMapsB[i] = texentry.OutMaps;
                        }

                    }

                    //Debug Export. Gotta use this for the export as well.
                    texentry.OutTexTest = new List<byte>();


                    byte[] DDSHeader = { 0x44, 0x44, 0x53, 0x20, 0x7c, 0x00, 0x00, 0x00, 0x07, 0x10, 0x08, 0x00, 0x00, 0x01, 0x00, 0x00,
                                         0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                                         0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x31, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                    if (texentry.FileName.Contains("NOMIP"))
                    {
                        DDSHeader[28] = Convert.ToByte(1);
                    }
                    else
                    {
                        DDSHeader[28] = Convert.ToByte(texentry.MipMapCount);
                    }

                    texentry.OutTexTest.AddRange(DDSHeader);

                    foreach (byte[] array in texentry.OutMapsB)
                    {
                        texentry.OutTexTest.AddRange(array);
                    }

                    int findex = filename.LastIndexOf("\\");
                    string outname = (filename.Substring(0, findex) + "\\") + texentry.TrueName;
                    string outpngname = outname + ".png";
                    outname = outname + ".dds";

                    texentry.OutMaps = texentry.OutTexTest.ToArray();
                    uint blargx = Convert.ToUInt32(texentry.XSize);
                    uint blargy = Convert.ToUInt32(texentry.YSize);

                    byte[] Xbytes = BitConverter.GetBytes(blargy);
                    byte[] Ybytes = BitConverter.GetBytes(blargx);

                    Array.Copy(Xbytes, 0, texentry.OutMaps, 12, 4);
                    Array.Copy(Ybytes, 0, texentry.OutMaps, 16, 4);

                    //Test to setup a nomip dds file of read texture for preview reasons.
                    byte[] DHTemp =    { 0x44, 0x44, 0x53, 0x20, 0x7c, 0x00, 0x00, 0x00, 0x07, 0x10, 0x08, 0x00, 0x00, 0x01, 0x00, 0x00,
                                         0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                                         0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x31, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                    byte[] LenTemp = BitConverter.GetBytes(texentry.YSize);
                    byte[] WidTemp = BitConverter.GetBytes(texentry.XSize);

                    Array.Copy(LenTemp, 0, DHTemp, 12, 4);
                    Array.Copy(WidTemp, 0, DHTemp, 16, 4);

                    texentry = DDSToRGBA(texentry);

                    break;

                #endregion

                #region Bitmap Textures with Transparency
                case "17":
                    texentry._Format = "DXT5/BC3";

                    //Gets the unsigned integers which hold data on the texture's dimensions.
                    bnr.BaseStream.Position = 4;
                    LWData[0] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);
                    LWData[1] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);
                    LWData[2] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);

                    //X and Y coordinates. This method is borrowed from the old TexCheck.py file.
                    texentry.XSize = Convert.ToInt32(((LWData[1] >> 6) & 0x1fff));
                    texentry._X = texentry.XSize;

                    texentry.YSize = Convert.ToInt32(((LWData[1] >> 19) & 0x1fff));
                    texentry._Y = texentry.YSize;

                    texentry.Mips = Convert.ToInt32(((LWData[1]) & 0x3f));
                    texentry._MipMapCount = texentry.Mips;

                    texentry.PixelCount = texentry._X * texentry._Y;

                    texentry.CSize = ((texentry._X / 4) * (texentry._Y / 4));

                    texentry.DSize = 16;

                    Array.Clear(DTemp, 0, 4);

                    int v17 = 0x10;

                    texentry.MipOffsets = new int[texentry.MipMapCount];

                    for (int i = 0; i < texentry._MipMapCount; i++)
                    {
                        //Gets offsets of MipMapData.
                        bnr.BaseStream.Position = v17;
                        texentry.MipOffsets[i] = bnr.ReadInt32();
                        v17 = v17 + 8;
                    }
                    v = 0x10;
                    int w17 = 0;
                    int u17 = 0;
                    //Extracts and separates the Mip Maps.
                    texentry.OutMapsB = new byte[texentry._MipMapCount][];
                    for (int i = 0; i < texentry._MipMapCount; i++)
                    {
                        if ((i) == (texentry.MipOffsets.Length - 1))
                        {
                            texentry.WTemp = new byte[(Convert.ToInt32(texentry.UncompressedData.Length) - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)];
                            Array.Copy(texentry.UncompressedData.ToArray(), texentry.MipOffsets[i], texentry.WTemp, 0, (Convert.ToInt32(texentry.UncompressedData.Length) - texentry.MipOffsets[i]));
                            w17 = texentry.WTemp.Length;
                            u17 = u17 + texentry.WTemp.Length;
                            texentry.OutMaps = texentry.WTemp;
                            texentry.OutMapsB[i] = texentry.OutMaps;
                        }
                        else
                        {
                            texentry.WTemp = new byte[(texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)]; Array.Copy(texentry.UncompressedData.ToArray(), texentry.MipOffsets[i], texentry.WTemp, 0, (texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i]));
                            w = texentry.WTemp.Length;
                            u17 = u17 + texentry.WTemp.Length;
                            texentry.OutMaps = texentry.WTemp;
                            texentry.OutMapsB[i] = texentry.OutMaps;
                        }
                    }
                    //Debug Export. Gotta use this for the export as well.
                    texentry.OutTexTest = new List<byte>();



                    byte[] DDSHeader17 = { 0x44, 0x44, 0x53, 0x20, 0x7c, 0x00, 0x00, 0x00, 0x07, 0x10, 0x08, 0x00, 0x00, 0x01, 0x00, 0x00,
                                         0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                                         0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x35, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                    if (texentry.FileName.Contains("NOMIP"))
                    {
                        DDSHeader17[28] = Convert.ToByte(1);
                    }
                    else
                    {
                        DDSHeader17[28] = Convert.ToByte(texentry.MipMapCount);
                    }

                    texentry.OutTexTest.AddRange(DDSHeader17);

                    foreach (byte[] array in texentry.OutMapsB)
                    {
                        texentry.OutTexTest.AddRange(array);
                    }

                    int findex17 = filename.LastIndexOf("\\");
                    string outname17 = (filename.Substring(0, findex17) + "\\") + texentry.TrueName;
                    string outpngname17 = outname17 + ".png";
                    outname17 = outname17 + ".dds";

                    texentry.OutMaps = texentry.OutTexTest.ToArray();

                    uint blargx17 = Convert.ToUInt32(texentry.XSize);
                    uint blargy17 = Convert.ToUInt32(texentry.YSize);

                    byte[] Xbytes17 = BitConverter.GetBytes(blargy17);
                    byte[] Ybytes17 = BitConverter.GetBytes(blargx17);

                    Array.Copy(Xbytes17, 0, texentry.OutMaps, 12, 4);
                    Array.Copy(Ybytes17, 0, texentry.OutMaps, 16, 4);

                    texentry = DDSToRGBA(texentry);

                    break;

                #endregion

                #region Specular Tetures
                case "19":

                    texentry._Format = "BC4_UNORM/Metalic/Specular Map";

                    //Gets the unsigned integers which hold data on the texture's dimensions.
                    bnr.BaseStream.Position = 4;
                    LWData[0] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);
                    LWData[1] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);
                    LWData[2] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);

                    byte[] bytemp19 = { 0x09, 0x80, 0x10, 0x01 };
                    uint uinttemp19;
                    int inttemp19;
                    int inttempw19;

                    //X and Y coordinates. This method is borrowed from the old TexCheck.py file.
                    texentry.XSize = Convert.ToInt32(((LWData[1] >> 6) & 0x1fff));
                    texentry._X = texentry.XSize;

                    texentry.YSize = Convert.ToInt32(((LWData[1] >> 19) & 0x1fff));
                    texentry._Y = texentry.YSize;

                    texentry.Mips = Convert.ToInt32(((LWData[1]) & 0x3f));
                    texentry._MipMapCount = texentry.Mips;

                    texentry.PixelCount = texentry._X * texentry._Y;

                    texentry.CSize = ((texentry._X / 4) * (texentry._Y / 4));

                    texentry.DSize = 8;

                    Array.Clear(DTemp, 0, 4);

                    int v19 = 0x10;

                    texentry.Dimensions = Convert.ToInt32(((LWData[0] >> 28) & 0xf));

                    texentry.MipOffsets = new int[texentry.MipMapCount];

                    //For CubeMaps.
                    if (texentry.Dimensions == 6)
                    {
                        texentry._Format = "Cube Map(Unsupported)";

                        return texentry;
                    }

                    for (int i = 0; i < texentry._MipMapCount; i++)
                    {
                        //Gets offsets of MipMapData.
                        bnr.BaseStream.Position = v19;
                        texentry.MipOffsets[i] = bnr.ReadInt32();
                        v19 = v19 + 8;
                    }

                    v19 = 0x10;

                    int w19 = 0;
                    int u19 = 0;

                    //Extracts and separates the Mip Maps.
                    texentry.OutMapsB = new byte[texentry._MipMapCount][];

                    for (int i = 0; i < texentry._MipMapCount; i++)
                    {

                        if ((i) == (texentry.MipOffsets.Length - 1))
                        {
                            texentry.WTemp = new byte[(texentry.UncompressedData.Length - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)];

                            System.Buffer.BlockCopy(texentry.UncompressedData, texentry.MipOffsets[i], texentry.WTemp, 0, (texentry.UncompressedData.Length - texentry.MipOffsets[i]));
                            w19 = texentry.WTemp.Length;
                            u19 = u19 + texentry.WTemp.Length;

                            texentry.OutMaps = texentry.WTemp;
                            texentry.OutMapsB[i] = texentry.OutMaps;
                        }
                        else
                        {
                            texentry.WTemp = new byte[(texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)];
                            System.Buffer.BlockCopy(texentry.UncompressedData, texentry.MipOffsets[i], texentry.WTemp, 0, (texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i]));
                            w19 = texentry.WTemp.Length;
                            u19 = u19 + texentry.WTemp.Length;

                            texentry.OutMaps = texentry.WTemp;
                            texentry.OutMapsB[i] = texentry.OutMaps;
                        }

                    }

                    //Debug Export. Gotta use this for the export as well.
                    texentry.OutTexTest = new List<byte>();


                    byte[] DDSHeader19 = { 0x44, 0x44, 0x53, 0x20, 0x7c, 0x00, 0x00, 0x00, 0x07, 0x10, 0x08, 0x00, 0x00, 0x01, 0x00, 0x00,
                                         0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                                         0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x31, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                    if (texentry.FileName.Contains("NOMIP"))
                    {
                        DDSHeader19[28] = Convert.ToByte(1);
                    }
                    else
                    {
                        DDSHeader19[28] = Convert.ToByte(texentry.MipMapCount);
                    }

                    texentry.OutTexTest.AddRange(DDSHeader19);

                    foreach (byte[] array in texentry.OutMapsB)
                    {
                        texentry.OutTexTest.AddRange(array);
                    }

                    int findex19 = filename.LastIndexOf("\\");
                    string outname19 = (filename.Substring(0, findex19) + "\\") + texentry.TrueName;
                    string outpngname19 = outname19 + ".png";
                    outname19 = outname19 + ".dds";

                    texentry.OutMaps = texentry.OutTexTest.ToArray();
                    uint blargx19 = Convert.ToUInt32(texentry.XSize);
                    uint blargy19 = Convert.ToUInt32(texentry.YSize);

                    byte[] Xbytes19 = BitConverter.GetBytes(blargy19);
                    byte[] Ybytes19 = BitConverter.GetBytes(blargx19);

                    Array.Copy(Xbytes19, 0, texentry.OutMaps, 12, 4);
                    Array.Copy(Ybytes19, 0, texentry.OutMaps, 16, 4);

                    //Test to setup a nomip dds file of read texture for preview reasons.
                    byte[] DHTemp19 =    { 0x44, 0x44, 0x53, 0x20, 0x7c, 0x00, 0x00, 0x00, 0x07, 0x10, 0x08, 0x00, 0x00, 0x01, 0x00, 0x00,
                                         0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                                         0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x31, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                    byte[] LenTemp19 = BitConverter.GetBytes(texentry.YSize);
                    byte[] WidTemp19 = BitConverter.GetBytes(texentry.XSize);

                    Array.Copy(LenTemp19, 0, DHTemp19, 12, 4);
                    Array.Copy(WidTemp19, 0, DHTemp19, 16, 4);

                    texentry = DDSToRGBA(texentry);

                    break;

                #endregion

                #region Unknown Cloth Textures
                case "1E":
                    texentry._Format = "Cloth?";

                    //Gets the unsigned integers which hold data on the texture's dimensions.
                    bnr.BaseStream.Position = 4;
                    LWData[0] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);
                    LWData[1] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);
                    LWData[2] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);

                    //X and Y coordinates. This method is borrowed from the old TexCheck.py file.
                    texentry.XSize = Convert.ToInt32(((LWData[1] >> 6) & 0x1fff));
                    texentry._X = texentry.XSize;

                    texentry.YSize = Convert.ToInt32(((LWData[1] >> 19) & 0x1fff));
                    texentry._Y = texentry.YSize;

                    texentry.Mips = Convert.ToInt32(((LWData[1]) & 0x3f));
                    texentry._MipMapCount = texentry.Mips;

                    texentry.PixelCount = texentry._X * texentry._Y;

                    texentry.CSize = ((texentry._X / 4) * (texentry._Y / 4));

                    texentry.DSize = 16;

                    Array.Clear(DTemp, 0, 4);

                    int v1e = 0x10;

                    texentry.MipOffsets = new int[texentry.MipMapCount];

                    for (int i = 0; i < texentry._MipMapCount; i++)
                    {
                        //Gets offsets of MipMapData.
                        bnr.BaseStream.Position = v1e;
                        texentry.MipOffsets[i] = bnr.ReadInt32();
                        v1e = v1e + 8;
                    }
                    v = 0x10;
                    int w1e = 0;
                    int u1e = 0;
                    //Extracts and separates the Mip Maps.
                    texentry.OutMapsB = new byte[texentry._MipMapCount][];
                    for (int i = 0; i < texentry._MipMapCount; i++)
                    {
                        if ((i) == (texentry.MipOffsets.Length - 1))
                        {
                            texentry.WTemp = new byte[(Convert.ToInt32(texentry.UncompressedData.Length) - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)];
                            Array.Copy(texentry.UncompressedData.ToArray(), texentry.MipOffsets[i], texentry.WTemp, 0, (Convert.ToInt32(texentry.UncompressedData.Length) - texentry.MipOffsets[i]));
                            w1e = texentry.WTemp.Length;
                            u1e = u1e + texentry.WTemp.Length;
                            texentry.OutMaps = texentry.WTemp;
                            texentry.OutMapsB[i] = texentry.OutMaps;
                        }
                        else
                        {
                            texentry.WTemp = new byte[(texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)]; Array.Copy(texentry.UncompressedData.ToArray(), texentry.MipOffsets[i], texentry.WTemp, 0, (texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i]));
                            w = texentry.WTemp.Length;
                            u1e = u1e + texentry.WTemp.Length;
                            texentry.OutMaps = texentry.WTemp;
                            texentry.OutMapsB[i] = texentry.OutMaps;
                        }
                    }
                    //Debug Export. Gotta use this for the export as well.
                    texentry.OutTexTest = new List<byte>();



                    byte[] DDSHeader1e = { 0x44, 0x44, 0x53, 0x20, 0x7c, 0x00, 0x00, 0x00, 0x07, 0x10, 0x08, 0x00, 0x00, 0x01, 0x00, 0x00,
                                         0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                                         0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x35, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                    if (texentry.FileName.Contains("NOMIP"))
                    {
                        DDSHeader1e[28] = Convert.ToByte(1);
                    }
                    else
                    {
                        DDSHeader1e[28] = Convert.ToByte(texentry.MipMapCount);
                    }

                    texentry.OutTexTest.AddRange(DDSHeader1e);

                    foreach (byte[] array in texentry.OutMapsB)
                    {
                        texentry.OutTexTest.AddRange(array);
                    }

                    int findex1e = filename.LastIndexOf("\\");
                    string outname1e = (filename.Substring(0, findex1e) + "\\") + texentry.TrueName;
                    string outpngname1e = outname1e + ".png";
                    outname1e = outname1e + ".dds";

                    texentry.OutMaps = texentry.OutTexTest.ToArray();

                    uint blargx1e = Convert.ToUInt32(texentry.XSize);
                    uint blargy1e = Convert.ToUInt32(texentry.YSize);

                    byte[] Xbytes1e = BitConverter.GetBytes(blargy1e);
                    byte[] Ybytes1e = BitConverter.GetBytes(blargx1e);

                    Array.Copy(Xbytes1e, 0, texentry.OutMaps, 12, 4);
                    Array.Copy(Ybytes1e, 0, texentry.OutMaps, 16, 4);

                    texentry = DDSToRGBA(texentry);

                    break;
                #endregion

                #region Normal Maps(Incomplete)
                case "1F":

                    texentry._Format = "BC5/Normal Map";

                    //Gets the unsigned integers which hold data on the texture's dimensions.
                    bnr.BaseStream.Position = 4;
                    LWData[0] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);
                    LWData[1] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);
                    LWData[2] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);

                    byte[] bytemp1f = { 0x09, 0x80, 0x10, 0x01 };
                    uint uinttemp1f;
                    int inttemp1f;
                    int inttempw1f;

                    //X and Y coordinates. This method is borrowed from the old TexCheck.py file.
                    texentry.XSize = Convert.ToInt32(((LWData[1] >> 6) & 0x1fff));
                    texentry._X = texentry.XSize;

                    texentry.YSize = Convert.ToInt32(((LWData[1] >> 19) & 0x1fff));
                    texentry._Y = texentry.YSize;

                    texentry.Mips = Convert.ToInt32(((LWData[1]) & 0x3f));
                    texentry._MipMapCount = texentry.Mips;

                    texentry.PixelCount = texentry._X * texentry._Y;

                    texentry.CSize = ((texentry._X / 4) * (texentry._Y / 4));

                    texentry.DSize = 16;

                    Array.Clear(DTemp, 0, 4);

                    int v1f = 0x10;

                    texentry.Dimensions = Convert.ToInt32(((LWData[0] >> 28) & 0xf));

                    texentry.MipOffsets = new int[texentry.MipMapCount];

                    //For CubeMaps.
                    if (texentry.Dimensions == 6)
                    {
                        texentry._Format = "Cube Map(Unsupported)";

                        return texentry;
                    }

                    for (int i = 0; i < texentry._MipMapCount; i++)
                    {
                        //Gets offsets of MipMapData.
                        bnr.BaseStream.Position = v1f;
                        texentry.MipOffsets[i] = bnr.ReadInt32();
                        v1f = v1f + 8;
                    }

                    v1f = 0x10;

                    int w1f = 0;
                    int u1f = 0;

                    //Extracts and separates the Mip Maps.
                    texentry.OutMapsB = new byte[texentry._MipMapCount][];
                    for (int i = 0; i < texentry._MipMapCount; i++)
                    {
                        if ((i) == (texentry.MipOffsets.Length - 1))
                        {
                            texentry.WTemp = new byte[(Convert.ToInt32(texentry.UncompressedData.Length) - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)];
                            System.Buffer.BlockCopy(texentry.UncompressedData, texentry.MipOffsets[i], texentry.WTemp, 0, (texentry.UncompressedData.Length - texentry.MipOffsets[i]));
                            w1f = texentry.WTemp.Length;
                            u1f = u1f + texentry.WTemp.Length;
                            texentry.OutMaps = texentry.WTemp;
                            texentry.OutMapsB[i] = texentry.OutMaps;
                        }
                        else
                        {
                            texentry.WTemp = new byte[(texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)]; Array.Copy(texentry.UncompressedData.ToArray(), texentry.MipOffsets[i], texentry.WTemp, 0, (texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i]));
                            w1f = texentry.WTemp.Length;
                            u1f = u1f + texentry.WTemp.Length;
                            texentry.OutMaps = texentry.WTemp;
                            texentry.OutMapsB[i] = texentry.OutMaps;
                        }
                    }

                    //Debug Export. Gotta use this for the export as well.
                    texentry.OutTexTest = new List<byte>();


                    byte[] DDSHeader1f = { 0x44, 0x44, 0x53, 0x20, 0x7c, 0x00, 0x00, 0x00, 0x07, 0x10, 0x08, 0x00, 0x00, 0x01, 0x00, 0x00,
                                         0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                                         0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x35, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                    texentry.OutTexTest.AddRange(DDSHeader1f);

                    foreach (byte[] array in texentry.OutMapsB)
                    {
                        texentry.OutTexTest.AddRange(array);
                    }

                    int findex1f = filename.LastIndexOf("\\");
                    string outname1f = (filename.Substring(0, findex1f) + "\\") + texentry.TrueName;
                    string outpngname1f = outname1f + ".png";
                    outname1f = outname1f + ".dds";

                    texentry.OutMaps = texentry.OutTexTest.ToArray();
                    uint blargx1f = Convert.ToUInt32(texentry.XSize);
                    uint blargy1f = Convert.ToUInt32(texentry.YSize);

                    byte[] Xbytes1f = BitConverter.GetBytes(blargy1f);
                    byte[] Ybytes1f = BitConverter.GetBytes(blargx1f);

                    Array.Copy(Xbytes1f, 0, texentry.OutMaps, 12, 4);
                    Array.Copy(Ybytes1f, 0, texentry.OutMaps, 16, 4);

                    texentry = DDSToRGBA(texentry);

                    break;

                #endregion

                #region Weird Toon Shader Textures
                case "27":
                    texentry._Format = "????/Toon Shader Picture";

                    //Gets the unsigned integers which hold data on the texture's dimensions.
                    bnr.BaseStream.Position = 4;
                    LWData[0] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);
                    LWData[1] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);
                    LWData[2] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);

                    //X and Y coordinates. This method is borrowed from the old TexCheck.py file.
                    texentry.XSize = Convert.ToInt32(((LWData[1] >> 6) & 0x1fff));
                    texentry._X = texentry.XSize;

                    texentry.YSize = Convert.ToInt32(((LWData[1] >> 19) & 0x1fff));
                    texentry._Y = texentry.YSize;

                    texentry.Mips = Convert.ToInt32(((LWData[1]) & 0x3f));
                    texentry._MipMapCount = texentry.Mips;

                    texentry.PixelCount = texentry._X * texentry._Y;

                    texentry.CSize = ((texentry._X / 4) * (texentry._Y / 4));

                    texentry.DSize = 16;

                    Array.Clear(DTemp, 0, 4);

                    int v27 = 0x10;

                    texentry.MipOffsets = new int[texentry.MipMapCount];

                    for (int i = 0; i < texentry._MipMapCount; i++)
                    {
                        //Gets offsets of MipMapData.
                        bnr.BaseStream.Position = v27;
                        texentry.MipOffsets[i] = bnr.ReadInt32();
                        v27 = v27 + 8;
                    }
                    v = 0x10;
                    int w27 = 0;
                    int u27 = 0;
                    //Extracts and separates the Mip Maps.
                    texentry.OutMapsB = new byte[texentry._MipMapCount][];
                    for (int i = 0; i < texentry._MipMapCount; i++)
                    {
                        if ((i) == (texentry.MipOffsets.Length - 1))
                        {
                            texentry.WTemp = new byte[(Convert.ToInt32(texentry.UncompressedData.Length) - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)];
                            Array.Copy(texentry.UncompressedData.ToArray(), texentry.MipOffsets[i], texentry.WTemp, 0, (Convert.ToInt32(texentry.UncompressedData.Length) - texentry.MipOffsets[i]));
                            w27 = texentry.WTemp.Length;
                            u27 = u27 + texentry.WTemp.Length;
                            texentry.OutMaps = texentry.WTemp;
                            texentry.OutMapsB[i] = texentry.OutMaps;
                        }
                        else
                        {
                            texentry.WTemp = new byte[(texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)]; Array.Copy(texentry.UncompressedData.ToArray(), texentry.MipOffsets[i], texentry.WTemp, 0, (texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i]));
                            w = texentry.WTemp.Length;
                            u27 = u27 + texentry.WTemp.Length;
                            texentry.OutMaps = texentry.WTemp;
                            texentry.OutMapsB[i] = texentry.OutMaps;
                        }
                    }
                    //Debug Export. Gotta use this for the export as well.
                    texentry.OutTexTest = new List<byte>();



                    byte[] DDSHeader27 = { 0x44, 0x44, 0x53, 0x20, 0x7c, 0x00, 0x00, 0x00, 0x07, 0x10, 0x08, 0x00, 0x00, 0x01, 0x00, 0x00,
                                         0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                                         0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x35, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                    if (texentry.FileName.Contains("NOMIP"))
                    {
                        DDSHeader27[28] = Convert.ToByte(1);
                    }
                    else
                    {
                        DDSHeader27[28] = Convert.ToByte(texentry.MipMapCount);
                    }

                    texentry.OutTexTest.AddRange(DDSHeader27);

                    foreach (byte[] array in texentry.OutMapsB)
                    {
                        texentry.OutTexTest.AddRange(array);
                    }

                    int findex27 = filename.LastIndexOf("\\");
                    string outname27 = (filename.Substring(0, findex27) + "\\") + texentry.TrueName;
                    string outpngname27 = outname27 + ".png";
                    outname27 = outname27 + ".dds";

                    texentry.OutMaps = texentry.OutTexTest.ToArray();

                    uint blargx27 = Convert.ToUInt32(texentry.XSize);
                    uint blargy27 = Convert.ToUInt32(texentry.YSize);

                    byte[] Xbytes27 = BitConverter.GetBytes(blargy27);
                    byte[] Ybytes27 = BitConverter.GetBytes(blargx27);

                    Array.Copy(Xbytes27, 0, texentry.OutMaps, 12, 4);
                    Array.Copy(Ybytes27, 0, texentry.OutMaps, 16, 4);

                    texentry = DDSToRGBA(texentry);

                    break;

                #endregion

                #region Weirdo Problematic Portrait Textures
                case "2A":

                    texentry._Format = "????/Problematic Portrait Picture";

                    //Gets the unsigned integers which hold data on the texture's dimensions.
                    bnr.BaseStream.Position = 4;
                    LWData[0] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);
                    LWData[1] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);
                    LWData[2] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);

                    //X and Y coordinates. This method is borrowed from the old TexCheck.py file.
                    texentry.XSize = Convert.ToInt32(((LWData[1] >> 6) & 0x1fff));
                    texentry._X = texentry.XSize;

                    texentry.YSize = Convert.ToInt32(((LWData[1] >> 19) & 0x1fff));
                    texentry._Y = texentry.YSize;

                    texentry.Mips = Convert.ToInt32(((LWData[1]) & 0x3f));
                    texentry._MipMapCount = texentry.Mips;

                    texentry.PixelCount = texentry._X * texentry._Y;

                    texentry.CSize = ((texentry._X / 4) * (texentry._Y / 4));

                    texentry.DSize = 16;

                    Array.Clear(DTemp, 0, 4);

                    int v2a = 0x10;

                    texentry.MipOffsets = new int[texentry.MipMapCount];

                    for (int i = 0; i < texentry._MipMapCount; i++)
                    {
                        //Gets offsets of MipMapData.
                        bnr.BaseStream.Position = v2a;
                        texentry.MipOffsets[i] = bnr.ReadInt32();
                        v2a = v2a + 8;
                    }
                    v = 0x10;
                    int w2a = 0;
                    int u2a = 0;
                    //Extracts and separates the Mip Maps.
                    texentry.OutMapsB = new byte[texentry._MipMapCount][];
                    for (int i = 0; i < texentry._MipMapCount; i++)
                    {
                        if ((i) == (texentry.MipOffsets.Length - 1))
                        {
                            texentry.WTemp = new byte[(Convert.ToInt32(texentry.UncompressedData.Length) - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)];
                            Array.Copy(texentry.UncompressedData.ToArray(), texentry.MipOffsets[i], texentry.WTemp, 0, (Convert.ToInt32(texentry.UncompressedData.Length) - texentry.MipOffsets[i]));
                            w2a = texentry.WTemp.Length;
                            u2a = u2a + texentry.WTemp.Length;
                            texentry.OutMaps = texentry.WTemp;
                            texentry.OutMapsB[i] = texentry.OutMaps;
                        }
                        else
                        {
                            texentry.WTemp = new byte[(texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)]; Array.Copy(texentry.UncompressedData.ToArray(), texentry.MipOffsets[i], texentry.WTemp, 0, (texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i]));
                            w = texentry.WTemp.Length;
                            u2a = u2a + texentry.WTemp.Length;
                            texentry.OutMaps = texentry.WTemp;
                            texentry.OutMapsB[i] = texentry.OutMaps;
                        }
                    }
                    //Debug Export. Gotta use this for the export as well.
                    texentry.OutTexTest = new List<byte>();



                    byte[] DDSHeader2a = { 0x44, 0x44, 0x53, 0x20, 0x7c, 0x00, 0x00, 0x00, 0x07, 0x10, 0x08, 0x00, 0x00, 0x01, 0x00, 0x00,
                                         0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                                         0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x35, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                    if (texentry.FileName.Contains("NOMIP"))
                    {
                        DDSHeader2a[28] = Convert.ToByte(1);
                    }
                    else
                    {
                        DDSHeader2a[28] = Convert.ToByte(texentry.MipMapCount);
                    }

                    texentry.OutTexTest.AddRange(DDSHeader2a);

                    foreach (byte[] array in texentry.OutMapsB)
                    {
                        texentry.OutTexTest.AddRange(array);
                    }

                    int findex2a = filename.LastIndexOf("\\");
                    string outname2a = (filename.Substring(0, findex2a) + "\\") + texentry.TrueName;
                    string outpngname2a = outname2a + ".png";
                    outname2a = outname2a + ".dds";

                    texentry.OutMaps = texentry.OutTexTest.ToArray();

                    uint blargx2a = Convert.ToUInt32(texentry.XSize);
                    uint blargy2a = Convert.ToUInt32(texentry.YSize);

                    byte[] Xbytes2a = BitConverter.GetBytes(blargy2a);
                    byte[] Ybytes2a = BitConverter.GetBytes(blargx2a);

                    Array.Copy(Xbytes2a, 0, texentry.OutMaps, 12, 4);
                    Array.Copy(Ybytes2a, 0, texentry.OutMaps, 16, 4);

                    texentry = DDSToRGBA(texentry);

                    break;

                #endregion

                #region Everything Else
                default:
                    texentry._Format = "??????";

                    //Gets the unsigned integers which hold data on the texture's dimensions.
                    bnr.BaseStream.Position = 4;
                    LWData[0] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);
                    LWData[1] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);
                    LWData[2] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);

                    //X and Y coordinates. This method is borrowed from the old TexCheck.py file.
                    texentry.XSize = Convert.ToInt32(((LWData[1] >> 6) & 0x1fff));
                    texentry._X = texentry.XSize;

                    texentry.YSize = Convert.ToInt32(((LWData[1] >> 19) & 0x1fff));
                    texentry._Y = texentry.YSize;

                    texentry.Mips = Convert.ToInt32(((LWData[1]) & 0x3f));
                    texentry._MipMapCount = texentry.Mips;

                    texentry.PixelCount = texentry._X * texentry._Y;

                    texentry.CSize = ((texentry._X / 4) * (texentry._Y / 4));

                    texentry.DSize = 16;

                    Array.Clear(DTemp, 0, 4);

                    int v2X = 0x10;

                    texentry.MipOffsets = new int[texentry.MipMapCount];

                    //For CubeMaps.
                    if (texentry.Dimensions == 6)
                    {
                        texentry._Format = "Cube Map(Unsupported)";

                        break;
                    }

                    break;

                    #endregion

            }

            return texentry;

        }

        public static TextureEntry DDSToRGBA(TextureEntry texentry)
        {

            #region ToRGBAPNG
            //Time to convert this to png for RGBA related reasons.
            byte[] DDSTemp13 = new byte[] { };
            texentry.OutTar = new byte[] { };
            byte[] RGBATemp13 = new byte[] { };
            texentry.OutTar = texentry.OutMaps;
            DDSTemp13 = texentry.OutMaps;

            using (Stream strim = new MemoryStream(DDSTemp13))
            {
                using (var image = Pfim.Pfim.FromStream(strim))
                {
                    PixelFormat format;

                    // Convert from Pfim's backend agnostic image format into GDI+'s image format
                    switch (image.Format)
                    {
                        case Pfim.ImageFormat.Rgba32:
                            format = PixelFormat.Format32bppArgb;
                            break;
                        case Pfim.ImageFormat.Rgb24:
                            format = PixelFormat.Format24bppRgb;
                            break;
                        default:
                            // see the sample for more details
                            throw new NotImplementedException();
                    }

                    // Pin pfim's data array so that it doesn't get reaped by GC, unnecessary
                    // in this snippet but useful technique if the data was going to be used in
                    // control like a picture box
                    var handle = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
                    try
                    {
                        var data = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);
                        var bitmap = new Bitmap(image.Width, image.Height, image.Stride, format, data);


                        using (var stream = new MemoryStream())
                        {
                            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                            RGBATemp13 = stream.ToArray();
                        }


                    }
                    finally
                    {
                        handle.Free();
                    }


                }
            }

            texentry.OutMaps = RGBATemp13;

            texentry.Picture = ByteUtilitarian.BitmapBuilderDX(texentry.OutMaps, texentry);
            #endregion

            return texentry;

        }

    }
}
