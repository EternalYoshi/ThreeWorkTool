namespace ThreeWorkTool.Resources
{
    public class ExportFilters
    {

        public string result;

        public static string rTexture = "Raw Texture File (*.tex)|*.tex|DirectDraw Surface Image(*.dds)| *.dds|Portable Network Graphics (*.png)|*.png";

        public static string TexImport = "Supported files(*.tex;*.dds)|*.tex;*.dds|Raw Texture File(*.tex)|*.tex|DirectDraw Surface Image(*.dds)| *.dds";

        public static string rResourcePathList = "Resources Path List (*.lrp)|*.lrp";

        public static string rMaterial = "Material File (*.mrl)|*.mrl";

        public static string rMotionList = "MotionList (*.lmt)|*.lmt";

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

                case ".mrl":
                    return rMaterial;

                case ".lmt":
                    return rMotionList;

                default:
                    return Etc; 
            }

        }


    }
}
