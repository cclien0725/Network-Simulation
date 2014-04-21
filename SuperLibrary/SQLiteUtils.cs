using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SQLite;
using Network_Simulation;
using System.Data;

namespace SuperLibrary
{
    public abstract class SQLiteUtils
    {
        protected string m_connection_string = @"Data Source=";
        protected string m_full_file_path;

        /// <summary>
        /// SQLite constructor: create db file.
        /// </summary>
        /// <param name="dbFileName">The file name of database.</param>
        public SQLiteUtils(string baseDirectory, string dbFileName, bool isAppendMode)
        {
            try
            {
                if (!Directory.Exists(baseDirectory))
                    Directory.CreateDirectory(baseDirectory);

                if (isAppendMode)
                    m_full_file_path = Path.Combine(baseDirectory, dbFileName + "_0");
                else
                {
                    m_full_file_path = Path.Combine(baseDirectory, dbFileName + "_0");

                    for (int i = 1; File.Exists(m_full_file_path + ".db"); i++)
                        m_full_file_path = Path.Combine(baseDirectory, dbFileName + "_" + i);
                }

                m_full_file_path += ".db";

                if (!File.Exists(m_full_file_path))
                    SQLiteConnection.CreateFile(m_full_file_path);

                m_connection_string += m_full_file_path + ";foreign keys=true;";
            }
            catch { }
        }

        public SQLiteUtils(string existFullFilePath)
        {
            if (!File.Exists(existFullFilePath))
                throw new Exception("The full file path you pass in is not exist.");

            m_full_file_path = existFullFilePath;
            m_connection_string += m_full_file_path + ";foreign keys=true;";
        }

        /// <summary>
        /// Create the tables if not exist.
        /// </summary>
        /// <param name="prefixNameOfTable">The prefix-name of table, maybe method name.</param>
        public abstract void CreateTable();

        public DataView GetResult(string sqlcmd, List<SQLiteParameter> parameters = null)
        {
            try
            {
                DataSet ds = new DataSet();

                using (SQLiteConnection connection = new SQLiteConnection(m_connection_string))
                {
                    connection.Open();
                    
                    SQLiteCommand cmd = connection.CreateCommand();

                    cmd.CommandText = sqlcmd;

                    if (parameters != null)
                        foreach (var item in parameters)
                            cmd.Parameters.Add(item);

                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                    adapter.Fill(ds);
                }

                return ds.Tables.Count > 0 ? ds.Tables[0].DefaultView : null;
            }
            catch { return null; }
        }

        public void RunCommnad(string sqlcmd, List<SQLiteParameter> parameters = null)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(m_connection_string))
                {
                    connection.Open();
                    SQLiteTransaction trans = connection.BeginTransaction();

                    SQLiteCommand cmd = connection.CreateCommand();
                    cmd.CommandText = sqlcmd;

                    if (parameters != null)
                        foreach (var item in parameters)
                            cmd.Parameters.Add(item);

                    cmd.ExecuteNonQuery();

                    trans.Commit();
                }
            }
            catch (SQLiteException ex)
            {
                if (ex.ErrorCode != SQLiteErrorCode.Constraint)
                    DataUtility.Log(ex.Message + "\n");
            }
        }
    }
}
