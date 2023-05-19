using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;

namespace ThreeWorkTool.Resources.Wrappers.ExtraNodes
{
    public class Vector4CollectionPropertyDescriptor : PropertyDescriptor
    {
        private Vector4Collection collection = null;
        private int index = -1;

        public Vector4CollectionPropertyDescriptor(Vector4Collection coll, int idx) :
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
                Vector4 vex4 = this.collection[index];
                string strv = vex4.W.ToString() + "," + vex4.X.ToString() + "," + vex4.Y.ToString() + "," + vex4.Z.ToString();
                return strv;
            }
        }

        public override string Description
        {
            get
            {
                Vector4 vex4 = this.collection[index];
                StringBuilder sb = new StringBuilder();
                sb.Append(vex4.W.ToString());
                sb.Append(", ");
                sb.Append(vex4.X.ToString());
                sb.Append(", ");
                sb.Append(vex4.Y.ToString());
                sb.Append(", ");
                sb.Append(vex4.Z.ToString());
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
