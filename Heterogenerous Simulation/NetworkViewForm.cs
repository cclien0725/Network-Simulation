using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Network_Simulation;

namespace Heterogenerous_Simulation
{
    public partial class NetworkViewForm : Form
    {
        private string fileName;

        public NetworkViewForm(string fileName)
        {
            InitializeComponent();

            this.fileName = fileName;
        }

        private void NetworkViewForm_Load(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex) 
            {
                DataUtility.Log(ex.Message);
            }
        }
    }
}
