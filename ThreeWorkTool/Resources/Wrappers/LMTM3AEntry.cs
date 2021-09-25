using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class LMTM3AEntry
    {
        public byte[] RawData;
        public int PossibleFrameCount;
        public int AnimationID;

        public LMTM3AEntry FillM3AProprties(LMTM3AEntry Anim, MemoryStream ms, int datalength, int ID, int RowTotal)
        {
            LMTM3AEntry M3a = new LMTM3AEntry();
            M3a.RawData = new byte[datalength];
            ms.Read(M3a.RawData,0,datalength);
            M3a.AnimationID = ID;

            int SecondaryCount = 0;
            int ValorA = 0;
            int ValorB = 0;
            int ValorC = 0;
            byte[] Sectemp = new byte[4];
            
            using (MemoryStream ms2 = new MemoryStream())
            {
                while (SecondaryCount < RowTotal)
                {
                    ms2.Position = (16 + (48 * SecondaryCount));
                    ValorA = ms2.Read(Sectemp,0,4);




                    SecondaryCount = SecondaryCount + 1;
                }
            }

            return M3a;
        }

    }
}
