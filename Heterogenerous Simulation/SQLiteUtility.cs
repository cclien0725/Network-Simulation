﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SQLite;

namespace Heterogenerous_Simulation
{
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
            {"FilteringEvents", "ID INTEGER PRIMARY KEY, PacketID INTEGER, Time REAL, FilteringNodeId INTEGER"}
        };

        private string baseDirectory = Path.Combine(Environment.CurrentDirectory, "Log");
        private string connectionString = @"Data Source=";
        private string fileName;
        private string prefixNameOfTable;

        /// <summary>
        /// SQLite constructor: create db file.
        /// </summary>
        /// <param name="dbFileName">The file name of database.</param>
        public SQLiteUtility(string dbFileName)
        {
            try
            {
                fileName = baseDirectory + @"\" + dbFileName + "_0";

                if (!Directory.Exists(baseDirectory))
                    Directory.CreateDirectory(baseDirectory);

                for (int i = 1; File.Exists(fileName + ".db"); i++)
                    fileName = baseDirectory + @"\" + dbFileName + "_" + i;
                fileName += ".db";

                SQLiteConnection.CreateFile(fileName);

                connectionString += fileName;
            }
            catch { }
        }

        /// <summary>
        /// Create the tables if not exist.
        /// </summary>
        /// <param name="prefixNameOfTable">The prefix-name of table, maybe method name.</param>
        public void CreateTable(string prefixNameOfTable)
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
                        cmd.CommandText = string.Format("CREATE TABLE IF NOT EXISTS {0}_{1}({2})", prefixNameOfTable, kvp.Key, kvp.Value);
                        cmd.ExecuteNonQuery();
                    }
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
    }
}