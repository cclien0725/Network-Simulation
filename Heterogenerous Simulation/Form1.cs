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

namespace Heterogenerous_Simulation
{
    public partial class Form1 : Form
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        private string fileName = @"C:\Users\RCK\Dropbox\RCLab\Personal Paper\TestMap3.brite";

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

                // Read network topology and initialize the attackers and victim.
                NetworkTopology networkTopology = new NetworkTopology(10, 1);
                networkTopology.ReadBriteFile(fileName);

                #region Test region
                //List<int> path = networkTopology.Path(1, 14);
                //foreach (int node in path)
                //    Console.Write(node + " ");
                #endregion

                // Doesn't use any deployment method.
                //networkTopology.Run();

                // Using randomly depolyment method.
                RandomDeployment randomDeploy = new RandomDeployment(30, 20, 10);
                networkTopology.Deploy(randomDeploy);
                networkTopology.Run();

                // Using tomato deployment method.
                //TomatoDeployment tomatoDeploy = new TomatoDeployment(30, 20, 10);
                //networkTopology.Deploy(tomatoDeploy);
                //networkTopology.Run();
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.Message);
                //MessageBox.Show(exception.Message);
            }
        }
    }
}
