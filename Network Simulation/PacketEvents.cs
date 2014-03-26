using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Network_Simulation
{
    public class PacketEvent
    {
        public int PacketID { get; set; }
        public double Time { get; set; }
        public int Source { get; set; }
        public int Destination { get; set; }
    }

    public class PacketSentEvent : PacketEvent
    {
        public int CurrentNodeID { get; set; }
        public int NextHopID { get; set; }
        public double Length { get; set; }
    }

    public class TunnelingEvent : PacketEvent
    {
        public int TunnelingSrc { get; set; }
        public int TunnelingDst { get; set; }
    }

    public class MarkingEvent : PacketEvent
    {
        public int MarkingNodeID { get; set; }
    }

    public class FilteringEvent : PacketEvent
    {
        public int FilteringNodeID { get; set; }
    }
}
