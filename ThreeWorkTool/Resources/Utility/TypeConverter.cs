using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using static ThreeWorkTool.Resources.Wrappers.MaterialMaterialEntry;
using ThreeWorkTool.Resources.Wrappers.ExtraNodes;
using YamlDotNet.Serialization;

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



    //Generic Deserialization Function.
    public static class SerializeAndDeserialize
    {
        public static string Serialize<T>(T obj)
        {
            var serializer = new SerializerBuilder().Build();
            return serializer.Serialize(obj);
        }
        public static MTMaterial Deserialize<MTMaterial>(string yaml)
        {
            var deserializer = new DeserializerBuilder().Build();
            return deserializer.Deserialize<MTMaterial>(yaml);
        }
    }

}
