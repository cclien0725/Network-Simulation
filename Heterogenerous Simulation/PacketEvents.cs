using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network_Simulation;

namespace Heterogenerous_Simulation
{
    public class PacketEvent
    {
        public int PacketID { get; set; }
        public double Time { get; set; }
        public int Source { get; set; }
        public int Destination { get; set; }
        public NetworkTopology.NodeType Type { get; set; }
    }

    public class PacketSentEvent : PacketEvent
    {
        public int CurrentNodeID { get; set; }
        public int NextHopID { get; set; }
        public double Length { get; set; }

        public PacketSentEvent(int packetID)
        {
            this.PacketID = packetID;
        }

        public PacketSentEvent(PacketEvent packetEvent)
        {
            this.PacketID = packetEvent.PacketID;
            this.Time = packetEvent.Time;
            this.Source = packetEvent.Source;
            this.Destination = packetEvent.Destination;
            this.Type = packetEvent.Type;
        }
    }

    public class TunnelingEvent : PacketEvent
    {
        public int TunnelingSrc { get; set; }
        public int TunnelingDst { get; set; }

        public TunnelingEvent(PacketEvent packetEvent)
        {
            this.PacketID = packetEvent.PacketID;
            this.Time = packetEvent.Time;
            this.Source = packetEvent.Source;
            this.Destination = packetEvent.Destination;
            this.Type = packetEvent.Type;
        }
    }

    public class MarkingEvent : PacketEvent
    {
        public int MarkingNodeID { get; set; }

        public MarkingEvent(PacketEvent packetEvent)
        {
            this.PacketID = packetEvent.PacketID;
            this.Time = packetEvent.Time;
            this.Source = packetEvent.Source;
            this.Destination = packetEvent.Destination;
            this.Type = packetEvent.Type;
        }
    }

    public class FilteringEvent : PacketEvent
    {
        public int FilteringNodeID { get; set; }

        public FilteringEvent(PacketEvent packetEvent)
        {
            this.PacketID = packetEvent.PacketID;
            this.Time = packetEvent.Time;
            this.Source = packetEvent.Source;
            this.Destination = packetEvent.Destination;
            this.Type = packetEvent.Type;
        }
    }
}
