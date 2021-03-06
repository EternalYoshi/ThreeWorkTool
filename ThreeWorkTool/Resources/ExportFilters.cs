using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeWorkTool.Resources
{
    public class ExportFilters
    {

        public string result;

        public static string rTexture = "Raw Texture File (*.tex)|*.tex" + "DirectDraw Surface Image(*.dds)| *.dds";

        public static string Etc = "All Files (*.*)|*.*";
        
        public static string GetFilter(string filetype)
        {

            switch (filetype)
            {
                case ".tex":
                    return rTexture;



                default:
                    return Etc; 
            }
        }


    }
}
