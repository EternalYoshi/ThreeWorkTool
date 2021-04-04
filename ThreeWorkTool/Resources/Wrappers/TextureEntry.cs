using DDSReader;
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
using System.Text;
using System.Windows.Forms;
using Pfim;
using System.Runtime.InteropServices;
using SixLabors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class TextureEntry
    {
        public string Magic;
        public int XSize;
        public int YSize;
        public int ZSize;
        public int CSize;
        public int DSize;
        public int version;
        public int PixelCount;
        public string TexType;
        public bool HasTransparency;
        public bool HasMips;
        public int Mips;
        public int EntryID;
        public string TrueName;
        public byte[] WTemp;
        public byte[] CompressedData;
        public byte[] UncompressedData;
        public byte[] OutMaps;
        public byte[][] OutMapsB;
        public byte[] OutMapsC;
        public int[] MipOffsets;
        public List<byte> OutTexTest;
        public static StringBuilder SBname;
        public string[] EntryDirs;
        public int OffsetTemp;
        public string EntryName;
        public int AOffset;
        public string FileExt;
        public static string TypeHash = "241F5DEB";
        public int SizeShift;

        public static TextureEntry FillTexEntry(string filename, List<string> subnames, TreeView tree, byte[] Bytes, int c, int ID, Type filetype = null)
        {
            TextureEntry texentry = new TextureEntry();

            using (FileStream fs = File.OpenRead(filename))
            {
                //This block gets the name of the entry.

                texentry.OffsetTemp = c;
                texentry.EntryID = ID;
                byte[] BTemp;
                BTemp = new byte[] { };
                BTemp = Bytes.Skip(texentry.OffsetTemp).Take(64).Where(x => x != 0x00).ToArray();

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
                Tempname = ascii.GetString(BTemp);

                //Compressed Data size.
                BTemp = new byte[] { };
                c = c + 68;
                BTemp = Bytes.Skip(c).Take(4).ToArray();
                BTemp.Reverse();
                texentry.CSize = BitConverter.ToInt32(BTemp, 0);

                //Uncompressed Data size.
                BTemp = new byte[] { };
                c = c + 4;
                BTemp = Bytes.Skip(c).Take(4).ToArray();
                Array.Reverse(BTemp);
                string TempStr = "";
                TempStr = BytesToString(BTemp, TempStr);
                BigInteger BN1, BN2, DIFF;
                BN2 = BigInteger.Parse("40000000", NumberStyles.HexNumber);
                BN1 = BigInteger.Parse(TempStr, NumberStyles.HexNumber);
                DIFF = BN1 - BN2;
                texentry.DSize = (int)DIFF;

                //Data Offset.
                BTemp = new byte[] { };
                c = c + 4;
                BTemp = Bytes.Skip(c).Take(4).ToArray();
                BTemp.Reverse();
                texentry.AOffset = BitConverter.ToInt32(BTemp, 0);

                //Compressed Data.
                BTemp = new byte[] { };
                c = texentry.AOffset;
                BTemp = Bytes.Skip(c).Take(texentry.CSize).ToArray();
                texentry.CompressedData = BTemp;

                //Gets the SizeShift.... whatever that is.
                texentry.SizeShift = texentry.CompressedData[7];

                //Namestuff.
                texentry.EntryName = Tempname;

                //Ensures existing subdirectories are cleared so the directories for files are displayed correctly.
                if (subnames != null)
                {
                    if (subnames.Count > 0)
                    {
                        subnames.Clear();
                    }
                }

                //Gets the filename without subdirectories.
                if (texentry.EntryName.Contains("\\"))
                {
                    string[] splstr = texentry.EntryName.Split('\\');

                    //foreach (string v in splstr)
                    for (int v = 0; v < (splstr.Length - 1); v++)
                    {
                        if (!subnames.Contains(splstr[v]))
                        {
                            subnames.Add(splstr[v]);
                        }
                    }


                    texentry.TrueName = texentry.EntryName.Substring(texentry.EntryName.IndexOf("\\") + 1);
                    Array.Clear(splstr, 0, splstr.Length);

                    while (texentry.TrueName.Contains("\\"))
                    {
                        texentry.TrueName = texentry.TrueName.Substring(texentry.TrueName.IndexOf("\\") + 1);
                    }
                }
                else
                {
                    texentry.TrueName = texentry.EntryName;
                }

                texentry._FileName = texentry.TrueName;

                texentry.EntryDirs = subnames.ToArray();
                texentry.FileExt = ".tex";
                texentry.EntryName = texentry.EntryName + texentry.FileExt;

                //Decompression Time.
                texentry.UncompressedData = ZlibStream.UncompressBuffer(texentry.CompressedData);

            }

            //Actual Tex Loading work here.
            byte[] VTemp = new byte[4];
            uint[] LWData = new uint[3];
            Array.Copy(texentry.UncompressedData, 8, VTemp, 0, 4);
            byte[] DTemp = new byte[4];

            //Gets the Texture type.
            texentry.TexType = texentry.UncompressedData[13].ToString("X2");
            texentry._TextureType = texentry.TexType;


            switch (texentry.TexType)
            {
                #region Bitmap Textures
                case "13":

                    texentry._Format = "DXT1/BC1";

                    //Gets the unsigned integers which hold data on the texture's dimensions.
                    Array.Copy(texentry.UncompressedData, 4, DTemp, 0, 4);
                    LWData[0] = BitConverter.ToUInt32(DTemp, 0);

                    Array.Copy(texentry.UncompressedData, 8, DTemp, 0, 4);
                    LWData[1] = BitConverter.ToUInt32(DTemp, 0);

                    Array.Copy(texentry.UncompressedData, 12, DTemp, 0, 4);
                    LWData[2] = BitConverter.ToUInt32(DTemp, 0);

                    byte[] bytemp = {0x09, 0x80, 0x10, 0x01};
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

                    texentry.MipOffsets = new int[texentry.MipMapCount];

                    for (int i = 0; i < texentry._MipMapCount; i++)
                    {
                        Array.Copy(texentry.UncompressedData, v, DTemp, 0, 4);

                        //Gets offsets of MipMapData.
                        texentry.MipOffsets[i] = BitConverter.ToInt32(DTemp, 0);

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

                    if(texentry.FileName.Contains("NOMIP"))
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

                    try
                    {

                        using (BinaryWriter bw = new BinaryWriter(File.Open(outname, FileMode.Create)))
                        {
                            bw.Write(texentry.OutMaps);
                            bw.Close();
                        }

                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("Unable to access the file. Maybe it's already in use by another proccess?", "Cannot write this file.");
                        break;
                    }

                    Stream stream = new MemoryStream(texentry.OutMaps);

                    //texentry.tex = BitmapBuilder(outpngname,stream);
                    stream.Close();
                    break;

                #endregion

                #region Bitmap Textures with Transparency
                case "17":
                    texentry._Format = "DXT5/BC3";

                    //Gets the unsigned integers which hold data on the texture's dimensions.
                    Array.Copy(texentry.UncompressedData, 4, DTemp, 0, 4);
                    LWData[0] = BitConverter.ToUInt32(DTemp, 0);

                    Array.Copy(texentry.UncompressedData, 8, DTemp, 0, 4);
                    LWData[1] = BitConverter.ToUInt32(DTemp, 0);

                    Array.Copy(texentry.UncompressedData, 12, DTemp, 0, 4);
                    LWData[2] = BitConverter.ToUInt32(DTemp, 0);

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
                        Array.Copy(texentry.UncompressedData, v17, DTemp, 0, 4);

                        //Gets offsets of MipMapData.
                        texentry.MipOffsets[i] = BitConverter.ToInt32(DTemp, 0);

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
                            texentry.WTemp = new byte[(texentry.UncompressedData.Length - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)];

                            System.Buffer.BlockCopy(texentry.UncompressedData, texentry.MipOffsets[i], texentry.WTemp, 0, (texentry.UncompressedData.Length - texentry.MipOffsets[i]));
                            w17 = texentry.WTemp.Length;
                            u17 = u17 + texentry.WTemp.Length;

                            texentry.OutMaps = texentry.WTemp;
                            texentry.OutMapsB[i] = texentry.OutMaps;
                        }
                        else
                        {
                            texentry.WTemp = new byte[(texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)];
                            System.Buffer.BlockCopy(texentry.UncompressedData, texentry.MipOffsets[i], texentry.WTemp, 0, (texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i]));
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

                    try
                    {

                        using (BinaryWriter bw = new BinaryWriter(File.Open(outname17, FileMode.Create)))
                        {
                            bw.Write(texentry.OutMaps);
                            bw.Close();
                        }

                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("Unable to access the file. Maybe it's already in use by another proccess?", "Cannot write this file.");
                        break;
                    }

                    Stream stream17 = new MemoryStream(texentry.OutMaps);

                    //texentry.tex = BitmapBuilder(outpngname17, stream17);
                    stream17.Close();
                    break;
                #endregion

                #region Specular Tetures
                case "19":

                    texentry._Format = "BC4_UNORM/Metalic/Specular Map";

                    //Gets the unsigned integers which hold data on the texture's dimensions.
                    Array.Copy(texentry.UncompressedData, 4, DTemp, 0, 4);
                    LWData[0] = BitConverter.ToUInt32(DTemp, 0);

                    Array.Copy(texentry.UncompressedData, 8, DTemp, 0, 4);
                    LWData[1] = BitConverter.ToUInt32(DTemp, 0);

                    Array.Copy(texentry.UncompressedData, 12, DTemp, 0, 4);
                    LWData[2] = BitConverter.ToUInt32(DTemp, 0);

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
                        Array.Copy(texentry.UncompressedData, v19, DTemp, 0, 4);

                        //Gets offsets of MipMapData.
                        texentry.MipOffsets[i] = BitConverter.ToInt32(DTemp, 0);

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

                    try
                    {

                        using (BinaryWriter bw = new BinaryWriter(File.Open(outname19, FileMode.Create)))
                        {
                            bw.Write(texentry.OutMaps);
                            bw.Close();
                        }

                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("Unable to access the file. Maybe it's already in use by another proccess?", "Cannot write this file.");
                        break;
                    }

                    Stream stream19 = new MemoryStream(texentry.OutMaps);

                    //texentry.tex = BitmapBuilder(outpngname19, stream19);

                    stream19.Close();

                    break;

                #endregion

                #region Normal Maps(Incomplete)
                case "1F":

                    texentry._Format = "BC5/Normal Map";

                    //Gets the unsigned integers which hold data on the texture's dimensions.
                    Array.Copy(texentry.UncompressedData, 4, DTemp, 0, 4);
                    LWData[0] = BitConverter.ToUInt32(DTemp, 0);

                    Array.Copy(texentry.UncompressedData, 8, DTemp, 0, 4);
                    LWData[1] = BitConverter.ToUInt32(DTemp, 0);

                    Array.Copy(texentry.UncompressedData, 12, DTemp, 0, 4);
                    LWData[2] = BitConverter.ToUInt32(DTemp, 0);

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
                        Array.Copy(texentry.UncompressedData, v1f, DTemp, 0, 4);

                        //Gets offsets of MipMapData.
                        texentry.MipOffsets[i] = BitConverter.ToInt32(DTemp, 0);

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
                            texentry.WTemp = new byte[(texentry.UncompressedData.Length - texentry.MipOffsets[i])];
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
                            texentry.OutMaps = new byte[(texentry._MipMapCount)];
                            System.Buffer.BlockCopy(texentry.UncompressedData, texentry.MipOffsets[i], texentry.WTemp, 0, (texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i]));
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

                    try
                    {

                        using (BinaryWriter bw = new BinaryWriter(File.Open(outname1f, FileMode.Create)))
                        {
                            bw.Write(texentry.OutMaps);
                            bw.Close();
                        }

                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("Unable to access the file. Maybe it's already in use by another proccess?", "Cannot write this file.");
                        break;
                    }

                    Stream stream1f = new MemoryStream(texentry.OutMaps);

                    //texentry.tex = BitmapBuilder(outpngname1f, stream1f);

                    stream1f.Close();

                    break;

                #endregion

                #region Weird Toon Shader Textures
                case "27":
                    texentry._Format = "????/Toon Shader Picture";

                    //Gets the unsigned integers which hold data on the texture's dimensions.
                    Array.Copy(texentry.UncompressedData, 4, DTemp, 0, 4);
                    LWData[0] = BitConverter.ToUInt32(DTemp, 0);

                    Array.Copy(texentry.UncompressedData, 8, DTemp, 0, 4);
                    LWData[1] = BitConverter.ToUInt32(DTemp, 0);

                    Array.Copy(texentry.UncompressedData, 12, DTemp, 0, 4);
                    LWData[2] = BitConverter.ToUInt32(DTemp, 0);

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
                        Array.Copy(texentry.UncompressedData, v27, DTemp, 0, 4);

                        //Gets offsets of MipMapData.
                        texentry.MipOffsets[i] = BitConverter.ToInt32(DTemp, 0);

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
                            texentry.WTemp = new byte[(texentry.UncompressedData.Length - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)];

                            System.Buffer.BlockCopy(texentry.UncompressedData, texentry.MipOffsets[i], texentry.WTemp, 0, (texentry.UncompressedData.Length - texentry.MipOffsets[i]));
                            w27 = texentry.WTemp.Length;
                            u27 = u27 + texentry.WTemp.Length;

                            texentry.OutMaps = texentry.WTemp;
                            texentry.OutMapsB[i] = texentry.OutMaps;
                        }
                        else
                        {
                            texentry.WTemp = new byte[(texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)];
                            System.Buffer.BlockCopy(texentry.UncompressedData, texentry.MipOffsets[i], texentry.WTemp, 0, (texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i]));
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
                            /*
                            if (i % 4 == 0 && vswap == 0)
                            {
                                barray[i - 2] = SSwapA;
                                barray[i - 4] = SSwapG;
                                vswap = 3;
                                SSwapA = barray[i];
                            }
                            else if (i % 4 == 0)
                            {
                                SSwapA = barray[i];
                                vswap--;
                            }
                            else if (i % 4 == 1)
                            {
                                SSwapR = barray[i];
                                vswap--;
                            }
                            else if (i % 4 == 2)
                            {
                                SSwapG = barray[i];
                                vswap--;
                            }
                            else if (i % 4 == 3)
                            {
                                SSwapB = barray[i];
                                vswap--;
                            }
                            /*
                            if (i % 16 == 0 && vswap == 0)
                            {

                                Array.Copy(SSwapC, 0, barray, (i - 4), 4);
                                Array.Copy(SSwapD, 0, barray, (i - 8), 4);


                                Array.Clear(SSwapA, 0, SSwapA.Length);
                                Array.Clear(SSwapB, 0, SSwapA.Length);
                                Array.Clear(SSwapC, 0, SSwapA.Length);
                                Array.Clear(SSwapD, 0, SSwapA.Length);

                                vswap = 15;
                            }
                            else if(i % 16 < 4)
                            {
                                SSwapA[i % 4] = barray[i];
                                vswap--;
                            }
                            else if (i % 16 < 8)
                            {
                                SSwapB[i % 4] = barray[i];
                                vswap--;
                            }
                            else if (i % 16 < 12)
                            {
                                SSwapC[i % 4] = barray[i];
                                vswap--;
                            }
                            else if (i % 16 < 16)
                            {
                                SSwapD[i % 4] = barray[i];
                                vswap--;
                            }
                            */
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

                    try
                    {

                        using (BinaryWriter bw = new BinaryWriter(File.Open(outname27, FileMode.Create)))
                        {
                            bw.Write(texentry.OutMaps);
                            bw.Close();
                        }

                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("Unable to access the file. Maybe it's already in use by another proccess?", "Cannot write this file.");
                        break;
                    }

                    Stream stream27 = new MemoryStream(texentry.OutMaps);

                    //texentry.tex = BitmapBuilder(outpngname27, stream27);
                    stream27.Close();
                    break;

                #endregion

                #region Weirdo Problematic Portrait Textures
                case "2A":

                    texentry._Format = "????/Problematic Portrait Picture";

                    //Gets the unsigned integers which hold data on the texture's dimensions.
                    Array.Copy(texentry.UncompressedData, 4, DTemp, 0, 4);
                    LWData[0] = BitConverter.ToUInt32(DTemp, 0);

                    Array.Copy(texentry.UncompressedData, 8, DTemp, 0, 4);
                    LWData[1] = BitConverter.ToUInt32(DTemp, 0);

                    Array.Copy(texentry.UncompressedData, 12, DTemp, 0, 4);
                    LWData[2] = BitConverter.ToUInt32(DTemp, 0);

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
                        Array.Copy(texentry.UncompressedData, v2a, DTemp, 0, 4);

                        //Gets offsets of MipMapData.
                        texentry.MipOffsets[i] = BitConverter.ToInt32(DTemp, 0);

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
                            texentry.WTemp = new byte[(texentry.UncompressedData.Length - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)];

                            System.Buffer.BlockCopy(texentry.UncompressedData, texentry.MipOffsets[i], texentry.WTemp, 0, (texentry.UncompressedData.Length - texentry.MipOffsets[i]));
                            w2a = texentry.WTemp.Length;
                            u2a = u2a + texentry.WTemp.Length;

                            texentry.OutMaps = texentry.WTemp;
                            texentry.OutMapsB[i] = texentry.OutMaps;
                        }
                        else
                        {
                            texentry.WTemp = new byte[(texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i])];
                            texentry.OutMaps = new byte[(texentry._MipMapCount)];
                            System.Buffer.BlockCopy(texentry.UncompressedData, texentry.MipOffsets[i], texentry.WTemp, 0, (texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i]));
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

                    try
                    {

                        using (BinaryWriter bw = new BinaryWriter(File.Open(outname2a, FileMode.Create)))
                        {
                            bw.Write(texentry.OutMaps);
                            bw.Close();
                        }

                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("Unable to access the file. Maybe it's already in use by another proccess?", "Cannot write this file.");
                        break;
                    }

                    Stream stream2a = new MemoryStream(texentry.OutMaps);

                    //texentry.tex = BitmapBuilder(outpngname2a, stream2a);
                    stream2a.Close();
                    break;

                #endregion


                default:
                    break;
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

                    //Actual Tex Loading work here.
                    byte[] VTemp = new byte[4];
                    uint[] LWData = new uint[3];
                    Array.Copy(teXentry.UncompressedData, 8, VTemp, 0, 4);
                    byte[] DTemp = new byte[4];

                    switch (teXentry.TexType)
                    {
                        #region Bitmap Textures
                        case "13":

                            teXentry._Format = "DXT1/BC1";

                            //Gets the unsigned integers which hold data on the texture's dimensions.
                            Array.Copy(teXentry.UncompressedData, 4, DTemp, 0, 4);
                            LWData[0] = BitConverter.ToUInt32(DTemp, 0);

                            Array.Copy(teXentry.UncompressedData, 8, DTemp, 0, 4);
                            LWData[1] = BitConverter.ToUInt32(DTemp, 0);

                            Array.Copy(teXentry.UncompressedData, 12, DTemp, 0, 4);
                            LWData[2] = BitConverter.ToUInt32(DTemp, 0);

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
                                Array.Copy(teXentry.UncompressedData, v, DTemp, 0, 4);

                                //Gets offsets of MipMapData.
                                teXentry.MipOffsets[i] = BitConverter.ToInt32(DTemp, 0);

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

                            break;

                        #endregion

                        #region Bitmap Textures with Transparency
                        case "17":
                            teXentry._Format = "DXT5/BC3";

                            //Gets the unsigned integers which hold data on the texture's dimensions.
                            Array.Copy(teXentry.UncompressedData, 4, DTemp, 0, 4);
                            LWData[0] = BitConverter.ToUInt32(DTemp, 0);

                            Array.Copy(teXentry.UncompressedData, 8, DTemp, 0, 4);
                            LWData[1] = BitConverter.ToUInt32(DTemp, 0);

                            Array.Copy(teXentry.UncompressedData, 12, DTemp, 0, 4);
                            LWData[2] = BitConverter.ToUInt32(DTemp, 0);

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
                                Array.Copy(teXentry.UncompressedData, v17, DTemp, 0, 4);

                                //Gets offsets of MipMapData.
                                teXentry.MipOffsets[i] = BitConverter.ToInt32(DTemp, 0);

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

                            break;
                        #endregion

                        #region Specular Tetures
                        case "19":

                            teXentry._Format = "BC4_UNORM/Metalic/Specular Map";

                            //Gets the unsigned integers which hold data on the texture's dimensions.
                            Array.Copy(teXentry.UncompressedData, 4, DTemp, 0, 4);
                            LWData[0] = BitConverter.ToUInt32(DTemp, 0);

                            Array.Copy(teXentry.UncompressedData, 8, DTemp, 0, 4);
                            LWData[1] = BitConverter.ToUInt32(DTemp, 0);

                            Array.Copy(teXentry.UncompressedData, 12, DTemp, 0, 4);
                            LWData[2] = BitConverter.ToUInt32(DTemp, 0);

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

                            int v19 = 0x10;

                            teXentry.MipOffsets = new int[teXentry.MipMapCount];

                            for (int i = 0; i < teXentry._MipMapCount; i++)
                            {
                                Array.Copy(teXentry.UncompressedData, v19, DTemp, 0, 4);

                                //Gets offsets of MipMapData.
                                teXentry.MipOffsets[i] = BitConverter.ToInt32(DTemp, 0);

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
                                    w = teXentry.WTemp.Length;
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
                            break;

                        #endregion

                        #region Normal Maps(Incomplete)
                        case "1F":

                            teXentry._Format = "BC5/Normal Map";

                            //Gets the unsigned integers which hold data on the texture's dimensions.
                            Array.Copy(teXentry.UncompressedData, 4, DTemp, 0, 4);
                            LWData[0] = BitConverter.ToUInt32(DTemp, 0);

                            Array.Copy(teXentry.UncompressedData, 8, DTemp, 0, 4);
                            LWData[1] = BitConverter.ToUInt32(DTemp, 0);

                            Array.Copy(teXentry.UncompressedData, 12, DTemp, 0, 4);
                            LWData[2] = BitConverter.ToUInt32(DTemp, 0);

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
                                Array.Copy(teXentry.UncompressedData, v1f, DTemp, 0, 4);

                                //Gets offsets of MipMapData.
                                teXentry.MipOffsets[i] = BitConverter.ToInt32(DTemp, 0);

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
                            break;


                        #endregion

                        #region Weird Toon Shader Textures
                        case "27":
                            teXentry._Format = "????/Problematic Portrait Picture";

                            //Gets the unsigned integers which hold data on the texture's dimensions.
                            Array.Copy(teXentry.UncompressedData, 4, DTemp, 0, 4);
                            LWData[0] = BitConverter.ToUInt32(DTemp, 0);

                            Array.Copy(teXentry.UncompressedData, 8, DTemp, 0, 4);
                            LWData[1] = BitConverter.ToUInt32(DTemp, 0);

                            Array.Copy(teXentry.UncompressedData, 12, DTemp, 0, 4);
                            LWData[2] = BitConverter.ToUInt32(DTemp, 0);

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
                                Array.Copy(teXentry.UncompressedData, v27, DTemp, 0, 4);

                                //Gets offsets of MipMapData.
                                teXentry.MipOffsets[i] = BitConverter.ToInt32(DTemp, 0);

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
                            break;

                        #endregion

                        #region Weirdo Problematic Portrait Textures
                        case "2A":

                            teXentry._Format = "????/Problematic Portrait Picture";

                            //Gets the unsigned integers which hold data on the texture's dimensions.
                            Array.Copy(teXentry.UncompressedData, 4, DTemp, 0, 4);
                            LWData[0] = BitConverter.ToUInt32(DTemp, 0);

                            Array.Copy(teXentry.UncompressedData, 8, DTemp, 0, 4);
                            LWData[1] = BitConverter.ToUInt32(DTemp, 0);

                            Array.Copy(teXentry.UncompressedData, 12, DTemp, 0, 4);
                            LWData[2] = BitConverter.ToUInt32(DTemp, 0);

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
                                Array.Copy(teXentry.UncompressedData, v2a, DTemp, 0, 4);

                                //Gets offsets of MipMapData.
                                teXentry.MipOffsets[i] = BitConverter.ToInt32(DTemp, 0);

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
                            break;

                        #endregion

                        default:
                            break;
                    }

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

                }
            }
            catch (Exception ex)
            {

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

                    //Then Compress.
                    texentry.CompressedData = Zlibber.Compressor(texentry.UncompressedData);

                    //Gets the filename of the file to inject without the directory.
                    string trname = filename;
                    while (trname.Contains("\\"))
                    {
                        trname = trname.Substring(trname.IndexOf("\\") + 1);
                    }


                    //Gets Dimensions and Tex Type.
                    texentry.TexType = texentry.UncompressedData[13].ToString("X2");
                    texentry._TextureType = texentry.TexType;

                    //Actual Tex Loading work here.
                    byte[] VTemp = new byte[4];
                    uint[] LWData = new uint[3];
                    Array.Copy(texentry.UncompressedData, 8, VTemp, 0, 4);
                    byte[] DTemp = new byte[4];

                    switch (texentry.TexType)
                    {
                        #region Bitmap Textures
                        case "13":

                            texentry._Format = "DXT1/BC1";

                            //Gets the unsigned integers which hold data on the texture's dimensions.
                            Array.Copy(texentry.UncompressedData, 4, DTemp, 0, 4);
                            LWData[0] = BitConverter.ToUInt32(DTemp, 0);

                            Array.Copy(texentry.UncompressedData, 8, DTemp, 0, 4);
                            LWData[1] = BitConverter.ToUInt32(DTemp, 0);

                            Array.Copy(texentry.UncompressedData, 12, DTemp, 0, 4);
                            LWData[2] = BitConverter.ToUInt32(DTemp, 0);

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

                            texentry.MipOffsets = new int[texentry.MipMapCount];

                            for (int i = 0; i < texentry._MipMapCount; i++)
                            {
                                Array.Copy(texentry.UncompressedData, v, DTemp, 0, 4);

                                //Gets offsets of MipMapData.
                                texentry.MipOffsets[i] = BitConverter.ToInt32(DTemp, 0);

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

                            texentry.OutTexTest.AddRange(DDSHeader);

                            foreach (byte[] array in texentry.OutMapsB)
                            {
                                texentry.OutTexTest.AddRange(array);
                            }

                            texentry.OutMaps = texentry.OutTexTest.ToArray();
                            uint blargx = Convert.ToUInt32(texentry.XSize);
                            uint blargy = Convert.ToUInt32(texentry.YSize);

                            byte[] Xbytes = BitConverter.GetBytes(blargy);
                            byte[] Ybytes = BitConverter.GetBytes(blargx);

                            Array.Copy(Xbytes, 0, texentry.OutMaps, 12, 4);
                            Array.Copy(Ybytes, 0, texentry.OutMaps, 16, 4);

                            break;

                        #endregion

                        #region Bitmap Textures with Transparency
                        case "17":
                            texentry._Format = "DXT5/BC3";

                            //Gets the unsigned integers which hold data on the texture's dimensions.
                            Array.Copy(texentry.UncompressedData, 4, DTemp, 0, 4);
                            LWData[0] = BitConverter.ToUInt32(DTemp, 0);

                            Array.Copy(texentry.UncompressedData, 8, DTemp, 0, 4);
                            LWData[1] = BitConverter.ToUInt32(DTemp, 0);

                            Array.Copy(texentry.UncompressedData, 12, DTemp, 0, 4);
                            LWData[2] = BitConverter.ToUInt32(DTemp, 0);

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

                            int v17 = 0x10;

                            texentry.MipOffsets = new int[texentry.MipMapCount];

                            for (int i = 0; i < texentry._MipMapCount; i++)
                            {
                                Array.Copy(texentry.UncompressedData, v17, DTemp, 0, 4);

                                //Gets offsets of MipMapData.
                                texentry.MipOffsets[i] = BitConverter.ToInt32(DTemp, 0);

                                v17 = v17 + 8;
                            }

                            v17 = 0x10;

                            int w17 = 0;
                            int u17 = 0;

                            //Extracts and separates the Mip Maps.
                            texentry.OutMapsB = new byte[texentry._MipMapCount][];

                            for (int i = 0; i < texentry._MipMapCount; i++)
                            {

                                if ((i) == (texentry.MipOffsets.Length - 1))
                                {
                                    texentry.WTemp = new byte[(texentry.UncompressedData.Length - texentry.MipOffsets[i])];
                                    texentry.OutMaps = new byte[(texentry._MipMapCount)];

                                    System.Buffer.BlockCopy(texentry.UncompressedData, texentry.MipOffsets[i], texentry.WTemp, 0, (texentry.UncompressedData.Length - texentry.MipOffsets[i]));
                                    w17 = texentry.WTemp.Length;
                                    u17 = u17 + texentry.WTemp.Length;

                                    texentry.OutMaps = texentry.WTemp;
                                    texentry.OutMapsB[i] = texentry.OutMaps;
                                }
                                else
                                {
                                    texentry.WTemp = new byte[(texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i])];
                                    texentry.OutMaps = new byte[(texentry._MipMapCount)];
                                    System.Buffer.BlockCopy(texentry.UncompressedData, texentry.MipOffsets[i], texentry.WTemp, 0, (texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i]));
                                    w17 = texentry.WTemp.Length;
                                    u17 = u17 + texentry.WTemp.Length;

                                    texentry.OutMaps = texentry.WTemp;
                                    texentry.OutMapsB[i] = texentry.OutMaps;
                                }

                            }

                            texentry.OutTexTest = new List<byte>();

                            byte[] DDSHeader17 = { 0x44, 0x44, 0x53, 0x20, 0x7c, 0x00, 0x00, 0x00, 0x07, 0x10, 0x08, 0x00, 0x00, 0x01, 0x00, 0x00,
                                         0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                                         0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x35, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                            texentry.OutTexTest.AddRange(DDSHeader17);

                            foreach (byte[] array in texentry.OutMapsB)
                            {
                                texentry.OutTexTest.AddRange(array);
                            }

                            texentry.OutMaps = texentry.OutTexTest.ToArray();
                            uint blargx17 = Convert.ToUInt32(texentry.XSize);
                            uint blargy17 = Convert.ToUInt32(texentry.YSize);

                            byte[] Xbytes17 = BitConverter.GetBytes(blargy17);
                            byte[] Ybytes17 = BitConverter.GetBytes(blargx17);

                            Array.Copy(Xbytes17, 0, texentry.OutMaps, 12, 4);
                            Array.Copy(Ybytes17, 0, texentry.OutMaps, 16, 4);

                            break;
                        #endregion

                        #region Specular Tetures
                        case "19":

                            texentry._Format = "BC4_UNORM/Metalic/Specular Map";

                            //Gets the unsigned integers which hold data on the texture's dimensions.
                            Array.Copy(texentry.UncompressedData, 4, DTemp, 0, 4);
                            LWData[0] = BitConverter.ToUInt32(DTemp, 0);

                            Array.Copy(texentry.UncompressedData, 8, DTemp, 0, 4);
                            LWData[1] = BitConverter.ToUInt32(DTemp, 0);

                            Array.Copy(texentry.UncompressedData, 12, DTemp, 0, 4);
                            LWData[2] = BitConverter.ToUInt32(DTemp, 0);

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
                                Array.Copy(texentry.UncompressedData, v19, DTemp, 0, 4);

                                //Gets offsets of MipMapData.
                                texentry.MipOffsets[i] = BitConverter.ToInt32(DTemp, 0);

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

                            texentry.OutTexTest = new List<byte>();


                            byte[] DDSHeader19 = { 0x44, 0x44, 0x53, 0x20, 0x7c, 0x00, 0x00, 0x00, 0x07, 0x10, 0x08, 0x00, 0x00, 0x01, 0x00, 0x00,
                                         0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                                         0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x31, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                            texentry.OutTexTest.AddRange(DDSHeader19);

                            foreach (byte[] array in texentry.OutMapsB)
                            {
                                texentry.OutTexTest.AddRange(array);
                            }

                            texentry.OutMaps = texentry.OutTexTest.ToArray();
                            uint blargx19 = Convert.ToUInt32(texentry.XSize);
                            uint blargy19 = Convert.ToUInt32(texentry.YSize);

                            byte[] Xbytes19 = BitConverter.GetBytes(blargy19);
                            byte[] Ybytes19 = BitConverter.GetBytes(blargx19);

                            Array.Copy(Xbytes19, 0, texentry.OutMaps, 12, 4);
                            Array.Copy(Ybytes19, 0, texentry.OutMaps, 16, 4);
                            break;

                        #endregion

                        #region Normal Maps(Incomplete)
                        case "1F":

                            texentry._Format = "BC5/Normal Map";

                            //Gets the unsigned integers which hold data on the texture's dimensions.
                            Array.Copy(texentry.UncompressedData, 4, DTemp, 0, 4);
                            LWData[0] = BitConverter.ToUInt32(DTemp, 0);

                            Array.Copy(texentry.UncompressedData, 8, DTemp, 0, 4);
                            LWData[1] = BitConverter.ToUInt32(DTemp, 0);

                            Array.Copy(texentry.UncompressedData, 12, DTemp, 0, 4);
                            LWData[2] = BitConverter.ToUInt32(DTemp, 0);

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
                                Array.Copy(texentry.UncompressedData, v1f, DTemp, 0, 4);

                                //Gets offsets of MipMapData.
                                texentry.MipOffsets[i] = BitConverter.ToInt32(DTemp, 0);

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
                                    texentry.WTemp = new byte[(texentry.UncompressedData.Length - texentry.MipOffsets[i])];
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
                                    texentry.OutMaps = new byte[(texentry._MipMapCount)];
                                    System.Buffer.BlockCopy(texentry.UncompressedData, texentry.MipOffsets[i], texentry.WTemp, 0, (texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i]));
                                    w1f = texentry.WTemp.Length;
                                    u1f = u1f + texentry.WTemp.Length;

                                    texentry.OutMaps = texentry.WTemp;
                                    texentry.OutMapsB[i] = texentry.OutMaps;
                                }

                            }

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

                            texentry.OutMaps = texentry.OutTexTest.ToArray();
                            uint blargx1f = Convert.ToUInt32(texentry.XSize);
                            uint blargy1f = Convert.ToUInt32(texentry.YSize);

                            byte[] Xbytes1f = BitConverter.GetBytes(blargy1f);
                            byte[] Ybytes1f = BitConverter.GetBytes(blargx1f);

                            Array.Copy(Xbytes1f, 0, texentry.OutMaps, 12, 4);
                            Array.Copy(Ybytes1f, 0, texentry.OutMaps, 16, 4);
                            break;


                        #endregion

                        #region Weird Toon Shader Textures
                        case "27":
                            texentry._Format = "????/Problematic Portrait Picture";

                            //Gets the unsigned integers which hold data on the texture's dimensions.
                            Array.Copy(texentry.UncompressedData, 4, DTemp, 0, 4);
                            LWData[0] = BitConverter.ToUInt32(DTemp, 0);

                            Array.Copy(texentry.UncompressedData, 8, DTemp, 0, 4);
                            LWData[1] = BitConverter.ToUInt32(DTemp, 0);

                            Array.Copy(texentry.UncompressedData, 12, DTemp, 0, 4);
                            LWData[2] = BitConverter.ToUInt32(DTemp, 0);

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

                            int v27 = 0x10;

                            texentry.MipOffsets = new int[texentry.MipMapCount];

                            for (int i = 0; i < texentry._MipMapCount; i++)
                            {
                                Array.Copy(texentry.UncompressedData, v27, DTemp, 0, 4);

                                //Gets offsets of MipMapData.
                                texentry.MipOffsets[i] = BitConverter.ToInt32(DTemp, 0);

                                v27 = v27 + 8;
                            }

                            v27 = 0x10;

                            int w27 = 0;
                            int u27 = 0;

                            //Extracts and separates the Mip Maps.
                            texentry.OutMapsB = new byte[texentry._MipMapCount][];

                            for (int i = 0; i < texentry._MipMapCount; i++)
                            {

                                if ((i) == (texentry.MipOffsets.Length - 1))
                                {
                                    texentry.WTemp = new byte[(texentry.UncompressedData.Length - texentry.MipOffsets[i])];
                                    texentry.OutMaps = new byte[(texentry._MipMapCount)];

                                    System.Buffer.BlockCopy(texentry.UncompressedData, texentry.MipOffsets[i], texentry.WTemp, 0, (texentry.UncompressedData.Length - texentry.MipOffsets[i]));
                                    w27 = texentry.WTemp.Length;
                                    u27 = u27 + texentry.WTemp.Length;

                                    texentry.OutMaps = texentry.WTemp;
                                    texentry.OutMapsB[i] = texentry.OutMaps;
                                }
                                else
                                {
                                    texentry.WTemp = new byte[(texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i])];
                                    texentry.OutMaps = new byte[(texentry._MipMapCount)];
                                    System.Buffer.BlockCopy(texentry.UncompressedData, texentry.MipOffsets[i], texentry.WTemp, 0, (texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i]));
                                    w27 = texentry.WTemp.Length;
                                    u27 = u27 + texentry.WTemp.Length;

                                    texentry.OutMaps = texentry.WTemp;
                                    texentry.OutMapsB[i] = texentry.OutMaps;
                                }

                            }

                            texentry.OutTexTest = new List<byte>();


                            byte[] DDSHeader27 = { 0x44, 0x44, 0x53, 0x20, 0x7c, 0x00, 0x00, 0x00, 0x07, 0x10, 0x08, 0x00, 0x00, 0x01, 0x00, 0x00,
                                           0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                                           0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x35, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                            texentry.OutTexTest.AddRange(DDSHeader27);

                            foreach (byte[] array in texentry.OutMapsB)
                            {
                                texentry.OutTexTest.AddRange(array);
                            }

                            texentry.OutMaps = texentry.OutTexTest.ToArray();
                            uint blargx27 = Convert.ToUInt32(texentry.XSize);
                            uint blargy27 = Convert.ToUInt32(texentry.YSize);

                            byte[] Xbytes27 = BitConverter.GetBytes(blargy27);
                            byte[] Ybytes27 = BitConverter.GetBytes(blargx27);

                            Array.Copy(Xbytes27, 0, texentry.OutMaps, 12, 4);
                            Array.Copy(Ybytes27, 0, texentry.OutMaps, 16, 4);
                            break;

                        #endregion

                        #region Weirdo Problematic Portrait Textures
                        case "2A":

                            texentry._Format = "????/Problematic Portrait Picture";

                            //Gets the unsigned integers which hold data on the texture's dimensions.
                            Array.Copy(texentry.UncompressedData, 4, DTemp, 0, 4);
                            LWData[0] = BitConverter.ToUInt32(DTemp, 0);

                            Array.Copy(texentry.UncompressedData, 8, DTemp, 0, 4);
                            LWData[1] = BitConverter.ToUInt32(DTemp, 0);

                            Array.Copy(texentry.UncompressedData, 12, DTemp, 0, 4);
                            LWData[2] = BitConverter.ToUInt32(DTemp, 0);

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

                            int v2a = 0x10;

                            texentry.MipOffsets = new int[texentry.MipMapCount];

                            for (int i = 0; i < texentry._MipMapCount; i++)
                            {
                                Array.Copy(texentry.UncompressedData, v2a, DTemp, 0, 4);

                                //Gets offsets of MipMapData.
                                texentry.MipOffsets[i] = BitConverter.ToInt32(DTemp, 0);

                                v2a = v2a + 8;
                            }

                            v19 = 0x10;

                            int w2a = 0;
                            int u2a = 0;

                            //Extracts and separates the Mip Maps.
                            texentry.OutMapsB = new byte[texentry._MipMapCount][];

                            for (int i = 0; i < texentry._MipMapCount; i++)
                            {

                                if ((i) == (texentry.MipOffsets.Length - 1))
                                {
                                    texentry.WTemp = new byte[(texentry.UncompressedData.Length - texentry.MipOffsets[i])];
                                    texentry.OutMaps = new byte[(texentry._MipMapCount)];

                                    System.Buffer.BlockCopy(texentry.UncompressedData, texentry.MipOffsets[i], texentry.WTemp, 0, (texentry.UncompressedData.Length - texentry.MipOffsets[i]));
                                    w2a = texentry.WTemp.Length;
                                    u2a = u2a + texentry.WTemp.Length;

                                    texentry.OutMaps = texentry.WTemp;
                                    texentry.OutMapsB[i] = texentry.OutMaps;
                                }
                                else
                                {
                                    texentry.WTemp = new byte[(texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i])];
                                    texentry.OutMaps = new byte[(texentry._MipMapCount)];
                                    System.Buffer.BlockCopy(texentry.UncompressedData, texentry.MipOffsets[i], texentry.WTemp, 0, (texentry.MipOffsets[(i + 1)] - texentry.MipOffsets[i]));
                                    w2a = texentry.WTemp.Length;
                                    u2a = u2a + texentry.WTemp.Length;

                                    texentry.OutMaps = texentry.WTemp;
                                    texentry.OutMapsB[i] = texentry.OutMaps;
                                }

                            }

                            texentry.OutTexTest = new List<byte>();


                            byte[] DDSHeader2a = { 0x44, 0x44, 0x53, 0x20, 0x7c, 0x00, 0x00, 0x00, 0x07, 0x10, 0x08, 0x00, 0x00, 0x01, 0x00, 0x00,
                                           0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                                           0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x35, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                            texentry.OutTexTest.AddRange(DDSHeader2a);

                            foreach (byte[] array in texentry.OutMapsB)
                            {
                                texentry.OutTexTest.AddRange(array);
                            }

                            texentry.OutMaps = texentry.OutTexTest.ToArray();
                            uint blargx2a = Convert.ToUInt32(texentry.XSize);
                            uint blargy2a = Convert.ToUInt32(texentry.YSize);

                            byte[] Xbytes2a = BitConverter.GetBytes(blargy2a);
                            byte[] Ybytes2a = BitConverter.GetBytes(blargx2a);

                            Array.Copy(Xbytes2a, 0, texentry.OutMaps, 12, 4);
                            Array.Copy(Ybytes2a, 0, texentry.OutMaps, 16, 4);
                            break;

                        #endregion

                        default:
                            break;
                    }

                    //Enters name related parameters of the arcentry.
                    texentry.TrueName = trname;
                    texentry._FileName = texentry.TrueName;
                    texentry.TrueName = Path.GetFileNameWithoutExtension(trname);
                    texentry.FileExt = trname.Substring(trname.LastIndexOf("."));

                    string TypeHash = "241F5DEB";

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
            catch (Exception ex)
            {
                MessageBox.Show("Read error. Is the file readable?");
            }



            return node.entryfile as TextureEntry;
        }

        public static TextureEntry InsertTextureFromDDS(TreeView tree, ArcEntryWrapper node, string filename, FrmTexEncodeDialog FTED, Type filetype = null)
        {
            TextureEntry teXentry = new TextureEntry();
            
            try
            {
                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
                {

                    /*
                    //We build the arcentry starting from the uncompressed data.
                    teXentry.UncompressedData = System.IO.File.ReadAllBytes(filename);
                    teXentry.DSize = teXentry.UncompressedData.Length;

                    //Then Compress.
                    teXentry.CompressedData = Zlibber.Compressor(teXentry.UncompressedData);
                    teXentry.CSize = teXentry.CompressedData.Length;
                    */


                    if(FTED.TXx > FTED.TXy)
                    {
                       double XD = Convert.ToDouble(FTED.TXx);
                       teXentry.MipMapCount = Convert.ToInt32(Math.Log(XD,2.0));
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
                    string MipBinary = Convert.ToString(teXentry.MipMapCount,2);
                    if(MipBinary.Length < 8)
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
                    TexTemp = BinaryStringToByteArray(bytesstr);

                    switch (teXentry.TexType)
                    {
                        #region Bitmap Textures
                        case "13":
                            teXentry._Format = "DXT1/BC1";

                            byte[] TEXHeader13 = { 0x54, 0x45, 0x58, 0x00, 0x9d, 0xa0, 0x00, 0x20};

                            //What's the opposite of a bitwise AND operation?

                            
                            /*
                    //Gets the unsigned integers which hold data on the texture's dimensions.
                    Array.Copy(texentry.UncompressedData, 4, DTemp, 0, 4);
                    LWData[0] = BitConverter.ToUInt32(DTemp, 0);

                    Array.Copy(texentry.UncompressedData, 8, DTemp, 0, 4);
                    LWData[1] = BitConverter.ToUInt32(DTemp, 0);

                    Array.Copy(texentry.UncompressedData, 12, DTemp, 0, 4);
                    LWData[2] = BitConverter.ToUInt32(DTemp, 0);

                    //X and Y coordinates. This method is borrowed from the old TexCheck.py file.
                    texentry.XSize = Convert.ToInt32(((LWData[1] >> 6) & 0x1fff));
                    texentry._X = texentry.XSize;

                    texentry.YSize = Convert.ToInt32(((LWData[1] >> 19) & 0x1fff));
                    texentry._Y = texentry.YSize;

                    texentry.Mips = Convert.ToInt32(((LWData[1]) & 0x3f));
                    texentry._MipMapCount = texentry.Mips;
                             */

                            break;

                        #endregion

                        #region Bitmap Textures with Transparency
                        case "17":
                            teXentry._Format = "DXT5/BC3";

 
                            break;
                        #endregion

                        #region Specular Tetures
                        case "19":
                            teXentry._Format = "BC4_UNORM/Metalic/Specular Map";


                            break;

                        #endregion

                        #region Normal Maps(Incomplete)
                        case "1F":
                            teXentry._Format = "BC5/Normal Map";


                            break;


                        #endregion

                        #region Weird Toon Shader Textures
                        case "27":
                            teXentry._Format = "????/Problematic Portrait Picture";

 
                            break;

                        #endregion

                        #region Weirdo Problematic Portrait Textures
                        case "2A":
                            teXentry._Format = "????/Problematic Portrait Picture";

 
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

            }



            return teXentry;
        }

    }
}
