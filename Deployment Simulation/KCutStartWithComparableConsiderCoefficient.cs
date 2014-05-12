using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network_Simulation;
using System.Data.SQLite;
using System.Data;

namespace Deployment_Simulation
{
    public class KCutStartWithComparableConsiderCoefficient : Deployment
    {
        private int lastDeployCount;
        private bool isNeedRecompute;
        private List<List<int>> allLevelDeploy;
        private int upperBoundOfMinDegree;
        private long m_undetected_count;

        public KCutStartWithComparableConsiderCoefficient(double percentageOfTunnelingTracer, double percentageOfMarkingTracer, double percentageOfFilteringTracer, int KCutValue, int numberOfInsideScopeNode)
            : base(percentageOfTunnelingTracer, percentageOfMarkingTracer, percentageOfFilteringTracer)
        {
            K = KCutValue;
            N = numberOfInsideScopeNode;
            upperBoundOfMinDegree = (int)Math.Floor(N * 0.4);
        }

        protected override void doDeploy(NetworkTopology networkTopology)
        {
            if (checkHaveRunned(networkTopology))
            {
                isNeedWriteing2SQLite = false;
                isNeedReset = false;
            }
            else
            {
                NetworkTopology process_topo = networkTopology;
                NetworkTopology center_tmp_topo = null;
                NetworkTopology side_tmp_topo = null;
                int centerNode;
                int sideNode;
                int eccentricity;
                int minDegree;

                isNeedRecompute = false;
                lastDeployCount = 0;

                allLevelDeploy = new List<List<int>>();

                while (process_topo.Nodes.Count > 0)
                {
                    centerNode = -1;
                    sideNode = -1;
                    minDegree = int.MaxValue;

                    process_topo.FindCenterNodeID(out centerNode, out eccentricity, false);

                    foreach (var n in process_topo.Nodes)
                    {
                        if (minDegree > n.Degree)
                        {
                            minDegree = n.Degree;
                            sideNode = n.ID;
                        }
                    }

                    NetworkTopology center_scope_net_topo = new NetworkTopology(networkTopology.Nodes);
                    NetworkTopology side_scope_net_topo = new NetworkTopology(networkTopology.Nodes);

                    List<int> center_deploy = new List<int>();
                    List<int> side_deploy = new List<int>();

                    center_scope_net_topo.AdjacentMatrix = networkTopology.AdjacentMatrix;
                    side_scope_net_topo.AdjacentMatrix = networkTopology.AdjacentMatrix;

                    if (centerNode != -1)
                    {
                        center_scope_net_topo.Nodes.Add(process_topo.Nodes.Find(n => n.ID == centerNode));
                        center_tmp_topo = startAlgorithm(process_topo, center_scope_net_topo, center_deploy);
                    }

                    if (sideNode != -1)
                    {
                        side_scope_net_topo.Nodes.Add(process_topo.Nodes.Find(n => n.ID == sideNode));
                        side_tmp_topo = startAlgorithm(process_topo, side_scope_net_topo, side_deploy);
                    }

                    if (center_deploy.Count < side_deploy.Count)
                    {
                        process_topo = center_tmp_topo;
                        allRoundScopeList.Add(center_scope_net_topo);
                        deployNodes.AddRange(center_deploy);
                        allLevelDeploy.Add(center_deploy);

                        if (center_scope_net_topo.Nodes.Count > 1)
                            m_undetected_count += DataUtility.Combination(center_scope_net_topo.Nodes.Count, 2);

                        DataUtility.Log(string.Format("================= Level {0} ==================\n", allRoundScopeList.Count));
                        DataUtility.Log(string.Format("Start From Center Node:\t{0}\n", center_scope_net_topo.Nodes[0].ID));
                        DataUtility.Log(string.Format("Scope Node Count:\t{0}\n", center_scope_net_topo.Nodes.Count));
                        DataUtility.Log(string.Format("Deploy Count/Node Count:\t{0}/{1} = {2:0.0000}\n", deployNodes.Count, networkTopology.Nodes.Count, (float)deployNodes.Count / (float)networkTopology.Nodes.Count));
                    }
                    else
                    {
                        process_topo = side_tmp_topo;
                        allRoundScopeList.Add(side_scope_net_topo);
                        deployNodes.AddRange(side_deploy);
                        allLevelDeploy.Add(side_deploy);

                        if (side_scope_net_topo.Nodes.Count > 1)
                            m_undetected_count += DataUtility.Combination(side_scope_net_topo.Nodes.Count, 2);

                        DataUtility.Log(string.Format("================= Level {0} ==================\n", allRoundScopeList.Count));
                        DataUtility.Log(string.Format("Start From Side Node:\t{0}\n", side_scope_net_topo.Nodes[0].ID));
                        DataUtility.Log(string.Format("Scope Node Count:\t{0}\n", side_scope_net_topo.Nodes.Count));
                        DataUtility.Log(string.Format("Deploy Count/Node Count:\t{0}/{1} = {2:0.0000}\n", deployNodes.Count, networkTopology.Nodes.Count, (float)deployNodes.Count / (float)networkTopology.Nodes.Count));
                    }

                    isNeedRecompute = deployNodes.Count != lastDeployCount;
                    lastDeployCount = deployNodes.Count;
                }

                if (process_topo.Nodes.Count != 0)
                    allRoundScopeList.Add(process_topo);
            }

            // Modifying actual tracer type on network topology depend on computed deployment node.
            foreach (int id in deployNodes)
                networkTopology.Nodes.Find(n => n.ID == id).Tracer = NetworkTopology.TracerType.Tunneling;
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
                if (level < allLevelDeploy.Count)
                {
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
            }

            if (itemCount % 499 != 0)
                sqlite_utility.RunCommnad(sb.ToString().Remove(sb.ToString().Length - 6, 6));

            double ratio = (double)m_undetected_count / (double)DataUtility.Combination(networkTopology.Nodes.Count, 2);

            cmd = "INSERT INTO UndetectedRatio(file_name, node_counts, edge_counts, diameter, k, n, metric_name, ratio) VALUES(@file_name, @node_counts, @edge_counts, @diameter, @k, @n, @metric_name, @ratio);";
            sqlite_utility.RunCommnad(cmd, new List<SQLiteParameter>()
            {
                new SQLiteParameter("@file_name", networkTopology.FileName),
                new SQLiteParameter("@node_counts", networkTopology.Nodes.Count),
                new SQLiteParameter("@edge_counts", networkTopology.Edges.Count),
                new SQLiteParameter("@diameter", networkTopology.Diameter),
                new SQLiteParameter("@k", K),
                new SQLiteParameter("@n", N),
                new SQLiteParameter("@metric_name", "Theoretical Undetected Ratio"),
                new SQLiteParameter("@ratio", ratio)
            });

            double ratio_ub = 0;
            for (int i = 1; i <= K - 1; i++)
                ratio_ub += networkTopology.m_prob_hop[i];

            cmd = "INSERT INTO UndetectedRatio(file_name, node_counts, edge_counts, diameter, k, n, metric_name, ratio) VALUES(@file_name, @node_counts, @edge_counts, @diameter, @k, @n, @metric_name, @ratio);";
            sqlite_utility.RunCommnad(cmd, new List<SQLiteParameter>()
            {
                new SQLiteParameter("@file_name", networkTopology.FileName),
                new SQLiteParameter("@node_counts", networkTopology.Nodes.Count),
                new SQLiteParameter("@edge_counts", networkTopology.Edges.Count),
                new SQLiteParameter("@diameter", networkTopology.Diameter),
                new SQLiteParameter("@k", K),
                new SQLiteParameter("@n", N),
                new SQLiteParameter("@metric_name", "Theoretical Undetected Ratio Upper Bound"),
                new SQLiteParameter("@ratio", ratio_ub)
            });
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
            List<int> neighbor = new List<int>();
            int max_hop_count = int.MinValue;
            int selectNode = scope_net_topo.Nodes[0].ID;

            while (max_hop_count < K)
            {
                if (scope_net_topo.Nodes.Count >= 2 * N - upperBoundOfMinDegree)
                    break;

                neighbor.Remove(selectNode);
                neighbor.AddRange(src_net_topo.GetNeighborNodeIDs(selectNode).Except(scope_net_topo.Nodes.Select(n => n.ID)));
                neighbor = neighbor.Distinct().ToList();

                selectNode = -1;

                if (scope_net_topo.Nodes.Count < upperBoundOfMinDegree)
                {
                    int minD = int.MaxValue;

                    foreach (int id in neighbor)
                    {
                        int tmpD = src_net_topo.Nodes.Find(n => n.ID == id).Degree;

                        if (minD > tmpD)
                        {
                            minD = tmpD;
                            selectNode = id;
                        }
                    }
                }
                else
                {
                    double maxC = double.MinValue;

                    foreach (int id in neighbor)
                    {
                        double tmpc = src_net_topo.ClusteringCoefficient(id);

                        if (maxC < tmpc)
                        {
                            maxC = tmpc;
                            selectNode = id;
                        }
                    }

                    if (scope_net_topo.Nodes.Count >= N && maxC < 0.6)
                        selectNode = -1;
                }

                // if nothing found, break the loop.
                if (selectNode == -1)
                    break;
                // adding the node to the scope set, and computing the max hop count.
                else
                {
                    scope_net_topo.Edges.AddRange(src_net_topo.Edges.Where(e =>
                                                    e.Node1 == selectNode && scope_net_topo.Nodes.Exists(n => n.ID == e.Node2) ||
                                                    e.Node2 == selectNode && scope_net_topo.Nodes.Exists(n => n.ID == e.Node1)
                                                    ));

                    scope_net_topo.Nodes.Add(src_net_topo.Nodes.Find(n => n.ID == selectNode));


                    foreach (var scopeNode in scope_net_topo.Nodes)
                    {
                        int hop_count = scope_net_topo.GetShortestPathCount(scopeNode.ID, selectNode);

                        if (max_hop_count < hop_count)
                            max_hop_count = hop_count;
                    }
                }
            }

            NetworkTopology remain_topo;

            // Handling the neighbor nodes of each node in the scope network topology.
            if (selectNode != -1)
            {
                neighbor.Remove(selectNode);
                neighbor.AddRange(src_net_topo.GetNeighborNodeIDs(selectNode).Except(scope_net_topo.Nodes.Select(n => n.ID)));
                neighbor = neighbor.Distinct().ToList();
            }

            // During above process the tmp list will be deployment nodes, and add to deployNodes list.
            nowDeployNodes.AddRange(neighbor);

            // Adding deploy nodes to the scope network topology.
            foreach (int id in neighbor)
            {
                scope_net_topo.Edges.AddRange(src_net_topo.Edges.Where(e =>
                                                    e.Node1 == id && scope_net_topo.Nodes.Exists(n => n.ID == e.Node2) ||
                                                    e.Node2 == id && scope_net_topo.Nodes.Exists(n => n.ID == e.Node1)
                                                    ));

                scope_net_topo.Nodes.Add(src_net_topo.Nodes.Find(n => n.ID == id));
            }

            // Computing the complement set between source and scope network topology.
            remain_topo = src_net_topo - scope_net_topo;

            // Removing deployment nodes and edges from scope network topology.
            foreach (int id in neighbor)
            {
                scope_net_topo.Nodes.RemoveAll(x => x.ID == id);
                scope_net_topo.Edges.RemoveAll(x => x.Node1 == id || x.Node2 == id);
            }

            return remain_topo;
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

            if (dv != null && dv.Count > 0)
            {
                int now_level;
                int pre_level = -1;
                NetworkTopology scope = null;

                for (int i = 0; i < dv.Count; i++)
                {
                    switch (Convert.ToString(dv[i]["deploy_type"]))
                    {
                        case "Scope":
                            now_level = Convert.ToInt32(dv[i]["level"]);

                            if (now_level != pre_level)
                            {
                                if (scope != null)
                                {
                                    //for (int e1 = 0; e1 < scope.Nodes.Count; e1++)
                                    //{
                                    //    for (int e2 = 0; e2 < scope.Nodes.Count; e2++)
                                    //        scope.Edges.AddRange(topo.Edges.Where(e => e.Node1 == scope.Nodes[e1].ID && e.Node2 == scope.Nodes[e2].ID ||
                                    //                                                    e.Node2 == scope.Nodes[e1].ID && e.Node1 == scope.Nodes[e2].ID));
                                    //}
                                    //scope.Edges = scope.Edges.Distinct().ToList();
                                    //scope.ComputingShortestPath();

                                    allRoundScopeList.Add(scope);
                                }
                                scope = new NetworkTopology(topo.Nodes);
                                scope.Edges = new List<NetworkTopology.Edge>(topo.Edges);
                                scope.AdjacentMatrix = topo.AdjacentMatrix;

                                scope.Nodes.Add(topo.Nodes.Where(n => n.ID == Convert.ToInt32(dv[i]["node_id"])).First());
                            }
                            else
                            {
                                scope.Nodes.Add(topo.Nodes.Where(n => n.ID == Convert.ToInt32(dv[i]["node_id"])).First());
                            }

                            pre_level = now_level;
                            break;
                        case "Deploy":
                            deployNodes.Add(Convert.ToInt32(dv[i]["node_id"]));
                            break;
                    }
                }

                if (scope != null)
                {
                    //for (int e1 = 0; e1 < scope.Nodes.Count; e1++)
                    //{
                    //    for (int e2 = 0; e2 < scope.Nodes.Count; e2++)
                    //        scope.Edges.AddRange(topo.Edges.Where(e => e.Node1 == scope.Nodes[e1].ID && e.Node2 == scope.Nodes[e2].ID ||
                    //                                                    e.Node2 == scope.Nodes[e1].ID && e.Node1 == scope.Nodes[e2].ID));
                    //}
                    //scope.Edges = scope.Edges.Distinct().ToList();
                    //scope.ComputingShortestPath();

                    allRoundScopeList.Add(scope);
                }
                return true;
            }
            return false;
        }
    }
}
