using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SQLite;

namespace Network_Simulation
{
    public class SQLiteUtility
    {
        /// <summary>
        ///   Key: Table name.
        /// Value: Schema.
        /// </summary>
        private Dictionary<string, string> tableDic = new Dictionary<string, string>()
        {
            {"PacketEvent", "PacketID INTEGER PRIMARY KEY, Time INTEGER, Source INTEGER, Destination INTEGER"},
            {"PacketSentEvents", "ID INTEGER PRIMARY KEY, PacketID INTEGER, Time INTEGER, CurrentNodeID INTEGER, NextHopID INTEGER, Length REAL"},
            {"TunnelingEvents", "ID INTEGER PRIMARY KEY, PacketID INTEGER, Time INTEGER, TunnelingSrc INTEGER, TunnelingDst INTEGER"},
            {"MarkingEvents", "ID INTEGER PRIMARY KEY, PacketID INTEGER, Time INTEGER, MarkingNodeID INTEGER"},
            {"FilteringEvents", "ID INTEGER PRIMARY KEY, PacketID INTEGER, Time INTEGER, FilteringNodeId INTEGER"}
        };

        private string baseDirectory = Environment.CurrentDirectory + @"\Log";
        private string connectionString = @"Data Source=";
        private string fileName;

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

        }

        public void InsertTunnelingEvent(TunnelingEvent tunnelingEvent)
        { }

        public void InsertFilteringEvent(FilteringEvent filteringEvent)
        { }

        public void InsertPacketSentEvent(PacketSentEvent packetSentEvent)
        { }
    }
}
