using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Archives;
using ThreeWorkTool.Resources.Utility;
using ThreeWorkTool.Resources.Wrappers;
using ThreeWorkTool.Resources.Wrappers.ExtraNodes;

namespace ThreeWorkTool.Resources.Wrappers
{
    public class MissionEntry : DefaultWrapper
    {
        public string Magic;
        public string Constant;
        public int MissionCount;
        public int UnknownOC;
        public List<Mission> Missions;

        public static MissionEntry FillMissionEntry(string filename, List<string> subnames, TreeView tree, BinaryReader br, int c, int ID, Type filetype = null)
        {
            MissionEntry missionEntry = new MissionEntry();

            FillEntry(filename, subnames, tree, br, c, ID, missionEntry);

            //Specific file type work goes here!
            using (MemoryStream MisStream = new MemoryStream(missionEntry.UncompressedData))
            {
                using (BinaryReader bnr = new BinaryReader(MisStream))
                {
                    BuildMissionEntry(bnr, missionEntry);
                }
            }

            return missionEntry;
        }

        public static MissionEntry ReplaceMIS(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {
            MissionEntry MISNentry = new MissionEntry();
            MissionEntry MISoldentry = new MissionEntry();

            tree.BeginUpdate();

            ReplaceEntry(tree, node, filename, MISNentry, MISoldentry);

            //Type Specific Work Here.
            using (MemoryStream LmtStream = new MemoryStream(MISNentry.UncompressedData))
            {
                using (BinaryReader bnr = new BinaryReader(LmtStream))
                {
                    BuildMissionEntry(bnr, MISNentry);
                }
            }

            return node.entryfile as MissionEntry;
        }

        public static MissionEntry InsertMissionEntry(TreeView tree, ArcEntryWrapper node, string filename, Type filetype = null)
        {

            MissionEntry mission = new MissionEntry();

            InsertEntry(tree, node, filename, mission);

            //Decompression Time.
            mission.UncompressedData = ZlibStream.UncompressBuffer(mission.CompressedData);

            try
            {
                using (BinaryReader bnr = new BinaryReader(File.OpenRead(filename)))
                {
                    BuildMissionEntry(bnr, mission);
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText("Log.txt"))
                {
                    sw.WriteLine("Caught an exception using the BinaryReader. Here's the details:\n" + ex);
                }
            }

            return mission;

        }

        public static MissionEntry BuildMissionEntry(BinaryReader bnr, MissionEntry misentry)
        {
            //Header.
            bnr.BaseStream.Position = 4;
            misentry.Constant = BitConverter.ToString(bnr.ReadBytes(4));
            misentry.MissionCount = bnr.ReadInt32();
            misentry.UnknownOC = bnr.ReadInt32();

            //Entries.
            misentry.Missions = new List<Mission>();
            int PreviousOffset = Convert.ToInt32(bnr.BaseStream.Position);
            for (int i = 0; i < misentry.MissionCount; i++)
            {
                bnr.BaseStream.Position = PreviousOffset;
                Mission mis = new Mission();
                mis.Unknown00 = bnr.ReadInt32();
                mis.DataOffset = bnr.ReadInt32();

                bnr.BaseStream.Position = mis.DataOffset;
                mis.ID = bnr.ReadInt32();
                mis.WeirdDescriptiveString = Encoding.ASCII.GetString(bnr.ReadBytes(64)).Trim('\0');
                mis.Unknown44 = bnr.ReadInt32();
                bnr.BaseStream.Position = bnr.BaseStream.Position + 32;

                mis.P1PointCharacterID = bnr.ReadInt32();
                bnr.BaseStream.Position = bnr.BaseStream.Position + 8;
                mis.P1PointCharacterAIFlag = bnr.ReadInt32();
                mis.P1PointCharacterAssistType = bnr.ReadInt32();
                mis.P1PointCharacterUnkownParamA = bnr.ReadInt32();
                bnr.BaseStream.Position = bnr.BaseStream.Position + 16;
                mis.P1PointCharacterUnkownParamB = bnr.ReadSingle();
                bnr.BaseStream.Position = bnr.BaseStream.Position + 8;

                mis.P1Assist1CharID = bnr.ReadInt32();
                bnr.BaseStream.Position = bnr.BaseStream.Position + 8;
                mis.P1Assist1CharAIFlag = bnr.ReadInt32();
                mis.P1Assist1CharAssistType = bnr.ReadInt32();
                mis.P1Assist1CharUnkownParamA = bnr.ReadInt32();
                bnr.BaseStream.Position = bnr.BaseStream.Position + 16;
                mis.P1Assist1CharUnkownParamB = bnr.ReadSingle();
                bnr.BaseStream.Position = bnr.BaseStream.Position + 8;

                mis.P1Assist2CharID = bnr.ReadInt32();
                bnr.BaseStream.Position = bnr.BaseStream.Position + 8;
                mis.P1Assist2CharAIFlag = bnr.ReadInt32();
                mis.P1Assist2CharAssistType = bnr.ReadInt32();
                mis.P1Assist2CharUnkownParamA = bnr.ReadInt32();
                bnr.BaseStream.Position = bnr.BaseStream.Position + 16;
                mis.P1Assist2CharUnkownParamB = bnr.ReadSingle();
                bnr.BaseStream.Position = bnr.BaseStream.Position + 4;

                mis.Unknown100 = bnr.ReadInt32();

                mis.P2PointCharacterID = bnr.ReadInt32();
                bnr.BaseStream.Position = bnr.BaseStream.Position + 8;
                mis.P2PointCharacterAIFlag = bnr.ReadInt32();
                mis.P2PointCharacterAssistType = bnr.ReadInt32();
                mis.P2PointCharacterUnkownParamA = bnr.ReadInt32();
                bnr.BaseStream.Position = bnr.BaseStream.Position + 16;
                mis.P2PointCharacterUnkownParamB = bnr.ReadSingle();
                bnr.BaseStream.Position = bnr.BaseStream.Position + 8;

                mis.P2Assist1CharID = bnr.ReadInt32();
                bnr.BaseStream.Position = bnr.BaseStream.Position + 8;
                mis.P2Assist1CharAIFlag = bnr.ReadInt32();
                mis.P2Assist1CharAssistType = bnr.ReadInt32();
                mis.P2Assist1CharUnkownParamA = bnr.ReadInt32();
                bnr.BaseStream.Position = bnr.BaseStream.Position + 16;
                mis.P2Assist1CharUnkownParamB = bnr.ReadSingle();
                bnr.BaseStream.Position = bnr.BaseStream.Position + 8;

                mis.P2Assist2CharID = bnr.ReadInt32();
                bnr.BaseStream.Position = bnr.BaseStream.Position + 8;
                mis.P2Assist2CharAIFlag = bnr.ReadInt32();
                mis.P2Assist2CharAssistType = bnr.ReadInt32();
                mis.P2Assist2CharUnkownParamA = bnr.ReadInt32();
                bnr.BaseStream.Position = bnr.BaseStream.Position + 16;
                mis.P2Assist2CharUnkownParamB = bnr.ReadSingle();
                bnr.BaseStream.Position = bnr.BaseStream.Position + 8;

                mis.MovePartPointer = bnr.ReadInt32();

                mis.MovePartString = Encoding.ASCII.GetString(bnr.ReadBytes(64)).Trim('\0');

                bnr.BaseStream.Position = bnr.BaseStream.Position + 284;

                mis.ComboListString = Encoding.ASCII.GetString(bnr.ReadBytes(64)).Trim('\0');

                mis.ComboListFlagA = bnr.ReadInt32();
                mis.ComboListFlagB = bnr.ReadInt32();

                mis.AnmChrMoveIDList = new int[82];
                int Move;
                for (int j = 0; j < 82; j++)
                {

                    Move = bnr.ReadInt32();
                    mis.AnmChrMoveIDList[j] = Move;
                }

                misentry.Missions.Add(mis);
                PreviousOffset = PreviousOffset + 8;
            }


            return misentry;
        }

        public static MissionEntry SaveMissionEntry(MissionEntry misentry, TreeNode node)
        {

            using (MemoryStream MisStream = new MemoryStream(misentry.UncompressedData))
            {
                using (BinaryWriter bwr = new BinaryWriter(MisStream))
                {
                    //Gets All The Mission Files From Child Nodes variables and writes to the main mission file.
                    foreach (ArcEntryWrapper youngn in node.Nodes)
                    {
                        Mission msn = youngn.Tag as Mission;
                        bwr.BaseStream.Position = msn.DataOffset;
                        bwr.BaseStream.Position = bwr.BaseStream.Position + 104;
                        bwr.Write(msn.Player1PointCharacter);
                        bwr.BaseStream.Position = bwr.BaseStream.Position + 8;
                        bwr.Write(msn.P1PointCharacterAIFlag);
                        bwr.Write(msn.P1PointCharacterAssistType);
                        bwr.Write(msn.P1PointCharacterUnkownParamA);
                        bwr.BaseStream.Position = bwr.BaseStream.Position + 16;
                        bwr.Write(msn.P1PointCharacterUnkownParamB);
                        bwr.BaseStream.Position = bwr.BaseStream.Position + 8;

                        bwr.Write(msn.P1Assist1CharID);
                        bwr.BaseStream.Position = bwr.BaseStream.Position + 8;
                        bwr.Write(msn.P1Assist1CharAIFlag);
                        bwr.Write(msn.P1Assist1CharAssistType);
                        bwr.Write(msn.P1Assist1CharUnkownParamA);
                        bwr.BaseStream.Position = bwr.BaseStream.Position + 16;
                        bwr.Write(msn.P1Assist1CharUnkownParamB);
                        bwr.BaseStream.Position = bwr.BaseStream.Position + 8;

                        bwr.Write(msn.P1Assist2CharID);
                        bwr.BaseStream.Position = bwr.BaseStream.Position + 8;
                        bwr.Write(msn.P1Assist2CharAIFlag);
                        bwr.Write(msn.P1Assist2CharAssistType);
                        bwr.Write(msn.P1Assist2CharUnkownParamA);
                        bwr.BaseStream.Position = bwr.BaseStream.Position + 16;
                        bwr.Write(msn.P1Assist2CharUnkownParamB);
                        bwr.BaseStream.Position = bwr.BaseStream.Position + 4;

                        bwr.Write(msn.Unknown100);

                        bwr.Write(msn.P2PointCharacterID);
                        bwr.BaseStream.Position = bwr.BaseStream.Position + 8;
                        bwr.Write(msn.P2PointCharacterAIFlag);
                        bwr.Write(msn.P2PointCharacterAssistType);
                        bwr.Write(msn.P2PointCharacterUnkownParamA);
                        bwr.BaseStream.Position = bwr.BaseStream.Position + 16;
                        bwr.Write(msn.P2PointCharacterUnkownParamB);
                        bwr.BaseStream.Position = bwr.BaseStream.Position + 8;

                        bwr.Write(msn.P2Assist1CharID);
                        bwr.BaseStream.Position = bwr.BaseStream.Position + 8;
                        bwr.Write(msn.P2Assist1CharAIFlag);
                        bwr.Write(msn.P2Assist1CharAssistType);
                        bwr.Write(msn.P2Assist1CharUnkownParamA);
                        bwr.BaseStream.Position = bwr.BaseStream.Position + 16;
                        bwr.Write(msn.P2Assist1CharUnkownParamB);
                        bwr.BaseStream.Position = bwr.BaseStream.Position + 8;

                        bwr.Write(msn.P2Assist2CharID);
                        bwr.BaseStream.Position = bwr.BaseStream.Position + 8;
                        bwr.Write(msn.P2Assist2CharAIFlag);
                        bwr.Write(msn.P2Assist2CharAssistType);
                        bwr.Write(msn.P2Assist2CharUnkownParamA);
                        bwr.BaseStream.Position = bwr.BaseStream.Position + 16;
                        bwr.Write(msn.P2Assist2CharUnkownParamB);
                        bwr.BaseStream.Position = bwr.BaseStream.Position + 424;

                        bwr.Write(msn.ComboListFlagA);
                        bwr.Write(msn.ComboListFlagB);

                        for (int j = 0; j < 82; j++)
                        {
                            bwr.Write(msn.AnmChrMoveIDList[j]);
                        }

                    }
                }
            }

            misentry.CompressedData = Zlibber.Compressor(misentry.UncompressedData);


            return misentry;

        }

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

        [Category("Filename"), ReadOnlyAttribute(true)]
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

        [Category("Mission"), ReadOnlyAttribute(true)]
        public int MissionTotal
        {

            get
            {
                return MissionCount;
            }
            set
            {
                MissionCount = value;
            }
        }



    }
}
