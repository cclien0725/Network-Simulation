using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Network_Simulation;
using System.Runtime.InteropServices;
using Heterogenerous_Simulation;

namespace Deployment_Simulation
{
    public partial class Form1 : Form
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        private string fileName = string.Format(@"C:\Users\{0}\Dropbox\RCLab\Personal Paper\TestMap3.brite", Environment.UserName);
        //private string fileName = @"C:\Users\Chia-Chun Lien\Desktop\n1kd4_4.brite";
        //private string fileName = @"C:\Users\Chia-Chun Lien\Desktop\TestMap4.brite";
        //private string fileName = @"C:\Users\Chia-Chun Lien\Desktop\test.brite";
        //private string fileName = @"C:\Users\Chia-Chun Lien\Desktop\n5kd6_20.brite";
        private NetworkTopology networkTopology;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                // Show console window
                AllocConsole();

                // Read network topology and initialize the attackers, normal users and victim.
                networkTopology = new NetworkTopology(10, 10, 1);
                networkTopology.SetupDrawingControl(this.panel1);
                networkTopology.ReadBriteFile(fileName);

                // Using kcutwithclustering depolyment method.
                KCutWithClusteringDeployment clusteringDeploy = new KCutWithClusteringDeployment(30, 20, 10);
                clusteringDeploy.Deploy(networkTopology);

                //TomatoDeployment tomato = new TomatoDeployment(30, 20, 10);
                //networkTopology.Deploy(tomato);
                //networkTopology.Run();
            }
            catch (Exception exception) 
            {
                Console.WriteLine("\n========== {0} ==========", exception.Message);
                Console.WriteLine(exception.StackTrace);
                Console.WriteLine("".PadLeft(exception.Message.Length + 22, '='));
            }
        }
    }
}
