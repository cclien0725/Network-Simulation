using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Network_Simulation;

namespace Network_Simulation
{
    public partial class PrecomputingForm : Form
    {
        private int m_now_index;

        public PrecomputingForm()
        {
            InitializeComponent();

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;

            m_now_index = 0;

            worker.DoWork += (s, e) =>
            {
                ListViewItem item = e.Argument as ListViewItem;

                if (item == null)
                {
                    e.Cancel = true;
                    worker.CancelAsync();
                }
                else
                {
                    worker.ReportProgress(m_now_index);

                    NetworkTopology topo = new NetworkTopology(10, 10);
                    topo.ReadBriteFile(item.SubItems[0].Text);
                }
            };

            worker.RunWorkerCompleted += (s, e) =>
            {
                if (!e.Cancelled)
                {
                    getItem().SubItems[1].Text = "Completed.";
                    m_now_index++;
                    progress_bar.Value = (int)((float)m_now_index / (float)lv_status.Items.Count * 100);
                    worker.RunWorkerAsync(getItem());
                }
                else
                {
                    btn_computing.Enabled = true;
                    tb_select_file.Enabled = true;
                }
            };

            worker.ProgressChanged += (s, e) =>
            {
                getItem().SubItems[1].Text = "Computing...";
            };

            tb_select_file.Click += (s, e) =>
            {
                openFileDialog1.Multiselect = true;
                openFileDialog1.InitialDirectory = Environment.CurrentDirectory;
                openFileDialog1.Filter = "Birte File|*.brite";
                openFileDialog1.Title = "Select Brite Files";
                openFileDialog1.FileName = "";

                if (openFileDialog1.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    lv_status.BeginUpdate();

                    tb_select_file.Text = "";
                    lv_status.Items.Clear();

                    foreach (string file in openFileDialog1.FileNames)
                    {
                        tb_select_file.Text += file + ";";

                        ListViewItem lvi = new ListViewItem(new string[] { file, "Queued..." });
                        lv_status.Items.Add(lvi);
                    }

                    lv_status.EndUpdate();
                }
            };

            btn_computing.Click += (s, e) =>
            {
                m_now_index = 0;
                worker.RunWorkerAsync(getItem());
                btn_computing.Enabled = false;
                tb_select_file.Enabled = false;
            };
        }

        private ListViewItem getItem()
        {
            if (m_now_index < lv_status.Items.Count)
                return lv_status.Items[m_now_index];
            else
                return null;
        }
    }
}
