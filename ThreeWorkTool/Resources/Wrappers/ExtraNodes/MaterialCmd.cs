using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeWorkTool.Resources.Wrappers.ExtraNodes
{
    public class MaterialCmd
    {
        //public static readonly List<string> TYPES = new List<string> { "flag", "cbuffer", "samplerstate", "texture" };
        
        public string Type { get; set; }
        public string Name { get; set; }
        public object Data { get; set; }

        //public MaterialCmd(string type = "", string name = "", object data = null)
        //{
        //    Type = type;
        //    Name = name;
        //    Data = data;
        //}

    }

    public class CbufferData
    {
        public List<float> Values { get; set; }
    }



}
