//Based heavily on code from TGE's model importer.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ThreeWorkTool.Resources.Wrappers.ExtraNodes
{
    //This is specific class is written in a way that makes it easier to build .tex files.
    public class RTextureHeader
    {

        public uint Magic = 0x00584554;

        private uint _desc;
        public uint Desc { get => _desc; set => _desc = value; }
        //This should always be 0xA09D.
        public int TypeVal { get => (int)(_desc & 0xFFFF); set => _desc = (_desc & ~0xFFFFu) | ((uint)(value & 0xFFFF)); }
        //6 for cube map, while equaling 2 for everything else.
        public int Dimensions { get => (int)((_desc >> 28) & 0xF); set => _desc = (_desc & ~(0xFu << 28)) | ((uint)(value & 0xF) << 28); }

        public const int DefaultTypeVal = 0xA09D;

        //This next word has bits used for the MipMapCount, Width, and Height.
        private uint _dim;
        public uint Dim { get => _dim; set => _dim = value; }
        public int MipCount { get => (int)(_dim & 0x3F); set => _dim = (_dim & ~0x3Fu) | ((uint)(value & 0x3F)); }
        public int Width { get => (int)((_dim >> 6) & 0x1FFF); set => _dim = (_dim & ~(0x1FFFu << 6)) | ((uint)(value & 0x1FFF) << 6); }
        public int Height { get => (int)((_dim >> 19) & 0x1FFF); set => _dim = (_dim & ~(0x1FFFu << 19)) | ((uint)(value & 0x1FFF) << 19); }

        //Yet another bitpacked word.
        private uint _fmt;
        public uint Fmt { get => _fmt; set => _fmt = value; }
        public int SurfaceCount { get => (int)(_fmt & 0xFF); set => _fmt = (_fmt & ~0xFFu) | ((uint)(value & 0xFF)); }
        public int SurfaceFmt { get => (int)((_fmt >> 8) & 0xFF); set => _fmt = (_fmt & ~(0xFFu << 8)) | ((uint)(value & 0xFF) << 8); }
        public int Field3 { get => (int)((_fmt >> 16) & 0x1FFF); set => _fmt = (_fmt & ~(0x1FFFu << 16)) | ((uint)(value & 0x1FFF) << 16); }
        public int Field4 { get => (int)((_fmt >> 29) & 0x3); set => _fmt = (_fmt & ~(0x3u << 29)) | ((uint)(value & 0x3) << 29); }

        public void Read(BinaryReader r)
        {
            Magic = r.ReadUInt32();
            _desc = r.ReadUInt32();
            _dim = r.ReadUInt32();
            _fmt = r.ReadUInt32();
        }

        //Gotta get _desc setup first before writing the rest of the header.
        public void Write(BinaryWriter w)
        {
            w.Write(Magic);
            w.Write(_desc);
            w.Write(_dim);
            w.Write(_fmt);
        }
    }

    public class RTextureData
    {
        public RTextureHeader Header = new RTextureHeader();
        public RTextureFace[] Faces = Array.Empty<RTextureFace>();
        //Raw Pixel bytes.
        public List<List<byte[]>> Surfaces = new List<List<byte[]>>();

        public static RTextureData FromFile(string path)
        {
            var tex = new RTextureData();
            var r = new BinaryReader(File.OpenRead(path));
            tex.Read(r, (int)new FileInfo(path).Length);
            return tex;
        }

        private void Read(BinaryReader r, int fileSize)
        {
            Header.Read(r);

            //Cubemaps store 3 face lighting records immediately after the header.
            if (Header.Dimensions == 6)
            {
                Faces = new RTextureFace[3];
                for (int i = 0; i < 3; i++)
                    Faces[i] = RTextureFace.ReadFaceFromTEX(r);
            }

            int surfCount = Header.SurfaceCount;
            int mipCount = Header.MipCount;
            int totalMips = surfCount * mipCount;

            ulong[] mipOffsets = new ulong[totalMips];
            for (int i = 0; i < totalMips; i++)
                mipOffsets[i] = r.ReadUInt64();

            //Rebuild each surface and its mip chain from offsets.
            Surfaces = new List<List<byte[]>>();
            for (int s = 0; s < surfCount; s++)
            {
                var mips = new List<byte[]>();
                for (int m = 0; m < mipCount; m++)
                {
                    ulong off = mipOffsets[s * mipCount + m];

                    int nextIdx = s * mipCount + m + 1;
                    ulong nextOff = nextIdx < totalMips
                        ? mipOffsets[nextIdx]
                        : (ulong)fileSize;

                    int size = (int)(nextOff - off);
                    r.BaseStream.Seek((long)off, SeekOrigin.Begin);
                    mips.Add(r.ReadBytes(size));
                }
                Surfaces.Add(mips);
            }
        }

        //Made a tweaked function just for writing to a byte array.
        public byte[] WriteToBytes()
        {
            var ms = new MemoryStream();
            var w = new BinaryWriter(ms);
            Write(w);
            return ms.ToArray();
        }

        public void SaveToFile(string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? ".");
            var w = new BinaryWriter(File.Create(path));
            Write(w);
        }

        public void Write(BinaryWriter w)
        {
            Header.Write(w);

            foreach (var face in Faces)
                face.Write(w);

            long tableStart = w.BaseStream.Position;
            int surfCount = Surfaces.Count;
            int mipCount = Surfaces[0].Count;
            int totalMips = surfCount * mipCount;

            //Placeholder Zeros to reserve space for the Offset Table.
            for (int i = 0; i < totalMips; i++)
                w.Write(0UL);

            long dataStart = w.BaseStream.Position;
            long tablePos = tableStart;
            long writePos = dataStart;

            for (int s = 0; s < surfCount; s++)
            {
                for (int m = 0; m < mipCount; m++)
                {
                    w.BaseStream.Seek(tablePos, SeekOrigin.Begin);
                    w.Write((ulong)writePos);
                    tablePos += 8;

                    w.BaseStream.Seek(writePos, SeekOrigin.Begin);
                    w.Write(Surfaces[s][m]);
                    writePos += Surfaces[s][m].Length;
                }
            }
        }

        //This function adapted from the model importer handles the process from .tex to .dds.
        public byte[] ToDds()
        {
            bool hasMips = Header.MipCount > 1;
            bool isCubeMap = Header.SurfaceCount > 1;
            uint fourCC = RTextureSurfaceFormat.GetDDSFourCC(Header.SurfaceFmt);
            bool isCompressed = fourCC != DDSConstants.FOURCC_NONE;
            //Block size is either 8 bytes, 16 bytes, or none for LIN maps.
            int blockSize = RTextureSurfaceFormat.GetBlockSize(Header.SurfaceFmt);

            //Build the 128-byte header DDS Files have.

            var ms = new MemoryStream();
            var w = new BinaryWriter(ms);

            //Magic.
            w.Write((uint)0x20534444);

            w.Write((uint)124);

            //dwFlags, which are in the header.
            uint dwFlags = DDSConstants.DDSD_CAPS
                         | DDSConstants.DDSD_HEIGHT
                         | DDSConstants.DDSD_WIDTH
                         | DDSConstants.DDSD_PIXELFORMAT
                         | DDSConstants.DDSD_LINEARSIZE;
            if (hasMips) dwFlags |= DDSConstants.DDSD_MIPMAPCOUNT;
            w.Write(dwFlags);

            //Height & Width.
            w.Write((uint)Header.Height);
            w.Write((uint)Header.Width);

            uint pitchOrLinearSize = isCompressed
                ? (uint)DDSCalc.LinearSizeBlockCompressed(Header.Width, Header.Height, blockSize)
                : (uint)DDSCalc.PitchBpp(Header.Width, 32);
            w.Write(pitchOrLinearSize);
            w.Write((uint)0);

            //MipMap count.
            w.Write((uint)Header.MipCount);

            for (int i = 0; i < 11; i++) w.Write((uint)0);

            //This deals with the DDS Pixel Format.
            w.Write((uint)32);
            if (isCompressed)
            {
                w.Write(DDSConstants.DDPF_FOURCC);
                w.Write(fourCC);
                for (int i = 0; i < 5; i++) w.Write((uint)0);
            }
            else
            {
                //For Uncompressed RGBA Maps(LIN).
                w.Write(DDSConstants.DDPF_RGBA);
                w.Write((uint)0);          
                w.Write((uint)32);         
                w.Write((uint)0x000000FF);  
                w.Write((uint)0x0000FF00);  
                w.Write((uint)0x00FF0000);  
                w.Write((uint)0xFF000000); 
            }

            uint dwCaps = DDSConstants.DDSCAPS_TEXTURE;
            if (hasMips) dwCaps |= DDSConstants.DDSCAPS_MIPMAP;
            if (isCubeMap) dwCaps |= DDSConstants.DDSCAPS_COMPLEX;
            w.Write(dwCaps);

            //Flags used for Cubemaps.
            uint dwCaps2 = isCubeMap ? DDSConstants.DDS_CUBEMAP_ALLFACES : 0u;
            w.Write(dwCaps2);

            w.Write((uint)0);
            w.Write((uint)0);
            w.Write((uint)0);

            //Related to Pixel Data.
            foreach (var surface in Surfaces)
                foreach (var mip in surface)
                    w.Write(mip);

            return ms.ToArray();
        }

        public void SaveDds(string path)
        {
            byte[] ddsBytes = ToDds();
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? ".");
            File.WriteAllBytes(path, ddsBytes);
        }


        //Constructs RTextureData from a raw .dds byte array.
        public static RTextureData FromDds(byte[] ddsBytes,
                                           string textureName = null,
                                           bool hasAlpha = false)
        {
            var r = new BinaryReader(new MemoryStream(ddsBytes));
            return FromDds(r, textureName, hasAlpha);
        }

        //This one from a BinaryReader of a .dds file or .dds data.
        public static RTextureData FromDds(BinaryReader r,
                                           string textureName = null,
                                           bool hasAlpha = false)
        {
            uint magic = r.ReadUInt32();
            uint dwSize = r.ReadUInt32();
            uint dwFlags = r.ReadUInt32();
            uint dwHeight = r.ReadUInt32();
            uint dwWidth = r.ReadUInt32();
            uint dwPitch = r.ReadUInt32();
            uint dwDepth = r.ReadUInt32();
            uint dwMipCnt = r.ReadUInt32();
            r.ReadBytes(11 * 4);

            //DDS_PIXELFORMAT
            uint pfSize = r.ReadUInt32();
            uint pfFlags = r.ReadUInt32();
            uint pfFourCC = r.ReadUInt32();
            uint pfBitCnt = r.ReadUInt32();
            uint pfRMask = r.ReadUInt32();
            uint pfGMask = r.ReadUInt32();
            uint pfBMask = r.ReadUInt32();
            uint pfAMask = r.ReadUInt32();

            uint dwCaps = r.ReadUInt32();
            uint dwCaps2 = r.ReadUInt32();
            r.ReadBytes(3 * 4);

            bool isCubeMap = (dwCaps2 & DDSConstants.DDSCAPS2_CUBEMAP) != 0;
            int surfCount = isCubeMap ? 6 : 1;
            int mipCount = (int)Math.Max(1, dwMipCnt);
            bool isCompressed = (pfFlags & DDSConstants.DDPF_FOURCC) != 0;

            //Determines the Format.
            int texFmt;
            if (textureName != null)
            {
                texFmt = RTextureSurfaceFormat.GetFormatFromTextureName(textureName, hasAlpha)
                         ?? RTextureSurfaceFormat.BM_OPA;
            }
            else if (!isCompressed)
            {
                texFmt = RTextureSurfaceFormat.LIN;
            }
            else
            {
                texFmt = (pfFourCC == DDSConstants.FOURCC_DXT1)
                    ? RTextureSurfaceFormat.BM_OPA
                    : RTextureSurfaceFormat.BM_XLU;
            }

            //Block size is now driven by the resolved texFmt, not the raw DDS FourCC.
            int blockSize = RTextureSurfaceFormat.GetBlockSize(texFmt);

            //Read the pixel data.
            var surfaces = new List<List<byte[]>>();
            for (int s = 0; s < surfCount; s++)
            {
                var mips = new List<byte[]>();
                int w = (int)dwWidth, h = (int)dwHeight;
                for (int m = 0; m < mipCount; m++)
                {
                    int size = (blockSize > 0)
                        ? DDSCalc.LinearSizeBlockCompressed(w, h, blockSize)
                        : DDSCalc.LinearSizeBpp(w, h, (int)pfBitCnt);
                    mips.Add(r.ReadBytes(size));
                    w = Math.Max(1, w / 2);
                    h = Math.Max(1, h / 2);
                }
                surfaces.Add(mips);
            }

            //Assemble the raw .tex.
            var tex = new RTextureData { Surfaces = surfaces };
            tex.Header.MipCount = mipCount;
            tex.Header.Width = (int)dwWidth;
            tex.Header.Height = (int)dwHeight;
            tex.Header.SurfaceFmt = texFmt;
            tex.Header.Field3 = 1;
            tex.Header.Field4 = 0;
            tex.Header.SurfaceCount = surfaces.Count;
            if (isCubeMap)
            {
                tex.Header.Dimensions = 6;
                tex.SetDefaultCubeMapFaces();
            }
            return tex;
        }

        //For default cubemap face data.
        public void SetDefaultCubeMapFaces()
        {
            Faces = new RTextureFace[3];

            Faces[0] = new RTextureFace
            {
                Unkown00 = 0.4837379f,
                NegativeX = -0.05054046f,
                NegativeY = -0.04008521f,
                NegativeZ = -0.01001853f,
                PositiveX = 0.005333615f,
                PositiveY = -0.03039154f,
                PositiveZ = -0.1364427f,
                U = 0.01015308f,
                V = 0.02520043f
            };
            Faces[1] = new RTextureFace
            {
                Unkown00 = 1.357952f,
                NegativeX = -0.009200307f,
                NegativeY = -0.04620323f,
                NegativeZ = 0.02067803f,
                PositiveX = 0.0100703f,
                PositiveY = -0.09296682f,
                PositiveZ = 0.2509833f,
                U = 0.02939349f,
                V = 0.03288069f
            };
            Faces[2] = new RTextureFace
            {
                Unkown00 = 1.029677f,
                NegativeX = -0.04863077f,
                NegativeY = 0.01793304f,
                NegativeZ = 0.02242514f,
                PositiveX = 0.009546677f,
                PositiveY = -0.02904901f,
                PositiveZ = 0.221782f,
                U = 0.03200468f,
                V = 0.06776269f
            };
        }
    }

    public class RTextureFace
    {
        public float Unkown00;
        public float NegativeX;
        public float NegativeY;
        public float NegativeZ;
        public float PositiveX;
        public float PositiveY;
        public float PositiveZ;
        public float U;
        public float V;

        //4 + 12 + 12 + 8 = 36.
        public const int FaceSize = 36;

        public static RTextureFace ReadFaceFromTEX(BinaryReader bnr)
        {

            RTextureFace face = new RTextureFace();
            face.Unkown00 = bnr.ReadSingle();
            face.NegativeX = bnr.ReadSingle();
            face.NegativeY = bnr.ReadSingle();
            face.NegativeZ = bnr.ReadSingle();
            face.PositiveX = bnr.ReadSingle();
            face.PositiveY = bnr.ReadSingle();
            face.PositiveZ = bnr.ReadSingle();
            face.U = bnr.ReadSingle();
            face.V = bnr.ReadSingle();
            return face;

        }

        public void Write(BinaryWriter w)
        {
            w.Write(Unkown00);
            w.Write(NegativeX); w.Write(NegativeY); w.Write(NegativeZ);
            w.Write(PositiveX); w.Write(PositiveY); w.Write(PositiveZ);
            w.Write(U); w.Write(V);
        }

    }

    //Taken & Adapted from TGE's DDS Library.
    //Also thank goodness that 010 Editor has a easily available DDS Template to fill in the rest.

    public static class DDSConstants
    {
        public static readonly uint FOURCC_DXT1 = MakeFourCC('D', 'X', 'T', '1');
        public static readonly uint FOURCC_DXT2 = MakeFourCC('D', 'X', 'T', '2');
        public static readonly uint FOURCC_DXT3 = MakeFourCC('D', 'X', 'T', '3');
        public static readonly uint FOURCC_DXT4 = MakeFourCC('D', 'X', 'T', '4');
        public static readonly uint FOURCC_DXT5 = MakeFourCC('D', 'X', 'T', '5');
        public const uint FOURCC_NONE = 0;

        //Has to be in all .DDS files.
        public const uint DDSD_CAPS = 0x00000001;
        public const uint DDSD_HEIGHT = 0x00000002;
        public const uint DDSD_WIDTH = 0x00000004;
        //According to TGE's comment, Pitch is provided for uncompressed textures.
        public const uint DDSD_PITCH = 0x00000008;
        public const uint DDSD_PIXELFORMAT = 0x00001000;
        public const uint DDSD_MIPMAPCOUNT = 0x00020000;
        public const uint DDSD_LINEARSIZE = 0x00080000;
        //For Depth Textures.
        public const uint DDSD_DEPTH = 0x00800000;

        //DDS Caps.
        public const uint DDSCAPS_COMPLEX = 0x00000008;
        public const uint DDSCAPS_MIPMAP = 0x00400000;
        public const uint DDSCAPS_TEXTURE = 0x00001000;

        //Params for CubeMap faces.
        public const uint DDSCAPS2_CUBEMAP = 0x00000200;
        public const uint DDSCAPS2_CUBEMAP_POSITIVEX = 0x00000400;
        public const uint DDSCAPS2_CUBEMAP_NEGATIVEX = 0x00000800;
        public const uint DDSCAPS2_CUBEMAP_POSITIVEY = 0x00001000;
        public const uint DDSCAPS2_CUBEMAP_NEGATIVEY = 0x00002000;
        public const uint DDSCAPS2_CUBEMAP_POSITIVEZ = 0x00004000;
        public const uint DDSCAPS2_CUBEMAP_NEGATIVEZ = 0x00008000;
        public const uint DDS_CUBEMAP_ALLFACES =
              DDSCAPS2_CUBEMAP
            | DDSCAPS2_CUBEMAP_POSITIVEX | DDSCAPS2_CUBEMAP_NEGATIVEX
            | DDSCAPS2_CUBEMAP_POSITIVEY | DDSCAPS2_CUBEMAP_NEGATIVEY
            | DDSCAPS2_CUBEMAP_POSITIVEZ | DDSCAPS2_CUBEMAP_NEGATIVEZ;

        public const uint DDPF_ALPHAPIXELS = 0x00000001;
        public const uint DDPF_ALPHA = 0x00000002;
        public const uint DDPF_FOURCC = 0x00000004;
        public const uint DDPF_RGB = 0x00000040;
        public const uint DDPF_YUV = 0x00000200;
        public const uint DDPF_LUMINANCE = 0x00002000;
        public const uint DDPF_RGBA = DDPF_RGB | DDPF_ALPHAPIXELS;
        public const uint DDPF_LUMINANCEA = DDPF_LUMINANCE | DDPF_ALPHAPIXELS;

        public const uint DDS_SURFACE_FLAGS_MIPMAP = DDSCAPS_COMPLEX | DDSCAPS_MIPMAP;
        public const uint DDS_SURFACE_FLAGS_TEXTURE = DDSCAPS_TEXTURE;
        public const uint DDS_SURFACE_FLAGS_CUBEMAP = DDSCAPS_COMPLEX;

        //More params for CubMapFaces. These are for indivdual faces apparently.
        public const uint DDS_CUBEMAP_POSITIVEX = DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_POSITIVEX;
        public const uint DDS_CUBEMAP_NEGATIVEX = DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_NEGATIVEX;
        public const uint DDS_CUBEMAP_POSITIVEY = DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_POSITIVEY;
        public const uint DDS_CUBEMAP_NEGATIVEY = DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_NEGATIVEY;
        public const uint DDS_CUBEMAP_POSITIVEZ = DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_POSITIVEZ;
        public const uint DDS_CUBEMAP_NEGATIVEZ = DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_NEGATIVEZ;

        public static uint MakeFourCC(char a, char b, char c, char d)
            => (uint)a | ((uint)b << 8) | ((uint)c << 16) | ((uint)d << 24);
    }

    public static class DDSCalc
    {

        public static int PitchBlockCompressed(int width, int blockSize)
            => Math.Max(1, (width + 3) / 4) * blockSize;

        public static int PitchRgb(int width)
            => ((width + 1) >> 1) * 4;

        public static int PitchBpp(int width, int bitsPerPixel)
            => Math.Max(1, (width * bitsPerPixel + 7) / 8);

        public static int LinearSizeBlockCompressed(int width, int height, int blockSize)
        {
            int blocksW = Math.Max(1, (width + 3) / 4);
            int blocksH = Math.Max(1, (height + 3) / 4);
            return blocksW * blocksH * blockSize;
        }

        public static int LinearSizeBpp(int width, int height, int bitsPerPixel)
            => width * height * (bitsPerPixel / 8);
    }

    public static class RTextureSurfaceFormat
    {
        //Gonna preserve TGE's comments for reference here.
        // OPA: Opaque
        // XLU: translucent
        public const int BM_OPA = 19;  //DXT1  – opaque base map
        public const int DXT1_20 = 20;  //DXT1  – unused in MvC3
        public const int DXT5_21 = 21;  //DXT5  – unused in MvC3
        public const int DXT5_22 = 22;  //DXT5  – unused in MvC3
        public const int BM_XLU = 23;  //DXT5  – translucent base map
        public const int DXT5_24 = 24;  //DXT5  – unused in MvC3
        public const int MM_OPA = 25;  //DXT1  – opaque mask/material map
        public const int DXT1_26 = 26;  //DXT1  – unused in MvC3
        public const int DXT5_27 = 27;  //DXT5  – unused in MvC3
        public const int DXT1_30 = 30;  //DXT1  – unused in MvC3
        public const int NM = 31;  //DXT5  – normal map
        public const int DXT5_32 = 32;  //DXT5  – unused in MvC3
        public const int DXT5_33 = 33;  //DXT5  – unused in MvC3
        public const int DXT5_35 = 35;  //DXT5  – unused in MvC3
        public const int DXT5_36 = 36;  //DXT5  – unused in MvC3
        public const int DXT5_37 = 37;  //DXT5  – unused in MvC3
        public const int LIN = 39;  //RGBA  – used for linear textures/ramps
        public const int DXT1_41 = 41;  //DXT1  – unused in MvC3
        public const int BM_HQ = 42;  //DXT5  – high-quality base map
        public const int DXT5_43 = 43;  //DXT5  – unused in MvC3
        public const int DXT5_47 = 47;  //DXT5  – unused in MvC3

        //Based on the _specialTextureNames dictionary in rtexture.py.
        private static readonly Dictionary<string, int> SpecialNames =
            new Dictionary<string, int>(StringComparer.Ordinal)
            {
                { "DEAmoji00_MM",     DXT5_21 },
                { "yari_MM",          LIN     },
                { "Wesker_nuno_DM",   DXT1_30 },
                { "DefaultCube_CM",   DXT5_32 },
                { "ZERmoyou02",       LIN     },
                { "ZERmoyoured02",    LIN     },
                { "ZERmoyouyellow02", LIN     },
            };

        //Returns the DDS FourCC for a given surfaceFmt value.
        public static uint GetDDSFourCC(int surfaceFmt)
        {
            switch (surfaceFmt)
            {
                case BM_OPA:
                case DXT1_20:
                case MM_OPA:
                case DXT1_26:
                case DXT1_30:
                case DXT1_41:
                    return DDSConstants.FOURCC_DXT1;

                case DXT5_21:
                case DXT5_22:
                case BM_XLU:
                case DXT5_24:
                case DXT5_27:
                case NM:
                case DXT5_32:
                case DXT5_33:
                case DXT5_35:
                case DXT5_36:
                case DXT5_37:
                case BM_HQ:
                case DXT5_43:
                case DXT5_47:
                    return DDSConstants.FOURCC_DXT5;

                case LIN:
                    return DDSConstants.FOURCC_NONE;

                default:
                    throw new NotSupportedException($"Unknown Surface Format: {surfaceFmt}");
            }
        }

        //Returns the DXT block size in bytes for a given Surface Format.
        public static int GetBlockSize(int surfaceFmt)
        {
            uint fourCC = GetDDSFourCC(surfaceFmt);
            if (fourCC == DDSConstants.FOURCC_DXT1) return 8;
            if (fourCC == DDSConstants.FOURCC_NONE) return 0;  //For LIN and RGBA.
            return 16;
        }

        //Bascially a replica of the model importer's function of a similar name that uses the suffix to infer details on the incoming texture.
        public static int? GetFormatFromTextureName(string baseName, bool hasAlpha)
        {
            if (SpecialNames.TryGetValue(baseName, out int special))
                return special;

            if (baseName.Contains("_BM"))
            {
                if (baseName.Contains("toon")) return LIN;
                if (baseName.Contains("_HQ")) return BM_HQ;
                return hasAlpha ? BM_XLU : BM_OPA;
            }

            if (baseName.Contains("_LM") ||
                baseName.Contains("_CM") ||
                baseName.Contains("_NUKI"))
                return BM_OPA;

            if (baseName.Contains("_AM") ||
                baseName.Contains("_MM"))
                return MM_OPA;

            if (baseName.Contains("_DM") ||
                baseName.Contains("_NM"))
                return NM;

            if (baseName.Contains("_LIN"))
                return LIN;

            return null;
        }
    }

}
