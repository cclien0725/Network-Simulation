namespace Network_Simulation
{
    partial class DeployStatisticTool
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
            this.lv_hetero = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tb_hetero_select_files = new System.Windows.Forms.TextBox();
            this.btn_hetero_start = new System.Windows.Forms.Button();
            this.pb_hetero = new System.Windows.Forms.ProgressBar();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btn_deploy_start = new System.Windows.Forms.Button();
            this.pb_deploy = new System.Windows.Forms.ProgressBar();
            this.tb_deploy_select_files = new System.Windows.Forms.TextBox();
            this.lv_deploy = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.btn_merge = new System.Windows.Forms.Button();
            this.pb_merge = new System.Windows.Forms.ProgressBar();
            this.tb_merge = new System.Windows.Forms.TextBox();
            this.lv_merge = new System.Windows.Forms.ListView();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // lv_hetero
            // 
            this.lv_hetero.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lv_hetero.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lv_hetero.FullRowSelect = true;
            this.lv_hetero.GridLines = true;
            this.lv_hetero.Location = new System.Drawing.Point(3, 40);
            this.lv_hetero.Name = "lv_hetero";
            this.lv_hetero.ShowItemToolTips = true;
            this.lv_hetero.Size = new System.Drawing.Size(518, 302);
            this.lv_hetero.TabIndex = 0;
            this.lv_hetero.UseCompatibleStateImageBehavior = false;
            this.lv_hetero.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "File Name";
            this.columnHeader1.Width = 360;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Process Status";
            this.columnHeader2.Width = 140;
            // 
            // tb_hetero_select_files
            // 
            this.tb_hetero_select_files.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_hetero_select_files.Location = new System.Drawing.Point(3, 7);
            this.tb_hetero_select_files.Name = "tb_hetero_select_files";
            this.tb_hetero_select_files.ReadOnly = true;
            this.tb_hetero_select_files.Size = new System.Drawing.Size(403, 22);
            this.tb_hetero_select_files.TabIndex = 1;
            this.tb_hetero_select_files.Text = "Select Heterogenerous Simulation Database to Process...";
            // 
            // btn_hetero_start
            // 
            this.btn_hetero_start.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_hetero_start.Location = new System.Drawing.Point(412, 3);
            this.btn_hetero_start.Name = "btn_hetero_start";
            this.btn_hetero_start.Size = new System.Drawing.Size(109, 31);
            this.btn_hetero_start.TabIndex = 2;
            this.btn_hetero_start.Text = "Start Process";
            this.btn_hetero_start.UseVisualStyleBackColor = true;
            // 
            // pb_hetero
            // 
            this.pb_hetero.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pb_hetero.Location = new System.Drawing.Point(3, 348);
            this.pb_hetero.Name = "pb_hetero";
            this.pb_hetero.Size = new System.Drawing.Size(521, 23);
            this.pb_hetero.TabIndex = 3;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(535, 403);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btn_deploy_start);
            this.tabPage1.Controls.Add(this.pb_deploy);
            this.tabPage1.Controls.Add(this.tb_deploy_select_files);
            this.tabPage1.Controls.Add(this.lv_deploy);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(527, 374);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Deploy Statistic";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btn_deploy_start
            // 
            this.btn_deploy_start.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_deploy_start.Location = new System.Drawing.Point(412, 2);
            this.btn_deploy_start.Name = "btn_deploy_start";
            this.btn_deploy_start.Size = new System.Drawing.Size(109, 31);
            this.btn_deploy_start.TabIndex = 6;
            this.btn_deploy_start.Text = "Start Process";
            this.btn_deploy_start.UseVisualStyleBackColor = true;
            // 
            // pb_deploy
            // 
            this.pb_deploy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pb_deploy.Location = new System.Drawing.Point(3, 347);
            this.pb_deploy.Name = "pb_deploy";
            this.pb_deploy.Size = new System.Drawing.Size(521, 23);
            this.pb_deploy.TabIndex = 7;
            // 
            // tb_deploy_select_files
            // 
            this.tb_deploy_select_files.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_deploy_select_files.Location = new System.Drawing.Point(3, 6);
            this.tb_deploy_select_files.Name = "tb_deploy_select_files";
            this.tb_deploy_select_files.ReadOnly = true;
            this.tb_deploy_select_files.Size = new System.Drawing.Size(403, 22);
            this.tb_deploy_select_files.TabIndex = 5;
            this.tb_deploy_select_files.Text = "Select Deploy Simulation Database to Process...";
            // 
            // lv_deploy
            // 
            this.lv_deploy.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lv_deploy.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
            this.lv_deploy.FullRowSelect = true;
            this.lv_deploy.GridLines = true;
            this.lv_deploy.Location = new System.Drawing.Point(3, 39);
            this.lv_deploy.Name = "lv_deploy";
            this.lv_deploy.ShowItemToolTips = true;
            this.lv_deploy.Size = new System.Drawing.Size(518, 302);
            this.lv_deploy.TabIndex = 4;
            this.lv_deploy.UseCompatibleStateImageBehavior = false;
            this.lv_deploy.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "File Name";
            this.columnHeader3.Width = 340;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Process Status";
            this.columnHeader4.Width = 160;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btn_hetero_start);
            this.tabPage2.Controls.Add(this.pb_hetero);
            this.tabPage2.Controls.Add(this.tb_hetero_select_files);
            this.tabPage2.Controls.Add(this.lv_hetero);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(527, 374);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Hetero Statistic";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.btn_merge);
            this.tabPage3.Controls.Add(this.pb_merge);
            this.tabPage3.Controls.Add(this.tb_merge);
            this.tabPage3.Controls.Add(this.lv_merge);
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(527, 374);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Merge Deploy Database";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // btn_merge
            // 
            this.btn_merge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_merge.Location = new System.Drawing.Point(412, 3);
            this.btn_merge.Name = "btn_merge";
            this.btn_merge.Size = new System.Drawing.Size(109, 31);
            this.btn_merge.TabIndex = 10;
            this.btn_merge.Text = "Start Merge";
            this.btn_merge.UseVisualStyleBackColor = true;
            // 
            // pb_merge
            // 
            this.pb_merge.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pb_merge.Location = new System.Drawing.Point(3, 348);
            this.pb_merge.Name = "pb_merge";
            this.pb_merge.Size = new System.Drawing.Size(521, 23);
            this.pb_merge.TabIndex = 11;
            // 
            // tb_merge
            // 
            this.tb_merge.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_merge.Location = new System.Drawing.Point(3, 7);
            this.tb_merge.Name = "tb_merge";
            this.tb_merge.ReadOnly = true;
            this.tb_merge.Size = new System.Drawing.Size(403, 22);
            this.tb_merge.TabIndex = 9;
            this.tb_merge.Text = "Select Deploy Simulation Database to Merge...";
            // 
            // lv_merge
            // 
            this.lv_merge.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lv_merge.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader6});
            this.lv_merge.FullRowSelect = true;
            this.lv_merge.GridLines = true;
            this.lv_merge.Location = new System.Drawing.Point(3, 40);
            this.lv_merge.Name = "lv_merge";
            this.lv_merge.ShowItemToolTips = true;
            this.lv_merge.Size = new System.Drawing.Size(518, 302);
            this.lv_merge.TabIndex = 8;
            this.lv_merge.UseCompatibleStateImageBehavior = false;
            this.lv_merge.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "File Name";
            this.columnHeader5.Width = 340;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Process Status";
            this.columnHeader6.Width = 160;
            // 
            // DeployStatisticTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(535, 403);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.267326F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "DeployStatisticTool";
            this.Text = "Statistic Deployment Tools";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lv_hetero;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.TextBox tb_hetero_select_files;
        private System.Windows.Forms.Button btn_hetero_start;
        private System.Windows.Forms.ProgressBar pb_hetero;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btn_deploy_start;
        private System.Windows.Forms.ProgressBar pb_deploy;
        private System.Windows.Forms.TextBox tb_deploy_select_files;
        private System.Windows.Forms.ListView lv_deploy;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button btn_merge;
        private System.Windows.Forms.ProgressBar pb_merge;
        private System.Windows.Forms.TextBox tb_merge;
        private System.Windows.Forms.ListView lv_merge;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
    }
}