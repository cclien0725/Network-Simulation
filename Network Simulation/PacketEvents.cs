using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Network_Simulation
{
    abstract class PacketEvents
    {
        int Id { get; set; }                // The id of packet event.
        long Time { get; set; }             // The event arise time.
        int SourceNodeId { get; set; }      // The id of the source node.
        int DestinationNodeId { get; set; } // The id of the destination node.

    }

    class PacketSendedEvent : PacketEvents
    {
        int SendNodeId { get; set; }
    }

    class PacketArriveEvent : PacketEvents
    {
        int ArrivalNodeId { get; set; }
    }

    class PacketTunnelingEvent : PacketEvents
    {
        int TunnelingSourceNodeId { get; set; }
        int TunnelingDestinationNodeId { get; set; }
    }

    class PacketMarkingEvent : PacketEvents 
    {
        int MarkingNodeId { get; set; }
    }

    class PacketFilteringEvent : PacketEvents 
    {
        int FilteringNodeId { get; set; }
    }
}
