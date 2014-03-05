using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.IO;

namespace Network_Simulation
{
    class SQLiteUtility
    {
        private string baseDirectory = Environment.CurrentDirectory + @"\Log";
        private string connectionString = @"Data Source=";
        private string fileName;

        public SQLiteUtility(string dbFileName, string tableName)
        {
            this.fileName = baseDirectory + @"\" + dbFileName + ".db";

            if (!Directory.Exists(baseDirectory))
                Directory.CreateDirectory(baseDirectory);

            if (!File.Exists(this.fileName))
                SQLiteConnection.CreateFile(this.fileName);

            connectionString += fileName;

            // Create table
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand cmd = connection.CreateCommand();
                cmd.CommandText = string.Format("CREATE TABLE IF NOT EXISTS {0}", tableName);
                cmd.ExecuteNonQuery();
            } 


        }
    }
}
