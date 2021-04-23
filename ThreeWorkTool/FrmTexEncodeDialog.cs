using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ThreeWorkTool.Resources;

namespace ThreeWorkTool
{
    public partial class FrmTexEncodeDialog : Form
    {
        public FrmTexEncodeDialog()
        {
            InitializeComponent();
        }

        public int TXx;
        public int TXy;
        public int TXmips;
        public string TXfilename;
        public byte[] DDSData;
        public byte[] DDTemp;
        public byte[] XDTemp;
        public int TXPixelFormat;
        public string TXTextureType;
        public string[] TXTxTypeDescriptions;
        public string ShortName;
        public byte[] FirstMip;
        public List<int> MMOffsets;
        public List<int> MMipSizes;
        public List<int> TXOffsets;
        public byte[] MipMaps;
        public Bitmap TXpreview;
        public byte[] TexData;
        public List<byte> TempTexData;
        public bool IsReplacing;


        public static FrmTexEncodeDialog LoadDDSData(string openedfile, OpenFileDialog ofd)
        {
            FrmTexEncodeDialog fted = new FrmTexEncodeDialog();

            try
            {
                using (FileStream fs = File.OpenRead(openedfile))
                {
                    //Checks Magic.
                    byte[] Magic = new byte[4];
                    fs.Read(Magic,0,4);

                    if(Magic[0] == 0x44 && Magic[1] == 0x44 && Magic[2] == 0x53 && Magic[3] == 0x20)
                    {
                        //Gets Dimensions.
                        byte[] DimTemp = new byte[4];

                        fs.Position = 16;
                        fs.Read(DimTemp, 0, 4);
                        Array.Reverse(DimTemp);
                        string TempStr = "";
                        TempStr = BytesToString(DimTemp, TempStr);
                        BigInteger BN1;
                        BN1 = BigInteger.Parse(TempStr, NumberStyles.HexNumber);
                        fted.TXx = (int)BN1;

                        fs.Position = 12;
                        fs.Read(DimTemp, 0, 4);
                        Array.Reverse(DimTemp);
                        TempStr = "";
                        TempStr = BytesToString(DimTemp, TempStr);
                        BN1 = BigInteger.Parse(TempStr, NumberStyles.HexNumber);
                        fted.TXy = (int)BN1;

                        fs.Position = 84;
                        fs.Read(DimTemp, 0, 4);
                        TempStr = "";
                        TempStr = BytesToString(DimTemp, TempStr);
                        /*
                        switch (TempStr)
                        {
                            //DXT1
                            case "44585431":
                                fted.cmBoxTextureType.SelectedIndex = 0;
                                fted.TXTextureType = "13";
                                break;

                            //DXT5
                            case "44585435":
                                fted.cmBoxTextureType.SelectedIndex = 1;
                                fted.TXTextureType = "17";
                                break;

                            //Etc.
                            default:

                                break;
                        }
                        */

                        fs.Position = 28;
                        fs.Read(DimTemp, 0, 4);
                        fted.TXmips = DimTemp[0];

                        //Inserts the data in the form.
                        fted.lblMips.Text = Convert.ToString(fted.TXmips);
                        fted.lblY.Text = Convert.ToString(fted.TXy);
                        fted.lblX.Text = Convert.ToString(fted.TXx);
                        fted.lblY.Text = Convert.ToString(fted.TXy);
                        fted.TXfilename = openedfile;
                        
                        //Gets Filename without the extension and the previous directories.
                        while (fted.TXfilename.Contains("\\"))
                        {
                            fted.TXfilename = fted.TXfilename.Substring(fted.TXfilename.IndexOf("\\") + 1);
                        }

                        int index = fted.TXfilename.IndexOf(".");
                        if (index > 0)
                            fted.TXfilename = fted.TXfilename.Substring(0, index);

                        fs.Close();

                        fted.ShortName = fted.TXfilename;
                        fted.DDSData = File.ReadAllBytes(openedfile);

                        Stream ztrim = new MemoryStream(fted.DDSData);
                        byte[] TexData = new byte[] { };

                        switch (TempStr)
                        {
                            //DXT1
                            case "44585431":
                                fted.cmBoxTextureType.SelectedIndex = 0;
                                fted.TXTextureType = "13";
                                break;

                            //DXT5
                            case "44585435":
                                fted.cmBoxTextureType.SelectedIndex = 1;
                                fted.TXTextureType = "17";
                                break;

                            //Etc.
                            default:

                                break;
                        }


                        //From the pfim website. Modified for my uses.
                        using (var image = Pfim.Pfim.FromStream(ztrim))
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



                                var datai = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);
                                var pmap = new Bitmap(fted.TXx, fted.TXy, image.Stride, format, datai);

                            }
                            finally
                            {
                                handle.Free();
                            }
                        }

                        fted.txtTexConvFile.Text = openedfile;


                    }
                    else
                    {
                        MessageBox.Show("This ain't no DDS file.", "Boy...");
                        return null;
                    }


                }
            }
            catch (Exception vi)
            {
                MessageBox.Show("Either this file isn't a proper DDS file or I can't read it because it's in use by some other proccess.", "Hey.");
                return null;
            }

            return fted;
        }

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

        private void btnTexCancel_Click(object sender, EventArgs e)
        {
            //Closes the form without making changes.
            DialogResult = DialogResult.Cancel;
            Hide();
        }

        private void btnTexOK_Click(object sender, EventArgs e)
        {                                  
            //Closes form with changes made above.
            DialogResult = DialogResult.OK;
            Hide();
        }

        private void cmBoxTextureType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.MMipSizes = new List<int>();
            this.MMOffsets = new List<int>();
            this.TXOffsets = new List<int>();

            switch (cmBoxTextureType.SelectedIndex)
            {

                case 0:
                    this.TXTextureType = "13";

                    //Little Endian Binary. Gotta love it.
                    this.TempTexData = new List<byte>();
                    Byte[] TexHeader = { 0x54, 0x45, 0x58, 0x00, 0x9D, 0xA0, 0x00, 0x20 };
                    this.TempTexData.AddRange(TexHeader);
                    string WidthTemp = Convert.ToString(this.TXx, 2);
                    WidthTemp = WidthTemp.Substring(0, WidthTemp.Length - 2);
                    int wt = 11 - WidthTemp.Length;
                    if (wt > 0)
                    {
                        WidthTemp = WidthTemp.PadLeft(11, '0');
                    }

                    string LengthTemp = Convert.ToString(this.TXy, 2);
                    int lt = 13 - LengthTemp.Length;
                    if (lt > 0)
                    {
                        LengthTemp = LengthTemp.PadLeft(13, '0');
                    }

                    string WidthTA = WidthTemp.Substring(WidthTemp.Length - 8);
                    string WidthTB = WidthTemp.Substring(0, WidthTemp.Length - 8);

                    string LengthTA = LengthTemp.Substring(LengthTemp.Length - 5);
                    string LengthTB = LengthTemp.Substring(0, LengthTemp.Length - 5);

                    string Byte2 = WidthTA;
                    string Byte3 = LengthTA + WidthTB;
                    string Byte4 = LengthTB;

                    byte[] B2 = BSWConverter.BinaryToByteArray(Byte2);
                    byte[] B3 = BSWConverter.BinaryToByteArray(Byte3);
                    byte[] B4 = BSWConverter.BinaryToByteArray(Byte4);

                    TempTexData.Add(Convert.ToByte(this.TXmips));
                    TempTexData.AddRange(B2);
                    TempTexData.AddRange(B3);
                    TempTexData.AddRange(B4);
                    TempTexData.Add(0x01);
                    TempTexData.Add(0x13);
                    TempTexData.Add(0x01);
                    TempTexData.Add(0x00);

                    int MpMpTest = Math.Max(1, (this.TXx + 3) / 4) * Math.Max(1, (this.TXy + 3) / 4) * 8;

                    //Allocating room for Mip Offsets and the start of the MipMapData by calculating the size of each mip map and offsets for addresses.
                    int MippMapPixelSizeThingDXT1 = 0;
                    int MipOffset = 0;
                    int OffTemp = 0;
                    int DDSOffset = 128;
                    for (int p = 0; p < this.TXmips; p++)
                    {
                        MipOffset = 16 + (8 * p);
                        this.MMOffsets.Add(MipOffset);
                        MippMapPixelSizeThingDXT1 = Math.Max(1, ((this.TXx / (Convert.ToInt32(Math.Pow(2, p)))) + 3) / 4) * Math.Max(1, ((this.TXy / (Convert.ToInt32(Math.Pow(2, p)))) + 3) / 4) * 8;
                        this.MMipSizes.Add(MippMapPixelSizeThingDXT1);
                    }

                    byte[][] MipMaps = new byte[this.TXmips][];
                    //Reads and extracts from the DDS file stored in memory.
                    for (int q = 0; q < this.TXmips; q++)
                    {

                        DDTemp = new byte[this.MMipSizes[q]];

                        XDTemp = new byte[(this.TXmips)];

                        Buffer.BlockCopy(this.DDSData, DDSOffset, DDTemp, 0, (this.MMipSizes[q]));
                        DDSOffset = DDSOffset + DDTemp.Length;

                        MipMaps[q] = DDTemp;

                    }

                    this.FirstMip = MipMaps[0];

                    OffTemp = 16 + (8 * this.TXmips);
                    byte[] FillerBytes = {0x00,0x00,0x00,0x00};

                    //Finishes the tex header by putting in the offsets as double words.
                    for (int r =0;r < this.TXmips; r++)
                    {
                        byte[] OTemp = BitConverter.GetBytes(OffTemp);
                        //Array.Reverse(OTemp);
                        this.TempTexData.AddRange(OTemp);
                        OffTemp = OffTemp + MipMaps[r].Length;
                        this.TempTexData.AddRange(FillerBytes);
                    }

                    //Now the MipMaps go in the AFTER Tex header.
                    for(int s = 0; s < this.TXmips; s++)
                    {
                        this.TempTexData.AddRange(MipMaps[s]); 
                    }

                    this.TexData = this.TempTexData.ToArray();



                    break;

                case 1:
                    this.TXTextureType = "17";

                    //Little Endian Binary. Gotta love it.
                    this.TempTexData = new List<byte>();
                    Byte[] TexHeader17 = { 0x54, 0x45, 0x58, 0x00, 0x9D, 0xA0, 0x00, 0x20 };
                    this.TempTexData.AddRange(TexHeader17);
                    string WidthTemp17 = Convert.ToString(this.TXx, 2);
                    WidthTemp17 = WidthTemp17.Substring(0, WidthTemp17.Length - 2);
                    int wt17 = 11 - WidthTemp17.Length;
                    if (wt17 > 0)
                    {
                        WidthTemp17 = WidthTemp17.PadLeft(11, '0');
                    }

                    string LengthTemp17 = Convert.ToString(this.TXy, 2);
                    int lt17 = 13 - LengthTemp17.Length;
                    if (lt17 > 0)
                    {
                        LengthTemp17 = LengthTemp17.PadLeft(13, '0');
                    }

                    string WidthTA17 = WidthTemp17.Substring(WidthTemp17.Length - 8);
                    string WidthTB17 = WidthTemp17.Substring(0, WidthTemp17.Length - 8);

                    string LengthTA17 = LengthTemp17.Substring(LengthTemp17.Length - 5);
                    string LengthTB17 = LengthTemp17.Substring(0, LengthTemp17.Length - 5);

                    string Byte217 = WidthTA17;
                    string Byte317 = LengthTA17 + WidthTB17;
                    string Byte417 = LengthTB17;

                    byte[] B217 = BSWConverter.BinaryToByteArray(Byte217);
                    byte[] B317 = BSWConverter.BinaryToByteArray(Byte317);
                    byte[] B417 = BSWConverter.BinaryToByteArray(Byte417);

                    TempTexData.Add(Convert.ToByte(this.TXmips));
                    TempTexData.AddRange(B217);
                    TempTexData.AddRange(B317);
                    TempTexData.AddRange(B417);
                    TempTexData.Add(0x01);
                    TempTexData.Add(0x17);
                    TempTexData.Add(0x01);
                    TempTexData.Add(0x00);

                    int MpMpTest17 = Math.Max(1, (this.TXx + 3) / 4) * Math.Max(1, (this.TXy + 3) / 4) * 16;

                    //Allocating room for Mip Offsets and the start of the MipMapData by calculating the size of each mip map and offsets for addresses.
                    int MippMapPixelSizeThingDXT117 = 0;
                    int MipOffset17 = 0;
                    int OffTemp17 = 0;
                    int DDSOffset17 = 128;
                    for (int p = 0; p < this.TXmips; p++)
                    {
                        MipOffset = 16 + (8 * p);
                        this.MMOffsets.Add(MipOffset17);
                        MippMapPixelSizeThingDXT117 = Math.Max(1, ((this.TXx / (Convert.ToInt32(Math.Pow(2, p)))) + 3) / 4) * Math.Max(1, ((this.TXy / (Convert.ToInt32(Math.Pow(2, p)))) + 3) / 4) * 16;
                        this.MMipSizes.Add(MippMapPixelSizeThingDXT117);
                    }

                    byte[][] MipMaps17 = new byte[this.TXmips][];
                    //Reads and extracts from the DDS file stored in memory.
                    for (int q = 0; q < this.TXmips; q++)
                    {

                        DDTemp = new byte[this.MMipSizes[q]];

                        XDTemp = new byte[(this.TXmips)];

                        Buffer.BlockCopy(this.DDSData, DDSOffset17, DDTemp, 0, (this.MMipSizes[q]));
                        DDSOffset17 = DDSOffset17 + DDTemp.Length;

                        MipMaps17[q] = DDTemp;

                    }

                    this.FirstMip = MipMaps17[0];

                    OffTemp17 = 16 + (8 * this.TXmips);
                    byte[] FillerBytes17 = { 0x00, 0x00, 0x00, 0x00 };

                    //Finishes the tex header by putting in the offsets as double words.
                    for (int r = 0; r < this.TXmips; r++)
                    {
                        byte[] OTemp = BitConverter.GetBytes(OffTemp17);
                        //Array.Reverse(OTemp);
                        this.TempTexData.AddRange(OTemp);
                        OffTemp17 = OffTemp17 + MipMaps17[r].Length;
                        this.TempTexData.AddRange(FillerBytes17);
                    }

                    //Now the MipMaps go in the AFTER Tex header.
                    for (int s = 0; s < this.TXmips; s++)
                    {
                        this.TempTexData.AddRange(MipMaps17[s]);
                    }

                    this.TexData = this.TempTexData.ToArray();

                    break;

                case 2:
                    this.TXTextureType = "19";
                    break;

                case 3:
                    this.TXTextureType = "1F";
                    break;

                case 4:
                    this.TXTextureType = "27";
                    break;

                case 5:
                    this.TXTextureType = "2A";
                    break;

                default:
                    this.TXTextureType = "13";
                    break;

            }
        }
    }

}
