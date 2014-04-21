using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SuperLibrary;
using Deployment_Simulation;
using System.Text.RegularExpressions;
using System.Data.SQLite;

namespace Network_Simulation
{
    public partial class DeployStatisticTool : Form
    {
        private List<string> m_hetero_file_names;
        private List<string> m_deploy_file_names;
        private List<string> m_deploy_names;
        private List<HeteroSimulationResults> m_hetero_simulation_result_list;
        private QuerySQLiteUtils m_query_sql;
        private DeployStatisticSQLiteUtils m_deploy_statistic_sql;
        private HeteroDeployStatisticSQLiteUtils m_hetero_statistic_sql;
        private BackgroundWorker m_deploy_worker;
        private BackgroundWorker m_hetero_worker;

        public DeployStatisticTool()
        {
            InitializeComponent();

            m_hetero_file_names = new List<string>();
            m_deploy_file_names = new List<string>();

            m_hetero_simulation_result_list = new List<HeteroSimulationResults>();

            m_deploy_names = new List<string>();
            m_deploy_names.Add(typeof(KCutStartWithCenterNode).Name);
            m_deploy_names.Add(typeof(KCutStartWithCenterNodeConsiderCoefficient).Name);
            m_deploy_names.Add(typeof(KCutStartWithComparableConsiderCoefficient).Name);
            m_deploy_names.Add(typeof(KCutStartWithConsider2KConsiderCoefficient).Name);
            m_deploy_names.Add(typeof(KCutStartWithSideNode).Name);
            m_deploy_names.Add(typeof(KCutStartWithSideNodeConsiderCoefficient).Name);

            m_deploy_worker = new BackgroundWorker();
            m_deploy_worker.WorkerSupportsCancellation = true;
            m_deploy_worker.WorkerReportsProgress = true;

            m_hetero_worker = new BackgroundWorker();
            m_hetero_worker.WorkerSupportsCancellation = true;
            m_hetero_worker.WorkerReportsProgress = true;

            deployEventRegist();
            heteroEventRegist();
        }

        private void deployEventRegist()
        {
            m_deploy_worker.DoWork += (s, e) =>
            {
                double count_of_total_runs = m_deploy_file_names.Count * 2 + 2;
                double now_runs = 0;
                string cmd;
                DataView dv;
                m_deploy_statistic_sql = new DeployStatisticSQLiteUtils();
                int itemCount;
                StringBuilder sb = new StringBuilder();

                foreach (string filePath in m_deploy_file_names)
                {
                    m_query_sql = new QuerySQLiteUtils(filePath);

                    // Count of All Level.
                    m_deploy_worker.ReportProgress((int)(now_runs / count_of_total_runs * 100.0), new object[] { filePath, "Processing (1/2)..." });
                    cmd = "SELECT file_name, node_counts, edge_counts, diameter, k, n, deploy_name, level, deploy_type, COUNT(*) AS count_of_nodes FROM (NetworkTopology INNER JOIN DeploySimulation ON NetworkTopology.n_id = DeploySimulation.n_id) INNER JOIN LevelRecord ON DeploySimulation.job_id = LevelRecord.job_id GROUP BY file_name, node_counts, diameter, k, n, deploy_name, level, deploy_type;";
                    dv = m_query_sql.GetResult(cmd);

                    itemCount = 0;
                    sb.Clear();
                    sb.Append("INSERT INTO CountOfAllLevel(file_name, node_counts, edge_counts, diameter, k, n, deploy_name, level, deploy_type, count_of_nodes)");

                    if (dv != null && dv.Count > 0)
                    {
                        for (int i = 0; i < dv.Count; i++)
                        {
                            if (itemCount != 0 && itemCount % 499 == 0)
                            {
                                m_deploy_statistic_sql.RunCommnad(sb.ToString().Remove(sb.ToString().Length - 6, 6));
                                sb.Clear();
                                sb.Append("INSERT INTO CountOfAllLevel(file_name, node_counts, edge_counts, diameter, k, n, deploy_name, level, deploy_type, count_of_nodes)");
                                sb.AppendFormat(" SELECT '{0}',{1},{2},{3},{4},{5},'{6}',{7},'{8}',{9} UNION", dv[i]["file_name"], dv[i]["node_counts"], dv[i]["edge_counts"], dv[i]["diameter"], dv[i]["k"], dv[i]["n"], dv[i]["deploy_name"], dv[i]["level"], dv[i]["deploy_type"], dv[i]["count_of_nodes"]);
                            }
                            else
                                sb.AppendFormat(" SELECT '{0}',{1},{2},{3},{4},{5},'{6}',{7},'{8}',{9} UNION", dv[i]["file_name"], dv[i]["node_counts"], dv[i]["edge_counts"], dv[i]["diameter"], dv[i]["k"], dv[i]["n"], dv[i]["deploy_name"], dv[i]["level"], dv[i]["deploy_type"], dv[i]["count_of_nodes"]);
                            itemCount++;
                        }

                        if (itemCount % 499 != 0)
                            m_deploy_statistic_sql.RunCommnad(sb.ToString().Remove(sb.ToString().Length - 6, 6));
                    }

                    // Count of Non-Level.
                    m_deploy_worker.ReportProgress((int)(++now_runs / count_of_total_runs * 100.0), new object[] { filePath, "Processing (2/2)..." });
                    cmd = "SELECT file_name, node_counts, edge_counts, diameter, k, n, deploy_name, deploy_type, COUNT(*) AS count_of_nodes FROM (NetworkTopology INNER JOIN DeploySimulation ON NetworkTopology.n_id = DeploySimulation.n_id) INNER JOIN LevelRecord ON DeploySimulation.job_id = LevelRecord.job_id GROUP BY file_name, node_counts, diameter, k, n, deploy_name, deploy_type;";
                    dv = m_query_sql.GetResult(cmd);

                    itemCount = 0;
                    sb.Clear();
                    sb.Append("INSERT INTO CountOfNonLevel(file_name, node_counts, edge_counts, diameter, k, n, deploy_name, deploy_type, count_of_nodes)");

                    if (dv != null && dv.Count > 0)
                    {
                        for (int i = 0; i < dv.Count; i++)
                        {
                            if (itemCount != 0 && itemCount % 499 == 0)
                            {
                                m_deploy_statistic_sql.RunCommnad(sb.ToString().Remove(sb.ToString().Length - 6, 6));
                                sb.Clear();
                                sb.Append("INSERT INTO CountOfNonLevel(file_name, node_counts, edge_counts, diameter, k, n, deploy_name, deploy_type, count_of_nodes)");
                                sb.AppendFormat(" SELECT '{0}',{1},{2},{3},{4},{5},'{6}','{7}',{8} UNION", dv[i]["file_name"], dv[i]["node_counts"], dv[i]["edge_counts"], dv[i]["diameter"], dv[i]["k"], dv[i]["n"], dv[i]["deploy_name"], dv[i]["deploy_type"], dv[i]["count_of_nodes"]);
                            }
                            else
                                sb.AppendFormat(" SELECT '{0}',{1},{2},{3},{4},{5},'{6}','{7}',{8} UNION", dv[i]["file_name"], dv[i]["node_counts"], dv[i]["edge_counts"], dv[i]["diameter"], dv[i]["k"], dv[i]["n"], dv[i]["deploy_name"], dv[i]["deploy_type"], dv[i]["count_of_nodes"]);
                            itemCount++;
                        }

                        if (itemCount % 499 != 0)
                            m_deploy_statistic_sql.RunCommnad(sb.ToString().Remove(sb.ToString().Length - 6, 6));
                    }

                    m_deploy_worker.ReportProgress((int)(++now_runs / count_of_total_runs * 100.0), new object[] { filePath, "Completed (2/2)." });
                }

                // Count of All Level with Max N.
                cmd = "INSERT INTO CountOfAllLevelWithMaxN(file_name, node_counts, edge_counts, diameter, k, n, deploy_name, level, deploy_type, count_of_nodes) SELECT c.file_name, c.node_counts, c.edge_counts, c.diameter, c.k, c.n,c.deploy_name, c.level, c.deploy_type, c.count_of_nodes FROM (SELECT file_name, node_counts, edge_counts, diameter, k, MAX(n) AS max_n FROM CountOfAllLevel GROUP BY file_name, node_counts, edge_counts, diameter, k) AS m JOIN CountOfAllLevel AS c ON m.file_name=c.file_name and m.node_counts=c.node_counts and m.edge_counts=c.edge_counts and m.diameter=c.diameter and m.k=c.k and m.max_n=c.n;";
                m_deploy_statistic_sql.RunCommnad(cmd);

                // Count of Non-Level with Max N.
                m_deploy_worker.ReportProgress((int)(++now_runs / count_of_total_runs * 100.0));
                cmd = "INSERT INTO CountOfNonLevelWithMaxN(file_name, node_counts, edge_counts, diameter, k, n, deploy_name, deploy_type, count_of_nodes) SELECT c.file_name, c.node_counts, c.edge_counts, c.diameter, c.k, c.n,c.deploy_name, c.deploy_type, c.count_of_nodes FROM (SELECT file_name, node_counts, edge_counts, diameter, k, MAX(n) AS max_n FROM CountOfNonLevel GROUP BY file_name, node_counts, edge_counts, diameter, k) AS m JOIN CountOfNonLevel AS c ON m.file_name=c.file_name and m.node_counts=c.node_counts and m.edge_counts=c.edge_counts and m.diameter=c.diameter and m.k=c.k and m.max_n=c.n;";
                m_deploy_statistic_sql.RunCommnad(cmd);

                m_deploy_worker.ReportProgress((int)(++now_runs / count_of_total_runs * 100.0));
            };

            m_deploy_worker.ProgressChanged += (s, e) =>
            {
                if (e.UserState != null)
                {
                    object[] args = e.UserState as object[];
                    string file = args[0] as string;
                    string status = args[1] as string;

                    lv_deploy.Items[file].SubItems[1].Text = status;

                    foreach (int index in lv_deploy.SelectedIndices)
                        lv_deploy.Items[index].Selected = false;

                    lv_deploy.Items[file].Selected = true;
                    lv_deploy.Items[file].Focused = true;
                    lv_deploy.TopItem = lv_deploy.Items[file];
                }

                pb_deploy.Value = e.ProgressPercentage;
            };

            m_deploy_worker.RunWorkerCompleted += (s, e) =>
            {
                btn_deploy_start.Enabled = true;
                tb_deploy_select_files.Enabled = true;
            };

            tb_deploy_select_files.Click += (s, e) =>
            {
                OpenFileDialog ofd = new OpenFileDialog();

                ofd.Filter = "Database Files|*.db";
                ofd.Multiselect = true;
                ofd.Title = "Select Deploy Simulation Database to Process...";
                ofd.InitialDirectory = Path.Combine(Environment.CurrentDirectory, "Deploy");

                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    m_deploy_file_names = ofd.FileNames.ToList();

                    lv_deploy.BeginUpdate();

                    lv_deploy.Items.Clear();

                    foreach (var file in ofd.FileNames)
                    {
                        ListViewItem lvi = new ListViewItem(new string[] { Path.GetFileName(file), "Queued." });
                        lvi.Name = file;

                        lv_deploy.Items.Add(lvi);
                    }

                    lv_deploy.EndUpdate();
                }
            };

            btn_deploy_start.Click += (s, e) =>
            {
                if (!m_deploy_worker.IsBusy)
                {
                    lv_deploy.Focus();
                    m_deploy_worker.RunWorkerAsync();

                    btn_deploy_start.Enabled = false;
                    tb_deploy_select_files.Enabled = false;
                }
            };
        }

        private void heteroEventRegist()
        {
            m_hetero_worker.DoWork += (s, e) =>
            {
                double count_of_total_runs = m_hetero_file_names.Count * m_deploy_names.Count;
                double now_runs = 0;
                int k, n, countOfDeploy, countOfTunneling;
                int tunneling, marking, filtering, attacker, victim, packets;
                string fileName;

                m_hetero_simulation_result_list.Clear();

                foreach (string filePath in m_hetero_file_names)
                {
                    m_query_sql = new QuerySQLiteUtils(filePath);

                    if (Regex.IsMatch(Path.GetFileNameWithoutExtension(filePath), @"(^[\w\d]+)_T(\d+)M(\d+)F(\d+)_A(\d+)V(\d+)_Pkt(\d+)_(\d+)"))
                    {
                        GroupCollection gc = Regex.Match(Path.GetFileNameWithoutExtension(filePath), @"(^[\w\d]+)_T(\d+)M(\d+)F(\d+)_A(\d+)V(\d+)_Pkt(\d+)_(\d+)").Groups;

                        fileName = Convert.ToString(gc[1].Value);
                        tunneling = Convert.ToInt32(gc[2].Value);
                        marking = Convert.ToInt32(gc[3].Value);
                        filtering = Convert.ToInt32(gc[4].Value);
                        attacker = Convert.ToInt32(gc[5].Value);
                        victim = Convert.ToInt32(gc[6].Value);
                        packets = Convert.ToInt32(gc[7].Value);
                    }
                    else
                    {
                        fileName = "Unkown";
                        tunneling = 0;
                        marking = 0;
                        filtering = 0;
                        attacker = 0;
                        victim = 0;
                        packets = 0;
                    }

                    DataUtility.Log(string.Format("f={0}, t={1}, m={2}, f={3}, a={4}, v={5}, p={6}\n", fileName, tunneling, marking, filtering, attacker, victim, packets));

                    foreach (string deploy in m_deploy_names)
                    {
                        string cmd = string.Format("SELECT K, N, count_of_deploy, count_of_tunneling FROM (SELECT K, N, COUNT(*) AS count_of_deploy FROM {0}_Deployment WHERE TracerType=2), (SELECT COUNT(*) AS count_of_tunneling FROM {0}_TunnelingEvents);",
                                                    deploy);

                        DataView dv = m_query_sql.GetResult(cmd);

                        if (dv != null && dv.Count > 0)
                        {
                            k = Convert.ToInt32(dv[0]["K"]);
                            n = Convert.ToInt32(dv[0]["N"]);
                            countOfDeploy = Convert.ToInt32(dv[0]["count_of_deploy"]);
                            countOfTunneling = Convert.ToInt32(dv[0]["count_of_tunneling"]);

                            m_hetero_simulation_result_list.Add(new HeteroSimulationResults()
                            {
                                file_name = fileName,
                                tunneling = tunneling,
                                marking = marking,
                                filtering = filtering,
                                attacker = attacker,
                                victim = victim,
                                packets = packets,
                                k = k,
                                n = n,
                                deploy_name = deploy,
                                count_of_deploy = countOfDeploy,
                                count_of_tunneling = countOfTunneling
                            });

                            DataUtility.Log(string.Format("k={0}, n={1}, d={2}, t={3}, d_name={4}\n\n", k, n, countOfDeploy, countOfTunneling, deploy));
                        }

                        m_hetero_worker.ReportProgress((int)(++now_runs / count_of_total_runs * 100.0));
                    }
                    m_hetero_worker.ReportProgress((int)(now_runs / count_of_total_runs * 100.0), new object[] { filePath, "Completed." });
                }

                write2SQLite();
            };

            m_hetero_worker.ProgressChanged += (s, e) =>
            {
                if (e.UserState != null)
                {
                    object[] args = e.UserState as object[];
                    string file = args[0] as string;
                    string status = args[1] as string;

                    lv_hetero.Items[file].SubItems[1].Text = status;

                    foreach (int index in lv_hetero.SelectedIndices)
                        lv_hetero.Items[index].Selected = false;

                    lv_hetero.Items[file].Selected = true;
                    lv_hetero.Items[file].Focused = true;
                    lv_hetero.TopItem = lv_hetero.Items[file];
                }

                pb_hetero.Value = e.ProgressPercentage;
            };

            m_hetero_worker.RunWorkerCompleted += (s, e) =>
            {
                btn_hetero_start.Enabled = true;
                tb_hetero_select_files.Enabled = true;
            };

            tb_hetero_select_files.Click += (s, e) =>
            {
                OpenFileDialog ofd = new OpenFileDialog();

                ofd.Filter = "Database Files|*.db";
                ofd.Multiselect = true;
                ofd.Title = "Select Heterogenerous Simulation Database to Process...";
                ofd.InitialDirectory = Path.Combine(Environment.CurrentDirectory, "Log");

                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    m_hetero_file_names = ofd.FileNames.ToList();

                    lv_hetero.BeginUpdate();

                    lv_hetero.Items.Clear();

                    foreach (var file in ofd.FileNames)
                    {
                        ListViewItem lvi = new ListViewItem(new string[] { Path.GetFileName(file), "Queued." });
                        lvi.Name = file;

                        lv_hetero.Items.Add(lvi);
                    }

                    lv_hetero.EndUpdate();
                }
            };

            btn_hetero_start.Click += (s, e) =>
            {
                if (!m_hetero_worker.IsBusy)
                {
                    lv_hetero.Focus();
                    m_hetero_worker.RunWorkerAsync();

                    btn_hetero_start.Enabled = false;
                    tb_hetero_select_files.Enabled = false;
                }
            };
        }

        private void write2SQLite()
        {
            int itemCount = 0;
            StringBuilder sb = new StringBuilder();

            m_hetero_statistic_sql = new HeteroDeployStatisticSQLiteUtils();

            sb.Append("INSERT INTO HeteroSimulationResults(file_name, tunneling, marking, filtering, attacker, victim, packets, k, n, deploy_name, count_of_deploy, count_of_tunneling)");

            foreach (var item in m_hetero_simulation_result_list)
            {
                if (itemCount != 0 && itemCount % 499 == 0)
                {
                    m_hetero_statistic_sql.RunCommnad(sb.ToString().Remove(sb.ToString().Length - 6, 6));
                    sb.Clear();
                    sb.Append("INSERT INTO HeteroSimulationResults(file_name, tunneling, marking, filtering, attacker, victim, packets, k, n, deploy_name, count_of_deploy, count_of_tunneling)");
                    sb.AppendFormat(" SELECT '{0}',{1},{2},{3},{4},{5},{6},{7},{8},'{9}',{10},{11} UNION", item.file_name, item.tunneling, item.marking, item.filtering, item.attacker, item.victim, item.packets, item.k, item.n, item.deploy_name, item.count_of_deploy, item.count_of_tunneling);
                }
                else
                    sb.AppendFormat(" SELECT '{0}',{1},{2},{3},{4},{5},{6},{7},{8},'{9}',{10},{11} UNION", item.file_name, item.tunneling, item.marking, item.filtering, item.attacker, item.victim, item.packets, item.k, item.n, item.deploy_name, item.count_of_deploy, item.count_of_tunneling);
                itemCount++;
            }

            if (itemCount % 499 != 0)
                m_hetero_statistic_sql.RunCommnad(sb.ToString().Remove(sb.ToString().Length - 6, 6));
        }

        private class QuerySQLiteUtils : SQLiteUtils
        {
            public QuerySQLiteUtils(string existFullFilePath) : base(existFullFilePath) { }

            public override void CreateTable()
            {
                //throw new NotImplementedException();
            }
        }

        private class DeployStatisticSQLiteUtils : SQLiteUtils
        {
            private Dictionary<string, string> m_tables = new Dictionary<string, string>()
            {
                {"CountOfAllLevel", "d_id INTEGER PRIMARY KEY AUTOINCREMENT, file_name TEXT, node_counts INTEGER, edge_counts INTEGER, diameter INTEGER, k INTEGER, n INTEGER, deploy_name TEXT, level INTEGER, deploy_type TEXT, count_of_nodes INTEGER"},
                {"CountOfNonLevel", "d_id INTEGER PRIMARY KEY AUTOINCREMENT, file_name TEXT, node_counts INTEGER, edge_counts INTEGER, diameter INTEGER, k INTEGER, n INTEGER, deploy_name TEXT, deploy_type TEXT, count_of_nodes INTEGER"},
                {"CountOfAllLevelWithMaxN", "d_id INTEGER PRIMARY KEY AUTOINCREMENT, file_name TEXT, node_counts INTEGER, edge_counts INTEGER, diameter INTEGER, k INTEGER, n INTEGER, deploy_name TEXT, level INTEGER, deploy_type TEXT, count_of_nodes INTEGER"},
                {"CountOfNonLevelWithMaxN", "d_id INTEGER PRIMARY KEY AUTOINCREMENT, file_name TEXT, node_counts INTEGER, edge_counts INTEGER, diameter INTEGER, k INTEGER, n INTEGER, deploy_name TEXT, deploy_type TEXT, count_of_nodes INTEGER"}
            };

            public DeployStatisticSQLiteUtils()
                : base(Path.Combine(Environment.CurrentDirectory, "Deploy"), "DeployStatistic", false)
            {
                CreateTable();
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

                        foreach (var kvp in m_tables)
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

        private class HeteroDeployStatisticSQLiteUtils : SQLiteUtils
        {
            private Dictionary<string, string> m_tables = new Dictionary<string, string>()
            {
                {"HeteroSimulationResults", "h_id INTEGER PRIMARY KEY AUTOINCREMENT, file_name TEXT, tunneling INTEGER, marking INTEGER, filtering INTEGER, attacker INTEGER, victim INTEGER, packets INTEGER, k INTEGER, n INTEGER, deploy_name TEXT, count_of_deploy INTEGER, count_of_tunneling INTEGER"}
            };

            public HeteroDeployStatisticSQLiteUtils()
                : base(Path.Combine(Environment.CurrentDirectory, "Deploy"), "HeteroStatistic", false)
            {
                CreateTable();
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

                        foreach (var kvp in m_tables)
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

        private class HeteroSimulationResults
        {
            public string file_name { get; set; }
            public int tunneling { get; set; }
            public int marking { get; set; }
            public int filtering { get; set; }
            public int attacker { get; set; }
            public int victim { get; set; }
            public int packets { get; set; }
            public int k { get; set; }
            public int n { get; set; }
            public string deploy_name { get; set; }
            public int count_of_deploy { get; set; }
            public int count_of_tunneling { get; set; }
        }
    }
}
