using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeWorkTool.Resources.Utility
{
    public class ENumerators
    {
        public enum IMatType
        {
            flag = 0,
            cbuffer = 1,
            samplerstate = 2,
            texture = 3
        };
        //enum IMatType {flag, cbuffer, samplerstate, texture};

        public enum ETrackType
        {
            localrotation = 0,
            localposition = 1,
            localscale = 2,
            absoluterotation = 3,
            absoluteposition = 4,
            xpto = 5
        };

        public enum ShaderObjectInfo
        {


        };

        public enum KnownExtensions
        {
            tex = 0,
            ean = 1,
            mrl = 2,
            mod = 3,
            chn = 4,
            ccl = 5,
            cst = 6,
            lmt = 7,
            anm = 8,
            ati = 9,
            cli = 10,
            cba = 11,
            csp = 12,
            ccm = 13,
            chs = 14,
            cpi = 15,
            cpu = 16,
            sht = 17,
            lmcm = 18,
            fsm = 19,
            slo = 20,
            gem = 21,
            lsh = 22,
            efl = 23,
            sdl = 24,
            rpl = 25,
            lrp = 26,
            xsew = 27,
            sbkr = 28,
            srqr = 29,
            stqr = 30
        };

    }
}
