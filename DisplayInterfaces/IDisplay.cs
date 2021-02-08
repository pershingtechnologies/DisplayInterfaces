using System;

namespace DisplayInterfaces
{
    public interface IDisplay
    {
        bool Power { get; set; }
        ushort Volume { get; set; }
        eDisplayInput Input { get; set; }
        
        bool SupportsQuad { get; }
        bool QuadDisplay { get; set; }

        /*
         * Events get triggered from "true feedback" from
         * the display. When a display sends a status update,
         * it should be parsed and the appropriate event should
         * be raised.
         */
        event EventHandler<PowerEventArgs> PowerStateChanged;
        event EventHandler<VolumeEventArgs> VolumeChanged;
        event EventHandler<InputEventArgs> InputChanged;
        event EventHandler<QuadViewEventArgs> QuadViewChanged;

        //TCP Client related objects
        string IpAddress { get; set; }
        void Connect();
    }
}