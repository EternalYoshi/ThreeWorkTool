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

        public static string rTexture = "Raw Texture File (*.tex)|*.tex|DirectDraw Surface Image(*.dds)| *.dds|Portable Network Graphics (*.png)|*.png";

        public static string TexImport = "Raw Texture File(*.tex)|*.tex|DirectDraw Surface Image(*.dds)| *.dds";

        public static string rResourcePathList = "Resources Path List (*.lrp*)|*.lrp*";
        

        public static string Etc = "All Files (*.*)|*.*";
        
        public static string GetFilter(string filetype)
        {

            switch (filetype)
            {
                case ".tex":
                    return rTexture;

                case "ReplaceTexture":
                    return TexImport;

                case ".lrp":
                    return rResourcePathList;

                default:
                    return Etc; 
            }
        }


    }
}
