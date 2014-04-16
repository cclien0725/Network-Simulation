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
        protected int postfixIndex;
        protected BackgroundWorker bg;

        protected enum StatusType { TotalProgress, NoneDeploymentStatus, RandomDeploymentStatus, KCutDeploymentStatus}
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
            }
        }

        void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            //try
            //{
                foreach (string filename in filenames)
                {
                    string dbName = string.Format("{0}_T{1}M{2}F{3}_A{4}V{5}_Pkt{6}", Path.GetFileNameWithoutExtension(filename),
                                                                               TunnelingTracer.Text,
                                                                               MarkingTracer.Text,
                                                                               FilteringTracer.Text,
                                                                               AttackNodes.Text,
                                                                               VictimNodes.Text,
                                                                               TotalPacket.Text);

                    SQLiteUtility sql = new SQLiteUtility(ref dbName);
                    postfixIndex = Convert.ToInt32(Path.GetFileNameWithoutExtension(dbName).Split('_').Last());

                    // Read network topology and initialize the attackers, normal users and victim.
                    NetworkTopology networkTopology = new NetworkTopology(Convert.ToDouble(AttackNodes.Text), Convert.ToInt32(VictimNodes.Text));
                    networkTopology.ReadBriteFile(filename);

                    //// Doesn't use any deployment method.
                    NoneDeployment noneDeply = new NoneDeployment(0, 0, 0);
                    sql.CreateTable(noneDeply.GetType().Name);
                    noneDeply.Deploy(networkTopology);
                    Simulator noneSimulator = new Simulator(noneDeply, networkTopology, sql);
                    noneSimulator.onReportOccur += delegate(object obj, Simulator.ReportArgument ra)
                    {
                        bg.ReportProgress(ra.CurrentPacket * 100 / ra.TotalPacket, new ProgressReportArg() { KEY = filename, ST = StatusType.NoneDeploymentStatus });
                    };
                    noneSimulator.Run(Convert.ToInt32(AttackPacketPerSec.Text), Convert.ToInt32(NormalPacketPerSec.Text), Convert.ToInt32(TotalPacket.Text), Convert.ToInt32(PercentageOfAttackPacket.Text), Convert.ToDouble(ProbibilityOfPacketTunneling.Text), Convert.ToDouble(ProbibilityOfPacketMarking.Text), Convert.ToDouble(StartFiltering.Text), Convert.ToInt32(InitTimeOfAttackPacket.Text), DynamicProbability.Checked);

                    bg.ReportProgress((filenames.IndexOf(filename) + 1) * 33 / filenames.Count, new ProgressReportArg() { ST = StatusType.TotalProgress });

                    //// Using randomly depolyment method.
                    RandomDeployment randomDeploy = new RandomDeployment(Convert.ToDouble(TunnelingTracer.Text), Convert.ToDouble(MarkingTracer.Text), Convert.ToDouble(FilteringTracer.Text));
                    sql.CreateTable(randomDeploy.GetType().Name);
                    randomDeploy.Deploy(networkTopology);
                    Simulator randomSimulator = new Simulator(randomDeploy, networkTopology, sql);
                    randomSimulator.onReportOccur += delegate(object obj, Simulator.ReportArgument ra)
                    {
                        bg.ReportProgress(ra.CurrentPacket * 100 / ra.TotalPacket, new ProgressReportArg() { KEY = filename, ST = StatusType.RandomDeploymentStatus });
                    };
                    randomSimulator.Run(Convert.ToInt32(AttackPacketPerSec.Text), Convert.ToInt32(NormalPacketPerSec.Text), Convert.ToInt32(TotalPacket.Text), Convert.ToInt32(PercentageOfAttackPacket.Text), Convert.ToDouble(ProbibilityOfPacketTunneling.Text), Convert.ToDouble(ProbibilityOfPacketMarking.Text), Convert.ToDouble(StartFiltering.Text), Convert.ToInt32(InitTimeOfAttackPacket.Text), DynamicProbability.Checked);

                    bg.ReportProgress((filenames.IndexOf(filename) + 1) * 66 / filenames.Count, new ProgressReportArg() { ST = StatusType.TotalProgress });

                    //// Using KCut deployment method.
                    KCutDeployment kCutDeploy = new KCutDeployment(Convert.ToDouble(TunnelingTracer.Text), Convert.ToDouble(MarkingTracer.Text), Convert.ToDouble(FilteringTracer.Text));
                    sql.CreateTable(kCutDeploy.GetType().Name);
                    kCutDeploy.Deploy(networkTopology);
                    Simulator kCutSimulator = new Simulator(kCutDeploy.Deployment, networkTopology, sql);
                    kCutSimulator.onReportOccur += delegate(object obj, Simulator.ReportArgument ra)
                    {
                        bg.ReportProgress(ra.CurrentPacket * 100 / ra.TotalPacket, new ProgressReportArg() { KEY = filename, ST = StatusType.KCutDeploymentStatus });
                    };
                    kCutSimulator.Run(Convert.ToInt32(AttackPacketPerSec.Text), Convert.ToInt32(NormalPacketPerSec.Text), Convert.ToInt32(TotalPacket.Text), Convert.ToInt32(PercentageOfAttackPacket.Text), Convert.ToDouble(ProbibilityOfPacketTunneling.Text), Convert.ToDouble(ProbibilityOfPacketMarking.Text), Convert.ToDouble(StartFiltering.Text), Convert.ToInt32(InitTimeOfAttackPacket.Text), DynamicProbability.Checked);

                    bg.ReportProgress((filenames.IndexOf(filename) + 1) * 100 / filenames.Count, new ProgressReportArg() { ST = StatusType.TotalProgress });

                    //// Using tomato deployment method.
                    //tomatodeployment tomatodeploy = new tomatodeployment(convert.todouble(tunnelingtracer.text), convert.todouble(markingtracer.text), convert.todouble(filteringtracer.text));
                    //sql.createtable(tomatodeploy.gettype().name);
                    //tomatodeploy.deploy(networktopology);
                    //simulator tomatosimulator = new simulator(tomatodeploy, networktopology);
                    //tomatosimulator.run(convert.toint32(attackpacketpersec.text), convert.toint32(normalpacketpersec.text), convert.toint32(numberofattackpacket.text), convert.toint32(numberofnormalpacket.text), convert.todouble(probibilityofpackettunneling.text), convert.todouble(probibilityofpacketmarking.text), convert.todouble(startfiltering.text));
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
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedIndices.Count > 0)
            {
                ListViewItem item = listView1.Items[listView1.SelectedIndices[0]];
                string dbFileName = string.Format("{0}_T{1}M{2}F{3}_A{4}V{5}_Pkt{6}_{7}.db", Path.GetFileNameWithoutExtension(item.Name),
                                                                                   TunnelingTracer.Text,
                                                                                   MarkingTracer.Text,
                                                                                   FilteringTracer.Text,
                                                                                   AttackNodes.Text,
                                                                                   VictimNodes.Text,
                                                                                   TotalPacket.Text,
                                                                                   postfixIndex);
                using (NetworkViewForm nvf = new NetworkViewForm(item.Name, dbFileName)) 
                {
                    nvf.ShowDialog();
                }
            }
        }
    }
}
