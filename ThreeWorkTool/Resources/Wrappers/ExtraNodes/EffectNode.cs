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
using ThreeWorkTool.Resources.Utility;
using ThreeWorkTool.Resources.Archives;

namespace ThreeWorkTool.Resources.Wrappers.ExtraNodes
{
    public class EffectNode : DefaultWrapper
    {

        public struct Field
        {
            public int EntryType;
            public int OffsetType;
            public int Offset;
            public int type;
            public byte[] buffer;
            public string FileRef;

        }
        public List<Field> Fields;

        public struct Buffer
        {
            public int pointer;
            public List<int> Offsets;

        }

        public static EffectNode BuildEffect(EffectNode Effect, int ID, BinaryReader bnr, EffectListEntry efl)
        {

            int Temp = 0;
            Temp = bnr.ReadInt32();



            return Effect;

        }


    }
}
