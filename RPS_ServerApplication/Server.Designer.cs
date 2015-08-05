namespace RPS_ServerApplication
{
    partial class Server
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Server));
            this.eventsLbl = new System.Windows.Forms.Label();
            this.ipLbl = new System.Windows.Forms.Label();
            this.portLbl = new System.Windows.Forms.Label();
            this.ipTB = new System.Windows.Forms.TextBox();
            this.portTB = new System.Windows.Forms.TextBox();
            this.enableBtn = new System.Windows.Forms.Button();
            this.shutdownBtn = new System.Windows.Forms.Button();
            this.playersLbl = new System.Windows.Forms.Label();
            this.eventsTB = new System.Windows.Forms.TextBox();
            this.playerLB = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // eventsLabel
            // 
            this.eventsLbl.AutoSize = true;
            this.eventsLbl.Location = new System.Drawing.Point(13, 13);
            this.eventsLbl.Name = "eventsLbl";
            this.eventsLbl.Size = new System.Drawing.Size(43, 13);
            this.eventsLbl.TabIndex = 0;
            this.eventsLbl.Text = "Events:";
            // 
            // ipLbl
            // 
            this.ipLbl.AutoSize = true;
            this.ipLbl.Location = new System.Drawing.Point(313, 16);
            this.ipLbl.Name = "ipLbl";
            this.ipLbl.Size = new System.Drawing.Size(90, 13);
            this.ipLbl.TabIndex = 2;
            this.ipLbl.Text = "Local IP Address:";
            // 
            // portLbl
            // 
            this.portLbl.AutoSize = true;
            this.portLbl.Location = new System.Drawing.Point(314, 39);
            this.portLbl.Name = "portLbl";
            this.portLbl.Size = new System.Drawing.Size(69, 13);
            this.portLbl.TabIndex = 3;
            this.portLbl.Text = "Port Number:";
            // 
            // ipTB
            // 
            this.ipTB.Location = new System.Drawing.Point(409, 13);
            this.ipTB.Name = "ipTB";
            this.ipTB.ReadOnly = true;
            this.ipTB.Size = new System.Drawing.Size(100, 20);
            this.ipTB.TabIndex = 4;
            // 
            // portTB
            // 
            this.portTB.Location = new System.Drawing.Point(409, 39);
            this.portTB.Name = "portTB";
            this.portTB.Size = new System.Drawing.Size(100, 20);
            this.portTB.TabIndex = 5;
            // 
            // enableBtn
            // 
            this.enableBtn.Location = new System.Drawing.Point(355, 207);
            this.enableBtn.Name = "enableBtn";
            this.enableBtn.Size = new System.Drawing.Size(130, 23);
            this.enableBtn.TabIndex = 6;
            this.enableBtn.Text = "Enable Server";
            this.enableBtn.UseVisualStyleBackColor = true;
            this.enableBtn.Click += new System.EventHandler(this.enableBtn_Click);
            // 
            // shutdownBtn
            // 
            this.shutdownBtn.Location = new System.Drawing.Point(355, 236);
            this.shutdownBtn.Name = "shutdownBtn";
            this.shutdownBtn.Size = new System.Drawing.Size(130, 23);
            this.shutdownBtn.TabIndex = 7;
            this.shutdownBtn.Text = "Shutdown";
            this.shutdownBtn.UseVisualStyleBackColor = true;
            this.shutdownBtn.Click += new System.EventHandler(this.shutdownBtn_Click);
            // 
            // playersLbl
            // 
            this.playersLbl.AutoSize = true;
            this.playersLbl.Location = new System.Drawing.Point(316, 73);
            this.playersLbl.Name = "playersLbl";
            this.playersLbl.Size = new System.Drawing.Size(99, 13);
            this.playersLbl.TabIndex = 8;
            this.playersLbl.Text = "Connected Players:";
            // 
            // eventsTB
            // 
            this.eventsTB.Location = new System.Drawing.Point(18, 29);
            this.eventsTB.Multiline = true;
            this.eventsTB.Name = "eventsTB";
            this.eventsTB.Size = new System.Drawing.Size(284, 324);
            this.eventsTB.TabIndex = 10;
            // 
            // playerLB
            // 
            this.playerLB.FormattingEnabled = true;
            this.playerLB.Location = new System.Drawing.Point(319, 89);
            this.playerLB.Name = "playerLB";
            this.playerLB.Size = new System.Drawing.Size(190, 95);
            this.playerLB.TabIndex = 11;
            // 
            // Server
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(530, 372);
            this.Controls.Add(this.playerLB);
            this.Controls.Add(this.eventsTB);
            this.Controls.Add(this.playersLbl);
            this.Controls.Add(this.shutdownBtn);
            this.Controls.Add(this.enableBtn);
            this.Controls.Add(this.portTB);
            this.Controls.Add(this.ipTB);
            this.Controls.Add(this.portLbl);
            this.Controls.Add(this.ipLbl);
            this.Controls.Add(this.eventsLbl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Server";
            this.Text = "Rock, Paper, Scissors | Server";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label eventsLbl;
        private System.Windows.Forms.Label ipLbl;
        private System.Windows.Forms.Label portLbl;
        private System.Windows.Forms.TextBox ipTB;
        private System.Windows.Forms.TextBox portTB;
        private System.Windows.Forms.Button enableBtn;
        private System.Windows.Forms.Button shutdownBtn;
        private System.Windows.Forms.Label playersLbl;
        private System.Windows.Forms.TextBox eventsTB;
        private System.Windows.Forms.ListBox playerLB;
    }
}

