using Ionic.Zlib;
using Kaitai;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Archives;
using ThreeWorkTool.Resources.Utility;
using ThreeWorkTool.Resources.Wrappers;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class LMTEntry : DefaultWrapper
    {
        public int Version;
        public int SecondOffsetList;
        public int RowCount;
        public int SomeValue1;
        public int OffsetOfInterest;
        public int Length;
        public List<long> OffsetList;
        public List<LMTM3AEntry> LstM3A;
        public Lmt MotionData;
        //public List<Lmt.Animentry> MotionEntries;

        public static LMTEntry FillLMTEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
        {

            LMTEntry lmtentry = new LMTEntry();
            List<byte> BTemp = new List<byte>();

            FillEntry(filename, subnames, tree, br, c, ID, lmtentry, filetype);

            lmtentry._FileName = lmtentry.TrueName + lmtentry.FileExt;
            lmtentry._FileType = lmtentry.FileExt;

            //Decompression Time.
            lmtentry.UncompressedData = ZlibStream.UncompressBuffer(lmtentry.CompressedData);
            lmtentry._DecompressedFileLength = lmtentry.UncompressedData.Length;
            lmtentry._CompressedFileLength = lmtentry.CompressedData.Length;

            int count = 0;
            byte[] STemp = new byte[2];
            byte[] OTemp = new byte[4];

            lmtentry.LstM3A = new List<LMTM3AEntry>();
            lmtentry.OffsetList = new List<long>();
            int SecondaryCount = 0;

            using (MemoryStream LmtStream = new MemoryStream(lmtentry.UncompressedData))
            {
                using (BinaryReader bnr = new BinaryReader(LmtStream))
                {
                    bnr.BaseStream.Position = 6;
                    lmtentry.Version = bnr.ReadInt16();
                    lmtentry.EntryCount = lmtentry.Version;

                    //Gets all the offsets. ALL OF THEM.
                    while (count < (lmtentry.Version))
                    {
                        lmtentry.OffsetList.Add(bnr.ReadInt64());
                        count++;

                    }

                    count = 0;
                    //Goes through the offsets to get the data. Ignores offsets of 0.
                    for (int i = 0; i < lmtentry.OffsetList.Count; i++)
                    {
                        if (lmtentry.OffsetList[i] != 0)
                        {

                            LMTM3AEntry aEntry = new LMTM3AEntry();
                            aEntry = aEntry.FillM3AProprties(aEntry, lmtentry.Length, i, lmtentry.RowCount, lmtentry.SecondOffsetList, bnr, SecondaryCount, lmtentry);
                            lmtentry.LstM3A.Add(aEntry);


                        }
                        else
                        {
                            LMTM3AEntry aEntry = new LMTM3AEntry();
                            aEntry = aEntry.FillBlankM3A(aEntry, lmtentry.Length, i, lmtentry.RowCount, lmtentry.SecondOffsetList, bnr, SecondaryCount, lmtentry);
                            lmtentry.LstM3A.Add(aEntry);

                        }

                    }

                    bnr.BaseStream.Position = 0;

                }

            }

            //Only runs in debug mode. Nabs the Keyframes through the KaitaiStruct coding.
#if DEBUG
            lmtentry.MotionData = new Lmt(new KaitaiStream(lmtentry.UncompressedData));


            for (int j = 0; j < lmtentry.LstM3A.Count; j++)
            {
                Lmt.Animentry Anim = lmtentry.MotionData.Entries[j].Entry;
                if (Anim != null)
                {
                    int Framecount = Anim.Numframes;
                    for (int k = 0; k < Anim.Tracklist.Count; k++)
                    {
                        if (Anim.Tracklist[k].Boneid == 255)
                        {
                            continue;
                        }
                        else
                        {
                            if (Anim.Tracklist[k].Buffer != null)
                            {




                            }


                        }



                    }

                }
                //    var anim = 

            }

#endif


            return lmtentry;



        }

        public static LMTEntry ReplaceLMTEntry(TreeView tree, ArcEntryWrapper node, ArcEntryWrapper OldNode, string filename, Type filetype = null)
        {
            LMTEntry lmtentry = new LMTEntry();
            LMTEntry oldentry = new LMTEntry();
            oldentry = OldNode.Tag as LMTEntry;

            tree.BeginUpdate();

            try
            {
                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
                {
                    //We build the lmtentry starting from the uncompressed data.
                    lmtentry.UncompressedData = System.IO.File.ReadAllBytes(filename);
                    lmtentry.DecompressedFileLength = lmtentry.UncompressedData.Length;
                    lmtentry._DecompressedFileLength = lmtentry.UncompressedData.Length;

                    //Then Compress.
                    lmtentry.CompressedData = Zlibber.Compressor(lmtentry.UncompressedData);
                    lmtentry.CompressedFileLength = lmtentry.CompressedData.Length;
                    lmtentry._CompressedFileLength = lmtentry.CompressedData.Length;

                    //Gets the filename of the file to inject without the directory.
                    string trname = filename;
                    while (trname.Contains("\\"))
                    {
                        trname = trname.Substring(trname.IndexOf("\\") + 1);
                    }

                    //Enters name related parameters of the lmtentry.
                    lmtentry.TrueName = trname;
                    lmtentry._FileName = lmtentry.TrueName;
                    lmtentry.FileName = lmtentry.TrueName;
                    lmtentry.TrueName = Path.GetFileNameWithoutExtension(trname);
                    lmtentry.FileExt = trname.Substring(trname.LastIndexOf("."));
                    lmtentry._FileType = lmtentry.FileExt;

                    string TypeHash = "";

                    //Looks through the archive_filetypes.cfg file to find the typehash associated with the extension.
                    try
                    {
                        string ProperPath = "";
                        ProperPath = Globals.ToolPath + "archive_filetypes.cfg";
                        using (var sr2 = new StreamReader(ProperPath))
                        {
                            while (!sr2.EndOfStream)
                            {
                                var keyword = Console.ReadLine() ?? lmtentry.FileExt;
                                var line = sr2.ReadLine();
                                if (String.IsNullOrEmpty(line)) continue;
                                if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                                {
                                    TypeHash = line;
                                    TypeHash = TypeHash.Split(' ')[0];
                                    lmtentry.TypeHash = TypeHash;
                                    break;
                                }
                            }
                        }
                    }
                    catch (FileNotFoundException)
                    {
                        MessageBox.Show("I cannot find and/or access archive_filetypes.cfg so I cannot finish parsing the arc.", "Oh Boy");
                        string ProperPath = "";
                        ProperPath = Globals.ToolPath + "Log.txt"; using (StreamWriter sw = File.AppendText(ProperPath))
                        {
                            sw.WriteLine("Cannot find archive_filetypes.cfg so I cannot continue parsing the file.");
                        }
                        return null;
                    }


                    int count = 0;
                    int SecondaryCount = 0;

                    using (MemoryStream msm3a = new MemoryStream(lmtentry.UncompressedData))
                    {
                        using (BinaryReader brm3a = new BinaryReader(msm3a))
                        {

                            bnr.BaseStream.Position = 6;
                            lmtentry.Version = bnr.ReadInt16();
                            lmtentry.EntryCount = lmtentry.Version;
                            lmtentry.OffsetList = new List<long>();
                            lmtentry.LstM3A = new List<LMTM3AEntry>();

                            //Gets all the offsets. ALL OF THEM.
                            while (count < (lmtentry.Version))
                            {
                                lmtentry.OffsetList.Add(bnr.ReadInt64());
                                count++;

                            }

                            count = 0;
                            //Goes through the offsets to get the data. Ignores offsets of 0.
                            for (int i = 0; i < lmtentry.OffsetList.Count; i++)
                            {
                                if (lmtentry.OffsetList[i] != 0)
                                {

                                    LMTM3AEntry aEntry = new LMTM3AEntry();
                                    aEntry = aEntry.FillM3AProprties(aEntry, lmtentry.Length, i, lmtentry.RowCount, lmtentry.SecondOffsetList, bnr, SecondaryCount, lmtentry);
                                    lmtentry.LstM3A.Add(aEntry);


                                }
                                else
                                {
                                    LMTM3AEntry aEntry = new LMTM3AEntry();
                                    aEntry = aEntry.FillBlankM3A(aEntry, lmtentry.Length, i, lmtentry.RowCount, lmtentry.SecondOffsetList, bnr, SecondaryCount, lmtentry);
                                    lmtentry.LstM3A.Add(aEntry);

                                }

                            }


                        }
                    }


                    lmtentry.TrueName = oldentry.TrueName;
                    lmtentry._FileName = oldentry._FileName;
                    lmtentry.EntryName = oldentry.EntryName;


                    var tag = node.Tag;
                    if (tag is LMTEntry)
                    {
                        oldentry = tag as LMTEntry;
                    }
                    string path = "";
                    int index = oldentry.EntryName.LastIndexOf("\\");
                    if (index > 0)
                    {
                        path = oldentry.EntryName.Substring(0, index);
                    }

                    lmtentry.EntryName = path + "\\" + lmtentry.TrueName;

                    tag = lmtentry;

                    if (node.Tag is LMTEntry)
                    {
                        node.Tag = lmtentry;
                        node.Name = Path.GetFileNameWithoutExtension(lmtentry.EntryName);
                        node.Text = Path.GetFileNameWithoutExtension(lmtentry.EntryName);

                    }

                    var aew = node as ArcEntryWrapper;

                    string type = node.GetType().ToString();
                    if (type == "ThreeWorkTool.Resources.Wrappers.ArcEntryWrapper")
                    {
                        aew.entryfile = lmtentry;
                    }

                    node = aew;
                    node.entryfile = lmtentry;
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
                string ProperPath = "";
                ProperPath = Globals.ToolPath + "Log.txt"; using (StreamWriter sw = File.AppendText(ProperPath))
                {
                    sw.WriteLine("Read Error! Here's the exception info:\n" + ex);
                }
            }



            return node.entryfile as LMTEntry;
        }

        public static LMTEntry InsertLMTEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            LMTEntry lmtentry = new LMTEntry();

            try
            {
                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
                {
                    //We build the lmtentry starting from the uncompressed data.
                    lmtentry.UncompressedData = System.IO.File.ReadAllBytes(filename);
                    lmtentry.DecompressedFileLength = lmtentry.UncompressedData.Length;
                    lmtentry._DecompressedFileLength = lmtentry.UncompressedData.Length;
                    lmtentry.DSize = lmtentry.UncompressedData.Length;

                    //Then Compress.
                    lmtentry.CompressedData = Zlibber.Compressor(lmtentry.UncompressedData);
                    lmtentry.CompressedFileLength = lmtentry.CompressedData.Length;
                    lmtentry._CompressedFileLength = lmtentry.CompressedData.Length;
                    lmtentry.CSize = lmtentry.CompressedData.Length;

                    //Gets the filename of the file to inject without the directory.
                    string trname = filename;
                    while (trname.Contains("\\"))
                    {
                        trname = trname.Substring(trname.IndexOf("\\") + 1);
                    }

                    lmtentry.TrueName = trname;
                    lmtentry._FileName = lmtentry.TrueName;
                    lmtentry.TrueName = Path.GetFileNameWithoutExtension(trname);
                    lmtentry.FileExt = trname.Substring(trname.LastIndexOf("."));
                    lmtentry._FileType = lmtentry.FileExt;

                    lmtentry.LstM3A = new List<LMTM3AEntry>();
                    lmtentry.OffsetList = new List<long>();

                    bnr.BaseStream.Position = 6;
                    lmtentry.Version = bnr.ReadInt16();
                    lmtentry.EntryCount = lmtentry.Version;

                    int count = 0;
                    int SecondaryCount = 0;
                    //Gets all the offsets. ALL OF THEM.
                    while (count < (lmtentry.Version))
                    {
                        lmtentry.OffsetList.Add(bnr.ReadInt64());
                        count++;

                    }

                    count = 0;
                    //Goes through the offsets to get the data. Ignores offsets of 0.
                    for (int i = 0; i < lmtentry.OffsetList.Count; i++)
                    {
                        if (lmtentry.OffsetList[i] != 0)
                        {

                            LMTM3AEntry aEntry = new LMTM3AEntry();
                            aEntry = aEntry.FillM3AProprties(aEntry, lmtentry.Length, i, lmtentry.RowCount, lmtentry.SecondOffsetList, bnr, SecondaryCount, lmtentry);
                            lmtentry.LstM3A.Add(aEntry);


                        }
                        else
                        {
                            LMTM3AEntry aEntry = new LMTM3AEntry();
                            aEntry = aEntry.FillBlankM3A(aEntry, lmtentry.Length, i, lmtentry.RowCount, lmtentry.SecondOffsetList, bnr, SecondaryCount, lmtentry);
                            lmtentry.LstM3A.Add(aEntry);

                        }

                    }

                    //Gets the path of the selected node to inject here.
                    string nodepath = tree.SelectedNode.FullPath;
                    nodepath = nodepath.Substring(nodepath.IndexOf("\\") + 1);

                    string[] sepstr = { "\\" };
                    lmtentry.EntryDirs = nodepath.Split(sepstr, StringSplitOptions.RemoveEmptyEntries);
                    lmtentry.EntryName = lmtentry.FileName;

                    //Looks through the archive_filetypes.cfg file to find the typehash associated with the extension.
                    try
                    {
                        string ProperPath = "";
                        ProperPath = Globals.ToolPath + "archive_filetypes.cfg";
                        using (var sr2 = new StreamReader(ProperPath))
                        {
                            while (!sr2.EndOfStream)
                            {
                                var keyword = Console.ReadLine() ?? lmtentry.FileExt;
                                var line = sr2.ReadLine();
                                if (String.IsNullOrEmpty(line)) continue;
                                if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                                {
                                    lmtentry.TypeHash = line;
                                    lmtentry.TypeHash = lmtentry.TypeHash.Split(' ')[0];

                                    break;
                                }
                            }
                        }
                    }
                    catch (FileNotFoundException)
                    {
                        MessageBox.Show("I cannot find and/or access archive_filetypes.cfg so I cannot finish parsing the arc.", "Oh Boy");
                        string ProperPath = "";
                        ProperPath = Globals.ToolPath + "Log.txt"; using (StreamWriter sw = File.AppendText(ProperPath))
                        {
                            sw.WriteLine("Cannot find archive_filetypes.cfg so I cannot continue parsing the file.");
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                string ProperPath = "";
                ProperPath = Globals.ToolPath + "Log.txt"; using (StreamWriter sw = File.AppendText(ProperPath))
                {
                    sw.WriteLine("Caught an exception using the BinaryReader. Here's the details:\n" + ex);
                }
            }



            return lmtentry;
        }

        public static LMTEntry RebuildLMTEntry(TreeView tree, ArcEntryWrapper node, Type filetype = null)
        {
            //Gets the nodes and stuff and starts rebuilding from scratch.
            LMTEntry lMT = new LMTEntry();

            int ChildCount = 0;

            //Fetches and Iterates through all the children and extracts the files tagged in the nodes.
            List<TreeNode> Children = new List<TreeNode>();
            foreach (TreeNode thisNode in tree.SelectedNode.Nodes)
            {
                Children.Add(thisNode);
                ChildCount++;
            }

            //Now to rebuild from scratch.
            List<byte> NewUncompressedData = new List<byte>();
            byte[] Header = { 0x4C, 0x4D, 0x54, 0x00, 0x43, 0x00 };
            byte[] PlaceHolderEntry = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            byte[] BlankLine = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            byte[] BlankHalf = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            //Gets Entry Count.
            short Total = Convert.ToInt16(Children.Count);
            NewUncompressedData.AddRange(Header);
            NewUncompressedData.AddRange(BitConverter.GetBytes(Total));
            //Adds in dummy bytes for Entry Offset List based on amount of child nodes of the lmt node. Adds an extra entry because the default LMTs do.
            NewUncompressedData.AddRange(PlaceHolderEntry);
            for (int w = 0; w < Children.Count; w++)
            {
                NewUncompressedData.AddRange(PlaceHolderEntry);
            }
            int MA3DataStart = NewUncompressedData.Count;
            lMT.OffsetList = new List<long>();
            List<int> TempOffsetList = new List<int>();
            List<int> DataOffsetList = new List<int>();
            List<bool> IsBlank = new List<bool>();
            //Starts putting in the Block Data and updating the offset list.
            lMT.OffsetList.Add(NewUncompressedData.Count);
            for (int x = 0; x < Children.Count; x++)
            {
                TreeNode TN = tree.SelectedNode.Nodes.Find(x.ToString(), true)[0];
                LMTM3AEntry tag = TN.Tag as LMTM3AEntry;
                if (tag != null)
                {
                    IsBlank.Add(tag.IsBlank);
                    if (tag.IsBlank == false)
                    {

                        //Checks the BlockData for the ReuseAnmation Flag and sets it based on user setting.


                        NewUncompressedData.AddRange(tag.MotionData);
                        
                        //The ending of the block data segments always has the raw data start on the 8 of the hex instead of the 0 of the hex offset for some reason.
                        //This is there to preserve that.
                        if (x == (Children.Count - 1))
                        {

                        }
                        else
                        {
                            NewUncompressedData.AddRange(BlankHalf);
                        }
                        
                    }

                }
                lMT.OffsetList.Add(NewUncompressedData.Count);
            }
            //Now for the RawData. Oh joy.
            DataOffsetList.Add(NewUncompressedData.Count);
            for (int y = 0; y < Children.Count; y++)
            {
                TreeNode TN = tree.SelectedNode.Nodes.Find(y.ToString(), true)[0];
                LMTM3AEntry tag = TN.Tag as LMTM3AEntry;
                if (tag != null)
                {
                    //int Tempint = tag.RawData.Length - 88;
                    if (IsBlank[y] == false)
                    {
                        //Gotta ensure the bottom 88 bytes are not written to the NewUncompressedData.
                        NewUncompressedData.AddRange(tag.RawData);
                        DataOffsetList.Add(NewUncompressedData.Count);
                    }
                }

            }
            byte[] UnCompressedBuffer = NewUncompressedData.ToArray();
            int Capacity = UnCompressedBuffer.Length;
            int EntryAmount = lMT.OffsetList.Count - 1;
            List<int> IndexRows = new List<int>();
            using (MemoryStream ms3 = new MemoryStream(UnCompressedBuffer))
            {
                using (BinaryReader br3 = new BinaryReader(ms3))
                {
                    using (BinaryWriter bw3 = new BinaryWriter(ms3))
                    {
                        bw3.BaseStream.Position = 8;
                        //Offsets For The Block Data.
                        for (int z = 0; z < EntryAmount; z++)
                        {

                            if (IsBlank[z] == false)
                            {
                                bw3.Write(lMT.OffsetList[z]);
                            }
                            else
                            {
                                bw3.BaseStream.Position = bw3.BaseStream.Position + 8;
                            }
                        }
                        //Offsets For the Raw Data.
                        int EndingOffset = 0;
                        int OffTemp = 0;
                        bw3.BaseStream.Position = lMT.OffsetList[0];
                        for (int zz = 0; zz < DataOffsetList.Count; zz++)
                        {


                            bw3.Write(DataOffsetList[zz]);

                            bw3.BaseStream.Position = bw3.BaseStream.Position + 4;
                            IndexRows.Add(br3.ReadInt32());
                            bw3.BaseStream.Position = bw3.BaseStream.Position + 60;
                            //bw3.BaseStream.Position = bw3.BaseStream.Position + 68;
                            if (zz == (DataOffsetList.Count - 1))
                            {
                                EndingOffset = UnCompressedBuffer.Length;
                                bw3.Write(EndingOffset);
                            }
                            else
                            {
                                EndingOffset = (DataOffsetList[zz + 1] - 352);
                                bw3.Write(EndingOffset);
                            }


                            bw3.BaseStream.Position = bw3.BaseStream.Position + 20;

                        }
                        //Lastly the offsets in the M3A entries themeslves. Sigh........
                        bw3.BaseStream.Position = DataOffsetList[0];
                        int CountTemp = DataOffsetList.Count - 1;
                        for (int yy = 0; yy < CountTemp; yy++)
                        {
                            bw3.BaseStream.Position = DataOffsetList[yy];
                            //bw3.BaseStream.Position = IndexRows[yy];
                            for (int xx = 0; xx < IndexRows[yy]; xx++)
                            {
                                bw3.BaseStream.Position = DataOffsetList[yy];
                                bw3.BaseStream.Position = DataOffsetList[yy] + 16 + (48 * xx);
                                OffTemp = br3.ReadInt32();
                                bw3.BaseStream.Position = (bw3.BaseStream.Position - 4);
                                if (OffTemp > 0)
                                {
                                    OffTemp = OffTemp + DataOffsetList[yy];
                                    bw3.Write(OffTemp);
                                }
                                bw3.BaseStream.Position = DataOffsetList[yy] + 40 + (48 * xx);

                                OffTemp = br3.ReadInt32();
                                bw3.BaseStream.Position = (bw3.BaseStream.Position - 4);
                                if (OffTemp > 0)
                                {
                                    OffTemp = OffTemp + DataOffsetList[yy];
                                    bw3.Write(OffTemp);
                                }

                            }

                            //Footer Things.
                            bw3.BaseStream.Position = (DataOffsetList[(yy + 1)] - 280);
                            //OffTemp = br3.ReadInt32();
                            OffTemp = DataOffsetList[(yy + 1)] - 32;
                            //bw3.BaseStream.Position = (bw3.BaseStream.Position - 4);
                            bw3.Write(OffTemp);
                            bw3.BaseStream.Position = bw3.BaseStream.Position + 76;
                            //OffTemp = br3.ReadInt32();
                            OffTemp = DataOffsetList[(yy + 1)] - 24;
                            //bw3.BaseStream.Position = (bw3.BaseStream.Position - 4);
                            bw3.Write(OffTemp);
                            bw3.BaseStream.Position = bw3.BaseStream.Position + 76;
                            //OffTemp = br3.ReadInt32();
                            OffTemp = DataOffsetList[(yy + 1)] - 16;
                            //bw3.BaseStream.Position = (bw3.BaseStream.Position - 4);
                            bw3.Write(OffTemp);
                            bw3.BaseStream.Position = bw3.BaseStream.Position + 76;
                            //OffTemp = br3.ReadInt32();
                            OffTemp = DataOffsetList[(yy + 1)] - 8;
                            //bw3.BaseStream.Position = (bw3.BaseStream.Position - 4);
                            bw3.Write(OffTemp);

                        }


                    }
                }
            }

            lMT.UncompressedData = UnCompressedBuffer;
            lMT.CompressedData = Zlibber.Compressor(lMT.UncompressedData);
            lMT.EntryCount = Children.Count;
            lMT._EntryCount = Children.Count;
            lMT._FileType = ".lmt";
            lMT.FileExt = ".lmt";

            return lMT;

        }

        public static LMTEntry TransferLMTEntryProperties(LMTEntry OldLMT, LMTEntry NewLMT, Type filetype = null)
        {

            NewLMT._FileName = OldLMT._FileName;
            NewLMT._CompressedFileLength = NewLMT.CompressedData.Length;
            NewLMT._DecompressedFileLength = NewLMT.UncompressedData.Length;
            NewLMT.FileExt = OldLMT.FileExt;
            NewLMT.FileName = OldLMT.FileName;
            NewLMT.TrueName = OldLMT.TrueName;
            NewLMT.EntryDirs = OldLMT.EntryDirs;
            NewLMT.EntryID = OldLMT.EntryID;
            NewLMT.TypeHash = OldLMT.TypeHash;
            NewLMT.EntryName = OldLMT.EntryName;

            return NewLMT;
        }

        #region LMTEntry Properties
        private string _FileName;
        [Category("Filename"), ReadOnlyAttribute(true)]
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

        private long _CompressedFileLength;
        [Category("MT ARC Entry"), ReadOnlyAttribute(true)]
        public long CompressedFileLength
        {

            get
            {
                return _CompressedFileLength;
            }
            set
            {
                _CompressedFileLength = value;
            }
        }

        private long _DecompressedFileLength;
        [Category("MT ARC Entry"), ReadOnlyAttribute(true)]
        public long DecompressedFileLength
        {

            get
            {
                return _DecompressedFileLength;
            }
            set
            {
                _DecompressedFileLength = value;
            }
        }

        private string _FileType;
        [Category("Filename"), ReadOnlyAttribute(true)]
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

        private long _EntryCount;
        [Category("MT ARC Entry"), ReadOnlyAttribute(true)]
        public long EntryCount
        {

            get
            {
                return _EntryCount;
            }
            set
            {
                _EntryCount = value;
            }
        }

        #endregion

    }
}
