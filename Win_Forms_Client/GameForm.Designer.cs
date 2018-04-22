namespace Tanki
{
	partial class GameForm
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
			this.Message = new System.Windows.Forms.Label();
			this.ToLobby_btn = new System.Windows.Forms.Button();
			this.WatchGame_btn = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// Message
			// 
			this.Message.AutoSize = true;
			this.Message.BackColor = System.Drawing.Color.Black;
			this.Message.Font = new System.Drawing.Font("Comic Sans MS", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Message.ForeColor = System.Drawing.Color.White;
			this.Message.Location = new System.Drawing.Point(64, 49);
			this.Message.Name = "Message";
			this.Message.Size = new System.Drawing.Size(88, 38);
			this.Message.TabIndex = 0;
			this.Message.Text = "label1";
			this.Message.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ToLobby_btn
			// 
			this.ToLobby_btn.BackColor = System.Drawing.Color.Gray;
			this.ToLobby_btn.FlatAppearance.BorderColor = System.Drawing.Color.Yellow;
			this.ToLobby_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ToLobby_btn.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ToLobby_btn.Location = new System.Drawing.Point(0, 0);
			this.ToLobby_btn.Name = "ToLobby_btn";
			this.ToLobby_btn.Size = new System.Drawing.Size(100, 35);
			this.ToLobby_btn.TabIndex = 1;
			this.ToLobby_btn.Text = "To lobby";
			this.ToLobby_btn.UseVisualStyleBackColor = false;
			// 
			// WatchGame_btn
			// 
			this.WatchGame_btn.BackColor = System.Drawing.Color.Gray;
			this.WatchGame_btn.FlatAppearance.BorderColor = System.Drawing.Color.Yellow;
			this.WatchGame_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.WatchGame_btn.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.WatchGame_btn.Location = new System.Drawing.Point(118, 0);
			this.WatchGame_btn.Name = "WatchGame_btn";
			this.WatchGame_btn.Size = new System.Drawing.Size(100, 35);
			this.WatchGame_btn.TabIndex = 2;
			this.WatchGame_btn.Text = "Watch";
			this.WatchGame_btn.UseVisualStyleBackColor = false;
			// 
			// GameForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 261);
			this.Controls.Add(this.WatchGame_btn);
			this.Controls.Add(this.ToLobby_btn);
			this.Controls.Add(this.Message);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GameForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "GameForm";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GameForm_FormClosing);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GameForm_KeyDown);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.GameForm_KeyUp);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label Message;
		private System.Windows.Forms.Button ToLobby_btn;
		private System.Windows.Forms.Button WatchGame_btn;
	}
}