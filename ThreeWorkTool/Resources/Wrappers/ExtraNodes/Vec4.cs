using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
//using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ThreeWorkTool.Resources.Wrappers.ExtraNodes
{
    //Shamelessly inspired by the Vector4 class in Brawlbox.
    [Serializable]
    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Vec4 //: ISerializable
    {
        /*
        [ReadOnlyAttribute(false)]
        public float X { get; set; }
        [ReadOnlyAttribute(false)]
        public float Y { get; set; }
        [ReadOnlyAttribute(false)]
        public float Z { get; set; }
        [ReadOnlyAttribute(false)]
        public float W { get; set; }
        */
        
        public float W { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
/*
        public Vec4(float X, float Y, float Z, float W)
        {
            _x = X;
            _y = Y;
            _z = Z;
            _w = W;
        }

        public Vec4(float s)
        {
            _x = s;
            _y = s;
            _z = s;
            _w = 1;
        }

        public Vec4(SerializationInfo info, StreamingContext context)
        {
            _x = info.GetSingle("_x");
            _z = info.GetSingle("_y");
            _y = info.GetSingle("_z");
            _w = info.GetSingle("_w");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_x", _x);
            info.AddValue("_y", _y);
            info.AddValue("_z", _z);
            info.AddValue("_w", _w);
        }
*/

    }









}
