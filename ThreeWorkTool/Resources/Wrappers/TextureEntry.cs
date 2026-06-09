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
using SharpDX;
using SharpDX.Direct3D11;
using ThreeWorkTool.Resources.Wrappers.ExtraNodes;

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
        public int UnkField03;
        public int UnkField04;
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
        public List<ulong> MipOffsets;
        public List<byte> OutTexTest;
        public int Shift;
        public Bitmap Picture;
        public bool IsCubeMap = false;
        public List<RTextureFace> Faces;
        [Category("Texture"), ReadOnlyAttribute(true)]
        public int SurfaceCount { get; set; }
        [Category("Texture"), ReadOnlyAttribute(true)]
        public int MipMapCount { get; set; }
        //First time using dimensional lists.
        public List<List<byte[]>> Surfaces = new List<List<byte[]>>();

        //public List<Face> CubeFace;
        //public struct Face
        //{
        //    public float field00;
        //    public System.Numerics.Vector3 Negative;
        //    public System.Numerics.Vector3 Postiive;
        //    public System.Numerics.Vector2 UV;
        //}


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
                    //brStream.BaseStream.Position = 12;
                    //texentry.SurfaceCount = brStream.ReadByte();
                    //texentry.TexType = brStream.ReadByte().ToString("X2");
                    //texentry._TextureType = texentry.TexType;

                    //List<byte> PreviewTemp = new List<byte>();

                    //texentry = InitializeTexture(tree, texentry, brStream, TexStream, filename);
                    texentry = BuildTextureEntry(texentry, brStream);
                }
            }

            return texentry;

        }

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
            TextureEntry texentry = new TextureEntry();
            InsertEntry(tree, node, filename, texentry);

            //texentry.DecompressedFileLength = texentry.UncompressedData.Length;
            //texentry._DecompressedFileLength = texentry.UncompressedData.Length;
            //texentry.CompressedFileLength = texentry.CompressedData.Length;
            //texentry._CompressedFileLength = texentry.CompressedData.Length;
            texentry._FileName = texentry.TrueName;
            //texentry._FileType = texentry.FileExt;
            texentry.EntryName = texentry.FileName;

            //Type Specific Work Here.
            using (MemoryStream LmtStream = new MemoryStream(texentry.UncompressedData))
            {
                using (BinaryReader bnr = new BinaryReader(LmtStream))
                {
                    BuildTextureEntry(texentry, bnr);
                }
            }

            return texentry;

        }

        public static TextureEntry ReplaceTextureEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {

            TextureEntry texentry = new TextureEntry();
            TextureEntry oldentry = new TextureEntry();

            tree.BeginUpdate();

            ReplaceEntry(tree, node, filename, texentry, oldentry);
            texentry.FileName = texentry.TrueName;
            //texentry.DecompressedFileLength = texentry.UncompressedData.Length;
            //texentry._DecompressedFileLength = texentry.UncompressedData.Length;
            //texentry.CompressedFileLength = texentry.CompressedData.Length;
            //texentry._CompressedFileLength = texentry.CompressedData.Length;
            texentry._FileName = texentry.TrueName;
            //texentry._FileType = texentry.FileExt;

            //Type Specific Work Here.
            using (MemoryStream LmtStream = new MemoryStream(texentry.UncompressedData))
            {
                using (BinaryReader bnr = new BinaryReader(LmtStream))
                {
                    BuildTextureEntry(texentry, bnr);
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
                    ByteUtilitarian.FromDDSToTex(teXentry, bnr, FTED);

                }
            }
            catch (Exception ex)
            {
                string ProperPath = ""; ProperPath = Globals.ToolPath + "Log.txt"; using (StreamWriter sw = File.AppendText(ProperPath))
                {
                    sw.WriteLine("Texture insertion from .DDS file failed. Here's details:\n" + ex);
                }
            }

            //Filling in the rest of the data for the teXentry.
            teXentry.TrueName = FTED.ShortName;
            teXentry._FileName = teXentry.TrueName;
            FTED.TXfilename = teXentry.EntryName;
            FTED.TXfilename = teXentry.TrueName;
            teXentry._FileName = teXentry.TrueName;
            teXentry.FileExt = ".tex";
            teXentry.DSize = teXentry.UncompressedData.Length;
            teXentry.CSize = teXentry.CompressedData.Length;
            teXentry.OutMaps = FTED.DDSData;

            //Gets the path of the selected node to inject here.
            string nodepath = tree.SelectedNode.FullPath;
            nodepath = nodepath.Substring(nodepath.IndexOf("\\") + 1);

            string[] sepstr = { "\\" };
            teXentry.EntryDirs = nodepath.Split(sepstr, StringSplitOptions.RemoveEmptyEntries);
            teXentry.EntryName = teXentry.FileName;

            //Lastly the bitmap for the png preview.
            teXentry.Picture = FTED.TXpreview;


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
                    ByteUtilitarian.FromDDSToTex(teXentry, bnr, FTED);
                }

                //Filling in the rest of the data for the teXentry.
                teXentry.TrueName = FTED.ShortName;
                teXentry._FileName = teXentry.TrueName;
                FTED.TXfilename = teXentry.EntryName;
                FTED.TXfilename = teXentry.TrueName;
                teXentry._FileName = teXentry.TrueName;
                teXentry.FileExt = ".tex";
                teXentry.DSize = teXentry.UncompressedData.Length;
                teXentry.CSize = teXentry.CompressedData.Length;
                teXentry.OutMaps = FTED.DDSData;

                //Gets the path of the selected node to inject here.
                string nodepath = tree.SelectedNode.FullPath;
                nodepath = nodepath.Substring(nodepath.IndexOf("\\") + 1);

                string[] sepstr = { "\\" };
                teXentry.EntryDirs = nodepath.Split(sepstr, StringSplitOptions.RemoveEmptyEntries);
                teXentry.EntryName = teXentry.FileName;

                //Lastly the bitmap for the png preview.
                teXentry.Picture = FTED.TXpreview;

            }
            catch (Exception ex)
            {
                string ProperPath = ""; ProperPath = Globals.ToolPath + "Log.txt"; using (StreamWriter sw = File.AppendText(ProperPath))
                {
                    sw.WriteLine("Texture replacement from .DDS file failed. Here's details:\n" + ex);
                }
            }


            //teXentry.OutMaps = ;

            return teXentry;
        }

        //A whole new texture reading function for the future.
        public static TextureEntry BuildTextureEntry(TextureEntry texentry, BinaryReader bnr)
        {
            bnr.BaseStream.Position = 0;
            texentry.Magic = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), texentry.Magic);

            //Tweaking my old method for reading the header data.
            List<byte> BTemp = new List<byte>();
            byte[] VTemp = new byte[4];
            uint[] RawHeaderData = new uint[3];
            byte[] DTemp = new byte[4];
            RawHeaderData[0] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);
            RawHeaderData[1] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);
            RawHeaderData[2] = BitConverter.ToUInt32(bnr.ReadBytes(4), 0);

            texentry.Dimensions = Convert.ToInt32(((RawHeaderData[0] >> 28) & 0xf));
            texentry.MipMapCount = Convert.ToInt32(RawHeaderData[1] & 0x3f);
            texentry.XSize = Convert.ToInt32(((RawHeaderData[1] >> 6) & 0x1FFF));
            texentry.YSize = Convert.ToInt32(((RawHeaderData[1] >> 19) & 0x1FFF));

            texentry.X = texentry.XSize;
            texentry.Y = texentry.YSize;

            texentry.SurfaceCount = Convert.ToInt32(RawHeaderData[2] & 0x3f);
            texentry.Type = Convert.ToInt32(((RawHeaderData[2] >> 8) & 0xFF));
            texentry.UnkField03 = Convert.ToInt32(((RawHeaderData[2] >> 16) & 0x1FFF));
            texentry.UnkField04 = Convert.ToInt32(((RawHeaderData[2] >> 29) & 0x3));

            texentry.Faces = new List<RTextureFace>();
            texentry.TexType = Convert.ToString(texentry.Type);
            texentry.TextureType = texentry.TexType;
            //Now for the second part. Reserving this for the Cube Map first.

            #region Cube Maps

            if (texentry.Dimensions == 6)
            {
                texentry._Format = "Cube Map(DXT1)";
                texentry.IsCubeMap = true;

                //Faces.
                for (int y = 0; y < 3; y++)
                {
                    RTextureFace rFace = new RTextureFace();
                    rFace = RTextureFace.ReadFaceFromTEX(bnr);
                    texentry.Faces.Add(rFace);
                }
            }

            #endregion
            else
            {
                switch (texentry.Type)
                {
                    case 0x13:
                        texentry._Format = "DXT1/BC1";
                        break;

                    case 0x15:
                        texentry._Format = "Alternate DXT5/BC3";
                        break;

                    case 0x17:
                        texentry._Format = "DXT5/BC3";
                        break;

                    case 0x19:
                        texentry._Format = "BC4_UNORM/Metalic/Specular Map";
                        break;

                    case 0x1E:
                        texentry._Format = "Cloth (Undocumented)";
                        break;

                    case 0x1F:
                        texentry._Format = "BC5/Normal Map";
                        break;

                    case 0x27:
                        texentry._Format = "RGBA Linear Texture";
                        break;

                    case 0x2A:
                        texentry._Format = "LAB Color/Problematic UI Texture";
                        break;

                    default:
                        break;
                }

            }

            int TotalMips = texentry.SurfaceCount * texentry.MipMapCount;

            //Next we gotta read the flat mip offset table. Turns out these are unsigned longs instead of unsigned integers...
            texentry.MipOffsets = new List<ulong>();
            for (int j = 0; j < TotalMips; j++)
            {
                texentry.MipOffsets.Add(bnr.ReadUInt64());
            }

            //Now we have to rebuild each surface and MipMap chain from the above offsets.
            texentry.Surfaces = new List<List<byte[]>>();
            for (int k = 0; k < texentry.SurfaceCount; k++)
            {
                var MipMaps = new List<byte[]>();
                //Needs to be done carefully unlike other formats.
                for (int l = 0; l < texentry.MipMapCount; l++)
                {
                    ulong Offset = texentry.MipOffsets[k * texentry.MipMapCount + l];

                    int NextID = k * texentry.MipMapCount + l + 1;
                    ulong NextOffset = NextID < TotalMips ? texentry.MipOffsets[NextID] : (ulong)texentry.UncompressedData.Length;

                    int MipSize = (int)(NextOffset - Offset);
                    bnr.BaseStream.Seek((long)Offset, SeekOrigin.Begin);
                    MipMaps.Add(bnr.ReadBytes(MipSize));
                }
                texentry.Surfaces.Add(MipMaps);
            }

            //Debug Export. Gotta use this conversion to DDS for the export as well.
            texentry.OutTexTest = new List<byte>();

            //OutMaps = Surfaces?
            using (var memstream = new MemoryStream())
            using (var bnrforDDS = new BinaryWriter(memstream))
            {

                uint fourCC = RTextureSurfaceFormat.GetDDSFourCC(texentry.Type);
                bool IsCompressed = fourCC != DDSConstants.FOURCC_NONE;
                int Blocksize = RTextureSurfaceFormat.GetBlockSize(texentry.Type);

                //Magic.
                bnrforDDS.Write((uint)0x20534444);

                //DDS Header.
                bnrforDDS.Write((uint)124);

                uint dwFlags = DDSConstants.DDSD_CAPS | DDSConstants.DDSD_HEIGHT | DDSConstants.DDSD_WIDTH | DDSConstants.DDSD_PIXELFORMAT | DDSConstants.DDSD_LINEARSIZE;

                if (texentry.MipMapCount > 1)
                {
                    dwFlags |= DDSConstants.DDSD_MIPMAPCOUNT;
                }

                bnrforDDS.Write(dwFlags);

                //Length and Width.
                bnrforDDS.Write((uint)texentry.YSize);
                bnrforDDS.Write((uint)texentry.XSize);


                uint pitchOrLinearSize = IsCompressed
                    ? (uint)DDSCalc.LinearSizeBlockCompressed(texentry.XSize, texentry.YSize, Blocksize)
                    : (uint)DDSCalc.PitchBpp(texentry.XSize, 32); // RGBA assumed for LIN
                bnrforDDS.Write(pitchOrLinearSize);

                bnrforDDS.Write((uint)0);
                bnrforDDS.Write((uint)texentry.MipMapCount);
                for (int i = 0; i < 11; i++)
                {
                    bnrforDDS.Write((uint)0);
                }

                //Now for the DDS_PIXELFORMAT section.
                bnrforDDS.Write((uint)32);
                if (IsCompressed)
                {
                    bnrforDDS.Write(DDSConstants.DDPF_FOURCC);
                    bnrforDDS.Write(fourCC);
                    // dwRGBBitCount, dwRBitMask, dwGBitMask, dwBBitMask, dwABitMask
                    for (int i = 0; i < 5; i++)
                    {
                        bnrforDDS.Write((uint)0);
                    }
                }
                else
                {
                    // Uncompressed RGBA (LIN)
                    bnrforDDS.Write(DDSConstants.DDPF_RGBA);
                    bnrforDDS.Write((uint)0);           //dwFourCC (unused)
                    bnrforDDS.Write((uint)32);          //dwRGBBitCount
                    bnrforDDS.Write((uint)0x000000FF);  //dwRBitMask
                    bnrforDDS.Write((uint)0x0000FF00);  //dwGBitMask
                    bnrforDDS.Write((uint)0x00FF0000);  //dwBBitMask
                    bnrforDDS.Write((uint)0xFF000000);  //dwABitMask
                }

                uint dwCaps = DDSConstants.DDSCAPS_TEXTURE;
                if (texentry.MipMapCount > 1)
                {
                    dwCaps |= DDSConstants.DDSCAPS_MIPMAP;
                }
                if (texentry.IsCubeMap)
                {
                    dwCaps |= DDSConstants.DDSCAPS_COMPLEX;
                }

                bnrforDDS.Write(dwCaps);

                uint dwCaps2 = texentry.IsCubeMap ? DDSConstants.DDS_CUBEMAP_ALLFACES : 0u;
                bnrforDDS.Write(dwCaps2);


                bnrforDDS.Write((uint)0);
                bnrforDDS.Write((uint)0);
                bnrforDDS.Write((uint)0);

                //Now for the pixel data.
                foreach (var Surface in texentry.Surfaces)
                    foreach (var mip in Surface)
                        bnrforDDS.Write(mip);

                texentry.OutTexTest.AddRange(memstream.ToArray());

                //#if DEBUG
                //                File.WriteAllBytes("D:\\Workshop\\LMTHub\\Test\\DDSTEST2026" + ".DDS", texentry.OutTexTest.ToArray());
                //#endif

                if (texentry.IsCubeMap)
                {
                    texentry.Picture = ByteUtilitarian.CubemapToCross(texentry.OutTexTest.ToArray());
                }
                else
                {
                    texentry.Picture = ByteUtilitarian.NewBitmapBuilder(texentry.OutTexTest.ToArray());
                }
                bnrforDDS.Flush();
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

                //PixelFormat format = PixelFormat.Format32bppArgb;
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
        [Category("Texture"), ReadOnlyAttribute(true)]
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
        [Category("Texture"), ReadOnlyAttribute(true)]
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

        //private int _MipMapCount;
        //[Category("Filename"), ReadOnlyAttribute(true)]
        //public int MipMapCount
        //{

        //    get
        //    {
        //        return _MipMapCount;
        //    }
        //    set
        //    {
        //        _MipMapCount = value;
        //    }
        //}

        private string _Format;
        [Category("Texture"), ReadOnlyAttribute(true)]
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


    }
}
