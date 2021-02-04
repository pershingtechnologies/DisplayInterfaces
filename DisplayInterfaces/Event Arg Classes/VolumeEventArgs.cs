using System;

namespace DisplayInterfaces
{
    public class VolumeEventArgs : EventArgs
    {
        public ushort CurrentLevel { get; internal set; }
    }
}
