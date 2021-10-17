using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Archives;
using ThreeWorkTool.Resources.Wrappers;

namespace ThreeWorkTool.Resources
{
    public static class ExportFileWriter
    {

        //public StreamWriter sw;

        public static void ArcEntryWriter(string filename, ArcEntry entrytowrite)
        {
            try
            {

                using (BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Create)))
                {
                    bw.Write(entrytowrite.UncompressedData);
                    bw.Close();
                }

            }                                
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Unable to access the file. Maybe it's already in use by another proccess?", "Cannot write this file.");
                return;
            }
        }

        public static void MSDEntryWriter(string filename, MSDEntry entrytowrite)
        {
            try
            {

                using (BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Create)))
                {
                    bw.Write(entrytowrite.UncompressedData);
                    bw.Close();
                }

            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Unable to access the file. Maybe it's already in use by another proccess?", "Cannot write this file.");
                return;
            }
        }

        public static void RPListEntryWriter(string filename, ResourcePathListEntry entrytowrite)
        {
            try
            {
                
                using (BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Create)))
                {
                    bw.Write(entrytowrite.UncompressedData);
                    bw.Close();
                }

            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Unable to access the file. Maybe it's already in use by another proccess?", "Cannot write this file.");
                return;
            }
        }

        public static void LMTEntryWriter(string filename, LMTEntry entrytowrite)
        {
            try
            {

                using (BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Create)))
                {
                    bw.Write(entrytowrite.UncompressedData);
                    bw.Close();
                }

            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Unable to access the file. Maybe it's already in use by another proccess?", "Cannot write this file.");
                return;
            }
        }

        public static void MA3EntryWriter(string filename, LMTM3AEntry entrytowrite)
        {
            try
            {

                using (BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Create)))
                {
                    bw.Write(entrytowrite.RawData);
                    bw.Close();
                }

            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Unable to access the file. Maybe it's already in use by another proccess?", "Cannot write this file.");
                return;
            }
        }

        public static void TexEntryWriter(string filename, TextureEntry entrytowrite)
        {

            try
            {
                Stream strim = new MemoryStream(entrytowrite.OutMaps);
                //From the pfim website. Modified for my uses.
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
                        string ext = Path.GetExtension(filename);

                        if(ext == ".png")
                        {
                            bitmap.Save(Path.ChangeExtension(filename, ".png"), System.Drawing.Imaging.ImageFormat.Png);
                        }
                        else if(ext == ".dds")
                        {
                            using (BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Create)))
                            {
                                bw.Write(entrytowrite.OutMaps);
                                bw.Close();
                            }
                        }
                        else if(ext == ".tex")
                        {
                            using (BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Create)))
                            {
                                bw.Write(entrytowrite.UncompressedData);
                                bw.Close();
                            }
                        }

                    }
                    finally
                    {
                        handle.Free();
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Unable to access the file. Maybe it's already in use by another proccess?", "Cannot write this file.");
                return;
            }

        }

        public static void MaterialEntryWriter(string filename, MaterialEntry entrytowrite)
        {
            try
            {

                using (BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Create)))
                {
                    bw.Write(entrytowrite.UncompressedData);
                    bw.Close();
                }
                
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Unable to access the file. Maybe it's already in use by another proccess?", "Cannot write this file.");
                return;
            }
        }


    }
}
