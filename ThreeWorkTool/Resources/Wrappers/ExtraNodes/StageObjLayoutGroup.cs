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
    public class StageObjLayoutGroup : DefaultWrapper
    {

        public int DataOffset;
        public int UnknownFlags;
        public string GroupName;
        public byte[] BufferA;

        public Vector3 VectorA;
        public Vector3 VectorB;
        public Vector3 VectorC;

        public float SomeFloat1;
        public float SomeFloat2;
        public float SomeFloat3;
        
        public string FileReference1;
        public string FileReference2;
        public string FileReference3;
        public string FileReference4;
        public string FileReference5;
        public string FileReference6;
        public string FileReference7;

        public byte[] BufferFooter;

        public static StageObjLayoutGroup BuildSLOGroup(StageObjLayoutEntry sloentry, int ID, BinaryReader bnr, StageObjLayoutGroup slog)
        {
            slog.EntryID = ID;
            slog.DataOffset = Convert.ToInt32(bnr.BaseStream.Position);
            slog.UnknownFlags = bnr.ReadInt32();
            slog.GroupName = Encoding.ASCII.GetString(bnr.ReadBytes(32)).Trim('\0');
            slog.BufferA = bnr.ReadBytes(12);

            slog.VectorA = new Vector3();
            slog.VectorB = new Vector3();
            slog.VectorC = new Vector3();

            slog.VectorA.X = bnr.ReadSingle();
            slog.VectorA.Y = bnr.ReadSingle();
            slog.VectorA.Z = bnr.ReadSingle();
            slog.SomeFloat1 = bnr.ReadSingle();

            slog.VectorB.X = bnr.ReadSingle();
            slog.VectorB.Y = bnr.ReadSingle();
            slog.VectorB.Z = bnr.ReadSingle();
            slog.SomeFloat2 = bnr.ReadSingle();

            slog.VectorC.X = bnr.ReadSingle();
            slog.VectorC.Y = bnr.ReadSingle();
            slog.VectorC.Z = bnr.ReadSingle();
            slog.SomeFloat3 = bnr.ReadSingle();

            slog.FileReference1 = Encoding.ASCII.GetString(bnr.ReadBytes(64)).Trim('\0');
            slog.FileReference2 = Encoding.ASCII.GetString(bnr.ReadBytes(64)).Trim('\0');
            slog.FileReference3 = Encoding.ASCII.GetString(bnr.ReadBytes(64)).Trim('\0');
            slog.FileReference4 = Encoding.ASCII.GetString(bnr.ReadBytes(64)).Trim('\0');
            slog.FileReference5 = Encoding.ASCII.GetString(bnr.ReadBytes(64)).Trim('\0');
            slog.FileReference6 = Encoding.ASCII.GetString(bnr.ReadBytes(64)).Trim('\0');
            slog.FileReference7 = Encoding.ASCII.GetString(bnr.ReadBytes(64)).Trim('\0');

            slog.BufferFooter = bnr.ReadBytes(96);

            return slog;
        }

        [Category("Filename"), ReadOnlyAttribute(false)]
        public string PossibleGroupName
        {

            get
            {
                return GroupName;
            }
            set
            {
                GroupName = value;
            }
        }

        [Category("Vector1(Translation)"), ReadOnlyAttribute(false)]
        public float Vector1X
        {

            get
            {
                return VectorA.X;
            }
            set
            {
                VectorA.X = value;
            }
        }

        [Category("Vector1(Translation)"), ReadOnlyAttribute(false)]
        public float Vector1Y
        {

            get
            {
                return VectorA.Y;
            }
            set
            {
                VectorA.Y = value;
            }
        }

        [Category("Vector1(Translation)"), ReadOnlyAttribute(false)]
        public float Vector1Z
        {

            get
            {
                return VectorA.Z;
            }
            set
            {
                VectorA.Z = value;
            }
        }

        [Category("Vector2(Rotation)"), ReadOnlyAttribute(false)]
        public float Vector2X
        {

            get
            {
                return VectorB.X;
            }
            set
            {
                VectorB.X = value;
            }
        }

        [Category("Vector2(Rotation)"), ReadOnlyAttribute(false)]
        public float Vector2Y
        {

            get
            {
                return VectorB.Y;
            }
            set
            {
                VectorB.Y = value;
            }
        }

        [Category("Vector2(Rotation)"), ReadOnlyAttribute(false)]
        public float Vector2Z
        {

            get
            {
                return VectorB.Z;
            }
            set
            {
                VectorB.Z = value;
            }
        }

        [Category("Vector3(Scale)"), ReadOnlyAttribute(false)]
        public float Vector3X
        {

            get
            {
                return VectorC.X;
            }
            set
            {
                VectorC.X = value;
            }
        }

        [Category("Vector3(Scale)"), ReadOnlyAttribute(false)]
        public float Vector3Y
        {

            get
            {
                return VectorC.Y;
            }
            set
            {
                VectorC.Y = value;
            }
        }

        [Category("Vector3(Scale)"), ReadOnlyAttribute(false)]
        public float Vector3Z
        {

            get
            {
                return VectorC.Z;
            }
            set
            {
                VectorC.Z = value;
            }
        }


        [Category("Data"), ReadOnlyAttribute(false)]
        public float UnknownFloat1
        {

            get
            {
                return SomeFloat1;
            }
            set
            {
                SomeFloat1 = value;
            }
        }

        [Category("Data"), ReadOnlyAttribute(false)]
        public float UnknownFloat2
        {

            get
            {
                return SomeFloat2;
            }
            set
            {
                SomeFloat2 = value;
            }
        }

        [Category("Data"), ReadOnlyAttribute(false)]
        public float UnknownFloat3
        {

            get
            {
                return SomeFloat3;
            }
            set
            {
                SomeFloat3 = value;
            }
        }

        [Category("Filename"), ReadOnlyAttribute(false)]
        public string FileRef1
        {

            get
            {
                return FileReference1;
            }
            set
            {
                FileReference1 = value;
            }
        }

        [Category("Filename"), ReadOnlyAttribute(false)]
        public string FileRef2
        {

            get
            {
                return FileReference2;
            }
            set
            {
                FileReference2 = value;
            }
        }

        [Category("Filename"), ReadOnlyAttribute(false)]
        public string FileRef3
        {

            get
            {
                return FileReference3;
            }
            set
            {
                FileReference3 = value;
            }
        }

        [Category("Filename"), ReadOnlyAttribute(false)]
        public string FileRef4
        {

            get
            {
                return FileReference4;
            }
            set
            {
                FileReference4 = value;
            }
        }

        [Category("Filename"), ReadOnlyAttribute(false)]
        public string FileRef5
        {

            get
            {
                return FileReference5;
            }
            set
            {
                FileReference5 = value;
            }
        }

        [Category("Filename"), ReadOnlyAttribute(false)]
        public string FileRef6
        {

            get
            {
                return FileReference6;
            }
            set
            {
                FileReference6 = value;
            }
        }

        [Category("Filename"), ReadOnlyAttribute(false)]
        public string FileRef7
        {

            get
            {
                return FileReference7;
            }
            set
            {
                FileReference7 = value;
            }
        }

    }
}
