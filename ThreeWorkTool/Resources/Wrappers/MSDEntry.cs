using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Archives;
using ThreeWorkTool.Resources.Utility;
using ThreeWorkTool.Resources.Wrappers;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class MSDEntry : DefaultWrapper
    {
        public string Magic;
        public string Constant;
        public int EntryCount;
        public byte[] WTemp;
        public StringBuilder SBTextBuild;
        public List<string> TextBackup;

        public struct MessageEntries
        {
            public int MSLength;
            public string contents;
        }

        public List<MessageEntries> EntryList;

        public static MSDEntry FillMSDEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, byte[] Bytes, int c, int ID, Type filetype = null)
        {
            MSDEntry MSEntry = new MSDEntry();

            FillEntry(filename, subnames, tree, br, c, ID, MSEntry);

            MSEntry._FileType = MSEntry.FileExt;
            MSEntry._FileName = MSEntry.TrueName;

            //Specific file type work goes here!

            //Gets the Magic.
            MSEntry.Magic = BitConverter.ToString(MSEntry.UncompressedData, 0,4).Replace("-", string.Empty);

            //Gets Entry count. Apparently it's a 32-bit int and not a 16-bit one here.
            byte[] FCTemp = new byte[4];
            Array.Copy(MSEntry.UncompressedData, 4, FCTemp, 0, 4);
            MSEntry.EntryCount = BitConverter.ToInt32(FCTemp, 0);
            MSEntry._EntryTotal = MSEntry.EntryCount;

            MSEntry.EntryList = new List<MessageEntries>();
            int OSTemp = 8;

            //Fills in the Entries.
            for (int g = 0; g < MSEntry.EntryCount; g++)
            {
                MessageEntries me = new MessageEntries();
                Array.Copy(MSEntry.UncompressedData, OSTemp, FCTemp, 0, 4);
                me.MSLength = BitConverter.ToInt32(FCTemp, 0);
                OSTemp = OSTemp + 2;
                StringBuilder SBTextBuild = new StringBuilder();

                //Gets each word and translates it into text.
                for (int h = 0; h < me.MSLength; h++)
                {
                    SBTextBuild.Clear();
                    byte ByTempA = (byte)(MSEntry.UncompressedData[OSTemp] + 0x20);
                    OSTemp = OSTemp + 2;
                    byte ByTempB = MSEntry.UncompressedData[OSTemp];

                    SBTextBuild.Append((char)ByTempA);

                    me.contents = me.contents + SBTextBuild.ToString();
                }
                

                MSEntry.EntryList.Add(me);
                OSTemp = OSTemp + 2;

            }



            MSEntry.TextBackup = new List<string>();

            return MSEntry;
        }

        #region MSD Properties

        private string _FileName;
        [Category("Resource Path List"), ReadOnlyAttribute(true)]
        public string FileName
        {

            get
            {
                return _FileName;
            }
            set
            {
                _FileName = value;
            }
        }

        private string _FileType;
        [Category("Resource Path List"), ReadOnlyAttribute(true)]
        public string FileType
        {

            get
            {
                return _FileType;
            }
            set
            {
                _FileType = value;
            }
        }

        private long _FileLength;
        [Category("Resource Path List"), ReadOnlyAttribute(true)]
        public long FileLength
        {

            get
            {
                return _FileLength;
            }
            set
            {
                _FileLength = value;
            }
        }

        private int _EntryTotal;
        [Category("Resource Path List"), ReadOnlyAttribute(true)]
        public int EntryTotal
        {
            get
            {
                return _EntryTotal;
            }
            set
            {
                _EntryTotal = value;
            }
        }

        #endregion

        public static TextBox LoadMSDInTextBox(TextBox texbox, MSDEntry msde)
        {

            texbox.Text = "";

            bool isEmpty = !msde.TextBackup.Any();
            if (isEmpty)
            {
                for (int t = 0; t < msde.EntryList.Count; t++)
                {
                    texbox.Text = texbox.Text + System.Environment.NewLine;
                    msde.TextBackup.Add(System.Environment.NewLine);
                }
            }
            else
            {

                for (int t = 0; t < msde.EntryList.Count; t++)
                {
                    texbox.Text = texbox.Text + msde.EntryList[t].contents + System.Environment.NewLine;
                }

            }



            return texbox;
        }


        public static ResourcePathListEntry ReplaceMSD(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            MSDEntry MSDNentry = new MSDEntry();
            MSDEntry MSDoldentry = new MSDEntry();

            tree.BeginUpdate();

            //Gotta Fix this up then test insert and replacing.
            try
            {
                using (FileStream fs = File.OpenRead(filename))
                {
                    //We build the arcentry starting from the uncompressed data.
                    MSDNentry.UncompressedData = System.IO.File.ReadAllBytes(filename);

                    //Then Compress.
                    MSDNentry.CompressedData = Zlibber.Compressor(MSDNentry.UncompressedData);

                    //Gets the filename of the file to inject without the directory.
                    string trname = filename;
                    while (trname.Contains("\\"))
                    {
                        trname = trname.Substring(trname.IndexOf("\\") + 1);
                    }

                    //Enters name related parameters of the arcentry.
                    MSDNentry.TrueName = trname;
                    MSDNentry._FileName = MSDNentry.TrueName;
                    MSDNentry.TrueName = Path.GetFileNameWithoutExtension(trname);
                    MSDNentry.FileExt = trname.Substring(trname.LastIndexOf("."));
                    MSDNentry._FileType = MSDNentry.FileExt;

                    string TypeHash = "";

                    //Looks through the archive_filetypes.cfg file to find the typehash associated with the extension.
                    try
                    {
                        using (var sr2 = new StreamReader("archive_filetypes.cfg"))
                        {
                            while (!sr2.EndOfStream)
                            {
                                var keyword = Console.ReadLine() ?? MSDNentry.FileExt;
                                var line = sr2.ReadLine();
                                if (String.IsNullOrEmpty(line)) continue;
                                if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                                {
                                    TypeHash = line;
                                    TypeHash = TypeHash.Split(' ')[0];
                                    //arcentry.TypeHash = TypeHash;
                                    break;
                                }
                            }
                        }
                    }
                    catch (FileNotFoundException)
                    {
                        MessageBox.Show("I cannot find and/or access archive_filetypes.cfg so I cannot finish parsing the arc.", "Oh Boy");

                    }

                    ASCIIEncoding ascii = new ASCIIEncoding();

                    //Gets the Magic.
                    byte[] MTemp = new byte[4];
                    string STemp = " ";
                    Array.Copy(MSDNentry.UncompressedData, 0, MTemp, 0, 4);
                    MSDNentry.Magic = ByteUtilitarian.BytesToString(MTemp, MSDNentry.Magic);

                    Array.Copy(MSDNentry.UncompressedData, 12, MTemp, 0, 4);
                    Array.Reverse(MTemp);
                    STemp = ByteUtilitarian.BytesToString(MTemp, STemp);

                    int ECTemp = Convert.ToInt32(STemp, 16);
                    MSDNentry._EntryTotal = ECTemp;
                    MSDNentry.EntryCount = ECTemp;

                    //Starts occupying the entry list via structs. 
                    MSDNentry.EntryList = new List<MessageEntries>();
                    byte[] PLName = new byte[] { };
                    byte[] PTHName = new byte[] { };

                    int p = 16;

                    for (int g = 0; g < MSDNentry.EntryCount; g++)
                    {
                        MessageEntries pe = new MessageEntries();
                        //Fill in msd populating code.

                    }

                    MSDNentry.TextBackup = new List<string>();
                    MSDNentry._FileLength = MSDNentry.UncompressedData.Length;

                    var tag = node.Tag;
                    if (tag is MSDEntry)
                    {
                        MSDoldentry = tag as MSDEntry;
                    }
                    string path = "";
                    int index = MSDoldentry.EntryName.LastIndexOf("\\");
                    if (index > 0)
                    {
                        path = MSDoldentry.EntryName.Substring(0, index);
                    }

                    MSDNentry.EntryName = path + "\\" + MSDNentry.TrueName;

                    tag = MSDNentry;

                    if (node.Tag is ResourcePathListEntry)
                    {
                        node.Tag = MSDNentry;
                        node.Name = Path.GetFileNameWithoutExtension(MSDNentry.EntryName);
                        node.Text = Path.GetFileNameWithoutExtension(MSDNentry.EntryName);

                    }

                    var aew = node as ArcEntryWrapper;

                    string type = node.GetType().ToString();
                    if (type == "ThreeWorkTool.Resources.Wrappers.ArcEntryWrapper")
                    {
                        aew.entryfile = MSDNentry;
                    }

                    node = aew;
                    node.entryfile = MSDNentry;
                    tree.EndUpdate();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Read error. Is the file readable?");
                using (StreamWriter sw = File.AppendText("Log.txt"))
                {
                    sw.WriteLine("MSD Replacement failed. Here's details:\n" + ex);
                }
            }



            return node.entryfile as ResourcePathListEntry;
        }


    }
}
