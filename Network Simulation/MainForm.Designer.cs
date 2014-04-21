namespace Network_Simulation
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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btn_precompute = new System.Windows.Forms.Button();
            this.btn_statistic = new System.Windows.Forms.Button();
            this.btn_deploy_sim = new System.Windows.Forms.Button();
            this.btn_hetero_sim = new System.Windows.Forms.Button();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btn_precompute);
            this.flowLayoutPanel1.Controls.Add(this.btn_statistic);
            this.flowLayoutPanel1.Controls.Add(this.btn_deploy_sim);
            this.flowLayoutPanel1.Controls.Add(this.btn_hetero_sim);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(10);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(5);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(365, 208);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // btn_precompute
            // 
            this.btn_precompute.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btn_precompute.Location = new System.Drawing.Point(15, 15);
            this.btn_precompute.Margin = new System.Windows.Forms.Padding(10);
            this.btn_precompute.Name = "btn_precompute";
            this.btn_precompute.Padding = new System.Windows.Forms.Padding(10);
            this.btn_precompute.Size = new System.Drawing.Size(157, 75);
            this.btn_precompute.TabIndex = 0;
            this.btn_precompute.Text = "Precompute Tool";
            this.btn_precompute.UseVisualStyleBackColor = true;
            // 
            // btn_statistic
            // 
            this.btn_statistic.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btn_statistic.Location = new System.Drawing.Point(192, 15);
            this.btn_statistic.Margin = new System.Windows.Forms.Padding(10);
            this.btn_statistic.Name = "btn_statistic";
            this.btn_statistic.Padding = new System.Windows.Forms.Padding(10);
            this.btn_statistic.Size = new System.Drawing.Size(157, 75);
            this.btn_statistic.TabIndex = 1;
            this.btn_statistic.Text = "Deploy Statistic";
            this.btn_statistic.UseVisualStyleBackColor = true;
            // 
            // btn_deploy_sim
            // 
            this.btn_deploy_sim.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btn_deploy_sim.Location = new System.Drawing.Point(15, 110);
            this.btn_deploy_sim.Margin = new System.Windows.Forms.Padding(10);
            this.btn_deploy_sim.Name = "btn_deploy_sim";
            this.btn_deploy_sim.Padding = new System.Windows.Forms.Padding(10);
            this.btn_deploy_sim.Size = new System.Drawing.Size(157, 75);
            this.btn_deploy_sim.TabIndex = 2;
            this.btn_deploy_sim.Text = "Deploy Simulator";
            this.btn_deploy_sim.UseVisualStyleBackColor = true;
            // 
            // btn_hetero_sim
            // 
            this.btn_hetero_sim.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btn_hetero_sim.Location = new System.Drawing.Point(192, 110);
            this.btn_hetero_sim.Margin = new System.Windows.Forms.Padding(10);
            this.btn_hetero_sim.Name = "btn_hetero_sim";
            this.btn_hetero_sim.Padding = new System.Windows.Forms.Padding(10);
            this.btn_hetero_sim.Size = new System.Drawing.Size(157, 75);
            this.btn_hetero_sim.TabIndex = 3;
            this.btn_hetero_sim.Text = "Heterogenerous Simulator";
            this.btn_hetero_sim.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(365, 208);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Font = new System.Drawing.Font("Microsoft JhengHei", 9.267326F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = "Network Simulation Tools";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btn_precompute;
        private System.Windows.Forms.Button btn_statistic;
        private System.Windows.Forms.Button btn_deploy_sim;
        private System.Windows.Forms.Button btn_hetero_sim;
    }
}