using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThreeWorkTool.Resources.Geometry
{
    public class KeyboardStateHandler
    {

        private readonly HashSet<Keys> _heldKeys = new HashSet<Keys>();

        public void KeyDown(Keys key) => _heldKeys.Add(key);
        public void KeyUp(Keys key) => _heldKeys.Remove(key);
        public HashSet<Keys> HeldKeys => _heldKeys;

    }
}
