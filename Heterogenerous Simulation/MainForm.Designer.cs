namespace Heterogenerous_Simulation
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
            this.button1 = new System.Windows.Forms.Button();
            this.MapFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.TunnelingTracer = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.MarkingTracer = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.FilteringTracer = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.AttackNodes = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.VictimNodes = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.NormalUsers = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(314, 291);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Run";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MapFile
            // 
            this.MapFile.Location = new System.Drawing.Point(152, 18);
            this.MapFile.Name = "MapFile";
            this.MapFile.Size = new System.Drawing.Size(216, 22);
            this.MapFile.TabIndex = 1;
            this.MapFile.Text = "Click here to choose map file...";
            this.MapFile.Click += new System.EventHandler(this.MapFile_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(117, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "Map:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(155, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "Percentage of Tunneling Tracer:";
            // 
            // TunnelingTracer
            // 
            this.TunnelingTracer.Location = new System.Drawing.Point(167, 15);
            this.TunnelingTracer.Name = "TunnelingTracer";
            this.TunnelingTracer.Size = new System.Drawing.Size(100, 22);
            this.TunnelingTracer.TabIndex = 5;
            this.TunnelingTracer.Text = "30";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 46);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(147, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "Percentage of Marking Tracer:";
            // 
            // MarkingTracer
            // 
            this.MarkingTracer.Location = new System.Drawing.Point(167, 43);
            this.MarkingTracer.Name = "MarkingTracer";
            this.MarkingTracer.Size = new System.Drawing.Size(100, 22);
            this.MarkingTracer.TabIndex = 7;
            this.MarkingTracer.Text = "20";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(146, 12);
            this.label5.TabIndex = 10;
            this.label5.Text = "Percentage of Filtering Tracer:";
            // 
            // FilteringTracer
            // 
            this.FilteringTracer.Location = new System.Drawing.Point(167, 71);
            this.FilteringTracer.Name = "FilteringTracer";
            this.FilteringTracer.Size = new System.Drawing.Size(100, 22);
            this.FilteringTracer.TabIndex = 9;
            this.FilteringTracer.Text = "10";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 46);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(136, 12);
            this.label6.TabIndex = 12;
            this.label6.Text = "Percentage of Attack Nodes:";
            // 
            // AttackNodes
            // 
            this.AttackNodes.Location = new System.Drawing.Point(152, 46);
            this.AttackNodes.Name = "AttackNodes";
            this.AttackNodes.Size = new System.Drawing.Size(100, 22);
            this.AttackNodes.TabIndex = 11;
            this.AttackNodes.Text = "10";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(53, 102);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(93, 12);
            this.label7.TabIndex = 14;
            this.label7.Text = "Number of Victim:";
            // 
            // VictimNodes
            // 
            this.VictimNodes.Location = new System.Drawing.Point(152, 102);
            this.VictimNodes.Name = "VictimNodes";
            this.VictimNodes.Size = new System.Drawing.Size(100, 22);
            this.VictimNodes.TabIndex = 13;
            this.VictimNodes.Text = "1";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.TunnelingTracer);
            this.groupBox1.Controls.Add(this.MarkingTracer);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.FilteringTracer);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(12, 178);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(377, 107);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tracer Setting";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.NormalUsers);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.MapFile);
            this.groupBox2.Controls.Add(this.AttackNodes);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.VictimNodes);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(377, 160);
            this.groupBox2.TabIndex = 18;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Map information";
            // 
            // NormalUsers
            // 
            this.NormalUsers.Location = new System.Drawing.Point(152, 74);
            this.NormalUsers.Name = "NormalUsers";
            this.NormalUsers.Size = new System.Drawing.Size(100, 22);
            this.NormalUsers.TabIndex = 15;
            this.NormalUsers.Text = "10";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(140, 12);
            this.label2.TabIndex = 16;
            this.label2.Text = "Percentage of  Normal Users:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(401, 319);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainForm";
            this.Text = "Hetergeneous Simulation V2";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox MapFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TunnelingTracer;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox MarkingTracer;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox FilteringTracer;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox AttackNodes;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox VictimNodes;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox NormalUsers;
        private System.Windows.Forms.Label label2;
    }
}

