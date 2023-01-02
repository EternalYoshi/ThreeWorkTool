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
                using (var sr = new StreamReader("archive_filetypes.cfg"))
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
                using (StreamWriter sw = File.AppendText("Log.txt"))
                {
                    sw.WriteLine("Cannot find archive_filetypes.cfg so I cannot continue parsing this file.\n Find archive_filetypes.cfg and then restart this program.");
                    Process.GetCurrentProcess().Kill();
                }
                return null;
            }



            return str;
        }

        public static string ShaderHashToName(string str, int Index)
        {
            string line = "";
            try
            {
                line = File.ReadLines("mvc3shadertypes.cfg").Skip(Index).Take(1).First();
            }
            catch (Exception xx)
            {
                MessageBox.Show("mvc3shadertypes.cfg is missing or not read. Can't continue parsing materials.\n" + xx, "Uh-Oh");
                return null;
            }
            str = line;



            return str;
        }

        //Creates an older version of the .cfg file.
        public static void MakeTheCFG()
        {
            string CFGBackup = "";


        }

    }
}
