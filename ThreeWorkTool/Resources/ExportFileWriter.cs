using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Archives;
using ThreeWorkTool.Resources.Wrappers;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ThreeWorkTool.Resources
{
    public static class ExportFileWriter
    {

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
            catch (Exception ex)
            {
                ExceptionCatchAll(ex);
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
            catch (Exception ex)
            {
                ExceptionCatchAll(ex);
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
            catch (Exception ex)
            {
                ExceptionCatchAll(ex);
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
            catch (Exception ex)
            {
                ExceptionCatchAll(ex);
                return;
            }
        }

        public static void MA3EntryWriter(string filename, LMTM3AEntry entrytowrite)
        {
            try
            {

                using (BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Create)))
                {
                    bw.Write(entrytowrite.FullData);
                    bw.Close();
                }

            }
            catch (Exception ex)
            {
                ExceptionCatchAll(ex);
                return;
            }
        }

        public static void TexEntryWriter(string filename, TextureEntry entrytowrite)
        {
            if (entrytowrite.IsCubeMap == true)
            {
                try
                {
                    MessageBox.Show("CubeMaps aren't currently supported, so only the raw .tex file can be extracted.");
                    using (BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Create)))
                    {
                        bw.Write(entrytowrite.UncompressedData);
                        bw.Close();
                    }

                }
                catch (Exception ex)
                {
                    ExceptionCatchAll(ex);
                    return;
                }

            }
            else if (entrytowrite.TexType == "15" || entrytowrite.Format == "??????")
            {
                try
                {
                    MessageBox.Show("This undocumented texture type isn't supported for conversion, so only the raw .tex file can be extracted.");
                    using (BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Create)))
                    {
                        bw.Write(entrytowrite.UncompressedData);
                        bw.Close();
                    }

                }
                catch (Exception ex)
                {
                    ExceptionCatchAll(ex);
                    return;
                }

            }
            else
            {
                try
                {

                    Stream strim = new MemoryStream(entrytowrite.OutTar);
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

                            if (ext == ".png")
                            {
                                bitmap.Save(Path.ChangeExtension(filename, ".png"), System.Drawing.Imaging.ImageFormat.Png);
                            }
                            else if (ext == ".dds")
                            {
                                using (BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Create)))
                                {
                                    bw.Write(entrytowrite.OutTar);
                                    bw.Close();
                                }
                            }
                            else if (ext == ".tex")
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
                catch (Exception ex)
                {


                    ExceptionCatchAll(ex);
                    return;
                }
            }
        }

        public static void MaterialEntryWriter(string filename, MaterialEntry entrytowrite)
        {
            try
            {
                string ext = Path.GetExtension(filename);
                if (ext == ".mrl")
                {
                    using (BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Create)))
                    {

                        bw.Write(entrytowrite.UncompressedData);
                        bw.Close();

                    }
                }
                else if (ext == ".yml")
                {

                    using (StreamWriter sw = new StreamWriter(File.Open(filename, FileMode.Create)))
                    {

                        sw.Write(entrytowrite.YMLText);
                        sw.Close();

                    }

                }

            }
            catch (Exception ex)
            {
                ExceptionCatchAll(ex);
                return;
            }



        }

        public static void ChainListEntryWriter(string filename, ChainListEntry entrytowrite)
        {
            try
            {

                using (BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Create)))
                {
                    bw.Write(entrytowrite.UncompressedData);
                    bw.Close();
                }

            }
            catch (Exception ex)
            {
                ExceptionCatchAll(ex);
                return;
            }
        }

        public static void ChainEntryWriter(string filename, ChainEntry entrytowrite)
        {
            try
            {

                using (BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Create)))
                {
                    bw.Write(entrytowrite.UncompressedData);
                    bw.Close();
                }

            }
            catch (Exception ex)
            {
                ExceptionCatchAll(ex);
                return;
            }
        }

        public static void ChainCollisionEntryWriter(string filename, ChainCollisionEntry entrytowrite)
        {
            try
            {

                using (BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Create)))
                {
                    bw.Write(entrytowrite.UncompressedData);
                    bw.Close();
                }

            }
            catch (Exception ex)
            {
                ExceptionCatchAll(ex);
                return;
            }
        }

        public static void ModelEntryWriter(string filename, ModelEntry entrytowrite)
        {
            try
            {

                using (BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Create)))
                {
                    bw.Write(entrytowrite.UncompressedData);
                    bw.Close();
                }

            }
            catch (Exception ex)
            {
                ExceptionCatchAll(ex);
                return;
            }
        }

        public static void KeyFrameWriter(string filename, LMTM3AEntry entrytowrite)
        {

            string ext = Path.GetExtension(filename);
            switch (ext)
            {
                case ".yml":
                    try
                    {

                        //Time to start getting the data from the M3A Entry. For Testing Purposes.
                        using (StreamWriter sw = new StreamWriter(File.Open(filename, FileMode.Create)))
                        {

                            var serializer = new SerializerBuilder().DisableAliases().EnsureRoundtrip().WithTagMapping("!LMTM3AEntry", typeof(ThreeWorkTool.Resources.Wrappers.LMTM3AEntry)).Build();

                            // Save Changes
                            serializer.Serialize(sw, entrytowrite);

                        }

                    }
                    catch (Exception ex)
                    {
                        ExceptionCatchAll(ex);
                        return;
                    }
                    break;

                case ".anim":
                    try
                    {

                        //Prepares the Keyframes.

                    }
                    catch (Exception ex)
                    {
                        ExceptionCatchAll(ex);
                        return;
                    }

                    break;


            }

        }

        public static void MissionWriter(string filename, MissionEntry entrytowrite)
        {
            try
            {

                using (BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Create)))
                {
                    bw.Write(entrytowrite.UncompressedData);
                    bw.Close();
                }

            }
            catch (Exception ex)
            {
                ExceptionCatchAll(ex);
                return;
            }
        }

        public static void GemWriter(string filename, GemEntry entrytowrite)
        {
            try
            {

                using (BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Create)))
                {
                    bw.Write(entrytowrite.UncompressedData);
                    bw.Close();
                }

            }
            catch (Exception ex)
            {
                ExceptionCatchAll(ex);
                return;
            }
        }

        public static void EffectListWriter(string filename, EffectListEntry entrytowrite)
        {
            try
            {

                using (BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Create)))
                {
                    bw.Write(entrytowrite.UncompressedData);
                    bw.Close();
                }

            }
            catch (Exception ex)
            {
                ExceptionCatchAll(ex);
                return;
            }
        }

        public static void RIFFWriter(string filename, RIFFEntry entrytowrite)
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

        public static void ShotListWriter(string filename, ShotListEntry entrytowrite)
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

        public static void StageOBJLayoutWriter(string filename, StageObjLayoutEntry entrytowrite)
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

        public static void STQRWriter(string filename, STQREntry entrytowrite)
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

        public static void AtkInfoWriter(string filename, AtkInfoEntry entrytowrite)
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

        public static void AnmCmdWriter(string filename, AnmCmdEntry entrytowrite)
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

        public static void ChrBaseActWriter(string filename, ChrBaseActEntry entrytowrite)
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

        public static void ShotWriter(string filename, ShotEntry entrytowrite)
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

        public static void SoundBankWriter(string filename, SoundBankEntry entrytowrite)
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

        public static void SoundRequestWriter(string filename, SoundRequestEntry entrytowrite)
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

        public static void MiscFileWriter(string filename, byte[] entrytowrite)
        {
            try
            {

                using (BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Create)))
                {
                    bw.Write(entrytowrite);
                    bw.Close();
                }

            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Unable to access the file. Maybe it's already in use by another proccess?", "Cannot write this file.");
                return;
            }
        }

        public static void ExceptionCatchAll(Exception ex)
        {
            if (ex is UnauthorizedAccessException)
            {
                MessageBox.Show("Unable to access the file. Maybe it's already in use by another proccess or in a directory that requires administrative rights??", "Cannot write this file.");
                return;
            }
            else if (ex is IOException)
            {
                MessageBox.Show("The export failed because the chosen file is already in use by another proccess.");

                string ProperPath = "";
                ProperPath = Globals.ToolPath + "Log.txt";
                using (StreamWriter sw = File.AppendText(ProperPath))
                {
                    sw.WriteLine("Export failed!\n");
                    sw.WriteLine("Exception info:" + ex);
                    sw.WriteLine("===============================================================================================================");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Unable to complete the operation. Here's details: \n" + ex, "Cannot write this file.");
                return;
            }

        }

        #region New Entries
        //New Entries Go like this!
        /*
         
        public static void *****Writer(string filename, ***** entrytowrite)
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

        */
        #endregion

    }
}
