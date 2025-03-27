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
using ThreeWorkTool.Resources.Utility;

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
        public bool IsDXT1;
        public bool Dialoginit;
        public string DXType;

        public static FrmTexEncodeDialog LoadDDSData(string openedfile, OpenFileDialog ofd)
        {
            FrmTexEncodeDialog fted = new FrmTexEncodeDialog();

            try
            {
                using (FileStream fs = File.OpenRead(openedfile))
                {

                    //Checks Magic.
                    byte[] Magic = new byte[4];
                    fs.Read(Magic, 0, 4);

                    if (Magic[0] == 0x44 && Magic[1] == 0x44 && Magic[2] == 0x53 && Magic[3] == 0x20)
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

                        fted.DXType = TempStr;

                        //Checks Suffix and will switch to appropriate map if true.
                        string Suffix = openedfile.Substring((openedfile.LastIndexOf("_") + 1), 2);

                        switch (TempStr)
                        {
                            //DXT1
                            case "44585431":



                                byte[] DHTemp =    { 0x44, 0x44, 0x53, 0x20, 0x7c, 0x00, 0x00, 0x00, 0x07, 0x10, 0x08, 0x00, 0x00, 0x01, 0x00, 0x00,
                                                     0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
                                                     0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                     0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                     0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                                                     0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x31, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                     0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                                                     0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };


                                List<byte> PrevTemp = new List<byte>();
                                byte[] LenTemp = BitConverter.GetBytes(fted.TXy);
                                byte[] WidTemp = BitConverter.GetBytes(fted.TXx);

                                Array.Copy(LenTemp, 0, DHTemp, 12, 4);
                                Array.Copy(WidTemp, 0, DHTemp, 16, 4);

                                int FirstMipSize = Math.Max(1, (fted.TXx + 3) / 4) * Math.Max(1, (fted.TXy + 3) / 4) * 8;

                                byte[] MipPix = new byte[FirstMipSize];

                                Array.Copy(fted.DDSData, 128, MipPix, 0, FirstMipSize);


                                PrevTemp.AddRange(DHTemp);
                                PrevTemp.AddRange(MipPix);

                                fted.FirstMip = PrevTemp.ToArray();
                                fted.IsDXT1 = true;

                                if (Suffix == "MM")
                                {
                                    fted.cmBoxTextureType.SelectedIndex = 2;
                                    fted.TXTextureType = "19";
                                }
                                else
                                {
                                    fted.cmBoxTextureType.SelectedIndex = 0;
                                    fted.TXTextureType = "13";
                                }

                                break;

                            //DXT5
                            case "44585435":




                                byte[] DHTemp17 =    { 0x44, 0x44, 0x53, 0x20, 0x7c, 0x00, 0x00, 0x00, 0x07, 0x10, 0x08, 0x00, 0x00, 0x01, 0x00, 0x00,
                                                     0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
                                                     0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                     0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                     0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                                                     0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x35, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                     0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                                                     0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };


                                List<byte> PrevTemp17 = new List<byte>();
                                byte[] LenTemp17 = BitConverter.GetBytes(fted.TXy);
                                byte[] WidTemp17 = BitConverter.GetBytes(fted.TXx);

                                Array.Copy(LenTemp17, 0, DHTemp17, 12, 4);
                                Array.Copy(WidTemp17, 0, DHTemp17, 16, 4);

                                int FirstMipSize17 = Math.Max(1, (fted.TXx + 3) / 4) * Math.Max(1, (fted.TXy + 3) / 4) * 16;

                                byte[] MipPix17 = new byte[FirstMipSize17];

                                Array.Copy(fted.DDSData, 128, MipPix17, 0, FirstMipSize17);


                                PrevTemp17.AddRange(DHTemp17);
                                PrevTemp17.AddRange(MipPix17);

                                fted.FirstMip = PrevTemp17.ToArray();
                                fted.IsDXT1 = false;

                                if (Suffix == "NM")
                                {
                                    fted.cmBoxTextureType.SelectedIndex = 3;
                                    fted.TXTextureType = "1F";
                                }
                                else
                                {
                                    fted.cmBoxTextureType.SelectedIndex = 1;
                                    fted.TXTextureType = "17";
                                }

                                break;

                            //Etc.
                            default:
                                fted.IsDXT1 = false;
                                break;
                        }

                        //Time to convert this to png for RGBA related reasons.
                        byte[] DDSTemp = new byte[fted.DDSData.Length];
                        DDSTemp = fted.DDSData;
                        byte[] RGBATemp13 = new byte[] { };

                        using (Stream strim = new MemoryStream(DDSTemp))
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


                        fted.txtTexConvFile.Text = openedfile;
                        fted.TXpreview = ByteUtilitarian.BitmapBuilderDXEncode(fted.DDSData, fted);


                        if (fted.TXpreview == null)
                        {
                            fted.PicBoxTex.Image = fted.PicBoxTex.ErrorImage;
                        }
                        else
                        {
                            if (fted.TXpreview.Width > fted.PicBoxTex.Width || fted.TXpreview.Height > fted.PicBoxTex.Height)
                            {
                                int OldX = fted.PicBoxTex.Width;
                                int OldY = fted.PicBoxTex.Height;
                                fted.PicBoxTex.Image = fted.TXpreview;
                                fted.PicBoxTex.SizeMode = fted.TXpreview.Width > OldX || fted.TXpreview.Height > OldY ?
                                PictureBoxSizeMode.Zoom : PictureBoxSizeMode.CenterImage;

                            }
                            else
                            {
                                fted.PicBoxTex.Image = fted.TXpreview;
                                fted.PicBoxTex.SizeMode = fted.TXpreview.Width < fted.PicBoxTex.Width || fted.TXpreview.Height < fted.PicBoxTex.Height ?
                                PictureBoxSizeMode.Zoom : PictureBoxSizeMode.CenterImage;
                            }
                        }

                        //Creates Background Checkerboard for PictureBox. Thank you TaW.

                        if (fted.PicBoxTex.BackgroundImage != null)
                        {
                            fted.PicBoxTex.BackgroundImage.Dispose();
                            fted.PicBoxTex.BackgroundImage = null;
                            //return;
                        }
                        int size = 16;
                        Bitmap bmp = new Bitmap(size * 2, size * 2);
                        using (SolidBrush brush = new SolidBrush(Color.Gray))
                        using (Graphics G = Graphics.FromImage(bmp))
                        {
                            G.FillRectangle(brush, 0, 0, size, size);
                            G.FillRectangle(brush, size, size, size, size);
                        }
                        fted.PicBoxTex.BackgroundImage = bmp;
                        fted.PicBoxTex.BackgroundImageLayout = ImageLayout.Tile;

                        //fted.PicBoxTex

                    }
                    else
                    {
                        MessageBox.Show("This ain't no DDS file.", "Boy...");
                        return null;
                    }


                }
            }
            catch (Exception ex)
            {

                if (ex is IOException)
                {
                    MessageBox.Show("Unable to import because another proccess is using it.");
                    string ProperPath = "";
                    ProperPath = Globals.ToolPath + "Log.txt"; using (StreamWriter sw = File.AppendText(ProperPath))
                    {
                        sw.WriteLine("Cannot access the file: " + "\nbecause another process is using it.");
                    }
                    return null;
                }
                else
                {
                    MessageBox.Show("The DDS file is either malinformed or is not the correct format/kind.");
                    string ProperPath = "";
                    ProperPath = Globals.ToolPath + "Log.txt"; using (StreamWriter sw = File.AppendText(ProperPath))
                    {
                        sw.WriteLine("Cannot import: " + "\nbecause it's an invalid dds file.");
                    }
                    return null;
                }

            }

            fted.Dialoginit = false;

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

                    if (this.IsDXT1 == false && this.DXType == "44585435")
                    {
                        MessageBox.Show("This texture was saved as a DXT5 .DDS file, meaning the DXT compression method used for this is not compatible with this texture type.\nIf you want to import this as this type of texture, return to the image editing software you used to save this and make sure to save it as a DXT1 instead.", "Hold it!");
                        cmBoxTextureType.SelectedIndex = 1;
                        return;
                    }
                    else
                    {
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
                        byte[] FillerBytes = { 0x00, 0x00, 0x00, 0x00 };

                        //Finishes the tex header by putting in the offsets as double words.
                        for (int r = 0; r < this.TXmips; r++)
                        {
                            byte[] OTemp = BitConverter.GetBytes(OffTemp);
                            //Array.Reverse(OTemp);
                            this.TempTexData.AddRange(OTemp);
                            OffTemp = OffTemp + MipMaps[r].Length;
                            this.TempTexData.AddRange(FillerBytes);
                        }

                        //Now the MipMaps go in the AFTER Tex header.
                        for (int s = 0; s < this.TXmips; s++)
                        {
                            this.TempTexData.AddRange(MipMaps[s]);
                        }

                        this.TexData = this.TempTexData.ToArray();

                        this.Dialoginit = true;

                        break;
                    }

                case 1:

                    if (this.IsDXT1 == true && this.DXType == "44585431")
                    {
                        MessageBox.Show("This texture was saved as a DXT1 .DDS file, meaning the DXT compression method used for this is not compatible with this texture type.\nIf you want to import this as this type of texture, return to the image editing software you used to save this and make sure to save it as a DXT5 instead.", "Hold it!");
                        cmBoxTextureType.SelectedIndex = 0;
                        return;
                    }
                    else
                    {

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
                            MipOffset17 = 16 + (8 * p);
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

                        this.Dialoginit = true;

                        break;

                    }



                case 2:

                    if (this.IsDXT1 == false)
                    {
                        MessageBox.Show("This texture was saved as a DXT5 .DDS file, meaning the DXT compression method used for this is not compatible with this texture type.\nIf you want to import this as this type of texture, return to the image editing software you used to save this and make sure to save it as a DXT1 instead.", "Hold it!");
                        cmBoxTextureType.SelectedIndex = 1;
                        return;
                    }
                    else
                    {
                        this.TXTextureType = "19";

                        //Little Endian Binary. Gotta love it.
                        this.TempTexData = new List<byte>();
                        Byte[] TexHeader19 = { 0x54, 0x45, 0x58, 0x00, 0x9D, 0xA0, 0x00, 0x20 };
                        this.TempTexData.AddRange(TexHeader19);
                        string WidthTemp19 = Convert.ToString(this.TXx, 2);
                        WidthTemp19 = WidthTemp19.Substring(0, WidthTemp19.Length - 2);
                        int wt19 = 11 - WidthTemp19.Length;
                        if (wt19 > 0)
                        {
                            WidthTemp19 = WidthTemp19.PadLeft(11, '0');
                        }

                        string LengthTemp19 = Convert.ToString(this.TXy, 2);
                        int lt19 = 13 - LengthTemp19.Length;
                        if (lt19 > 0)
                        {
                            LengthTemp19 = LengthTemp19.PadLeft(13, '0');
                        }

                        string WidthTA19 = WidthTemp19.Substring(WidthTemp19.Length - 8);
                        string WidthTB19 = WidthTemp19.Substring(0, WidthTemp19.Length - 8);

                        string LengthTA19 = LengthTemp19.Substring(LengthTemp19.Length - 5);
                        string LengthTB19 = LengthTemp19.Substring(0, LengthTemp19.Length - 5);

                        string Byte219 = WidthTA19;
                        string Byte319 = LengthTA19 + WidthTB19;
                        string Byte419 = LengthTB19;

                        byte[] B219 = BSWConverter.BinaryToByteArray(Byte219);
                        byte[] B319 = BSWConverter.BinaryToByteArray(Byte319);
                        byte[] B419 = BSWConverter.BinaryToByteArray(Byte419);

                        TempTexData.Add(Convert.ToByte(this.TXmips));
                        TempTexData.AddRange(B219);
                        TempTexData.AddRange(B319);
                        TempTexData.AddRange(B419);
                        TempTexData.Add(0x01);
                        TempTexData.Add(0x19);
                        TempTexData.Add(0x01);
                        TempTexData.Add(0x00);

                        int MpMpTest19 = Math.Max(1, (this.TXx + 3) / 4) * Math.Max(1, (this.TXy + 3) / 4) * 8;

                        //Allocating room for Mip Offsets and the start of the MipMapData by calculating the size of each mip map and offsets for addresses.
                        int MippMapPixelSizeThingDXT119 = 0;
                        int MipOffset19 = 0;
                        int OffTemp19 = 0;
                        int DDSOffset19 = 128;
                        for (int p = 0; p < this.TXmips; p++)
                        {
                            MipOffset19 = 16 + (8 * p);
                            this.MMOffsets.Add(MipOffset19);
                            MippMapPixelSizeThingDXT119 = Math.Max(1, ((this.TXx / (Convert.ToInt32(Math.Pow(2, p)))) + 3) / 4) * Math.Max(1, ((this.TXy / (Convert.ToInt32(Math.Pow(2, p)))) + 3) / 4) * 8;
                            this.MMipSizes.Add(MippMapPixelSizeThingDXT119);
                        }

                        byte[][] MipMaps19 = new byte[this.TXmips][];
                        //Reads and extracts from the DDS file stored in memory.
                        for (int q = 0; q < this.TXmips; q++)
                        {

                            DDTemp = new byte[this.MMipSizes[q]];

                            XDTemp = new byte[(this.TXmips)];

                            Buffer.BlockCopy(this.DDSData, DDSOffset19, DDTemp, 0, (this.MMipSizes[q]));
                            DDSOffset19 = DDSOffset19 + DDTemp.Length;

                            MipMaps19[q] = DDTemp;

                        }

                        this.FirstMip = MipMaps19[0];

                        OffTemp19 = 16 + (8 * this.TXmips);
                        byte[] FillerBytes19 = { 0x00, 0x00, 0x00, 0x00 };

                        //Finishes the tex header by putting in the offsets as double words.
                        for (int r = 0; r < this.TXmips; r++)
                        {
                            byte[] OTemp19 = BitConverter.GetBytes(OffTemp19);
                            //Array.Reverse(OTemp);
                            this.TempTexData.AddRange(OTemp19);
                            OffTemp19 = OffTemp19 + MipMaps19[r].Length;
                            this.TempTexData.AddRange(FillerBytes19);
                        }

                        //Now the MipMaps go in the AFTER Tex header.
                        for (int s = 0; s < this.TXmips; s++)
                        {
                            this.TempTexData.AddRange(MipMaps19[s]);
                        }

                        this.TexData = this.TempTexData.ToArray();

                        break;
                    }

                case 3:

                    if (this.IsDXT1 == true)
                    {
                        MessageBox.Show("This texture was saved as a DXT1 .DDS file, meaning the DXT compression method used for this is not compatible with this texture type.\nIf you want to import this as this type of texture, return to the image editing software you used to save this and make sure to save it as a DXT5 instead.", "Hold it!");
                        cmBoxTextureType.SelectedIndex = 0;
                        return;
                    }
                    else
                    {
                        this.TXTextureType = "1F";

                        //Little Endian Binary. Gotta love it.
                        this.TempTexData = new List<byte>();
                        Byte[] TexHeader1F = { 0x54, 0x45, 0x58, 0x00, 0x9D, 0xA0, 0x00, 0x20 };
                        this.TempTexData.AddRange(TexHeader1F);
                        string WidthTemp1F = Convert.ToString(this.TXx, 2);
                        WidthTemp1F = WidthTemp1F.Substring(0, WidthTemp1F.Length - 2);
                        int wt1F = 11 - WidthTemp1F.Length;
                        if (wt1F > 0)
                        {
                            WidthTemp1F = WidthTemp1F.PadLeft(11, '0');
                        }

                        string LengthTemp1F = Convert.ToString(this.TXy, 2);
                        int lt1F = 13 - LengthTemp1F.Length;
                        if (lt1F > 0)
                        {
                            LengthTemp1F = LengthTemp1F.PadLeft(13, '0');
                        }

                        string WidthTA1F = WidthTemp1F.Substring(WidthTemp1F.Length - 8);
                        string WidthTB1F = WidthTemp1F.Substring(0, WidthTemp1F.Length - 8);

                        string LengthTA1F = LengthTemp1F.Substring(LengthTemp1F.Length - 5);
                        string LengthTB1F = LengthTemp1F.Substring(0, LengthTemp1F.Length - 5);

                        string Byte21F = WidthTA1F;
                        string Byte31F = LengthTA1F + WidthTB1F;
                        string Byte41F = LengthTB1F;

                        byte[] B21F = BSWConverter.BinaryToByteArray(Byte21F);
                        byte[] B31F = BSWConverter.BinaryToByteArray(Byte31F);
                        byte[] B41F = BSWConverter.BinaryToByteArray(Byte41F);

                        TempTexData.Add(Convert.ToByte(this.TXmips));
                        TempTexData.AddRange(B21F);
                        TempTexData.AddRange(B31F);
                        TempTexData.AddRange(B41F);
                        TempTexData.Add(0x01);
                        TempTexData.Add(0x1F);
                        TempTexData.Add(0x01);
                        TempTexData.Add(0x00);

                        int MpMpTest1F = Math.Max(1, (this.TXx + 3) / 4) * Math.Max(1, (this.TXy + 3) / 4) * 16;

                        //Allocating room for Mip Offsets and the start of the MipMapData by calculating the size of each mip map and offsets for addresses.
                        int MippMapPixelSizeThingDXT11F = 0;
                        int MipOffset1F = 0;
                        int OffTemp1F = 0;
                        int DDSOffset1F = 128;
                        for (int p = 0; p < this.TXmips; p++)
                        {
                            MipOffset1F = 16 + (8 * p);
                            this.MMOffsets.Add(MipOffset1F);
                            MippMapPixelSizeThingDXT11F = Math.Max(1, ((this.TXx / (Convert.ToInt32(Math.Pow(2, p)))) + 3) / 4) * Math.Max(1, ((this.TXy / (Convert.ToInt32(Math.Pow(2, p)))) + 3) / 4) * 16;
                            this.MMipSizes.Add(MippMapPixelSizeThingDXT11F);
                        }

                        byte[][] MipMaps1F = new byte[this.TXmips][];
                        //Reads and extracts from the DDS file stored in memory.
                        for (int q = 0; q < this.TXmips; q++)
                        {

                            DDTemp = new byte[this.MMipSizes[q]];

                            XDTemp = new byte[(this.TXmips)];

                            Buffer.BlockCopy(this.DDSData, DDSOffset1F, DDTemp, 0, (this.MMipSizes[q]));
                            DDSOffset1F = DDSOffset1F + DDTemp.Length;

                            MipMaps1F[q] = DDTemp;

                        }

                        this.FirstMip = MipMaps1F[0];

                        OffTemp1F = 16 + (8 * this.TXmips);
                        byte[] FillerBytes1F = { 0x00, 0x00, 0x00, 0x00 };

                        //Finishes the tex header by putting in the offsets as double words.
                        for (int r = 0; r < this.TXmips; r++)
                        {
                            byte[] OTemp = BitConverter.GetBytes(OffTemp1F);
                            //Array.Reverse(OTemp);
                            this.TempTexData.AddRange(OTemp);
                            OffTemp1F = OffTemp1F + MipMaps1F[r].Length;
                            this.TempTexData.AddRange(FillerBytes1F);
                        }

                        //Now the MipMaps go in the AFTER Tex header.
                        for (int s = 0; s < this.TXmips; s++)
                        {
                            this.TempTexData.AddRange(MipMaps1F[s]);
                        }

                        this.TexData = this.TempTexData.ToArray();

                        break;
                    }

                case 4:

                    if (this.IsDXT1 == true)
                    {
                        MessageBox.Show("This texture was saved as a DXT1 .DDS file, meaning the DXT compression method used for this is not compatible with this texture type.\nIf you want to import this as this type of texture, return to the image editing software you used to save this and make sure to save it as a DXT5 instead.", "Hold it!");
                        cmBoxTextureType.SelectedIndex = 0;
                        return;
                    }
                    else
                    {

                        this.TXTextureType = "27";

                        //Little Endian Binary. Gotta love it.
                        this.TempTexData = new List<byte>();
                        Byte[] TexHeader27 = { 0x54, 0x45, 0x58, 0x00, 0x9D, 0xA0, 0x00, 0x20 };
                        this.TempTexData.AddRange(TexHeader27);
                        string WidthTemp27 = Convert.ToString(this.TXx, 2);
                        WidthTemp27 = WidthTemp27.Substring(0, WidthTemp27.Length - 2);
                        int wt27 = 11 - WidthTemp27.Length;
                        if (wt27 > 0)
                        {
                            WidthTemp27 = WidthTemp27.PadLeft(11, '0');
                        }

                        string LengthTemp27 = Convert.ToString(this.TXy, 2);
                        int lt27 = 13 - LengthTemp27.Length;
                        if (lt27 > 0)
                        {
                            LengthTemp27 = LengthTemp27.PadLeft(13, '0');
                        }

                        string WidthTA27 = WidthTemp27.Substring(WidthTemp27.Length - 8);
                        string WidthTB27 = WidthTemp27.Substring(0, WidthTemp27.Length - 8);

                        string LengthTA27 = LengthTemp27.Substring(LengthTemp27.Length - 5);
                        string LengthTB27 = LengthTemp27.Substring(0, LengthTemp27.Length - 5);

                        string Byte227 = WidthTA27;
                        string Byte327 = LengthTA27 + WidthTB27;
                        string Byte427 = LengthTB27;

                        byte[] B227 = BSWConverter.BinaryToByteArray(Byte227);
                        byte[] B327 = BSWConverter.BinaryToByteArray(Byte327);
                        byte[] B427 = BSWConverter.BinaryToByteArray(Byte427);

                        TempTexData.Add(Convert.ToByte(this.TXmips));
                        TempTexData.AddRange(B227);
                        TempTexData.AddRange(B327);
                        TempTexData.AddRange(B427);
                        TempTexData.Add(0x01);
                        TempTexData.Add(0x27);
                        TempTexData.Add(0x01);
                        TempTexData.Add(0x00);

                        int MpMpTest27 = Math.Max(1, (this.TXx + 3) / 4) * Math.Max(1, (this.TXy + 3) / 4) * 16;

                        //Allocating room for Mip Offsets and the start of the MipMapData by calculating the size of each mip map and offsets for addresses.
                        int MippMapPixelSizeThingDXT127 = 0;
                        int MipOffset27 = 0;
                        int OffTemp27 = 0;
                        int DDSOffset27 = 128;
                        for (int p = 0; p < this.TXmips; p++)
                        {
                            MipOffset27 = 16 + (8 * p);
                            this.MMOffsets.Add(MipOffset27);
                            MippMapPixelSizeThingDXT127 = Math.Max(1, ((this.TXx / (Convert.ToInt32(Math.Pow(2, p)))) + 3) / 4) * Math.Max(1, ((this.TXy / (Convert.ToInt32(Math.Pow(2, p)))) + 3) / 4) * 16;
                            this.MMipSizes.Add(MippMapPixelSizeThingDXT127);
                        }

                        byte[][] MipMaps27 = new byte[this.TXmips][];
                        //Reads and extracts from the DDS file stored in memory.
                        for (int q = 0; q < this.TXmips; q++)
                        {

                            DDTemp = new byte[this.MMipSizes[q]];

                            XDTemp = new byte[(this.TXmips)];

                            Buffer.BlockCopy(this.DDSData, DDSOffset27, DDTemp, 0, (this.MMipSizes[q]));
                            DDSOffset27 = DDSOffset27 + DDTemp.Length;

                            MipMaps27[q] = DDTemp;

                        }

                        this.FirstMip = MipMaps27[0];

                        OffTemp27 = 16 + (8 * this.TXmips);
                        byte[] FillerBytes27 = { 0x00, 0x00, 0x00, 0x00 };

                        //Finishes the tex header by putting in the offsets as double words.
                        for (int r = 0; r < this.TXmips; r++)
                        {
                            byte[] OTemp = BitConverter.GetBytes(OffTemp27);
                            //Array.Reverse(OTemp);
                            this.TempTexData.AddRange(OTemp);
                            OffTemp27 = OffTemp27 + MipMaps27[r].Length;
                            this.TempTexData.AddRange(FillerBytes27);
                        }

                        //Now the MipMaps go in the AFTER Tex header.
                        for (int s = 0; s < this.TXmips; s++)
                        {
                            this.TempTexData.AddRange(MipMaps27[s]);
                        }

                        this.TexData = this.TempTexData.ToArray();

                        break;
                    }

                case 5:
                    if (this.IsDXT1 == true)
                    {
                        MessageBox.Show("This texture was saved as a DXT1 .DDS file, meaning the DXT compression method used for this is not compatible with this texture type.\nIf you want to import this as this type of texture, return to the image editing software you used to save this and make sure to save it as a DXT5 instead.", "Hold it!");
                        cmBoxTextureType.SelectedIndex = 0;
                        return;
                    }
                    else
                    {
                        this.TXTextureType = "2A";

                        //Little Endian Binary. Gotta love it.
                        this.TempTexData = new List<byte>();
                        Byte[] TexHeader2A = { 0x54, 0x45, 0x58, 0x00, 0x9D, 0xA0, 0x00, 0x20 };
                        this.TempTexData.AddRange(TexHeader2A);
                        string WidthTemp2A = Convert.ToString(this.TXx, 2);
                        WidthTemp2A = WidthTemp2A.Substring(0, WidthTemp2A.Length - 2);
                        int wt2A = 11 - WidthTemp2A.Length;
                        if (wt2A > 0)
                        {
                            WidthTemp2A = WidthTemp2A.PadLeft(11, '0');
                        }

                        string LengthTemp2A = Convert.ToString(this.TXy, 2);
                        int lt2A = 13 - LengthTemp2A.Length;
                        if (lt2A > 0)
                        {
                            LengthTemp2A = LengthTemp2A.PadLeft(13, '0');
                        }

                        string WidthTA2A = WidthTemp2A.Substring(WidthTemp2A.Length - 8);
                        string WidthTB2A = WidthTemp2A.Substring(0, WidthTemp2A.Length - 8);

                        string LengthTA2A = LengthTemp2A.Substring(LengthTemp2A.Length - 5);
                        string LengthTB2A = LengthTemp2A.Substring(0, LengthTemp2A.Length - 5);

                        string Byte22A = WidthTA2A;
                        string Byte32A = LengthTA2A + WidthTB2A;
                        string Byte42A = LengthTB2A;

                        byte[] B22A = BSWConverter.BinaryToByteArray(Byte22A);
                        byte[] B32A = BSWConverter.BinaryToByteArray(Byte32A);
                        byte[] B42A = BSWConverter.BinaryToByteArray(Byte42A);

                        TempTexData.Add(Convert.ToByte(this.TXmips));
                        TempTexData.AddRange(B22A);
                        TempTexData.AddRange(B32A);
                        TempTexData.AddRange(B42A);
                        TempTexData.Add(0x01);
                        TempTexData.Add(0x2A);
                        TempTexData.Add(0x01);
                        TempTexData.Add(0x00);

                        int MpMpTest2A = Math.Max(1, (this.TXx + 3) / 4) * Math.Max(1, (this.TXy + 3) / 4) * 16;

                        //Allocating room for Mip Offsets and the start of the MipMapData by calculating the size of each mip map and offsets for addresses.
                        int MippMapPixelSizeThingDXT12A = 0;
                        int MipOffset2A = 0;
                        int OffTemp2A = 0;
                        int DDSOffset2A = 128;
                        for (int p = 0; p < this.TXmips; p++)
                        {
                            MipOffset2A = 16 + (8 * p);
                            this.MMOffsets.Add(MipOffset2A);
                            MippMapPixelSizeThingDXT12A = Math.Max(1, ((this.TXx / (Convert.ToInt32(Math.Pow(2, p)))) + 3) / 4) * Math.Max(1, ((this.TXy / (Convert.ToInt32(Math.Pow(2, p)))) + 3) / 4) * 16;
                            this.MMipSizes.Add(MippMapPixelSizeThingDXT12A);
                        }

                        byte[][] MipMaps2A = new byte[this.TXmips][];
                        //Reads and extracts from the DDS file stored in memory.
                        for (int q = 0; q < this.TXmips; q++)
                        {

                            DDTemp = new byte[this.MMipSizes[q]];

                            XDTemp = new byte[(this.TXmips)];

                            Buffer.BlockCopy(this.DDSData, DDSOffset2A, DDTemp, 0, (this.MMipSizes[q]));
                            DDSOffset2A = DDSOffset2A + DDTemp.Length;

                            MipMaps2A[q] = DDTemp;

                        }

                        this.FirstMip = MipMaps2A[0];

                        OffTemp2A = 16 + (8 * this.TXmips);
                        byte[] FillerBytes2A = { 0x00, 0x00, 0x00, 0x00 };

                        //Finishes the tex header by putting in the offsets as double words.
                        for (int r = 0; r < this.TXmips; r++)
                        {
                            byte[] OTemp = BitConverter.GetBytes(OffTemp2A);
                            //Array.Reverse(OTemp);
                            this.TempTexData.AddRange(OTemp);
                            OffTemp2A = OffTemp2A + MipMaps2A[r].Length;
                            this.TempTexData.AddRange(FillerBytes2A);
                        }

                        //Now the MipMaps go in the AFTER Tex header.
                        for (int s = 0; s < this.TXmips; s++)
                        {
                            this.TempTexData.AddRange(MipMaps2A[s]);
                        }

                        this.TexData = this.TempTexData.ToArray();

                        break;
                    }

                default:
                    this.TXTextureType = "13";
                    break;

            }
        }

        private void btnInvertGreen_Click(object sender, EventArgs e)
        {
            //Inverts Green Channel on Preview. Only works with DXT5/BC3 images.
            if (DDSData == null)
            {
                MessageBox.Show("This isn't supposed to be empty...", "Uhhhh");
                return;
            }
            if (this.IsDXT1 == true)
            {
                MessageBox.Show("This feature supports Only DXT5 Textures.", "Hmmmm");
                return;
            }

            Bitmap pic = new Bitmap(PicBoxTex.Image);
            for (int y = 0; (y <= (pic.Height - 1)); y++)
            {
                for (int x = 0; (x <= (pic.Width - 1)); x++)
                {
                    Color inv = pic.GetPixel(x, y);
                    inv = Color.FromArgb(inv.A, (inv.R), (255 - inv.G), (inv.B));
                    pic.SetPixel(x, y, inv);
                }
            }
            PicBoxTex.Image = pic;

            //Now for the DDS Data itself.
            int max = (DDSData.Length - 128) / 16;
            using (MemoryStream TexStream = new MemoryStream(DDSData))
            {
                using (BinaryWriter bwStream = new BinaryWriter(TexStream))
                {
                    bwStream.BaseStream.Position = 128;

                    for (int w = 0; w > max; w++)
                    {

                    }
                }
            }




        }

        private void btnRedAlphaSwap_Click(object sender, EventArgs e)
        {

            //Swaps Red and Alpha channels on Preview. Only works with DXT5/BC3 images.
            if (DDSData == null)
            {
                MessageBox.Show("This isn't supposed to be empty.", "Uhhhh");
                return;
            }
            if (this.IsDXT1 == true)
            {
                MessageBox.Show("This feature supports Only DXT5 Textures.", "Hmmmm");
                return;
            }

            Bitmap pic = new Bitmap(PicBoxTex.Image);
            for (int y = 0; (y <= (pic.Height - 1)); y++)
            {
                for (int x = 0; (x <= (pic.Width - 1)); x++)
                {
                    Color inv = pic.GetPixel(x, y);
                    inv = Color.FromArgb(inv.R, (inv.A), (255 - inv.G), (inv.B));
                    pic.SetPixel(x, y, inv);
                }
            }
            PicBoxTex.Image = pic;


        }
    }

}
