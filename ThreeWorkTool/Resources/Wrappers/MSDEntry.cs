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
            MSEntry.Magic = BitConverter.ToString(MSEntry.UncompressedData, 0,4).Replace("-", string.Empty);
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
                    while (bnr.BaseStream.Position < bnr.BaseStream.Length)
                    {
                        MessageEntries me = new MessageEntries();
                        me.MSLength = bnr.ReadInt16();
                        SBuild.Clear();
                        for (int i = 0; i < me.MSLength; i++)
                        {
                            byte bA = (byte)(bnr.ReadByte() + 0x20);
                            byte bB = bnr.ReadByte();
                            if (bB != 0)
                            {

                            }
                            SBuild.Append((char)bA);
                        }
                        me.contents = SBuild.ToString();
                        msde.TextBackup.Add((me.contents));
                        texbox.Text = texbox.Text + me.contents + "\n"; 
                        short termchar = bnr.ReadInt16();
                        if (termchar != -1)
                        {

                        }

                        msde.EntryList.Add(me);
                    }
                }
            }

            return texbox;




        }

        public static MSDEntry InsertMSD(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            MSDEntry msdentry = new MSDEntry();

            try
            {
                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
                {
                    InsertKnownEntry(tree, node, filename, msdentry,bnr);

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

            ReplaceKnownEntry(tree,node,filename,MSDNentry,MSDoldentry);

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


    }
}
