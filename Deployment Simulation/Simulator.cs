using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network_Simulation;
using System.Data.SQLite;
using SuperLibrary;
using System.IO;

namespace Deployment_Simulation
{
    public class Simulator
    {
        private class PacketEvent
        {
            public int PacketID { get; set; }
            public double Time { get; set; }
            public int Source { get; set; }
            public int Destination { get; set; }
            public NetworkTopology.NodeType Type { get; set; }
        }

        private class MarkingEvent : PacketEvent
        {
            public int MarkingNodeID { get; set; }

            public MarkingEvent(PacketEvent packetEvent)
            {
                this.PacketID = packetEvent.PacketID;
                this.Time = packetEvent.Time;
                this.Source = packetEvent.Source;
                this.Destination = packetEvent.Destination;
                this.Type = packetEvent.Type;
            }
        }

        private class PacketSentEvent : PacketEvent
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

        private class SimulationSqliteUtility : SQLiteUtils
        {
            /// <summary>
            ///   Key: Table name.
            ///   Value: Schema.
            /// </summary>
            private Dictionary<string, string> tableDic = new Dictionary<string, string>()
            {
                {"UndetectedRatio", "u_id INTEGER PRIMARY KEY AUTOINCREMENT, file_name TEXT, node_counts INTEGER, edge_counts INTEGER, diameter INTEGER, k INTEGER, n INTEGER, metric_name TEXT, ratio REAL"},
                {"SearchingCost", "s_id INTEGER PRIMARY KEY AUTOINCREMENT, file_name TEXT, node_counts INTEGER, edge_counts INTEGER, diameter INTEGER, k INTEGER, n INTEGER, metric_name TEXT, ratio REAL"},
                {"SavingCost", "s_id INTEGER PRIMARY KEY AUTOINCREMENT, file_name TEXT, node_counts INTEGER, edge_counts INTEGER, diameter INTEGER, k INTEGER, n INTEGER, ratio REAL"},
                {"SurvivalMaliciousTrafficRatio", "s_id INTEGER PRIMARY KEY AUTOINCREMENT, file_name TEXT, node_counts INTEGER, edge_counts INTEGER, diameter INTEGER, k INTEGER, n INTEGER, ratio REAL"}
            };

            public SimulationSqliteUtility(string dbFileName)
                : base(Path.Combine(Environment.CurrentDirectory, "Deploy", "SimLog"), dbFileName, false)
            {
            }

            public override void CreateTable()
            {
                try
                {
                    // Create tables
                    using (SQLiteConnection connection = new SQLiteConnection(m_connection_string))
                    {
                        connection.Open();

                        SQLiteCommand cmd = connection.CreateCommand();

                        foreach (var kvp in tableDic)
                        {
                            cmd.CommandText = string.Format("CREATE TABLE IF NOT EXISTS {0}({1})", kvp.Key, kvp.Value);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (SQLiteException ex)
                {
                    if (ex.ErrorCode != SQLiteErrorCode.Constraint)
                        DataUtility.Log(ex.Message + "\n");
                }
            }
        }

        public Deployment Deployment
        {
            set
            {
                if (value != null)
                    m_deployment = value;
            }
        }

        private Deployment m_deployment;
        private NetworkTopology m_networkTopology;
        private SimulationSqliteUtility m_sqlite_utils;

        public Simulator(NetworkTopology networkTopology, string deployName)
        {
            this.m_networkTopology = networkTopology;

            this.m_sqlite_utils = new SimulationSqliteUtility(deployName);
            this.m_sqlite_utils.CreateTable();
        }

        public void Run()
        {
            int packetNumber = 1000;
            int centerID, minE;

            List<int> path;

            List<MarkingEvent> markingEventList = new List<MarkingEvent>();
            List<PacketEvent> packetEventList = new List<PacketEvent>();
            List<PacketSentEvent> packetSentEventList = new List<PacketSentEvent>();

            List<int> firstMeetTracerHopCountList = new List<int>();
            List<int> srcToScopeCenterHopCountList = new List<int>();
            List<int> pathCountList = new List<int>();

            Random rd = new Random(Guid.NewGuid().GetHashCode());

            foreach (int nodeID in m_deployment.DeployNodes)
                m_networkTopology.Nodes.Find(n => n.ID == nodeID).Tracer = NetworkTopology.TracerType.Marking;

            foreach (var scope in m_deployment.AllRoundScopeList)
                scope.FindCenterNodeID(out centerID, out minE, true);

            for (int i = 0; i < packetNumber; i++)
            {
                bool isMarking = false;
                NetworkTopology.Node srcNode = m_networkTopology.Nodes[rd.Next(m_networkTopology.Nodes.Count)];
                NetworkTopology.Node desNode = null;
                while (desNode == null || desNode == srcNode)
                    desNode = m_networkTopology.Nodes[rd.Next(m_networkTopology.Nodes.Count)];

                NetworkTopology sourceScope = m_deployment.AllRoundScopeList.Find(scope => scope.Nodes.Exists(n => n.ID == srcNode.ID));

                if (sourceScope != null)
                {
                    if (sourceScope.FindCenterNodeID(out centerID, out minE) && srcNode.ID != centerID)
                        srcToScopeCenterHopCountList.Add(m_networkTopology.GetShortestPathCount(srcNode.ID, centerID) - 1);
                    else
                        srcToScopeCenterHopCountList.Add(0);
                }
                else
                    srcToScopeCenterHopCountList.Add(0);

                path = m_networkTopology.GetShortestPath(srcNode.ID, desNode.ID);

                PacketEvent packetEvent = new PacketEvent()
                {
                    PacketID = i,
                    Source = srcNode.ID,
                    Destination = desNode.ID,
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

                                firstMeetTracerHopCountList.Add(j);
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

                    PacketSentEvent packetSentEvent = new PacketSentEvent(packetEvent);
                    packetSentEvent.CurrentNodeID = path[j];
                    packetSentEvent.NextHopID = j == path.Count - 1 ? -1 : path[j + 1];
                    packetSentEvent.Length = j == path.Count - 1 ? 0 : m_networkTopology.AdjacentMatrix[m_networkTopology.NodeID2Index(path[j]), m_networkTopology.NodeID2Index(path[j + 1])].Length;

                    packetSentEventList.Add(packetSentEvent);
                }

                pathCountList.Add(path.Count - 1);

                if (!isMarking)
                    firstMeetTracerHopCountList.Add(path.Count - 1);
            }

            m_networkTopology.Reset();

            // Log into db
            double theoreticalUndetectedRatio = (double)m_deployment.AllRoundScopeList.Sum(s => s.Nodes.Count > 1 ? DataUtility.Combination(s.Nodes.Count, 2) : 0) / (double)DataUtility.Combination(m_networkTopology.Nodes.Count, 2);
            double upperboundUndetectedRatio = m_networkTopology.m_prob_hop.Sum(i => (m_networkTopology.m_prob_hop.ToList().IndexOf(i) >= 1 && m_networkTopology.m_prob_hop.ToList().IndexOf(i) <= m_deployment.K - 1) ? i : 0);
            double undetectedRatio = (double)(packetNumber - markingEventList.Count) / (double)packetNumber;
            double firstMeetTracerSearchingCost = (double)firstMeetTracerHopCountList.Sum() / (double)packetNumber;
            double srcToScopeCenterSearchingCost = (double)srcToScopeCenterHopCountList.Sum() / (double)packetNumber;
            double savingCost = (double)pathCountList.Sum(x => x - firstMeetTracerHopCountList[pathCountList.IndexOf(x)]) / (double)packetNumber;
            double survivalTrafficRatio = (double)firstMeetTracerHopCountList.Sum() / (double)pathCountList.Sum();

            string cmd;
            // UndetectedRatio
            cmd = "INSERT INTO UndetectedRatio(file_name, node_counts, edge_counts, diameter, k, n, metric_name, ratio) VALUES(@file_name, @node_counts, @edge_counts, @diameter, @k, @n, @metric_name, @ratio);";
            m_sqlite_utils.RunCommnad(cmd, new List<SQLiteParameter>()
            {
                new SQLiteParameter("@file_name", m_networkTopology.FileName),
                new SQLiteParameter("@node_counts", m_networkTopology.Nodes.Count),
                new SQLiteParameter("@edge_counts", m_networkTopology.Edges.Count),
                new SQLiteParameter("@diameter", m_networkTopology.Diameter),
                new SQLiteParameter("@k", m_deployment.K),
                new SQLiteParameter("@n", m_deployment.N),
                new SQLiteParameter("@metric_name", "Undetected Ratio"),
                new SQLiteParameter("@ratio", undetectedRatio)
            });

            cmd = "INSERT INTO UndetectedRatio(file_name, node_counts, edge_counts, diameter, k, n, metric_name, ratio) VALUES(@file_name, @node_counts, @edge_counts, @diameter, @k, @n, @metric_name, @ratio);";
            m_sqlite_utils.RunCommnad(cmd, new List<SQLiteParameter>()
            {
                new SQLiteParameter("@file_name", m_networkTopology.FileName),
                new SQLiteParameter("@node_counts", m_networkTopology.Nodes.Count),
                new SQLiteParameter("@edge_counts", m_networkTopology.Edges.Count),
                new SQLiteParameter("@diameter", m_networkTopology.Diameter),
                new SQLiteParameter("@k", m_deployment.K),
                new SQLiteParameter("@n", m_deployment.N),
                new SQLiteParameter("@metric_name", "Theoretical Undetected Ratio"),
                new SQLiteParameter("@ratio", theoreticalUndetectedRatio)
            });

            cmd = "INSERT INTO UndetectedRatio(file_name, node_counts, edge_counts, diameter, k, n, metric_name, ratio) VALUES(@file_name, @node_counts, @edge_counts, @diameter, @k, @n, @metric_name, @ratio);";
            m_sqlite_utils.RunCommnad(cmd, new List<SQLiteParameter>()
            {
                new SQLiteParameter("@file_name", m_networkTopology.FileName),
                new SQLiteParameter("@node_counts", m_networkTopology.Nodes.Count),
                new SQLiteParameter("@edge_counts", m_networkTopology.Edges.Count),
                new SQLiteParameter("@diameter", m_networkTopology.Diameter),
                new SQLiteParameter("@k", m_deployment.K),
                new SQLiteParameter("@n", m_deployment.N),
                new SQLiteParameter("@metric_name", "Theoretical Undetected Ratio Upper Bound"),
                new SQLiteParameter("@ratio", upperboundUndetectedRatio)
            });

            // Searching Cost
            cmd = "INSERT INTO SearchingCost(file_name, node_counts, edge_counts, diameter, k, n, metric_name, ratio) VALUES(@file_name, @node_counts, @edge_counts, @diameter, @k, @n, @metric_name, @ratio);";
            m_sqlite_utils.RunCommnad(cmd, new List<SQLiteParameter>()
            {
                new SQLiteParameter("@file_name", m_networkTopology.FileName),
                new SQLiteParameter("@node_counts", m_networkTopology.Nodes.Count),
                new SQLiteParameter("@edge_counts", m_networkTopology.Edges.Count),
                new SQLiteParameter("@diameter", m_networkTopology.Diameter),
                new SQLiteParameter("@k", m_deployment.K),
                new SQLiteParameter("@n", m_deployment.N),
                new SQLiteParameter("@metric_name", "Searching Cost of First Meet Tracer"),
                new SQLiteParameter("@ratio", firstMeetTracerSearchingCost)
            });

            cmd = "INSERT INTO SearchingCost(file_name, node_counts, edge_counts, diameter, k, n, metric_name, ratio) VALUES(@file_name, @node_counts, @edge_counts, @diameter, @k, @n, @metric_name, @ratio);";
            m_sqlite_utils.RunCommnad(cmd, new List<SQLiteParameter>()
            {
                new SQLiteParameter("@file_name", m_networkTopology.FileName),
                new SQLiteParameter("@node_counts", m_networkTopology.Nodes.Count),
                new SQLiteParameter("@edge_counts", m_networkTopology.Edges.Count),
                new SQLiteParameter("@diameter", m_networkTopology.Diameter),
                new SQLiteParameter("@k", m_deployment.K),
                new SQLiteParameter("@n", m_deployment.N),
                new SQLiteParameter("@metric_name", @"Searching Cost of Attacker to Scope Center"),
                new SQLiteParameter("@ratio", srcToScopeCenterSearchingCost)
            });

            // Saving Cost
            cmd = "INSERT INTO SavingCost(file_name, node_counts, edge_counts, diameter, k, n, ratio) VALUES(@file_name, @node_counts, @edge_counts, @diameter, @k, @n, @ratio);";
            m_sqlite_utils.RunCommnad(cmd, new List<SQLiteParameter>()
            {
                new SQLiteParameter("@file_name", m_networkTopology.FileName),
                new SQLiteParameter("@node_counts", m_networkTopology.Nodes.Count),
                new SQLiteParameter("@edge_counts", m_networkTopology.Edges.Count),
                new SQLiteParameter("@diameter", m_networkTopology.Diameter),
                new SQLiteParameter("@k", m_deployment.K),
                new SQLiteParameter("@n", m_deployment.N),
                new SQLiteParameter("@ratio", savingCost)
            });

            // Survival Malicious Traffic Ratio
            cmd = "INSERT INTO SurvivalMaliciousTrafficRatio(file_name, node_counts, edge_counts, diameter, k, n, ratio) VALUES(@file_name, @node_counts, @edge_counts, @diameter, @k, @n, @ratio);";
            m_sqlite_utils.RunCommnad(cmd, new List<SQLiteParameter>()
            {
                new SQLiteParameter("@file_name", m_networkTopology.FileName),
                new SQLiteParameter("@node_counts", m_networkTopology.Nodes.Count),
                new SQLiteParameter("@edge_counts", m_networkTopology.Edges.Count),
                new SQLiteParameter("@diameter", m_networkTopology.Diameter),
                new SQLiteParameter("@k", m_deployment.K),
                new SQLiteParameter("@n", m_deployment.N),
                new SQLiteParameter("@ratio", survivalTrafficRatio)
            });
        }
    }
}
