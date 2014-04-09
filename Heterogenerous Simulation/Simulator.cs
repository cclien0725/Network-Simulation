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
            public int CurrentNode { get; set; }
            public int TotalActiveNode { get; set; }
        }

        private SQLiteUtility sql;
        private NetworkTopology topology;
        private Deployment deployment;

        private List<NetworkTopology.Node> activeNode;

        public event EventHandler<ReportArgument> onReportOccur;

        public Simulator(Deployment deployment, NetworkTopology topology, SQLiteUtility sql)
        {
            if (topology.Nodes.Count == 0)
                throw new Exception("Run() Fail: There are 0 nodes in the network.");

            //if (sql == null)
            //    sql = new SQLiteUtility(dbName);

            //sql.CreateTable(deployment.GetType().Name);
            this.sql = sql;

            this.topology = topology;
            this.deployment = deployment;

            activeNode = new List<NetworkTopology.Node>();

            var attackNodes = from node in topology.Nodes
                              where node.Type == NetworkTopology.NodeType.Attacker
                              select node;
            var normalUser = from node in topology.Nodes
                             where node.Type == NetworkTopology.NodeType.Normal
                             select node;

            activeNode.AddRange(attackNodes.ToList());
            activeNode.AddRange(normalUser.ToList());

            sql.LogDeploymentResult(topology);
        }

        public void Run(int attackPacketPerSec, int normalPacketPerSec, int numberOfAttackPacket, int numberOfNormalPacket, double probibilityOfPacketTunneling, double probibilityOfPacketMarking, double StartFiltering)
        {
            Random rd = new Random();
            int victim;
            int packetCount = 0;
            List<int> path;

            foreach (var node in activeNode)
            {
                int time = 0;
                int totalPacket = 0;
                int packetPerSec = 0;

                switch (node.Type)
                {
                    case NetworkTopology.NodeType.Normal:
                        totalPacket = numberOfNormalPacket;
                        packetPerSec = normalPacketPerSec;
                        break;
                    case NetworkTopology.NodeType.Attacker:
                        totalPacket = numberOfAttackPacket;
                        packetPerSec = attackPacketPerSec;
                        break;
                }

                victim = topology.idOfVictims[rd.Next(topology.idOfVictims.Count)];

                for (int i = 0; i < totalPacket; i++)
                {
                    bool isTunneling = false;
                    bool isMarking = false;
                    bool isFiltering = false;

                    bool shouldMarking = false;
                    bool shouldFiltering = false;

                    path = topology.GetShortestPath(node.ID, topology.Nodes[victim].ID);

                    PacketEvent packetEvent = new PacketEvent()
                    {
                        PacketID = packetCount++,
                        Source = node.ID,
                        Destination = topology.Nodes[victim].ID,
                        Time = i == 0 ? time : time += Convert.ToInt32(topology.Poisson(packetPerSec)),
                        Type = node.Type
                    };
                    sql.InsertPacketEvent(packetEvent);

                    for (int j = 0; j < path.Count - 1; j++)
                    {
                        packetEvent.Time = j == 0 ? packetEvent.Time : packetEvent.Time += topology.AdjacentMatrix[topology.NodeID2Index(path[j - 1]), topology.NodeID2Index(path[j])].Delay;

                        switch (topology.Nodes[topology.NodeID2Index(path[j])].Tracer)
                        {
                            case NetworkTopology.TracerType.Tunneling:
                                if (!isTunneling && !isMarking && rd.NextDouble() <= probibilityOfPacketTunneling)
                                {
                                    //re-computing path...
                                    if (i < StartFiltering * totalPacket / 100)
                                    {
                                        path = ChooseTunnelingNode(path, j, NetworkTopology.TracerType.Marking);
                                        shouldMarking = true;
                                    }
                                    else
                                    {
                                        path = ChooseTunnelingNode(path, j, NetworkTopology.TracerType.Filtering);
                                        shouldFiltering = true;
                                    }

                                    TunnelingEvent tunnelingEvent = new TunnelingEvent(packetEvent);
                                    tunnelingEvent.TunnelingSrc = path[j];
                                    tunnelingEvent.TunnelingDst = path[j + 1];
                                    sql.InsertTunnelingEvent(tunnelingEvent);
                                    isTunneling = true;
                                }
                                break;
                            case NetworkTopology.TracerType.Marking:
                                if ((!isMarking && rd.NextDouble() <= probibilityOfPacketMarking && i < StartFiltering * totalPacket / 100 && !shouldFiltering) || shouldMarking)
                                {
                                    MarkingEvent markingEvent = new MarkingEvent(packetEvent);
                                    markingEvent.MarkingNodeID = path[j];
                                    sql.InsertMarkingEvent(markingEvent);
                                    isMarking = true;
                                    shouldMarking = false;
                                }
                                break;
                            case NetworkTopology.TracerType.Filtering:
                                if (shouldFiltering || i >= StartFiltering * totalPacket / 100)
                                {
                                    if (packetEvent.Type == NetworkTopology.NodeType.Attacker)
                                    {
                                        FilteringEvent filteringEvent = new FilteringEvent(packetEvent);
                                        filteringEvent.FilteringNodeID = path[j];
                                        sql.InsertFilteringEvent(filteringEvent);
                                        isFiltering = true;
                                    }
                                    shouldFiltering = true;
                                }
                                break;
                        }

                        if (isFiltering)
                            break;

                        PacketSentEvent packetSentEvent = new PacketSentEvent(packetEvent);
                        packetSentEvent.CurrentNodeID = path[j];
                        packetSentEvent.NextHopID = path[j + 1];
                        packetSentEvent.Length = topology.AdjacentMatrix[topology.NodeID2Index(path[j]), topology.NodeID2Index(path[j + 1])].Length;

                        sql.InsertPacketSentEvent(packetSentEvent);
                    }
                }
                Report(activeNode.IndexOf(node) + 1, activeNode.Count);
            }
        }

        private List<int> ChooseTunnelingNode(List<int> path, int source, NetworkTopology.TracerType tracerType)
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

                if (!path.Contains(topology.NodeIndex2ID(tunnelingNodeID)))
                {
                    tmpPath = topology.GetShortestPath(path[source], topology.NodeIndex2ID(tunnelingNodeID));
                    tmpPath.Remove(tmpPath.Last());
                    tmpPath.AddRange(topology.GetShortestPath(topology.NodeIndex2ID(tunnelingNodeID), path.Last()));
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
                }
            }

            for (int i = source - 1; i >= 0; i--)
            {
                newPath.Insert(0, path[i]);
            }

            return newPath;
        }

        private void Report(int currentNode, int totalActiveNode)
        {
            if (onReportOccur != null)
                onReportOccur.Invoke(this, new ReportArgument() { CurrentNode = currentNode, TotalActiveNode = totalActiveNode });
        }
    }
}
