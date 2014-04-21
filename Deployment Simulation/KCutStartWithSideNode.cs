using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network_Simulation;
using System.Data.SQLite;
using System.Data;

namespace Deployment_Simulation
{
    public class KCutStartWithSideNode : Deployment
    {
        private int lastDeployCount;
        private int allLevelScopeCount;
        private bool isNeedRecompute;
        private List<List<int>> allLevelDeploy;

        public KCutStartWithSideNode(double percentageOfTunnelingTracer, double percentageOfMarkingTracer, double percentageOfFilteringTracer, int KCutValue, int numberOfInsideScopeNode)
            : base(percentageOfTunnelingTracer, percentageOfMarkingTracer, percentageOfFilteringTracer)
        {
            K = KCutValue;
            N = numberOfInsideScopeNode;
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
                int centerID;

                isNeedRecompute = false;
                lastDeployCount = 0;
                allLevelScopeCount = 0;

                allLevelDeploy = new List<List<int>>();

                // Finding the center node to run all level's process.
                //while (tmp_src_net_topo.FindCenterNodeID(out centerID, isNeedRecompute))
                while (selectStartNode(process_topo, out centerID, isNeedRecompute))
                {
                    NetworkTopology scope_net_topo = new NetworkTopology(networkTopology.Nodes);
                    scope_net_topo.AdjacentMatrix = networkTopology.AdjacentMatrix;

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
                int selectNode = -1;
                List<int> neighbor = new List<int>();

                foreach (var scopeNode in scope_net_topo.Nodes)
                    neighbor.AddRange(src_net_topo.GetNeighborNodeIDs(scopeNode.ID).Except(scope_net_topo.Nodes.Select(x => x.ID)));
                neighbor = neighbor.Distinct().ToList();

                int minD = int.MaxValue;

                foreach (int id in neighbor)
                {
                    int tmpD = src_net_topo.Nodes.Where(n => n.ID == id).Select(n => n.Degree).First();

                    if (minD > tmpD)
                    {
                        minD = tmpD;
                        selectNode = id;
                    }
                }

                if (scope_net_topo.Nodes.Count > N)
                    selectNode = -1;

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

                    //scope_net_topo.ComputingShortestPath();

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
            int minDegree = int.MaxValue;
            bool isSelected = false;
            selectNode = -1;

            foreach (var n in topo.Nodes)
            {
                if (minDegree > n.Degree)
                {
                    minDegree = n.Degree;
                    selectNode = n.ID;
                    isSelected = true;
                }
            }

            return isSelected;
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

