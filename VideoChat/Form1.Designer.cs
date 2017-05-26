namespace VideoChat
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pb_Video = new System.Windows.Forms.PictureBox();
            this.gb_standartButtons = new System.Windows.Forms.GroupBox();
            this.lblUserName = new System.Windows.Forms.Label();
            this.btn_Abort_Call_Group = new System.Windows.Forms.Button();
            this.btn_Call = new System.Windows.Forms.Button();
            this.cb_Users = new System.Windows.Forms.ComboBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tS_CB_Cameras = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.newNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateListUsersTimer = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Video)).BeginInit();
            this.gb_standartButtons.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pb_Video
            // 
            this.pb_Video.Location = new System.Drawing.Point(0, 33);
            this.pb_Video.Margin = new System.Windows.Forms.Padding(4);
            this.pb_Video.Name = "pb_Video";
            this.pb_Video.Size = new System.Drawing.Size(874, 408);
            this.pb_Video.TabIndex = 0;
            this.pb_Video.TabStop = false;
            // 
            // gb_standartButtons
            // 
            this.gb_standartButtons.Controls.Add(this.label1);
            this.gb_standartButtons.Controls.Add(this.lblUserName);
            this.gb_standartButtons.Controls.Add(this.btn_Abort_Call_Group);
            this.gb_standartButtons.Controls.Add(this.btn_Call);
            this.gb_standartButtons.Controls.Add(this.cb_Users);
            this.gb_standartButtons.Location = new System.Drawing.Point(0, 448);
            this.gb_standartButtons.Name = "gb_standartButtons";
            this.gb_standartButtons.Size = new System.Drawing.Size(874, 98);
            this.gb_standartButtons.TabIndex = 4;
            this.gb_standartButtons.TabStop = false;
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Font = new System.Drawing.Font("Segoe Marker", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserName.Location = new System.Drawing.Point(12, 64);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(0, 31);
            this.lblUserName.TabIndex = 9;
            // 
            // btn_Abort_Call_Group
            // 
            this.btn_Abort_Call_Group.Location = new System.Drawing.Point(657, 56);
            this.btn_Abort_Call_Group.Name = "btn_Abort_Call_Group";
            this.btn_Abort_Call_Group.Size = new System.Drawing.Size(176, 37);
            this.btn_Abort_Call_Group.TabIndex = 8;
            this.btn_Abort_Call_Group.Text = "Abort";
            this.btn_Abort_Call_Group.UseVisualStyleBackColor = true;
            this.btn_Abort_Call_Group.Click += new System.EventHandler(this.btn_Abort_Call_Group_Click);
            // 
            // btn_Call
            // 
            this.btn_Call.Location = new System.Drawing.Point(657, 9);
            this.btn_Call.Name = "btn_Call";
            this.btn_Call.Size = new System.Drawing.Size(176, 38);
            this.btn_Call.TabIndex = 6;
            this.btn_Call.Text = "Call";
            this.btn_Call.UseVisualStyleBackColor = true;
            this.btn_Call.Click += new System.EventHandler(this.btn_Call_Click);
            // 
            // cb_Users
            // 
            this.cb_Users.FormattingEnabled = true;
            this.cb_Users.Location = new System.Drawing.Point(168, 15);
            this.cb_Users.Name = "cb_Users";
            this.cb_Users.Size = new System.Drawing.Size(466, 29);
            this.cb_Users.TabIndex = 4;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(874, 29);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cameraToolStripMenuItem,
            this.newNameToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(81, 25);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // cameraToolStripMenuItem
            // 
            this.cameraToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tS_CB_Cameras,
            this.toolStripMenuItem1});
            this.cameraToolStripMenuItem.Name = "cameraToolStripMenuItem";
            this.cameraToolStripMenuItem.Size = new System.Drawing.Size(158, 26);
            this.cameraToolStripMenuItem.Text = "Camera";
            // 
            // tS_CB_Cameras
            // 
            this.tS_CB_Cameras.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tS_CB_Cameras.Name = "tS_CB_Cameras";
            this.tS_CB_Cameras.Size = new System.Drawing.Size(220, 29);
            this.tS_CB_Cameras.SelectedIndexChanged += new System.EventHandler(this.tS_CB_Cameras_SelectedIndexChanged);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(280, 26);
            this.toolStripMenuItem1.Text = "Update";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // newNameToolStripMenuItem
            // 
            this.newNameToolStripMenuItem.Name = "newNameToolStripMenuItem";
            this.newNameToolStripMenuItem.Size = new System.Drawing.Size(158, 26);
            this.newNameToolStripMenuItem.Text = "New name";
            this.newNameToolStripMenuItem.Click += new System.EventHandler(this.newNameToolStripMenuItem_Click);
            // 
            // updateListUsersTimer
            // 
            this.updateListUsersTimer.Interval = 1000;
            this.updateListUsersTimer.Tick += new System.EventHandler(this.updateListUsersTimer_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(105, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 21);
            this.label1.TabIndex = 10;
            this.label1.Text = "Users:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(874, 550);
            this.Controls.Add(this.gb_standartButtons);
            this.Controls.Add(this.pb_Video);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VideoChat";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pb_Video)).EndInit();
            this.gb_standartButtons.ResumeLayout(false);
            this.gb_standartButtons.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pb_Video;
        private System.Windows.Forms.GroupBox gb_standartButtons;
        private System.Windows.Forms.ComboBox cb_Users;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cameraToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox tS_CB_Cameras;
        private System.Windows.Forms.Button btn_Call;
        private System.Windows.Forms.Button btn_Abort_Call_Group;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.Timer updateListUsersTimer;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.ToolStripMenuItem newNameToolStripMenuItem;
        private System.Windows.Forms.Label label1;
    }
}

