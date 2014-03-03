using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Network_Simulation
{
	public partial class NetworkTopology
	{
        /// <summary>
        /// Translating to the index number on our structure with specific node ID.
        /// </summary>
        /// <param name="NodeID">The node ID that want to translating.</param>
        /// <returns>The index on our structure.</returns>
        public int NodeID2Index(int NodeID)
        {
            if (Nodes.Count == 0)
                throw new Exception("NodeID2Index() Fail: There is 0 node on the network.");
            
            if (!Nodes.Exists(x => x.ID == NodeID))
                throw new Exception(string.Format("NodeID2Index() Fail: There is no match NodeID: {0} on our structure.", NodeID));

            return Nodes.FindIndex(x => x.ID == NodeID);
        }

        /// <summary>
        /// Translating to the node ID with specific index number on our structure.
        /// </summary>
        /// <param name="NodeIndex">The index number.</param>
        /// <returns>The node ID.</returns>
        public int NodeIndex2ID(int NodeIndex)
        {
            if (Nodes.Count == 0)
                throw new Exception("NodeIndex2ID() Fail: There is 0 node on the network.");

            if (NodeIndex >= Nodes.Count || NodeIndex < 0)
                throw new Exception(string.Format("NodeIndex2ID() Fail: Out of range on our structure of the index: {0}.", NodeIndex));

            return Nodes[NodeIndex].ID;
        }

        /// <summary>
        /// Using Floyd-Warshar algorithm computing shortest path.
        /// </summary>
        public void ComputingShortestPath()
        {
            Console.WriteLine("Computing shortest path...");

            // Create the space of adjacent matrix
            AdjacentMatrix = null;
            AdjacentMatrix = new Adjacency[Nodes.Count, Nodes.Count];

            foreach (var edge in Edges)
            {
                AdjacentMatrix[NodeID2Index(edge.Node1), NodeID2Index(edge.Node2)] = new Adjacency() { Delay = edge.Delay, Length = edge.Length };
                AdjacentMatrix[NodeID2Index(edge.Node2), NodeID2Index(edge.Node1)] = new Adjacency() { Delay = edge.Delay, Length = edge.Length };
            }

            // Floyd-Warshall algorithm
            for (int i = 0; i < Nodes.Count; i++)
            {
                for (int j = 0; j < Nodes.Count; j++)
                {
                    for (int k = j + 1; k < Nodes.Count; k++)
                    {
                        if ((AdjacentMatrix[j, k] == null && AdjacentMatrix[j, i] != null && AdjacentMatrix[i, k] != null) ||
                            (AdjacentMatrix[j, k] != null && AdjacentMatrix[j, i] != null && AdjacentMatrix[i, k] != null && AdjacentMatrix[j, k].Length > AdjacentMatrix[j, i].Length + AdjacentMatrix[i, k].Length))
                        {
                            if (AdjacentMatrix[k, j] == null)
                            {
                                AdjacentMatrix[k, j] = new Adjacency();
                                AdjacentMatrix[j, k] = new Adjacency();
                            }

                            AdjacentMatrix[k, j].Length = AdjacentMatrix[j, k].Length = AdjacentMatrix[j, i].Length + AdjacentMatrix[i, k].Length;
                            AdjacentMatrix[k, j].Delay = AdjacentMatrix[j, k].Delay = AdjacentMatrix[j, i].Delay + AdjacentMatrix[i, k].Delay;
                            AdjacentMatrix[k, j].Predecessor = AdjacentMatrix[j, k].Predecessor = i;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Return the shortest path from source node to destination node.
        /// </summary>
        /// <param name="SourceNodeId">The source node id.</param>
        /// <param name="DestinationNodeId">The destination node id.</param>
        /// <returns>The sequence of the node id, which is shortest path.</returns>
        public List<int> Path(int SourceNodeId, int DestinationNodeId)
        {
            if (Nodes.Count == 0)
                throw new Exception("Path() Fail: There are 0 nodes in the network.");

            List<int> path = new List<int>() { NodeID2Index(SourceNodeId), NodeID2Index(DestinationNodeId) };

            for (int i = 0; i < path.Count - 1; i++)
            {
                if (AdjacentMatrix[path[i], path[i + 1]] != null &&
                    AdjacentMatrix[path[i], path[i + 1]].Predecessor != -1)
                {
                    path.Insert(i + 1, AdjacentMatrix[path[i], path[i + 1]].Predecessor);
                    i--;
                }
            }

            for (int i = 0; i < path.Count; i++)
                path[i] = NodeIndex2ID(path[i]);

            return path;
        }

        /// <summary>
        /// Return the eccentricity value of the specific node ID on the network topology.
        /// </summary>
        /// <param name="NodeID">The node ID on the network topology.</param>
        /// <returns>The eccentricity value for the node ID on the network topology.</returns>
        public double Eccentricity(int NodeID)
        {
            double result = double.MinValue;

            for (int i = 0; i < Nodes.Count; i++)
                if (AdjacentMatrix[NodeID2Index(NodeID), i] != null &&
                    result < AdjacentMatrix[NodeID2Index(NodeID), i].Length)
                    result = AdjacentMatrix[NodeID2Index(NodeID), i].Length;

            return result;
        }

        /// <summary>
        /// Retrun the degree value of the specific node ID on the network topology.
        /// </summary>
        /// <param name="NodeID">The node ID on the network topology.</param>
        /// <returns>The degree value for the node ID on the network topology.</returns>
        public int Degree(int NodeID)
        {
            return Edges.Where(n => n.Node1 == NodeID || n.Node2 == NodeID).ToList().Count;
        }

        /// <summary>
        /// Computing the clustering coefficeint of the node id on the network topology.
        /// </summary>
        /// <param name="NodeID">The node id on the network topology.</param>
        /// <returns>The clustering coefficeint value.</returns>
        public double ClusteringCoefficeint(int NodeID)
        {
            List<int> neighbor = GetNeighborNodeIDs(NodeID);
            List<Edge> neighbor_edge_connect_set = new List<Edge>();
            
            for (int i = 0; i < neighbor.Count; i++)
            {
                List<Edge> tmp = new List<Edge>(Edges.Where(e => e.Node1 == neighbor[i] || e.Node2 == neighbor[i]));

                for (int j = i + 1; j < neighbor.Count; j++)
                    neighbor_edge_connect_set.AddRange(tmp.Where(e => e.Node1 == neighbor[j] || e.Node2 == neighbor[j]));
            }

            double max_edges = (neighbor.Count * (neighbor.Count - 1)) / 2;
            double neighbor_edge_count = neighbor_edge_connect_set.Count;

            if (max_edges > 0)
                return neighbor_edge_count / max_edges;
            else
                return 0;
        }

        /// <summary>
        /// Getting the neighbor node IDs list for the specific node ID on the network topology.
        /// </summary>
        /// <param name="NodeID">The node Id on the network topology.</param>
        /// <returns>The list of neighbors ID which side on the specific node ID.</returns>
        public List<int> GetNeighborNodeIDs(int NodeID)
        {
            List<int> result = Edges.Where(n => n.Node1 == NodeID).Select(n => n.Node2).ToList();

            result.AddRange(Edges.Where(n => n.Node2 == NodeID).Select(n => n.Node1).ToList());

            return result;
        }

        /// <summary>
        /// Finding the center of node id on the network topology.
        /// </summary>
        /// <param name="centerID">The center node id will output.</param>
        /// <returns>Do the network topology can find the center node id.</returns>
        public bool FindCenterNodeID(out int centerID)
        {
            double minE = int.MaxValue;
            double eccentricity;
            centerID = -1;

            foreach (var item in Nodes)
            {
                eccentricity = Eccentricity(item.ID);

                if (minE > eccentricity && eccentricity != double.MinValue)
                {
                    minE = eccentricity;
                    centerID = item.ID;
                }
                else if (minE == eccentricity)
                    if (Degree(centerID) > Degree(item.ID))
                        centerID = item.ID;
            }

            return centerID != -1;
        }

        /// <summary>
        /// Operator overloading(-, minus) that define the complement set between the two network topologies.
        /// </summary>
        /// <param name="left_n">The minuend of the network topology.</param>
        /// <param name="right_n">The subtrahend of the network topology.</param>
        /// <returns>The complement set between the two network topologies.</returns>
        public static NetworkTopology operator -(NetworkTopology left_n, NetworkTopology right_n)
        {
            NetworkTopology result = new NetworkTopology(left_n.percentageOfAttackers, left_n.numberOfVictims);

            result.Nodes = left_n.Nodes.Except(right_n.Nodes).ToList();
            result.Edges = left_n.Edges.Except(right_n.Edges).ToList();
            foreach (var n in right_n.Nodes)
                result.Edges.RemoveAll(e => e.Node1 == n.ID || e.Node2 == n.ID);

            //if (result.Nodes.Count > 0)
            //{
            //    result.Initialize();
            //    result.ComputingShortestPath();
            //}

            return result;
        }

        private long Poisson(double lambda)
        {
            Random rd = new Random();
            long k = 0;
            double L = Math.Exp(-lambda), p = 1;
            double r;

            do
            {
                ++k;
                r = rd.NextDouble();
                p *= r;
            } while (p > L);

            return --k;
        }
	}
}
