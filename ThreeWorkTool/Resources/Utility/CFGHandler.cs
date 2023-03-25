using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zlib;
using System.IO;
using System.Runtime.InteropServices;
using System.Numerics;
using System.Windows.Forms;
using System.Diagnostics;
using ThreeWorkTool.Resources.Archives;

namespace ThreeWorkTool.Resources.Utility
{
    public class CFGHandler
    {
        public static StringBuilder SBname;

        public static string ArchiveHashToName(string str, string Typehash)
        {

            //Looks through the archive_filetypes.cfg file to find the extension associated with the typehash.
            try
            {
                //Gets the Corrected path for the cfg.
                string ProperPath = "";
                ProperPath = Globals.ToolPath + "archive_filetypes.cfg";
                using (var sr = new StreamReader(ProperPath))
                {
                    while (!sr.EndOfStream)
                    {
                        var keyword = Console.ReadLine() ?? Typehash;
                        var line = sr.ReadLine();
                        if (String.IsNullOrEmpty(line)) continue;
                        if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                        {
                            str = line;
                            str = str.Split(' ')[2];
                            break;
                        }
                    }
                }

            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("I cannot find archive_filetypes.cfg so I cannot finish parsing this file.", "Oh Boy");

                string ProperPath = "";
                ProperPath = Globals.ToolPath + "Log.txt";
                using (StreamWriter sw = File.AppendText(ProperPath))
                {
                    sw.WriteLine("Cannot find archive_filetypes.cfg so I cannot continue parsing this file.\n Find archive_filetypes.cfg and then restart this program.");
                    Process.GetCurrentProcess().Kill();
                }
                return null;
            }



            return str;
        }

        public static string ArchiveHashToExtension(string str, string Typehash)
        {

            //Looks through the archive_filetypes.cfg file to find the extension associated with the typehash.
            try
            {
                //Gets the Corrected path for the cfg.
                string ProperPath = "";
                ProperPath = Globals.ToolPath + "archive_filetypes.cfg";
                using (var sr = new StreamReader(ProperPath))
                {
                    while (!sr.EndOfStream)
                    {
                        var keyword = Console.ReadLine() ?? Typehash;
                        var line = sr.ReadLine();
                        if (String.IsNullOrEmpty(line)) continue;
                        if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                        {
                            str = line;
                            str = str.Split(' ')[1];
                            break;
                        }
                    }
                }

            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("I cannot find archive_filetypes.cfg so I cannot finish parsing this file.", "Oh Boy");
                string ProperPath = "";
                ProperPath = Globals.ToolPath + "Log.txt";
                using (StreamWriter sw = File.AppendText(ProperPath))
                {
                    sw.WriteLine("Cannot find archive_filetypes.cfg so I cannot continue parsing this file.\n Find archive_filetypes.cfg and then restart this program.");
                    Process.GetCurrentProcess().Kill();
                }
                return null;
            }



            return str;
        }

        public static string Logger(string str, string filename, string sentence)
        {
            //Gets the Corrected path for the cfg.
            string ProperPath = "";
            ProperPath = Globals.ToolPath + "Log.txt";


            return sentence;
        }

        public static string ShaderHashToName(string str, int Index)
        {
            string line = "";
            try
            {
                //Gets the Corrected path for the cfg.
                string ProperPath = "";
                ProperPath = Globals.ToolPath +"mvc3shadertypes.cfg";
                line = File.ReadLines(ProperPath).Skip(Index).Take(1).First();
            }
            catch (Exception xx)
            {
                MessageBox.Show("mvc3shadertypes.cfg is missing or not read. Can't continue parsing materials.\n" + xx, "Uh-Oh");
                return null;
            }
            str = line;



            return str;
        }

        //Looks through the cfg file to find the Typehash and returns it.
        public static string TypeHashFinder(string hashtext)
        {
            string TypeHash = "";

            if (hashtext.Length == 9)
            {
                TypeHash = hashtext;
                TypeHash = TypeHash.Substring(1);
                if (System.Text.RegularExpressions.Regex.IsMatch(TypeHash, @"\A\b[0-9A-F]+\b\Z") == true)
                {
                    return TypeHash;
                }
            }

            //Gets the Corrected path for the cfg.
            string ProperPath = "";
            ProperPath = Globals.ToolPath + "archive_filetypes.cfg";
            //Looks through the archive_filetypes.cfg file to find the typehash associated with the extension.
            try
            {
                using (var sr2 = new StreamReader(ProperPath))
                {
                    while (!sr2.EndOfStream)
                    {
                        var keyword = Console.ReadLine() ?? hashtext;
                        var line = sr2.ReadLine();
                        if (String.IsNullOrEmpty(line)) continue;
                        if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                        {
                            TypeHash = line;
                            TypeHash = TypeHash.Split(' ')[0];

                            break;
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Cannot find archive_filetypes.cfg so I cannot continue parsing this file.\n Find archive_filetypes.cfg and then restart this program.", " ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Process.GetCurrentProcess().Kill();
            }

            return TypeHash;
        }

        //Creates an older version of the .cfg file.
        public static void MakeTheCFG()
        {
            string CFGBackup = "";


        }

    }
}
