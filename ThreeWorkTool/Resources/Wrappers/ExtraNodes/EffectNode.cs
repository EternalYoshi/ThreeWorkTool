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
            public List<Buffer> FieldBuffer;
            public byte[] RawBuffer;
            public List<string> FileRef;

        }
        public List<Field> Fields;

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

            Effect.Fields = new List<Field>();
            Field fld = new Field();
            fld.FileRef = new List<string>();
            fld.OffsetType = Temp;
            fld.Offset = Temp >> 8;
            fld.Type = Temp & 0xFF;
            OffsetTemp = Convert.ToInt32(bnr.BaseStream.Position);
            fld.EntryType = ID;
            //string TempStr = "";
            byte[] PLName = new byte[] { };
            byte[] PTHName = new byte[] { };


            if (fld.Offset != 0)
            {

                bnr.BaseStream.Position = (fld.Offset + 0x30);
                fld.FieldBuffer = new List<Buffer>();
                Buffer buffer = new Buffer();
                buffer.Offsets = new List<int>();
                string Teme = "";

                switch (fld.EntryType)
                {

                    case 0:
                        break;

                    case 1:

                        bnr.BaseStream.Position = fld.Offset + 0x30 + 0x70;
                        buffer.Offsets.Add(Convert.ToInt32(bnr.BaseStream.Position));

                        PLName = efl.UncompressedData.Skip(Convert.ToInt32(bnr.BaseStream.Position)).Take(64).Where(x => x != 0x00).ToArray();
                        Teme = ascii.GetString(PLName);
                        fld.FileRef.Add(Teme);

                        bnr.BaseStream.Position = bnr.BaseStream.Position + 0x40;

                        PLName = efl.UncompressedData.Skip(Convert.ToInt32(bnr.BaseStream.Position)).Take(64).Where(x => x != 0x00).ToArray();
                        Teme = ascii.GetString(PLName);
                        fld.FileRef.Add(Teme);

                        bnr.BaseStream.Position = bnr.BaseStream.Position + 0x40;

                        PLName = efl.UncompressedData.Skip(Convert.ToInt32(bnr.BaseStream.Position)).Take(64).Where(x => x != 0x00).ToArray();
                        Teme = ascii.GetString(PLName);
                        fld.FileRef.Add(Teme);

                        bnr.BaseStream.Position = bnr.BaseStream.Position + 0x40;

                        PLName = efl.UncompressedData.Skip(Convert.ToInt32(bnr.BaseStream.Position)).Take(64).Where(x => x != 0x00).ToArray();
                        Teme = ascii.GetString(PLName);
                        fld.FileRef.Add(Teme);


                        break;

                    case 2:
                        break;

                    default:
                        break;
                }

            }

            return Effect;

        }


        [Category("Effect List"), ReadOnlyAttribute(true)]
        public Field Field1
        {

            get
            {
                return Fields[0];
            }
            set
            {
                Fields[0] = value;
            }
        }

        
        [Category("Effect List"), ReadOnlyAttribute(true)]
        public Field Field2
        {

            get
            {
                return Fields[1];
            }
            set
            {
                Fields[1] = value;
            }
        }

        [Category("Effect List"), ReadOnlyAttribute(true)]
        public Field Field3
        {

            get
            {
                return Fields[2];
            }
            set
            {
                Fields[2] = value;
            }
        }

        [Category("Effect List"), ReadOnlyAttribute(true)]
        public Field Field4
        {

            get
            {
                return Fields[3];
            }
            set
            {
                Fields[3] = value;
            }
        }
        

    }
}
