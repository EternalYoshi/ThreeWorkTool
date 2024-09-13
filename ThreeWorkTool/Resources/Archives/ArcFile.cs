using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Wrappers;
using ThreeWorkTool.Resources.Utility;


namespace ThreeWorkTool.Resources.Archives
{
    public class ArcFile
    {
        public List<ArcEntry> arctable;
        public List<ArcEntry> arctableBACKUP;
        public List<object> arcfiles;
        public List<String> FileList;
        public List<String> ArcFileNames;
        public List<String> TypeHashes;
        public int FLOffset;
        public static StringBuilder SBname;
        public byte[] HeaderMagic;
        public byte[] HeaderB;
        public int FileCount;
        public Type FileType;
        public byte UnknownFlag;
        public string Tempname;
        public string th;
        public string tn;
        public int k;
        public int IDCounter;
        public int Totalsize;
        public int MaterialCount;
        public List<string> subdref;
        public static int EntryStart = 0x08;
        public static int EntrySize = 0x50;

        public static string[] RecogExtensions = { ".TEX", ".MOD", ".MRL", ".CHN", ".CCL", ".CST", ".LMT", ".GEM", ".XSEW", ".LSH" };

        public static ArcFile LoadArc(TreeView tree, string filename, List<string> foldernames, bool IsBigEndian, bool Verifier = false, Type filetype = null, int arcsize = -1)
        {

            ArcFile arcfile = new ArcFile();
            byte[] Bytes = File.ReadAllBytes(filename);

            using (BinaryReader br = new BinaryReader(File.Open(filename, FileMode.Open)))
            {

                arcsize = Bytes.Length;
                int Totalsize = arcsize;
                arcfile.FileLength = arcsize;
                arcfile.Tempname = filename;

                br.BaseStream.Position = 0;
                byte[] HeaderMagic = br.ReadBytes(4);
                //Checks file signature/Endianess.
                if (HeaderMagic[0] == 0x00 && HeaderMagic[1] == 0x43 && HeaderMagic[2] == 0x52 && HeaderMagic[3] == 0x41)
                {
                    /*
                    MessageBox.Show("This .arc file is not in the kind of endian I can deal with right now, so these will be in read only.\nDon't expect save to work... or for the program to be stable", "Just so you know....");

                    IsBigEndian = true;
                    arcfile.HeaderMagic = HeaderMagic;
                    arcfile.MaterialCount = 0;
                    arcfile.arctable = new List<ArcEntry>();
                    arcfile.arcfiles = new List<object>();
                    arcfile.FileList = new List<string>();
                    arcfile.TypeHashes = new List<string>();

                    br.BaseStream.Position = 4;
                    var data = br.ReadBytes(2);
                    Array.Reverse(data);
                    arcfile.UnknownFlag = br.ReadByte();



                    return arcfile;

                    
                     
                    */

                    MessageBox.Show("This .arc file is not in the kind of endian I can deal with right now. Closing.", "Just so you know....");
                    br.Close();
                    return null;

                }

                if (HeaderMagic[0] != 0x41 && HeaderMagic[1] != 0x52 && HeaderMagic[2] != 0x43 && HeaderMagic[3] != 0x00)
                {
                    MessageBox.Show("This .arc file is either not the correct kind or is not properly extracted, so I'm closing it.", "Oh dear");
                    br.Close();
                    return null;
                }

                #region PC Arc
                IsBigEndian = false;
                arcfile.HeaderMagic = HeaderMagic;
                arcfile.MaterialCount = 0;
                arcfile.arctable = new List<ArcEntry>();
                arcfile.arcfiles = new List<object>();
                arcfile.FileList = new List<string>();
                arcfile.ArcFileNames = new List<string>();
                arcfile.TypeHashes = new List<string>();

                br.BaseStream.Position = 4;
                arcfile.UnknownFlag = br.ReadByte();

                br.BaseStream.Position = 6;
                arcfile.FileCount = BitConverter.ToInt16((br.ReadBytes(2)), 0);
                arcfile.Version = arcfile.UnknownFlag;

                List<String> filenames = new List<String>();

                List<string> subdref = new List<string>();
                foldernames = subdref;


                //byte[] BytesTemp;
                //BytesTemp = new byte[] { };
                List<byte> BytesTemp = new List<byte>();
                byte[] HTTemp = new byte[] { };
                int j = 8;
                int l = 64;
                int m = 80;
                int n = 4;

                //Iterates through the header/first part of the arc to get all the filenames and occupy the filename list.
                for (int i = 0; i < arcfile.FileCount; i++)
                {
                    BytesTemp.Clear();
                    BytesTemp.TrimExcess();
                    j = 8 + (m * i);
                    //Copies the specified range to isolate the bytes containing a filename.
                    br.BaseStream.Position = j;
                    BytesTemp.AddRange(br.ReadBytes(l));
                    BytesTemp.RemoveAll(ByteUtilitarian.IsZeroByte);
                    filenames.Add(ByteUtilitarian.BytesToStringL(BytesTemp));
                    //For The Typehashes.
                    n = 72 + (m * i);

                    br.BaseStream.Position = n;
                    HTTemp = br.ReadBytes(4);
                    Array.Reverse(HTTemp);
                    arcfile.TypeHashes.Add(ByteUtilitarian.HashBytesToString(HTTemp));

                }

                //Fills in each file as an ArcEntry or TextureEntry as needed. 
                j = 8;
                int IDCounter = 0;
                for (int i = 0; i < arcfile.FileCount; i++)
                {
                    j = 8 + (80 * i);
                    int dotin = 0;
                    switch (arcfile.TypeHashes[i])
                    {
                        //Texture Files.
                        case "241F5DEB":
                            TextureEntry newtexen = TextureEntry.FillTexEntry(filename, foldernames, tree, br, j, IDCounter);
                            arcfile.arcfiles.Add(newtexen);
                            arcfile.FileList.Add(newtexen.EntryName);
                            dotin = newtexen.EntryName.LastIndexOf(".");
                            arcfile.ArcFileNames.Add(newtexen.EntryName.Substring(0, (dotin)));
                            newtexen.BaseEntryName = newtexen.EntryName.Substring(0, (dotin));
                            foldernames.Clear();
                            IDCounter++;
                            break;

                        //Resource Path Lists.
                        case "357EF6D4":
                            ResourcePathListEntry newplen = ResourcePathListEntry.FillRPLEntry(filename, foldernames, tree, br, j, IDCounter);
                            arcfile.arcfiles.Add(newplen);
                            arcfile.FileList.Add(newplen.EntryName);
                            dotin = newplen.EntryName.LastIndexOf(".");
                            arcfile.ArcFileNames.Add(newplen.EntryName.Substring(0, (dotin)));
                            newplen.BaseEntryName = newplen.EntryName.Substring(0, (dotin));
                            foldernames.Clear();
                            IDCounter++;
                            break;


                        //Materials. Almost there.                        
                        case "2749C8A8":
                            MaterialEntry Maten = MaterialEntry.FillMatEntry(filename, foldernames, tree, br, j, IDCounter);
                            arcfile.arcfiles.Add(Maten);
                            arcfile.FileList.Add(Maten.EntryName);
                            dotin = Maten.EntryName.LastIndexOf(".");
                            arcfile.ArcFileNames.Add(Maten.EntryName.Substring(0, (dotin)));
                            Maten.BaseEntryName = Maten.EntryName.Substring(0, (dotin));
                            foldernames.Clear();
                            IDCounter++;
                            arcfile.MaterialCount++;
                            break;


                        //LMT Files.
                        case "76820D81":
                            LMTEntry LMTen = LMTEntry.FillLMTEntry(filename, foldernames, tree, br, j, IDCounter);
                            arcfile.arcfiles.Add(LMTen);
                            arcfile.FileList.Add(LMTen.EntryName);
                            dotin = LMTen.EntryName.LastIndexOf(".");
                            arcfile.ArcFileNames.Add(LMTen.EntryName.Substring(0, (dotin)));
                            LMTen.BaseEntryName = LMTen.EntryName.Substring(0, (dotin));
                            foldernames.Clear();
                            IDCounter++;
                            break;

                        //MSD Files.
                        case "5B55F5B1":
                            MSDEntry newmsden = MSDEntry.FillMSDEntry(filename, foldernames, tree, br, Bytes, j, IDCounter);
                            arcfile.arcfiles.Add(newmsden);
                            arcfile.FileList.Add(newmsden.EntryName);
                            dotin = newmsden.EntryName.LastIndexOf(".");
                            arcfile.ArcFileNames.Add(newmsden.EntryName.Substring(0, (dotin)));
                            newmsden.BaseEntryName = newmsden.EntryName.Substring(0, (dotin));
                            foldernames.Clear();
                            IDCounter++;
                            break;

                        //CST Files.
                        case "326F732E":
                            ChainListEntry CSTen = ChainListEntry.FillCSTEntry(filename, foldernames, tree, br, j, IDCounter);
                            arcfile.arcfiles.Add(CSTen);
                            arcfile.FileList.Add(CSTen.EntryName);
                            dotin = CSTen.EntryName.LastIndexOf(".");
                            arcfile.ArcFileNames.Add(CSTen.EntryName.Substring(0, (dotin)));
                            CSTen.BaseEntryName = CSTen.EntryName.Substring(0, (dotin));
                            foldernames.Clear();
                            IDCounter++;
                            break;

                        //CHN Files.
                        case "3E363245":
                            ChainEntry CHNen = ChainEntry.FillChainEntry(filename, foldernames, tree, br, j, IDCounter);
                            arcfile.arcfiles.Add(CHNen);
                            arcfile.FileList.Add(CHNen.EntryName);
                            dotin = CHNen.EntryName.LastIndexOf(".");
                            arcfile.ArcFileNames.Add(CHNen.EntryName.Substring(0, (dotin)));
                            CHNen.BaseEntryName = CHNen.EntryName.Substring(0, (dotin));
                            foldernames.Clear();
                            IDCounter++;
                            break;

                        //CCL Files.
                        case "0026E7FF":
                            ChainCollisionEntry CCLen = ChainCollisionEntry.FillChainCollEntry(filename, foldernames, tree, br, j, IDCounter);
                            arcfile.arcfiles.Add(CCLen);
                            arcfile.FileList.Add(CCLen.EntryName);
                            dotin = CCLen.EntryName.LastIndexOf(".");
                            arcfile.ArcFileNames.Add(CCLen.EntryName.Substring(0, (dotin)));
                            CCLen.BaseEntryName = CCLen.EntryName.Substring(0, (dotin));
                            foldernames.Clear();
                            IDCounter++;
                            break;

                        //MOD Files.
                        case "58A15856":
                            ModelEntry MODen = ModelEntry.FillModelEntry(filename, foldernames, tree, br, j, IDCounter);
                            arcfile.arcfiles.Add(MODen);
                            arcfile.FileList.Add(MODen.EntryName);
                            dotin = MODen.EntryName.LastIndexOf(".");
                            arcfile.ArcFileNames.Add(MODen.EntryName.Substring(0, (dotin)));
                            MODen.BaseEntryName = MODen.EntryName.Substring(0, (dotin));
                            foldernames.Clear();
                            IDCounter++;
                            break;

                        //MIS Files.
                        case "361EA2A5":
                            MissionEntry MISen = MissionEntry.FillMissionEntry(filename, foldernames, tree, br, j, IDCounter);
                            arcfile.arcfiles.Add(MISen);
                            arcfile.FileList.Add(MISen.EntryName);
                            dotin = MISen.EntryName.LastIndexOf(".");
                            arcfile.ArcFileNames.Add(MISen.EntryName.Substring(0, (dotin)));
                            MISen.BaseEntryName = MISen.EntryName.Substring(0, (dotin));
                            foldernames.Clear();
                            IDCounter++;
                            break;

                        //Gem Files.
                        case "448BBDD4":
                            GemEntry GEMen = GemEntry.FillGEMEntry(filename, foldernames, tree, br, j, IDCounter);
                            arcfile.arcfiles.Add(GEMen);
                            arcfile.FileList.Add(GEMen.EntryName);
                            dotin = GEMen.EntryName.LastIndexOf(".");
                            arcfile.ArcFileNames.Add(GEMen.EntryName.Substring(0, (dotin)));
                            GEMen.BaseEntryName = GEMen.EntryName.Substring(0, (dotin));
                            foldernames.Clear();
                            IDCounter++;
                            break;

                        //EFL Files.
                        case "6D5AE854":
                            EffectListEntry EFLen = EffectListEntry.FillEFLEntry(filename, foldernames, tree, br, j, IDCounter);
                            arcfile.arcfiles.Add(EFLen);
                            arcfile.FileList.Add(EFLen.EntryName);
                            dotin = EFLen.EntryName.LastIndexOf(".");
                            arcfile.ArcFileNames.Add(EFLen.EntryName.Substring(0, (dotin)));
                            EFLen.BaseEntryName = EFLen.EntryName.Substring(0, (dotin));
                            foldernames.Clear();
                            IDCounter++;
                            break;

                        //RIF Files.
                        case "724DF879":
                            RIFFEntry RIFen = RIFFEntry.FillRIFFEntry(filename, foldernames, tree, br, j, IDCounter);
                            arcfile.arcfiles.Add(RIFen);
                            arcfile.FileList.Add(RIFen.EntryName);
                            dotin = RIFen.EntryName.LastIndexOf(".");
                            arcfile.ArcFileNames.Add(RIFen.EntryName.Substring(0, (dotin)));
                            RIFen.BaseEntryName = RIFen.EntryName.Substring(0, (dotin));
                            foldernames.Clear();
                            IDCounter++;
                            break;

                        //ShotList Files.
                        case "141D851F":
                            ShotListEntry LSHen = ShotListEntry.FillShotListEntry(filename, foldernames, tree, br, j, IDCounter);
                            arcfile.arcfiles.Add(LSHen);
                            arcfile.FileList.Add(LSHen.EntryName);
                            dotin = LSHen.EntryName.LastIndexOf(".");
                            arcfile.ArcFileNames.Add(LSHen.EntryName.Substring(0, (dotin)));
                            LSHen.BaseEntryName = LSHen.EntryName.Substring(0, (dotin));
                            foldernames.Clear();
                            IDCounter++;
                            break;

                        //Stage Object Layout Files.
                        case "2C7171FA":
                            StageObjLayoutEntry SLOen = StageObjLayoutEntry.FillSLOEntry(filename, foldernames, tree, br, j, IDCounter);
                            arcfile.arcfiles.Add(SLOen);
                            arcfile.FileList.Add(SLOen.EntryName);
                            dotin = SLOen.EntryName.LastIndexOf(".");
                            arcfile.ArcFileNames.Add(SLOen.EntryName.Substring(0, (dotin)));
                            SLOen.BaseEntryName = SLOen.EntryName.Substring(0, (dotin));
                            foldernames.Clear();
                            IDCounter++;
                            break;

                        //STQR files.
                        case "167DBBFF":
                            STQREntry stqren = STQREntry.FillSTQREntry(filename, foldernames, tree, br, j, IDCounter);
                            arcfile.arcfiles.Add(stqren);
                            arcfile.FileList.Add(stqren.EntryName);
                            dotin = stqren.EntryName.LastIndexOf(".");
                            arcfile.ArcFileNames.Add(stqren.EntryName.Substring(0, (dotin)));
                            foldernames.Clear();
                            stqren.BaseEntryName = stqren.EntryName.Substring(0, (dotin));
                            IDCounter++;
                            break;

                        //ATI files.
                        case "227A8048":
                            AtkInfoEntry AtkInfoen = AtkInfoEntry.FillAtkInfoEntry(filename, foldernames, tree, br, j, IDCounter);
                            arcfile.arcfiles.Add(AtkInfoen);
                            arcfile.FileList.Add(AtkInfoen.EntryName);
                            dotin = AtkInfoen.EntryName.LastIndexOf(".");
                            arcfile.ArcFileNames.Add(AtkInfoen.EntryName.Substring(0, (dotin)));
                            AtkInfoen.BaseEntryName = AtkInfoen.EntryName.Substring(0, (dotin));
                            foldernames.Clear();
                            IDCounter++;
                            break;

                        //SHT files.
                        case "10BE43D4":
                            ShotEntry Shoten = ShotEntry.FillShotEntry(filename, foldernames, tree, br, j, IDCounter);
                            arcfile.arcfiles.Add(Shoten);
                            arcfile.FileList.Add(Shoten.EntryName);
                            dotin = Shoten.EntryName.LastIndexOf(".");
                            arcfile.ArcFileNames.Add(Shoten.EntryName.Substring(0, (dotin)));
                            Shoten.BaseEntryName = Shoten.EntryName.Substring(0, (dotin));
                            foldernames.Clear();
                            IDCounter++;
                            break;

                        //ANM files.
                        case "5A7E5D8A":
                            AnmCmdEntry AnmCmden = AnmCmdEntry.FillAnmCmdEntry(filename, foldernames, tree, br, j, IDCounter);
                            arcfile.arcfiles.Add(AnmCmden);
                            arcfile.FileList.Add(AnmCmden.EntryName);
                            dotin = AnmCmden.EntryName.LastIndexOf(".");
                            arcfile.ArcFileNames.Add(AnmCmden.EntryName.Substring(0, (dotin)));
                            AnmCmden.BaseEntryName = AnmCmden.EntryName.Substring(0, (dotin));
                            foldernames.Clear();
                            IDCounter++;
                            break;

                        //CBA files.
                        case "3C6EA504":
                            ChrBaseActEntry ChrBaseActen = ChrBaseActEntry.FillChrBaseActEntry(filename, foldernames, tree, br, j, IDCounter);
                            arcfile.arcfiles.Add(ChrBaseActen);
                            arcfile.FileList.Add(ChrBaseActen.EntryName);
                            dotin = ChrBaseActen.EntryName.LastIndexOf(".");
                            arcfile.ArcFileNames.Add(ChrBaseActen.EntryName.Substring(0, (dotin)));
                            ChrBaseActen.BaseEntryName = ChrBaseActen.EntryName.Substring(0, (dotin));
                            foldernames.Clear();
                            IDCounter++;
                            break;

                        //SBKR files.
                        case "15D782FB":
                            SoundBankEntry SoundBanken = SoundBankEntry.FillSoundBankEntry(filename, foldernames, tree, br, j, IDCounter);
                            arcfile.arcfiles.Add(SoundBanken);
                            arcfile.FileList.Add(SoundBanken.EntryName);
                            dotin = SoundBanken.EntryName.LastIndexOf(".");
                            arcfile.ArcFileNames.Add(SoundBanken.EntryName.Substring(0, (dotin)));
                            SoundBanken.BaseEntryName = SoundBanken.EntryName.Substring(0, (dotin));
                            foldernames.Clear();
                            IDCounter++;
                            break;

                        //SRQR files.
                        case "1BCC4966":
                            SoundRequestEntry SoundRequesten = SoundRequestEntry.FillSoundRequestEntry(filename, foldernames, tree, br, j, IDCounter);
                            arcfile.arcfiles.Add(SoundRequesten);
                            arcfile.FileList.Add(SoundRequesten.EntryName);
                            dotin = SoundRequesten.EntryName.LastIndexOf(".");
                            arcfile.ArcFileNames.Add(SoundRequesten.EntryName.Substring(0, (dotin)));
                            SoundRequesten.BaseEntryName = SoundRequesten.EntryName.Substring(0, (dotin));
                            foldernames.Clear();
                            IDCounter++;
                            break;

                        //New Formats go like this!   
                        /*
                        case "********":
                           *****Entry ****en = *****Entry.Fill*****Entry(filename, foldernames, tree, br, j, IDCounter);
                           arcfile.arcfiles.Add(*****en);
                           arcfile.FileList.Add(*****.EntryName);
                           foldernames.Clear();
                           IDCounter++;
                           break;
                        */

                        default:
                            //Everything not listed above.
                            ArcEntry newentry = ArcEntry.FillArcEntry(filename, foldernames, tree, br, j, IDCounter);
                            arcfile.arcfiles.Add(newentry);
                            arcfile.FileList.Add(newentry.EntryName);
                            dotin = newentry.EntryName.LastIndexOf(".");
                            arcfile.ArcFileNames.Add(newentry.EntryName.Substring(0, (dotin)));
                            newentry.BaseEntryName = newentry.EntryName.Substring(0, (dotin));
                            foldernames.Clear();
                            IDCounter++;
                            break;
                    }
                }

                arcfile._FileAmount = Convert.ToUInt16(IDCounter);

                br.Close();
            }

            return arcfile;

            #endregion

        }

        public static ArcFile SyncMaterialNames(ArcFile archive, Type filetype = null)
        {

            List<int> IndicesOfNote = new List<int>();
            string FindThis = "mrl";
            string searchline = "";
            List<string> MatFileNames = new List<string>();
            for (int i = 0; i < archive.FileList.Count; i++)
            {

                searchline = archive.FileList[i].Substring(archive.FileList[i].Length - 3);
                if (searchline == FindThis)
                {
                    MatFileNames.Add(archive.FileList[i]);
                    IndicesOfNote.Add(i);
                }

            }



            return archive;
        }

        public static string BytesToString(byte[] bytes)
        {
            if (SBname == null)
            {
                SBname = new StringBuilder();
            }
            else
            {
                SBname.Clear();
            }

            string Tempname;
            ASCIIEncoding ascii = new ASCIIEncoding();
            Tempname = ascii.GetString(bytes);
            //Tempname = Tempname.Replace(@"\\", @"\");
            return Tempname;
        }

        public static string HashBytesToString(byte[] bytes)
        {
            string temps;
            string tru = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                temps = bytes[i].ToString("X");
                if (temps == "0")
                {
                    temps = "00";
                }
                else if (temps == "1")
                {
                    temps = "01";
                }
                else if (temps == "2")
                {
                    temps = "02";
                }
                else if (temps == "3")
                {
                    temps = "03";
                }
                else if (temps == "4")
                {
                    temps = "04";
                }
                else if (temps == "5")
                {
                    temps = "05";
                }
                else if (temps == "6")
                {
                    temps = "06";
                }
                else if (temps == "7")
                {
                    temps = "07";
                }
                else if (temps == "8")
                {
                    temps = "08";
                }
                else if (temps == "9")
                {
                    temps = "09";
                }
                else if (temps == "A")
                {
                    temps = "0A";
                }
                else if (temps == "B")
                {
                    temps = "0B";
                }
                else if (temps == "C")
                {
                    temps = "0C";
                }
                else if (temps == "D")
                {
                    temps = "0D";
                }
                else if (temps == "E")
                {
                    temps = "0E";
                }
                else if (temps == "F")
                {
                    temps = "0F";
                }
                tru += temps;
            }
            return tru;
        }

        #region Arc Class
        //Affects what you see in the Property Grid, grdMain.

        private UInt16 _FileAmount;
        [Category("MT ARC Entry"), ReadOnlyAttribute(true)]
        public UInt16 FileAmount
        {

            get
            {
                return _FileAmount;
            }
            set
            {
                _FileAmount = value;
            }
        }

        private UInt16 _Version;
        [Category("MT ARC Entry"), ReadOnlyAttribute(true)]
        public UInt16 Version
        {

            get
            {
                return _Version;
            }
            set
            {
                _Version = value;
            }
        }

        private long _FileLength;
        [Category("MT ARC Entry"), ReadOnlyAttribute(true)]
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

        #endregion

    }
}
