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
            this.btn_Start = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.btn_Stop = new System.Windows.Forms.Button();
            this.gb_standartButtons = new System.Windows.Forms.GroupBox();
            this.cb_Users = new System.Windows.Forms.ComboBox();
            this.btnGetListUsers = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Video)).BeginInit();
            this.gb_standartButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // pb_Video
            // 
            this.pb_Video.Location = new System.Drawing.Point(0, 0);
            this.pb_Video.Margin = new System.Windows.Forms.Padding(4);
            this.pb_Video.Name = "pb_Video";
            this.pb_Video.Size = new System.Drawing.Size(895, 492);
            this.pb_Video.TabIndex = 0;
            this.pb_Video.TabStop = false;
            // 
            // btn_Start
            // 
            this.btn_Start.Location = new System.Drawing.Point(74, 8);
            this.btn_Start.Margin = new System.Windows.Forms.Padding(4);
            this.btn_Start.Name = "btn_Start";
            this.btn_Start.Size = new System.Drawing.Size(214, 38);
            this.btn_Start.TabIndex = 1;
            this.btn_Start.Text = "Start";
            this.btn_Start.UseVisualStyleBackColor = true;
            this.btn_Start.Click += new System.EventHandler(this.btn_Start_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(541, 14);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(259, 29);
            this.comboBox1.TabIndex = 2;
            // 
            // btn_Stop
            // 
            this.btn_Stop.Location = new System.Drawing.Point(296, 8);
            this.btn_Stop.Margin = new System.Windows.Forms.Padding(4);
            this.btn_Stop.Name = "btn_Stop";
            this.btn_Stop.Size = new System.Drawing.Size(231, 38);
            this.btn_Stop.TabIndex = 3;
            this.btn_Stop.Text = "Stop";
            this.btn_Stop.UseVisualStyleBackColor = true;
            this.btn_Stop.Click += new System.EventHandler(this.btn_Stop_Click);
            // 
            // gb_standartButtons
            // 
            this.gb_standartButtons.Controls.Add(this.cb_Users);
            this.gb_standartButtons.Controls.Add(this.btnGetListUsers);
            this.gb_standartButtons.Controls.Add(this.btn_Start);
            this.gb_standartButtons.Controls.Add(this.comboBox1);
            this.gb_standartButtons.Controls.Add(this.btn_Stop);
            this.gb_standartButtons.Location = new System.Drawing.Point(0, 499);
            this.gb_standartButtons.Name = "gb_standartButtons";
            this.gb_standartButtons.Size = new System.Drawing.Size(895, 94);
            this.gb_standartButtons.TabIndex = 4;
            this.gb_standartButtons.TabStop = false;
            // 
            // cb_Users
            // 
            this.cb_Users.FormattingEnabled = true;
            this.cb_Users.Location = new System.Drawing.Point(541, 55);
            this.cb_Users.Name = "cb_Users";
            this.cb_Users.Size = new System.Drawing.Size(259, 29);
            this.cb_Users.TabIndex = 4;
            // 
            // btnGetListUsers
            // 
            this.btnGetListUsers.Location = new System.Drawing.Point(296, 49);
            this.btnGetListUsers.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetListUsers.Name = "btnGetListUsers";
            this.btnGetListUsers.Size = new System.Drawing.Size(231, 38);
            this.btnGetListUsers.TabIndex = 5;
            this.btnGetListUsers.Text = "Get list users";
            this.btnGetListUsers.UseVisualStyleBackColor = true;
            this.btnGetListUsers.Click += new System.EventHandler(this.btnGetListUsers_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(898, 605);
            this.Controls.Add(this.gb_standartButtons);
            this.Controls.Add(this.pb_Video);
            this.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VideoChat";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pb_Video)).EndInit();
            this.gb_standartButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pb_Video;
        private System.Windows.Forms.Button btn_Start;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button btn_Stop;
        private System.Windows.Forms.GroupBox gb_standartButtons;
        private System.Windows.Forms.ComboBox cb_Users;
        private System.Windows.Forms.Button btnGetListUsers;
    }
}

