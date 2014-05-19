using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network_Simulation;

namespace Deployment_Simulation
{
    public class Simulator
    {
        public class PacketEvent
        {
            public int PacketID { get; set; }
            public double Time { get; set; }
            public int Source { get; set; }
            public int Destination { get; set; }
            public NetworkTopology.NodeType Type { get; set; }
        }

        public class MarkingEvent : PacketEvent
        {
            public int MarkingNodeID { get; set; }
            public int HopCount { get; set; }

            public MarkingEvent(PacketEvent packetEvent)
            {
                this.PacketID = packetEvent.PacketID;
                this.Time = packetEvent.Time;
                this.Source = packetEvent.Source;
                this.Destination = packetEvent.Destination;
                this.Type = packetEvent.Type;
            }
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

        private Deployment m_deployment;
        private NetworkTopology m_networkTopology;

        public Simulator(Deployment deployment, NetworkTopology networkTopology)
        {
            this.m_deployment = deployment;
            this.m_networkTopology = networkTopology;

            foreach (int nodeID in m_deployment.DeployNodes)
                m_networkTopology.Nodes.Find(n => n.ID == nodeID).Tracer = NetworkTopology.TracerType.Marking;
        }

        public void Run()
        {
            int packetNumber = 1000;

            List<int> path;

            List<MarkingEvent> markingEventList = new List<MarkingEvent>();
            List<PacketEvent> packetEventList = new List<PacketEvent>();
            List<PacketSentEvent> packetSentEventList = new List<PacketSentEvent>();

            Random rd = new Random();

            for (int i = 0; i < packetNumber; i++)
            {
                bool isMarking = false;
                NetworkTopology.Node source = m_networkTopology.Nodes.Find(node => node.ID == rd.Next(m_networkTopology.Nodes.Count));
                NetworkTopology.Node destination = m_networkTopology.Nodes.Find(node => node.ID == rd.Next(m_networkTopology.Nodes.Count) && node.ID != source.ID);

                path = m_networkTopology.GetShortestPath(source.ID, destination.ID);

                PacketEvent packetEvent = new PacketEvent()
                {
                    PacketID = i,
                    Source = source.ID,
                    Destination = destination.ID,
                    Time = 0,
                    Type = NetworkTopology.NodeType.Attacker
                };

                packetEventList.Add(packetEvent);

                for (int j = 0; j < path.Count; j++)
                {
                    switch (m_networkTopology.Nodes[m_networkTopology.NodeID2Index(path[j])].Tracer)
                    {
                        case NetworkTopology.TracerType.None:
                            break;
                        case NetworkTopology.TracerType.Marking:
                            if (!isMarking)
                            {
                                MarkingEvent markingEvent = new MarkingEvent(packetEvent);
                                markingEvent.MarkingNodeID = path[j];
                                markingEvent.HopCount = m_networkTopology.GetShortestPathCount(source.ID, path[j]) - 1;
                                isMarking = true;

                                markingEventList.Add(markingEvent);
                            }
                            break;
                        case NetworkTopology.TracerType.Tunneling:
                            break;
                        case NetworkTopology.TracerType.Filtering:
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
