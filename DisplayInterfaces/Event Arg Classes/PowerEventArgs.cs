using System;

namespace DisplayInterfaces
{
    public class PowerEventArgs : EventArgs
    {
        public bool State { get; internal set; }
        public ushort uState { get; internal set; }
    }
}
