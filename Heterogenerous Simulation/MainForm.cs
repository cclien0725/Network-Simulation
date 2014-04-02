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

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Show console window
                AllocConsole();

                string dbName = string.Format("{0}_T{1}M{2}F{3}_A{4}N{5}V{6}", Path.GetFileNameWithoutExtension(MapFile.Text),
                                                                               TunnelingTracer.Text,
                                                                               MarkingTracer.Text,
                                                                               FilteringTracer.Text,
                                                                               AttackNodes.Text,
                                                                               NormalUsers.Text,
                                                                               VictimNodes.Text);

                // Read network topology and initialize the attackers, normal users and victim.
                NetworkTopology networkTopology = new NetworkTopology(Convert.ToDouble(AttackNodes.Text), Convert.ToDouble(NormalUsers.Text), Convert.ToInt32(VictimNodes.Text));
                networkTopology.ReadBriteFile(MapFile.Text);

                // Doesn't use any deployment method.
                //networkTopology.Run();

                // Using randomly depolyment method.
                RandomDeployment randomDeploy = new RandomDeployment(Convert.ToDouble(TunnelingTracer.Text), Convert.ToDouble(MarkingTracer.Text), Convert.ToDouble(FilteringTracer.Text));
                randomDeploy.Deploy(networkTopology);
                Simulator randomSimulator = new Simulator(dbName, randomDeploy, networkTopology);
                randomSimulator.Run();

                //networkTopology.Run(dbName, "Random");

                // Using tomato deployment method.
                //TomatoDeployment tomatoDeploy = new TomatoDeployment(Convert.ToDouble(TunnelingTracer.Text), Convert.ToDouble(MarkingTracer.Text), Convert.ToDouble(FilteringTracer.Text));
                //tomatoDeploy.Deploy(networkTopology);

                //networkTopology.Run(dbName, "Tomato");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                //MessageBox.Show(exception.Message);
            }
        }

        private void MapFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MapFile.Text = op.FileName;
            }
        }
    }
}
