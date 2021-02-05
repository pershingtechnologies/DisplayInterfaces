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
         * These properties are for back-compatibility with SIMPL+,
         * and are required because SIMPL+ does not support 
         * bool or enum types. Make sure these properties
         * return the equivalent value as the bool/enum
         * versions.
         */
        ushort uPower { get; set; }
        ushort uInput { get; set; }
        ushort uSupportsQuad { get; }
        ushort uQuadDisplay { get; set; }

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