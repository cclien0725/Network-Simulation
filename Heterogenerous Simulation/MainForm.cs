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

namespace Heterogenerous_Simulation
{
    public partial class MainForm : Form
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        protected List<string> filenames;
        protected BackgroundWorker bg;

        protected enum StatusType { TotalProgress, NoneDeploymentStatus, RandomDeploymentStatus, TomatoDeploymentStatus}
        protected class ProgressReportArg 
        {
            public string KEY { get; set; }
            public StatusType ST { get; set; }
        }


        public MainForm()
        {
            InitializeComponent();
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
                    listView1.Items[pra.KEY].SubItems[1].Text = e.ProgressPercentage.ToString() + "%";
                    break;
                case StatusType.RandomDeploymentStatus:
                    listView1.Items[pra.KEY].SubItems[2].Text = e.ProgressPercentage.ToString() + "%";
                    break;
                case StatusType.TomatoDeploymentStatus:
                    listView1.Items[pra.KEY].SubItems[3].Text = e.ProgressPercentage.ToString() + "%";
                    break;
                default:
                    break;
            }
        }

        void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            //try
            //{
                foreach (string filename in filenames)
                {
                    string dbName = string.Format("{0}_T{1}M{2}F{3}_A{4}N{5}V{6}", Path.GetFileNameWithoutExtension(filename),
                                                                               TunnelingTracer.Text,
                                                                               MarkingTracer.Text,
                                                                               FilteringTracer.Text,
                                                                               AttackNodes.Text,
                                                                               NormalUsers.Text,
                                                                               VictimNodes.Text);

                    // Read network topology and initialize the attackers, normal users and victim.
                    NetworkTopology networkTopology = new NetworkTopology(Convert.ToDouble(AttackNodes.Text), Convert.ToDouble(NormalUsers.Text), Convert.ToInt32(VictimNodes.Text));
                    networkTopology.ReadBriteFile(filename);

                    //// Doesn't use any deployment method.
                    NoneDeployment noneDeply = new NoneDeployment(0, 0, 0);
                    noneDeply.Deploy(networkTopology);
                    Simulator noneSimulator = new Simulator(dbName, noneDeply, networkTopology);
                    noneSimulator.onReportOccur += delegate(object obj, Simulator.ReportArgument ra)
                    {
                        bg.ReportProgress(ra.CurrentNode * 100 / ra.TotalActiveNode, new ProgressReportArg() { KEY = filename, ST = StatusType.NoneDeploymentStatus });
                    };
                    noneSimulator.Run(Convert.ToInt32(AttackPacketPerSec.Text), Convert.ToInt32(NormalPacketPerSec.Text), Convert.ToInt32(NumberOfAttackPacket.Text), Convert.ToInt32(NumberOfNormalPacket.Text), Convert.ToDouble(ProbibilityOfPacketTunneling.Text), Convert.ToDouble(ProbibilityOfPacketMarking.Text), Convert.ToDouble(StartFiltering.Text));

                    bg.ReportProgress((filenames.IndexOf(filename) + 1) * 100 / filenames.Count, new ProgressReportArg() { ST = StatusType.TotalProgress });

                    //// Using randomly depolyment method.
                    RandomDeployment randomDeploy = new RandomDeployment(Convert.ToDouble(TunnelingTracer.Text), Convert.ToDouble(MarkingTracer.Text), Convert.ToDouble(FilteringTracer.Text));
                    randomDeploy.Deploy(networkTopology);
                    Simulator randomSimulator = new Simulator(dbName, randomDeploy, networkTopology);
                    randomSimulator.onReportOccur += delegate(object obj, Simulator.ReportArgument ra)
                    {
                        bg.ReportProgress(ra.CurrentNode * 100 / ra.TotalActiveNode, new ProgressReportArg() { KEY = filename, ST = StatusType.RandomDeploymentStatus });
                    };
                    randomSimulator.Run(Convert.ToInt32(AttackPacketPerSec.Text), Convert.ToInt32(NormalPacketPerSec.Text), Convert.ToInt32(NumberOfAttackPacket.Text), Convert.ToInt32(NumberOfNormalPacket.Text), Convert.ToDouble(ProbibilityOfPacketTunneling.Text), Convert.ToDouble(ProbibilityOfPacketMarking.Text), Convert.ToDouble(StartFiltering.Text));

                    bg.ReportProgress((filenames.IndexOf(filename) + 1) * 100 / filenames.Count, new ProgressReportArg() { ST = StatusType.TotalProgress });

                    //// Using tomato deployment method.
                    //TomatoDeployment tomatoDeploy = new TomatoDeployment(Convert.ToDouble(TunnelingTracer.Text), Convert.ToDouble(MarkingTracer.Text), Convert.ToDouble(FilteringTracer.Text));
                    //tomatoDeploy.Deploy(networkTopology);
                    //Simulator tomatoSimulator = new Simulator(dbName, tomatoDeploy, networkTopology);
                    //tomatoSimulator.Run(Convert.ToInt32(AttackPacketPerSec.Text), Convert.ToInt32(NormalPacketPerSec.Text), Convert.ToInt32(NumberOfAttackPacket.Text), Convert.ToInt32(NumberOfNormalPacket.Text), Convert.ToDouble(ProbibilityOfPacketTunneling.Text), Convert.ToDouble(ProbibilityOfPacketMarking.Text), Convert.ToDouble(StartFiltering.Text));
                }
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
            op.Multiselect = true;

            if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filenames = op.FileNames.ToList();
                RefreshListView();
            }
        }

        private void RefreshListView()
        {
            foreach (string filename in filenames)
            {
                ListViewItem item = listView1.Items.Add(new ListViewItem() { Name = filename, Text = Path.GetFileName(filename) });
                item.SubItems.Add(new ListViewItem.ListViewSubItem() { Text = "" });
                item.SubItems.Add(new ListViewItem.ListViewSubItem() { Text = "" });
                item.SubItems.Add(new ListViewItem.ListViewSubItem() { Text = "" });
            }
        }
    }
}
