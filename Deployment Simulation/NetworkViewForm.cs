using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Network_Simulation;

namespace Deployment_Simulation
{
    public partial class NetworkViewForm : Form
    {
        private NetworkTopology m_topo;

        public NetworkViewForm(string briteFilePath)
        {
            InitializeComponent();

            tb_file_name.Text = briteFilePath;

            m_topo = new NetworkTopology(0, 0);

            m_topo.SetupDrawingControl(draw_panel);

            tb_k.TextChanged += new EventHandler(checkValid);
            tb_n.TextChanged += new EventHandler(checkValid);

            tb_k.KeyPress += new KeyPressEventHandler(checkInputValue);
            tb_n.KeyPress += new KeyPressEventHandler(checkInputValue);

            btn_show.Click += (s, e) =>
            {
                m_topo.ReadBriteFile(tb_file_name.Text);
                
                int K = int.Parse(tb_k.Text);
                int N = int.Parse(tb_n.Text);

                KCutStartWithConsider2KConsiderCoefficient deployment = new KCutStartWithConsider2KConsiderCoefficient(10, 10, 10, K, N);
                deployment.Deploy(m_topo);

                draw_panel.Invalidate();
            };
        }

        private void checkInputValue(object sender, KeyPressEventArgs e)
        {
            int tmp;

            e.Handled = !int.TryParse((sender as TextBox).Text + e.KeyChar, out tmp) && e.KeyChar != (char)Keys.Back;
        }

        private void checkValid(object sender, EventArgs e)
        {
            int tmp;

            btn_show.Enabled = int.TryParse(tb_k.Text, out tmp) && int.TryParse(tb_n.Text, out tmp);
        }
    }
}
