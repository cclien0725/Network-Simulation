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
            this.StartFiltering = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label16 = new System.Windows.Forms.Label();
            this.InitTimeOfAttackPacket = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.ProbibilityOfPacketMarking = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.ProbibilityOfPacketTunneling = new System.Windows.Forms.TextBox();
            this.AttackPacketPerSec = new System.Windows.Forms.TextBox();
            this.TotalPacket = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.NormalPacketPerSec = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.PercentageOfAttackPacket = new System.Windows.Forms.TextBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label15 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(889, 492);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Run";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MapFile
            // 
            this.MapFile.Location = new System.Drawing.Point(155, 15);
            this.MapFile.Name = "MapFile";
            this.MapFile.Size = new System.Drawing.Size(183, 22);
            this.MapFile.TabIndex = 1;
            this.MapFile.Text = "Click here to choose map file...";
            this.MapFile.Click += new System.EventHandler(this.MapFile_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(120, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "Map:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(77, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(155, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "Percentage of Tunneling Tracer:";
            // 
            // TunnelingTracer
            // 
            this.TunnelingTracer.Location = new System.Drawing.Point(238, 15);
            this.TunnelingTracer.Name = "TunnelingTracer";
            this.TunnelingTracer.Size = new System.Drawing.Size(100, 22);
            this.TunnelingTracer.TabIndex = 5;
            this.TunnelingTracer.Text = "30";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(85, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(147, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "Percentage of Marking Tracer:";
            // 
            // MarkingTracer
            // 
            this.MarkingTracer.Location = new System.Drawing.Point(238, 45);
            this.MarkingTracer.Name = "MarkingTracer";
            this.MarkingTracer.Size = new System.Drawing.Size(100, 22);
            this.MarkingTracer.TabIndex = 7;
            this.MarkingTracer.Text = "20";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(86, 78);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(146, 12);
            this.label5.TabIndex = 10;
            this.label5.Text = "Percentage of Filtering Tracer:";
            // 
            // FilteringTracer
            // 
            this.FilteringTracer.Location = new System.Drawing.Point(238, 75);
            this.FilteringTracer.Name = "FilteringTracer";
            this.FilteringTracer.Size = new System.Drawing.Size(100, 22);
            this.FilteringTracer.TabIndex = 9;
            this.FilteringTracer.Text = "10";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 48);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(136, 12);
            this.label6.TabIndex = 12;
            this.label6.Text = "Percentage of Attack Nodes:";
            // 
            // AttackNodes
            // 
            this.AttackNodes.Location = new System.Drawing.Point(155, 45);
            this.AttackNodes.Name = "AttackNodes";
            this.AttackNodes.Size = new System.Drawing.Size(100, 22);
            this.AttackNodes.TabIndex = 11;
            this.AttackNodes.Text = "50";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(56, 76);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(93, 12);
            this.label7.TabIndex = 14;
            this.label7.Text = "Number of Victim:";
            // 
            // VictimNodes
            // 
            this.VictimNodes.Location = new System.Drawing.Point(155, 73);
            this.VictimNodes.Name = "VictimNodes";
            this.VictimNodes.Size = new System.Drawing.Size(100, 22);
            this.VictimNodes.TabIndex = 13;
            this.VictimNodes.Text = "1";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.StartFiltering);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.TunnelingTracer);
            this.groupBox1.Controls.Add(this.MarkingTracer);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.FilteringTracer);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(12, 122);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(351, 135);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tracer Setting";
            // 
            // StartFiltering
            // 
            this.StartFiltering.Location = new System.Drawing.Point(238, 105);
            this.StartFiltering.Name = "StartFiltering";
            this.StartFiltering.Size = new System.Drawing.Size(100, 22);
            this.StartFiltering.TabIndex = 11;
            this.StartFiltering.Text = "10";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 108);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(222, 12);
            this.label8.TabIndex = 12;
            this.label8.Text = "Percentage of Packet sent, then begin Filtering:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.MapFile);
            this.groupBox2.Controls.Add(this.AttackNodes);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.VictimNodes);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(351, 104);
            this.groupBox2.TabIndex = 18;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Map information";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label16);
            this.groupBox3.Controls.Add(this.InitTimeOfAttackPacket);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.ProbibilityOfPacketMarking);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.ProbibilityOfPacketTunneling);
            this.groupBox3.Controls.Add(this.AttackPacketPerSec);
            this.groupBox3.Controls.Add(this.TotalPacket);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.NormalPacketPerSec);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.PercentageOfAttackPacket);
            this.groupBox3.Location = new System.Drawing.Point(12, 265);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(351, 220);
            this.groupBox3.TabIndex = 19;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Packet Setting";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(30, 192);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(141, 12);
            this.label16.TabIndex = 23;
            this.label16.Text = "Initial Time of Attack Packet:";
            // 
            // InitTimeOfAttackPacket
            // 
            this.InitTimeOfAttackPacket.Location = new System.Drawing.Point(177, 189);
            this.InitTimeOfAttackPacket.Name = "InitTimeOfAttackPacket";
            this.InitTimeOfAttackPacket.Size = new System.Drawing.Size(100, 22);
            this.InitTimeOfAttackPacket.TabIndex = 22;
            this.InitTimeOfAttackPacket.Text = "1";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(28, 164);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(143, 12);
            this.label14.TabIndex = 21;
            this.label14.Text = "Probiblity of Packet Marking:";
            // 
            // ProbibilityOfPacketMarking
            // 
            this.ProbibilityOfPacketMarking.Location = new System.Drawing.Point(177, 161);
            this.ProbibilityOfPacketMarking.Name = "ProbibilityOfPacketMarking";
            this.ProbibilityOfPacketMarking.Size = new System.Drawing.Size(100, 22);
            this.ProbibilityOfPacketMarking.TabIndex = 20;
            this.ProbibilityOfPacketMarking.Text = "0.5";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(17, 136);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(154, 12);
            this.label13.TabIndex = 19;
            this.label13.Text = "Probibility of Packet Tunneling:";
            // 
            // ProbibilityOfPacketTunneling
            // 
            this.ProbibilityOfPacketTunneling.Location = new System.Drawing.Point(177, 133);
            this.ProbibilityOfPacketTunneling.Name = "ProbibilityOfPacketTunneling";
            this.ProbibilityOfPacketTunneling.Size = new System.Drawing.Size(100, 22);
            this.ProbibilityOfPacketTunneling.TabIndex = 18;
            this.ProbibilityOfPacketTunneling.Text = "0.5";
            // 
            // AttackPacketPerSec
            // 
            this.AttackPacketPerSec.Location = new System.Drawing.Point(177, 15);
            this.AttackPacketPerSec.Name = "AttackPacketPerSec";
            this.AttackPacketPerSec.Size = new System.Drawing.Size(100, 22);
            this.AttackPacketPerSec.TabIndex = 17;
            this.AttackPacketPerSec.Text = "1";
            // 
            // TotalPacket
            // 
            this.TotalPacket.Location = new System.Drawing.Point(177, 75);
            this.TotalPacket.Name = "TotalPacket";
            this.TotalPacket.Size = new System.Drawing.Size(100, 22);
            this.TotalPacket.TabIndex = 15;
            this.TotalPacket.Text = "5000";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(106, 78);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 16;
            this.label9.Text = "Total Packet:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(47, 18);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(124, 12);
            this.label10.TabIndex = 2;
            this.label10.Text = "Attack packet per second:";
            // 
            // NormalPacketPerSec
            // 
            this.NormalPacketPerSec.Location = new System.Drawing.Point(177, 45);
            this.NormalPacketPerSec.Name = "NormalPacketPerSec";
            this.NormalPacketPerSec.Size = new System.Drawing.Size(100, 22);
            this.NormalPacketPerSec.TabIndex = 11;
            this.NormalPacketPerSec.Text = "10";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(42, 48);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(129, 12);
            this.label11.TabIndex = 12;
            this.label11.Text = "Normal packet per second:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(41, 108);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(127, 12);
            this.label12.TabIndex = 14;
            this.label12.Text = "PercentageOfAttackPacket";
            // 
            // PercentageOfAttackPacket
            // 
            this.PercentageOfAttackPacket.Location = new System.Drawing.Point(177, 105);
            this.PercentageOfAttackPacket.Name = "PercentageOfAttackPacket";
            this.PercentageOfAttackPacket.Size = new System.Drawing.Size(100, 22);
            this.PercentageOfAttackPacket.TabIndex = 13;
            this.PercentageOfAttackPacket.Text = "30";
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.listView1.Location = new System.Drawing.Point(369, 12);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(594, 473);
            this.listView1.TabIndex = 20;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Map File";
            this.columnHeader1.Width = 230;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "None Deployment";
            this.columnHeader2.Width = 120;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Random Deployment";
            this.columnHeader3.Width = 120;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "KCut Deployment";
            this.columnHeader4.Width = 120;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(10, 497);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(68, 12);
            this.label15.TabIndex = 21;
            this.label15.Text = "Total Prgress:";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(84, 491);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(799, 23);
            this.progressBar1.TabIndex = 22;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(975, 523);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.groupBox3);
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
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.TextBox StartFiltering;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox TotalPacket;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox NormalPacketPerSec;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox PercentageOfAttackPacket;
        private System.Windows.Forms.TextBox AttackPacketPerSec;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox ProbibilityOfPacketMarking;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox ProbibilityOfPacketTunneling;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox InitTimeOfAttackPacket;
    }
}

