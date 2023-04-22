using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ThreeWorkTool.Resources.Archives;
using ThreeWorkTool.Resources.Wrappers;

namespace ThreeWorkTool.Resources.Wrappers.ModelNodes
{
    public class ModelGroupEntry : DefaultWrapper
    {
        public int ID;
        public int PrimitiveCount;
        public string GroupName;
        public List<string> PrimitiveNames;
        public int Field04;
        public int Field08;
        public int Field0C;
        public BoundingSphere SphereBound;

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public struct BoundingSphere
        {
            public Vector3 Center;
            public float Radius { get; set; }
        }

        public ModelGroupEntry FillModelGroupEntry(ModelGroupEntry MGE, ModelEntry ParentMod, BinaryReader bnr, int OffsetToStart, int ID)
        {

            MGE.ID = bnr.ReadInt32();
            MGE.Field04 = bnr.ReadInt32();
            MGE.Field08 = bnr.ReadInt32();
            MGE.Field0C = bnr.ReadInt32();

            MGE.SphereBound = new BoundingSphere();
            MGE.SphereBound.Center.X = bnr.ReadSingle();
            MGE.SphereBound.Center.Y = bnr.ReadSingle();
            MGE.SphereBound.Center.Z = bnr.ReadSingle();
            MGE.SphereBound.Radius = bnr.ReadSingle();

            OffsetToStart = Convert.ToInt32(bnr.BaseStream.Position);

            return MGE;

        }

        #region Model Group Entry Properties

        [Category("Group"), ReadOnlyAttribute(true)]
        public float Radius 
        {

            get
            {
                return SphereBound.Radius;
            }
            set
            {
                SphereBound.Radius = value;
            }
        }



        #endregion



    }
}
