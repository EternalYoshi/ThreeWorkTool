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

    }
}
