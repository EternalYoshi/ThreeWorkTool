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
using ThreeWorkTool.Resources.Wrappers.AnimNodes;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class LMTM3AEntry
    {
        [YamlIgnore] public byte[] FullData;
        [YamlIgnore] public byte[] RawData;
        [YamlIgnore] public byte[] MotionData;
        [YamlIgnore] public int AnimationID;
        public int version = 1;
        [YamlIgnore]
        public bool IsReusingTrackData
        { get; set; }
        //public int TrackDataReference { get; set; }
        [YamlIgnore] public string FileName;
        [YamlMember(Alias = "Name", ApplyNamingConventions = false)] public string ShortName;
        [YamlMember(ApplyNamingConventions = false)] public int FrameCount;
        [YamlIgnore] public int TrackCount;
        [YamlIgnore] public long TrackPointer;
        [YamlIgnore] public long EventClassesPointer;
        [YamlMember(ApplyNamingConventions = false)] public int LoopFrame;
        [YamlIgnore] public string UnknownValue14;
        [YamlIgnore] public string UnknownValue18;
        [YamlIgnore] public string UnknownValue1C;
        [YamlIgnore] public Vector4 EndFramesAdditiveScenePosition;
        [YamlIgnore] public Vector4 EndFramesAdditiveSceneRotation;
        [YamlIgnore] public long AnimationFlags;
        [YamlIgnore] public int AnimDataSize;
        [YamlIgnore] public long FloatTracksPointer;
        [YamlIgnore] public int Unknown58;
        [YamlIgnore] public float Unknown5C;
        [YamlIgnore] public string FileExt;
        [YamlIgnore] public int PrevOffset;
        [YamlIgnore] public int PrevOffsetTwo;
        [YamlIgnore] public int PrevOffsetThree;

        [YamlIgnore] public List<AnimEvent> Events;

        public enum BufferType
        {
            unknown = 0,
            singlevector3 = 1,
            singlerotationquat3 = 2,
            linearvector3 = 3,
            bilinearvector3_16bit = 4,
            bilinearvector3_8bit = 5,
            linearrotationquat4_14bit = 6,
            bilinearrotationquat4_7bit = 7,
            T_VECTOR3_0 = 8,
            T_QUATERNION4 = 9,
            T_POLAR3 = 10,
            bilinearrotationquatxw_14bit = 11,
            bilinearrotationquatyw_14bit = 12,
            bilinearrotationquatzw_14bit = 13,
            bilinearrotationquat4_11bit = 14,
            bilinearrotationquat4_9bit = 15
        }

        public enum ETrackType
        {
            localrotation = 0,
            localposition = 1,
            localscale = 2,
            absoluterotation = 3,
            absoluteposition = 4,
            xpto = 5
        }

        [YamlIgnore] public List<LMTTrackNode> Tracks;

        [YamlMember(ApplyNamingConventions = false)] public List<KeyFrame> KeyFrames;

        public class KeyFrame
        {
            [YamlMember(ApplyNamingConventions = false)] public int Frame;
            [YamlIgnore] public string KeyType;
            [YamlIgnore] public string Buffertype;
            [YamlMember(ApplyNamingConventions = false)] public string TrackType;
            [YamlMember(ApplyNamingConventions = false)] public int BoneID;
            [YamlMember(ApplyNamingConventions = false)] public Vector4 data;
            [YamlIgnore] public int TempFrameValue;
            //[YamlIgnore] public Vector3 EulerKeys;
        }

        public class AnimEvent
        {
            [YamlMember()] public List<int> EventRemap;
            [YamlMember()] public int EventCount;
            [YamlMember()] public int EventsPointer;
            [YamlMember()] public int EventBit;
            [YamlMember()] public int FrameNumber;

            [YamlIgnore]
            [Category("Event"), ReadOnlyAttribute(true)]
            [DisplayName("Event Count")]
            public int EventsTotal
            {

                get
                {
                    return EventCount;
                }
                set
                {
                    EventCount = value;
                }

            }

            [YamlIgnore]
            [Category("Event"), ReadOnlyAttribute(true)]
            [DisplayName("Event Offset")]
            public int EventOffset
            {

                get
                {
                    return EventsPointer;
                }
                set
                {
                    EventsPointer = value;
                }

            }

            [YamlIgnore]
            [Category("Event"), ReadOnlyAttribute(true)]
            [DisplayName("Frame")]
            public int Frame
            {

                get
                {
                    return FrameNumber;
                }
                set
                {
                    FrameNumber = value;
                }

            }

        }

        public class Extremes
        {
            public Vector4 min;
            public Vector4 max;
        }

        public LMTM3AEntry FillM3AProprties(LMTM3AEntry Anim, int datalength, int ID, int RowTotal, int SecondOffset, BinaryReader bnr, int SecondaryCount, LMTEntry lmtentry)
        {
            //Reads the AnnimBlock Header.
            LMTM3AEntry M3a = new LMTM3AEntry();
            M3a.KeyFrames = new List<KeyFrame>();
            M3a._FileType = ".m3a";
            M3a.FileExt = M3a._FileType;
            bnr.BaseStream.Position = lmtentry.OffsetList[ID];
            M3a.TrackPointer = bnr.ReadInt64();
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

            //Checks the animation Flags to see if track data is reused from other entry.

            bool bitcheck = Convert.ToBoolean((M3a.AnimationFlags >> 24) & 1U);
            M3a.IsReusingTrackData = bitcheck;

            M3a.EventClassesPointer = bnr.ReadInt64();
            M3a.AnimDataSize = Convert.ToInt32(M3a.EventClassesPointer - M3a.TrackPointer) + 352;
            M3a.FloatTracksPointer = bnr.ReadInt64();


            M3a.Unknown58 = bnr.ReadInt32();
            M3a.Unknown5C = bnr.ReadSingle();

            PrevOffsetThree = Convert.ToInt32(bnr.BaseStream.Position);
            bnr.BaseStream.Position = M3a.TrackPointer;
            M3a.RawData = new byte[M3a.AnimDataSize];
            M3a.RawData = bnr.ReadBytes(M3a.AnimDataSize);
            M3a.MotionData = new byte[88];
            bnr.BaseStream.Position = lmtentry.OffsetList[ID];
            M3a.MotionData = bnr.ReadBytes(88);
            bnr.BaseStream.Position = PrevOffsetThree;

            try
            {

                //Gets the Tracks.
                M3a.Tracks = new List<LMTTrackNode>();
                bnr.BaseStream.Position = M3a.TrackPointer;

                for (int j = 0; j < M3a.TrackCount; j++)
                {

                    LMTTrackNode track = new LMTTrackNode();
                    track.TrackNumber = j;
                    track.ExtremesArray = new float[8];

                    //For The Buffer type.
                    track.BufferType = bnr.ReadByte();
                    BufferType Btype = (BufferType)track.BufferType;
                    track.BufferKind = Btype.ToString();

                    //For the Track Type.
                    track.TrackType = bnr.ReadByte();
                    ETrackType Ttype = (ETrackType)track.TrackType;
                    track.TrackKind = Ttype.ToString();

                    track.BoneType = bnr.ReadByte();
                    track.BoneID = bnr.ReadByte();
                    track.Weight = bnr.ReadSingle();
                    track.BufferSize = bnr.ReadInt32();
                    bnr.BaseStream.Position = bnr.BaseStream.Position + 4;
                    track.BufferPointer = bnr.ReadInt32();
                    bnr.BaseStream.Position = bnr.BaseStream.Position + 4;
                    track.ReferenceDataPointer = Convert.ToInt32(bnr.BaseStream.Position);
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

                    else
                    {
                        track.Buffer = new byte[0];
                    }


                    if (track.ExtremesPointer != 0)
                    {

                        //MessageBox.Show("Track # " + j + " inside " + lmtentry.EntryName + "\nhas an actual extremes pointer.", "Debug Note");
                        bnr.BaseStream.Position = Convert.ToInt32(track.ExtremesPointer);

                        track.Extreme = new LMTTrackNode.Extremes();

                        track.Extreme.min.W = bnr.ReadSingle();
                        track.Extreme.min.X = bnr.ReadSingle();
                        track.Extreme.min.Y = bnr.ReadSingle();
                        track.Extreme.min.Z = bnr.ReadSingle();

                        track.Extreme.max.W = bnr.ReadSingle();
                        track.Extreme.max.X = bnr.ReadSingle();
                        track.Extreme.max.Y = bnr.ReadSingle();
                        track.Extreme.max.Z = bnr.ReadSingle();

                        track.ExtremesArray[0] = track.Extreme.min.W;
                        track.ExtremesArray[1] = track.Extreme.min.X;
                        track.ExtremesArray[2] = track.Extreme.min.Y;
                        track.ExtremesArray[3] = track.Extreme.min.Z;
                        track.ExtremesArray[4] = track.Extreme.max.W;
                        track.ExtremesArray[5] = track.Extreme.max.X;
                        track.ExtremesArray[6] = track.Extreme.max.Y;
                        track.ExtremesArray[7] = track.Extreme.max.Z;

                        //Keyframes Take 1.

                        //IEnumerable<KeyFrame> Key = LMTM3ATrackBuffer.Convert(track.BufferType, track.Buffer, track.ExtremesArray, track.BoneID, track.BufferKind);
                        //M3a.KeyFrames.AddRange(Key.ToList());

                    }

                    else
                    {

                        //IEnumerable<KeyFrame> Key = LMTM3ATrackBuffer.Convert(track.BufferType, track.Buffer, track.ExtremesArray, track.BoneID, track.BufferKind);
                        //M3a.KeyFrames.AddRange(Key.ToList());
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
                                    OffTemp = OffTemp - Convert.ToInt32(M3a.TrackPointer);
                                    bwm3a.Write(OffTemp);
                                }
                                bwm3a.BaseStream.Position = 40 + (48 * y);
                                OffTemp = brm3a.ReadInt32();
                                bwm3a.BaseStream.Position = (bwm3a.BaseStream.Position - 4);
                                if (OffTemp > 0)
                                {
                                    OffTemp = OffTemp - Convert.ToInt32(M3a.TrackPointer);
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
                M3a.FullData = new byte[(M3a.AnimDataSize + 88)];
                M3a._FileLength = M3a.FullData.LongLength;
                Array.Copy(M3a.RawData, 0, M3a.FullData, 0, M3a.RawData.Length);
                Array.Copy(M3a.MotionData, 0, M3a.FullData, M3a.RawData.Length, M3a.MotionData.Length);
                Anim.KeyFrames = new List<KeyFrame>();
                try
                {
                    for (int i = 0; i < Anim.Tracks.Count; i++)
                    {
                        if (Anim.Tracks[i].BoneID == 255) continue;

                        if (Anim.Tracks[i].Buffer != null && Anim.Tracks[i].Buffer.Length != 0)
                        {
                            using (MemoryStream memory = new MemoryStream(Anim.Tracks[i].Buffer))
                            {
                                using (BinaryReader BufferBnr = new BinaryReader(memory))
                                {
                                    if (Anim.Tracks[i].ExtremesArray != null)
                                    {
                                        List<KeyFrame> CurrentKeys = new List<KeyFrame>();
                                        CurrentKeys.AddRange(LMTM3ATrackBuffer.Convert(Anim.Tracks[i].BufferType, Anim.Tracks[i].Buffer, Anim.Tracks[i].BoneID, Anim.Tracks[i].ExtremesArray, BufferBnr, Anim.Tracks[i].TrackType));
                                        int TempFrameCount = 0;

                                        for (int l = 0; l < CurrentKeys.Count; l++)
                                        {
                                            CurrentKeys[l].Frame = TempFrameCount;
                                            TempFrameCount = TempFrameCount + CurrentKeys[l].TempFrameValue;

                                        }

                                        Anim.KeyFrames.AddRange(CurrentKeys);
                                    }
                                    else
                                    {
                                        List<KeyFrame> CurrentKeys = new List<KeyFrame>();
                                        CurrentKeys.AddRange(LMTM3ATrackBuffer.Convert(Anim.Tracks[i].BufferType, Anim.Tracks[i].Buffer, Anim.Tracks[i].BoneID, null, BufferBnr, Anim.Tracks[i].TrackType));

                                        int TempFrameCount = 0;

                                        for (int l = 0; l < CurrentKeys.Count; l++)
                                        {
                                            CurrentKeys[l].Frame = TempFrameCount;
                                            TempFrameCount = TempFrameCount + CurrentKeys[l].TempFrameValue;

                                        }

                                        Anim.KeyFrames.AddRange(CurrentKeys);

                                    }
                                }
                            }


                        }

                    }
                }
                catch (Exception EXAnim)
                {
                    MessageBox.Show("An error occured when loading animation related data.\n" + EXAnim);
                }
                //And thus Keyframes.
                //PrepareTheKeyframes(Anim);

            }
            catch (Exception ex)
            {

                MessageBox.Show("The M3a at index: " + ID + " inside the file\n" + lmtentry.TrueName + " is malformed.\nAs long as you do not modify the named lmt file you should be able to save changes made to other files inside this arc and the lmt file will not be modified.", "Uh-Oh");

                bnr.BaseStream.Position = lmtentry.OffsetList[ID];
                M3a.AnimationID = ID;
                M3a.FileName = "AnimationID" + M3a.AnimationID + ".m3a";
                M3a.ShortName = "AnimationID" + M3a.AnimationID;
                M3a._IsBlank = false;
                Anim = M3a;
            }

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


                    int projdatlength = m3aentry.FullData.Length - 88;
                    m3aentry.RawData = new byte[(projdatlength)];
                    Array.Copy(m3aentry.FullData, 0, m3aentry.RawData, 0, projdatlength);
                    m3aentry.MotionData = new byte[88];
                    projdatlength = m3aentry.FullData.Length - 88;
                    Array.Copy(m3aentry.FullData, projdatlength, m3aentry.MotionData, 0, 88);
                    bnr.BaseStream.Position = (bnr.BaseStream.Length - 88);

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

                    m3aentry.AnimDataSize = Convert.ToInt32(m3aentry.EventClassesPointer - m3aentry.TrackPointer) + 352;


                    m3aentry.AnimDataSize = m3aentry.FullData.Length - 448;
                    m3aentry.FloatTracksPointer = bnr.ReadInt32();

                    m3aentry.Unknown58 = bnr.ReadInt32();
                    m3aentry.Unknown5C = bnr.ReadSingle();

                    m3aentry.PrevOffsetThree = Convert.ToInt32(bnr.BaseStream.Position);
                    bnr.BaseStream.Position = m3aentry.TrackPointer;
                    //m3aentry.RawData = new byte[m3aentry.AnimDataSize];
                    //Array.Copy(m3aentry.FullData, m3aentry.RawData, m3aentry.AnimDataSize);
                    //m3aentry.MotionData = new byte[88];
                    //bnr.BaseStream.Position = (m3aentry.FullData.Length - 88);
                    //m3aentry.MotionData = bnr.ReadBytes(88);
                    //bnr.BaseStream.Position = m3aentry.TrackPointer;

                    //Gets the Tracks.
                    m3aentry.Tracks = new List<LMTTrackNode>();
                    bnr.BaseStream.Position = 0;

                    for (int j = 0; j < m3aentry.TrackCount; j++)
                    {

                        LMTTrackNode track = new LMTTrackNode();
                        track.TrackNumber = j;
                        track.BufferType = bnr.ReadByte();
                        BufferType type = (BufferType)track.BufferType;
                        track.BufferKind = type.ToString();

                        //For the Track Type.
                        track.TrackType = bnr.ReadByte();
                        ETrackType Ttype = (ETrackType)track.TrackType;
                        track.TrackKind = Ttype.ToString();

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

                            track.Extreme = new LMTTrackNode.Extremes();

                            track.Extreme.min.W = bnr.ReadSingle();
                            track.Extreme.min.X = bnr.ReadSingle();
                            track.Extreme.min.Y = bnr.ReadSingle();
                            track.Extreme.min.Z = bnr.ReadSingle();

                            track.Extreme.max.W = bnr.ReadSingle();
                            track.Extreme.max.X = bnr.ReadSingle();
                            track.Extreme.max.Y = bnr.ReadSingle();
                            track.Extreme.max.Z = bnr.ReadSingle();


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
                                        //Gotta Correct this. It's outputting negative values...
                                        //OffTemp = OffTemp - Convert.ToInt32(m3aentry.TrackPointer);
                                        //OffTemp = OffTemp + Convert.ToInt32(oldentry.TrackPointer);
                                        OffTemp = OffTemp + Convert.ToInt32(oldentry.TrackPointer) + 64;
                                        /*
                                        if (OffTemp < 0)
                                        {

                                        }
                                        else
                                        {
                                            bwm3a.Write(OffTemp);
                                        }
                                        */
                                        //bwm3a.Write(OffTemp);
                                    }
                                    bwm3a.BaseStream.Position = 40 + (48 * y);
                                    OffTemp = brm3a.ReadInt32();
                                    bwm3a.BaseStream.Position = (bwm3a.BaseStream.Position - 4);
                                    if (OffTemp > 0)
                                    {
                                        //Gotta Correct this. It's outputting negative values...
                                        //OffTemp = OffTemp - Convert.ToInt32(m3aentry.TrackPointer);
                                        //OffTemp = OffTemp + Convert.ToInt32(oldentry.TrackPointer);
                                        OffTemp = OffTemp + Convert.ToInt32(oldentry.TrackPointer) + 64;
                                        /*
                                        if (OffTemp < 0)
                                        {

                                        }
                                        else
                                        {
                                            bwm3a.Write(OffTemp);
                                        }
                                        */
                                        //bwm3a.Write(OffTemp);
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
                    m3aentry.FullData = new byte[(m3aentry.RawData.Length + 88)];
                    m3aentry._FileLength = m3aentry.FullData.LongLength;
                    Array.Copy(m3aentry.RawData, 0, m3aentry.FullData, 0, m3aentry.RawData.Length);
                    Array.Copy(m3aentry.MotionData, 0, m3aentry.FullData, m3aentry.RawData.Length, m3aentry.MotionData.Length);





                }
            }

            try
            {
                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
                {

                    //m3aentry.RawData = System.IO.File.ReadAllBytes(filename);


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
                string ProperPath = "";
                ProperPath = Globals.ToolPath + "Log.txt"; using (StreamWriter sw = File.AppendText(ProperPath))
                {
                    sw.WriteLine("Read Error! Here's the exception info:\n" + ex);
                }
            }



            return node.entryfile as LMTM3AEntry;
        }

        public static LMTM3AEntry PrepareTheKeyframes(LMTM3AEntry M3a)
        {

            //M3a.KeyFrames = new List<KeyFrame>();
            /*
            foreach (LMTTrackNode track in M3a.Tracks)
            {
                IEnumerable<KeyFrame> Key = LMTM3ATrackBuffer.Convert(track.BufferType, track.Buffer, track.ExtremesArray, track.BoneID, track.BufferKind, track.TrackKind);

                M3a.KeyFrames.AddRange(Key.ToList());
            }

            for (int v = 0; v < M3a.KeyFrames.Count; v++)
            {
                Quaternion quat = new Quaternion();
                quat.W = M3a.KeyFrames[v].data.X;
                quat.X = M3a.KeyFrames[v].data.Y;
                quat.Y = M3a.KeyFrames[v].data.Z;
                quat.Z = M3a.KeyFrames[v].data.W;

                //M3a.KeyFrames[v].EulerKeys = QToEuler.ToEulerAngles(quat);
            }

            M3a.KeyFrames = M3a.KeyFrames.OrderBy(o => o.Frame).ToList();
            */
            return M3a;
        }

        public static LMTM3AEntry ParseM3AYMLPart1(LMTM3AEntry M3a, string filename)
        {

            LMTM3AEntry NewM3a = new LMTM3AEntry();

            using (var input = File.OpenText(filename))
            {
                var deserializer = new DeserializerBuilder().WithTagMapping("!LMTM3AEntry", typeof(ThreeWorkTool.Resources.Wrappers.LMTM3AEntry)).Build();
                NewM3a = deserializer.Deserialize<LMTM3AEntry>(input);
            }

            foreach (KeyFrame Key in NewM3a.KeyFrames)
            {

            }

            return NewM3a;

        }

        public static LMTM3AEntry ParseM3AYMLPart2(LMTM3AEntry M3a, string filename)
        {

            //Gonna build the M3a from scratch with the Keyframe data.
            List<byte> NewUncompressedData = new List<byte>();
            List<byte> TotalKeyframebufferData = new List<byte>();
            List<byte> TotalTrackListData = new List<byte>();

            int TrackCounter, PrevBoneID, FrameCount, LoopFrame;
            string PrevTackType = "";

            //These M3a files almost always have these tracks that point to bone 255.
            byte[] FirstTracks = { 0x02, 0x03, 0x00, 0xFF, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                   0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                   0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                   0x01, 0x04, 0x00, 0xFF, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                   0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                   0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                   0x01, 0x05, 0x00, 0xFF, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                   0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x80, 0x3F,
                                   0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };


            TrackCounter = 3;
            PrevBoneID = 255;
            FrameCount = M3a.FrameCount;
            LoopFrame = M3a.LoopFrame;

            for (int i = 0; i < M3a.KeyFrames.Count; i++)
            {

                if (i == 0)
                {
                    TrackCounter++;
                }

                //Continue the Track.
                if (i != 0 && PrevBoneID == M3a.KeyFrames[i].BoneID && PrevTackType == M3a.KeyFrames[i].TrackType)
                {

                }
                //End the current track and make new one.
                if ((i != 0 && PrevBoneID != M3a.KeyFrames[i].BoneID) || (i != 0 && PrevTackType == M3a.KeyFrames[i].TrackType))
                {

                }



            }

            return M3a;

        }

        public static void TestFromKeyframesToM3A(LMTM3AEntry M3a)
        {

            LMTM3AEntry NewM3a = new LMTM3AEntry();





        }

        public static LMTM3AEntry UpdateAnimDataFlag(LMTM3AEntry M3a)
        {
            //Checks the BlockData for the ReuseAnmation Flag and sets it based on user setting.
            if (M3a.IsReusingTrackData == true)
            {
                M3a.MotionData[67] = 0x01;
            }
            else
            {
                M3a.MotionData[67] = 0x00;
            }

            return M3a;

        }

        #region M3AEntry Properties

        private string _FileType;
        [Category("Filename"), ReadOnlyAttribute(true)]
        [YamlIgnore]
        public string FileType
        {

            get
            {
                return FileExt;
            }
            set
            {
                _FileType = value;
            }
        }

        [Category("Motion"), ReadOnlyAttribute(true)]
        [YamlIgnore]
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

        private long _FileLength;
        [Category("File Entry"), ReadOnlyAttribute(true)]
        [YamlIgnore]
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
        [YamlIgnore]
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
        [YamlIgnore]
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
        [YamlIgnore]
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
        [YamlIgnore]
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
        [YamlIgnore]
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
        [YamlIgnore]
        public List<LMTTrackNode> CollectionTracks
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
        [YamlIgnore]
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
