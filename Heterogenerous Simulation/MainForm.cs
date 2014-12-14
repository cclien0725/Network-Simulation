using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Network_Simulation;
using System.IO;
using Deployment_Simulation;

namespace Heterogenerous_Simulation
{
    public partial class MainForm : Form
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        protected List<string> filenames;
        protected int postfixIndex;
        protected BackgroundWorker bg;

        protected enum StatusType { TotalProgress, NoneDeploymentStatus, RandomDeploymentStatus, KCutDeploymentStatus, KCut2DeploymentStatus }
        protected class ProgressReportArg 
        {
            public string KEY { get; set; }
            public StatusType ST { get; set; }
        }

        private List<Type> m_deploy_types;

        public MainForm()
        {
            InitializeComponent();

            m_deploy_types = new List<Type>();
            //m_deploy_types.Add(typeof(KCutStartWithCenterNode));
            //m_deploy_types.Add(typeof(KCutStartWithCenterNodeConsiderCoefficient));
            //m_deploy_types.Add(typeof(KCutStartWithComparableConsiderCoefficient));
            //m_deploy_types.Add(typeof(KCutStartWithConsider2KConsiderCoefficient));
            m_deploy_types.Add(typeof(KCutStartWithSideNode));
            m_deploy_types.Add(typeof(KCutStartWithSideNodeConcentrateDegree));
            m_deploy_types.Add(typeof(KCutStartWithSideNodeConsiderCoefficient));
            m_deploy_types.Add(typeof(KCutStartWithSideNodeCoefficientAndMinDegree));
            m_deploy_types.Add(typeof(KCutStartWithSideMinDegreeAndCoefficient));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            filenames = new List<string>();

            bg = new BackgroundWorker();
            bg.WorkerReportsProgress = true;
            bg.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bg_RunWorkerCompleted);
            bg.DoWork += new DoWorkEventHandler(bg_DoWork);
            bg.ProgressChanged += new ProgressChangedEventHandler(bg_ProgressChanged);
        }

        void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            groupBox1.Enabled = true;
            groupBox2.Enabled = true;
            groupBox3.Enabled = true;
            button1.Enabled = true;
        }

        void bg_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressReportArg pra = e.UserState as ProgressReportArg;
            switch (pra.ST)
            {
                case StatusType.TotalProgress:
                    progressBar1.Value = e.ProgressPercentage;
                    break;
                case StatusType.NoneDeploymentStatus:
                    if (listView1.Items[pra.KEY].SubItems[1].Text != e.ProgressPercentage.ToString() + "%")
                        listView1.Items[pra.KEY].SubItems[1].Text = e.ProgressPercentage.ToString() + "%";
                    break;
                case StatusType.RandomDeploymentStatus:
                    if (listView1.Items[pra.KEY].SubItems[2].Text != e.ProgressPercentage.ToString() + "%")
                        listView1.Items[pra.KEY].SubItems[2].Text = e.ProgressPercentage.ToString() + "%";
                    break;
                case StatusType.KCutDeploymentStatus:
                    if (listView1.Items[pra.KEY].SubItems[3].Text != e.ProgressPercentage.ToString() + "%")
                        listView1.Items[pra.KEY].SubItems[3].Text = e.ProgressPercentage.ToString() + "%";
                    break;
                case StatusType.KCut2DeploymentStatus:
                    if (listView1.Items[pra.KEY].SubItems[4].Text != e.ProgressPercentage.ToString() + "%")
                        listView1.Items[pra.KEY].SubItems[4].Text = e.ProgressPercentage.ToString() + "%";
                    break;
            }
        }

        void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            //try
            //{
                foreach (string filename in filenames)
                {
                    string dbName = string.Format("{0}_T{1}M{2}F{3}_A{4}V{5}_Pkt{6}_{7}", Path.GetFileNameWithoutExtension(filename),
                                                                               TunnelingTracer.Text,
                                                                               MarkingTracer.Text,
                                                                               FilteringTracer.Text,
                                                                               AttackNodes.Text,
                                                                               VictimNodes.Text,
                                                                               TotalPacket.Text,
                                                                               PercentageOfAttackPacket.Text);

                    SQLiteUtility sql = new SQLiteUtility(ref dbName);
                    postfixIndex = Convert.ToInt32(Path.GetFileNameWithoutExtension(dbName).Split('_').Last());

                    // Read network topology and initialize the attackers, normal users and victim.
                    NetworkTopology networkTopology = new NetworkTopology(Convert.ToDouble(AttackNodes.Text), Convert.ToInt32(VictimNodes.Text));
                    networkTopology.ReadBriteFile(filename);

                    //// Doesn't use any deployment method.
                    NoneDeployment noneDeply = new NoneDeployment(0, 0, 0);
                    sql.CreateTable(noneDeply.ToString());
                    noneDeply.Deploy(networkTopology);
                    Simulator noneSimulator = new Simulator(noneDeply, networkTopology, sql, "None");
                    noneSimulator.onReportOccur += delegate(object obj, Simulator.ReportArgument ra)
                    {
                        bg.ReportProgress(ra.CurrentPacket * 100 / ra.TotalPacket, new ProgressReportArg() { KEY = filename, ST = StatusType.NoneDeploymentStatus });
                    };
                    noneSimulator.Run(Convert.ToInt32(AttackPacketPerSec.Text), Convert.ToInt32(NormalPacketPerSec.Text), Convert.ToInt32(TotalPacket.Text), Convert.ToInt32(PercentageOfAttackPacket.Text), Convert.ToDouble(ProbibilityOfPacketTunneling.Text), Convert.ToDouble(ProbibilityOfPacketMarking.Text), Convert.ToDouble(StartFiltering.Text), Convert.ToInt32(InitTimeOfAttackPacket.Text), DynamicProbability.Checked, ConsiderDistance.Checked);

                    bg.ReportProgress((filenames.IndexOf(filename) * 4 + 1) * 100 / (filenames.Count * 4), new ProgressReportArg() { ST = StatusType.TotalProgress });

                    //// Using randomly depolyment method.
                    RandomDeployment randomDeploy = new RandomDeployment(Convert.ToDouble(TunnelingTracer.Text) * Convert.ToDouble(PercentageOfTracer.Text) / 100, Convert.ToDouble(MarkingTracer.Text) * Convert.ToDouble(PercentageOfTracer.Text) / 100, Convert.ToDouble(FilteringTracer.Text) * Convert.ToDouble(PercentageOfTracer.Text) / 100);
                    sql.CreateTable(randomDeploy.ToString());
                    randomDeploy.Deploy(networkTopology);
                    Simulator randomSimulator = new Simulator(randomDeploy, networkTopology, sql, "Random");
                    randomSimulator.onReportOccur += delegate(object obj, Simulator.ReportArgument ra)
                    {
                        bg.ReportProgress(ra.CurrentPacket * 100 / ra.TotalPacket, new ProgressReportArg() { KEY = filename, ST = StatusType.RandomDeploymentStatus });
                    };
                    randomSimulator.Run(Convert.ToInt32(AttackPacketPerSec.Text), Convert.ToInt32(NormalPacketPerSec.Text), Convert.ToInt32(TotalPacket.Text), Convert.ToInt32(PercentageOfAttackPacket.Text), Convert.ToDouble(ProbibilityOfPacketTunneling.Text), Convert.ToDouble(ProbibilityOfPacketMarking.Text), Convert.ToDouble(StartFiltering.Text), Convert.ToInt32(InitTimeOfAttackPacket.Text), DynamicProbability.Checked, ConsiderDistance.Checked);

                    bg.ReportProgress((filenames.IndexOf(filename) * 4 + 2) * 100 / (filenames.Count * 4), new ProgressReportArg() { ST = StatusType.TotalProgress });

                    // Using KCut deployment method.
                    KCutDeployment kCutDeploy = new KCutDeployment(Convert.ToDouble(TunnelingTracer.Text) * Convert.ToDouble(PercentageOfTracer.Text) / 100, Convert.ToDouble(MarkingTracer.Text) * Convert.ToDouble(PercentageOfTracer.Text) / 100, Convert.ToDouble(FilteringTracer.Text) * Convert.ToDouble(PercentageOfTracer.Text) / 100, typeof(KCutStartWithSideNodeConsiderCoefficient));
                    sql.CreateTable("KCutDeployV1");
                    kCutDeploy.Deploy(networkTopology);
                    Simulator kCutSimulator = new Simulator(kCutDeploy.Deployment, networkTopology, sql, "V1");
                    kCutSimulator.onReportOccur += delegate(object obj, Simulator.ReportArgument ra)
                    {
                        bg.ReportProgress(ra.CurrentPacket * 100 / ra.TotalPacket, new ProgressReportArg() { KEY = filename, ST = StatusType.KCutDeploymentStatus });
                    };
                    kCutSimulator.Run(Convert.ToInt32(AttackPacketPerSec.Text), Convert.ToInt32(NormalPacketPerSec.Text), Convert.ToInt32(TotalPacket.Text), Convert.ToInt32(PercentageOfAttackPacket.Text), Convert.ToDouble(ProbibilityOfPacketTunneling.Text), Convert.ToDouble(ProbibilityOfPacketMarking.Text), Convert.ToDouble(StartFiltering.Text), Convert.ToInt32(InitTimeOfAttackPacket.Text), DynamicProbability.Checked, ConsiderDistance.Checked);

                    bg.ReportProgress((filenames.IndexOf(filename) * 4 + 3) * 100 / (filenames.Count * 4), new ProgressReportArg() { ST = StatusType.TotalProgress });

                    // Using KCutV2 deployment method.
                    KCutDeploymentV2 kCut2Deploy = new KCutDeploymentV2(Convert.ToDouble(TunnelingTracer.Text) * Convert.ToDouble(PercentageOfTracer.Text) / 100, Convert.ToDouble(MarkingTracer.Text) * Convert.ToDouble(PercentageOfTracer.Text) / 100, Convert.ToDouble(FilteringTracer.Text) * Convert.ToDouble(PercentageOfTracer.Text) / 100, typeof(KCutStartWithSideNodeConsiderCoefficient));
                    sql.CreateTable("KCutDeployV2");
                    kCut2Deploy.Deploy(networkTopology);
                    Simulator kCut2Simulator = new Simulator(kCut2Deploy.Deployment, networkTopology, sql, "V2");
                    kCut2Simulator.onReportOccur += delegate(object obj, Simulator.ReportArgument ra)
                    {
                        bg.ReportProgress(ra.CurrentPacket * 100 / ra.TotalPacket, new ProgressReportArg() { KEY = filename, ST = StatusType.KCut2DeploymentStatus });
                    };
                    kCut2Simulator.Run(Convert.ToInt32(AttackPacketPerSec.Text), Convert.ToInt32(NormalPacketPerSec.Text), Convert.ToInt32(TotalPacket.Text), Convert.ToInt32(PercentageOfAttackPacket.Text), Convert.ToDouble(ProbibilityOfPacketTunneling.Text), Convert.ToDouble(ProbibilityOfPacketMarking.Text), Convert.ToDouble(StartFiltering.Text), Convert.ToInt32(InitTimeOfAttackPacket.Text), DynamicProbability.Checked, ConsiderDistance.Checked);

                    bg.ReportProgress((filenames.IndexOf(filename) * 4 + 4) * 100 / (filenames.Count * 4), new ProgressReportArg() { ST = StatusType.TotalProgress });
                }

                File.WriteAllLines(Path.Combine(Environment.CurrentDirectory, "Log", "ARGS.txt"), new string[] { 
                    "Percentage of Attack node:" + AttackNodes.Text,
                    "Number of Victim:" + VictimNodes.Text,
                    "Percentage of Tracer:" + PercentageOfTracer.Text,
                    "Percentage of Tunneling Tracer:" + TunnelingTracer.Text,
                    "Percentage of Marking Tracer:" + MarkingTracer.Text,
                    "Percentage of Filtering Tracer:" + FilteringTracer.Text,
                    "Percentage of Marking receive, then begin filtering:" + StartFiltering.Text,
                    "Attack packet per second:" + AttackPacketPerSec.Text,
                    "Normal packet per second:" + NormalPacketPerSec.Text,
                    "Total Packet:" + TotalPacket.Text,
                    "Percentage of Attack Packet:" + PercentageOfAttackPacket.Text,
                    "Initial Time of Attack Packet:" + InitTimeOfAttackPacket.Text,
                    "Probability of Packet Tunneling:" + ProbibilityOfPacketTunneling.Text,
                    "Probability of Packet Marking:" + ProbibilityOfPacketMarking.Text
                });
            //}
            //catch (Exception exception)
            //{
            //    Console.WriteLine(exception.Message);
            //    //MessageBox.Show(exception.Message);
            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Show console window
            AllocConsole();

            bg.RunWorkerAsync();

            groupBox1.Enabled = false;
            groupBox2.Enabled = false;
            groupBox3.Enabled = false;
            button1.Enabled = false;
        }

        private void MapFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "Brite File | *.brite";
            op.Multiselect = true;

            if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filenames = op.FileNames.ToList();
                RefreshListView();
            }
        }

        private void RefreshListView()
        {
            listView1.Items.Clear();
            foreach (string filename in filenames)
            {
                ListViewItem item = listView1.Items.Add(new ListViewItem() { Name = filename, Text = Path.GetFileName(filename) });
                item.SubItems.Add(new ListViewItem.ListViewSubItem() { Text = "" });
                item.SubItems.Add(new ListViewItem.ListViewSubItem() { Text = "" });
                item.SubItems.Add(new ListViewItem.ListViewSubItem() { Text = "" });
                item.SubItems.Add(new ListViewItem.ListViewSubItem() { Text = "" });
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedIndices.Count > 0)
            {
                ListViewItem item = listView1.Items[listView1.SelectedIndices[0]];
                string dbFileName = string.Format("{0}_T{1}M{2}F{3}_A{4}V{5}_Pkt{6}_{7}_{8}.db", Path.GetFileNameWithoutExtension(item.Name),
                                                                                   TunnelingTracer.Text,
                                                                                   MarkingTracer.Text,
                                                                                   FilteringTracer.Text,
                                                                                   AttackNodes.Text,
                                                                                   VictimNodes.Text,
                                                                                   TotalPacket.Text,
                                                                                   PercentageOfAttackPacket.Text,
                                                                                   postfixIndex);
                using (NetworkViewForm nvf = new NetworkViewForm(item.Name, dbFileName)) 
                {
                    nvf.ShowDialog();
                }
            }
        }
    }
}
