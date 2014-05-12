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
        private List<string> m_deploy_file_names;
        private List<string> m_hetero_file_names;
        private List<string> m_merge_file_names;

        private List<string> m_deploy_names;

        private QuerySQLiteUtils m_query_sql;
        private DeployStatisticSQLiteUtils m_deploy_sql;
        private HeteroDeployStatisticSQLiteUtils m_hetero_sql;
        private Deployment.DeploySQLiteUtility m_merge_sql;

        private BackgroundWorker m_deploy_worker;
        private BackgroundWorker m_hetero_worker;
        private BackgroundWorker m_merge_worker;

        public DeployStatisticTool()
        {
            InitializeComponent();

            m_deploy_file_names = new List<string>();
            m_hetero_file_names = new List<string>();
            m_merge_file_names = new List<string>();

            m_deploy_names = new List<string>();
            m_deploy_names.Add(typeof(KCutStartWithCenterNode).Name);
            m_deploy_names.Add(typeof(KCutStartWithCenterNodeConsiderCoefficient).Name);
            m_deploy_names.Add(typeof(KCutStartWithComparableConsiderCoefficient).Name);
            m_deploy_names.Add(typeof(KCutStartWithConsider2KConsiderCoefficient).Name);
            m_deploy_names.Add(typeof(KCutStartWithSideNode).Name);
            m_deploy_names.Add(typeof(KCutStartWithSideNodeConcentrateDegree).Name);
            m_deploy_names.Add(typeof(KCutStartWithSideNodeConsiderCoefficient).Name);
            m_deploy_names.Add(typeof(KCutStartWithSideNodeConsiderScopeCoefficient).Name);
            m_deploy_names.Add(typeof(KCutStartWithSideNodeConsiderScopeCoefficientMinDegree).Name);

            m_deploy_worker = new BackgroundWorker();
            m_deploy_worker.WorkerSupportsCancellation = true;
            m_deploy_worker.WorkerReportsProgress = true;

            m_hetero_worker = new BackgroundWorker();
            m_hetero_worker.WorkerSupportsCancellation = true;
            m_hetero_worker.WorkerReportsProgress = true;

            m_merge_worker = new BackgroundWorker();
            m_merge_worker.WorkerSupportsCancellation = true;
            m_merge_worker.WorkerReportsProgress = true;

            deployEventRegist();
            heteroEventRegist();
            mergeEventRegist();

            this.FormClosing += (s, e) =>
            {
                if (m_deploy_worker.IsBusy)
                    m_deploy_worker.CancelAsync();

                if (m_hetero_worker.IsBusy)
                    m_hetero_worker.CancelAsync();

                if (m_merge_worker.IsBusy)
                    m_merge_worker.CancelAsync();
            };
        }

        private void deployEventRegist()
        {
            m_deploy_worker.DoWork += (s, e) =>
            {
                double count_of_total_runs = m_deploy_file_names.Count * 2 + 2;
                double now_runs = 0;
                string cmd;
                DataView dv;
                m_deploy_sql = new DeployStatisticSQLiteUtils();
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

                    for (int i = 0; dv != null && i < dv.Count; i++)
                    {
                        if (itemCount != 0 && itemCount % 499 == 0)
                        {
                            m_deploy_sql.RunCommnad(sb.ToString().Remove(sb.ToString().Length - 6, 6));
                            sb.Clear();
                            sb.Append("INSERT INTO CountOfAllLevel(file_name, node_counts, edge_counts, diameter, k, n, deploy_name, level, deploy_type, count_of_nodes)");
                            sb.AppendFormat(" SELECT '{0}',{1},{2},{3},{4},{5},'{6}',{7},'{8}',{9} UNION", dv[i]["file_name"], dv[i]["node_counts"], dv[i]["edge_counts"], dv[i]["diameter"], dv[i]["k"], dv[i]["n"], dv[i]["deploy_name"], dv[i]["level"], dv[i]["deploy_type"], dv[i]["count_of_nodes"]);
                        }
                        else
                            sb.AppendFormat(" SELECT '{0}',{1},{2},{3},{4},{5},'{6}',{7},'{8}',{9} UNION", dv[i]["file_name"], dv[i]["node_counts"], dv[i]["edge_counts"], dv[i]["diameter"], dv[i]["k"], dv[i]["n"], dv[i]["deploy_name"], dv[i]["level"], dv[i]["deploy_type"], dv[i]["count_of_nodes"]);
                        itemCount++;
                    }

                    if (itemCount % 499 != 0)
                        m_deploy_sql.RunCommnad(sb.ToString().Remove(sb.ToString().Length - 6, 6));

                    // Count of Non-Level.
                    m_deploy_worker.ReportProgress((int)(++now_runs / count_of_total_runs * 100.0), new object[] { filePath, "Processing (2/2)..." });
                    cmd = "SELECT file_name, node_counts, edge_counts, diameter, k, n, deploy_name, deploy_type, COUNT(*) AS count_of_nodes FROM (NetworkTopology INNER JOIN DeploySimulation ON NetworkTopology.n_id = DeploySimulation.n_id) INNER JOIN LevelRecord ON DeploySimulation.job_id = LevelRecord.job_id GROUP BY file_name, node_counts, diameter, k, n, deploy_name, deploy_type;";
                    dv = m_query_sql.GetResult(cmd);

                    itemCount = 0;
                    sb.Clear();
                    sb.Append("INSERT INTO CountOfNonLevel(file_name, node_counts, edge_counts, diameter, k, n, deploy_name, deploy_type, count_of_nodes)");

                    for (int i = 0; dv != null && i < dv.Count; i++)
                    {
                        if (itemCount != 0 && itemCount % 499 == 0)
                        {
                            m_deploy_sql.RunCommnad(sb.ToString().Remove(sb.ToString().Length - 6, 6));
                            sb.Clear();
                            sb.Append("INSERT INTO CountOfNonLevel(file_name, node_counts, edge_counts, diameter, k, n, deploy_name, deploy_type, count_of_nodes)");
                            sb.AppendFormat(" SELECT '{0}',{1},{2},{3},{4},{5},'{6}','{7}',{8} UNION", dv[i]["file_name"], dv[i]["node_counts"], dv[i]["edge_counts"], dv[i]["diameter"], dv[i]["k"], dv[i]["n"], dv[i]["deploy_name"], dv[i]["deploy_type"], dv[i]["count_of_nodes"]);
                        }
                        else
                            sb.AppendFormat(" SELECT '{0}',{1},{2},{3},{4},{5},'{6}','{7}',{8} UNION", dv[i]["file_name"], dv[i]["node_counts"], dv[i]["edge_counts"], dv[i]["diameter"], dv[i]["k"], dv[i]["n"], dv[i]["deploy_name"], dv[i]["deploy_type"], dv[i]["count_of_nodes"]);
                        itemCount++;
                    }

                    if (itemCount % 499 != 0)
                        m_deploy_sql.RunCommnad(sb.ToString().Remove(sb.ToString().Length - 6, 6));

                    m_deploy_worker.ReportProgress((int)(++now_runs / count_of_total_runs * 100.0), new object[] { filePath, "Completed (2/2)." });
                }

                // Count of All Level with Max N.
                cmd = "INSERT INTO CountOfAllLevelWithMaxN(file_name, node_counts, edge_counts, diameter, k, n, deploy_name, level, deploy_type, count_of_nodes) SELECT c.file_name, c.node_counts, c.edge_counts, c.diameter, c.k, c.n,c.deploy_name, c.level, c.deploy_type, c.count_of_nodes FROM (SELECT file_name, node_counts, edge_counts, diameter, k, MAX(n) AS max_n FROM CountOfAllLevel GROUP BY file_name, node_counts, edge_counts, diameter, k) AS m JOIN CountOfAllLevel AS c ON m.file_name=c.file_name and m.node_counts=c.node_counts and m.edge_counts=c.edge_counts and m.diameter=c.diameter and m.k=c.k and m.max_n=c.n;";
                m_deploy_sql.RunCommnad(cmd);

                // Count of Non-Level with Max N.
                m_deploy_worker.ReportProgress((int)(++now_runs / count_of_total_runs * 100.0));
                cmd = "INSERT INTO CountOfNonLevelWithMaxN(file_name, node_counts, edge_counts, diameter, k, n, deploy_name, deploy_type, count_of_nodes) SELECT c.file_name, c.node_counts, c.edge_counts, c.diameter, c.k, c.n,c.deploy_name, c.deploy_type, c.count_of_nodes FROM (SELECT file_name, node_counts, edge_counts, diameter, k, MAX(n) AS max_n FROM CountOfNonLevel GROUP BY file_name, node_counts, edge_counts, diameter, k) AS m JOIN CountOfNonLevel AS c ON m.file_name=c.file_name and m.node_counts=c.node_counts and m.edge_counts=c.edge_counts and m.diameter=c.diameter and m.k=c.k and m.max_n=c.n;";
                m_deploy_sql.RunCommnad(cmd);

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
                    //lv_deploy.TopItem = lv_deploy.Items[file];
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
                int itemCount = 0;
                int k, n, countOfDeploy, countOfTunneling;
                int tunneling, marking, filtering, attacker, victim, packets;
                string fileName;
                m_hetero_sql = new HeteroDeployStatisticSQLiteUtils();
                StringBuilder sb = new StringBuilder();

                sb.Append("INSERT INTO HeteroSimulationResults(file_name, tunneling, marking, filtering, attacker, victim, packets, k, n, deploy_name, count_of_deploy, count_of_tunneling)");

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

                            if (itemCount != 0 && itemCount % 499 == 0)
                            {
                                m_hetero_sql.RunCommnad(sb.ToString().Remove(sb.ToString().Length - 6, 6));
                                sb.Clear();
                                sb.Append("INSERT INTO HeteroSimulationResults(file_name, tunneling, marking, filtering, attacker, victim, packets, k, n, deploy_name, count_of_deploy, count_of_tunneling)");
                                sb.AppendFormat(" SELECT '{0}',{1},{2},{3},{4},{5},{6},{7},{8},'{9}',{10},{11} UNION", fileName, tunneling, marking, filtering, attacker, victim, packets, k, n, deploy, countOfDeploy, countOfTunneling);
                            }
                            else
                                sb.AppendFormat(" SELECT '{0}',{1},{2},{3},{4},{5},{6},{7},{8},'{9}',{10},{11} UNION", fileName, tunneling, marking, filtering, attacker, victim, packets, k, n, deploy, countOfDeploy, countOfTunneling);

                            itemCount++;

                            DataUtility.Log(string.Format("k={0}, n={1}, d={2}, t={3}, d_name={4}\n\n", k, n, countOfDeploy, countOfTunneling, deploy));
                        }

                        m_hetero_worker.ReportProgress((int)(++now_runs / count_of_total_runs * 100.0));
                    }
                    m_hetero_worker.ReportProgress((int)(now_runs / count_of_total_runs * 100.0), new object[] { filePath, "Completed." });
                }

                if (itemCount % 499 != 0)
                    m_hetero_sql.RunCommnad(sb.ToString().Remove(sb.ToString().Length - 6, 6));
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
                    //lv_hetero.TopItem = lv_hetero.Items[file];
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

        private void mergeEventRegist()
        {
            m_merge_worker.DoWork += (s, e) =>
            {
                double count_of_total_runs = m_merge_file_names.Count + 1;
                double now_runs = 0;
                DataView dv_network, dv_deploy, dv_level;
                StringBuilder sb_level = new StringBuilder();
                int levelCount = 0;
                long new_job_id;
                List<string> buff_list = new List<string>();

                m_merge_sql = new Deployment.DeploySQLiteUtility(string.Format("{0}.merge", Path.GetFileNameWithoutExtension(m_merge_file_names[0])));
                m_merge_sql.CreateTable();

                sb_level.Append("INSERT INTO LevelRecord(job_id, level, node_id, deploy_type)");
                
                foreach (string filePath in m_merge_file_names)
                {
                    m_query_sql = new QuerySQLiteUtils(filePath);

                    m_merge_worker.ReportProgress((int)(now_runs / count_of_total_runs * 100.0), new object[] { filePath, "Processing..." });

                    // Query NetworkTopology.
                    dv_network = m_query_sql.GetResult("SELECT * FROM NetworkTopology;");
                    for (int i = 0; dv_network != null && i < dv_network.Count; i++)
                    {
                        buff_list.Add(string.Format("INSERT INTO NetworkTopology(file_name, node_counts, edge_counts, diameter) VALUES('{0}',{1},{2},{3});", dv_network[i]["file_name"], dv_network[i]["node_counts"], dv_network[i]["edge_counts"], dv_network[i]["diameter"]));

                        // Query DeploySimulation.
                        dv_deploy = m_query_sql.GetResult("SELECT job_id, k, n, deploy_name FROM DeploySimulation WHERE n_id = @n_id;",
                                                            new List<SQLiteParameter>() { new SQLiteParameter("@n_id", dv_network[i]["n_id"]) });
                        for (int j = 0; dv_deploy != null && j < dv_deploy.Count; j++)
                        {
                            new_job_id = DateTime.Now.Subtract(new DateTime(1970, 1, 1)).Ticks;
                            buff_list.Add(string.Format("INSERT INTO DeploySimulation(job_id, n_id, k, n, deploy_name) SELECT {0}, n_id, {2}, {3}, '{4}' FROM NetworkTopology WHERE file_name LIKE '{1}';", new_job_id, dv_network[i]["file_name"], dv_deploy[j]["k"], dv_deploy[j]["n"], dv_deploy[j]["deploy_name"]));

                            // Query LevelRecord.
                            dv_level = m_query_sql.GetResult("SELECT level, node_id, deploy_type FROM LevelRecord WHERE job_id = @job_id;",
                                                                new List<SQLiteParameter>() { new SQLiteParameter("@job_id", dv_deploy[j]["job_id"]) });
                            for (int k = 0; dv_level != null && k < dv_level.Count; k++)
                            {
                                if (levelCount != 0 && levelCount % 499 == 0)
                                {
                                    buff_list.Add(sb_level.ToString().Remove(sb_level.ToString().Length - 6, 6));
                                    levelCount = 0;
                                    if (buff_list.Count >= 5000)
                                    {
                                        m_merge_sql.BulkInsertCommands(buff_list);
                                        buff_list.Clear();
                                    }
                                    sb_level.Clear();
                                    sb_level.Append("INSERT INTO LevelRecord(job_id, level, node_id, deploy_type)");
                                    sb_level.AppendFormat(" SELECT {0},{1},{2},'{3}' UNION", new_job_id, dv_level[k]["level"], dv_level[k]["node_id"], dv_level[k]["deploy_type"]);
                                }
                                else
                                    sb_level.AppendFormat(" SELECT {0},{1},{2},'{3}' UNION", new_job_id, dv_level[k]["level"], dv_level[k]["node_id"], dv_level[k]["deploy_type"]);
                                levelCount++;
                            }
                        }
                    }
                    m_merge_worker.ReportProgress((int)(++now_runs / count_of_total_runs * 100.0), new object[] { filePath, "Completed." });
                }

                if (levelCount % 499 != 0)
                    buff_list.Add(sb_level.ToString().Remove(sb_level.ToString().Length - 6, 6));

                if (buff_list.Count > 0)
                    m_merge_sql.BulkInsertCommands(buff_list);

                m_merge_worker.ReportProgress((int)(++now_runs / count_of_total_runs * 100.0));
            };

            m_merge_worker.ProgressChanged += (s, e) =>
            {
                if (e.UserState != null)
                {
                    object[] args = e.UserState as object[];
                    string file = args[0] as string;
                    string status = args[1] as string;

                    lv_merge.Items[file].SubItems[1].Text = status;

                    foreach (int index in lv_merge.SelectedIndices)
                        lv_merge.Items[index].Selected = false;

                    lv_merge.Items[file].Selected = true;
                    lv_merge.Items[file].Focused = true;
                    //lv_merge.TopItem = lv_merge.Items[file];
                }

                pb_merge.Value = e.ProgressPercentage;
            };

            m_merge_worker.RunWorkerCompleted += (s, e) =>
            {
                btn_merge.Enabled = true;
                tb_merge.Enabled = true;
            };

            tb_merge.Click += (s, e) =>
            {
                OpenFileDialog ofd = new OpenFileDialog();

                ofd.Filter = "Database Files|*.db";
                ofd.Multiselect = true;
                ofd.Title = "Select Deploy Simulation Database to Merge...";
                ofd.InitialDirectory = Path.Combine(Environment.CurrentDirectory, "Deploy");

                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    m_merge_file_names = ofd.FileNames.ToList();

                    lv_merge.BeginUpdate();

                    lv_merge.Items.Clear();

                    foreach (var file in ofd.FileNames)
                    {
                        ListViewItem lvi = new ListViewItem(new string[] { Path.GetFileName(file), "Queued." });
                        lvi.Name = file;

                        lv_merge.Items.Add(lvi);
                    }

                    lv_merge.EndUpdate();
                }
            };

            btn_merge.Click += (s, e) =>
            {
                if (!m_merge_worker.IsBusy)
                {
                    lv_merge.Focus();
                    m_merge_worker.RunWorkerAsync();

                    btn_merge.Enabled = false;
                    tb_merge.Enabled = false;
                }
            };
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
                {"CountOfAllLevel", "file_name TEXT, node_counts INTEGER, edge_counts INTEGER, diameter INTEGER, k INTEGER, n INTEGER, deploy_name TEXT, level INTEGER, deploy_type TEXT, count_of_nodes INTEGER"},
                {"CountOfNonLevel", "file_name TEXT, node_counts INTEGER, edge_counts INTEGER, diameter INTEGER, k INTEGER, n INTEGER, deploy_name TEXT, deploy_type TEXT, count_of_nodes INTEGER"},
                {"CountOfAllLevelWithMaxN", "file_name TEXT, node_counts INTEGER, edge_counts INTEGER, diameter INTEGER, k INTEGER, n INTEGER, deploy_name TEXT, level INTEGER, deploy_type TEXT, count_of_nodes INTEGER"},
                {"CountOfNonLevelWithMaxN", "file_name TEXT, node_counts INTEGER, edge_counts INTEGER, diameter INTEGER, k INTEGER, n INTEGER, deploy_name TEXT, deploy_type TEXT, count_of_nodes INTEGER"}
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
    }
}
