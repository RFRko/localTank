namespace Tanki
{
	partial class GameOptionsForm
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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.CreateRoom_btn = new System.Windows.Forms.Button();
			this.Cancel_btn = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.GameSpeed_nud = new System.Windows.Forms.NumericUpDown();
			this.ObjectSize_nud = new System.Windows.Forms.NumericUpDown();
			this.MapWidth_nud = new System.Windows.Forms.NumericUpDown();
			this.NamberOfPlayer_nud = new System.Windows.Forms.NumericUpDown();
			this.MapHeight_nud = new System.Windows.Forms.NumericUpDown();
			this.VictoryCondition_cb = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.GameSpeed_nud)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ObjectSize_nud)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.MapWidth_nud)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.NamberOfPlayer_nud)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.MapHeight_nud)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(25, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(67, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Game speed";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(25, 66);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(59, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Object size";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(25, 111);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(49, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "Map size";
			// 
			// CreateRoom_btn
			// 
			this.CreateRoom_btn.Location = new System.Drawing.Point(94, 265);
			this.CreateRoom_btn.Name = "CreateRoom_btn";
			this.CreateRoom_btn.Size = new System.Drawing.Size(75, 23);
			this.CreateRoom_btn.TabIndex = 4;
			this.CreateRoom_btn.Text = "Create";
			this.CreateRoom_btn.UseVisualStyleBackColor = true;
			this.CreateRoom_btn.Click += new System.EventHandler(this.CreateRoom_btn_Click);
			// 
			// Cancel_btn
			// 
			this.Cancel_btn.Location = new System.Drawing.Point(188, 265);
			this.Cancel_btn.Name = "Cancel_btn";
			this.Cancel_btn.Size = new System.Drawing.Size(75, 23);
			this.Cancel_btn.TabIndex = 5;
			this.Cancel_btn.Text = "Cancel";
			this.Cancel_btn.UseVisualStyleBackColor = true;
			this.Cancel_btn.Click += new System.EventHandler(this.Cancel_btn_Click);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(25, 146);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(92, 26);
			this.label4.TabIndex = 6;
			this.label4.Text = "\r\nNumber of players";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(25, 208);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(85, 13);
			this.label5.TabIndex = 7;
			this.label5.Text = "Victory condition";
			// 
			// GameSpeed_nud
			// 
			this.GameSpeed_nud.Location = new System.Drawing.Point(143, 20);
			this.GameSpeed_nud.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.GameSpeed_nud.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.GameSpeed_nud.Name = "GameSpeed_nud";
			this.GameSpeed_nud.Size = new System.Drawing.Size(120, 20);
			this.GameSpeed_nud.TabIndex = 9;
			this.GameSpeed_nud.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
			// 
			// ObjectSize_nud
			// 
			this.ObjectSize_nud.Location = new System.Drawing.Point(143, 64);
			this.ObjectSize_nud.Minimum = new decimal(new int[] {
            32,
            0,
            0,
            0});
			this.ObjectSize_nud.Name = "ObjectSize_nud";
			this.ObjectSize_nud.Size = new System.Drawing.Size(120, 20);
			this.ObjectSize_nud.TabIndex = 10;
			this.ObjectSize_nud.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
			// 
			// MapWidth_nud
			// 
			this.MapWidth_nud.Location = new System.Drawing.Point(143, 109);
			this.MapWidth_nud.Maximum = new decimal(new int[] {
            1920,
            0,
            0,
            0});
			this.MapWidth_nud.Minimum = new decimal(new int[] {
            500,
            0,
            0,
            0});
			this.MapWidth_nud.Name = "MapWidth_nud";
			this.MapWidth_nud.Size = new System.Drawing.Size(52, 20);
			this.MapWidth_nud.TabIndex = 11;
			this.MapWidth_nud.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
			// 
			// NamberOfPlayer_nud
			// 
			this.NamberOfPlayer_nud.Location = new System.Drawing.Point(143, 152);
			this.NamberOfPlayer_nud.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
			this.NamberOfPlayer_nud.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
			this.NamberOfPlayer_nud.Name = "NamberOfPlayer_nud";
			this.NamberOfPlayer_nud.Size = new System.Drawing.Size(120, 20);
			this.NamberOfPlayer_nud.TabIndex = 12;
			this.NamberOfPlayer_nud.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			// 
			// MapHeight_nud
			// 
			this.MapHeight_nud.Location = new System.Drawing.Point(219, 109);
			this.MapHeight_nud.Maximum = new decimal(new int[] {
            1080,
            0,
            0,
            0});
			this.MapHeight_nud.Minimum = new decimal(new int[] {
            500,
            0,
            0,
            0});
			this.MapHeight_nud.Name = "MapHeight_nud";
			this.MapHeight_nud.Size = new System.Drawing.Size(44, 20);
			this.MapHeight_nud.TabIndex = 13;
			this.MapHeight_nud.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
			// 
			// VictoryCondition_cb
			// 
			this.VictoryCondition_cb.FormattingEnabled = true;
			this.VictoryCondition_cb.Location = new System.Drawing.Point(143, 205);
			this.VictoryCondition_cb.Name = "VictoryCondition_cb";
			this.VictoryCondition_cb.Size = new System.Drawing.Size(120, 21);
			this.VictoryCondition_cb.TabIndex = 14;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.ForeColor = System.Drawing.Color.Red;
			this.label6.Location = new System.Drawing.Point(25, 240);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(0, 13);
			this.label6.TabIndex = 15;
			// 
			// GameOptionsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 303);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.VictoryCondition_cb);
			this.Controls.Add(this.MapHeight_nud);
			this.Controls.Add(this.NamberOfPlayer_nud);
			this.Controls.Add(this.MapWidth_nud);
			this.Controls.Add(this.ObjectSize_nud);
			this.Controls.Add(this.GameSpeed_nud);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.Cancel_btn);
			this.Controls.Add(this.CreateRoom_btn);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GameOptionsForm";
			this.Text = "GameOptions_Form";
			this.Load += new System.EventHandler(this.GameOptionsForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.GameSpeed_nud)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ObjectSize_nud)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.MapWidth_nud)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.NamberOfPlayer_nud)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.MapHeight_nud)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button CreateRoom_btn;
		private System.Windows.Forms.Button Cancel_btn;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown GameSpeed_nud;
		private System.Windows.Forms.NumericUpDown ObjectSize_nud;
		private System.Windows.Forms.NumericUpDown MapWidth_nud;
		private System.Windows.Forms.NumericUpDown NamberOfPlayer_nud;
		private System.Windows.Forms.NumericUpDown MapHeight_nud;
		private System.Windows.Forms.ComboBox VictoryCondition_cb;
		private System.Windows.Forms.Label label6;
	}
}