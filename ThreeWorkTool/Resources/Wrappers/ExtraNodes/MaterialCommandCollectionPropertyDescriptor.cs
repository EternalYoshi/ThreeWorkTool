using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeWorkTool.Resources.Wrappers.ExtraNodes;
using static ThreeWorkTool.Resources.Wrappers.MaterialMaterialEntry;

namespace ThreeWorkTool.Resources.Wrappers.ExtraNodes
{
    public class MaterialCommandCollectionPropertyDescriptor : PropertyDescriptor
    {

        private MaterialCommandCollection collection = null;
        private int index = -1;

        public MaterialCommandCollectionPropertyDescriptor(MaterialCommandCollection coll, int idx) :
    base("#" + idx.ToString(), null)
        {
            this.collection = coll;
            this.index = idx;
        }

        public override AttributeCollection Attributes
        {
            get
            {
                return new AttributeCollection(null);
            }
        }

        public override bool CanResetValue(object component)
        {
            return true;
        }

        public override Type ComponentType
        {
            get
            {
                return this.collection.GetType();
            }
        }

        public override string DisplayName
        {
            get
            {
                MatCmd cmd = this.collection[index];
                return ("Cmd" + cmd.cmdindex.ToString());
            }
        }

        public override string Description
        {
            get
            {
                MatCmd cmd = this.collection[index];
                StringBuilder sb = new StringBuilder();
                sb.Append(cmd.MCInfo.CmdFlag);
                sb.Append(", ");
                sb.Append(cmd.MaterialCommandData.VShaderObjectID.Hash);
                sb.Append(", ");
                sb.Append(cmd.CmdName);
                sb.Append(", ");
                return sb.ToString();
            }
        }

        public override object GetValue(object component)
        {
            return this.collection[index];
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override string Name
        {
            get { return "#" + index.ToString(); }
        }

        public override Type PropertyType
        {
            get { return this.collection[index].GetType(); }
        }

        public override void ResetValue(object component)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }

        public override void SetValue(object component, object value)
        {
            // this.collection[index] = value;
        }

    }
}
