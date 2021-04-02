using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        public int TXPixelFormat;
        public string TXTextureType;
        public string[] TXTxTypeDescriptions;
        public Bitmap TXpreview;
        

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

                        fted.DDSData = File.ReadAllBytes(openedfile);

                        Stream ztrim = new MemoryStream(fted.DDSData);

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
            catch (Exception)
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
            switch (cmBoxTextureType.SelectedIndex)
            {

                case 0:
                    this.TXTextureType = "13";
                    break;

                case 1:
                    this.TXTextureType = "17";
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
