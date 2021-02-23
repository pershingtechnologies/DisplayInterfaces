using System;
using NetworkUtilities;

namespace DisplayInterfaces
{
    public class PlanarUltraRes : IDisplay
    {
        /******interface implementation******/
        //power commands
        private bool _power;
        public bool Power
        {
            get { return _power; }
            set { SetPower(value); }
        }

        //volume commands
        private ushort _volume;
        public ushort Volume
        {
            get { return _volume; }
            set
            {
                string ValueToSend = FormatVolume(value).ToString();
                tcpClient.SendString($"Audio.Volume={ValueToSend}\r");
            }
        }

        //input commands
        private eDisplayInput _input;
        public eDisplayInput Input
        {
            get { return _input; }
            set { SetInput(value); }
        }

        //quadview commands
        public bool SupportsQuad { get; private set; }

        private bool _quadDisplay;
        public bool QuadDisplay
        {
            get { return _quadDisplay; }
            set { SetQuadview(value); }
        }

        //events get triggered by true feedback parsed from device responses
        public event EventHandler<PowerEventArgs> PowerStateChanged;
        public event EventHandler<VolumeEventArgs> VolumeChanged;
        public event EventHandler<InputEventArgs> InputChanged;
        public event EventHandler<QuadViewEventArgs> QuadViewChanged;
        public event EventHandler Connected;
        public event EventHandler Disconnected;

        //tcpClient properties/methods
        private string _ipAddress;
        public string IpAddress
        { 
            get { return _ipAddress; } 
            set
            {
                _ipAddress = value;
                tcpClient.IpAddress = _ipAddress;
            }
        }
        public void Connect()
        {
            tcpClient.Connect();
        }
        
        private void TcpClient_DataReceived(object sender, DataReceivedArgs e)
        {
            /*
             * Parse the data sent by the display, check for changes
             * in properties that the IDisplay interfaces uses, and
             * raise events accordingly
             */

            //example response: "MULTI.VIEW:QUAD"
            string[] ResponseValues = e.DataString.ToUpper().Split(':');
            string PropThatChanged = ResponseValues[0];
            string NewValue = ResponseValues[1];

            if(PropThatChanged.IndexOf("DISPLAY.POWER") >= 0)
            {
                _power = (NewValue.IndexOf("ON") >= 0);
                PowerStateChanged?.Invoke(this, new PowerEventArgs
                {
                    State = _power,
                    uState = _power ? (ushort)1 : (ushort)0
                });
            }
            else if(PropThatChanged.IndexOf("AUDIO.VOLUME") >= 0)
            {
                int sentValue = int.Parse(NewValue); //convert string value to int
                double scaledPercentage = (sentValue / 100) * ushort.MaxValue; //scale int to proportion of ushort value (65535)
                _volume = (ushort)Math.Floor(scaledPercentage); //convert to ushort
                VolumeChanged?.Invoke(this, new VolumeEventArgs()
                {
                    CurrentLevel = _volume
                });
            }
            else if(PropThatChanged.IndexOf("MULTI.VIEW") >= 0)
            {
                _quadDisplay = (NewValue.IndexOf("QUAD") >= 0);
                QuadViewChanged?.Invoke(this, new QuadViewEventArgs()
                {
                    State = _quadDisplay,
                    uState = _quadDisplay ? (ushort)1 : (ushort)0
                });
            }
            else if(PropThatChanged.IndexOf("SOURCE.SELECT(ZONE.1)") >= 0)
            {
                switch(NewValue)
                {
                    case ("OPS"):       { _input = eDisplayInput.OpsSlot; break; }
                    case ("HDMI.1"):    { _input = eDisplayInput.Hdmi1; break; }
                    case ("HDMI.2"):    { _input = eDisplayInput.Hdmi2; break; }
                    case ("HDMI.3"):    { _input = eDisplayInput.Hdmi3; break; }
                    case ("HDMI.4"):    { _input = eDisplayInput.Hdmi4; break; }
                    case ("DP"):        { _input = eDisplayInput.DisplayPort1; break; }
                }

                InputChanged?.Invoke(this, new InputEventArgs()
                {
                    CurrentInput = _input,
                    uCurrentInput = (ushort)_input
                });
            }
        }

        //constructor
        public PlanarUltraRes(string ipAddress)
        {
            //this type of display does support quad-view
            SupportsQuad = true;

            //initialize tcpClient
            //Planar displays use port 57 for control
            //register tcpClient events
            tcpClient = new PtTcpClient(ipAddress, 57);
            tcpClient.DataReceived += TcpClient_DataReceived;
            tcpClient.Connected += TcpClient_Connected;
            tcpClient.Disconnected += TcpClient_Disconnected;
            IpAddress = ipAddress;
        }

        //pass events from TCP client up to consumer class
        private void TcpClient_Disconnected(object sender, EventArgs e)
        {
            Disconnected?.Invoke(this, e);
        }
        private void TcpClient_Connected(object sender, EventArgs e)
        {
            Connected?.Invoke(this, e);
        }

        //fields & methods not included in interface
        private PtTcpClient tcpClient;
        private void SetPower(bool state)
        {
            string Command = state ? "Display.Power=ON\r" : "Display.Power=OFF\r";
            tcpClient.SendString(Command);
        }

        //scales full-range ushort (0-65535) to range required by display (0-100)
        private ushort FormatVolume(ushort volumeSet)
        {
            double percentage = (volumeSet / ushort.MaxValue) * 100;
            ushort uPercentage = (ushort)Math.Floor(percentage);
            return uPercentage;
        }
        
        private void SetInput(eDisplayInput input)
        {
            string sInput;
            switch (input)
            {
                case eDisplayInput.Default: { sInput = "HDMI.1"; break; }
                case eDisplayInput.Hdmi1:
                case eDisplayInput.Hdmi2:
                case eDisplayInput.Hdmi3:
                case eDisplayInput.Hdmi4:
                    { sInput = $"HDMI.{(ushort)input}"; break; }
                case eDisplayInput.DisplayPort1: { sInput = "DP"; break; }
                case eDisplayInput.DisplayPort2: { throw new NotSupportedException("Planar display only has one DP input"); }
                case eDisplayInput.OpsSlot: { sInput = "OPS"; break; }
                default: { sInput = "HDMI.1"; break; }
            }

            string Command = $"Source.Select(Zone.1)={sInput}\r";
            tcpClient.SendString(Command);
        }
        private void SetQuadview(bool state)
        {
            if (SupportsQuad)
            {
                string Command = state ? "Multi.View=Quad\r" : "Multi.View=Single\r";
                tcpClient.SendString(Command);
            }
            else { throw new NotSupportedException("Display does not support quadview"); }
        }
    }
}
