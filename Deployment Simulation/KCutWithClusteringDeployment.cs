using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network_Simulation;
using System.Data.SQLite;
using System.Data;

namespace Deployment_Simulation
{
    public class KCutWithClusteringDeployment : Deployment
    {
        private int K;
        private int N;
        private int lastDeployCount;
        private int allLevelScopeCount;
        private bool isNeedRecompute;
        private List<List<int>> allLevelDeploy;

        public KCutWithClusteringDeployment(double percentageOfTunnelingTracer, double percentageOfMarkingTracer, double percentageOfFilteringTracer, int KCutValue, int numberOfInsideScopeNode)
            : base(percentageOfTunnelingTracer, percentageOfMarkingTracer, percentageOfFilteringTracer)
        {
            K = KCutValue;
            N = numberOfInsideScopeNode;
        }

        protected override void doDeploy(NetworkTopology networkTopology)
        {
            if (checkHaveRunned(networkTopology))
                isNeedWriteing2SQLite = false;
            else
            {
                NetworkTopology process_topo = networkTopology;
                int centerID;

                isNeedRecompute = false;
                lastDeployCount = 0;
                allLevelScopeCount = 0;

                allLevelDeploy = new List<List<int>>();

                // Finding the center node to run all level's process.
                //while (tmp_src_net_topo.FindCenterNodeID(out centerID, isNeedRecompute))
                while (selectStartNode(process_topo, out centerID, isNeedRecompute))
                {
                    NetworkTopology scope_net_topo = new NetworkTopology(0, 0, 0);
                    List<int> now_level_depoly_id = new List<int>();

                    // Adding the center node to scope network topology.
                    scope_net_topo.Nodes.Add(process_topo.Nodes.Find(n => n.ID == centerID));

                    // Starting run algorithm with this level.
                    process_topo = startAlgorithm(process_topo, scope_net_topo, now_level_depoly_id);

                    // Adding this round generated scope network topology to list.
                    allRoundScopeList.Add(scope_net_topo);
                    deployNodes.AddRange(now_level_depoly_id);
                    allLevelDeploy.Add(now_level_depoly_id);

                    isNeedRecompute = deployNodes.Count != lastDeployCount;
                    lastDeployCount = deployNodes.Count;

                    allLevelScopeCount += scope_net_topo.Nodes.Count;

                    DataUtility.Log(string.Format("================= Level {0} ==================\n", allRoundScopeList.Count));
                    DataUtility.Log(string.Format("Center Node:\t{0}\n", centerID));
                    DataUtility.Log(string.Format("Scope Node Count:\t{0}\n", scope_net_topo.Nodes.Count));
                    DataUtility.Log(string.Format("All Scope Node Count/Run Level:\t{0}/{1} = {2:0.0000}\n", allLevelScopeCount, allRoundScopeList.Count, (float)allLevelScopeCount / (float)allRoundScopeList.Count));
                    DataUtility.Log(string.Format("Deploy Count/Node Count:\t{0}/{1} = {2:0.0000}\n", deployNodes.Count, networkTopology.Nodes.Count, (float)deployNodes.Count / (float)networkTopology.Nodes.Count));
                }
            }

            // Modifying actual tracer type on network topology depend on computed deployment node.
            foreach (int id in deployNodes)
                networkTopology.Nodes.Find(n => n.ID == id).Tracer = NetworkTopology.TracerType.Marking;
        }

        protected override void write2SQLite(NetworkTopology networkTopology)
        {
            string cmd = string.Format("INSERT INTO DeploySimulation(job_id, n_id, k, n, deploy_name) SELECT {0},n_id,{1},{2},'{3}' FROM NetworkTopology WHERE file_name='{4}'", jobID, K, N, GetType().Name, networkTopology.FileName);
            sqlite_utility.RunCommnad(cmd);
            int itemCount = 0;
            StringBuilder sb = new StringBuilder();

            sb.Append("INSERT INTO LevelRecord(job_id, level, node_id, deploy_type)");

            for (int level = 0; level < allRoundScopeList.Count; level++)
            {
                foreach (var n in allRoundScopeList[level].Nodes)
                {
                    if (itemCount != 0 && itemCount % 499 == 0)
                    {
                        sqlite_utility.RunCommnad(sb.ToString().Remove(sb.ToString().Length - 6, 6));
                        sb.Clear();
                        sb.Append("INSERT INTO LevelRecord(job_id, level, node_id, deploy_type)");
                        sb.AppendFormat(" SELECT {0},{1},{2},'{3}' UNION", jobID, level + 1, n.ID, "Scope");
                    }
                    else
                        sb.AppendFormat(" SELECT {0},{1},{2},'{3}' UNION", jobID, level + 1, n.ID, "Scope");

                    itemCount++;
                }
                foreach (int id in allLevelDeploy[level])
                {
                    if (itemCount % 499 == 0)
                    {
                        sqlite_utility.RunCommnad(sb.ToString().Remove(sb.ToString().Length - 6, 6));
                        sb.Clear();
                        sb.Append("INSERT INTO LevelRecord(job_id, level, node_id, deploy_type)");
                        sb.AppendFormat(" SELECT {0},{1},{2},'{3}' UNION", jobID, level + 1, id, "Deploy");
                    }
                    else
                        sb.AppendFormat(" SELECT {0},{1},{2},'{3}' UNION", jobID, level + 1, id, "Deploy");

                    itemCount++;
                }
            }

            if (itemCount % 499 != 0)
                sqlite_utility.RunCommnad(sb.ToString().Remove(sb.ToString().Length - 6, 6));
        }

        /// <summary>
        /// Starting run algorithm with specific source topology and K-diameter cut, 
        /// and this round process will generate scope topology to scop_net_topo,
        /// and will add the deployment node to deployNodes structure.
        /// Finally, it will return the remain network topology with this process.
        /// </summary>
        /// <param name="src_net_topo">The source network topology</param>
        /// <param name="scope_net_topo">This round process that generte scope topology</param>
        /// <param name="nowDeployNodes">The result of the deployment node id</param>
        /// <returns>The remain network topology after process the algorithm.</returns>
        private NetworkTopology startAlgorithm(NetworkTopology src_net_topo, NetworkTopology scope_net_topo, List<int> nowDeployNodes)
        {
            int max_hop_count = int.MinValue;

            while (max_hop_count < K)
            {
                int minDegree = int.MaxValue;
                int selectNode = -1;

                if (!isNeedconsiderClustering(src_net_topo, scope_net_topo, N, out selectNode) && scope_net_topo.Nodes.Count < N)
                {
                    // to finding the neighbor node with minimum degree
                    //foreach (var scopeNode in scope_net_topo.Nodes)
                    //{
                    //    foreach (int neighbor in src_net_topo.GetNeighborNodeIDs(scopeNode.ID))
                    //    {
                    //        if (!scope_net_topo.Nodes.Exists(x => x.ID == neighbor))
                    //        {
                    //            NetworkTopology complement = (src_net_topo - scope_net_topo);
                    //            int degree = complement.Degree(neighbor);

                    //            if (minDegree > degree)
                    //            {
                    //                minDegree = degree;
                    //                selectNode = neighbor;
                    //            }
                    //            else if (minDegree == degree && complement.ClusteringCoefficeint(neighbor) > complement.ClusteringCoefficeint(selectNode))
                    //                selectNode = neighbor;
                    //        }
                    //    } 
                    //}
                    List<int> neighbor = new List<int>();

                    foreach (var scopeNode in scope_net_topo.Nodes)
                        neighbor.AddRange(src_net_topo.GetNeighborNodeIDs(scopeNode.ID).Except(scope_net_topo.Nodes.Select(x => x.ID)));
                    neighbor = neighbor.Distinct().ToList();

                    double max = double.MinValue;

                    foreach (int id in neighbor)
                    {
                        double tmpc = src_net_topo.ClusteringCoefficeint(id);

                        if (max < tmpc)
                        {
                            max = tmpc;
                            selectNode = id;
                        }
                    }
                }

                // if nothing found, break the loop.
                if (selectNode == -1)
                    break;
                // adding the node to the scope set, and computing the max hop count.
                else
                {
                    scope_net_topo.Nodes.Add(src_net_topo.Nodes.Find(n => n.ID == selectNode));

                    foreach (var scopeNode in scope_net_topo.Nodes)
                    {
                        scope_net_topo.Edges.AddRange(src_net_topo.Edges.Where(e =>
                                                    e.Node1 == scopeNode.ID && e.Node2 == selectNode ||
                                                    e.Node1 == selectNode && e.Node2 == scopeNode.ID)
                                                    .ToList());
                    }

                    scope_net_topo.ComputingShortestPath();

                    foreach (var node1 in scope_net_topo.Nodes)
                        foreach (var node2 in scope_net_topo.Nodes)
                        {
                            int hop_count = scope_net_topo.GetShortestPath(node1.ID, node2.ID).Count;

                            if (max_hop_count < hop_count)
                                max_hop_count = hop_count;
                        }
                }
            }

            List<int> tmp = new List<int>();
            NetworkTopology remain_topo;

            // Handling the neighbor nodes of each node in the scope network topology.
            foreach (var scopeNode in scope_net_topo.Nodes)
                tmp.AddRange(src_net_topo.GetNeighborNodeIDs(scopeNode.ID).Except(scope_net_topo.Nodes.Select(x => x.ID)));
            tmp = tmp.Distinct().ToList();

            // During above process the tmp list will be deployment nodes, and add to deployNodes list.
            nowDeployNodes.AddRange(tmp);

            // Adding deploy nodes to the scope network topology.
            foreach (int id in tmp)
                scope_net_topo.Nodes.AddRange(src_net_topo.Nodes.Where(x => x.ID == id).ToList());

            // Adding deploy nodes's edge to the scope network topology.
            foreach (var scopeNode in scope_net_topo.Nodes)
                scope_net_topo.Edges.AddRange(src_net_topo.Edges.Where(x => x.Node1 == scopeNode.ID || x.Node2 == scopeNode.ID).ToList());
            scope_net_topo.Edges = scope_net_topo.Edges.Distinct().ToList();

            // Computing the complement set between source and scope network topology.
            remain_topo = src_net_topo - scope_net_topo;

            // Removing deployment nodes and edges from scope network topology.
            foreach (int id in tmp)
            {
                scope_net_topo.Nodes.RemoveAll(x => x.ID == id);
                scope_net_topo.Edges.RemoveAll(x => x.Node1 == id || x.Node2 == id);
            }

            return remain_topo;
        }

        private bool selectStartNode(NetworkTopology topo, out int selectNode, bool isNeedRecompute)
        {
            int eccentricity;
            int minDegree = int.MaxValue;
            bool isSelected = false;

            if (topo.FindCenterNodeID(out selectNode, out eccentricity, isNeedRecompute) && (eccentricity - Math.Ceiling((double)K / (double)2) > 2))
                isSelected = true;
            else
            {
                foreach (var n in topo.Nodes)
                {
                    if (minDegree > n.Degree)
                    {
                        minDegree = n.Degree;
                        selectNode = n.ID;
                        isSelected = true;
                    }
                }
            }

            return isSelected;
        }

        private bool isNeedconsiderClustering(NetworkTopology src, NetworkTopology scope, int N, out int select_node)
        {
            List<int> neighbor = new List<int>();
            select_node = -1;

            if (scope.Nodes.Count >= N - 1 && scope.Nodes.Count <= N)
            {
                foreach (var scopeNode in scope.Nodes)
                    neighbor.AddRange(src.GetNeighborNodeIDs(scopeNode.ID).Except(scope.Nodes.Select(x => x.ID)));
                neighbor = neighbor.Distinct().ToList();

                double max = double.MinValue;

                foreach (int id in neighbor)
                {
                    double tmp = src.ClusteringCoefficeint(id);

                    if (max < tmp)
                    {
                        max = tmp;
                        select_node = id;
                    }
                }

                if (max >= 0.8 && max <= 1)
                    return true;
                else
                {
                    select_node = -1;
                    return false;
                }
            }
            else if (scope.Nodes.Count > N)
                return true;
            else
                return false;
        }

        private bool checkHaveRunned(NetworkTopology topo)
        {
            string cmd = "SELECT node_id,level,deploy_type FROM NetworkTopology AS N JOIN DeploySimulation AS D on D.n_id=N.n_id JOIN LevelRecord AS L on D.job_id=L.job_id WHERE N.file_name LIKE @file_name AND D.k = @k AND D.n = @n AND D.deploy_name LIKE @deploy_name ORDER BY L.level,L.node_id";

            List<SQLiteParameter> parms = new List<SQLiteParameter>();

            parms.Add(new SQLiteParameter("@file_name", topo.FileName));
            parms.Add(new SQLiteParameter("@k", K));
            parms.Add(new SQLiteParameter("@n", N));
            parms.Add(new SQLiteParameter("@deploy_name", GetType().Name));

            DataView dv = sqlite_utility.GetResult(cmd, parms);

            if (dv != null & dv.Count > 0)
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    switch (Convert.ToString(dv[i]["deploy_type"]))
                    {
                        case "Scope":
                            
                            break;
                        case "Deploy":
                            deployNodes.Add(Convert.ToInt32(dv[i]["node_id"]));
                            break;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
