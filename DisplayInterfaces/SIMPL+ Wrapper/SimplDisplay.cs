using System;

namespace DisplayInterfaces
{
    public class SimplDisplay
    {
        //private instance of the display interface
        private IDisplay Display;

        //initialization method to instantiate Display
        public void Initialize(ushort DisplayType, string Address)
        {
            try
            {
                eDisplayType displayType = (eDisplayType)DisplayType;

                switch (displayType)
                {
                    case eDisplayType.PlanarUltraRes: { Display = new PlanarUltraRes(Address); break; }
                    default: { throw new IndexOutOfRangeException("Display type index entered not supported"); }
                }

                Display.InputChanged += Display_InputChanged;
                Display.PowerStateChanged += Display_PowerStateChanged;
                Display.QuadViewChanged += Display_QuadViewChanged;
                Display.VolumeChanged += Display_VolumeChanged;
                Display.Connected += Display_Connected;
                Display.Disconnected += Display_Disconnected;
            }
            catch
            {
                throw new IndexOutOfRangeException("Display type index entered not supported");
            }
        }

        //event handlers pass events from IDisplay "Display" up to consumer (that is, SIMPL+)
        private void Display_VolumeChanged(object sender, VolumeEventArgs e)
        {
            VolumeChanged?.Invoke(sender, e);
        }
        private void Display_QuadViewChanged(object sender, QuadViewEventArgs e)
        {
            QuadViewChanged?.Invoke(sender, e);
        }
        private void Display_PowerStateChanged(object sender, PowerEventArgs e)
        {
            PowerStateChanged?.Invoke(sender, e);
        }
        private void Display_InputChanged(object sender, InputEventArgs e)
        {
            InputChanged?.Invoke(sender, e);
        }
        private void Display_Connected(object sender, EventArgs e)
        {
            Connected?.Invoke(sender, e);   
        }
        private void Display_Disconnected(object sender, EventArgs e)
        {
            Disconnected?.Invoke(sender, e);
        }

        //display control properties
        public ushort SupportsQuad
        { 
            get { return Display.SupportsQuad ? (ushort)1 : (ushort)0; } 
        }
        public ushort Power
        { 
            get { return Display.Power ? (ushort)1 : (ushort)0; } 
            set { Display.Power = (value > 0); } 
        }
        public ushort Volume
        {
            get { return Display.Volume; }
            set { Display.Volume = value; } 
        }
        public ushort Input
        {
            get { return (ushort)Display.Input; }
            set { Display.Input = (eDisplayInput)value; }
        }
        public ushort QuadDisplay
        {
            get { return Display.QuadDisplay ? (ushort)1 : (ushort)0; }
            set { Display.QuadDisplay = (value > 0); }
        }
        
        //tcp client logic
        public string IpAddress
        {
            get { return Display.IpAddress; }
            set { Display.IpAddress = value; }
        }
        public void Connect()
        {
            Display.Connect();
        }

        public event EventHandler<PowerEventArgs> PowerStateChanged;
        public event EventHandler<VolumeEventArgs> VolumeChanged;
        public event EventHandler<InputEventArgs> InputChanged;
        public event EventHandler<QuadViewEventArgs> QuadViewChanged;
        public event EventHandler Connected;
        public event EventHandler Disconnected;
    }
}
