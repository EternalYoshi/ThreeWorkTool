using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeWorkTool.Resources.Wrappers.ExtraNodes
{
    public class MTMaterial
    {
        public string version { get; set; }

        public List<Material> Materials { get; set; }

        public struct Material
        {
            public string name { get; set; }
            public string type { get; set; }
            public string blendState { get; set; }
            public string depthStencilState { get; set; }
            public string resterizerState { get; set; }
            public string cmdListFlags { get; set; }
            public string matFlags { get; set; }
            public List<MatCommands> cmds { get; set; }
        }

        public struct MatCommands
        {
            //readonly List<string> TYPES = {"flag", "cbuffer", "samplerstate", "texture"};
            public string type { get; set; }
            public string name { get; set; }
            public object data { get; set; }

        }

    }
}
