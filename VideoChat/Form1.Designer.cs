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
            this.pb_Video = new System.Windows.Forms.PictureBox();
            this.gb_standartButtons = new System.Windows.Forms.GroupBox();
            this.btn_Abort_Call_Group = new System.Windows.Forms.Button();
            this.btn_Call = new System.Windows.Forms.Button();
            this.cb_Users = new System.Windows.Forms.ComboBox();
            this.btnGetListUsers = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tS_CB_Cameras = new System.Windows.Forms.ToolStripComboBox();
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
            this.gb_standartButtons.Controls.Add(this.btn_Abort_Call_Group);
            this.gb_standartButtons.Controls.Add(this.btn_Call);
            this.gb_standartButtons.Controls.Add(this.cb_Users);
            this.gb_standartButtons.Controls.Add(this.btnGetListUsers);
            this.gb_standartButtons.Location = new System.Drawing.Point(0, 448);
            this.gb_standartButtons.Name = "gb_standartButtons";
            this.gb_standartButtons.Size = new System.Drawing.Size(874, 98);
            this.gb_standartButtons.TabIndex = 4;
            this.gb_standartButtons.TabStop = false;
            // 
            // btn_Abort_Call_Group
            // 
            this.btn_Abort_Call_Group.Location = new System.Drawing.Point(487, 56);
            this.btn_Abort_Call_Group.Name = "btn_Abort_Call_Group";
            this.btn_Abort_Call_Group.Size = new System.Drawing.Size(176, 37);
            this.btn_Abort_Call_Group.TabIndex = 8;
            this.btn_Abort_Call_Group.Text = "Abort";
            this.btn_Abort_Call_Group.UseVisualStyleBackColor = true;
            this.btn_Abort_Call_Group.Click += new System.EventHandler(this.btn_Abort_Call_Group_Click);
            // 
            // btn_Call
            // 
            this.btn_Call.Location = new System.Drawing.Point(487, 9);
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
            this.cb_Users.Location = new System.Drawing.Point(187, 61);
            this.cb_Users.Name = "cb_Users";
            this.cb_Users.Size = new System.Drawing.Size(271, 29);
            this.cb_Users.TabIndex = 4;
            // 
            // btnGetListUsers
            // 
            this.btnGetListUsers.Location = new System.Drawing.Point(187, 9);
            this.btnGetListUsers.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetListUsers.Name = "btnGetListUsers";
            this.btnGetListUsers.Size = new System.Drawing.Size(271, 38);
            this.btnGetListUsers.TabIndex = 5;
            this.btnGetListUsers.Text = "Get list users";
            this.btnGetListUsers.UseVisualStyleBackColor = true;
            this.btnGetListUsers.Click += new System.EventHandler(this.btnGetListUsers_Click);
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
            this.cameraToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(81, 25);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // cameraToolStripMenuItem
            // 
            this.cameraToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tS_CB_Cameras});
            this.cameraToolStripMenuItem.Name = "cameraToolStripMenuItem";
            this.cameraToolStripMenuItem.Size = new System.Drawing.Size(137, 26);
            this.cameraToolStripMenuItem.Text = "Camera";
            // 
            // tS_CB_Cameras
            // 
            this.tS_CB_Cameras.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tS_CB_Cameras.Name = "tS_CB_Cameras";
            this.tS_CB_Cameras.Size = new System.Drawing.Size(220, 29);
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
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pb_Video;
        private System.Windows.Forms.GroupBox gb_standartButtons;
        private System.Windows.Forms.ComboBox cb_Users;
        private System.Windows.Forms.Button btnGetListUsers;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cameraToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox tS_CB_Cameras;
        private System.Windows.Forms.Button btn_Call;
        private System.Windows.Forms.Button btn_Abort_Call_Group;
    }
}

