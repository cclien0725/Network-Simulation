using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Network_Simulation;
using System.IO;
using System.Data.SQLite;
using Deployment_Simulation;

namespace Heterogenerous_Simulation
{
    public partial class NetworkViewForm : Form
    {
        private string dbFileName;
        private NetworkTopology networkTopology;

        public NetworkViewForm(string mapFileName, string dbFileName)
        {
            InitializeComponent();

            this.dbFileName = Path.Combine(Environment.CurrentDirectory, "Log", dbFileName); ;

            comboBox1.Items.Add(typeof(NoneDeployment).Name);
            comboBox1.Items.Add(typeof(RandomDeployment).Name);
            comboBox1.Items.Add("KCutDeployV1");
            comboBox1.Items.Add("KCutDeployV2");
            //comboBox1.Items.Add(typeof(KCutStartWithCenterNode).Name);
            //comboBox1.Items.Add(typeof(KCutStartWithCenterNodeConsiderCoefficient).Name);
            //comboBox1.Items.Add(typeof(KCutStartWithComparableConsiderCoefficient).Name);
            //comboBox1.Items.Add(typeof(KCutStartWithConsider2KConsiderCoefficient).Name);
            //comboBox1.Items.Add(typeof(KCutStartWithSideNode).Name);
            //comboBox1.Items.Add(typeof(KCutStartWithSideNodeConsiderCoefficient).Name);

            comboBox1.SelectedValue = typeof(NoneDeployment).Name;

            networkTopology = new NetworkTopology(0, 0);
            networkTopology.ReadBriteFile(mapFileName);

            networkTopology.SetupDrawingControl(panel1);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try {
                string deployment = comboBox1.Text.Replace(" ", "");

                using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + dbFileName))
                {
                    connection.Open();

                    SQLiteCommand cmd = connection.CreateCommand();
                    cmd.CommandText = string.Format("SELECT * FROM {0}_Deployment", deployment);

                    SQLiteDataReader rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        networkTopology.Nodes.Find(n => n.ID == Convert.ToInt32(rd["NodeID"])).Tracer = (NetworkTopology.TracerType)Convert.ToInt32(rd["TracerType"]);
                        networkTopology.Nodes.Find(n => n.ID == Convert.ToInt32(rd["NodeID"])).Type = (NetworkTopology.NodeType)Convert.ToInt32(rd["NodeType"]);
                    }
                }

                panel1.Invalidate();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
