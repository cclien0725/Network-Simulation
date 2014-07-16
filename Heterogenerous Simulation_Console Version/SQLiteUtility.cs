using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using Network_Simulation;
using System.IO;
using System.Data;

namespace Heterogenerous_Simulation_Console_Version
{
    public class PacketEvent
    {
        public int PacketID { get; set; }
        public double Time { get; set; }
        public int Source { get; set; }
        public int Destination { get; set; }
        public NetworkTopology.NodeType Type { get; set; }
    }

    public class PacketSentEvent : PacketEvent
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

    public class TunnelingEvent : PacketEvent
    {
        public int TunnelingSrc { get; set; }
        public int TunnelingDst { get; set; }

        public TunnelingEvent(PacketEvent packetEvent)
        {
            this.PacketID = packetEvent.PacketID;
            this.Time = packetEvent.Time;
            this.Source = packetEvent.Source;
            this.Destination = packetEvent.Destination;
            this.Type = packetEvent.Type;
        }
    }

    public class MarkingEvent : PacketEvent
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

    public class FilteringEvent : PacketEvent
    {
        public int FilteringNodeID { get; set; }

        public FilteringEvent(PacketEvent packetEvent)
        {
            this.PacketID = packetEvent.PacketID;
            this.Time = packetEvent.Time;
            this.Source = packetEvent.Source;
            this.Destination = packetEvent.Destination;
            this.Type = packetEvent.Type;
        }
    }

    public class SQLiteUtility
    {
        /// <summary>
        ///   Key: Table name.
        /// Value: Schema.
        /// </summary>
        private Dictionary<string, string> tableDic = new Dictionary<string, string>()
        {
            {"PacketEvent", "PacketID INTEGER PRIMARY KEY, Source INTEGER, Destination INTEGER, Type BOOLEAN"},
            {"PacketSentEvents", "ID INTEGER PRIMARY KEY, PacketID INTEGER, Time REAL, CurrentNodeID INTEGER, NextHopID INTEGER, Length REAL"},
            {"TunnelingEvents", "ID INTEGER PRIMARY KEY, PacketID INTEGER, Time REAL, TunnelingSrc INTEGER, TunnelingDst INTEGER"},
            {"MarkingEvents", "ID INTEGER PRIMARY KEY, PacketID INTEGER, Time REAL, MarkingNodeID INTEGER"},
            {"FilteringEvents", "ID INTEGER PRIMARY KEY, PacketID INTEGER, Time REAL, FilteringNodeId INTEGER"},
            {"Deployment", "NodeID INTEGER PRIMARY KEY, TracerType INTEGER, NodeType INTEGER, K INTEGER, N INTEGER, Status BOOLEAN"},
            {"TracingCost", "ID INTEGER PRIMARY KEY, PacketID INTEGER, CurrentNodeID INTEGER, NextHopID INTEGER, Length REAL, Time REAL"}
        };

        private string baseDirectory = Path.Combine(Environment.CurrentDirectory, "Log");
        private string connectionString = @"Data Source=";
        private string fileName;
        private string prefixNameOfTable;

        /// <summary>
        /// SQLite constructor: create db file.
        /// </summary>
        /// <param name="dbFileName">The file name of database.</param>
        public SQLiteUtility(ref string dbFileName)
        {
            try
            {
                fileName = baseDirectory + @"\" + dbFileName + "_0";

                if (!Directory.Exists(baseDirectory))
                    Directory.CreateDirectory(baseDirectory);

                //for (int i = 1; File.Exists(fileName + ".db"); i++)
                //    fileName = baseDirectory + @"\" + dbFileName + "_" + i;
                fileName += ".db";

                if (!File.Exists(fileName))
                    SQLiteConnection.CreateFile(fileName);

                connectionString += fileName;
                dbFileName = fileName;
            }
            catch { }
        }

        /// <summary>
        /// Create the tables if not exist.
        /// </summary>
        /// <param name="prefixNameOfTable">The prefix-name of table, maybe method name.</param>
        public bool CreateTable(string prefixNameOfTable)
        {
            try
            {
                this.prefixNameOfTable = prefixNameOfTable;

                // Create tables
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    SQLiteCommand cmd = connection.CreateCommand();
                    foreach (var kvp in tableDic)
                    {
                        cmd.CommandText = string.Format("SELECT name FROM sqlite_master WHERE type='table' AND name='{0}_{1}'", prefixNameOfTable, kvp.Key);
                        SQLiteDataReader rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                            return false;

                        rd.Close();
                        cmd.CommandText = string.Format("CREATE TABLE IF NOT EXISTS {0}_{1}({2})", prefixNameOfTable, kvp.Key, kvp.Value);
                        cmd.ExecuteNonQuery();
                    }
                }

                return true;
            }
            catch { return false; }
        }

        public void InsertMarkingEvent(List<MarkingEvent> markingEventList)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    SQLiteTransaction trans = connection.BeginTransaction();
                    SQLiteCommand cmd = connection.CreateCommand();

                    foreach (MarkingEvent markingEvent in markingEventList)
                    {
                        cmd.CommandText = string.Format("INSERT INTO {0}_MarkingEvents(PacketID, Time, MarkingNodeID) VALUES(@PacketID, @Time, @MarkingNodeID)", prefixNameOfTable);
                        cmd.Parameters.Add("@PacketID", System.Data.DbType.Int32).Value = markingEvent.PacketID;
                        cmd.Parameters.Add("@Time", System.Data.DbType.Double).Value = markingEvent.Time;
                        cmd.Parameters.Add("@MarkingNodeID", System.Data.DbType.Int32).Value = markingEvent.MarkingNodeID;
                        cmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                }
            }
            catch { }
        }

        public void InsertMarkingEvent(MarkingEvent markingEvent)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    SQLiteTransaction trans = connection.BeginTransaction();
                    SQLiteCommand cmd = connection.CreateCommand();
                    cmd.CommandText = string.Format("INSERT INTO {0}_MarkingEvents(PacketID, Time, MarkingNodeID) VALUES(@PacketID, @Time, @MarkingNodeID)", prefixNameOfTable);
                    cmd.Parameters.Add("@PacketID", System.Data.DbType.Int32).Value = markingEvent.PacketID;
                    cmd.Parameters.Add("@Time", System.Data.DbType.Double).Value = markingEvent.Time;
                    cmd.Parameters.Add("@MarkingNodeID", System.Data.DbType.Int32).Value = markingEvent.MarkingNodeID;
                    cmd.ExecuteNonQuery();
                    trans.Commit();
                }
            }
            catch { }
        }

        public void InsertTunnelingEvent(List<TunnelingEvent> tunnelingEventList)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    SQLiteTransaction trans = connection.BeginTransaction();
                    SQLiteCommand cmd = connection.CreateCommand();

                    foreach (TunnelingEvent tunnelingEvent in tunnelingEventList)
                    {
                        cmd.CommandText = string.Format("INSERT INTO {0}_TunnelingEvents(PacketID, Time, TunnelingSrc, TunnelingDst) VALUES(@PacketID, @Time, @TunnelingSrc, @TunnelingDst)", prefixNameOfTable);
                        cmd.Parameters.Add("@PacketID", System.Data.DbType.Int32).Value = tunnelingEvent.PacketID;
                        cmd.Parameters.Add("@Time", System.Data.DbType.Double).Value = tunnelingEvent.Time;
                        cmd.Parameters.Add("@TunnelingSrc", System.Data.DbType.Int32).Value = tunnelingEvent.TunnelingSrc;
                        cmd.Parameters.Add("@TunnelingDst", System.Data.DbType.Int32).Value = tunnelingEvent.TunnelingDst;
                        cmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                }
            }
            catch { }
        }

        public void InsertTunnelingEvent(TunnelingEvent tunnelingEvent)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    SQLiteTransaction trans = connection.BeginTransaction();
                    SQLiteCommand cmd = connection.CreateCommand();
                    cmd.CommandText = string.Format("INSERT INTO {0}_TunnelingEvents(PacketID, Time, TunnelingSrc, TunnelingDst) VALUES(@PacketID, @Time, @TunnelingSrc, @TunnelingDst)", prefixNameOfTable);
                    cmd.Parameters.Add("@PacketID", System.Data.DbType.Int32).Value = tunnelingEvent.PacketID;
                    cmd.Parameters.Add("@Time", System.Data.DbType.Double).Value = tunnelingEvent.Time;
                    cmd.Parameters.Add("@TunnelingSrc", System.Data.DbType.Int32).Value = tunnelingEvent.TunnelingSrc;
                    cmd.Parameters.Add("@TunnelingDst", System.Data.DbType.Int32).Value = tunnelingEvent.TunnelingDst;
                    cmd.ExecuteNonQuery();
                    trans.Commit();
                }
            }
            catch { }
        }

        public void InsertFilteringEvent(FilteringEvent filteringEvent)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    SQLiteTransaction trans = connection.BeginTransaction();
                    SQLiteCommand cmd = connection.CreateCommand();
                    cmd.CommandText = string.Format("INSERT INTO {0}_FilteringEvents(PacketID, Time, FilteringNodeId) VALUES(@PacketID, @Time, @FilteringNodeId)", prefixNameOfTable);
                    cmd.Parameters.Add("@PacketID", System.Data.DbType.Int32).Value = filteringEvent.PacketID;
                    cmd.Parameters.Add("@Time", System.Data.DbType.Double).Value = filteringEvent.Time;
                    cmd.Parameters.Add("@FilteringNodeId", System.Data.DbType.Int32).Value = filteringEvent.FilteringNodeID;
                    cmd.ExecuteNonQuery();
                    trans.Commit();
                }
            }
            catch { }
        }

        public void InsertFilteringEvent(List<FilteringEvent> filteringEventList)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    SQLiteTransaction trans = connection.BeginTransaction();
                    SQLiteCommand cmd = connection.CreateCommand();

                    foreach (FilteringEvent filteringEvent in filteringEventList)
                    {
                        cmd.CommandText = string.Format("INSERT INTO {0}_FilteringEvents(PacketID, Time, FilteringNodeId) VALUES(@PacketID, @Time, @FilteringNodeId)", prefixNameOfTable);
                        cmd.Parameters.Add("@PacketID", System.Data.DbType.Int32).Value = filteringEvent.PacketID;
                        cmd.Parameters.Add("@Time", System.Data.DbType.Double).Value = filteringEvent.Time;
                        cmd.Parameters.Add("@FilteringNodeId", System.Data.DbType.Int32).Value = filteringEvent.FilteringNodeID;
                        cmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                }
            }
            catch { }
        }

        public void InsertPacketSentEvent(PacketSentEvent packetSentEvent)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    SQLiteTransaction trans = connection.BeginTransaction();
                    SQLiteCommand cmd = connection.CreateCommand();
                    cmd.CommandText = string.Format("INSERT INTO {0}_PacketSentEvents(PacketID, Time, CurrentNodeID, NextHopID, Length) VALUES(@PacketID, @Time, @CurrentNodeID, @NextHopID, @Length)", prefixNameOfTable);
                    cmd.Parameters.Add("@PacketID", System.Data.DbType.Int32).Value = packetSentEvent.PacketID;
                    cmd.Parameters.Add("@Time", System.Data.DbType.Double).Value = packetSentEvent.Time;
                    cmd.Parameters.Add("@CurrentNodeID", System.Data.DbType.Int32).Value = packetSentEvent.CurrentNodeID;
                    cmd.Parameters.Add("@NextHopID", System.Data.DbType.Int32).Value = packetSentEvent.NextHopID;
                    cmd.Parameters.Add("@Length", System.Data.DbType.Double).Value = packetSentEvent.Length;
                    cmd.ExecuteNonQuery();
                    trans.Commit();
                }
            }
            catch { }
        }

        public void InsertPacketSentEvent(List<PacketSentEvent> packetSentEventList)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    SQLiteTransaction trans = connection.BeginTransaction();
                    SQLiteCommand cmd = connection.CreateCommand();

                    foreach (PacketSentEvent packetSentEvent in packetSentEventList)
                    {
                        cmd.CommandText = string.Format("INSERT INTO {0}_PacketSentEvents(PacketID, Time, CurrentNodeID, NextHopID, Length) VALUES(@PacketID, @Time, @CurrentNodeID, @NextHopID, @Length)", prefixNameOfTable);
                        cmd.Parameters.Add("@PacketID", System.Data.DbType.Int32).Value = packetSentEvent.PacketID;
                        cmd.Parameters.Add("@Time", System.Data.DbType.Double).Value = packetSentEvent.Time;
                        cmd.Parameters.Add("@CurrentNodeID", System.Data.DbType.Int32).Value = packetSentEvent.CurrentNodeID;
                        cmd.Parameters.Add("@NextHopID", System.Data.DbType.Int32).Value = packetSentEvent.NextHopID;
                        cmd.Parameters.Add("@Length", System.Data.DbType.Double).Value = packetSentEvent.Length;
                        cmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                }
            }
            catch { }
        }

        public void InsertPacketEvent(PacketEvent packetEvent)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    SQLiteTransaction trans = connection.BeginTransaction();
                    SQLiteCommand cmd = connection.CreateCommand();
                    cmd.CommandText = string.Format("INSERT INTO {0}_PacketEvent(PacketID, Source, Destination, Type) VALUES(@PacketID, @Source, @Destination, @Type)", prefixNameOfTable);
                    cmd.Parameters.Add("@PacketID", System.Data.DbType.Int32).Value = packetEvent.PacketID;
                    cmd.Parameters.Add("@Source", System.Data.DbType.Int32).Value = packetEvent.Source;
                    cmd.Parameters.Add("@Destination", System.Data.DbType.Int32).Value = packetEvent.Destination;
                    cmd.Parameters.Add("@Type", System.Data.DbType.Boolean).Value = packetEvent.Type == Network_Simulation.NetworkTopology.NodeType.Attacker ? true : false;
                    cmd.ExecuteNonQuery();
                    trans.Commit();
                }
            }
            catch { }
        }

        public void InsertPacketEvent(List<PacketEvent> packetEventList)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    SQLiteTransaction trans = connection.BeginTransaction();
                    SQLiteCommand cmd = connection.CreateCommand();

                    foreach (PacketEvent packetEvent in packetEventList)
                    {
                        cmd.CommandText = string.Format("INSERT INTO {0}_PacketEvent(PacketID, Source, Destination, Type) VALUES(@PacketID, @Source, @Destination, @Type)", prefixNameOfTable);
                        cmd.Parameters.Add("@PacketID", System.Data.DbType.Int32).Value = packetEvent.PacketID;
                        cmd.Parameters.Add("@Source", System.Data.DbType.Int32).Value = packetEvent.Source;
                        cmd.Parameters.Add("@Destination", System.Data.DbType.Int32).Value = packetEvent.Destination;
                        cmd.Parameters.Add("@Type", System.Data.DbType.Boolean).Value = packetEvent.Type == Network_Simulation.NetworkTopology.NodeType.Attacker ? true : false;
                        cmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                }
            }
            catch { }
        }

        public void LogDeploymentResult(NetworkTopology networkTopology, Deployment deployment)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    SQLiteTransaction trans = connection.BeginTransaction();
                    SQLiteCommand cmd = connection.CreateCommand();
                    cmd.CommandText = string.Format("INSERT INTO {0}_Deployment(NodeID, TracerType, NodeType, K, N, Status) VALUES(@NodeID, @TracerType, @NodeType, @K, @N, @Status)", prefixNameOfTable);

                    foreach (Network_Simulation.NetworkTopology.Node node in networkTopology.Nodes)
                    {
                        cmd.Parameters.Add("@NodeID", DbType.Int32).Value = node.ID;
                        cmd.Parameters.Add("@TracerType", DbType.Int32).Value = node.Tracer;
                        cmd.Parameters.Add("@NodeType", DbType.Int32).Value = node.Type;
                        cmd.Parameters.Add("@K", DbType.Int32).Value = deployment.K;
                        cmd.Parameters.Add("@N", DbType.Int32).Value = deployment.N;
                        cmd.Parameters.Add("@Status", DbType.Boolean).Value = node.IsTunnelingActive;
                        cmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                }
            }
            catch { }
        }

        public void InsertTracingCost(List<PacketSentEvent> tracingList)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    SQLiteTransaction trans = connection.BeginTransaction();
                    SQLiteCommand cmd = connection.CreateCommand();
                    cmd.CommandText = string.Format("INSERT INTO {0}_TracingCost(PacketID, CurrentNodeID, NextHopID, Length, Time) VALUES(@PacketID, @CurrentNodeID, @NextHopID, @Length, @Time)", prefixNameOfTable);

                    foreach (PacketSentEvent e in tracingList)
                    {
                        cmd.Parameters.Add("@PacketID", System.Data.DbType.Int32).Value = e.PacketID;
                        cmd.Parameters.Add("@CurrentNodeID", System.Data.DbType.Int32).Value = e.CurrentNodeID;
                        cmd.Parameters.Add("@NextHopID", System.Data.DbType.Int32).Value = e.NextHopID;
                        cmd.Parameters.Add("@Length", System.Data.DbType.Double).Value = e.Length;
                        cmd.Parameters.Add("@Time", System.Data.DbType.Double).Value = e.Time;
                        cmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                }
            }
            catch { }
        }

        internal void LoadAttackersAndVictim(ref NetworkTopology networkTopology)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    SQLiteCommand cmd = connection.CreateCommand();
                    cmd.CommandText = string.Format("SELECT * FROM NoneDeployment_Deployment");
                    SQLiteDataReader rd = cmd.ExecuteReader();

                    while (rd.Read())
                    {
                        NetworkTopology.Node node = networkTopology.Nodes.Find(n => n.ID == Convert.ToInt32(rd["NodeID"]));
                        node.Type = (NetworkTopology.NodeType)Convert.ToInt32(rd["NodeType"]);
                    }
                }
            }
            catch { }
        }
    }
}
