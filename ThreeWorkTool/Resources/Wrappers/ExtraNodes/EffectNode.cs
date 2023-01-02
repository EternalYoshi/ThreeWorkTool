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
            public int Type;
            public Buffer buffer;
            public int BufferOffset;
            public byte[] RawBuffer;
            public List<string> FileRef;
        }
        public Field[] Fields;
        public List<EffectFieldTextureRefernce> FXTXNameRefs;
        public List<int> FieldTextOffsets;
        public Buffer FBuffer;

        public struct Buffer
        {
            public int pointer;
            public List<int> Offsets;
            public int Type;
        }

        public static EffectNode BuildEffect(EffectNode Effect, int ID, BinaryReader bnr, EffectListEntry efl, int PrevOffset)
        {
            int OffsetTemp = 0;
            int Temp = 0;
            Temp = bnr.ReadInt32();

            ASCIIEncoding ascii = new ASCIIEncoding();
            int OffTemp;
            Effect.Fields = new Field[4];
            Effect.FieldTextOffsets = new List<int>();
            Effect.FXTXNameRefs = new List<EffectFieldTextureRefernce>();

            //First Field set.
            Field fld = new Field();
            fld.FileRef = new List<string>();
            fld.OffsetType = Temp;
            fld.Offset = Temp >> 8;
            fld.Type = Temp & 0xFF;
            OffsetTemp = Convert.ToInt32(bnr.BaseStream.Position);
            fld.EntryType = 0;
            Effect.Fields[0]=fld;

            //Second Field set.
            Temp = bnr.ReadInt32();
            fld = new Field();
            fld.FileRef = new List<string>();
            fld.OffsetType = Temp;
            fld.Offset = Temp >> 8;
            fld.Type = Temp & 0xFF;
            OffsetTemp = Convert.ToInt32(bnr.BaseStream.Position);
            fld.EntryType = 1;
            Effect.Fields[1] = fld;

            //Third Field set.
            Temp = bnr.ReadInt32();
            fld = new Field();
            fld.FileRef = new List<string>();
            fld.OffsetType = Temp;
            fld.Offset = Temp >> 8;
            fld.Type = Temp & 0xFF;
            OffsetTemp = Convert.ToInt32(bnr.BaseStream.Position);
            fld.EntryType = 2;
            Effect.Fields[2] = fld;

            //Fourth Field set.
            Temp = bnr.ReadInt32();
            fld = new Field();
            fld.FileRef = new List<string>();
            fld.OffsetType = Temp;
            fld.Offset = Temp >> 8;
            fld.Type = Temp & 0xFF;
            OffsetTemp = Convert.ToInt32(bnr.BaseStream.Position);
            fld.EntryType = 3;
            Effect.Fields[3] = fld;

            Effect.FBuffer = new Buffer();

            return Effect;

        }

        public static EffectNode BuildEffectPartTwo(EffectNode Effect, BinaryReader bnr, EffectListEntry efl, int PrevOffset)
        {

            //Effect.FBuffer = new Buffer();

            Effect.Fields[0].BufferOffset = Effect.Fields[0].Offset + 0x30;

            Effect.Fields[1].BufferOffset = Effect.Fields[1].Offset + 0x30;

            Effect.Fields[3].BufferOffset = Effect.Fields[3].Offset + 0x30;


            //string TempStr = "";
            byte[] PLName = new byte[] { };
            byte[] PTHName = new byte[] { };

            return Effect;

        }


        [Category("Effect"), ReadOnlyAttribute(true)]
        public Field[] FieldList
        {

            get
            {
                return Fields;
            }
            set
            {
                Fields = value;
            }
        }





    }
}
