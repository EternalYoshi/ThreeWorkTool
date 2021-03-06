using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeWorkTool.Resources.Archives;
using System.IO;
using System.Windows.Forms;

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

    }
}
