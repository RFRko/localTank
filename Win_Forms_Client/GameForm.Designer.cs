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
			this.SuspendLayout();
			// 
			// Message
			// 
			this.Message.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Message.Font = new System.Drawing.Font("Comic Sans MS", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Message.ForeColor = System.Drawing.Color.White;
			this.Message.Location = new System.Drawing.Point(0, 0);
			this.Message.Name = "Message";
			this.Message.Size = new System.Drawing.Size(284, 261);
			this.Message.TabIndex = 0;
			this.Message.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// GameForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 261);
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

		}

		#endregion

		private System.Windows.Forms.Label Message;
	}
}