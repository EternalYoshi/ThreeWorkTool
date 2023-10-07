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

            M3a.EndFramesAdditiveScenePosition.X = bnr.ReadSingle();
            M3a.EndFramesAdditiveScenePosition.Y = bnr.ReadSingle();
            M3a.EndFramesAdditiveScenePosition.Z = bnr.ReadSingle();
            M3a.EndFramesAdditiveScenePosition.W = bnr.ReadSingle();

            M3a.EndFramesAdditiveSceneRotation.X = bnr.ReadSingle();
            M3a.EndFramesAdditiveSceneRotation.Y = bnr.ReadSingle();
            M3a.EndFramesAdditiveSceneRotation.Z = bnr.ReadSingle();
            M3a.EndFramesAdditiveSceneRotation.W = bnr.ReadSingle();

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
                /*
                #if DEBUG

                                File.WriteAllBytes("D:\\Workshop\\LMTHub\\Test\\OldMotionDataTest" + ".bin", M3a.MotionData);
                                File.WriteAllBytes("D:\\Workshop\\LMTHub\\Test\\OldRawDataTest" + ".bin", M3a.RawData);
                                File.WriteAllBytes("D:\\Workshop\\LMTHub\\Test\\OldFullDataTest" + ".bin", M3a.FullData);

                #endif
                */

                //Gathers Keyframes.
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
                                        CurrentKeys.AddRange(LMTM3ATrackBuffer.Convert(Anim.Tracks[i].BufferType, Anim.Tracks[i].Buffer, Anim.Tracks[i].BoneID, Anim.Tracks[i].ExtremesArray, BufferBnr, Anim.Tracks[i].TrackType, i, Anim.Tracks[i].BufferSize));
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
                                        CurrentKeys.AddRange(LMTM3ATrackBuffer.Convert(Anim.Tracks[i].BufferType, Anim.Tracks[i].Buffer, Anim.Tracks[i].BoneID, null, BufferBnr, Anim.Tracks[i].TrackType, i, Anim.Tracks[i].BufferSize));

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
                        else if (Anim.Tracks[i].BoneID != 255)
                        {
                            KeyFrame Key = new KeyFrame();
                            Key.data.X = Anim.Tracks[i].ReferenceData.W;
                            Key.data.Y = Anim.Tracks[i].ReferenceData.X;
                            Key.data.Z = Anim.Tracks[i].ReferenceData.Y;
                            Key.data.W = Anim.Tracks[i].ReferenceData.Z;
                            Key.BoneID = Anim.Tracks[i].BoneID;
                            Key.Frame = 0;
                            var ETrackType = (ETrackType)Anim.Tracks[i].TrackType;
                            var EBufferType = (EBufferType)Anim.Tracks[i].BufferType;
                            Key.TrackType = ETrackType.ToString();
                            Key.Buffertype = EBufferType.ToString();
                            Anim.KeyFrames.Add(Key);


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

        public static LMTM3AEntry ParseM3AYMLPart1(LMTM3AEntry M3a, string filename, LMTM3AEntry oldentry)
        {
            List<byte> NewUncompressedData = new List<byte>();
            LMTM3AEntry NewM3a = new LMTM3AEntry();
            int q = 0;
            using (var input = File.OpenText(filename))
            {
                var deserializer = new DeserializerBuilder().WithTagMapping("!LMTM3AEntry", typeof(ThreeWorkTool.Resources.Wrappers.LMTM3AEntry)).Build();
                NewM3a = deserializer.Deserialize<LMTM3AEntry>(input);
            }

            NewM3a.Tracks = new List<LMTTrackNode>();

            List<KeyFrame> WorkingTrack = new List<KeyFrame>();

            //For the first 3 tracks I see in default characters' M3a files.
            BuildInitalTracks(NewM3a, WorkingTrack);

            //Inserts The First 3 tracks in the NewUncompressedData.

            int ExtremesCount = 0;
            List<byte> NewBufferData = new List<byte>();

            for (int i = 0; i < NewM3a.KeyFrames.Count; i++)
            {
                if (i != 0)
                {
                    //Gathers Keyframes in a separate list, which is then made into a track when the iterator encoutners a different TrackType or JointID.
                    if (NewM3a.KeyFrames[i].TrackType != NewM3a.KeyFrames[(i - 1)].TrackType || NewM3a.KeyFrames[i].BoneID != NewM3a.KeyFrames[(i - 1)].BoneID)
                    {
                        BuildTheTracks(NewM3a, WorkingTrack, i, NewM3a, q);
                        WorkingTrack.Clear();
                        WorkingTrack.Add(NewM3a.KeyFrames[i]);

                    }
                    else
                    {
                        WorkingTrack.Add(NewM3a.KeyFrames[i]);
                    }
                }
                else
                {
                    WorkingTrack.Add(NewM3a.KeyFrames[i]);
                }

            }


            List<byte> NewBlockData = new List<byte>();
            List<byte> NewFullData = new List<byte>();
            byte[] NewMotionData = new byte[88];
            byte[] BlankPointer = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            byte[] BlankFour = { 0x00, 0x00, 0x00, 0x00 };
            byte[] BlankLine = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            byte[] BlankHalf = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            byte[] EventsChunk = new byte[352];//Event Data for anim, 352 bytes, AKA 0x160 
            //Block Data.
            using (MemoryStream ms1 = new MemoryStream(NewMotionData))
            {
                using (BinaryReader br1 = new BinaryReader(ms1))
                {
                    using (BinaryWriter bw1 = new BinaryWriter(ms1))
                    {
                        bw1.BaseStream.Position = 8;
                        bw1.Write(NewM3a.Tracks.Count);
                        bw1.Write(NewM3a.FrameCount);
                        bw1.Write((int)NewM3a.LoopFrame);
                        bw1.BaseStream.Position = 60;
                        bw1.Write((float)1.0F);
                        bw1.Write((int)8388608);
                    }
                }
            }

            //Tallys count for Extremes.
            for (int r = 0; r < NewM3a.Tracks.Count; r++)
            {
                if (NewM3a.Tracks[r].ExtremesArray != null)
                {
                    ExtremesCount++;
                }

                byte[] EmptyLong = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                //NewBlockData.AddRange(EmptyLong);
                //NewBlockData.AddRange(NewM3a.Tracks.Count);
            }



            List<byte> NewExtremesData = new List<byte>();
            byte[] NewTrackData = new byte[(NewM3a.Tracks.Count * 48)];
            long PointerOfInterest = NewM3a.Tracks.Count * 48;
            //This builds the first part of the track data but does not apply the pointers for anything else.
            using (MemoryStream ms2 = new MemoryStream(NewTrackData))
            {
                using (BinaryReader br2 = new BinaryReader(ms2))
                {
                    using (BinaryWriter bw2 = new BinaryWriter(ms2))
                    {
                        bw2.BaseStream.Position = 0;
                        for (int s = 0; s < NewM3a.Tracks.Count; s++)
                        {
                            bw2.Write(Convert.ToByte(NewM3a.Tracks[s].BufferType));
                            bw2.Write(Convert.ToByte(NewM3a.Tracks[s].TrackType));
                            bw2.Write(Convert.ToByte(NewM3a.Tracks[s].BoneType));
                            bw2.Write(Convert.ToByte(NewM3a.Tracks[s].BoneID));

                            bw2.Write(NewM3a.Tracks[s].Weight);
                            bw2.Write(NewM3a.Tracks[s].BufferSize);
                            bw2.Write(BlankFour);
                            bw2.Write(BlankPointer);

                            bw2.Write(NewM3a.Tracks[s].ReferenceData.X);
                            bw2.Write(NewM3a.Tracks[s].ReferenceData.Y);
                            bw2.Write(NewM3a.Tracks[s].ReferenceData.Z);
                            bw2.Write(NewM3a.Tracks[s].ReferenceData.W);

                            //For the Extremes. Gotta do these first before the keyframe buffers themselves.
                            if (NewM3a.Tracks[s].ExtremesArray != null)
                            {
                                bw2.Write(PointerOfInterest);
                                PointerOfInterest = PointerOfInterest + 32;

                                //Adds the extremes to the Extreme Byte List.
                                NewExtremesData.AddRange(BitConverter.GetBytes(NewM3a.Tracks[s].ExtremesArray[0]));
                                NewExtremesData.AddRange(BitConverter.GetBytes(NewM3a.Tracks[s].ExtremesArray[1]));
                                NewExtremesData.AddRange(BitConverter.GetBytes(NewM3a.Tracks[s].ExtremesArray[2]));
                                NewExtremesData.AddRange(BitConverter.GetBytes(NewM3a.Tracks[s].ExtremesArray[3]));

                                NewExtremesData.AddRange(BitConverter.GetBytes(NewM3a.Tracks[s].ExtremesArray[4]));
                                NewExtremesData.AddRange(BitConverter.GetBytes(NewM3a.Tracks[s].ExtremesArray[5]));
                                NewExtremesData.AddRange(BitConverter.GetBytes(NewM3a.Tracks[s].ExtremesArray[6]));
                                NewExtremesData.AddRange(BitConverter.GetBytes(NewM3a.Tracks[s].ExtremesArray[7]));


                            }
                            else
                            {
                                bw2.Write(BlankPointer);
                            }

                        }

                        //For the Track Buffers.
                        PointerOfInterest = (NewM3a.Tracks.Count * 48) + NewExtremesData.Count; 
                        bw2.BaseStream.Position = 8;
                        for (int t = 0; t < NewM3a.Tracks.Count; t++)
                        {
                            if (NewM3a.Tracks[t].BoneID == 255) continue;
                            bw2.BaseStream.Position = (t * 48) + 8;
                            if (t != 0)
                            {
                                //To the next track.

                            }

                            if (NewM3a.Tracks[t].Buffer != null)
                            {
                                bw2.Write(NewM3a.Tracks[t].BufferSize);
                                bw2.Write(BlankFour);


                                bw2.Write(PointerOfInterest);
                                NewBufferData.AddRange(NewM3a.Tracks[t].Buffer);
                                //Recalculate the Pointer of Interest.
                                PointerOfInterest = PointerOfInterest + NewM3a.Tracks[t].Buffer.LongLength;

                            }

                        }



                    }
                }
            }

            NewFullData.AddRange(NewTrackData);
            NewFullData.AddRange(NewExtremesData);
            NewFullData.AddRange(NewBufferData);
            //Gotta add event data.
            //Fixing up the event data.
            int NumberToPost = 0;
            NewM3a.Events = new List<AnimEvent>();
            LMTM3AEntry.AnimEvent eve = new AnimEvent
            {
                EventBit = 0,
                EventCount = 1,
                EventsTotal = 1,
                Frame = NewM3a.FrameCount
            };

            using (MemoryStream ms3 = new MemoryStream(EventsChunk))
            {
                using (BinaryReader br3 = new BinaryReader(ms3))
                {
                    using (BinaryWriter bw3 = new BinaryWriter(ms3))
                    {
                        bw3.BaseStream.Position = 64;
                        bw3.Write(Convert.ToInt32(1));
                        bw3.BaseStream.Position = 72;
                        NumberToPost = 320 + NewFullData.Count;
                        bw3.Write(NumberToPost);
                        eve.EventsPointer = NumberToPost;
                        NewM3a.Events.Add(eve);

                        bw3.BaseStream.Position = 144;
                        bw3.Write(Convert.ToInt32(1));
                        bw3.BaseStream.Position = 152;
                        NumberToPost = 328 + NewFullData.Count;
                        bw3.Write(NumberToPost);
                        eve.EventsPointer = NumberToPost;
                        NewM3a.Events.Add(eve);

                        bw3.BaseStream.Position = 224;
                        bw3.Write(Convert.ToInt32(1));
                        bw3.BaseStream.Position = 232;
                        NumberToPost = 336 + NewFullData.Count;
                        bw3.Write(NumberToPost);
                        eve.EventsPointer = NumberToPost;
                        NewM3a.Events.Add(eve);

                        bw3.BaseStream.Position = 304;
                        bw3.Write(Convert.ToInt32(1));
                        bw3.BaseStream.Position = 312;
                        NumberToPost = 342 + NewFullData.Count;
                        bw3.Write(NumberToPost);
                        eve.EventsPointer = NumberToPost;
                        NewM3a.Events.Add(eve);

                        bw3.BaseStream.Position = 324;
                        bw3.Write(NewM3a.FrameCount);

                        bw3.BaseStream.Position = 332;
                        bw3.Write(NewM3a.FrameCount);

                        bw3.BaseStream.Position = 340;
                        bw3.Write(NewM3a.FrameCount);

                        bw3.BaseStream.Position = 348;
                        bw3.Write(NewM3a.FrameCount);

                    }
                }
            }

            //Gotta update the Events Class pointer in the motion data.
            using (MemoryStream ms4 = new MemoryStream(NewMotionData))
            {
                using (BinaryReader br4 = new BinaryReader(ms4))
                {
                    using (BinaryWriter bw4 = new BinaryWriter(ms4))
                    {

                        bw4.BaseStream.Position = 72;
                        bw4.Write(NewFullData.Count);
                        NewM3a.EventClassesPointer = NewFullData.Count;
                    }
                }
            }

            NewM3a.MotionData = NewMotionData;
            NewFullData.AddRange(EventsChunk);
            NewM3a.RawData = NewFullData.ToArray();
#if DEBUG

            File.WriteAllBytes("D:\\Workshop\\LMTHub\\Test\\NewRawDataTest" + ".bin", NewFullData.ToArray());

#endif

            NewM3a.AnimDataSize = NewFullData.Count;
            NewFullData.AddRange(NewMotionData);




            NewM3a.FullData = NewFullData.ToArray();

#if DEBUG

            File.WriteAllBytes("D:\\Workshop\\LMTHub\\Test\\NewTrackDataTest" + ".bin", NewTrackData);

            File.WriteAllBytes("D:\\Workshop\\LMTHub\\Test\\NewMotionDataTest" + ".bin", NewMotionData);

            File.WriteAllBytes("D:\\Workshop\\LMTHub\\Test\\NewExtremesDataTest" + ".bin", NewExtremesData.ToArray());

            File.WriteAllBytes("D:\\Workshop\\LMTHub\\Test\\NewFullDataTest" + ".bin", NewFullData.ToArray());


#endif

            //Builds the rest of the ma3entry.
            NewM3a._FileType = ".m3a";
            NewM3a.FileExt = NewM3a._FileType;
            NewM3a.AnimationID = oldentry.AnimationID;
            NewM3a.FileName = oldentry.FileName;
            NewM3a.ShortName = oldentry.ShortName;
            NewM3a.AnimationLoopFrame = NewM3a.LoopFrame;
            NewM3a._IsBlank = false;
            NewM3a.AnimationFlags = 8388608;
            NewM3a.FrameTotal = NewM3a.FrameCount;
            NewM3a.TrackCount = NewM3a.Tracks.Count;
            NewM3a.NumberOfTracks = NewM3a.TrackCount;
            NewM3a.CollectionTracks = NewM3a.Tracks;
            NewM3a.IsReusingTrackData = false;
            NewM3a.IsBlank = false;

            return NewM3a;

        }

        public static LMTM3AEntry ParseM3AYMLPart2(LMTM3AEntry M3a, string filename, ArcEntryWrapper NewWrapper, TreeView tree)
        {

            tree.BeginUpdate();



            tree.EndUpdate();
            NewWrapper.ImageIndex = 18;
            NewWrapper.SelectedImageIndex = 18;

            return M3a;
        }

        public static LMTM3AEntry BuildInitalTracks(LMTM3AEntry M3a, List<KeyFrame> WorkingTrack)
        {

            LMTTrackNode Track0 = new LMTTrackNode()
            {
                BufferType = 2,
                TrackType = 3,
                BoneType = 0,
                BoneID = 255,
                Weight = 1,
                BufferSize = 0,
                BufferPointer = 0,
                ExtremesPointer = 0
            };
            Track0.ReferenceData.X = 0;
            Track0.ReferenceData.Y = 0;
            Track0.ReferenceData.Z = 0;
            Track0.ReferenceData.W = 1;

            LMTTrackNode Track1 = new LMTTrackNode()
            {
                BufferType = 1,
                TrackType = 4,
                BoneType = 0,
                BoneID = 255,
                Weight = 1,
                BufferSize = 0,
                BufferPointer = 0,
                ExtremesPointer = 0
            };
            Track1.ReferenceData.X = 0;
            Track1.ReferenceData.Y = 0;
            Track1.ReferenceData.Z = 0;
            Track1.ReferenceData.W = 1;


            LMTTrackNode Track2 = new LMTTrackNode()
            {
                BufferType = 1,
                TrackType = 5,
                BoneType = 0,
                BoneID = 255,
                Weight = 1,
                BufferSize = 0,
                BufferPointer = 0,
                ExtremesPointer = 0
            };
            Track1.ReferenceData.X = 0;
            Track1.ReferenceData.Y = 0;
            Track1.ReferenceData.Z = 0;
            Track1.ReferenceData.W = 1;




            M3a.Tracks.Add(Track0);
            M3a.Tracks.Add(Track1);
            M3a.Tracks.Add(Track2);
            return M3a;

        }

        public static LMTM3AEntry BuildTheTracks(LMTM3AEntry M3a, List<KeyFrame> WorkingTrack, int i, LMTM3AEntry NewM3a, int counter)
        {

            LMTTrackNode NewTrack = new LMTTrackNode();

            int p = counter;


            switch (WorkingTrack[0].TrackType)
            {
                case "localposition":
                    if (WorkingTrack.Count == 1)
                    {
                        NewTrack.BufferKind = "singlevector3";
                        NewTrack.BufferType = 1;
                        NewTrack.BufferSize = 0;
                        NewTrack.BufferPointer = 0;
                    }
                    else
                    {
                        NewTrack.BufferKind = "bilinearvector3_16bit";
                        NewTrack.BufferType = 4;
                    }
                    NewTrack.TrackType = 1;
                    if (WorkingTrack[0].Frame == 0)
                    {
                        NewTrack.ReferenceData.X = WorkingTrack[0].data.X;
                        NewTrack.ReferenceData.Y = WorkingTrack[0].data.Y;
                        NewTrack.ReferenceData.Z = WorkingTrack[0].data.Z;
                        NewTrack.ReferenceData.W = 0;
                    }
                    else
                    {
                        NewTrack.ReferenceData.X = WorkingTrack[0].data.X;
                        NewTrack.ReferenceData.Y = WorkingTrack[0].data.Y;
                        NewTrack.ReferenceData.Z = WorkingTrack[0].data.Z;
                        NewTrack.ReferenceData.W = 0;
                    }

                    break;
                case "localpositionBiLi":
                    if (WorkingTrack.Count == 1)
                    {
                        NewTrack.BufferKind = "singlevector3";
                        NewTrack.BufferType = 1;
                        NewTrack.BufferSize = 0;
                        NewTrack.BufferPointer = 0;
                    }
                    else
                    {
                        NewTrack.BufferKind = "bilinearvector3_8bit";
                        NewTrack.BufferType = 5;
                    }
                    NewTrack.TrackType = 1;
                    if (WorkingTrack[0].Frame == 0)
                    {
                        NewTrack.ReferenceData.X = WorkingTrack[0].data.X;
                        NewTrack.ReferenceData.Y = WorkingTrack[0].data.Y;
                        NewTrack.ReferenceData.Z = WorkingTrack[0].data.Z;
                        NewTrack.ReferenceData.W = 0;
                    }
                    else
                    {
                        NewTrack.ReferenceData.X = WorkingTrack[0].data.X;
                        NewTrack.ReferenceData.Y = WorkingTrack[0].data.Y;
                        NewTrack.ReferenceData.Z = WorkingTrack[0].data.Z;
                        NewTrack.ReferenceData.W = 0;
                    }

                    break;
                case "localrotation":
                    if (WorkingTrack.Count == 1)
                    {
                        NewTrack.BufferKind = "singlerotationquat3";
                        NewTrack.BufferType = 2;
                        NewTrack.BufferSize = 0;
                        NewTrack.BufferPointer = 0;
                    }
                    else
                    {
                        NewTrack.BufferKind = "linearrotationquat4_14bit";
                        NewTrack.BufferType = 6;
                    }
                    NewTrack.TrackType = 0;
                    if (WorkingTrack[0].Frame == 0)
                    {
                        NewTrack.ReferenceData.X = WorkingTrack[0].data.X;
                        NewTrack.ReferenceData.Y = WorkingTrack[0].data.Y;
                        NewTrack.ReferenceData.Z = WorkingTrack[0].data.Z;
                        NewTrack.ReferenceData.W = WorkingTrack[0].data.W;
                    }
                    else
                    {
                        NewTrack.ReferenceData.X = WorkingTrack[0].data.X;
                        NewTrack.ReferenceData.Y = WorkingTrack[0].data.Y;
                        NewTrack.ReferenceData.Z = WorkingTrack[0].data.Z;
                        NewTrack.ReferenceData.W = WorkingTrack[0].data.W;
                    }
                    break;
                case "localrotationBI":
                    if (WorkingTrack.Count == 1)
                    {
                        NewTrack.BufferKind = "singlerotationquat3";
                        NewTrack.BufferType = 2;
                        NewTrack.BufferSize = 0;
                        NewTrack.BufferPointer = 0;
                    }
                    else
                    {
                        NewTrack.BufferKind = "bilinearrotationquat4_7bit";
                        NewTrack.BufferType = 7;
                    }
                    NewTrack.TrackType = 0;
                    if (WorkingTrack[0].Frame == 0)
                    {
                        NewTrack.ReferenceData.X = WorkingTrack[0].data.X;
                        NewTrack.ReferenceData.Y = WorkingTrack[0].data.Y;
                        NewTrack.ReferenceData.Z = WorkingTrack[0].data.Z;
                        NewTrack.ReferenceData.W = WorkingTrack[0].data.W;
                    }
                    else
                    {
                        NewTrack.ReferenceData.X = WorkingTrack[0].data.X;
                        NewTrack.ReferenceData.Y = WorkingTrack[0].data.Y;
                        NewTrack.ReferenceData.Z = WorkingTrack[0].data.Z;
                        NewTrack.ReferenceData.W = WorkingTrack[0].data.W;
                    }
                    break;
                case "localscale":
                    if (WorkingTrack.Count == 1)
                    {
                        NewTrack.BufferKind = "singlevector3";
                        NewTrack.BufferType = 1;
                        NewTrack.BufferSize = 0;
                        NewTrack.BufferPointer = 0;
                    }
                    else
                    {
                        NewTrack.BufferKind = "bilinearvector3_16bit";
                        NewTrack.BufferType = 4;
                    }
                    if (WorkingTrack[0].Frame == 0)
                    {
                        NewTrack.ReferenceData.X = WorkingTrack[0].data.X;
                        NewTrack.ReferenceData.Y = WorkingTrack[0].data.Y;
                        NewTrack.ReferenceData.Z = WorkingTrack[0].data.Z;
                        NewTrack.ReferenceData.W = 0;
                    }
                    else
                    {
                        NewTrack.ReferenceData.X = WorkingTrack[0].data.X;
                        NewTrack.ReferenceData.Y = WorkingTrack[0].data.Y;
                        NewTrack.ReferenceData.Z = WorkingTrack[0].data.Z;
                        NewTrack.ReferenceData.W = 0;
                    }
                    NewTrack.TrackType = 2;
                    break;
                default:
                    MessageBox.Show("This Tracktype " + WorkingTrack[0].TrackType + "\nhasn't been implmented yet!");
                    break;
            }

            NewTrack.BoneID = WorkingTrack[0].BoneID;
            NewTrack.TrackNumber = i;
            NewTrack.Weight = 1;
            NewTrack.BoneType = 0;

            if (NewTrack.BufferKind == "singlerotationquat3" || NewTrack.BufferKind == "singlevector3")
            {



            }
            else
            {
                //This part blank for creating buffer in case it hasn't been done before.
                List<byte> NewBuffer = new List<byte>();
                string binarystring = "";
                int buffer_size = 0;
                int bit_size = 0;
                for (int j = 0; j < WorkingTrack.Count; j++)
                {
                    switch (NewTrack.BufferType)
                    {

                        case 1: //singlevector3
                                //MessageBox.Show("This BufferType " + WorkingTrack[0].TrackType + "\nhasn't been implmented yet!");
                            buffer_size = 12;
                            bit_size = 32;

                            break;
                        case 2: //singlerotationquat3
                                //MessageBox.Show("This BufferType " + WorkingTrack[0].TrackType + "\nhasn't been implmented yet!");
                            buffer_size = 8;
                            bit_size = 14;

                            break;
                        case 3: //linearvector3
                                //MessageBox.Show("This BufferType " + WorkingTrack[0].TrackType + "\nhasn't been implmented yet!");
                            buffer_size = 16;
                            bit_size = 32;

                            break;
                        case 4: //bilinearvector3_16bit
                                //MessageBox.Show("This BufferType " + WorkingTrack[0].TrackType + "\nhasn't been implmented yet!");
                            buffer_size = 8;
                            bit_size = 16;
                            BuildTheKeyFrameBuffer(NewTrack, WorkingTrack, i, p);
                            break;
                        case 5: //bilinearvector3_8bit
                                //MessageBox.Show("This BufferType " + WorkingTrack[0].TrackType + "\nhasn't been implmented yet!");
                            buffer_size = 4;
                            bit_size = 8;
                            BuildTheKeyFrameBuffer(NewTrack, WorkingTrack, i, p);
                            break;
                        case 6: //linearrotationquat4_14bit
                            buffer_size = 8;
                            bit_size = 14;
                            BuildTheKeyFrameBuffer(NewTrack, WorkingTrack, i, p);

                            break;
                        case 7: //bilinearrotationquat4_7bit
                            buffer_size = 4;
                            bit_size = 7;
                            BuildTheKeyFrameBuffer(NewTrack, WorkingTrack, i, p);
                            break;
                        default:
                            break;

                    }
                }
                /*

                if (NewTrack.BufferSize != 0)
                {
                    //NewTrack.Buffer = NewBuffer.ToArray();
                    NewTrack.BufferSize = NewTrack.Buffer.Length;
                }
                */
            }








            M3a.Tracks.Add(NewTrack);
            return M3a;

        }

        public static LMTTrackNode BuildTheKeyFrameBuffer(LMTTrackNode NewTrack, List<KeyFrame> WorkingTrack, int i, int p)
        {


            List<byte> NewBuffer = new List<byte>();



            // The rest of it.
            int buffer_size = 0;
            int bit_size = 0;
            int bitmaskA = (2 ^ bit_size) - 1;
            int BitmaskB = (2 ^ (bit_size - 2)) - 1;
            var data = new float[4];
            int CurrentFrame = 0;
            switch (NewTrack.BufferType)
            {

                case 1: //singlevector3
                    MessageBox.Show("This BufferType " + WorkingTrack[0].TrackType + "\nis not supposed to have keyframes!");
                    break;

                case 2: //singlerotationquat3
                    MessageBox.Show("This BufferType " + WorkingTrack[0].TrackType + "\nis not supposed to have keyframes!");
                    buffer_size = 8;
                    bit_size = 14;

                    break;

                case 3: //linearvector3
                    MessageBox.Show("This BufferType " + WorkingTrack[0].TrackType + "\nis not supposed to have keyframes!");
                    buffer_size = 16;
                    bit_size = 32;

                    break;

                #region bilinearvector3_16bit
                case 4: //bilinearvector3_16bit

                    #region Extreme Calculation
                    double[] AllTheX = new double[WorkingTrack.Count];
                    double[] AllTheY = new double[WorkingTrack.Count];
                    double[] AllTheZ = new double[WorkingTrack.Count];
                    double[] AllTheW = new double[WorkingTrack.Count];

                    for (int n = 0; n < WorkingTrack.Count; n++)
                    {
                        AllTheX[n] = WorkingTrack[n].data.X;
                        AllTheY[n] = WorkingTrack[n].data.Y;
                        AllTheZ[n] = WorkingTrack[n].data.Z;
                        AllTheW[n] = WorkingTrack[n].data.W;

                    }

                    //The Max Extremes.
                    NewTrack.ExtremesArray = new float[8];
                    NewTrack.ExtremesArray[4] = Convert.ToSingle(AllTheX.Min());
                    NewTrack.ExtremesArray[5] = Convert.ToSingle(AllTheY.Min());
                    NewTrack.ExtremesArray[6] = Convert.ToSingle(AllTheZ.Min());
                    NewTrack.ExtremesArray[7] = Convert.ToSingle(AllTheW.Min());

                    //The Min Extremes. Gotta do a thing first.
                    for (int n = 0; n < WorkingTrack.Count; n++)
                    {
                        AllTheX[n] = AllTheX[n] - NewTrack.ExtremesArray[4];
                        AllTheY[n] = AllTheY[n] - NewTrack.ExtremesArray[5];
                        AllTheZ[n] = AllTheZ[n] - NewTrack.ExtremesArray[6];
                        AllTheW[n] = AllTheW[n] - NewTrack.ExtremesArray[7];

                    }

                    NewTrack.ExtremesArray[0] = Convert.ToSingle(AllTheX.Max());
                    NewTrack.ExtremesArray[1] = Convert.ToSingle(AllTheY.Max());
                    NewTrack.ExtremesArray[2] = Convert.ToSingle(AllTheZ.Max());
                    NewTrack.ExtremesArray[3] = Convert.ToSingle(AllTheW.Max());

                    //Checks if these extreme values are 0; Cannot have them as zero because the algebreic forumlas depend on them NOT being zero.
                    if (NewTrack.ExtremesArray[0] == 0) NewTrack.ExtremesArray[0] = 0.0001F;
                    if (NewTrack.ExtremesArray[1] == 0) NewTrack.ExtremesArray[1] = 0.0001F;
                    if (NewTrack.ExtremesArray[2] == 0) NewTrack.ExtremesArray[2] = 0.0001F;
                    if (NewTrack.ExtremesArray[3] == 0) NewTrack.ExtremesArray[3] = 0.0001F;

                    #endregion

                    buffer_size = 8;
                    bit_size = 16;

                    //From Keyframes to removing the extremes from the values.
                    for (int k = 0; k < WorkingTrack.Count; k++)
                    {
                        data[0] = (WorkingTrack[k].data.X / NewTrack.ExtremesArray[0]) - (NewTrack.ExtremesArray[4] / NewTrack.ExtremesArray[0]);
                        data[1] = (WorkingTrack[k].data.Y / NewTrack.ExtremesArray[1]) - (NewTrack.ExtremesArray[5] / NewTrack.ExtremesArray[1]);
                        data[2] = (WorkingTrack[k].data.Z / NewTrack.ExtremesArray[2]) - (NewTrack.ExtremesArray[6] / NewTrack.ExtremesArray[2]);
                        //The W is assumed to be 1.0. in Vector3 type coordinates.


                        //"Packing" the bytes.
                        uint[] vec2bin = new uint[3];
                        vec2bin[0] = Convert.ToUInt32(65520 * data[0] + 8);
                        vec2bin[1] = Convert.ToUInt32(65520 * data[1] + 8);
                        vec2bin[2] = Convert.ToUInt32(65520 * data[2] + 8);

                        /*
                        //"Packing" the bytes.
                        uint[] vec2bin = new uint[3];
                        vec2bin[0] = Convert.ToUInt32(65520 * data[0] + 8);
                        vec2bin[1] = Convert.ToUInt32(65520 * data[1] + 8);
                        vec2bin[2] = Convert.ToUInt32(65520 * data[2] + 8);
                        */

                        //Taking the byte data and storing it a long binary string, the reverse of what happens in the other Class.
                        string BinaryString = "";
                        string BinaryStringA = "";
                        string BinaryStringB = "";
                        string BinaryStringC = "";
                        string BinaryStringFrame = "";
                        BinaryStringA = Convert.ToString(vec2bin[2], 2);
                        BinaryStringB = BinaryString + Convert.ToString(vec2bin[1], 2);
                        BinaryStringC = BinaryString + Convert.ToString(vec2bin[0], 2);

                        BinaryStringA = CheckLength4(BinaryStringA, bit_size);
                        BinaryStringB = CheckLength4(BinaryStringB, bit_size);
                        BinaryStringC = CheckLength4(BinaryStringC, bit_size);

                        if ((k + 1) == WorkingTrack.Count)
                        {
                            CurrentFrame = 0;
                        }
                        else
                        {
                            CurrentFrame = (WorkingTrack[k + 1].Frame - WorkingTrack[k].Frame);
                        }

                        BinaryStringFrame = Convert.ToString(CurrentFrame, 2);
                        BinaryStringFrame = CheckLength4(BinaryStringFrame, bit_size);
                        BinaryString = BinaryStringFrame + BinaryStringA + BinaryStringB + BinaryStringC;

                        byte[] bytes = new byte[buffer_size];
                        for (int m = 0; m < buffer_size; ++m)
                        {
                            bytes[m] = Convert.ToByte(BinaryString.Substring(8 * m, 8), 2);
                        }
                        Array.Reverse(bytes);
                        NewBuffer.AddRange(bytes);

                    }


                    NewTrack.Buffer = NewBuffer.ToArray();
                    NewTrack.BufferSize = NewBuffer.Count;

#if DEBUG

                    File.WriteAllBytes("D:\\Workshop\\LMTHub\\Test\\TrackBufferTest_4" + p + ".bin", NewTrack.Buffer);

#endif

                    break;
                #endregion

                #region bilinearvector3_8bit

                case 5://bilinearvector3_8bit
                    buffer_size = 4;
                    bit_size = 8;

                    #region Extreme Calculation
                    double[] AllTheX5 = new double[WorkingTrack.Count];
                    double[] AllTheY5 = new double[WorkingTrack.Count];
                    double[] AllTheZ5 = new double[WorkingTrack.Count];
                    double[] AllTheW5 = new double[WorkingTrack.Count];

                    for (int n = 0; n < WorkingTrack.Count; n++)
                    {
                        AllTheX5[n] = WorkingTrack[n].data.X;
                        AllTheY5[n] = WorkingTrack[n].data.Y;
                        AllTheZ5[n] = WorkingTrack[n].data.Z;
                        AllTheW5[n] = WorkingTrack[n].data.W;

                    }

                    //The Max Extremes.
                    NewTrack.ExtremesArray = new float[8];
                    NewTrack.ExtremesArray[4] = Convert.ToSingle(AllTheX5.Min());
                    NewTrack.ExtremesArray[5] = Convert.ToSingle(AllTheY5.Min());
                    NewTrack.ExtremesArray[6] = Convert.ToSingle(AllTheZ5.Min());
                    NewTrack.ExtremesArray[7] = Convert.ToSingle(AllTheW5.Min());

                    //The Min Extremes. Gotta do a thing first.
                    for (int n = 0; n < WorkingTrack.Count; n++)
                    {
                        AllTheX5[n] = AllTheX5[n] - NewTrack.ExtremesArray[4];
                        AllTheY5[n] = AllTheY5[n] - NewTrack.ExtremesArray[5];
                        AllTheZ5[n] = AllTheZ5[n] - NewTrack.ExtremesArray[6];
                        AllTheW5[n] = AllTheW5[n] - NewTrack.ExtremesArray[7];

                    }

                    NewTrack.ExtremesArray[0] = Convert.ToSingle(AllTheX5.Max());
                    NewTrack.ExtremesArray[1] = Convert.ToSingle(AllTheY5.Max());
                    NewTrack.ExtremesArray[2] = Convert.ToSingle(AllTheZ5.Max());
                    NewTrack.ExtremesArray[3] = Convert.ToSingle(AllTheW5.Max());

                    //Checks if these extreme values are 0; Cannot have them as zero because the algebreic forumlas depend on them NOT being zero.
                    if (NewTrack.ExtremesArray[0] == 0) NewTrack.ExtremesArray[0] = 0.0001F;
                    if (NewTrack.ExtremesArray[1] == 0) NewTrack.ExtremesArray[1] = 0.0001F;
                    if (NewTrack.ExtremesArray[2] == 0) NewTrack.ExtremesArray[2] = 0.0001F;
                    if (NewTrack.ExtremesArray[3] == 0) NewTrack.ExtremesArray[3] = 0.0001F;

                    #endregion


                    //From Keyframes to removing the extremes from the values.
                    for (int k = 0; k < WorkingTrack.Count; k++)
                    {

                        data[0] = (WorkingTrack[k].data.X / NewTrack.ExtremesArray[0]) - (NewTrack.ExtremesArray[4] / NewTrack.ExtremesArray[0]);
                        data[1] = (WorkingTrack[k].data.Y / NewTrack.ExtremesArray[1]) - (NewTrack.ExtremesArray[5] / NewTrack.ExtremesArray[1]);
                        data[2] = (WorkingTrack[k].data.Z / NewTrack.ExtremesArray[2]) - (NewTrack.ExtremesArray[6] / NewTrack.ExtremesArray[2]);
                        //The W is assumed to be 1.0. in Vector3 type coordinates.

                        //"Packing" the bytes.
                        uint[] vec2bin = new uint[3];
                        vec2bin[0] = Convert.ToUInt32(240 * data[0] + 8);
                        vec2bin[1] = Convert.ToUInt32(240 * data[1] + 8);
                        vec2bin[2] = Convert.ToUInt32(240 * data[2] + 8);

                        //Taking the byte data and storing it a long binary string, the reverse of what happens in the other Class.
                        string BinaryString = "";
                        string BinaryStringA = "";
                        string BinaryStringB = "";
                        string BinaryStringC = "";
                        string BinaryStringFrame = "";
                        BinaryStringA = Convert.ToString(vec2bin[2], 2);
                        BinaryStringB = BinaryString + Convert.ToString(vec2bin[1], 2);
                        BinaryStringC = BinaryString + Convert.ToString(vec2bin[0], 2);

                        BinaryStringA = CheckLength4(BinaryStringA, bit_size);
                        BinaryStringB = CheckLength4(BinaryStringB, bit_size);
                        BinaryStringC = CheckLength4(BinaryStringC, bit_size);

                        if ((k + 1) == WorkingTrack.Count)
                        {
                            CurrentFrame = 0;
                        }
                        else
                        {
                            CurrentFrame = (WorkingTrack[k + 1].Frame - WorkingTrack[k].Frame);
                        }

                        BinaryStringFrame = Convert.ToString(CurrentFrame, 2);
                        BinaryStringFrame = CheckLength4(BinaryStringFrame, bit_size);
                        BinaryString = BinaryStringFrame + BinaryStringA + BinaryStringB + BinaryStringC;

                        byte[] bytes = new byte[buffer_size];
                        for (int m = 0; m < buffer_size; ++m)
                        {
                            bytes[m] = Convert.ToByte(BinaryString.Substring(8 * m, 8), 2);
                        }
                        Array.Reverse(bytes);
                        NewBuffer.AddRange(bytes);

                    }

                    NewTrack.Buffer = NewBuffer.ToArray();
                    NewTrack.BufferSize = NewBuffer.Count;
#if DEBUG

                    File.WriteAllBytes("D:\\Workshop\\LMTHub\\Test\\TrackBufferTest_5" + p + ".bin", NewTrack.Buffer);

#endif

                    break;

                #endregion

                #region linearrotationquat4_14bit
                case 6: //linearrotationquat4_14bit

                    buffer_size = 8;
                    bit_size = 14;
                    int TempA = 0;
                    int TempB = 0;
                    int TempC = 0;
                    int TempD = 0;

                    //No Extremes here.
                    for (int k = 0; k < WorkingTrack.Count; k++)
                    {
                        TempA = 0;
                        TempB = 0;
                        TempC = 0;
                        TempD = 0;

                        data[0] = (WorkingTrack[k].data.X);
                        data[1] = (WorkingTrack[k].data.Y);
                        data[2] = (WorkingTrack[k].data.Z);
                        data[3] = (WorkingTrack[k].data.W);

                        //"Packing" the bytes.
                        uint[] vec2bin = new uint[4];

                        TempA = Convert.ToInt32(data[0] * 4095);
                        TempB = Convert.ToInt32(data[1] * 4095);
                        TempC = Convert.ToInt32(data[2] * 4095);
                        TempD = Convert.ToInt32(data[3] * 4095);

                        int TestA = TempA & 18;
                        int TestB = ((1 << 14) - 1) & (TempA >> (18 - 1));
                        int TestC = TempA >> 18;

                        //Taking the byte data and storing it a long binary string, the reverse of what happens in the other Class.
                        string BinaryString = "";
                        string BinaryStringA = "";
                        string BinaryStringB = "";
                        string BinaryStringC = "";
                        string BinaryStringD = "";
                        string BinaryStringFrame = "";


                        BinaryStringA = Convert.ToString(TempA, 2);
                        BinaryStringB = Convert.ToString(TempB, 2);
                        BinaryStringC = Convert.ToString(TempC, 2);
                        BinaryStringD = Convert.ToString(TempD, 2);

                        //Extracts the bits we care about and discards the rest.
                        if (BinaryStringA.Length < 32)
                        {
                            BinaryStringA = CheckLength4(BinaryStringA, 32);
                        }
                        BinaryStringA = BinaryStringA.Substring(18, 14);


                        if (BinaryStringB.Length < 32)
                        {
                            BinaryStringB = CheckLength4(BinaryStringB, 32);
                        }
                        BinaryStringB = BinaryStringB.Substring(18, 14);


                        if (BinaryStringC.Length < 32)
                        {
                            BinaryStringC = CheckLength4(BinaryStringC, 32);
                        }
                        BinaryStringC = BinaryStringC.Substring(18, 14);

                        if (BinaryStringD.Length < 32)
                        {
                            BinaryStringD = CheckLength4(BinaryStringD, 32);
                        }
                        BinaryStringD = BinaryStringD.Substring(18, 14);

                        BinaryStringA = CheckLength4(BinaryStringA, bit_size);
                        BinaryStringB = CheckLength4(BinaryStringB, bit_size);
                        BinaryStringC = CheckLength4(BinaryStringC, bit_size);
                        BinaryStringD = CheckLength4(BinaryStringD, bit_size);

                        if ((k + 1) == WorkingTrack.Count)
                        {
                            CurrentFrame = 0;
                        }
                        else
                        {
                            CurrentFrame = (WorkingTrack[k + 1].Frame - WorkingTrack[k].Frame);
                        }

                        BinaryStringFrame = Convert.ToString(CurrentFrame, 2);
                        BinaryStringFrame = CheckLength4(BinaryStringFrame, 8);
                        BinaryString = BinaryStringFrame + BinaryStringA + BinaryStringB + BinaryStringC + BinaryStringD;

                        byte[] bytes = new byte[buffer_size];
                        for (int m = 0; m < buffer_size; ++m)
                        {
                            bytes[m] = Convert.ToByte(BinaryString.Substring(8 * m, 8), 2);
                        }
                        Array.Reverse(bytes);
                        NewBuffer.AddRange(bytes);

                    }


                    NewTrack.Buffer = NewBuffer.ToArray();
                    NewTrack.BufferSize = NewBuffer.Count;

#if DEBUG

                    File.WriteAllBytes("D:\\Workshop\\LMTHub\\Test\\TrackBufferTest2_6" + i + ".bin", NewTrack.Buffer);

#endif

                    break;
                #endregion

                #region bilinearrotationquat4_7bit
                case 7: //bilinearrotationquat4_7bit

                    buffer_size = 4;
                    bit_size = 7;

                    int Temp7A = 0;
                    int Temp7B = 0;
                    int Temp7C = 0;
                    int Temp7D = 0;

                    #region Extreme Calculation
                    double[] AllTheX7 = new double[WorkingTrack.Count];
                    double[] AllTheY7 = new double[WorkingTrack.Count];
                    double[] AllTheZ7 = new double[WorkingTrack.Count];
                    double[] AllTheW7 = new double[WorkingTrack.Count];

                    for (int n = 0; n < WorkingTrack.Count; n++)
                    {
                        AllTheX7[n] = WorkingTrack[n].data.X;
                        AllTheY7[n] = WorkingTrack[n].data.Y;
                        AllTheZ7[n] = WorkingTrack[n].data.Z;
                        AllTheW7[n] = WorkingTrack[n].data.W;

                    }

                    //The Max Extremes.
                    NewTrack.ExtremesArray = new float[8];
                    NewTrack.ExtremesArray[4] = Convert.ToSingle(AllTheX7.Min());
                    NewTrack.ExtremesArray[5] = Convert.ToSingle(AllTheY7.Min());
                    NewTrack.ExtremesArray[6] = Convert.ToSingle(AllTheZ7.Min());
                    NewTrack.ExtremesArray[7] = Convert.ToSingle(AllTheW7.Min());

                    //The Min Extremes. Gotta do a thing first.
                    for (int n = 0; n < WorkingTrack.Count; n++)
                    {
                        AllTheX7[n] = AllTheX7[n] - NewTrack.ExtremesArray[4];
                        AllTheY7[n] = AllTheY7[n] - NewTrack.ExtremesArray[5];
                        AllTheZ7[n] = AllTheZ7[n] - NewTrack.ExtremesArray[6];
                        AllTheW7[n] = AllTheW7[n] - NewTrack.ExtremesArray[7];

                    }

                    NewTrack.ExtremesArray[0] = Convert.ToSingle(AllTheX7.Max());
                    NewTrack.ExtremesArray[1] = Convert.ToSingle(AllTheY7.Max());
                    NewTrack.ExtremesArray[2] = Convert.ToSingle(AllTheZ7.Max());
                    NewTrack.ExtremesArray[3] = Convert.ToSingle(AllTheW7.Max());

                    //Checks if these extreme values are 0; Cannot have them as zero because the algebreic forumlas depend on them NOT being zero.
                    if (NewTrack.ExtremesArray[0] == 0) NewTrack.ExtremesArray[0] = 0.0001F;
                    if (NewTrack.ExtremesArray[1] == 0) NewTrack.ExtremesArray[1] = 0.0001F;
                    if (NewTrack.ExtremesArray[2] == 0) NewTrack.ExtremesArray[2] = 0.0001F;
                    if (NewTrack.ExtremesArray[3] == 0) NewTrack.ExtremesArray[3] = 0.0001F;

                    #endregion

                    //"Packing" the bytes.
                    //From Keyframes to removing the extremes from the values.
                    for (int k = 0; k < WorkingTrack.Count; k++)
                    {
                        data[0] = (WorkingTrack[k].data.X / NewTrack.ExtremesArray[0]) - (NewTrack.ExtremesArray[4] / NewTrack.ExtremesArray[0]);
                        data[1] = (WorkingTrack[k].data.Y / NewTrack.ExtremesArray[1]) - (NewTrack.ExtremesArray[5] / NewTrack.ExtremesArray[1]);
                        data[2] = (WorkingTrack[k].data.Z / NewTrack.ExtremesArray[2]) - (NewTrack.ExtremesArray[6] / NewTrack.ExtremesArray[2]);
                        data[3] = (WorkingTrack[k].data.W / NewTrack.ExtremesArray[3]) - (NewTrack.ExtremesArray[7] / NewTrack.ExtremesArray[3]);

                        //"Packing" the bytes.
                        uint[] vec2bin = new uint[4];
                        vec2bin[0] = Convert.ToUInt32(112 * data[0] + 8);
                        vec2bin[1] = Convert.ToUInt32(112 * data[1] + 8);
                        vec2bin[2] = Convert.ToUInt32(112 * data[2] + 8);
                        vec2bin[3] = Convert.ToUInt32(112 * data[3] + 8);

                        //Taking the byte data and storing it a long binary string, the reverse of what happens in the other Class.
                        string BinaryString = "";
                        string BinaryStringA = "";
                        string BinaryStringB = "";
                        string BinaryStringC = "";
                        string BinaryStringD = "";

                        string BinaryStringFrame = "";
                        BinaryStringA = Convert.ToString(vec2bin[0], 2);
                        BinaryStringB = BinaryString + Convert.ToString(vec2bin[1], 2);
                        BinaryStringC = BinaryString + Convert.ToString(vec2bin[2], 2);
                        BinaryStringD = BinaryString + Convert.ToString(vec2bin[3], 2);

                        BinaryStringA = CheckLength4(BinaryStringA, bit_size);
                        BinaryStringB = CheckLength4(BinaryStringB, bit_size);
                        BinaryStringC = CheckLength4(BinaryStringC, bit_size);
                        BinaryStringD = CheckLength4(BinaryStringD, bit_size);

                        if ((k + 1) == WorkingTrack.Count)
                        {
                            CurrentFrame = 0;
                        }
                        else
                        {
                            CurrentFrame = (WorkingTrack[k + 1].Frame - WorkingTrack[k].Frame);
                        }

                        BinaryStringFrame = Convert.ToString(CurrentFrame, 2);
                        BinaryStringFrame = CheckLength4(BinaryStringFrame, 4);
                        BinaryString = BinaryStringFrame + BinaryStringA + BinaryStringB + BinaryStringC + BinaryStringD;

                        byte[] bytes = new byte[buffer_size];
                        for (int m = 0; m < buffer_size; ++m)
                        {
                            bytes[m] = Convert.ToByte(BinaryString.Substring(8 * m, 8), 2);
                        }
                        Array.Reverse(bytes);
                        NewBuffer.AddRange(bytes);

                    }

                    NewTrack.Buffer = NewBuffer.ToArray();
                    NewTrack.BufferSize = NewBuffer.Count;

#if DEBUG

                    File.WriteAllBytes("D:\\Workshop\\LMTHub\\Test\\TrackBufferTest_7" + p + ".bin", NewTrack.Buffer);

#endif


                    break;

                #endregion

                default:

                    break;


            }



            return NewTrack;

        }

        //String gotta be 16 bits so this checks for strings less than that for type 4.
        public static string CheckLength4(string s, int Length)
        {
            string NewString = "";
            if (s.Length < Length)
            {
                int AmountToAdd = Length - s.Length;
                for (int l = 0; l < AmountToAdd; l++)
                {
                    NewString = NewString + "0";
                }
                NewString = NewString + s;

            }
            else
            {
                return s;
            }
            return NewString;
        }

        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
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

            //Updates the Scene Additive Position.
            var EFAPArray = new float[] { M3a.AdditiveScenePositionX, M3a.AdditiveScenePositionY, M3a.AdditiveScenePositionZ, M3a.AdditiveScenePositionW };
            var byteArray = new byte[16];
            Buffer.BlockCopy( EFAPArray ,0,byteArray,0,byteArray.Length);
            Array.Copy(byteArray, 0, M3a.MotionData,32,byteArray.Length);

            return M3a;

        }

        [Category("Motion - Scene")]
        [YamlIgnore]
        public float AdditiveScenePositionX
        {

            get
            {
                return EndFramesAdditiveScenePosition.X;
            }
            set
            {
                EndFramesAdditiveScenePosition.X = value;

            }
        }

        [Category("Motion - Scene")]
        [YamlIgnore]
        public float AdditiveScenePositionY
        {

            get
            {
                return EndFramesAdditiveScenePosition.Y;
            }
            set
            {
                EndFramesAdditiveScenePosition.Y = value;

            }
        }

        [Category("Motion - Scene")]
        [YamlIgnore]
        public float AdditiveScenePositionZ
        {

            get
            {
                return EndFramesAdditiveScenePosition.Z;
            }
            set
            {
                EndFramesAdditiveScenePosition.Z = value;

            }
        }

        [Category("Motion - Scene")]
        [YamlIgnore]
        public float AdditiveScenePositionW
        {

            get
            {
                return EndFramesAdditiveScenePosition.W;
            }
            set
            {
                EndFramesAdditiveScenePosition.W = value;

            }
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
