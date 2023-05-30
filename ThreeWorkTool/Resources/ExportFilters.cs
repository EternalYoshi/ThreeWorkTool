namespace ThreeWorkTool.Resources
{
    public class ExportFilters
    {

        public string result;

        public static string rTexture = "Raw Texture File (*.tex)|*.tex|DirectDraw Surface Image(*.dds)| *.dds|Portable Network Graphics (*.png)|*.png";

        public static string TexImport = "Supported files(*.tex;*.dds)|*.tex;*.dds|Raw Texture File(*.tex)|*.tex|DirectDraw Surface Image(*.dds)| *.dds";

        public static string rResourcePathList = "Resources Path List (*.lrp)|*.lrp";

        public static string rMaterial = "MT Material File(*.mrl;*.yml)|*.mrl;*.yml|Material File (*.mrl)|*.mrl|YAML MT Material File(*.yml)| *.yml";

        public static string rMotionList = "MotionList (*.lmt)|*.lmt";

        public static string rM3A = "Raw MT Animation(*.m3a)|*.m3a";

        public static string Etc = "All Files (*.*)|*.*";

        public static string rChainSetup = "MT Physics Setup File (*.cst)|*.cst";

        public static string rChain = "MT Physics File (*.chn)|*.chn";

        public static string rChainCollision = "MT Physics Collision File (*.ccl)|*.ccl";

        public static string rModel = "MT Framework Model File (*.mod)|*.mod";

        public static string rMission = "MT Mission File (*.mis)|*.mis";

        public static string rGameEffectModel = "Game Effect Model File (*.gem)|*.gem";

        public static string rEffectList = "Effect List (*.efl)|*.efl";

        public static string KeyFrameTest = "(*.txt)|.txt";

        public static string rShotList = "Shot List (*.lsh)|*.lsh|Shot List(*.141D851F)|*.141D851F";

        public static string rAnmCmd = "Action Script (*.anm)|*.anm|Action Script (*.5A7E5D8A)|*.5A7E5D8A";

        public static string rAtkInfo = "Moveset File (*.ati)|*.ati|Moveset File (*.227A8048)|*.227A8048";

        public static string rChrBaseact = "Moveset File (*.cba)|*.cba|Moveset File (*.3C6EA504)|*.3C6EA504";

        public static string rChrColList = "Moveset File (*.cli)|*.cli|Moveset File (*.5B486CCE)|*.5B486CCE";

        public static string rChrCpu = "Moveset File (*.cpu)|*.cpu|Moveset File (*.05500206)|*.05500206";

        public static string rChrProfile = "Moveset File (*.cpi)|*.cpi|Moveset File (*.1DF3E03E)|*.1DF3E03E";

        public static string rChrStatus = "Moveset File (*.chs)|*.chs|Moveset File (*.3C41466B)|*.3C41466B";

        public static string rChrCombo = "Moveset File (*.ccm)|*.ccm|Moveset File (*.28DD8317)|*.28DD8317";

        public static string rChrSpatk = "Moveset File (*.csp)|*.csp|Moveset File (*.52A8DBF6)|*.52A8DBF6";

        public static string rShot = "Projectile Shot File(*.sht)|*.sht|Projectile Shot File (*.10BE43D4)|*.10BE43D4";

        public static string rStgObjLayout = "Stage Object Layout (*.slo)|*.slo";

        public static string rSoundStreamRequest = "Sound Stream Request File (*.stqr)|*.stqr|Sound Stream Request File (*.167DBBFF)|*.167DBBFF";

        public static string rSoundBank = "Sound Bank File (*.sbkr)|*.sbkr|Sound Bank File (*.15D782FB)|*.15D782FB";

        public static string rSoundRequest = "Sound Bank File (*.srqr)|*.srqr|Sound Bank File (*.1BCC4966)|*.1BCC4966";

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

                case ".m3a":
                    return rM3A;

                case ".cst":
                    return rChainSetup;

                case ".chn":
                    return rChain;

                case ".ccl":
                    return rChainCollision;

                case ".mod":
                    return rModel;

                case ".mis": 
                    return rMission;

                case ".gem":
                    return rGameEffectModel;

                case ".efl":
                    return rEffectList;

                case ".lsh":
                case ".141D851F":
                    return rShotList;

                case ".anm":
                case ".5A7E5D8A":
                    return rAnmCmd;

                case ".ati":
                case ".227A8048":
                    return rAtkInfo;

                case ".cba":
                case ".3C6EA504":
                    return rChrBaseact;

                case ".cli":
                case ".5B486CCE":
                    return rChrColList;

                case ".cpu":
                case ".05500206":
                    return rChrCpu;

                case ".cpi":
                case ".1DF3E03E":
                    return rChrProfile;

                case ".chs":
                case ".3C41466B":
                    return rChrStatus;

                case ".ccm":
                case ".28DD8317":
                    return rChrCombo;

                case ".csp":
                case ".52A8DBF6":
                    return rChrSpatk;

                case ".sht":
                case ".10BE43D4":
                    return rShot;

                case ".slo":
                    return rStgObjLayout;

                case ".stqr":
                case ".167DBBFF":
                    return rSoundStreamRequest;

                case ".sbkr":
                case ".15D782FB":
                    return rSoundBank;

                case ".srqr":
                case ".1BCC4966":
                    return rSoundRequest;

                default:
                    return Etc; 
            }

        }


    }
}
