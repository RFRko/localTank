namespace Tanki
{
	partial class Lobby
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
			this.Create_Room_btn = new System.Windows.Forms.Button();
			this.Conect_btn = new System.Windows.Forms.Button();
			this.Refresh_btn = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.Name_tb = new System.Windows.Forms.TextBox();
			this.DGV_RoomList = new System.Windows.Forms.DataGridView();
			this.label3 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.DGV_RoomList)).BeginInit();
			this.SuspendLayout();
			// 
			// Create_Room_btn
			// 
			this.Create_Room_btn.Location = new System.Drawing.Point(225, 216);
			this.Create_Room_btn.Name = "Create_Room_btn";
			this.Create_Room_btn.Size = new System.Drawing.Size(89, 23);
			this.Create_Room_btn.TabIndex = 0;
			this.Create_Room_btn.Text = "Создать";
			this.Create_Room_btn.UseVisualStyleBackColor = true;
			this.Create_Room_btn.Click += new System.EventHandler(this.Create_Room_btn_Click);
			// 
			// Conect_btn
			// 
			this.Conect_btn.Location = new System.Drawing.Point(330, 216);
			this.Conect_btn.Name = "Conect_btn";
			this.Conect_btn.Size = new System.Drawing.Size(89, 23);
			this.Conect_btn.TabIndex = 1;
			this.Conect_btn.Text = "Подключится";
			this.Conect_btn.UseVisualStyleBackColor = true;
			this.Conect_btn.Click += new System.EventHandler(this.Conect_btn_Click);
			// 
			// Refresh_btn
			// 
			this.Refresh_btn.Location = new System.Drawing.Point(344, 3);
			this.Refresh_btn.Name = "Refresh_btn";
			this.Refresh_btn.Size = new System.Drawing.Size(75, 23);
			this.Refresh_btn.TabIndex = 2;
			this.Refresh_btn.Text = "Обновить";
			this.Refresh_btn.UseVisualStyleBackColor = true;
			this.Refresh_btn.Click += new System.EventHandler(this.Refresh_btn_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(18, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Список игр";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(18, 221);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(57, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Ваше имя";
			// 
			// Name_tb
			// 
			this.Name_tb.Location = new System.Drawing.Point(92, 218);
			this.Name_tb.Name = "Name_tb";
			this.Name_tb.Size = new System.Drawing.Size(118, 20);
			this.Name_tb.TabIndex = 5;
			// 
			// DGV_RoomList
			// 
			this.DGV_RoomList.AllowUserToAddRows = false;
			this.DGV_RoomList.AllowUserToDeleteRows = false;
			this.DGV_RoomList.AllowUserToResizeColumns = false;
			this.DGV_RoomList.AllowUserToResizeRows = false;
			this.DGV_RoomList.BackgroundColor = System.Drawing.SystemColors.ScrollBar;
			this.DGV_RoomList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.DGV_RoomList.Location = new System.Drawing.Point(12, 32);
			this.DGV_RoomList.MultiSelect = false;
			this.DGV_RoomList.Name = "DGV_RoomList";
			this.DGV_RoomList.ReadOnly = true;
			this.DGV_RoomList.RowHeadersVisible = false;
			this.DGV_RoomList.RowHeadersWidth = 40;
			this.DGV_RoomList.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			this.DGV_RoomList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.DGV_RoomList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.DGV_RoomList.Size = new System.Drawing.Size(407, 152);
			this.DGV_RoomList.TabIndex = 6;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.ForeColor = System.Drawing.Color.Red;
			this.label3.Location = new System.Drawing.Point(94, 196);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(0, 13);
			this.label3.TabIndex = 7;
			// 
			// Lobby
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.ClientSize = new System.Drawing.Size(431, 250);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.DGV_RoomList);
			this.Controls.Add(this.Name_tb);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.Refresh_btn);
			this.Controls.Add(this.Conect_btn);
			this.Controls.Add(this.Create_Room_btn);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Lobby";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Lobby";
			((System.ComponentModel.ISupportInitialize)(this.DGV_RoomList)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button Create_Room_btn;
		private System.Windows.Forms.Button Conect_btn;
		private System.Windows.Forms.Button Refresh_btn;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox Name_tb;
		private System.Windows.Forms.DataGridView DGV_RoomList;
		private System.Windows.Forms.Label label3;
	}
}