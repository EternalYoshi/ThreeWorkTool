using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Utility;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class LMTM3AEntry
    {
        public byte[] FullData;
        public byte[] RawData;
        public byte[] BlockData;
        public int AnimationID;
        public string FileName;
        public string ShortName;
        public int FrameCount;
        public int IndexRows;
        public int AnimStart;
        public int AnimEnd;
        public string UnknownValue10;
        public string UnknownValue14;
        public string UnknownValue18;
        public string UnknownValue1C;
        public string UnknownValue20;
        public string UnknownValue24;
        public string UnknownValue28;
        public string UnknownValue2C;
        public string UnknownValue30;
        public string UnknownValue34;
        public string UnknownValue38;
        public double UnknownFloat3C;
        public int UnknownValue40;
        public int UnknownValue44;
        public int AnimDataSize;
        public string FileExt;

        public LMTM3AEntry FillM3AProprties(LMTM3AEntry Anim, int datalength, int ID, int RowTotal, int SecondOffset, BinaryReader bnr, int SecondaryCount, LMTEntry lmtentry)
        {
            LMTM3AEntry M3a = new LMTM3AEntry();
            M3a._FileType = ".m3a";
            M3a.FileExt = M3a._FileType;
            bnr.BaseStream.Position = lmtentry.OffsetList[ID];
            M3a.AnimStart = bnr.ReadInt32();
            bnr.BaseStream.Position = bnr.BaseStream.Position + 4;
            M3a.IndexRows = bnr.ReadInt32();
            M3a.FrameCount = bnr.ReadInt32();
            M3a._FrameTotal = M3a.FrameCount;
            M3a.UnknownValue10 = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), M3a.UnknownValue10);
            M3a.UnknownValue14 = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), M3a.UnknownValue14);
            M3a.UnknownValue18 = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), M3a.UnknownValue18);
            M3a.UnknownValue1C = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), M3a.UnknownValue1C);
            M3a.UnknownValue20 = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), M3a.UnknownValue20);
            M3a.UnknownValue24 = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), M3a.UnknownValue24);
            M3a.UnknownValue28 = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), M3a.UnknownValue28);
            M3a.UnknownValue2C = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), M3a.UnknownValue2C);
            M3a.UnknownValue30 = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), M3a.UnknownValue30);
            M3a.UnknownValue34 = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), M3a.UnknownValue34);
            M3a.UnknownValue38 = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), M3a.UnknownValue38);
            M3a.UnknownFloat3C = bnr.ReadSingle();
            M3a.UnknownValue40 = bnr.ReadInt32();
            M3a.UnknownValue44 = bnr.ReadInt32();
            M3a.AnimEnd = bnr.ReadInt32();
            M3a.AnimDataSize = (M3a.AnimEnd - M3a.AnimStart) + 352;
            bnr.BaseStream.Position = M3a.AnimStart;
            M3a.RawData = new byte[M3a.AnimDataSize];
            M3a.RawData = bnr.ReadBytes(M3a.AnimDataSize);
            M3a.BlockData = new byte[80];
            bnr.BaseStream.Position = lmtentry.OffsetList[ID];
            M3a.BlockData = bnr.ReadBytes(80);
            M3a._MotionID = ID;
            using (MemoryStream msbd3a = new MemoryStream(M3a.BlockData))
            {
                using (BinaryWriter bwbd3a = new BinaryWriter(msbd3a))
                {
                    int z = 0;
                    bwbd3a.BaseStream.Position = 0;
                    bwbd3a.Write(z);
                    bwbd3a.BaseStream.Position = 72;
                    bwbd3a.Write(z);
                }
            }

            M3a.AnimationID = ID;
            M3a.FileName = "AnimationID" + M3a.AnimationID + ".m3a";
            M3a.ShortName = "AnimationID" + M3a.AnimationID;
            M3a._IsBlank = false;
            Anim = M3a;

            //Subtracts pointers in there by the data offset to get their base value.
            int OffTemp = 0;
            using (MemoryStream msm3a = new MemoryStream(M3a.RawData))
            {

                using (BinaryReader brm3a = new BinaryReader(msm3a))
                {
                    using (BinaryWriter bwm3a = new BinaryWriter(msm3a))
                    {

                        //Adjusts the offsets in the Rawdata of the m3a.
                        bwm3a.BaseStream.Position = 0;

                        for (int y = 0; y < M3a.IndexRows; y++)
                        {
                            bwm3a.BaseStream.Position = 0;
                            bwm3a.BaseStream.Position = 16 + (48 * y);
                            OffTemp = brm3a.ReadInt32();
                            bwm3a.BaseStream.Position = (bwm3a.BaseStream.Position - 4);
                            if (OffTemp > 0)
                            {
                                OffTemp = OffTemp - M3a.AnimStart;
                                bwm3a.Write(OffTemp);
                            }
                            bwm3a.BaseStream.Position = 40 + (48 * y);
                            OffTemp = brm3a.ReadInt32();
                            bwm3a.BaseStream.Position = (bwm3a.BaseStream.Position - 4);
                            if (OffTemp > 0)
                            {
                                OffTemp = OffTemp - M3a.AnimStart;
                                bwm3a.Write(OffTemp);
                            }

                        }

                        //Adjusts the offsets in the footer.
                        bwm3a.BaseStream.Position = (bwm3a.BaseStream.Length - 280);
                        //OffTemp = brm3a.ReadInt32();
                        OffTemp = M3a.RawData.Length - 32;
                        //bwm3a.BaseStream.Position = (bwm3a.BaseStream.Position - 4);
                        bwm3a.Write(OffTemp);
                        bwm3a.BaseStream.Position = bwm3a.BaseStream.Position + 76;
                        //OffTemp = brm3a.ReadInt32();
                        OffTemp = M3a.RawData.Length - 24;
                        //bwm3a.BaseStream.Position = (bwm3a.BaseStream.Position - 4);
                        bwm3a.Write(OffTemp);
                        bwm3a.BaseStream.Position = bwm3a.BaseStream.Position + 76;
                        //OffTemp = brm3a.ReadInt32();
                        OffTemp = M3a.RawData.Length - 16;
                        //bwm3a.BaseStream.Position = (bwm3a.BaseStream.Position - 4);
                        bwm3a.Write(OffTemp);
                        bwm3a.BaseStream.Position = bwm3a.BaseStream.Position + 76;
                        //OffTemp = brm3a.ReadInt32();
                        OffTemp = M3a.RawData.Length - 8;
                        //bwm3a.BaseStream.Position = (bwm3a.BaseStream.Position - 4);
                        bwm3a.Write(OffTemp);

                    }
                }

            }

            //Appends the Animation Block Data to the FullData.
            M3a.FullData = new byte[(M3a.AnimDataSize + 80)];
            M3a._FileLength = M3a.FullData.LongLength;
            Array.Copy(M3a.RawData, 0, M3a.FullData, 0, M3a.RawData.Length);
            Array.Copy(M3a.BlockData, 0, M3a.FullData, M3a.RawData.Length, M3a.BlockData.Length);

            return Anim;
        }

        public LMTM3AEntry FillBlankM3A(LMTM3AEntry Anim, int datalength, int ID, int RowTotal, int SecondOffset, BinaryReader bnr, int SecondaryCount, LMTEntry lmtentry)
        {
            LMTM3AEntry M3a = new LMTM3AEntry();
            M3a._FileType = ".m3a";
            M3a.FileExt = ".m3a";
            M3a.AnimStart = -1;
            M3a.IndexRows = -1;
            M3a.FrameCount = -1;
            M3a._FrameTotal = -1;
            M3a.UnknownValue10 = "N/A";
            M3a.UnknownFloat3C = -1.0;
            M3a.UnknownValue40 = -1;
            M3a.AnimEnd = -1;
            M3a.AnimDataSize = -1;
            M3a.AnimationID = ID;
            M3a.FileName = "AnimationID" + M3a.AnimationID + ".m3a";
            M3a.ShortName = "AnimationID" + M3a.AnimationID;
            M3a.RawData = new byte[1];
            M3a.RawData[0] = 0x00;
            M3a.BlockData = new byte[1];
            M3a.BlockData[0] = 0x00;
            M3a.FullData = new byte[1];
            M3a.FullData[0] = 0x00;
            M3a.IsBlank = true;
            M3a._MotionID = ID;
            Anim = M3a;
            return Anim;
        }

        public static LMTM3AEntry ReplaceLMTM3AEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            LMTM3AEntry m3aentry = new LMTM3AEntry();
            LMTM3AEntry oldentry = new LMTM3AEntry();

            tree.BeginUpdate();

            var tag = node.Tag;
            if (tag is LMTM3AEntry)
            {
                oldentry = tag as LMTM3AEntry;
            }

            //Builds the ma3entry. FInish This when you see it please.
            m3aentry._FileType = "m3a";
            m3aentry.FileExt = m3aentry._FileType;
            m3aentry.FullData = System.IO.File.ReadAllBytes(filename);

            using (MemoryStream MAThreeStream = new MemoryStream(m3aentry.FullData))
            {
                using (BinaryReader bnr = new BinaryReader(MAThreeStream))
                {
                    if (bnr.BaseStream.Length < 5)
                    {
                        MessageBox.Show("The entry you are trying to import is a blank one,\nso the replace command has been aborted.","We have a problem here.");
                        return null;
                    }
                    else
                    {
                        int projdatlength = m3aentry.FullData.Length - 80;
                        m3aentry.RawData = new byte[(projdatlength)];
                        Array.Copy(m3aentry.FullData, 0, m3aentry.RawData, 0, projdatlength);
                        m3aentry.BlockData = new byte[80];
                        projdatlength = m3aentry.FullData.Length - 80;
                        Array.Copy(m3aentry.FullData, projdatlength, m3aentry.BlockData, 0, 80);
                        bnr.BaseStream.Position = 0;

                        bnr.BaseStream.Position = (m3aentry.FullData.Length - 80);

                        m3aentry.AnimStart = bnr.ReadInt32();
                        bnr.BaseStream.Position = bnr.BaseStream.Position + 4;
                        m3aentry.IndexRows = bnr.ReadInt32();
                        m3aentry.FrameCount = bnr.ReadInt32();
                        m3aentry._FrameTotal = m3aentry.FrameCount;
                        m3aentry.IsBlank = false;
                        m3aentry.UnknownValue10 = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), m3aentry.UnknownValue10);
                        m3aentry.UnknownValue14 = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), m3aentry.UnknownValue14);
                        m3aentry.UnknownValue18 = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), m3aentry.UnknownValue18);
                        m3aentry.UnknownValue1C = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), m3aentry.UnknownValue1C);
                        m3aentry.UnknownValue20 = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), m3aentry.UnknownValue20);
                        m3aentry.UnknownValue24 = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), m3aentry.UnknownValue24);
                        m3aentry.UnknownValue28 = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), m3aentry.UnknownValue28);
                        m3aentry.UnknownValue2C = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), m3aentry.UnknownValue2C);
                        m3aentry.UnknownValue30 = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), m3aentry.UnknownValue30);
                        m3aentry.UnknownValue34 = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), m3aentry.UnknownValue34);
                        m3aentry.UnknownValue38 = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), m3aentry.UnknownValue38);
                        m3aentry.UnknownFloat3C = bnr.ReadSingle();
                        m3aentry.UnknownValue40 = bnr.ReadInt32();
                        m3aentry.UnknownValue44 = bnr.ReadInt32();
                        m3aentry.AnimEnd = bnr.ReadInt32();
                        m3aentry.AnimDataSize = m3aentry.RawData.Length;
                        m3aentry.AnimationID = oldentry.AnimationID;
                    }
                }
            }

            try
            {
                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
                {

                    m3aentry.RawData = System.IO.File.ReadAllBytes(filename);


                    /*
                    var tag = node.Tag;
                    if (tag is LMTM3AEntry)
                    {
                        oldentry = tag as LMTM3AEntry;
                    }
                    */

                    tag = m3aentry;

                    if (node.Tag is LMTM3AEntry)
                    {
                        node.Tag = m3aentry;
                    }

                    var aew = node as ArcEntryWrapper;

                    string type = node.GetType().ToString();
                    if (type == "ThreeWorkTool.Resources.Wrappers.ArcEntryWrapper")
                    {
                        aew.entryfile = m3aentry;
                    }

                    node = aew;
                    node.entryfile = m3aentry;
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



            return node.entryfile as LMTM3AEntry;
        }



        #region MA3Entry Properties

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

        private long _MotionID;
        [Category("MT ARC Entry"), ReadOnlyAttribute(true)]
        public long MotionID
        {

            get
            {
                return _MotionID;
            }
            set
            {
                _MotionID = value;
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

        private long _FrameTotal;
        [Category("MT ARC Entry"), ReadOnlyAttribute(true)]
        public long FrameTotal
        {

            get
            {
                return _FrameTotal;
            }
            set
            {
                _FrameTotal = value;
            }

        }

        [Category("MT ARC Entry"), ReadOnlyAttribute(true)]
        public int IndexRowTotal
        {

            get
            {
                //return _IndexRowTotal;
                return IndexRows;
            }
            set
            {
                //_IndexRowTotal = value;
                IndexRows = value;

            }

        }

        private bool _IsBlank;
        [Category("MT ARC Entry"), ReadOnlyAttribute(true)]
        public bool IsBlank
        {

            get
            {
                return _IsBlank;
            }
            set
            {
                _IsBlank = value;
            }

        }

        #endregion
    }
}
