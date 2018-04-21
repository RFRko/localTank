namespace Tanki
{
	partial class GameOver
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
			this.ReturnToLobby_btn = new System.Windows.Forms.Button();
			this.WatchGame_btn = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// ReturnToLobby_btn
			// 
			this.ReturnToLobby_btn.Location = new System.Drawing.Point(114, 71);
			this.ReturnToLobby_btn.Name = "ReturnToLobby_btn";
			this.ReturnToLobby_btn.Size = new System.Drawing.Size(75, 23);
			this.ReturnToLobby_btn.TabIndex = 0;
			this.ReturnToLobby_btn.Text = "Lobby";
			this.ReturnToLobby_btn.UseVisualStyleBackColor = true;
			this.ReturnToLobby_btn.Click += new System.EventHandler(this.ReturnToLobby_btn_Click);
			// 
			// WatchGame_btn
			// 
			this.WatchGame_btn.Location = new System.Drawing.Point(26, 71);
			this.WatchGame_btn.Name = "WatchGame_btn";
			this.WatchGame_btn.Size = new System.Drawing.Size(75, 23);
			this.WatchGame_btn.TabIndex = 1;
			this.WatchGame_btn.Text = "Watch";
			this.WatchGame_btn.UseVisualStyleBackColor = true;
			this.WatchGame_btn.Click += new System.EventHandler(this.WatchGame_btn_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(23, 26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(35, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "label1";
			// 
			// GameOver
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(214, 105);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.WatchGame_btn);
			this.Controls.Add(this.ReturnToLobby_btn);
			this.Name = "GameOver";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "GameOver";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button ReturnToLobby_btn;
		private System.Windows.Forms.Button WatchGame_btn;
		private System.Windows.Forms.Label label1;
	}
}