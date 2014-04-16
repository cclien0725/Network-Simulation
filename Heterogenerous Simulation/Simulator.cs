using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network_Simulation;

namespace Heterogenerous_Simulation
{
    public class Simulator
    {
        public class ReportArgument : EventArgs
        {
            public int CurrentPacket { get; set; }
            public int TotalPacket { get; set; }
        }

        private SQLiteUtility sql;
        private NetworkTopology topology;
        private Deployment deployment;

        private List<NetworkTopology.Node> attackNode;

        public event EventHandler<ReportArgument> onReportOccur;

        public Simulator(Deployment deployment, NetworkTopology topology, SQLiteUtility sql)
        {
            if (topology.Nodes.Count == 0)
                throw new Exception("Run() Fail: There are 0 nodes in the network.");

            this.sql = sql;
            this.topology = topology;
            this.deployment = deployment;

            attackNode = topology.Nodes.Where(node => node.Type == NetworkTopology.NodeType.Attacker).ToList();

            sql.LogDeploymentResult(topology);
        }

        public void Run(int attackPacketPerSec, int normalPacketPerSec, int totalPacket, int percentageOfAttackPacket, double probibilityOfPacketTunneling, double probibilityOfPacketMarking, double startFiltering, int initialTimeOfAttackPacket, bool dynamicProbability)
        {
            Random rd = new Random();
            int victimID;
            List<int> path;

            List<PacketEvent> packetEventList = new List<PacketEvent>();
            List<PacketSentEvent> packetSentEventList = new List<PacketSentEvent>();
            List<TunnelingEvent> tunnelingEventList = new List<TunnelingEvent>();
            List<MarkingEvent> markingEventList = new List<MarkingEvent>();
            List<FilteringEvent> filteringEventList = new List<FilteringEvent>();

            int totalAttackPacket = percentageOfAttackPacket * totalPacket / 100;
            int totalNormalPacket = totalPacket - totalAttackPacket;

            foreach (NetworkTopology.Node node in topology.Nodes)
            {
                if (node.Tracer == NetworkTopology.TracerType.Tunneling)
                    node.ProbabilityOfTunneling = probibilityOfPacketTunneling;
            }

            for (int i = 0; i < totalPacket; i++)
            {
                int tunnelingNodeID = -1;

                bool isTunneling = false;
                bool isMarking = false;
                bool isFiltering = false;

                bool shouldMarking = false;
                bool shouldFiltering = false;

                NetworkTopology.Node SourceNode;
                victimID = topology.idOfVictims[rd.Next(topology.idOfVictims.Count)];

                do
                {
                    if (i < totalAttackPacket)
                    {
                        SourceNode = attackNode[i % attackNode.Count];
                    }
                    else
                    {
                        SourceNode = topology.Nodes[rd.Next(topology.Nodes.Count)];
                    }
                } while (SourceNode.ID == victimID);

                
                path = topology.GetShortestPath(SourceNode.ID, victimID);

                PacketEvent packetEvent = new PacketEvent()
                {
                    PacketID = i,
                    Source = SourceNode.ID,
                    Destination = victimID,
                    Time = SourceNode.Type == NetworkTopology.NodeType.Attacker ? initialTimeOfAttackPacket : 0,
                    Type = SourceNode.Type
                };

                //sql.InsertPacketEvent(packetEvent);
                packetEventList.Add(packetEvent);

                for (int j = 0; j < path.Count; j++)
                {
                    packetEvent.Time = j == 0 ? packetEvent.Time : packetEvent.Time += topology.AdjacentMatrix[topology.NodeID2Index(path[j - 1]), topology.NodeID2Index(path[j])].Delay;

                    switch (topology.Nodes[topology.NodeID2Index(path[j])].Tracer)
                    {
                        case NetworkTopology.TracerType.Tunneling:
                            if (!isTunneling && !isMarking && rd.NextDouble() <= topology.Nodes.Find(node => node.ID == path[j]).ProbabilityOfTunneling/*probibilityOfPacketTunneling*/)
                            {
                                TunnelingEvent tunnelingEvent = new TunnelingEvent(packetEvent);
                                tunnelingEvent.TunnelingSrc = path[j];

                                // Re-computing path.
                                if (i < startFiltering * totalPacket / 100)
                                {
                                    path = ChooseTunnelingNode(path, j, NetworkTopology.TracerType.Marking, ref tunnelingEvent);
                                    shouldMarking = true;
                                }
                                else
                                {
                                    path = ChooseTunnelingNode(path, j, NetworkTopology.TracerType.Filtering, ref tunnelingEvent);
                                    shouldFiltering = true;
                                    tunnelingNodeID = tunnelingEvent.TunnelingSrc;
                                }

                                //sql.InsertTunnelingEvent(tunnelingEvent);
                                tunnelingEventList.Add(tunnelingEvent);
                                isTunneling = true;
                            }
                            break;
                        case NetworkTopology.TracerType.Marking:
                            if ((!isMarking && rd.NextDouble() <= probibilityOfPacketMarking && i < startFiltering * totalPacket / 100 && !shouldFiltering) || shouldMarking)
                            {
                                MarkingEvent markingEvent = new MarkingEvent(packetEvent);
                                markingEvent.MarkingNodeID = path[j];
                                //sql.InsertMarkingEvent(markingEvent);
                                markingEventList.Add(markingEvent);
                                isMarking = true;
                                shouldMarking = false;
                            }
                            break;
                        case NetworkTopology.TracerType.Filtering:
                            if (shouldFiltering || i >= startFiltering * totalPacket / 100)
                            {
                                if (packetEvent.Type == NetworkTopology.NodeType.Attacker)
                                {
                                    FilteringEvent filteringEvent = new FilteringEvent(packetEvent);
                                    filteringEvent.FilteringNodeID = path[j];
                                    //sql.InsertFilteringEvent(filteringEvent);
                                    filteringEventList.Add(filteringEvent);
                                    isFiltering = true;

                                    // TODO: adjust probability of Tunneling tracer which tunneling from...
                                    if (dynamicProbability && tunnelingNodeID != -1)
                                    {
                                        topology.Nodes.Find(node => node.ID == path[j]).FilteringCount = topology.Nodes.Find(node => node.ID == path[j]).FilteringCount >= 5 ? 5 : topology.Nodes.Find(node => node.ID == path[j]).FilteringCount + 1;
                                        topology.Nodes.Find(node => node.ID == tunnelingNodeID).ProbabilityOfTunneling = 0.2 * topology.Nodes.Find(node => node.ID == path[j]).FilteringCount / 5 + 0.8 * topology.Nodes.Find(node => node.ID == tunnelingNodeID).ProbabilityOfTunneling;
                                    }
                                }
                                else if (dynamicProbability && tunnelingNodeID != -1)
                                {
                                    topology.Nodes.Find(node => node.ID == path[j]).FilteringCount = topology.Nodes.Find(node => node.ID == path[j]).FilteringCount <= 0 ? 0 : topology.Nodes.Find(node => node.ID == path[j]).FilteringCount - 1;
                                    topology.Nodes.Find(node => node.ID == tunnelingNodeID).ProbabilityOfTunneling = 0.2 * topology.Nodes.Find(node => node.ID == path[j]).FilteringCount / 5 + 0.8 * topology.Nodes.Find(node => node.ID == tunnelingNodeID).ProbabilityOfTunneling;
                                }
                                //shouldFiltering = true;
                            }
                            break;
                    }

                    if (isFiltering)
                        break;

                    PacketSentEvent packetSentEvent = new PacketSentEvent(packetEvent);
                    packetSentEvent.CurrentNodeID = path[j];
                    packetSentEvent.NextHopID = j == path.Count - 1 ? -1 : path[j + 1];
                    packetSentEvent.Length = j == path.Count - 1 ? 0 : topology.AdjacentMatrix[topology.NodeID2Index(path[j]), topology.NodeID2Index(path[j + 1])].Length;

                    //sql.InsertPacketSentEvent(packetSentEvent);
                    packetSentEventList.Add(packetSentEvent);
                }
                Report(i + 1, totalPacket);
            }

            sql.InsertPacketEvent(packetEventList);
            sql.InsertPacketSentEvent(packetSentEventList);
            sql.InsertTunnelingEvent(tunnelingEventList);
            sql.InsertMarkingEvent(markingEventList);
            sql.InsertFilteringEvent(filteringEventList);
        }

        private List<int> ChooseTunnelingNode(List<int> path, int source, NetworkTopology.TracerType tracerType, ref TunnelingEvent tunelingEvent)
        {
            double totalDelay = double.MaxValue;
            List<int> newPath = new List<int>();
            List<int> tunnelingTargets = null;

            switch (tracerType)
            {
                case NetworkTopology.TracerType.Marking:
                    tunnelingTargets = deployment.MarkingTracerID;
                    break;
                case NetworkTopology.TracerType.Filtering:
                    tunnelingTargets = deployment.FilteringTracerID;
                    break;
            }

            foreach (var tunnelingNodeID in tunnelingTargets)
            {
                List<int> tmpPath = new List<int>();
                double tmpTotalDelay = 0;

                if (!path.GetRange(source, path.Count - source).Contains(tunnelingNodeID))
                {
                    tmpPath = topology.GetShortestPath(path[source], tunnelingNodeID);
                    tmpPath.Remove(tmpPath.Last());
                    tmpPath.AddRange(topology.GetShortestPath(tunnelingNodeID, path.Last()));

                    for (int i = source - 1; i >= 0; i--)
                    {
                        tmpPath.Insert(0, path[i]);
                    }
                }
                else 
                {
                    tmpPath = path;
                }

                for (int i = 0; i < tmpPath.Count - 1; i++)
                {
                    tmpTotalDelay += topology.AdjacentMatrix[topology.NodeID2Index(tmpPath[i]), topology.NodeID2Index(tmpPath[i + 1])].Delay;
                }

                if (tmpTotalDelay < totalDelay)
                {
                    newPath = tmpPath;
                    totalDelay = tmpTotalDelay;
                    tunelingEvent.TunnelingDst = tunnelingNodeID;
                }
            }

            return newPath;
        }

        private void Report(int currentNode, int totalActiveNode)
        {
            if (onReportOccur != null)
                onReportOccur.Invoke(this, new ReportArgument() { CurrentPacket = currentNode, TotalPacket = totalActiveNode });
        }
    }
}
