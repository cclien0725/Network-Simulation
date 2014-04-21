using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Network_Simulation
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            btn_precompute.Click += (s, e) =>
            {
                new PrecomputingForm().Show();
            };

            btn_statistic.Click += (s, e) =>
            {
                new DeployStatisticTool().Show();
            };

            btn_deploy_sim.Click += (s, e) =>
            {
                new Deployment_Simulation.MainForm().Show();
            };

            btn_hetero_sim.Click += (s, e) =>
            {
                new Heterogenerous_Simulation.MainForm().Show();
            };
        }
    }
}
