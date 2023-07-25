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

namespace ThreeWorkTool.Resources.Utility
{
    //This class is to store all the functions relating to bytes and conversions I used frequently in several classes and functions.
    public class ByteUtilitarian
    {
        public static StringBuilder SBname;

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

        public static string HashToMatName(string s, byte[] HashBytes )
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
                    Bitmap bmp;
                    using (var ms = new MemoryStream(fted.DDSData))
                    {
                        bmp = new Bitmap(ms);
                        return bmp;
                    }
                    #endregion
                }
                else
                {
                    #region DDS Files
                    Stream ztrim = new MemoryStream(fted.DDSData);
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

    }
}
