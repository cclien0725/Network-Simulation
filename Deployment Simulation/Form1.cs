﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Network_Simulation;
using System.Runtime.InteropServices;

namespace Deployment_Simulation
{
    public partial class Form1 : Form
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        private string fileName = string.Format(@"C:\Users\{0}\Dropbox\RCLab\Personal Paper\TestMap3.brite", Environment.UserName);

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

                // Using randomly depolyment method.
                KCutWithClusteringDeployment randomDeploy = new KCutWithClusteringDeployment(30, 20, 10);
                networkTopology.Deploy(randomDeploy);
                networkTopology.Run();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}