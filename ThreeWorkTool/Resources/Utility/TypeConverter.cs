using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using static ThreeWorkTool.Resources.Wrappers.MaterialMaterialEntry;
using ThreeWorkTool.Resources.Wrappers.ExtraNodes;

namespace ThreeWorkTool.Resources.Utility
{
    internal class MaterialCommandCollectionConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
        {
            if (destType == typeof(string) && value is MaterialCommandCollection)
            {                
                return "Material Command Collection";
            }
            return base.ConvertTo(context, culture, value, destType);
        }

    }

    internal class MaterialCommandConverter : ExpandableObjectConverter
    {

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
        {
            if (destType == typeof(string) && value is MatCmd)
            {
                //Casts value to MatCmd Type.
                MatCmd cmd = (MatCmd)value;

                return cmd.RawFloats;

            }
            return base.ConvertTo(context, culture, value, destType);
        }

    }

    internal class Vector4CommandCollectionConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
        {
            if (destType == typeof(string) && value is Vector4Collection)
            {
                return "Vector 4 Collection";
            }
            return base.ConvertTo(context, culture, value, destType);
        }

    }

}
