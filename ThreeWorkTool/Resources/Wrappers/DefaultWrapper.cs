using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Utility;
using ThreeWorkTool.Resources.Wrappers;

namespace ThreeWorkTool.Resources.Archives
{
    public abstract class DefaultWrapper
    {
        public string EntryName;
        public string TypeHash;
        public string TempStr;
        public string FileExt;
        public string TrueName;
        public string TempFolder;
        public int AOffset;
        public int CSize;
        public int DSize;
        public int OffsetTemp;
        public byte[] TBFlag;
        public byte[] CompressedData;
        public byte[] UncompressedData;
        public static StringBuilder SBname;
        public string[] EntryDirs;
        public int EntryID;

        public static void FillEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, DefaultWrapper entrytobuild, Type filetype = null)
        {
            List<byte> BTemp = new List<byte>();

            //This block gets the name of the entry.
            entrytobuild.OffsetTemp = c;
            entrytobuild.EntryID = ID;
            br.BaseStream.Position = entrytobuild.OffsetTemp;
            var TempName = Encoding.ASCII.GetString(br.ReadBytes(64)).Trim('\0');

            //This is for the bytes that have the typehash, the thing that dictates the type of file stored.
            BTemp = new List<byte>();
            c = c + 64;
            br.BaseStream.Position = c;
            entrytobuild.TypeHash = ByteUtilitarian.BytesToStringL2R(br.ReadBytes(4).ToList(), entrytobuild.TypeHash);

            //Compressed Data size.
            entrytobuild.CSize = br.ReadInt32();

            //Uncompressed Data size.
            entrytobuild.DSize = br.ReadInt32() - 1073741824;

            //Data Offset.
            entrytobuild.AOffset = br.ReadInt32();

            //Compressed Data.
            BTemp = new List<byte>();
            br.BaseStream.Position = entrytobuild.AOffset;
            entrytobuild.CompressedData = br.ReadBytes(entrytobuild.CSize);

            //Namestuff.
            entrytobuild.EntryName = TempName;

            //Ensures existing subdirectories are cleared so the directories for files are displayed correctly.
            if (subnames != null)
            {
                if (subnames.Count > 0)
                {
                    subnames.Clear();
                }
            }

            //Gets the filename without subdirectories.
            if (entrytobuild.EntryName.Contains("\\"))
            {
                string[] splstr = entrytobuild.EntryName.Split('\\');

                //foreach (string v in splstr)
                for (int v = 0; v < (splstr.Length - 1); v++)
                {
                    if (!subnames.Contains(splstr[v]))
                    {
                        subnames.Add(splstr[v]);
                    }
                }


                entrytobuild.TrueName = entrytobuild.EntryName.Substring(entrytobuild.EntryName.IndexOf("\\") + 1);
                Array.Clear(splstr, 0, splstr.Length);

                while (entrytobuild.TrueName.Contains("\\"))
                {
                    entrytobuild.TrueName = entrytobuild.TrueName.Substring(entrytobuild.TrueName.IndexOf("\\") + 1);
                }
            }
            else
            {
                entrytobuild.TrueName = entrytobuild.EntryName;
            }


            entrytobuild.EntryDirs = subnames.ToArray();


            //Looks through the archive_filetypes.cfg file to find the extension associated with the typehash.
            try
            {
                using (var sr = new StreamReader("archive_filetypes.cfg"))
                {
                    while (!sr.EndOfStream)
                    {
                        var keyword = Console.ReadLine() ?? entrytobuild.TypeHash;
                        var line = sr.ReadLine();
                        if (String.IsNullOrEmpty(line)) continue;
                        if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                        {
                            entrytobuild.FileExt = line;
                            entrytobuild.FileExt = entrytobuild.FileExt.Split(' ')[1];
                            entrytobuild.EntryName = entrytobuild.EntryName + entrytobuild.FileExt;
                            break;
                        }
                    }
                }

            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("I cannot find archive_filetypes.cfg so I cannot finish parsing the arc.", "Oh Boy");
                using (StreamWriter sw = File.AppendText("Log.txt"))
                {
                    sw.WriteLine("Cannot find archive_filetypes.cfg so I cannot continue parsing the file.");
                }
            }

            //Decompression Time.
            entrytobuild.UncompressedData = ZlibStream.UncompressBuffer(entrytobuild.CompressedData);

        }

        public static void ReplaceEntry(TreeView tree, ArcEntryWrapper node, string filename, DefaultWrapper entrytobuild, DefaultWrapper entrytoreplace, Type filetype = null)
        {

            tree.BeginUpdate();

            try
            {
                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
                {
                    //We build the entrytobuild starting from the uncompressed data.
                    entrytobuild.UncompressedData = System.IO.File.ReadAllBytes(filename);

                    //Then Compress.
                    entrytobuild.CompressedData = Zlibber.Compressor(entrytobuild.UncompressedData);


                    //Gets the filename of the file to inject without the directory.
                    string trname = filename;
                    while (trname.Contains("\\"))
                    {
                        trname = trname.Substring(trname.IndexOf("\\") + 1);
                    }

                    //Enters name related parameters of the entrytobuild.
                    entrytobuild.TrueName = trname;
                    entrytobuild.TrueName = Path.GetFileNameWithoutExtension(trname);
                    entrytobuild.FileExt = trname.Substring(trname.LastIndexOf("."));

                    string TypeHash = "";

                    //Looks through the archive_filetypes.cfg file to find the typehash associated with the extension.
                    try
                    {
                        using (var sr2 = new StreamReader("archive_filetypes.cfg"))
                        {
                            while (!sr2.EndOfStream)
                            {
                                var keyword = Console.ReadLine() ?? entrytobuild.FileExt;
                                var line = sr2.ReadLine();
                                if (String.IsNullOrEmpty(line)) continue;
                                if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                                {
                                    TypeHash = line;
                                    TypeHash = TypeHash.Split(' ')[0];
                                    entrytobuild.TypeHash = TypeHash;
                                    break;
                                }
                            }
                        }
                    }
                    catch (FileNotFoundException)
                    {
                        MessageBox.Show("I cannot find and/or access archive_filetypes.cfg so I cannot finish parsing the arc.", "Oh Boy");
                        using (StreamWriter sw = File.AppendText("Log.txt"))
                        {
                            sw.WriteLine("Cannot find archive_filetypes.cfg so I cannot continue parsing the file.");
                        }
                        return;
                    }

                    var tag = node.Tag;
                    if (tag is DefaultWrapper)
                    {
                        entrytoreplace = tag as DefaultWrapper;
                    }

                    string path = "";
                    int index = entrytoreplace.EntryName.LastIndexOf("\\");
                    if (index > 0)
                    {
                        path = entrytoreplace.EntryName.Substring(0, index);
                    }

                    entrytobuild.EntryName = path + "\\" + entrytobuild.TrueName;

                    tag = entrytobuild;

                    if (node.Tag is DefaultWrapper)
                    {
                        node.Tag = entrytobuild;
                        node.Name = Path.GetFileNameWithoutExtension(entrytobuild.EntryName);
                        node.Text = Path.GetFileNameWithoutExtension(entrytobuild.EntryName);

                    }

                    var aew = node as ArcEntryWrapper;

                    string type = node.GetType().ToString();
                    if (type == "ThreeWorkTool.Resources.Wrappers.ArcEntryWrapper")
                    {
                        aew.entryfile = entrytobuild;
                    }

                    node = aew;
                    node.entryfile = entrytobuild;
                    /*
                    //ArcEntryWrapper aew = new ArcEntryWrapper();
                    if (node is ArcEntryWrapper)
                    {
                        node.entryfile as ArcEntryWrapper = node.Tag;
                    }
                    */
                    tree.EndUpdate();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Read error. Is the file readable?");
                using (StreamWriter sw = File.AppendText("Log.txt"))
                {
                    sw.WriteLine("Read Error! Here's the exception info:\n" + ex);
                }
            }



            //return node.entryfile as ArcEntry;
        }

        public static void InsertEntry(TreeView tree, ArcEntryWrapper node, string filename, DefaultWrapper entrytobuild, Type filetype = null)
        {

            try
            {
                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
                {
                    //We build the entrytobuild starting from the uncompressed data.
                    entrytobuild.UncompressedData = System.IO.File.ReadAllBytes(filename);
                    entrytobuild.DSize = entrytobuild.UncompressedData.Length;

                    //Then Compress.
                    entrytobuild.CompressedData = Zlibber.Compressor(entrytobuild.UncompressedData);
                    entrytobuild.CSize = entrytobuild.CompressedData.Length;

                    //Gets the filename of the file to inject without the directory.
                    string trname = filename;
                    while (trname.Contains("\\"))
                    {
                        trname = trname.Substring(trname.IndexOf("\\") + 1);
                    }

                    entrytobuild.TrueName = trname;
                    entrytobuild.TrueName = Path.GetFileNameWithoutExtension(trname);
                    entrytobuild.FileExt = trname.Substring(trname.LastIndexOf("."));

                    //Gets the path of the selected node to inject here.
                    string nodepath = tree.SelectedNode.FullPath;
                    nodepath = nodepath.Substring(nodepath.IndexOf("\\") + 1);

                    string[] sepstr = { "\\" };
                    entrytobuild.EntryDirs = nodepath.Split(sepstr, StringSplitOptions.RemoveEmptyEntries);

                    //Looks through the archive_filetypes.cfg file to find the typehash associated with the extension.
                    try
                    {
                        using (var sr2 = new StreamReader("archive_filetypes.cfg"))
                        {
                            while (!sr2.EndOfStream)
                            {
                                var keyword = Console.ReadLine() ?? entrytobuild.FileExt;
                                var line = sr2.ReadLine();
                                if (String.IsNullOrEmpty(line)) continue;
                                if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                                {
                                    entrytobuild.TypeHash = line;
                                    entrytobuild.TypeHash = entrytobuild.TypeHash.Split(' ')[0];

                                    break;
                                }
                            }
                        }
                    }
                    catch (FileNotFoundException)
                    {
                        MessageBox.Show("I cannot find and/or access archive_filetypes.cfg so I cannot finish parsing the arc.", "Oh Boy");
                        using (StreamWriter sw = File.AppendText("Log.txt"))
                        {
                            sw.WriteLine("Cannot find archive_filetypes.cfg so I cannot continue parsing the file.");
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText("Log.txt"))
                {
                    sw.WriteLine("Caught an exception using the BinaryReader. Here's the details:\n" + ex);
                }
            }


        }

        public static void InsertKnownEntry(TreeView tree, ArcEntryWrapper node, string filename, DefaultWrapper entrytobuild, BinaryReader bnr, Type filetype = null)
        {

            //We build the entrytobuild starting from the uncompressed data.
            entrytobuild.UncompressedData = System.IO.File.ReadAllBytes(filename);
            entrytobuild.DSize = entrytobuild.UncompressedData.Length;

            //Then Compress.
            entrytobuild.CompressedData = Zlibber.Compressor(entrytobuild.UncompressedData);
            entrytobuild.CSize = entrytobuild.CompressedData.Length;

            //Gets the filename of the file to inject without the directory.
            string trname = filename;
            while (trname.Contains("\\"))
            {
                trname = trname.Substring(trname.IndexOf("\\") + 1);
            }

            entrytobuild.TrueName = trname;
            entrytobuild.TrueName = Path.GetFileNameWithoutExtension(trname);
            entrytobuild.FileExt = trname.Substring(trname.LastIndexOf("."));

            //Gets the path of the selected node to inject here.
            string nodepath = tree.SelectedNode.FullPath;
            nodepath = nodepath.Substring(nodepath.IndexOf("\\") + 1);

            string[] sepstr = { "\\" };
            entrytobuild.EntryDirs = nodepath.Split(sepstr, StringSplitOptions.RemoveEmptyEntries);

            //Looks through the archive_filetypes.cfg file to find the typehash associated with the extension.
            try
            {
                using (var sr2 = new StreamReader("archive_filetypes.cfg"))
                {
                    while (!sr2.EndOfStream)
                    {
                        var keyword = Console.ReadLine() ?? entrytobuild.FileExt;
                        var line = sr2.ReadLine();
                        if (String.IsNullOrEmpty(line)) continue;
                        if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                        {
                            entrytobuild.TypeHash = line;
                            entrytobuild.TypeHash = entrytobuild.TypeHash.Split(' ')[0];

                            break;
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("I cannot find and/or access archive_filetypes.cfg so I cannot finish parsing the arc.", "Oh Boy");
                using (StreamWriter sw = File.AppendText("Log.txt"))
                {
                    sw.WriteLine("Cannot find archive_filetypes.cfg so I cannot continue parsing the file.");
                }
            }


        }

        public static void ReplaceKnownEntry(TreeView tree, ArcEntryWrapper node, string filename, DefaultWrapper entrytobuild, DefaultWrapper entrytoreplace, Type filetype = null)
        {

            tree.BeginUpdate();


            //We build the entrytobuild starting from the uncompressed data.
            entrytobuild.UncompressedData = System.IO.File.ReadAllBytes(filename);

            //Then Compress.
            entrytobuild.CompressedData = Zlibber.Compressor(entrytobuild.UncompressedData);


            //Gets the filename of the file to inject without the directory.
            string trname = filename;
            while (trname.Contains("\\"))
            {
                trname = trname.Substring(trname.IndexOf("\\") + 1);
            }

            //Enters name related parameters of the entrytobuild.
            entrytobuild.TrueName = trname;
            entrytobuild.TrueName = Path.GetFileNameWithoutExtension(trname);
            entrytobuild.FileExt = trname.Substring(trname.LastIndexOf("."));

            string TypeHash = "";

            //Looks through the archive_filetypes.cfg file to find the typehash associated with the extension.
            try
            {
                using (var sr2 = new StreamReader("archive_filetypes.cfg"))
                {
                    while (!sr2.EndOfStream)
                    {
                        var keyword = Console.ReadLine() ?? entrytobuild.FileExt;
                        var line = sr2.ReadLine();
                        if (String.IsNullOrEmpty(line)) continue;
                        if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                        {
                            TypeHash = line;
                            TypeHash = TypeHash.Split(' ')[0];
                            entrytobuild.TypeHash = TypeHash;
                            break;
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("I cannot find and/or access archive_filetypes.cfg so I cannot finish parsing the arc.", "Oh Boy");
                using (StreamWriter sw = File.AppendText("Log.txt"))
                {
                    sw.WriteLine("Cannot find archive_filetypes.cfg so I cannot continue parsing the file.");
                }
                return;
            }

            var tag = node.Tag;
            if (tag is DefaultWrapper)
            {
                entrytoreplace = tag as DefaultWrapper;
            }

            string path = "";
            int index = entrytoreplace.EntryName.LastIndexOf("\\");
            if (index > 0)
            {
                path = entrytoreplace.EntryName.Substring(0, index);
            }

            entrytobuild.EntryName = path + "\\" + entrytobuild.TrueName;

            tag = entrytobuild;

            if (node.Tag is DefaultWrapper)
            {
                node.Tag = entrytobuild;
                node.Name = Path.GetFileNameWithoutExtension(entrytobuild.EntryName);
                node.Text = Path.GetFileNameWithoutExtension(entrytobuild.EntryName);

            }

            var aew = node as ArcEntryWrapper;

            string type = node.GetType().ToString();
            if (type == "ThreeWorkTool.Resources.Wrappers.ArcEntryWrapper")
            {
                aew.entryfile = entrytobuild;
            }

            node = aew;
            node.entryfile = entrytobuild;

        }

    }
}
