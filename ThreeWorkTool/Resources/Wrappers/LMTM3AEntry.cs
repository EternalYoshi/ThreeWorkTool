using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeWorkTool.Resources.Utility;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class LMTM3AEntry
    {
        public byte[] RawData;
        public int AnimationID;
        public string FileName;
        public string ShortName;
        public int FrameCount;
        public int IndexRows;
        public int AnimStart;
        public int AnimEnd;
        public string UnknownValue10;
        public double UnknownFloat3C;
        public int UnknownValue40;

        public LMTM3AEntry FillM3AProprties(LMTM3AEntry Anim, MemoryStream ms, int datalength, int ID, int RowTotal, int SecondOffset, BinaryReader bnr, int SecondaryCount, LMTEntry lmtentry)
        {
            LMTM3AEntry M3a = new LMTM3AEntry();

            bnr.BaseStream.Position = lmtentry.OffsetList[1];
            M3a.AnimStart = bnr.ReadInt32();
            bnr.BaseStream.Position = bnr.BaseStream.Position + 4;
            M3a.IndexRows = bnr.ReadInt32();
            M3a.FrameCount = bnr.ReadInt32();
            M3a._FrameTotal = M3a.FrameCount;
            M3a.UnknownValue10 = ByteUtilitarian.BytesToString(bnr.ReadBytes(4),M3a.UnknownValue10);
            bnr.BaseStream.Position = bnr.BaseStream.Position + 40;
            M3a.UnknownFloat3C = bnr.ReadSingle();
            M3a.UnknownValue40 = bnr.ReadInt32();
            bnr.BaseStream.Position = bnr.BaseStream.Position + 4;
            M3a.AnimEnd = bnr.ReadInt32();

            //Ported from Lean's thing. For reference only really.
            /*
            M3a.RawData = new byte[datalength];
            bnr.Read(M3a.RawData, 0, datalength);
            M3a.AnimationID = ID;
            M3a._MotionID = ID;
            M3a._FileType = ".m3a";
            M3a._FileLength = M3a.RawData.Length;

            int ValorA = 0;
            int ValorB = 0;
            int ValorC = 0;
            int ValorD = 0;
            int ValorL = 0;
            int ValorR = 0;
            int SecondCount = SecondaryCount;
            using (MemoryStream msm3a = new MemoryStream(M3a.RawData))
            {
                using (BinaryReader brm3a = new BinaryReader(msm3a))
                {
                    using (BinaryWriter bwm3a = new BinaryWriter(msm3a))
                    {
                        while (SecondCount < RowTotal)
                        {

                            brm3a.BaseStream.Position = (16 + (48 * SecondaryCount));
                            ValorA = brm3a.ReadInt32();

                            if (ValorA > 0)
                            {
                                ValorB = ValorA - SecondOffset;
                                brm3a.BaseStream.Position = (16 + (48 * SecondaryCount));
                            }

                            brm3a.BaseStream.Position = (40 + (48 * SecondaryCount));
                            ValorC = brm3a.ReadInt32();

                            if (ValorC > 0)
                            {
                                ValorD = ValorC - SecondOffset;
                                brm3a.BaseStream.Position = (40 + (48 * SecondaryCount));
                                ValorD = brm3a.ReadInt32();
                            }

                            SecondCount++;

                        }

                        brm3a.BaseStream.Position = brm3a.BaseStream.Length - 280;
                        ValorR = brm3a.ReadInt32();
                        ValorL = ValorR - SecondOffset;
                        brm3a.BaseStream.Position = brm3a.BaseStream.Length - 280;
                        bwm3a.BaseStream.Position = brm3a.BaseStream.Length - 280;
                        bwm3a.Write(ValorL);

                        brm3a.BaseStream.Position = brm3a.BaseStream.Length - 200;
                        ValorR = brm3a.ReadInt32();
                        ValorL = ValorR - SecondOffset;
                        brm3a.BaseStream.Position = brm3a.BaseStream.Length - 200;
                        bwm3a.BaseStream.Position = bwm3a.BaseStream.Length - 200;
                        bwm3a.Write(ValorL);

                        brm3a.BaseStream.Position = brm3a.BaseStream.Length - 120;
                        ValorR = brm3a.ReadInt32();
                        ValorL = ValorR - SecondOffset;
                        brm3a.BaseStream.Position = brm3a.BaseStream.Length - 120;
                        bwm3a.BaseStream.Position = bwm3a.BaseStream.Length - 120;
                        bwm3a.Write(ValorL);

                        brm3a.BaseStream.Position = brm3a.BaseStream.Length - 40;
                        ValorR = brm3a.ReadInt32();
                        ValorL = ValorR - SecondOffset;
                        brm3a.BaseStream.Position = brm3a.BaseStream.Length - 40;
                        bwm3a.BaseStream.Position = bwm3a.BaseStream.Length - 40;
                        bwm3a.Write(ValorL);

                        brm3a.BaseStream.Position = 0;
                        bwm3a.BaseStream.Position = 0;

                    }
                }
            }

            M3a.FileName = "AnimationID" + M3a.AnimationID + ".m3a";
            M3a.ShortName = "AnimationID" + M3a.AnimationID;

            Anim = M3a;
            */

            return Anim;
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

        #endregion
    }
}
