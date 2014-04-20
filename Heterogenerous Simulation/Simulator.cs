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

            sql.LogDeploymentResult(topology, deployment);
        }

        public void Run(int attackPacketPerSec, int normalPacketPerSec, int totalPacket, int percentageOfAttackPacket, double probibilityOfPacketTunneling, double probibilityOfPacketMarking, double startFiltering, int initialTimeOfAttackPacket, bool dynamicProbability)
        {
            Random rd = new Random();
            int victimID;
            int attackerIndex = 0;
            List<int> path;

            List<PacketEvent> packetEventList = new List<PacketEvent>();
            List<PacketSentEvent> packetSentEventList = new List<PacketSentEvent>();
            List<TunnelingEvent> tunnelingEventList = new List<TunnelingEvent>();
            List<MarkingEvent> markingEventList = new List<MarkingEvent>();
            List<FilteringEvent> filteringEventList = new List<FilteringEvent>();

            foreach (NetworkTopology.Node node in topology.Nodes)
            {
                if (node.Tracer == NetworkTopology.TracerType.Tunneling)
                {
                    node.ProbabilityOfTunneling = probibilityOfPacketTunneling;
                    node.FilteringCount = probibilityOfPacketTunneling * 10;
                }
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
                    if (rd.NextDouble() < (double)percentageOfAttackPacket / 100)
                    {
                        SourceNode = attackNode[attackerIndex % attackNode.Count];
                        attackerIndex++;
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

                                tunnelingEventList.Add(tunnelingEvent);
                                isTunneling = true;
                            }
                            break;
                        case NetworkTopology.TracerType.Marking:
                            if ((!isMarking && rd.NextDouble() <= probibilityOfPacketMarking && i < startFiltering * totalPacket / 100 && !shouldFiltering) || shouldMarking)
                            {
                                MarkingEvent markingEvent = new MarkingEvent(packetEvent);
                                markingEvent.MarkingNodeID = path[j];
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
                                    filteringEventList.Add(filteringEvent);
                                    isFiltering = true;

                                    // TODO: adjust probability of Tunneling tracer which tunneling from...
                                    if (dynamicProbability && tunnelingNodeID != -1)
                                    {
                                        Network_Simulation.NetworkTopology.Node node = topology.Nodes.Find(n => n.ID == tunnelingNodeID);
                                        node.FilteringCount = node.FilteringCount >= 10 ? 10 : node.FilteringCount + 1;
                                        node.ProbabilityOfTunneling = node.FilteringCount / 10;
                                    }
                                }
                                else if (dynamicProbability && tunnelingNodeID != -1)
                                {
                                    Network_Simulation.NetworkTopology.Node node = topology.Nodes.Find(n => n.ID == tunnelingNodeID);
                                    node.FilteringCount = node.FilteringCount <= 0 ? 0 : node.FilteringCount - 1;
                                    node.ProbabilityOfTunneling = node.FilteringCount / 10;
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

                    packetSentEventList.Add(packetSentEvent);
                }
                Report(i + 1, totalPacket);

                //DataUtility.Log("==============================");
                //foreach (NetworkTopology.Node node in topology.Nodes)
                //    DataUtility.Log(string.Format("Node%{0} FC:{1} Tp:{2}\n", node.ID, node.FilteringCount, node.ProbabilityOfTunneling));
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
