using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zlib;
using System.IO;
using System.Runtime.InteropServices;
using System.Numerics;
using System.Drawing;
using ThreeWorkTool.Resources.Wrappers;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Collections;
using System.Security.Cryptography;
using System.Buffers.Binary;
using Force.Crc32;
using OpenTK;
using static ThreeWorkTool.Resources.Utility.Mvc3ShaderDatabase;
using ThreeWorkTool.Resources.Wrappers.ExtraNodes;

namespace ThreeWorkTool.Resources.Utility
{
    //This class is to store all the functions relating to bytes and conversions I used frequently in several classes and functions.
    public class ByteUtilitarian
    {
        public static StringBuilder SBname;

        //This is to convert a Matrix for use in the Model Viewer.
        public static Matrix4 FromSystemNumericsMatrixToOpenTKMatrix(System.Numerics.Matrix4x4 m)
        {
            return new OpenTK.Matrix4(
                m.M11, m.M12, m.M13, m.M14,
                m.M21, m.M22, m.M23, m.M24,
                m.M31, m.M32, m.M33, m.M34,
                m.M41, m.M42, m.M43, m.M44
            );
        }

        public void DecomposeMatrix(Matrix4 m, out OpenTK.Vector3 position, out OpenTK.Quaternion rotation, out OpenTK.Vector3 scale)
        {
            position = m.ExtractTranslation();
            scale = m.ExtractScale();

            var rotMatrix = new Matrix4(
                m.Column0 / (scale.X == 0 ? 1 : scale.X),
                m.Column1 / (scale.Y == 0 ? 1 : scale.Y),
                m.Column2 / (scale.Z == 0 ? 1 : scale.Z),
                m.Column3
            );

            rotation = rotMatrix.ExtractRotation();
        }

        // Gets World Position from 4x4 Matrix.
        public static OpenTK.Vector3 GetPosition(System.Numerics.Matrix4x4 m) => new OpenTK.Vector3(m.M41, m.M42, m.M43);

        public static string BytesToString(byte[] bytes, string s)
        {
            string temps;
            string tru = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                temps = bytes[i].ToString("X");
                if (temps == "0")
                {
                    temps = "00";
                }
                else if (temps == "1")
                {
                    temps = "01";
                }
                else if (temps == "2")
                {
                    temps = "02";
                }
                else if (temps == "3")
                {
                    temps = "03";
                }
                else if (temps == "4")
                {
                    temps = "04";
                }
                else if (temps == "5")
                {
                    temps = "05";
                }
                else if (temps == "6")
                {
                    temps = "06";
                }
                else if (temps == "7")
                {
                    temps = "07";
                }
                else if (temps == "8")
                {
                    temps = "08";
                }
                else if (temps == "9")
                {
                    temps = "09";
                }
                else if (temps == "A")
                {
                    temps = "0A";
                }
                else if (temps == "B")
                {
                    temps = "0B";
                }
                else if (temps == "C")
                {
                    temps = "0C";
                }
                else if (temps == "D")
                {
                    temps = "0D";
                }
                else if (temps == "E")
                {
                    temps = "0E";
                }
                else if (temps == "F")
                {
                    temps = "0F";
                }
                tru += temps;
            }
            return tru;
        }

        public static string BytesToStringL(List<byte> bytes)
        {
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
            Tempname = ascii.GetString(bytes.ToArray());
            //Tempname = Tempname.Replace(@"\\", @"\");
            return Tempname;
        }

        public static byte[] StringToByteArray(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static byte[] BinaryStringToByteArray(string binary)
        {
            int numOfBytes = binary.Length / 8;
            byte[] bytes = new byte[numOfBytes];
            for (int i = 0; i < numOfBytes; ++i)
            {
                bytes[i] = Convert.ToByte(binary.Substring(8 * i, 8), 2);
            }
            return bytes;
        }

        public static string HashBytesToString(byte[] bytes)
        {
            string temps;
            string tru = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                temps = bytes[i].ToString("X");
                if (temps == "0")
                {
                    temps = "00";
                }
                else if (temps == "1")
                {
                    temps = "01";
                }
                else if (temps == "2")
                {
                    temps = "02";
                }
                else if (temps == "3")
                {
                    temps = "03";
                }
                else if (temps == "4")
                {
                    temps = "04";
                }
                else if (temps == "5")
                {
                    temps = "05";
                }
                else if (temps == "6")
                {
                    temps = "06";
                }
                else if (temps == "7")
                {
                    temps = "07";
                }
                else if (temps == "8")
                {
                    temps = "08";
                }
                else if (temps == "9")
                {
                    temps = "09";
                }
                else if (temps == "A")
                {
                    temps = "0A";
                }
                else if (temps == "B")
                {
                    temps = "0B";
                }
                else if (temps == "C")
                {
                    temps = "0C";
                }
                else if (temps == "D")
                {
                    temps = "0D";
                }
                else if (temps == "E")
                {
                    temps = "0E";
                }
                else if (temps == "F")
                {
                    temps = "0F";
                }
                tru += temps;
            }
            return tru;
        }

        public static string HashBytesToStringL(List<byte> bytes)
        {
            string temps;
            string tru = "";
            for (int i = 0; i < bytes.Count; i++)
            {
                temps = bytes[i].ToString("X");
                if (temps == "0")
                {
                    temps = "00";
                }
                else if (temps == "1")
                {
                    temps = "01";
                }
                else if (temps == "2")
                {
                    temps = "02";
                }
                else if (temps == "3")
                {
                    temps = "03";
                }
                else if (temps == "4")
                {
                    temps = "04";
                }
                else if (temps == "5")
                {
                    temps = "05";
                }
                else if (temps == "6")
                {
                    temps = "06";
                }
                else if (temps == "7")
                {
                    temps = "07";
                }
                else if (temps == "8")
                {
                    temps = "08";
                }
                else if (temps == "9")
                {
                    temps = "09";
                }
                else if (temps == "A")
                {
                    temps = "0A";
                }
                else if (temps == "B")
                {
                    temps = "0B";
                }
                else if (temps == "C")
                {
                    temps = "0C";
                }
                else if (temps == "D")
                {
                    temps = "0D";
                }
                else if (temps == "E")
                {
                    temps = "0E";
                }
                else if (temps == "F")
                {
                    temps = "0F";
                }
                tru += temps;
            }
            return tru;
        }

        public static bool IsZeroByte(byte b)
        {
            return b == 0x00;
        }

        public static string BytesToStringL2(List<byte> bytes, string s)
        {
            string temps;
            string tru = "";
            //int tempi;
            for (int i = 0; i < bytes.Count; i++)
            {
                temps = bytes[i].ToString("X");
                //Fix this for other single digit numbers!
                if (temps == "0")
                {
                    temps = "00";
                }
                else if (temps == "1")
                {
                    temps = "01";
                }
                else if (temps == "2")
                {
                    temps = "02";
                }
                else if (temps == "3")
                {
                    temps = "03";
                }
                else if (temps == "4")
                {
                    temps = "04";
                }
                else if (temps == "5")
                {
                    temps = "05";
                }
                else if (temps == "6")
                {
                    temps = "06";
                }
                else if (temps == "7")
                {
                    temps = "07";
                }
                else if (temps == "8")
                {
                    temps = "08";
                }
                else if (temps == "9")
                {
                    temps = "09";
                }
                else if (temps == "A")
                {
                    temps = "0A";
                }
                else if (temps == "B")
                {
                    temps = "0B";
                }
                else if (temps == "C")
                {
                    temps = "0C";
                }
                else if (temps == "D")
                {
                    temps = "0D";
                }
                else if (temps == "E")
                {
                    temps = "0E";
                }
                else if (temps == "F")
                {
                    temps = "0F";
                }
                tru += temps;
            }
            return tru;
        }

        public static string BytesToStringL2R(List<byte> bytes, string s)
        {
            string temps;
            string tru = "";
            bytes.Reverse();
            for (int i = 0; i < bytes.Count; i++)
            {
                temps = bytes[i].ToString("X");
                //Fix this for other single digit numbers!
                if (temps == "0")
                {
                    temps = "00";
                }
                else if (temps == "1")
                {
                    temps = "01";
                }
                else if (temps == "2")
                {
                    temps = "02";
                }
                else if (temps == "3")
                {
                    temps = "03";
                }
                else if (temps == "4")
                {
                    temps = "04";
                }
                else if (temps == "5")
                {
                    temps = "05";
                }
                else if (temps == "6")
                {
                    temps = "06";
                }
                else if (temps == "7")
                {
                    temps = "07";
                }
                else if (temps == "8")
                {
                    temps = "08";
                }
                else if (temps == "9")
                {
                    temps = "09";
                }
                else if (temps == "A")
                {
                    temps = "0A";
                }
                else if (temps == "B")
                {
                    temps = "0B";
                }
                else if (temps == "C")
                {
                    temps = "0C";
                }
                else if (temps == "D")
                {
                    temps = "0D";
                }
                else if (temps == "E")
                {
                    temps = "0E";
                }
                else if (temps == "F")
                {
                    temps = "0F";
                }
                tru += temps;
            }
            return tru;
        }

        public static string HashToMatName(string s, byte[] HashBytes)
        {
            string name = "";
            int ZestyTest = 0;
            Ionic.Zlib.CRC32 crc = new Ionic.Zlib.CRC32();

            ZestyTest = HashBytes.GetHashCode();
            string testthing = "XfB_N__E_m01_7";
            int testint = testthing.GetHashCode();
            /*
            using (MemoryStream HB = new MemoryStream(HashBytes))
            {
                ZestyTest = crc.GetCrc32(HB);

            }

            s = ZestyTest.ToString();
            */
            //crc.GetHashCode

            return name;
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static short decode(string loByte, string hiByte)
        {
            byte lo = Convert.ToByte(loByte, 16);
            byte hi = Convert.ToByte(hiByte, 16);

            short composed = (short)(lo + (hi << 7));

            short result = (short)(composed - 8192);

            return result;
        }

        //Makes Bitmap from byte array containing DDS file.
        public static Bitmap BitmapBuilderDX(byte[] ddsfile, TextureEntry textureEntry)
        {
            if (textureEntry.OutMaps != null)
            {

                if (textureEntry.OutMaps[0] == 137 && textureEntry.OutMaps[1] == 80 && textureEntry.OutMaps[2] == 78 && textureEntry.OutMaps[3] == 71 && textureEntry.OutMaps[4] == 13 && textureEntry.OutMaps[5] == 10 && textureEntry.OutMaps[6] == 26 && textureEntry.OutMaps[7] == 10)
                {
                    #region PNG
                    Bitmap bmp;
                    using (var ms = new MemoryStream(textureEntry.OutMaps))
                    {
                        bmp = new Bitmap(ms);
                        return bmp;
                    }
                    #endregion
                }
                else
                {
                    #region DDS Files
                    Stream ztrim = new MemoryStream(textureEntry.OutMaps);
                    //From the pfim website.
                    using (var image = Pfim.Pfim.FromStream(ztrim))
                    {
                        PixelFormat format;

                        // Convert from Pfim's backend agnostic image format into GDI+'s image format
                        switch (image.Format)
                        {
                            case Pfim.ImageFormat.Rgba32:
                                format = PixelFormat.Format32bppArgb;
                                break;
                            //case Pfim.ImageFormat.Rgb24:
                            // format = PixelFormat.Format24bppRgb;
                            //break;
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
                            var pmap = new Bitmap(image.Width, image.Height, image.Stride, format, data);
                            return pmap;
                        }
                        finally
                        {
                            handle.Free();
                        }
                    }
                    #endregion
                }
            }
            else
            {
                return null;
            }
        }

        public static Bitmap BitmapBuilderDXEncode(byte[] ddsfile, FrmTexEncodeDialog fted)
        {
            if (fted.DDSData != null)
            {

                if (fted.DDSData[0] == 137 && fted.DDSData[1] == 80 && fted.DDSData[2] == 78 && fted.DDSData[3] == 71 && fted.DDSData[4] == 13 && fted.DDSData[5] == 10 && fted.DDSData[6] == 26 && fted.DDSData[7] == 10)
                {
                    #region PNG
                    //Bitmap bmp;
                    //Makes a copy of the bitmap to be independent of the MemoryStream. Should Fix the random occassional crashing.
                    using (var Riskyms = new MemoryStream(fted.DDSData))
                    using (var ms = new Bitmap(Riskyms))
                    {
                        return new Bitmap(ms);
                    }
                    #endregion
                }
                else
                {
                    #region DDS Files
                    using (Stream ztrim = new MemoryStream(fted.DDSData))
                    //From the pfim website.
                    using (var image = Pfim.Pfim.FromStream(ztrim))
                    {
                        PixelFormat format;

                        // Convert from Pfim's backend agnostic image format into GDI+'s image format
                        switch (image.Format)
                        {
                            case Pfim.ImageFormat.Rgba32:
                                format = PixelFormat.Format32bppArgb;
                                break;
                            //case Pfim.ImageFormat.Rgb24:
                            // format = PixelFormat.Format24bppRgb;
                            //break;
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

                            //Safely copies the data before the handle is released.
                            //var pmap = new Bitmap(image.Width, image.Height, image.Stride, format, data);
                            //return pmap;
                            //using (var RiskyBitmap = new Bitmap(image.Width, image.Height, image.Stride, format, data))
                            //{
                            //    return RiskyBitmap;
                            //}

                            //Gonna try this a safer way, making a copy and then duplicating the pixel data and the like.
                            using (Bitmap RiskyBitmap = new Bitmap(image.Width, image.Height, image.Stride, format, data))
                            {
                                Bitmap SaferBitmap = new Bitmap(image.Width, image.Height, format);

                                BitmapData srcData = RiskyBitmap.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, format);
                                BitmapData dstData = SaferBitmap.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.WriteOnly, format);

                                try
                                {
                                    int rowBytes = Math.Abs(srcData.Stride);
                                    byte[] rowBuffer = new byte[rowBytes];

                                    //Now to duplicate the pixel data and format.
                                    for (int y = 0; y < image.Height; y++)
                                    {
                                        IntPtr src = srcData.Scan0 + y * srcData.Stride;
                                        IntPtr dst = dstData.Scan0 + y * dstData.Stride;
                                        Marshal.Copy(src, rowBuffer, 0, rowBytes);
                                        Marshal.Copy(rowBuffer, 0, dst, rowBytes);
                                    }
                                }
                                finally
                                {
                                    RiskyBitmap.UnlockBits(srcData);
                                    SaferBitmap.UnlockBits(dstData);
                                }
                                return SaferBitmap;


                            }


                        }
                        finally
                        {
                            handle.Free();
                        }
                    }
                    #endregion
                }
            }
            else
            {
                return null;
            }
        }

        //Todo: Use this properly.
        //Builds a bitmap from a raw dds file.
        public static Bitmap NewBitmapBuilder(byte[] ddsfile)
        {
            MemoryStream ms = new MemoryStream(ddsfile);
            var image = Pfim.Pfim.FromStream(ms);

            PixelFormat format;
            switch (image.Format)
            {
                case Pfim.ImageFormat.Rgba32:
                    format = PixelFormat.Format32bppArgb;
                    break;
                case Pfim.ImageFormat.Rgb24:
                    format = PixelFormat.Format24bppRgb;
                    break;
                default:
                    throw new NotSupportedException($"Unexpected Pfim format: {image.Format}");
            }

            //Pin Pfim's data array so GC won't move it while we read from it.
            var handle = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
            try
            {
                var data = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);

                //Construct a Bitmap directly over Pfim's buffer (zero-copy), then immediately clone it into a fully independent Bitmap so
                //the returned object is safe to use after this method returns and the GCHandle is released.
                var riskyBitmap = new Bitmap(image.Width, image.Height, image.Stride, format, data);

                var safeBitmap = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);

                BitmapData srcData = riskyBitmap.LockBits(
                    new Rectangle(0, 0, image.Width, image.Height),
                    ImageLockMode.ReadOnly, format);
                BitmapData dstData = safeBitmap.LockBits(
                    new Rectangle(0, 0, image.Width, image.Height),
                    ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                try
                {
                    int rowBytes = Math.Abs(srcData.Stride);
                    var rowBuffer = new byte[rowBytes];
                    for (int y = 0; y < image.Height; y++)
                    {
                        IntPtr src = srcData.Scan0 + y * srcData.Stride;
                        IntPtr dst = dstData.Scan0 + y * dstData.Stride;
                        Marshal.Copy(src, rowBuffer, 0, rowBytes);
                        Marshal.Copy(rowBuffer, 0, dst, rowBytes);
                    }
                }
                finally
                {
                    riskyBitmap.UnlockBits(srcData);
                    safeBitmap.UnlockBits(dstData);
                }

                return safeBitmap;
            }
            finally
            {
                handle.Free();
            }
        }

        public static Bitmap CubemapToCross(byte[] ddsBytes)
        {
            //Peek at the header to verify whether this is a cubemap.
            var hr = new BinaryReader(new MemoryStream(ddsBytes));
            if (hr.ReadUInt32() != 0x20534444u)
                throw new ArgumentException("Not a valid DDS file.");

            hr.ReadUInt32();                    // dwSize
            hr.ReadUInt32();                    // dwFlags
            int height = (int)hr.ReadUInt32();
            int width = (int)hr.ReadUInt32();
            hr.ReadBytes((4 + 11) * 4);         // pitch, depth, mipCount, reserved1
            hr.ReadBytes(32);                   // DDS_PIXELFORMAT
            hr.ReadUInt32();                    // dwCaps
            uint dwCaps2 = hr.ReadUInt32();

            bool isCubeMap = (dwCaps2 & DDSConstants.DDSCAPS2_CUBEMAP) != 0;

            // DDS/TEX face storage order: +X=0, -X=1, +Y=2, -Y=3, +Z=4, -Z=5.
            // Horizontal cross grid positions (col, row) for each face index:
            //   +X → (2,1)   -X → (0,1)   +Y → (1,0)
            //   -Y → (1,2)   +Z → (1,1)   -Z → (3,1)
            var gridPositions = new (int col, int row)[6]
            {
                (2, 1),  // 0: +X
                (0, 1),  // 1: -X
                (1, 0),  // 2: +Y
                (1, 2),  // 3: -Y
                (1, 1),  // 4: +Z
                (3, 1),  // 5: -Z
            };

            var Cross = new Bitmap(width * 4, height * 3, PixelFormat.Format32bppArgb);
            var g = Graphics.FromImage(Cross);
            g.Clear(Color.Transparent);

            for (int face = 0; face < 6; face++)
            {
                var faceBitmap = NewBitmapBuilderForCubeMaps(ddsBytes, face);
                var (col, row) = gridPositions[face];
                g.DrawImage(faceBitmap, col * width, row * height, width, height);
            }

            return Cross;
        }

        //A function meant to build a single surface DDS from a multi surface one. For use with the below function and Cube Maps only.
        private static byte[] BuildSingleSurfaceDds(byte[] src, int pixelOffset, int pixelSize, int width, int height, uint pfFlags, uint pfFourCC, uint pfBitCnt, int blockSize, bool isCompressed)
        {
            var ms = new MemoryStream();
            var w = new BinaryWriter(ms);

            //Magic.
            w.Write(0x20534444u);

            //The rest of the DDS header.
            w.Write(124u);                                              
            uint flags = DDSConstants.DDSD_CAPS  | DDSConstants.DDSD_HEIGHT
                       | DDSConstants.DDSD_WIDTH | DDSConstants.DDSD_PIXELFORMAT
                       | DDSConstants.DDSD_LINEARSIZE;
            w.Write(flags);
            w.Write((uint)height);
            w.Write((uint)width);
            uint pitch = isCompressed
                ? (uint)DDSCalc.PitchBlockCompressed(width, blockSize)
                : (uint)DDSCalc.PitchBpp(width, (int)pfBitCnt);
            w.Write(pitch);
            w.Write(0u);    // dwDepth
            w.Write(1u);    // dwMipMapCount
            for (int i = 0; i < 11; i++) w.Write(0u); // dwReserved1

            //DDS_PIXELFORMAT, based on TGE's original code.
            w.Write(32u);
            w.Write(pfFlags);
            w.Write(pfFourCC);
            w.Write(pfBitCnt);
            for (int i = 0; i < 4; i++) w.Write(0u);

            w.Write(DDSConstants.DDSCAPS_TEXTURE); 
            w.Write(0u);
            w.Write(0u);
            w.Write(0u);
            w.Write(0u);

            //Pixel data exclusively for the top-level mip of this face.
            w.Write(src, pixelOffset, pixelSize);

            return ms.ToArray();
        }

        public static Bitmap NewBitmapBuilderForCubeMaps(byte[] ddsfile, int FaceIndex)
        {

            if(FaceIndex < 0 || FaceIndex > 6)
            {
                FaceIndex = 0;
            }

            // Parse just enough of the header to know dimensions, mip count,
            // compression format, and whether this is a cubemap.
            BinaryReader hr = new BinaryReader(new MemoryStream(ddsfile));

            uint magic = hr.ReadUInt32(); // "DDS "
            if (magic != 0x20534444u)
                throw new ArgumentException("This isn't a valid DDS file.");

            hr.ReadUInt32();
            hr.ReadUInt32();
            int height = (int)hr.ReadUInt32();
            int width = (int)hr.ReadUInt32();
            hr.ReadUInt32();                    
            hr.ReadUInt32();
            int mipCount = (int)hr.ReadUInt32();
            if (mipCount == 0) mipCount = 1;
            hr.ReadBytes(11 * 4);

            //DDS_PIXELFORMAT
            hr.ReadUInt32();                    
            uint pfFlags = hr.ReadUInt32();
            uint pfFourCC = hr.ReadUInt32();
            uint pfBitCnt = hr.ReadUInt32();
            hr.ReadBytes(4 * 4);                

            hr.ReadUInt32();
            //This has Cubemap flags.
            uint dwCaps2 = hr.ReadUInt32();     

            bool isCubeMap = (dwCaps2 & DDSConstants.DDSCAPS2_CUBEMAP) != 0;
            bool isCompressed = (pfFlags & DDSConstants.DDPF_FOURCC) != 0;
            int blockSize = (pfFourCC == DDSConstants.FOURCC_DXT1) ? 8 : 16;

            if (!isCubeMap && FaceIndex != 0)
                throw new ArgumentException("FaceIndex must be 0 for non-cubemap textures.");
            if (isCubeMap && (FaceIndex < 0 || FaceIndex > 5))
                throw new ArgumentOutOfRangeException(nameof(FaceIndex), "Cubemap FaceIndex cannot be greater than 5.");


            // ── Cubemap: slice out the requested face ─────────────────────────
            // All six faces are stored consecutively after the 128-byte header,
            // each consisting of a full mip chain.  Walk the mip chain sizes to
            // find where each face starts.

            // Compute the byte size of one complete mip chain (all levels).
            int faceChainSize = 0;
            int w = width, h = height;
            for (int m = 0; m < mipCount; m++)
            {
                faceChainSize += isCompressed ? DDSCalc.LinearSizeBlockCompressed(w, h, blockSize) : DDSCalc.LinearSizeBpp(w, h, (int)pfBitCnt);
                w = Math.Max(1, w / 2);
                h = Math.Max(1, h / 2);
            }

            //Offset into ddsBytes where the requested face's mip chain begins.
            //128 is the DDS Header size.
            int faceDataOffset = 128 + FaceIndex * faceChainSize;

            // Size of just the top-level mip of the requested face.
            int topMipSize = isCompressed ? DDSCalc.LinearSizeBlockCompressed(width, height, blockSize) : DDSCalc.LinearSizeBpp(width, height, (int)pfBitCnt);

            // Build a minimal single-surface DDS containing only that face's
            // top-level mip so Pfim has exactly one surface to decode.
            byte[] faceDds = BuildSingleSurfaceDds(ddsfile, faceDataOffset, topMipSize, width, height, pfFlags, pfFourCC, pfBitCnt, blockSize, isCompressed);

            return NewBitmapBuilder(faceDds);
        }

        //Also based on a few functions from the model importer.
        public static TextureEntry FromDDSToTex(TextureEntry texentry, BinaryReader bnr, FrmTexEncodeDialog FTED)
        {
            //First we read the DDS file's header.
            uint Magic = bnr.ReadUInt32();
            uint Size = bnr.ReadUInt32();
            uint Flags = bnr.ReadUInt32();
            uint Height = bnr.ReadUInt32();
            uint Width = bnr.ReadUInt32();
            uint Pitch = bnr.ReadUInt32();
            uint Depth = bnr.ReadUInt32();
            uint MipCnt = bnr.ReadUInt32();

            //Jumps ahead to skip the reserved bytes.
            bnr.BaseStream.Position = bnr.BaseStream.Position + (0x2C);

            uint DDS_Size = bnr.ReadUInt32();
            uint DDS_Flags = bnr.ReadUInt32();
            uint DDS_FourCC = bnr.ReadUInt32();
            uint DDS_BitCounnt = bnr.ReadUInt32();
            uint DDS_RMask = bnr.ReadUInt32();
            uint DDS_GMask = bnr.ReadUInt32();
            uint DDS_BMask = bnr.ReadUInt32();
            uint DDS_AMask = bnr.ReadUInt32();

            uint dwCaps = bnr.ReadUInt32();
            uint dwCaps2 = bnr.ReadUInt32();
            uint dwCaps3 = bnr.ReadUInt32();

            //More reserved bytes to skip.
            bnr.BaseStream.Position = bnr.BaseStream.Position + (0x8);

            //Now that the header's been read, we need to verify the surface count, type, MipMapCount, and whether or not this is a cube map.

            bool isThisACubeMap = (dwCaps2 & DDSConstants.DDSCAPS2_CUBEMAP) != 0;
            int surfCount = isThisACubeMap ? 6 : 1;
            int mipCount = (int)Math.Max(1, MipCnt);
            bool isCompressed = (DDS_Flags & DDSConstants.DDPF_FOURCC) != 0;
            bool isUsingAlpha = DDS_FourCC == 844388420 || DDS_FourCC == 861165636 || DDS_FourCC == 877942852 || DDS_FourCC == 894720068;

            int texFmt;
            if (FTED.ShortName != null)
            {
                texFmt = RTextureSurfaceFormat.GetFormatFromTextureName(FTED.ShortName, isUsingAlpha)
                         ?? RTextureSurfaceFormat.BM_OPA;

                //Because we can't or don't know how to encode in LAB Color, I gotta disable that option for now 
                //and make it a regular DXT5 texture type instead.
                if (FTED.cmBoxTextureType.SelectedIndex == 1)
                {
                    texFmt = 23;
                }
            }
            else if (!isCompressed)
            {
                texFmt = RTextureSurfaceFormat.LIN;
            }
            else
            {
                texFmt = (DDS_FourCC == DDSConstants.FOURCC_DXT1)
                    ? RTextureSurfaceFormat.BM_OPA
                    : RTextureSurfaceFormat.BM_XLU;
            }

            
            int NewBlockSize = RTextureSurfaceFormat.GetBlockSize(texFmt);

            //Read the pixel data to start building the raw file.
            var surfaces = new List<List<byte[]>>();
            for (int s = 0; s < surfCount; s++)
            {
                var mips = new List<byte[]>();
                int w = (int)Width, h = (int)Height;
                for (int m = 0; m < mipCount; m++)
                {
                    int size = (NewBlockSize > 0)
                        ? DDSCalc.LinearSizeBlockCompressed(w, h, NewBlockSize)
                        : DDSCalc.LinearSizeBpp(w, h, (int)DDS_BitCounnt);
                    mips.Add(bnr.ReadBytes(size));
                    w = Math.Max(1, w / 2);
                    h = Math.Max(1, h / 2);
                }
                surfaces.Add(mips);
            }

            // Assemble TEX.
            var tex = new RTextureData { Surfaces = surfaces };
            tex.Header.MipCount = mipCount;
            tex.Header.Width = (int)Width;
            tex.Header.Height = (int)Height;
            tex.Header.SurfaceFmt = texFmt;
            tex.Header.Field3 = 1;
            tex.Header.Field4 = 0;
            tex.Header.SurfaceCount = surfaces.Count;
            tex.Header.TypeVal = RTextureHeader.DefaultTypeVal;

            if (isThisACubeMap)
            {
                tex.Header.Dimensions = 6;
                tex.SetDefaultCubeMapFaces();
            }
            else
            {
                tex.Header.Dimensions = 2;
            }

            texentry.UncompressedData = tex.WriteToBytes();
            texentry.CompressedData = Zlibber.Compressor(texentry.UncompressedData);
//#if DEBUG
//            File.WriteAllBytes("D:\\Workshop\\LMTHub\\Test\\TexTEST2026" + ".tex", texentry.UncompressedData);
//#endif

            //Now we start building the TexEntry from scratch with what we have above.
            texentry.Magic = "54455800";
            texentry.X = (int)Width;
            texentry.Y = (int)Height;
            texentry.XSize = (int)Width;
            texentry.YSize = (int)Height;
            texentry.MipMapCount = (int)MipCnt;
            texentry.SurfaceCount = surfCount;
            texentry.Dimensions = texentry.SurfaceCount;
            texentry.Type = texFmt;
            texentry.TextureType = texentry.Type.ToString();

            #region Cube Maps

            if (texentry.Dimensions == 6)
            {
                texentry.TexType = "Cube Map(DXT1)";
                texentry.Format = "Cube Map(DXT1)";
                texentry.IsCubeMap = true;

                //using (BinaryReader texbnr = new BinaryReader())
                //{
                //    //Faces.
                //    for (int y = 0; y < 3; y++)
                //    {
                //        RTextureFace rFace = new RTextureFace();
                //        rFace = RTextureFace.ReadFaceFromTEX(texbnr);
                //        texentry.Faces.Add(rFace);
                //    }
                //}
            }

            #endregion
            else
            {
                switch (texentry.Type)
                {
                    case 0x13:
                        texentry.Format = "DXT1/BC1";
                        texentry.TexType = "DXT1/BC1";
                        break;

                    case 0x15:
                        texentry.Format = "Alternate DXT5/BC3";
                        texentry.TexType = "Alternate DXT5/BC3";
                        break;

                    case 0x17:
                        texentry.Format = "DXT5/BC3";
                        texentry.TexType = "DXT5/BC3";
                        break;

                    case 0x19:
                        texentry.Format = "BC4_UNORM/Metalic/Specular Map";
                        texentry.TexType = "BC4_UNORM/Metalic/Specular Map";
                        break;

                    case 0x1E:
                        texentry.Format = "Cloth (Undocumented)";
                        texentry.TexType = "Cloth (Undocumented)";
                        break;

                    case 0x1F:
                        texentry.Format = "BC5/Normal Map";
                        texentry.TexType = "BC5/Normal Map";
                        break;

                    case 0x27:
                        texentry.Format = "RGBA Linear Texture";
                        texentry.TexType = "RGBA Linear Texture";
                        break;

                    case 0x2A:
                        texentry.Format = "LAB Color/Problematic UI Texture";
                        texentry.TexType = "LAB Color/Problematic UI Texture";
                        break;

                    default:
                        break;
                }

            }

            return texentry;
        }

        //From AGuyCalledGerald and Mike Two.
        public static int GetNthIndex(string s, char t, int n)
        {
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == t)
                {
                    count++;
                    if (count == n)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public static void ReverseBitArray(BitArray array)
        {
            int length = array.Length;
            int mid = (length / 2);

            for (int i = 0; i < mid; i++)
            {
                bool bit = array[i];
                array[i] = array[length - i - 1];
                array[length - i - 1] = bit;
            }
        }

        public class HashComputation
        {
            public static uint ComputeHash(string input)
            {
                // Computes CRC32/JAMCRC hash
                uint crc = (uint)Crc32Algorithm.Compute(System.Text.Encoding.ASCII.GetBytes(input));
                return ~crc;
            }
        }

        public static class Crc32Algorithm
        {
            private static readonly uint[] Table;

            static Crc32Algorithm()
            {
                const uint polynomial = 0xedb88320;
                Table = new uint[256];
                for (uint i = 0; i < 256; i++)
                {
                    uint crc = i;
                    for (uint j = 8; j > 0; j--)
                    {
                        if ((crc & 1) == 1)
                        {
                            crc = (crc >> 1) ^ polynomial;
                        }
                        else
                        {
                            crc >>= 1;
                        }
                    }
                    Table[i] = crc;
                }
            }

            public static uint Compute(byte[] bytes)
            {
                uint crc = 0xffffffff;
                foreach (byte b in bytes)
                {
                    byte tableIndex = (byte)((crc & 0xff) ^ b);
                    crc = (crc >> 8) ^ Table[tableIndex];
                }
                return crc ^ 0xffffffff;
            }
        }

        //Gets all offsets of instances of term represented by specified byte array. 
        public static List<int> GetAllCharPatterns(byte[] buffer, byte[] CharSequence)
        {
            List<int> Offsets = new List<int>();

            for (int i = 0; i <= buffer.Length - CharSequence.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < CharSequence.Length; j++)
                {
                    if (buffer[i + j] != CharSequence[j])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    Offsets.Add(i);
                }
            }


            return Offsets;

        }

        private static uint U32(int v)
        {
            return (uint)(v & 0xFFFFFFFF);
        }

        public static uint ComputeHash(string input)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(input);
            uint crc = Crc32Algorithm.Compute(bytes);  // returns uint
            return ~crc;  // bitwise NOT for JAMCRC
        }

        static uint ComputeJamcrc(string input)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(input);
            uint crc = Crc32Algorithm.Compute(bytes);  // Matches zlib.crc32
            return ~crc;
        }

        //Based on functions from the model importer plugin by TGE.
        public static float DecodeVertexComponent(int compType, BinaryReader reader)
        {
            switch (compType)
            {
                case 1:
                    return Convert.ToSingle(reader.ReadSingle());
                case 2:
                    //This is allegedly manual half-float conversion...
                    ushort bits = reader.ReadUInt16();
                    int sign = (bits >> 15) & 0x1;
                    int exponent = (bits >> 10) & 0x1F;
                    int mantissa = bits & 0x3FF;

                    float value;
                    if (exponent == 0)
                        value = (float)(Math.Pow(2, -14) * (mantissa / 1024.0f));  // subnormal
                    else if (exponent == 0x1F)
                        value = mantissa != 0 ? float.NaN : float.PositiveInfinity; // NaN / Inf
                    else
                        value = (float)(Math.Pow(2, exponent - 15) * (1 + mantissa / 1024.0f));

                    return sign == 1 ? -value : value;
                case 3:
                    return Convert.ToSingle(reader.ReadUInt16());
                case 4:
                    return Convert.ToSingle(reader.ReadInt16());
                case 5:
                    return Convert.ToSingle((reader.ReadUInt16()) / 32767.0f);
                case 6:
                    return Convert.ToSingle((reader.ReadUInt16()) / 65535.0f);
                case 7:
                    return Convert.ToSingle(reader.ReadByte());
                case 8:
                    return Convert.ToSingle((reader.ReadByte()));
                case 9:
                    return Convert.ToSingle(((reader.ReadByte()) - 127.0f) / 127.0f);
                case 10:
                    return Convert.ToSingle((reader.ReadByte()) / 255.0f);
                case 13:
                    return Convert.ToSingle(reader.ReadByte());
                default:
                    return -1;
            }
        }

        public static float[] DecodeVertex11And14Component(int compType, BinaryReader reader)
        {
            switch (compType)
            {
                case 11:
                    uint val = reader.ReadUInt32();
                    return new[]
                    {
                        ((byte)((val & 0x000000FFu) >>  0) - 127.0f) / 127.0f,
                        ((byte)((val & 0x0000FF00u) >>  8) - 127.0f) / 127.0f,
                        ((byte)((val & 0x00FF0000u) >> 16) - 127.0f) / 127.0f,
                        ((byte)((val & 0xFF000000u) >> 24) - 127.0f) / 127.0f,
                    };
                case 14:
                    uint valtwo = reader.ReadUInt32();
                    return new[]
                    {
                        (float)((valtwo & 0x000000FFu) >>  0),
                        (float)((valtwo & 0x0000FF00u) >>  8),
                        (float)((valtwo & 0x00FF0000u) >> 16),
                        (float)((valtwo & 0xFF000000u) >> 24),
                    };
                default:
                    return null;
                    break;
            }
        }

        //Unpacker stuff from the plugin, ported.
        public static float DecodeFS8(byte val)
        {
            return (val - 127f) / 127f;
        }

        public static float[] DecodeX8Y8Z8W8(uint packed) => new[]
        {
            DecodeFS8((byte)( packed        & 0xFF)),
            DecodeFS8((byte)((packed >>  8) & 0xFF)),
            DecodeFS8((byte)((packed >> 16) & 0xFF)),
            DecodeFS8((byte)((packed >> 24) & 0xFF)),
        };


        public class CRC32Helper
        {
            private static readonly uint[] Table = GenerateTable();

            public static int ComputeHash(string input)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(input);
                uint crc = 0xFFFFFFFF;

                foreach (byte b in bytes)
                {
                    byte index = (byte)((crc ^ b) & 0xFF);
                    crc = (crc >> 8) ^ Table[index];
                }

                crc = ~crc;

                // Convert to signed int (wraps if > int.MaxValue)
                return unchecked((int)crc);
            }

            private static uint[] GenerateTable()
            {
                uint[] table = new uint[256];
                const uint polynomial = 0xEDB88320;

                for (uint i = 0; i < table.Length; i++)
                {
                    uint c = i;
                    for (int j = 0; j < 8; j++)
                    {
                        if ((c & 1) != 0)
                            c = polynomial ^ (c >> 1);
                        else
                            c >>= 1;
                    }
                    table[i] = c;
                }

                return table;
            }
        }

    }
}
