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
            public byte[] Buffer;
            public Extremes Extremes;
            public float[] ExtremesArray;
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

        public class Extremes
        {
            public Vector4 min;
            public Vector4 max;
        }

        enum BufferType
        {
            singlevector3 = 1,
            singlerotationquat3,
            linearvector3,
            bilinearvector3_16bit,
            bilinearvector3_8bit,
            linearrotationquat4_14bit,
            bilinearrotationquat4_7bit
        }

        public LMTM3AEntry FillM3AProprties(LMTM3AEntry Anim, int datalength, int ID, int RowTotal, int SecondOffset, BinaryReader bnr, int SecondaryCount, LMTEntry lmtentry)
        {
            //Reads the AnnimBlock Header.
            LMTM3AEntry M3a = new LMTM3AEntry();
            M3a.KeyFrames = new List<KeyFrame>();
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

            M3a.EndFramesAdditiveScenePosition.W = bnr.ReadSingle();
            M3a.EndFramesAdditiveScenePosition.X = bnr.ReadSingle();
            M3a.EndFramesAdditiveScenePosition.Y = bnr.ReadSingle();
            M3a.EndFramesAdditiveScenePosition.Z = bnr.ReadSingle();

            M3a.EndFramesAdditiveSceneRotation.W = bnr.ReadSingle();
            M3a.EndFramesAdditiveSceneRotation.X = bnr.ReadSingle();
            M3a.EndFramesAdditiveSceneRotation.Y = bnr.ReadSingle();
            M3a.EndFramesAdditiveSceneRotation.Z = bnr.ReadSingle();

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
            M3a.MotionData = new byte[96];
            bnr.BaseStream.Position = lmtentry.OffsetList[ID];
            M3a.MotionData = bnr.ReadBytes(96);
            bnr.BaseStream.Position = PrevOffsetThree;

            //Gets the Tracks.
            M3a.Tracks = new List<Track>();
            bnr.BaseStream.Position = M3a.TrackPointer;

            for (int j = 0; j < M3a.TrackCount; j++)
            {

                Track track = new Track();
                track.ExtremesArray = new float[8];
                track.BufferType = bnr.ReadByte();
                track.TrackType = bnr.ReadByte();
                track.BoneType = bnr.ReadByte();
                track.BoneID = bnr.ReadByte();
                track.Weight = bnr.ReadSingle();
                track.BufferSize = bnr.ReadInt32();
                bnr.BaseStream.Position = bnr.BaseStream.Position + 4;
                track.BufferPointer = bnr.ReadInt32();
                bnr.BaseStream.Position = bnr.BaseStream.Position + 4;
                track.ReferenceData.W = bnr.ReadSingle();
                track.ReferenceData.X = bnr.ReadSingle();
                track.ReferenceData.Y = bnr.ReadSingle();
                track.ReferenceData.Z = bnr.ReadSingle();
                track.ExtremesPointer = bnr.ReadInt32();
                bnr.BaseStream.Position = bnr.BaseStream.Position + 4;
                PrevOffset = Convert.ToInt32(bnr.BaseStream.Position);


                if (track.BufferSize != 0)
                {

                    //MessageBox.Show("Track #" + j + " inside " + lmtentry.EntryName + "\nhas a buffer size that is NOT ZERO.", "Debug Note");
                    bnr.BaseStream.Position = track.BufferPointer;
                    track.Buffer = bnr.ReadBytes(track.BufferSize);

                }

                if (track.ExtremesPointer != 0)
                {

                    //MessageBox.Show("Track # " + j + " inside " + lmtentry.EntryName + "\nhas an actual extremes pointer.", "Debug Note");
                    bnr.BaseStream.Position = Convert.ToInt32(track.ExtremesPointer);

                    track.Extremes = new Extremes();

                    track.Extremes.min.W = bnr.ReadSingle();
                    track.Extremes.min.X = bnr.ReadSingle();
                    track.Extremes.min.Y = bnr.ReadSingle();
                    track.Extremes.min.Z = bnr.ReadSingle();

                    track.Extremes.max.W = bnr.ReadSingle();
                    track.Extremes.max.X = bnr.ReadSingle();
                    track.Extremes.max.Y = bnr.ReadSingle();
                    track.Extremes.max.Z = bnr.ReadSingle();

                    track.ExtremesArray[0] = track.Extremes.min.W;
                    track.ExtremesArray[1] = track.Extremes.min.X;
                    track.ExtremesArray[2] = track.Extremes.min.Y;
                    track.ExtremesArray[3] = track.Extremes.min.Z;
                    track.ExtremesArray[4] = track.Extremes.max.W;
                    track.ExtremesArray[5] = track.Extremes.max.X;
                    track.ExtremesArray[6] = track.Extremes.max.Y;
                    track.ExtremesArray[7] = track.Extremes.max.Z;

                    //Keyframes Take 1.
                    
                    KeyFrame key = LMTM3ATrackBuffer.Convert(track.BufferType, track.Buffer, track.ExtremesArray);
                    

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
            M3a.FullData = new byte[(M3a.AnimDataSize + 96)];
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

            //Builds the ma3entry.
            m3aentry._FileType = ".m3a";
            m3aentry.FileExt = m3aentry._FileType;
            m3aentry.FullData = System.IO.File.ReadAllBytes(filename);
            m3aentry.AnimationID = oldentry.AnimationID;
            m3aentry.FileName = oldentry.FileName;
            m3aentry.ShortName = oldentry.ShortName;
            m3aentry._IsBlank = false;

            using (MemoryStream MAThreeStream = new MemoryStream(m3aentry.FullData))
            {
                using (BinaryReader bnr = new BinaryReader(MAThreeStream))
                {
                    if (bnr.BaseStream.Length < 5)
                    {
                        MessageBox.Show("The entry you are trying to import is a blank one,\nso the replace command has been aborted.", "We have a problem here.");
                        return null;
                    }


                    int projdatlength = m3aentry.FullData.Length - 96;
                    m3aentry.RawData = new byte[(projdatlength)];
                    Array.Copy(m3aentry.FullData, 0, m3aentry.RawData, 0, projdatlength);
                    m3aentry.MotionData = new byte[96];
                    projdatlength = m3aentry.FullData.Length - 96;
                    Array.Copy(m3aentry.FullData, projdatlength, m3aentry.MotionData, 0, 96);
                    bnr.BaseStream.Position = (bnr.BaseStream.Length - 96);

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

                    m3aentry.EndFramesAdditiveScenePosition.W = bnr.ReadSingle();
                    m3aentry.EndFramesAdditiveScenePosition.X = bnr.ReadSingle();
                    m3aentry.EndFramesAdditiveScenePosition.Y = bnr.ReadSingle();
                    m3aentry.EndFramesAdditiveScenePosition.Z = bnr.ReadSingle();

                    m3aentry.EndFramesAdditiveSceneRotation.W = bnr.ReadSingle();
                    m3aentry.EndFramesAdditiveSceneRotation.X = bnr.ReadSingle();
                    m3aentry.EndFramesAdditiveSceneRotation.Y = bnr.ReadSingle();
                    m3aentry.EndFramesAdditiveSceneRotation.Z = bnr.ReadSingle();

                    m3aentry.AnimationFlags = bnr.ReadInt64();

                    m3aentry.EventClassesPointer = bnr.ReadInt32();

                    //m3aentry.EventClassesPointer = bnr.ReadInt32();
                    //bnr.BaseStream.Position = bnr.BaseStream.Position + 4;

                    m3aentry.AnimDataSize = (m3aentry.EventClassesPointer - m3aentry.TrackPointer) + 352;


                    m3aentry.AnimDataSize = m3aentry.FullData.Length - 448;
                    m3aentry.FloatTracksPointer = bnr.ReadInt32();
                    bnr.BaseStream.Position = bnr.BaseStream.Position + 4;

                    m3aentry.Unknown58 = bnr.ReadInt32();
                    m3aentry.Unknown5C = bnr.ReadSingle();

                    m3aentry.PrevOffsetThree = Convert.ToInt32(bnr.BaseStream.Position);
                    bnr.BaseStream.Position = m3aentry.TrackPointer;
                    m3aentry.RawData = new byte[m3aentry.AnimDataSize];
                    Array.Copy(m3aentry.FullData, m3aentry.RawData, m3aentry.AnimDataSize);
                    m3aentry.MotionData = new byte[96];
                    bnr.BaseStream.Position = (m3aentry.FullData.Length - 96);
                    m3aentry.MotionData = bnr.ReadBytes(96);
                    bnr.BaseStream.Position = m3aentry.PrevOffsetThree;

                    //Gets the Tracks.
                    m3aentry.Tracks = new List<Track>();
                    bnr.BaseStream.Position = 0;

                    for (int j = 0; j < m3aentry.TrackCount; j++)
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
                        track.ReferenceData.W = bnr.ReadSingle();
                        track.ReferenceData.X = bnr.ReadSingle();
                        track.ReferenceData.Y = bnr.ReadSingle();
                        track.ReferenceData.Z = bnr.ReadSingle();
                        track.ExtremesPointer = bnr.ReadInt32();
                        bnr.BaseStream.Position = bnr.BaseStream.Position + 4;
                        m3aentry.PrevOffset = Convert.ToInt32(bnr.BaseStream.Position);


                        if (track.BufferSize != 0)
                        {

                            //MessageBox.Show("Track #" + j + " inside " + lmtentry.EntryName + "\nhas a buffer size that is NOT ZERO.", "Debug Note");
                            bnr.BaseStream.Position = track.BufferPointer;
                            track.Buffer = bnr.ReadBytes(track.BufferSize);

                        }

                        if (track.ExtremesPointer != 0)
                        {

                            //MessageBox.Show("Track # " + j + " inside " + lmtentry.EntryName + "\nhas an actual extremes pointer.", "Debug Note");
                            bnr.BaseStream.Position = Convert.ToInt32(track.ExtremesPointer);

                            track.Extremes = new Extremes();

                            track.Extremes.min.W = bnr.ReadSingle();
                            track.Extremes.min.X = bnr.ReadSingle();
                            track.Extremes.min.Y = bnr.ReadSingle();
                            track.Extremes.min.Z = bnr.ReadSingle();

                            track.Extremes.max.W = bnr.ReadSingle();
                            track.Extremes.max.X = bnr.ReadSingle();
                            track.Extremes.max.Y = bnr.ReadSingle();
                            track.Extremes.max.Z = bnr.ReadSingle();


                        }
                        bnr.BaseStream.Position = m3aentry.PrevOffset;
                        m3aentry.Tracks.Add(track);

                    }


                    bnr.BaseStream.Position = m3aentry.FullData.Length - 448;
                    //Animation Events.
                    m3aentry.Events = new List<AnimEvent>();

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

                        m3aentry.PrevOffsetTwo = Convert.ToInt32(bnr.BaseStream.Position);
                        bnr.BaseStream.Position = animEvent.EventsPointer;
                        animEvent.EventBit = bnr.ReadInt32();
                        animEvent.FrameNumber = bnr.ReadInt32();

                        m3aentry.Events.Add(animEvent);
                        bnr.BaseStream.Position = m3aentry.PrevOffsetTwo;

                    }

                    //Subtracts pointers in there by the data offset to get their base value.
                    int OffTemp = 0;
                    using (MemoryStream msm3a = new MemoryStream(m3aentry.RawData))
                    {

                        using (BinaryReader brm3a = new BinaryReader(msm3a))
                        {
                            using (BinaryWriter bwm3a = new BinaryWriter(msm3a))
                            {

                                //Adjusts the offsets in the Rawdata of the m3a.
                                bwm3a.BaseStream.Position = 0;

                                for (int y = 0; y < m3aentry.TrackCount; y++)
                                {
                                    bwm3a.BaseStream.Position = 0;
                                    bwm3a.BaseStream.Position = 16 + (48 * y);
                                    OffTemp = brm3a.ReadInt32();
                                    bwm3a.BaseStream.Position = (bwm3a.BaseStream.Position - 4);
                                    if (OffTemp > 0)
                                    {
                                        OffTemp = OffTemp - m3aentry.TrackPointer;
                                        bwm3a.Write(OffTemp);
                                    }
                                    bwm3a.BaseStream.Position = 40 + (48 * y);
                                    OffTemp = brm3a.ReadInt32();
                                    bwm3a.BaseStream.Position = (bwm3a.BaseStream.Position - 4);
                                    if (OffTemp > 0)
                                    {
                                        OffTemp = OffTemp - m3aentry.TrackPointer;
                                        bwm3a.Write(OffTemp);
                                    }

                                }

                                //Adjusts the offsets in the Events.
                                bwm3a.BaseStream.Position = (bwm3a.BaseStream.Length - 280);

                                OffTemp = m3aentry.RawData.Length - 32;

                                bwm3a.Write(OffTemp);
                                bwm3a.BaseStream.Position = bwm3a.BaseStream.Position + 76;

                                OffTemp = m3aentry.RawData.Length - 24;

                                bwm3a.Write(OffTemp);
                                bwm3a.BaseStream.Position = bwm3a.BaseStream.Position + 76;

                                OffTemp = m3aentry.RawData.Length - 16;

                                bwm3a.Write(OffTemp);
                                bwm3a.BaseStream.Position = bwm3a.BaseStream.Position + 76;

                                OffTemp = m3aentry.RawData.Length - 8;

                                bwm3a.Write(OffTemp);

                            }
                        }

                    }

                    //Appends the Animation Block Data to the FullData.
                    m3aentry.FullData = new byte[(m3aentry.AnimDataSize + 96)];
                    m3aentry._FileLength = m3aentry.FullData.LongLength;
                    Array.Copy(m3aentry.RawData, 0, m3aentry.FullData, 0, m3aentry.RawData.Length);
                    Array.Copy(m3aentry.MotionData, 0, m3aentry.FullData, m3aentry.RawData.Length, m3aentry.MotionData.Length);





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
                    node.ImageIndex = 18;
                    node.SelectedImageIndex = 18;
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
