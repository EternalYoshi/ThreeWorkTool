using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeWorkTool.Resources.Wrappers.ExtraNodes;

namespace ThreeWorkTool.Resources.Wrappers.ExtraNodes
{
    public class MTMaterial
    {
        public string version { get; set; }

        public List<Dictionary<string, Material>> materials { get; set; }

        public struct Material
        {
            public string name { get; set; }
            public string type { get; set; }
            public string blendState { get; set; }
            public string depthStencilState { get; set; }
            public string rasterizerState { get; set; }
            public string cmdListFlags { get; set; }
            public string matFlags { get; set; }
            public List<object> cmds { get; set; }
            public string animData { get; set; }
        }

        //public IDictionary<string,string> MatCommands { get; set; }

        //public struct MatCommands
        //{
        //    //readonly List<string> TYPES = {"flag", "cbuffer", "samplerstate", "texture"};
        //    public string type { get; set; }
        //    public string name { get; set; }
        //    public object data { get; set; }

        //}

    }
}
