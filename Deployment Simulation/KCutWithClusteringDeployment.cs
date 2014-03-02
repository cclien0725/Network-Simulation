using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network_Simulation;

namespace Deployment_Simulation
{
    class KCutWithClusteringDeployment : Deployment
    {
        public KCutWithClusteringDeployment(double percentageOfTunnelingTracer, double percentageOfMarkingTracer, double percentageOfFilteringTracer)
            : base(percentageOfTunnelingTracer, percentageOfMarkingTracer, percentageOfFilteringTracer)
        { }

        public override void Deploy(NetworkTopology networkTopology)
        {
            Console.WriteLine("KCut With Clustering Deployment.");

            List<NetworkTopology> allRoundScopeList = new List<NetworkTopology>();
            List<int> deployNodes = new List<int>();
            NetworkTopology tmp_src_net_topo = networkTopology;
            int centerID;

            // Finding the center node to run all level's process.
            while (findCenterNodeID(tmp_src_net_topo, out centerID))
            {
                NetworkTopology scope_net_topo = new NetworkTopology(0, 0);

                // Adding the center node to scope network topology.
                scope_net_topo.Nodes.Add(tmp_src_net_topo.Nodes[tmp_src_net_topo.NodeID2Index(centerID)]);

                // Starting run algorithm with this level.
                tmp_src_net_topo = startAlgorithm(tmp_src_net_topo, ref scope_net_topo, 3, ref deployNodes);

                // Adding this round generated scope network topology to list.
                allRoundScopeList.Add(scope_net_topo);
            }

            // Adding the remain nodes to deployment node list.
            deployNodes.AddRange(tmp_src_net_topo.Nodes.Select(x => x.ID));

            // Modifying actual tracer type on network topology depend on computed deployment node.
            foreach (int id in deployNodes)
                networkTopology.Nodes[networkTopology.NodeID2Index(id)].Tracer = NetworkTopology.TracerType.Marking;
        }

        /// <summary>
        /// Starting run algorithm with specific source topology and K-diameter cut, 
        /// and this round process will generate scope topology to scop_net_topo,
        /// and will add the deployment node to deployNodes structure.
        /// Finally, it will return the remain network topology with this process.
        /// </summary>
        /// <param name="src_net_topo">The source network topology</param>
        /// <param name="scope_net_topo">This round process that generte scope topology</param>
        /// <param name="K">The K-diameter cut value</param>
        /// <param name="deployNodes">The result of the deployment node id</param>
        /// <returns>The remain network topology after process the algorithm.</returns>
        private NetworkTopology startAlgorithm(NetworkTopology src_net_topo, ref NetworkTopology scope_net_topo, int K, ref List<int> deployNodes)
        {
            double max_hop_count = double.MinValue;

            while (max_hop_count < K - 1)
            {
                int minDegree = int.MaxValue;
                int selectNode = -1;

                // to finding the neighbor node with minimum degree
                foreach (var scopeNode in scope_net_topo.Nodes)
                {
                    foreach (int neighbor in src_net_topo.GetNeighborNodeIDs(scopeNode.ID))
                    {
                        if (minDegree > src_net_topo.Degree(neighbor) && !scope_net_topo.Nodes.Exists(x => x.ID == neighbor))
                        {
                            minDegree = src_net_topo.Degree(neighbor);
                            selectNode = neighbor;
                        }
                    }
                }

                // if nothing found, break the loop.
                if (selectNode == -1)
                    break;
                // adding the node to the scope set, and computing the max hop count.
                else
                {
                    scope_net_topo.Nodes.Add(src_net_topo.Nodes[src_net_topo.NodeID2Index(selectNode)]);

                    foreach (var scopeNode in scope_net_topo.Nodes)
                    {
                        scope_net_topo.Edges.AddRange(src_net_topo.Edges.Where(e =>
                                                    e.Node1 == scopeNode.ID && e.Node2 == selectNode ||
                                                    e.Node1 == selectNode & e.Node2 == scopeNode.ID)
                                                    .ToList());
                    }

                    scope_net_topo.ComputingShortestPath();

                    foreach (var node1 in scope_net_topo.Nodes)
                        foreach (var node2 in scope_net_topo.Nodes)
                            if (max_hop_count < scope_net_topo.Path(node1.ID, node2.ID).Count)
                                max_hop_count = scope_net_topo.Path(node1.ID, node2.ID).Count;
                }
            }

            List<int> tmp = new List<int>();
            NetworkTopology remain_topo;

            foreach (var scopeNode in scope_net_topo.Nodes)
                tmp.AddRange(src_net_topo.GetNeighborNodeIDs(scopeNode.ID).Except(scope_net_topo.Nodes.Select(x => x.ID)));
            tmp = tmp.Distinct().ToList();

            deployNodes.AddRange(tmp);

            foreach (int id in tmp)
                scope_net_topo.Nodes.AddRange(src_net_topo.Nodes.Where(x => x.ID == id).ToList());

            foreach (var scopeNode in scope_net_topo.Nodes)
                scope_net_topo.Edges.AddRange(src_net_topo.Edges.Where(x => x.Node1 == scopeNode.ID || x.Node2 == scopeNode.ID).ToList());
            scope_net_topo.Edges = scope_net_topo.Edges.Distinct().ToList();

            remain_topo = src_net_topo - scope_net_topo;

            foreach (int id in tmp)
            {
                scope_net_topo.Nodes.RemoveAll(x => x.ID == id);
                scope_net_topo.Edges.RemoveAll(x => x.Node1 == id || x.Node2 == id);
            }

            return remain_topo;
        }

        private bool findCenterNodeID(NetworkTopology networkTopology, out int centerID)
        {
            double minE = int.MaxValue;
            centerID = -1;

            foreach (var item in networkTopology.Nodes)
            {
                if (minE > networkTopology.Eccentricity(item.ID) && networkTopology.Eccentricity(item.ID) != double.MinValue)
                {
                    minE = networkTopology.Eccentricity(item.ID);
                    centerID = item.ID;
                }
                else if (minE == networkTopology.Eccentricity(item.ID))
                {
                    int minD = networkTopology.Degree(centerID);

                    if (minD > networkTopology.Degree(item.ID))
                        centerID = item.ID;
                }
            }

            return centerID != -1;
        }
    }
}
