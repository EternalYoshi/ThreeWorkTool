using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
        //public StringBuilder SBTextBuild;
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
            MSEntry.Magic = BitConverter.ToString(MSEntry.UncompressedData, 0, 4).Replace("-", string.Empty);
            using (MemoryStream mstream = new MemoryStream(MSEntry.UncompressedData))
            {
                using (BinaryReader bnr = new BinaryReader(mstream))
                {
                    bnr.BaseStream.Position = 4;
                    //Apparently the entry count is 32-bit and not 16-bit. Considering that some of these MSDs end up with over 10,000 entries...  yeah.
                    MSEntry.EntryCount = bnr.ReadInt32();
                    MSEntry._EntryTotal = MSEntry.EntryCount;
                    MSEntry._FileLength = MSEntry.UncompressedData.Length;
                }
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
                return TrueName;
            }
            set
            {
                TrueName = value;
            }
        }

        private string _FileType;
        [Category("Resource Path List"), ReadOnlyAttribute(true)]
        public string FileType
        {

            get
            {
                return FileExt;
            }
            set
            {
                FileExt = value;
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
                return EntryCount;
            }
            set
            {
                EntryCount = value;
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

        public static RichTextBox LoadMSDInTexEditorForm(RichTextBox texbox, MSDEntry msde)
        {
            texbox.Text = "";


            //Fills in the entries using a hopefully more efficient method based on what I see from Anotak and co's MSD Editor from a few years back.
            using (MemoryStream mstream = new MemoryStream(msde.UncompressedData))
            {
                using (BinaryReader bnr = new BinaryReader(mstream))
                {
                    bnr.BaseStream.Position = 8;
                    msde.EntryList = new List<MessageEntries>();
                    StringBuilder SBuild = new StringBuilder();
                    msde.TextBackup = new List<string>(msde.EntryCount);
                    MessageEntries me = new MessageEntries();
                    byte[] TestArray = new byte[2];
                    int ITChar = 0;
                    string TChar = "";

                    while (bnr.BaseStream.Position < bnr.BaseStream.Length)
                    {
                        SBuild.Clear();
                        me.MSLength = bnr.ReadInt32();

                        for (int i = 0; i < me.MSLength; i++)
                        {
                            ITChar = bnr.ReadInt16();
                            TChar = ITChar.ToString("X4");

                            try
                            {
                                using (var sr = new StreamReader("MSDTable.cfg"))
                                {
                                    while (!sr.EndOfStream)
                                    {
                                        var keyword = Console.ReadLine() ?? TChar;
                                        var line = sr.ReadLine();
                                        if (String.IsNullOrEmpty(line)) continue;
                                        if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                                        {
                                            TChar = line;
                                            TChar = TChar.Split(' ')[1];
                                            break;
                                        }
                                    }
                                }

                            }
                            catch (FileNotFoundException)
                            {
                                MessageBox.Show("I cannot find MSDTable.cfg and cannot continue.\n Restart with this file in the same directory as the exe file itself.", "Oh Boy");
                                using (StreamWriter sw = File.AppendText("Log.txt"))
                                {
                                    sw.WriteLine("Cannot find MSDTable.cfg so I cannot load the MSD file.");
                                }
                                Process.GetCurrentProcess().Kill();
                            }

                            //For Irregular cases.
                            switch (TChar)
                            {

                                case "FFFFFF01":
                                    TChar = "[*1DPAD*]";
                                    break;

                                case "FFFFFF05":
                                    TChar = "[*5LTRIG*]";
                                    break;

                                case "FFFFFF06":
                                    TChar = "[*6BUMPERL*]";
                                    break;

                                case "FFFFFF09":
                                    TChar = "[*9ABTN*]";
                                    break;

                                case "FFFFFF0A":
                                    TChar = "[*AYBTN*]";
                                    break;

                                case "FFFFFF0B":
                                    TChar = "[*BXBTN*]";
                                    break;

                                case "FFFFFF0C":
                                    TChar = "[*CBBTN*]";
                                    break;

                                case "FFFFFF0D":
                                    TChar = "[*DRTRIG*]";
                                    break;

                                case "FFFFFF0E":
                                    TChar = "[*ERBUMPER*]";
                                    break;

                                case "FFFFFFFE":
                                    TChar = "[*line break*]";
                                    break;

                                case "FFFFFFFF":
                                    TChar = "";
                                    break;

                                case "":
                                    TChar = " ";
                                    break;

                                default:
                                    break;
                            }

                            SBuild.Append(TChar);

                        }

                        me.contents = SBuild.ToString();
                        msde.TextBackup.Add((me.contents));
                        texbox.Text = texbox.Text + me.contents + "\n";

                        msde.EntryList.Add(me);

                    }
                }
            }

            return texbox;

        }

        public static MSDEntry UpdateMSDFromTexEditorForm(RichTextBox texbox, MSDEntry msde)
        {
            //This gets the line count of the text box and eliminates the last line if empty.
            int lineCount = texbox.Lines.Count();
            lineCount -= String.IsNullOrWhiteSpace(texbox.Lines.Last()) ? 1 : 0;

            //Builds a new MSD File to replace the uncompressed and compressed data variables.
            List<byte> newMSDData = new List<byte>();
            List<byte> TempMSDData = new List<byte>();
            byte[] MSDTemp = new byte[4] { 0x4D, 0x53, 0x44, 0x00 };
            newMSDData.AddRange(MSDTemp);
            MSDTemp = BitConverter.GetBytes(lineCount);
            newMSDData.AddRange(MSDTemp);

            string STemp = "";
            string HexTemp = "";
            byte[] ByTemp = new byte[4];
            byte[] WTemp = new byte[4];
            byte[] HTemp = new byte[2];
            int LTemp;

            Encoding ShouldBeShiftJIS = Encoding.GetEncoding(932);

            for (int i = 0; i < lineCount; i++)
            {
                //Gets the line.
                STemp = texbox.Lines[i];
                LTemp = texbox.Lines[i].Length;
                ByTemp = BitConverter.GetBytes(LTemp);
                TempMSDData.Clear();

                //Iterates through each character in the line and does its thing.

                for (int j = 0; j < STemp.Length; j++)
                {
                    //This if statement is to check for line breaks in the middle of entries and if so, puts in the appropriate text and skips ahead to the next character.
                    if (STemp[j] == 91 && STemp[j + 1] == 42)
                    {
                        //Switch Case Time for Special Characters.
                        switch(STemp[j + 2])
                        {

                            case 'l':
                                TempMSDData.Add(0xFE);
                                TempMSDData.Add(0xFF);
                                j = j + 13;
                                break;

                            case '1':
                                TempMSDData.Add(0x01);
                                TempMSDData.Add(0xFF);
                                j = j + 8;
                                break;

                            case '5':
                                TempMSDData.Add(0x05);
                                TempMSDData.Add(0xFF);
                                j = j + 9;
                                break;

                            case '6':
                                TempMSDData.Add(0x06);
                                TempMSDData.Add(0xFF);
                                j = j + 11;
                                break;

                            case '9':
                                TempMSDData.Add(0x09);
                                TempMSDData.Add(0xFF);
                                j = j + 8;
                                break;

                            case 'A':
                                TempMSDData.Add(0x0A);
                                TempMSDData.Add(0xFF);
                                j = j + 8;
                                break;

                            case 'B':
                                TempMSDData.Add(0x0B);
                                TempMSDData.Add(0xFF);
                                j = j + 8;
                                break;

                            case 'C':
                                TempMSDData.Add(0x0C);
                                TempMSDData.Add(0xFF);
                                j = j + 8;
                                break;

                            case 'D':
                                TempMSDData.Add(0x0D);
                                TempMSDData.Add(0xFF);
                                j = j + 9;
                                break;

                            case 'E':
                                TempMSDData.Add(0x0E);
                                TempMSDData.Add(0xFF);
                                j = j + 11;
                                break;

                            default:
                                break;

                        }

                    }
                    else
                    {
                        HexTemp = " " + STemp[j].ToString();
                        try
                        {
                            using (var sr = new StreamReader("MSDTable.cfg"))
                            {
                                while (!sr.EndOfStream)
                                {
                                    var keyword = Console.ReadLine() ?? HexTemp;
                                    var line = sr.ReadLine();
                                    if (String.IsNullOrEmpty(line)) continue;
                                    if (line.IndexOf(keyword, StringComparison.CurrentCulture) >= 0)
                                    {
                                        HexTemp = line;
                                        HexTemp = HexTemp.Split(' ')[0];
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
                        if (HexTemp == "" || HexTemp == " " || HexTemp == "  ") HexTemp = "0000";
                        HTemp[0] = (byte)Int16.Parse(HexTemp.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                        HTemp[1] = (byte)Int16.Parse(HexTemp.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                        TempMSDData.AddRange(HTemp);

                    }
                }

                //Gets the Character Count by getting the Byte Count of the Raw MSD Data, and dividing it by 2.
                int CharCount = (TempMSDData.Count/2) + 1;
                ByTemp = BitConverter.GetBytes(CharCount);


                newMSDData.AddRange(ByTemp);
                newMSDData.AddRange(TempMSDData);

                byte[] TerTemp = { 0xFF, 0xFF };
                newMSDData.AddRange(TerTemp);

            }

            msde.UncompressedData = newMSDData.ToArray();
            msde.CompressedData = Zlibber.Compressor(msde.UncompressedData);

            msde.EntryCount = lineCount;

            return msde;

        }

        public static MSDEntry InsertMSD(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            MSDEntry msdentry = new MSDEntry();

            try
            {
                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
                {
                    InsertKnownEntry(tree, node, filename, msdentry, bnr);

                    //Gets the Magic.
                    msdentry.Magic = BitConverter.ToString(msdentry.UncompressedData, 0, 4).Replace("-", string.Empty);

                    bnr.BaseStream.Position = 4;
                    //Apparently the entry count is 32-bit and not 16-bit. Considering that some of these MSDs end up with over 10,000 entries...  yeah.
                    msdentry.EntryCount = bnr.ReadInt32();
                    msdentry._EntryTotal = msdentry.EntryCount;
                    msdentry._FileLength = msdentry.UncompressedData.Length;
                    msdentry._FileName = msdentry.TrueName;
                    msdentry._FileType = msdentry.FileExt;
                    msdentry.EntryName = msdentry.FileName;
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText("Log.txt"))
                {
                    sw.WriteLine("Caught the exception:" + ex);
                }
            }

            msdentry.TextBackup = new List<string>();


            return msdentry;
        }

        public static MSDEntry ReplaceMSD(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            MSDEntry MSDNentry = new MSDEntry();
            MSDEntry MSDoldentry = new MSDEntry();

            tree.BeginUpdate();

            ReplaceKnownEntry(tree, node, filename, MSDNentry, MSDoldentry);

            //Gets the Magic.
            MSDNentry.Magic = BitConverter.ToString(MSDNentry.UncompressedData, 0, 4).Replace("-", string.Empty);
            try
            {
                using (MemoryStream mstream = new MemoryStream(MSDNentry.UncompressedData))
                {
                    using (BinaryReader bnr = new BinaryReader(mstream))
                    {
                        bnr.BaseStream.Position = 4;
                        //Apparently the entry count is 32-bit and not 16-bit. Considering that some of these MSDs end up with over 10,000 entries...  yeah.
                        MSDNentry.EntryCount = bnr.ReadInt32();
                        MSDNentry._EntryTotal = MSDNentry.EntryCount;
                        MSDNentry._FileLength = MSDNentry.UncompressedData.Length;
                    }
                }

                MSDNentry.TextBackup = new List<string>();

                //Hmmm.

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

                if (node.Tag is MSDEntry)
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
            catch (Exception ex)
            {
                MessageBox.Show("Read error. Is the file readable?");
                using (StreamWriter sw = File.AppendText("Log.txt"))
                {
                    sw.WriteLine("Read error. Cannot access the file:" + filename + "\n" + ex);
                }
            }

            return node.entryfile as MSDEntry;
        }

        public static string SwitchCaser(string str)
        {



            return str;
        }

    }
}
