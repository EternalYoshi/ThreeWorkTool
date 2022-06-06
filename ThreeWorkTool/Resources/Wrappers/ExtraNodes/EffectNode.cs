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
        public List<EffectFieldTextureRefernce> FXTXNameRefs;
        public List<int> FieldTextOffsets;

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
            Effect.Fields = new List<Field>();
            Effect.FieldTextOffsets = new List<int>();
            Effect.FXTXNameRefs = new List<EffectFieldTextureRefernce>();
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

                bnr.BaseStream.Position = fld.Offset + 0x30 + 0x70;
                EffectFieldTextureRefernce FXTRef = new EffectFieldTextureRefernce();
                OffTemp = Convert.ToInt32(bnr.BaseStream.Position);
                buffer.Offsets.Add(OffTemp);
                Effect.FieldTextOffsets.Add(OffTemp);
                FXTRef.Offset = OffTemp;
                PLName = efl.UncompressedData.Skip(Convert.ToInt32(bnr.BaseStream.Position)).Take(64).Where(x => x != 0x00).ToArray();
                Teme = ascii.GetString(PLName);
                FXTRef.TextureName = Teme;
                fld.FileRef.Add(Teme);
                OffTemp = 0;
                Effect.FXTXNameRefs.Add(FXTRef);

                bnr.BaseStream.Position = bnr.BaseStream.Position + 0x40;
                EffectFieldTextureRefernce FXTRefTwo = new EffectFieldTextureRefernce();

                OffTemp = Convert.ToInt32(bnr.BaseStream.Position);
                Effect.FieldTextOffsets.Add(OffTemp);
                FXTRefTwo.Offset = OffTemp;
                PLName = efl.UncompressedData.Skip(Convert.ToInt32(bnr.BaseStream.Position)).Take(64).Where(x => x != 0x00).ToArray();
                Teme = ascii.GetString(PLName);
                FXTRefTwo.TextureName = Teme;
                fld.FileRef.Add(Teme);
                OffTemp = 0;
                Effect.FXTXNameRefs.Add(FXTRefTwo);


                bnr.BaseStream.Position = bnr.BaseStream.Position + 0x40;
                EffectFieldTextureRefernce FXTRefThree = new EffectFieldTextureRefernce();
                OffTemp = Convert.ToInt32(bnr.BaseStream.Position);
                Effect.FieldTextOffsets.Add(OffTemp);
                FXTRefThree.Offset = OffTemp;
                PLName = efl.UncompressedData.Skip(Convert.ToInt32(bnr.BaseStream.Position)).Take(64).Where(x => x != 0x00).ToArray();
                Teme = ascii.GetString(PLName);
                FXTRefThree.TextureName = Teme;
                fld.FileRef.Add(Teme);
                OffTemp = 0;
                Effect.FXTXNameRefs.Add(FXTRefThree);


                bnr.BaseStream.Position = bnr.BaseStream.Position + 0x40;
                EffectFieldTextureRefernce FXTRefFour = new EffectFieldTextureRefernce();
                OffTemp = Convert.ToInt32(bnr.BaseStream.Position);
                Effect.FieldTextOffsets.Add(OffTemp);
                FXTRefFour.Offset = OffTemp;
                PLName = efl.UncompressedData.Skip(Convert.ToInt32(bnr.BaseStream.Position)).Take(64).Where(x => x != 0x00).ToArray();
                Teme = ascii.GetString(PLName);
                FXTRefFour.TextureName = Teme;
                fld.FileRef.Add(Teme);
                Effect.Fields.Add(fld);

                Effect.FXTXNameRefs.Add(FXTRefFour);

                /*
                switch (fld.EntryType)
                {

                    case 0:
                        //Effect.Fields.Add(fld);
                        break;

                    case 1:

                        bnr.BaseStream.Position = fld.Offset + 0x30 + 0x70;
                        EffectFieldTextureRefernce FXTRef = new EffectFieldTextureRefernce();
                        OffTemp = Convert.ToInt32(bnr.BaseStream.Position);
                        buffer.Offsets.Add(OffTemp);
                        Effect.FieldTextOffsets.Add(OffTemp);
                        FXTRef.Offset = OffTemp;
                        PLName = efl.UncompressedData.Skip(Convert.ToInt32(bnr.BaseStream.Position)).Take(64).Where(x => x != 0x00).ToArray();
                        Teme = ascii.GetString(PLName);
                        FXTRef.TextureName = Teme;
                        fld.FileRef.Add(Teme);
                        OffTemp = 0;
                        Effect.FXTXNameRefs.Add(FXTRef);

                        bnr.BaseStream.Position = bnr.BaseStream.Position + 0x40;
                        EffectFieldTextureRefernce FXTRefTwo = new EffectFieldTextureRefernce();

                        OffTemp = Convert.ToInt32(bnr.BaseStream.Position);
                        Effect.FieldTextOffsets.Add(OffTemp);
                        FXTRefTwo.Offset = OffTemp;
                        PLName = efl.UncompressedData.Skip(Convert.ToInt32(bnr.BaseStream.Position)).Take(64).Where(x => x != 0x00).ToArray();
                        Teme = ascii.GetString(PLName);
                        FXTRefTwo.TextureName = Teme;
                        fld.FileRef.Add(Teme);
                        OffTemp = 0;
                        Effect.FXTXNameRefs.Add(FXTRefTwo);


                        bnr.BaseStream.Position = bnr.BaseStream.Position + 0x40;
                        EffectFieldTextureRefernce FXTRefThree = new EffectFieldTextureRefernce();
                        OffTemp = Convert.ToInt32(bnr.BaseStream.Position);
                        Effect.FieldTextOffsets.Add(OffTemp);
                        FXTRefThree.Offset = OffTemp;
                        PLName = efl.UncompressedData.Skip(Convert.ToInt32(bnr.BaseStream.Position)).Take(64).Where(x => x != 0x00).ToArray();
                        Teme = ascii.GetString(PLName);
                        FXTRefThree.TextureName = Teme;
                        fld.FileRef.Add(Teme);
                        OffTemp = 0;
                        Effect.FXTXNameRefs.Add(FXTRefThree);


                        bnr.BaseStream.Position = bnr.BaseStream.Position + 0x40;
                        EffectFieldTextureRefernce FXTRefFour = new EffectFieldTextureRefernce();
                        OffTemp = Convert.ToInt32(bnr.BaseStream.Position);
                        Effect.FieldTextOffsets.Add(OffTemp);
                        FXTRefFour.Offset = OffTemp;
                        PLName = efl.UncompressedData.Skip(Convert.ToInt32(bnr.BaseStream.Position)).Take(64).Where(x => x != 0x00).ToArray();
                        Teme = ascii.GetString(PLName);
                        FXTRefFour.TextureName = Teme;
                        fld.FileRef.Add(Teme);
                        Effect.Fields.Add(fld);

                        Effect.FXTXNameRefs.Add(FXTRefFour);

                        break;

                    case 2:
                        //Effect.Fields.Add(fld);

                        break;

                    default:
                        //Effect.Fields.Add(fld);

                        break;
                }
                */

            }

            return Effect;

        }


        [Category("Effect"), ReadOnlyAttribute(true)]
        public List<Field> FieldList
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
