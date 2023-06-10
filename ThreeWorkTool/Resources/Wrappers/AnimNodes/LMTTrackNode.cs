using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ThreeWorkTool.Resources.Archives;

namespace ThreeWorkTool.Resources.Wrappers.AnimNodes
{
    public class LMTTrackNode : DefaultWrapper
    {

        public int TrackNumber;
        public int BufferType;
        public string BufferKind, TrackKind;
        public int TrackType;
        public int BoneType;
        public int BoneID;
        public float Weight;
        public int BufferSize;
        public int BufferPointer;
        public Vector4 ReferenceData;
        public float ExtremesPointer;
        public byte[] Buffer;
        public float[] ExtremesArray;
        public int ReferenceDataPointer;
        public string ReferenceDataStr { get; set; }
        public class Extremes
        {
            public Vector4 min;
            public Vector4 max;
        }

        public Extremes Extreme;

        public static LMTTrackNode SetString(LMTTrackNode Track)
        {

            Track.ReferenceDataStr = "(" + Track.ReferenceData.W + "," + Track.ReferenceData.X + "," + Track.ReferenceData.Y + "," + Track.ReferenceData.Z + ")";


            return Track;
        }

        [Category("Motion"), ReadOnlyAttribute(true)]
        public int RefDataPointer
        {
            
            get
            {
                //return _IndexRowTotal;
                return ReferenceDataPointer;
            }
            set
            {
                //_IndexRowTotal = value;
                ReferenceDataPointer = value;

            }

        }

    }


}
