using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Utility;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class LMTM3AEntry
    {
        public byte[] FullData;
        public byte[] RawData;
        public byte[] MotionData;
        public int AnimationID;
        public string FileName;
        public string ShortName;
        public int FrameCount;
        public int TrackCount;
        public int TrackPointer;
        public int EventClassesPointer;
        public int LoopFrame;
        public string UnknownValue14;
        public string UnknownValue18;
        public string UnknownValue1C;
        public Vector4 EndFramesAdditiveScenePosition;
        public Vector4 EndFramesAdditiveSceneRotation;
        public long AnimationFlags;
        public int AnimDataSize;
        public int FloatTracksPointer;
        public int Unknown58;
        public float Unknown5C;
        public string FileExt;
        public byte[] Buffer;
        public byte[] Extremes;
        public List<AnimEvent> Events;
        public int PrevOffset;
        public int PrevOffsetTwo;
        public int PrevOffsetThree;

        public List<Track> Tracks;
        public struct Track
        {

            public int BufferType;
            public int TrackType;
            public int BoneType;
            public int BoneID;
            public float Weight;
            public int BufferSize;
            public int BufferPointer;
            public Vector4 ReferenceData;
            public float ExtremesPointer;
            public byte[] buffer;
        }
        
        public List<KeyFrame> KeyFrames;
        public struct KeyFrame
        {
            public Vector3 Coordinates;
            public int Frame;
            public int BoneID;
        }

        public struct AnimEvent
        {
            public List<int> EventRemap;
            public int EventCount;
            public int EventsPointer;
            public int EventBit;
            public int FrameNumber;
        }

        public LMTM3AEntry FillM3AProprties(LMTM3AEntry Anim, int datalength, int ID, int RowTotal, int SecondOffset, BinaryReader bnr, int SecondaryCount, LMTEntry lmtentry)
        {
            //Reads the AnnimBlock Header.
            LMTM3AEntry M3a = new LMTM3AEntry();
            M3a._FileType = ".m3a";
            M3a.FileExt = M3a._FileType;
            bnr.BaseStream.Position = lmtentry.OffsetList[ID];
            M3a.TrackPointer = bnr.ReadInt32();
            bnr.BaseStream.Position = bnr.BaseStream.Position + 4;
            M3a.TrackCount = bnr.ReadInt32();
            M3a.FrameCount = bnr.ReadInt32();
            M3a._FrameTotal = M3a.FrameCount;
            M3a.LoopFrame = bnr.ReadInt32();

            M3a.UnknownValue14 = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), M3a.UnknownValue14);
            M3a.UnknownValue18 = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), M3a.UnknownValue18);
            M3a.UnknownValue1C = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), M3a.UnknownValue1C);

            M3a.EndFramesAdditiveScenePosition.X = bnr.ReadSingle();
            M3a.EndFramesAdditiveScenePosition.Y = bnr.ReadSingle();
            M3a.EndFramesAdditiveScenePosition.Z = bnr.ReadSingle();
            M3a.EndFramesAdditiveScenePosition.W = bnr.ReadSingle();

            M3a.EndFramesAdditiveSceneRotation.X = bnr.ReadSingle();
            M3a.EndFramesAdditiveSceneRotation.Y = bnr.ReadSingle();
            M3a.EndFramesAdditiveSceneRotation.Z = bnr.ReadSingle();
            M3a.EndFramesAdditiveSceneRotation.W = bnr.ReadSingle();

            M3a.AnimationFlags = bnr.ReadInt64();

            M3a.EventClassesPointer = bnr.ReadInt32();
            bnr.BaseStream.Position = bnr.BaseStream.Position + 4;
            M3a.AnimDataSize = (M3a.EventClassesPointer - M3a.TrackPointer) + 352;
            M3a.FloatTracksPointer = bnr.ReadInt32();
            bnr.BaseStream.Position = bnr.BaseStream.Position + 4;

            M3a.Unknown58 = bnr.ReadInt32();
            M3a.Unknown5C = bnr.ReadSingle();

            PrevOffsetThree = Convert.ToInt32(bnr.BaseStream.Position);
            bnr.BaseStream.Position = M3a.TrackPointer;
            M3a.RawData = new byte[M3a.AnimDataSize];
            M3a.RawData = bnr.ReadBytes(M3a.AnimDataSize);
            M3a.MotionData = new byte[80];
            bnr.BaseStream.Position = lmtentry.OffsetList[ID];
            M3a.MotionData = bnr.ReadBytes(80);
            bnr.BaseStream.Position = PrevOffsetThree;

            //Gets the Tracks.
            M3a.Tracks = new List<Track>();
            bnr.BaseStream.Position = M3a.TrackPointer;

            for (int j = 0; j < M3a.TrackCount; j++)
            {


                Track track = new Track();
                track.BufferType = bnr.ReadByte();
                track.TrackType = bnr.ReadByte();
                track.BoneType = bnr.ReadByte();
                track.BoneID = bnr.ReadByte();
                track.Weight = bnr.ReadSingle();
                track.BufferSize = bnr.ReadInt32();
                bnr.BaseStream.Position = bnr.BaseStream.Position + 4;
                track.BufferPointer = bnr.ReadInt32();
                bnr.BaseStream.Position = bnr.BaseStream.Position + 4;
                track.ReferenceData.X = bnr.ReadSingle();
                track.ReferenceData.Y = bnr.ReadSingle();
                track.ReferenceData.Z = bnr.ReadSingle();
                track.ReferenceData.W = bnr.ReadSingle();
                track.ExtremesPointer = bnr.ReadInt32();
                bnr.BaseStream.Position = bnr.BaseStream.Position + 4;
                PrevOffset = Convert.ToInt32(bnr.BaseStream.Position);

                if (track.BufferSize == 0)
                {

                }

                if (track.ExtremesPointer == 0)
                {

                }
                bnr.BaseStream.Position = PrevOffset;
                M3a.Tracks.Add(track);
            }

            bnr.BaseStream.Position = M3a.EventClassesPointer;
            //Animation Events.
            M3a.Events = new List<AnimEvent>();

            for (int k = 0; k < 4; k++)
            {
                AnimEvent animEvent = new AnimEvent();

                for (int l = 0; l < 32; l++)
                {

                    animEvent.EventRemap = new List<int>();
                    animEvent.EventRemap.Add(bnr.ReadInt16());

                }

                animEvent.EventCount = bnr.ReadInt32();
                bnr.BaseStream.Position = bnr.BaseStream.Position + 4;

                animEvent.EventsPointer = bnr.ReadInt32();
                bnr.BaseStream.Position = bnr.BaseStream.Position + 4;

                PrevOffsetTwo = Convert.ToInt32(bnr.BaseStream.Position);
                bnr.BaseStream.Position = animEvent.EventsPointer;
                animEvent.EventBit = bnr.ReadInt32();
                animEvent.FrameNumber = bnr.ReadInt32();

                M3a.Events.Add(animEvent);
                bnr.BaseStream.Position = PrevOffsetTwo;

            }

            M3a.AnimationID = ID;
            M3a.FileName = "AnimationID" + M3a.AnimationID + ".m3a";
            M3a.ShortName = "AnimationID" + M3a.AnimationID;
            M3a._IsBlank = false;
            Anim = M3a;

            //Subtracts pointers in there by the data offset to get their base value.
            int OffTemp = 0;
            using (MemoryStream msm3a = new MemoryStream(M3a.RawData))
            {

                using (BinaryReader brm3a = new BinaryReader(msm3a))
                {
                    using (BinaryWriter bwm3a = new BinaryWriter(msm3a))
                    {

                        //Adjusts the offsets in the Rawdata of the m3a.
                        bwm3a.BaseStream.Position = 0;

                        for (int y = 0; y < M3a.TrackCount; y++)
                        {
                            bwm3a.BaseStream.Position = 0;
                            bwm3a.BaseStream.Position = 16 + (48 * y);
                            OffTemp = brm3a.ReadInt32();
                            bwm3a.BaseStream.Position = (bwm3a.BaseStream.Position - 4);
                            if (OffTemp > 0)
                            {
                                OffTemp = OffTemp - M3a.TrackPointer;
                                bwm3a.Write(OffTemp);
                            }
                            bwm3a.BaseStream.Position = 40 + (48 * y);
                            OffTemp = brm3a.ReadInt32();
                            bwm3a.BaseStream.Position = (bwm3a.BaseStream.Position - 4);
                            if (OffTemp > 0)
                            {
                                OffTemp = OffTemp - M3a.TrackPointer;
                                bwm3a.Write(OffTemp);
                            }

                        }

                        //Adjusts the offsets in the Events.
                        bwm3a.BaseStream.Position = (bwm3a.BaseStream.Length - 280);

                        OffTemp = M3a.RawData.Length - 32;

                        bwm3a.Write(OffTemp);
                        bwm3a.BaseStream.Position = bwm3a.BaseStream.Position + 76;

                        OffTemp = M3a.RawData.Length - 24;

                        bwm3a.Write(OffTemp);
                        bwm3a.BaseStream.Position = bwm3a.BaseStream.Position + 76;

                        OffTemp = M3a.RawData.Length - 16;

                        bwm3a.Write(OffTemp);
                        bwm3a.BaseStream.Position = bwm3a.BaseStream.Position + 76;

                        OffTemp = M3a.RawData.Length - 8;

                        bwm3a.Write(OffTemp);

                    }
                }

            }

            //Appends the Animation Block Data to the FullData.
            M3a.FullData = new byte[(M3a.AnimDataSize + 80)];
            M3a._FileLength = M3a.FullData.LongLength;
            Array.Copy(M3a.RawData, 0, M3a.FullData, 0, M3a.RawData.Length);
            Array.Copy(M3a.MotionData, 0, M3a.FullData, M3a.RawData.Length, M3a.MotionData.Length);

            return Anim;
        }

        public LMTM3AEntry FillBlankM3A(LMTM3AEntry Anim, int datalength, int ID, int RowTotal, int SecondOffset, BinaryReader bnr, int SecondaryCount, LMTEntry lmtentry)
        {
            LMTM3AEntry M3a = new LMTM3AEntry();
            M3a._FileType = ".m3a";
            M3a.FileExt = ".m3a";
            M3a.TrackPointer = -1;
            M3a.TrackCount = -1;
            M3a.FrameCount = -1;
            M3a._FrameTotal = -1;
            M3a.LoopFrame = -1;
            M3a.AnimationFlags = -1;
            M3a.EventClassesPointer = -1;
            M3a.AnimDataSize = -1;
            M3a.AnimationID = ID;
            M3a.FileName = "AnimationID" + M3a.AnimationID + ".m3a";
            M3a.ShortName = "AnimationID" + M3a.AnimationID;
            M3a.RawData = new byte[1];
            M3a.RawData[0] = 0x00;
            M3a.MotionData = new byte[1];
            M3a.MotionData[0] = 0x00;
            M3a.FullData = new byte[1];
            M3a.FullData[0] = 0x00;
            M3a.IsBlank = true;
            Anim = M3a;
            return Anim;
        }

        public static LMTM3AEntry ReplaceLMTM3AEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            LMTM3AEntry m3aentry = new LMTM3AEntry();
            LMTM3AEntry oldentry = new LMTM3AEntry();

            tree.BeginUpdate();

            var tag = node.Tag;
            if (tag is LMTM3AEntry)
            {
                oldentry = tag as LMTM3AEntry;
            }

            //Builds the ma3entry. FInish This when you see it please.
            m3aentry._FileType = "m3a";
            m3aentry.FileExt = m3aentry._FileType;
            m3aentry.FullData = System.IO.File.ReadAllBytes(filename);

            using (MemoryStream MAThreeStream = new MemoryStream(m3aentry.FullData))
            {
                using (BinaryReader bnr = new BinaryReader(MAThreeStream))
                {
                    if (bnr.BaseStream.Length < 5)
                    {
                        MessageBox.Show("The entry you are trying to import is a blank one,\nso the replace command has been aborted.", "We have a problem here.");
                        return null;
                    }
                    else
                    {
                        int projdatlength = m3aentry.FullData.Length - 80;
                        m3aentry.RawData = new byte[(projdatlength)];
                        Array.Copy(m3aentry.FullData, 0, m3aentry.RawData, 0, projdatlength);
                        m3aentry.MotionData = new byte[80];
                        projdatlength = m3aentry.FullData.Length - 80;
                        Array.Copy(m3aentry.FullData, projdatlength, m3aentry.MotionData, 0, 80);
                        bnr.BaseStream.Position = 0;

                        bnr.BaseStream.Position = (m3aentry.FullData.Length - 80);

                        m3aentry.TrackPointer = bnr.ReadInt32();
                        bnr.BaseStream.Position = bnr.BaseStream.Position + 4;
                        m3aentry.TrackCount = bnr.ReadInt32();
                        m3aentry.FrameCount = bnr.ReadInt32();
                        m3aentry._FrameTotal = m3aentry.FrameCount;
                        m3aentry.IsBlank = false;
                        m3aentry.LoopFrame = bnr.ReadInt32();
                        m3aentry.UnknownValue14 = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), m3aentry.UnknownValue14);
                        m3aentry.UnknownValue18 = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), m3aentry.UnknownValue18);
                        m3aentry.UnknownValue1C = ByteUtilitarian.BytesToString(bnr.ReadBytes(4), m3aentry.UnknownValue1C);

                        m3aentry.EndFramesAdditiveScenePosition.X = bnr.ReadSingle();
                        m3aentry.EndFramesAdditiveScenePosition.Y = bnr.ReadSingle();
                        m3aentry.EndFramesAdditiveScenePosition.Z = bnr.ReadSingle();
                        m3aentry.EndFramesAdditiveScenePosition.W = bnr.ReadSingle();

                        m3aentry.EndFramesAdditiveSceneRotation.X = bnr.ReadSingle();
                        m3aentry.EndFramesAdditiveSceneRotation.Y = bnr.ReadSingle();
                        m3aentry.EndFramesAdditiveSceneRotation.Z = bnr.ReadSingle();
                        m3aentry.EndFramesAdditiveSceneRotation.W = bnr.ReadSingle();

                        m3aentry.AnimationFlags = bnr.ReadInt64();

                        m3aentry.EventClassesPointer = bnr.ReadInt32();
                        m3aentry.AnimDataSize = m3aentry.RawData.Length;
                        m3aentry.AnimationID = oldentry.AnimationID;
                    }
                }
            }

            try
            {
                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
                {

                    m3aentry.RawData = System.IO.File.ReadAllBytes(filename);


                    /*
                    var tag = node.Tag;
                    if (tag is LMTM3AEntry)
                    {
                        oldentry = tag as LMTM3AEntry;
                    }
                    */

                    tag = m3aentry;

                    if (node.Tag is LMTM3AEntry)
                    {
                        node.Tag = m3aentry;
                    }

                    var aew = node as ArcEntryWrapper;

                    string type = node.GetType().ToString();
                    if (type == "ThreeWorkTool.Resources.Wrappers.ArcEntryWrapper")
                    {
                        aew.entryfile = m3aentry;
                    }

                    node = aew;
                    node.entryfile = m3aentry;
                    /*
                    //ArcEntryWrapper aew = new ArcEntryWrapper();
                    if (node is ArcEntryWrapper)
                    {
                        node.entryfile as ArcEntryWrapper = node.Tag;
                    }
                    */
                    tree.EndUpdate();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Read error. Is the file readable?");
                using (StreamWriter sw = File.AppendText("Log.txt"))
                {
                    sw.WriteLine("Read Error! Here's the exception info:\n" + ex);
                }
            }



            return node.entryfile as LMTM3AEntry;
        }



        #region M3AEntry Properties

        private string _FileType;
        [Category("Filename"), ReadOnlyAttribute(true)]
         public string FileType
        {

            get
            {
                return _FileType;
            }
            set
            {
                _FileType = value;
            }
        }

        [Category("Motion"), ReadOnlyAttribute(true)]
        public int MotionID
        {

            get
            {
                return AnimationID;
            }
            set
            {
                AnimationID = value;
            }
        }

        /*
        [Category("Motion"), ReadOnlyAttribute(true)]
        public int BufferSize
        {

            get
            {
                return AnimationID;
            }
            set
            {
                AnimationID = value;
            }

        }
        */

        private long _FileLength;
        [Category("File Entry"), ReadOnlyAttribute(true)]
        public long FileLength
        {

            get
            {
                return _FileLength;
            }
            set
            {
                _FileLength = value;
            }
        }

        private long _FrameTotal;
        [Category("Motion"), ReadOnlyAttribute(true)]
        public long FrameTotal
        {

            get
            {
                return _FrameTotal;
            }
            set
            {
                _FrameTotal = value;
            }

        }

        [Category("Motion"), ReadOnlyAttribute(true)]
        public int NumberOfTracks
        {

            get
            {
                //return _IndexRowTotal;
                return TrackCount;
            }
            set
            {
                //_IndexRowTotal = value;
                TrackCount = value;

            }

        }

        [Category("Motion"), ReadOnlyAttribute(true)]
        public int AnimationLoopFrame
        {

            get
            {
                //return _IndexRowTotal;
                return LoopFrame;
            }
            set
            {
                //_IndexRowTotal = value;
                LoopFrame = value;

            }

        }

        private bool _IsBlank;
        [Category("File Entry"), ReadOnlyAttribute(true)]
        public bool IsBlank
        {

            get
            {
                return _IsBlank;
            }
            set
            {
                _IsBlank = value;
            }

        }

        [Category("Motion"), ReadOnlyAttribute(true)]
        public long MotionFlags
        {

            get
            {
                //return _IndexRowTotal;
                return AnimationFlags;
            }
            set
            {
                //_IndexRowTotal = value;
                AnimationFlags = value;

            }
        }

        [Category("Motion"), ReadOnlyAttribute(true)]
        public List<Track> CollectionTracks
        {

            get
            {
                //return _IndexRowTotal;
                return Tracks;
            }
            set
            {
                //_IndexRowTotal = value;
                Tracks = value;

            }
        }

        [Category("Motion"), ReadOnlyAttribute(true)]
        public List<AnimEvent> CollectionEvents
        {

            get
            {
                //return _IndexRowTotal;
                return Events;
            }
            set
            {
                //_IndexRowTotal = value;
                Events = value;

            }
        }

        #endregion

    }
}
