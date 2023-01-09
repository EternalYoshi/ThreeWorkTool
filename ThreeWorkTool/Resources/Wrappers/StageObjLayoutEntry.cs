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
    public class StageObjLayoutEntry : DefaultWrapper
    {
        public string Magic;
        public int unknown04;
        public int EntryCount;
        public int InterpolatedFileSize;
        public List<StageObjLayoutGroup> Groups;

        public static StageObjLayoutEntry FillSLOEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
        {
            StageObjLayoutEntry sloentry = new StageObjLayoutEntry();
            List<byte> BTemp = new List<byte>();

            FillEntry(filename, subnames, tree, br, c, ID, sloentry, filetype);

            //Decompression Time.
            sloentry.UncompressedData = ZlibStream.UncompressBuffer(sloentry.CompressedData);

            //Type Specific Work Here.
            using (MemoryStream LmtStream = new MemoryStream(sloentry.UncompressedData))
            {
                using (BinaryReader bnr = new BinaryReader(LmtStream))
                {
                    BuildSLOEntry(bnr, sloentry);
                }
            }

            return sloentry;
        }

        public static StageObjLayoutEntry BuildSLOEntry(BinaryReader bnr, StageObjLayoutEntry sloentry)
        {

            //Header.
            sloentry.Magic = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), sloentry.Magic);
            sloentry.unknown04 = bnr.ReadInt32();
            sloentry.EntryCount = bnr.ReadInt32();
            sloentry.InterpolatedFileSize = bnr.ReadInt32();

            sloentry.Groups = new List<StageObjLayoutGroup>();

            //GroupBox Entries.
            for (int i=0; i < sloentry.EntryCount; i++)
            {
                StageObjLayoutGroup slg = new StageObjLayoutGroup();
                slg = StageObjLayoutGroup.BuildSLOGroup(sloentry,i,bnr,slg);
                sloentry.Groups.Add(slg);
            }

            return sloentry;

        }

        public static StageObjLayoutEntry ReplaceSLOEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            StageObjLayoutEntry sloentry = new StageObjLayoutEntry();
            StageObjLayoutEntry oldentry = new StageObjLayoutEntry();

            tree.BeginUpdate();

            ReplaceEntry(tree, node, filename, sloentry, oldentry);

            //Type Specific Work Here.
            using (MemoryStream LmtStream = new MemoryStream(sloentry.UncompressedData))
            {
                using (BinaryReader bnr = new BinaryReader(LmtStream))
                {
                    BuildSLOEntry(bnr, sloentry);
                }
            }

            return node.entryfile as StageObjLayoutEntry;
        }

        public static StageObjLayoutEntry InsertSLOEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            StageObjLayoutEntry slo = new StageObjLayoutEntry();

            InsertEntry(tree, node, filename, slo);

            //Decompression Time.
            slo.UncompressedData = ZlibStream.UncompressBuffer(slo.CompressedData);

            try
            {
                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
                {
                    BuildSLOEntry(bnr, slo);
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText("Log.txt"))
                {
                    sw.WriteLine("Caught an exception using the BinaryReader. Here's the details:\n" + ex);
                }
            }

            return slo;
        }

        public static StageObjLayoutEntry RebuildSLOEntry(ArcEntryWrapper node, StageObjLayoutEntry sloentry)
        {



            return sloentry;

        }

        #region Stage Object Layout Properties

        [Category("Filename"), ReadOnlyAttribute(true)]
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

        [Category("Data"), ReadOnlyAttribute(true)]
        public int GroupCount
        {

            get
            {
                return EntryCount;
            }
            set
            {
                EntryCount = value;
            }
        }

        [Category("Data"), ReadOnlyAttribute(true)]
        public int FileSize
        {

            get
            {
                return InterpolatedFileSize;
            }
            set
            {
                InterpolatedFileSize = value;
            }
        }

        #endregion

    }
}
