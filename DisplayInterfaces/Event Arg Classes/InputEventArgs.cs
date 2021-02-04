using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayInterfaces
{
    public class InputEventArgs : EventArgs
    {
        public eDisplayInput CurrentInput { get; internal set; }
        public ushort uCurrentInput { get; internal set; }
    }
}
