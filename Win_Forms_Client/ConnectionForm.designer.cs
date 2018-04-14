namespace Tanki
{
	partial class ConnectionForm
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
			this.Ip_tb = new System.Windows.Forms.TextBox();
			this.Port_tb = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.Cancel_btn = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// Ip_tb
			// 
			this.Ip_tb.Location = new System.Drawing.Point(103, 13);
			this.Ip_tb.Name = "Ip_tb";
			this.Ip_tb.Size = new System.Drawing.Size(100, 20);
			this.Ip_tb.TabIndex = 0;
			// 
			// Port_tb
			// 
			this.Port_tb.Location = new System.Drawing.Point(103, 56);
			this.Port_tb.Name = "Port_tb";
			this.Port_tb.Size = new System.Drawing.Size(100, 20);
			this.Port_tb.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(25, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(51, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "IP adress";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(25, 59);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(65, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Remote port";
			// 
			// Cancel_btn
			// 
			this.Cancel_btn.Location = new System.Drawing.Point(28, 112);
			this.Cancel_btn.Name = "Cancel_btn";
			this.Cancel_btn.Size = new System.Drawing.Size(75, 23);
			this.Cancel_btn.TabIndex = 5;
			this.Cancel_btn.Text = "Cancel";
			this.Cancel_btn.UseVisualStyleBackColor = true;
			this.Cancel_btn.Click += new System.EventHandler(this.Cancel_btn_Click);
			// 
			// Connect_btn
			// 
			this.button2.Location = new System.Drawing.Point(128, 112);
			this.button2.Name = "Connect_btn";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 6;
			this.button2.Text = "Connect";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.Connect_btn_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.ForeColor = System.Drawing.Color.Red;
			this.label3.Location = new System.Drawing.Point(49, 138);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(0, 13);
			this.label3.TabIndex = 7;
			// 
			// ConnectionForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(237, 148);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.Cancel_btn);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.Port_tb);
			this.Controls.Add(this.Ip_tb);
			this.Name = "ConnectionForm";
			this.Text = "ConnectionForm";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox Ip_tb;
		private System.Windows.Forms.TextBox Port_tb;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button Cancel_btn;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Label label3;
	}
}