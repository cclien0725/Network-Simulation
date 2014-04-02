using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network_Simulation;

namespace Heterogenerous_Simulation
{
    public class Simulator
    {
        internal class Config
        {
            internal const int ATTACK_PACKET_PER_SEC = 1;
            internal const int NORMAL_PACKET_PER_SEC = 10;
            internal const int NUMBER_OF_ATTACK_PACKET = 300;
            internal const int NUMBER_OF_NORMAL_PACKET = 30;
            internal const double PROBIBILITY_OF_PACKET_TUNNELING = 0.5;
            internal const double PROBIBILITY_OF_PACKET_MARKING = 0.5;
        }

        private SQLiteUtility sql;
        private NetworkTopology topology;
        private Deployment deployment;

        private List<NetworkTopology.Node> activeNode;

        public Simulator(string dbName, Deployment deployment, NetworkTopology topology)
        {
            if (topology.Nodes.Count == 0)
                throw new Exception("Run() Fail: There are 0 nodes in the network.");

            if (sql == null)
                sql = new SQLiteUtility(dbName);

            sql.CreateTable(deployment.GetType().Name);
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
        }

        public void Run()
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
                        totalPacket = Config.NUMBER_OF_NORMAL_PACKET;
                        packetPerSec = Config.NORMAL_PACKET_PER_SEC;
                        break;
                    case NetworkTopology.NodeType.Attacker:
                        totalPacket = Config.NUMBER_OF_ATTACK_PACKET;
                        packetPerSec = Config.ATTACK_PACKET_PER_SEC;
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
                                if (!isTunneling && !isMarking && rd.NextDouble() <= Config.PROBIBILITY_OF_PACKET_TUNNELING)
                                {
                                    //re-computing path...
                                    if (i < totalPacket / 2)
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
                                if ((!isMarking && rd.NextDouble() <= Config.PROBIBILITY_OF_PACKET_MARKING && i < totalPacket / 2 && !shouldFiltering) || shouldMarking)
                                {
                                    MarkingEvent markingEvent = new MarkingEvent(packetEvent);
                                    markingEvent.MarkingNodeID = path[j];
                                    sql.InsertMarkingEvent(markingEvent);
                                    isMarking = true;
                                    shouldMarking = false;
                                }
                                break;
                            case NetworkTopology.TracerType.Filtering:
                                if (shouldFiltering || i >= totalPacket / 2)
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

                tmpPath = topology.GetShortestPath(path[source], topology.NodeIndex2ID(tunnelingNodeID));
                tmpPath.Remove(tmpPath.Last());
                tmpPath.AddRange(topology.GetShortestPath(topology.NodeIndex2ID(tunnelingNodeID), path.Last()));

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
    }
}
