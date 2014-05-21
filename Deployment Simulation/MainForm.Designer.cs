namespace Deployment_Simulation
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btn_export = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_query = new System.Windows.Forms.Button();
            this.tb_sql_cmd = new System.Windows.Forms.TextBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cb_deployment = new System.Windows.Forms.ComboBox();
            this.lb_main_progress = new System.Windows.Forms.Label();
            this.lb_sub_progress = new System.Windows.Forms.Label();
            this.progress_sub = new System.Windows.Forms.ProgressBar();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rdoBtn_all = new System.Windows.Forms.RadioButton();
            this.rdoBtn_specific = new System.Windows.Forms.RadioButton();
            this.progress_main = new System.Windows.Forms.ProgressBar();
            this.tb_n = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tb_k = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_run = new System.Windows.Forms.Button();
            this.lv_list = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tb_select_file = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cb_run_sim = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.btn_export);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.btn_query);
            this.panel1.Controls.Add(this.tb_sql_cmd);
            this.panel1.Controls.Add(this.dataGridView1);
            this.panel1.Location = new System.Drawing.Point(331, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(687, 654);
            this.panel1.TabIndex = 0;
            // 
            // btn_export
            // 
            this.btn_export.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_export.Location = new System.Drawing.Point(578, 10);
            this.btn_export.Name = "btn_export";
            this.btn_export.Size = new System.Drawing.Size(106, 30);
            this.btn_export.TabIndex = 5;
            this.btn_export.Text = "Export to Excel";
            this.btn_export.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 17);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "SQL Command:";
            // 
            // btn_query
            // 
            this.btn_query.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_query.Location = new System.Drawing.Point(497, 10);
            this.btn_query.Name = "btn_query";
            this.btn_query.Size = new System.Drawing.Size(75, 30);
            this.btn_query.TabIndex = 3;
            this.btn_query.Text = "Query";
            this.btn_query.UseVisualStyleBackColor = true;
            // 
            // tb_sql_cmd
            // 
            this.tb_sql_cmd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_sql_cmd.Location = new System.Drawing.Point(114, 14);
            this.tb_sql_cmd.Name = "tb_sql_cmd";
            this.tb_sql_cmd.Size = new System.Drawing.Size(377, 25);
            this.tb_sql_cmd.TabIndex = 2;
            this.tb_sql_cmd.Text = resources.GetString("tb_sql_cmd.Text");
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dataGridView1.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(7, 48);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(677, 599);
            this.dataGridView1.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cb_deployment);
            this.groupBox1.Controls.Add(this.lb_main_progress);
            this.groupBox1.Controls.Add(this.lb_sub_progress);
            this.groupBox1.Controls.Add(this.progress_sub);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.progress_main);
            this.groupBox1.Controls.Add(this.tb_n);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.tb_k);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btn_run);
            this.groupBox1.Controls.Add(this.lv_list);
            this.groupBox1.Controls.Add(this.tb_select_file);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(331, 654);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Setting";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 158);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 17);
            this.label4.TabIndex = 16;
            this.label4.Text = "Method:";
            // 
            // cb_deployment
            // 
            this.cb_deployment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cb_deployment.FormattingEnabled = true;
            this.cb_deployment.Location = new System.Drawing.Point(71, 155);
            this.cb_deployment.Name = "cb_deployment";
            this.cb_deployment.Size = new System.Drawing.Size(251, 25);
            this.cb_deployment.TabIndex = 15;
            // 
            // lb_main_progress
            // 
            this.lb_main_progress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lb_main_progress.AutoSize = true;
            this.lb_main_progress.Location = new System.Drawing.Point(4, 604);
            this.lb_main_progress.Name = "lb_main_progress";
            this.lb_main_progress.Size = new System.Drawing.Size(98, 17);
            this.lb_main_progress.TabIndex = 14;
            this.lb_main_progress.Text = "Main Progress:";
            // 
            // lb_sub_progress
            // 
            this.lb_sub_progress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lb_sub_progress.AutoSize = true;
            this.lb_sub_progress.Location = new System.Drawing.Point(4, 558);
            this.lb_sub_progress.Name = "lb_sub_progress";
            this.lb_sub_progress.Size = new System.Drawing.Size(90, 17);
            this.lb_sub_progress.TabIndex = 5;
            this.lb_sub_progress.Text = "Sub Progress:";
            // 
            // progress_sub
            // 
            this.progress_sub.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progress_sub.Location = new System.Drawing.Point(7, 578);
            this.progress_sub.Name = "progress_sub";
            this.progress_sub.Size = new System.Drawing.Size(317, 23);
            this.progress_sub.TabIndex = 5;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cb_run_sim);
            this.groupBox2.Controls.Add(this.rdoBtn_all);
            this.groupBox2.Controls.Add(this.rdoBtn_specific);
            this.groupBox2.Location = new System.Drawing.Point(7, 50);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(125, 99);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Simulation Mode";
            // 
            // rdoBtn_all
            // 
            this.rdoBtn_all.AutoSize = true;
            this.rdoBtn_all.Location = new System.Drawing.Point(6, 45);
            this.rdoBtn_all.Name = "rdoBtn_all";
            this.rdoBtn_all.Size = new System.Drawing.Size(96, 21);
            this.rdoBtn_all.TabIndex = 14;
            this.rdoBtn_all.Text = "Simulate All";
            this.rdoBtn_all.UseVisualStyleBackColor = true;
            // 
            // rdoBtn_specific
            // 
            this.rdoBtn_specific.AutoSize = true;
            this.rdoBtn_specific.Checked = true;
            this.rdoBtn_specific.Location = new System.Drawing.Point(6, 19);
            this.rdoBtn_specific.Name = "rdoBtn_specific";
            this.rdoBtn_specific.Size = new System.Drawing.Size(97, 21);
            this.rdoBtn_specific.TabIndex = 13;
            this.rdoBtn_specific.TabStop = true;
            this.rdoBtn_specific.Text = "Specific K, N";
            this.rdoBtn_specific.UseVisualStyleBackColor = true;
            // 
            // progress_main
            // 
            this.progress_main.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progress_main.Location = new System.Drawing.Point(7, 624);
            this.progress_main.Name = "progress_main";
            this.progress_main.Size = new System.Drawing.Size(317, 23);
            this.progress_main.TabIndex = 4;
            // 
            // tb_n
            // 
            this.tb_n.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_n.Location = new System.Drawing.Point(172, 104);
            this.tb_n.Name = "tb_n";
            this.tb_n.Size = new System.Drawing.Size(150, 25);
            this.tb_n.TabIndex = 12;
            this.tb_n.Text = "13";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(147, 107);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(21, 17);
            this.label6.TabIndex = 11;
            this.label6.Text = "N:";
            // 
            // tb_k
            // 
            this.tb_k.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_k.Location = new System.Drawing.Point(172, 76);
            this.tb_k.Name = "tb_k";
            this.tb_k.Size = new System.Drawing.Size(150, 25);
            this.tb_k.TabIndex = 8;
            this.tb_k.Text = "3";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(147, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "K:";
            // 
            // btn_run
            // 
            this.btn_run.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_run.Enabled = false;
            this.btn_run.Location = new System.Drawing.Point(7, 186);
            this.btn_run.Name = "btn_run";
            this.btn_run.Size = new System.Drawing.Size(317, 30);
            this.btn_run.TabIndex = 7;
            this.btn_run.Text = "Start Simulation";
            this.btn_run.UseVisualStyleBackColor = true;
            // 
            // lv_list
            // 
            this.lv_list.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lv_list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader2});
            this.lv_list.FullRowSelect = true;
            this.lv_list.GridLines = true;
            this.lv_list.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lv_list.Location = new System.Drawing.Point(7, 222);
            this.lv_list.MultiSelect = false;
            this.lv_list.Name = "lv_list";
            this.lv_list.ShowItemToolTips = true;
            this.lv_list.Size = new System.Drawing.Size(317, 333);
            this.lv_list.TabIndex = 2;
            this.lv_list.UseCompatibleStateImageBehavior = false;
            this.lv_list.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "File Name";
            this.columnHeader1.Width = 88;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "K";
            this.columnHeader3.Width = 35;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "N";
            this.columnHeader4.Width = 33;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Status";
            this.columnHeader2.Width = 150;
            // 
            // tb_select_file
            // 
            this.tb_select_file.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_select_file.Location = new System.Drawing.Point(71, 19);
            this.tb_select_file.Name = "tb_select_file";
            this.tb_select_file.ReadOnly = true;
            this.tb_select_file.Size = new System.Drawing.Size(253, 25);
            this.tb_select_file.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 22);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Files(s):";
            // 
            // cb_run_sim
            // 
            this.cb_run_sim.AutoSize = true;
            this.cb_run_sim.Location = new System.Drawing.Point(6, 72);
            this.cb_run_sim.Name = "cb_run_sim";
            this.cb_run_sim.Size = new System.Drawing.Size(117, 21);
            this.cb_run_sim.TabIndex = 17;
            this.cb_run_sim.Text = "Run Actual Sim";
            this.cb_run_sim.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1018, 654);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Microsoft JhengHei", 9.267326F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = "Simulation of Deployment";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tb_n;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tb_k;
        private System.Windows.Forms.Button btn_run;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListView lv_list;
        private System.Windows.Forms.TextBox tb_select_file;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btn_query;
        private System.Windows.Forms.TextBox tb_sql_cmd;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ProgressBar progress_main;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rdoBtn_all;
        private System.Windows.Forms.RadioButton rdoBtn_specific;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lb_main_progress;
        private System.Windows.Forms.Label lb_sub_progress;
        private System.Windows.Forms.ProgressBar progress_sub;
        private System.Windows.Forms.Button btn_export;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cb_deployment;
        private System.Windows.Forms.CheckBox cb_run_sim;
    }
}

