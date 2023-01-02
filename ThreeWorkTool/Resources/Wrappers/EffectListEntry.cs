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
using ThreeWorkTool.Resources.Wrappers.ExtraNodes;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class EffectListEntry : DefaultWrapper
    {
        public string Magic;
        public int Version;
        public int FileSize;
        public int FPS;
        public int EntryCount;
        public byte[] WTemp;
        public new static string TypeHash = "6D5AE854";
        public int DataSize;
        public string WeirdConstant;
        public string OtherWeirdConstant;
        public int EntryCountA;
        public int EntryCountB;
        public int CountXor;
        public int Unknown18;
        public int Unknown1C;
        public int Buffer10;
        public int Buffer11;
        public int Buffer12;
        public int Buffer13;
        public List<EffectNode> Effects;

        public static EffectListEntry FillEFLEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
        {
            EffectListEntry effectList = new EffectListEntry();
            List<byte> BTemp = new List<byte>();

            FillEntry(filename, subnames, tree, br, c, ID, effectList, filetype);

            //Decompression Time.
            effectList.UncompressedData = ZlibStream.UncompressBuffer(effectList.CompressedData);

            //Type Specific Work Here.
            using (MemoryStream LmtStream = new MemoryStream(effectList.UncompressedData))
            {
                using (BinaryReader bnr = new BinaryReader(LmtStream))
                {
                    BuildEffectListEntry(bnr, effectList);
                }
            }

            return effectList;
        }

        public static EffectListEntry BuildEffectListEntry(BinaryReader bnr, EffectListEntry eflentry)
        {

            //Specific file type work goes here!
            int ID = 0;

            //Header Stuff.
            eflentry.Magic = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), eflentry.Magic);
            eflentry.Version = bnr.ReadInt32();
            eflentry.FileSize = bnr.ReadInt32();
            eflentry.FPS = Convert.ToInt32(bnr.ReadSingle());
            eflentry.EntryCountA = bnr.ReadInt16();
            eflentry.EntryCountB = bnr.ReadInt16();
            eflentry.CountXor = bnr.ReadInt32();
            eflentry.Unknown18 = bnr.ReadInt32();
            eflentry.Unknown1C = bnr.ReadInt32();
            eflentry.Buffer10 = bnr.ReadInt32();
            eflentry.Buffer11 = bnr.ReadInt32();
            eflentry.Buffer12 = bnr.ReadInt32();
            eflentry.Buffer13 = bnr.ReadInt32();

            eflentry.Effects = new List<EffectNode>();
            int PrevOffset = Convert.ToInt32(bnr.BaseStream.Position);

            try
            {
                //This is for the first part of Entry Fields for the effects.
                for (int i = 0; i < eflentry.EntryCountA; i++)
                {
                    EffectNode fx = new EffectNode();
                    fx = EffectNode.BuildEffect(fx, i, bnr, eflentry, PrevOffset);
                    eflentry.Effects.Add(fx);
                    ID++;
                }

                //This is for Part Two.
                for (int i = 0; i < eflentry.Effects.Count; i++)
                {
                    eflentry.Effects[i] = EffectNode.BuildEffectPartTwo(eflentry.Effects[i], bnr, eflentry, PrevOffset);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("The efl at index: " + ID + " inside the file\n" + eflentry.TrueName + " threw out an error.\nAs long as you do not modify the named file you should be able to save changes made to other files inside this arc and the file will not be modified.", "Uh-Oh");
            }
            

            return eflentry;

        }

        public static EffectListEntry InsertEFL(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {

            EffectListEntry effectList = new EffectListEntry();

            InsertEntry(tree, node, filename, effectList);

            //Decompression Time.
            effectList.UncompressedData = ZlibStream.UncompressBuffer(effectList.CompressedData);

            try
            {
                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
                {
                    BuildEffectListEntry(bnr, effectList);
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText("Log.txt"))
                {
                    sw.WriteLine("Caught an exception using the BinaryReader. Here's the details:\n" + ex);
                }
            }

            return effectList;

        }

        public static EffectListEntry ReplaceEFL(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {

            EffectListEntry eflentry = new EffectListEntry();
            EffectListEntry oldentry = new EffectListEntry();

            tree.BeginUpdate();

            ReplaceEntry(tree, node, filename, eflentry, oldentry);

            //Type Specific Work Here.
            using (MemoryStream LmtStream = new MemoryStream(eflentry.UncompressedData))
            {
                using (BinaryReader bnr = new BinaryReader(LmtStream))
                {
                    BuildEffectListEntry(bnr, eflentry);
                }
            }

            return node.entryfile as EffectListEntry;

        }

        #region EffectListEntryProperties

        [Category("Effect List"), ReadOnlyAttribute(true)]
        public string FileName
        {

            get
            {
                return TrueName;
            }
            set
            {
                TrueName = value;
            }
        }

        [Category("Effect List"), ReadOnlyAttribute(true)]
        public string FileType
        {

            get
            {
                return FileExt;
            }
            set
            {
                FileExt = value;
            }
        }

        [Category("Effect List"), ReadOnlyAttribute(true)]
        public int FileLength
        {

            get
            {
                return FileSize;
            }
            set
            {
                FileSize = value;
            }
        }

        [Category("Effect List"), ReadOnlyAttribute(true)]
        public int FrameRate
        {

            get
            {
                return FPS;
            }
            set
            {
                FPS = value;
            }
        }

        [Category("Effect List"), ReadOnlyAttribute(true)]
        public int EntryTotalA
        {
            get
            {
                return EntryCountA;
            }
            set
            {
                EntryCountA = value;
            }
        }

        [Category("Effect List"), ReadOnlyAttribute(true)]
        public int EntryTotalB
        {
            get
            {
                return EntryCountB;
            }
            set
            {
                EntryCountB = value;
            }
        }

        #endregion

    }
}
