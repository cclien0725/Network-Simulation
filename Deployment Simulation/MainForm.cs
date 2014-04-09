using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Network_Simulation;
using System.Runtime.InteropServices;
using System.IO;

namespace Deployment_Simulation
{
    public partial class MainForm : Form
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        private NetworkTopology m_topo;
        private BackgroundWorker m_simulation_worker;
        private BackgroundWorker m_data_worker;

        public MainForm()
        {
            InitializeComponent();

            AllocConsole();

            m_topo = new NetworkTopology(10, 10, 1);

            m_simulation_worker = new BackgroundWorker();
            m_simulation_worker.WorkerSupportsCancellation = true;
            m_simulation_worker.WorkerReportsProgress = true;

            m_data_worker = new BackgroundWorker();
            m_data_worker.WorkerSupportsCancellation = true;
            m_data_worker.WorkerReportsProgress = true;

            eventRegist();
        }

        private void eventRegist()
        {
            m_simulation_worker.DoWork += (s, e) =>
            {
                string[] files = tb_select_file.Text.Split(';');
                int K, N;

                for (int i = 0; i < files.Length; i++)
                {
                    m_simulation_worker.ReportProgress((int)(((double)(i) / (double)files.Count()) * 100.0),
                                                new object[] { true, string.Format("Running {0}...", Path.GetFileNameWithoutExtension(files[i])) });

                    if (rdoBtn_specific.Checked)
                    {
                        K = int.Parse(tb_k.Text);
                        N = int.Parse(tb_n.Text);

                        m_simulation_worker.ReportProgress(0,
                                                new object[] { false, "Preprocessing file...", true, files[i], K, N });

                        m_topo.ReadBriteFile(files[i]);

                        m_simulation_worker.ReportProgress(33,
                                                new object[] { false, string.Format("Starting Deployment with K: {0}, N: {1}...", K, N), true, files[i], K, N });

                        // Using kcutwithclustering depolyment method.
                        KCutWithClusteringDeployment deployment = new KCutWithClusteringDeployment(30, 20, 10, K, N);
                        deployment.Deploy(m_topo);

                        m_simulation_worker.ReportProgress(100,
                                                new object[] { false, string.Format("Completed for K: {0}, N: {1}.", K, N), true, files[i], K, N });
                    }
                    else
                    {
                        m_simulation_worker.ReportProgress(0,
                                                new object[] { false, "Preprocessing file...", true, files[i], 0, 0 });

                        m_topo.ReadBriteFile(files[i]);

                        Deployment deployment = null;
                        List<int> last_deploy_count;
                        int satisfy_count;

                        for (K = 1; K <= m_topo.Diameter; K++)
                        {
                            N = 0;
                            satisfy_count = 0;

                            do
                            {
                                if (deployment != null)
                                    last_deploy_count = new List<int>(deployment.DeployNodes);
                                else
                                    last_deploy_count = new List<int>();

                                deployment = new KCutWithClusteringCompareCenterAndMinDegreeDeployment(30, 20, 10, K, ++N);

                                m_simulation_worker.ReportProgress((int)((double)K / (double)m_topo.Diameter * 100),
                                                    new object[] { false, string.Format("Starting Deployment with K: {0}, N: {1}...", K, N), true, files[i], K, N });

                                deployment.Deploy(m_topo);

                                m_simulation_worker.ReportProgress((int)((double)K / (double)m_topo.Diameter * 100),
                                                   new object[] { false, string.Format("Completed for K: {0}, N: {1}.", K, N), true, files[i], K, N });

                                if (deployment.DeployNodes.Except(last_deploy_count).Count() == 0 && last_deploy_count.Except(deployment.DeployNodes).Count() == 0)
                                {
                                    satisfy_count++;
                                }
                                else
                                {
                                    satisfy_count = 0;
                                }
                            } while (satisfy_count < 2);
                        }
                    }

                    m_simulation_worker.ReportProgress((int)(((double)(i + 1) / (double)files.Count()) * 100.0),
                                                new object[] { true, string.Format("Running {0}...", Path.GetFileNameWithoutExtension(files[i])) });
                }
            };

            m_simulation_worker.ProgressChanged += (s, e) =>
            {
                object[] obj = e.UserState as object[];

                if (obj != null)
                {
                    bool isMainProgress = (bool)obj[0];
                    string status = (string)obj[1];

                    if (isMainProgress)
                    {
                        progress_main.Value = e.ProgressPercentage;
                        lb_main_progress.Text = string.Format("Main Progress: {0}", status);
                    }
                    else
                    {
                        bool isNeedRefreshList = (bool)obj[2];

                        progress_sub.Value = e.ProgressPercentage;
                        lb_sub_progress.Text = string.Format("Sub Progress: {0}", status);

                        if (isNeedRefreshList)
                        {
                            string fileName = (string)obj[3];
                            int k = (int)obj[4];
                            int n = (int)obj[5];

                            lv_list.Items[fileName].SubItems[1].Text = k.ToString();
                            lv_list.Items[fileName].SubItems[2].Text = n.ToString();
                            lv_list.Items[fileName].SubItems[3].Text = status;
                        }
                    }
                }
            };

            m_simulation_worker.RunWorkerCompleted += (s, e) =>
            {
                btn_run.Enabled = true;
                groupBox2.Enabled = true;

                if (rdoBtn_specific.Checked)
                {
                    tb_k.Enabled = true;
                    tb_n.Enabled = true;
                }

                tb_select_file.Enabled = true;

                lb_main_progress.Text = "Main Progress: Completed.";
                lb_sub_progress.Text = "Sub Progress: Completed.";
            };

            m_data_worker.DoWork += (s, e) =>
            {
                object[] data = e.Argument as object[];

                if (data != null)
                {
                    bool isQuery = (bool)data[0];
                    string sqlCmd;
                    KCutWithClusteringDeployment.DeploySQLiteUtility utilit = new Deployment.DeploySQLiteUtility("deploy_simulation");

                    if (isQuery)
                    {
                        sqlCmd = (string)data[1];

                        DataView dv = utilit.GetResult(sqlCmd, null);

                        e.Result = new object[] { true, dv };
                    }
                    else
                    {
                        string filePath = (string)data[1];
                        sqlCmd = "SELECT k,n,deploy_type,file_name,node_counts,level,edge_counts,diameter,job_id,deploy_name,node_id FROM (SELECT * FROM NetworkTopology AS N JOIN DeploySimulation AS D on N.n_id=D.n_id JOIN LevelRecord AS L on D.job_id=L.job_id ) AS Data";

                        DataView dv = utilit.GetResult(sqlCmd, null);

                        if (dv != null && dv.Count > 0)
                        {
                            DataUtility.ExcelExport(dv.Table, filePath, false);
                            e.Result = data;
                        }
                    }
                }
            };

            m_data_worker.RunWorkerCompleted += (s, e) =>
            {
                object[] data = e.Result as object[];

                if (data != null)
                {
                    bool isQuery = (bool)data[0];

                    if (isQuery)
                    {
                        DataView dv = data[1] as DataView;
                        if (dv != null && dv.Count > 0)
                            dataGridView1.DataSource = dv;
                        btn_query.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show(this, string.Format("The file: {0} has been exported.", Path.GetFileName((string)data[1])), "Export Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        btn_export.Enabled = true;
                    }
                    Cursor = Cursors.Arrow;
                }
            };

            lv_list.MouseDoubleClick += (s, e) =>
            {
                if (lv_list.SelectedIndices.Count > 0)
                {
                    ListViewItem item = lv_list.Items[lv_list.SelectedIndices[0]];

                    if (item.SubItems[3].Text.Contains("Completed"))
                    {
                        NetworkViewForm view = new NetworkViewForm(item.Name);
                        view.Show(this);
                    }
                }
            };

            tb_select_file.Click += (s, e) =>
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = true;
                ofd.InitialDirectory = Environment.CurrentDirectory;
                ofd.Filter = "Birte File|*.brite";
                ofd.Title = "Select Brite Files";
                ofd.FileName = "";

                if (ofd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    tb_select_file.Clear();

                    lv_list.BeginUpdate();
                    lv_list.Items.Clear();

                    foreach (string file in ofd.FileNames)
                    {
                        tb_select_file.Text += file + ";";

                        ListViewItem lvi = new ListViewItem(new string[] { Path.GetFileNameWithoutExtension(file), "0", "0", "Queued." });
                        lvi.Name = file;

                        lv_list.Items.Add(lvi);
                    }

                    tb_select_file.Text = tb_select_file.Text.TrimEnd(';');

                    lv_list.EndUpdate();
                }
            };

            btn_run.Click += (s, e) =>
            {
                m_simulation_worker.RunWorkerAsync();
                btn_run.Enabled = false;
                groupBox2.Enabled = false;

                if (rdoBtn_specific.Checked)
                {
                    tb_k.Enabled = false;
                    tb_n.Enabled = false;
                }

                tb_select_file.Enabled = false;
            };

            btn_query.Click += (s, e) =>
            {
                btn_query.Enabled = false;
                Cursor = Cursors.WaitCursor;
                m_data_worker.RunWorkerAsync(new object[] { true, tb_sql_cmd.Text });
            };

            btn_export.Click += (s, e) =>
            {
                SaveFileDialog sfd = new SaveFileDialog();

                sfd.InitialDirectory = Environment.CurrentDirectory;
                sfd.OverwritePrompt = true;
                sfd.Title = "Export to";
                sfd.Filter = "Excel Files|*.xls";
                sfd.FileName = "";

                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    btn_export.Enabled = false;
                    Cursor = Cursors.WaitCursor;
                    m_data_worker.RunWorkerAsync(new object[] { false, sfd.FileName });
                }
            };

            Load += (s, e) =>
            {
                btn_query.PerformClick();
            };

            tb_select_file.TextChanged += new EventHandler(checkValid);
            tb_k.TextChanged += new EventHandler(checkValid);
            tb_n.TextChanged += new EventHandler(checkValid);

            tb_k.KeyPress += new KeyPressEventHandler(checkInputValue);
            tb_n.KeyPress += new KeyPressEventHandler(checkInputValue);

            rdoBtn_specific.CheckedChanged += new EventHandler(rdoBtn_CheckedChanged);
            rdoBtn_all.CheckedChanged += new EventHandler(rdoBtn_CheckedChanged);
        }

        private void rdoBtn_CheckedChanged(object sender, EventArgs e)
        {
            bool isSpecific = sender.Equals(rdoBtn_specific);

            tb_k.Enabled = isSpecific;
            tb_n.Enabled = isSpecific;

            checkValid(sender, e);
        }

        private void checkInputValue(object sender, KeyPressEventArgs e)
        {
            int tmp;

            e.Handled = !int.TryParse((sender as TextBox).Text + e.KeyChar, out tmp) && e.KeyChar != (char)Keys.Back;
        }

        private void checkValid(object sender, EventArgs e)
        {
            int tmp;

            btn_run.Enabled = !string.IsNullOrEmpty(tb_select_file.Text) && 
                                (
                                    tb_k.Enabled == false && tb_n.Enabled == false ||
                                    int.TryParse(tb_k.Text, out tmp) && int.TryParse(tb_n.Text, out tmp)
                                );
        }
    }
}
