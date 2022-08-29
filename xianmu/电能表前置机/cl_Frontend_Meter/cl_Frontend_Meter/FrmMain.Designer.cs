namespace cl_Frontend_Meter
{
    partial class FrmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.chk_IsLog = new System.Windows.Forms.CheckBox();
            this.tabTableLog = new System.Windows.Forms.TabControl();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.tabPage8 = new System.Windows.Forms.TabPage();
            this.tabPage9 = new System.Windows.Forms.TabPage();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.dgv_Config = new System.Windows.Forms.DataGridView();
            this.lab_Prompting = new System.Windows.Forms.Label();
            this.btn_SaveData = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.lab_TableCount = new System.Windows.Forms.Label();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItem_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.Column1 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabTableLog.SuspendLayout();
            this.tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Config)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Location = new System.Drawing.Point(4, 4);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1229, 505);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.chk_IsLog);
            this.tabPage1.Controls.Add(this.tabTableLog);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage1.Size = new System.Drawing.Size(1221, 476);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "  通信报文  ";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // chk_IsLog
            // 
            this.chk_IsLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chk_IsLog.AutoSize = true;
            this.chk_IsLog.Location = new System.Drawing.Point(1096, 446);
            this.chk_IsLog.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chk_IsLog.Name = "chk_IsLog";
            this.chk_IsLog.Size = new System.Drawing.Size(119, 19);
            this.chk_IsLog.TabIndex = 64;
            this.chk_IsLog.Text = "报文生成文件";
            this.chk_IsLog.UseVisualStyleBackColor = true;
            // 
            // tabTableLog
            // 
            this.tabTableLog.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabTableLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabTableLog.Controls.Add(this.tabPage6);
            this.tabTableLog.Controls.Add(this.tabPage7);
            this.tabTableLog.Controls.Add(this.tabPage8);
            this.tabTableLog.Controls.Add(this.tabPage9);
            this.tabTableLog.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.tabTableLog.ItemSize = new System.Drawing.Size(48, 17);
            this.tabTableLog.Location = new System.Drawing.Point(8, 8);
            this.tabTableLog.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabTableLog.Name = "tabTableLog";
            this.tabTableLog.SelectedIndex = 0;
            this.tabTableLog.Size = new System.Drawing.Size(1203, 422);
            this.tabTableLog.TabIndex = 58;
            // 
            // tabPage6
            // 
            this.tabPage6.Location = new System.Drawing.Point(4, 4);
            this.tabPage6.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage6.Size = new System.Drawing.Size(1195, 397);
            this.tabPage6.TabIndex = 0;
            this.tabPage6.Text = "表01";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // tabPage7
            // 
            this.tabPage7.Location = new System.Drawing.Point(4, 4);
            this.tabPage7.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Size = new System.Drawing.Size(1195, 397);
            this.tabPage7.TabIndex = 1;
            this.tabPage7.Text = "表02";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // tabPage8
            // 
            this.tabPage8.Location = new System.Drawing.Point(4, 4);
            this.tabPage8.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage8.Name = "tabPage8";
            this.tabPage8.Size = new System.Drawing.Size(1195, 397);
            this.tabPage8.TabIndex = 2;
            this.tabPage8.Text = "表03";
            this.tabPage8.UseVisualStyleBackColor = true;
            // 
            // tabPage9
            // 
            this.tabPage9.Location = new System.Drawing.Point(4, 4);
            this.tabPage9.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage9.Name = "tabPage9";
            this.tabPage9.Size = new System.Drawing.Size(1195, 397);
            this.tabPage9.TabIndex = 3;
            this.tabPage9.Text = "表04";
            this.tabPage9.UseVisualStyleBackColor = true;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.button2);
            this.tabPage5.Controls.Add(this.button1);
            this.tabPage5.Controls.Add(this.dgv_Config);
            this.tabPage5.Controls.Add(this.lab_Prompting);
            this.tabPage5.Controls.Add(this.btn_SaveData);
            this.tabPage5.Controls.Add(this.textBox1);
            this.tabPage5.Controls.Add(this.lab_TableCount);
            this.tabPage5.Location = new System.Drawing.Point(4, 25);
            this.tabPage5.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage5.Size = new System.Drawing.Size(1221, 476);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "表位设置  ";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(27, 180);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(133, 29);
            this.button2.TabIndex = 20;
            this.button2.Text = "删除选中数据";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(27, 125);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(133, 29);
            this.button1.TabIndex = 19;
            this.button1.Text = "增加一行数据";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // dgv_Config
            // 
            this.dgv_Config.AllowUserToAddRows = false;
            this.dgv_Config.AllowUserToDeleteRows = false;
            this.dgv_Config.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv_Config.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dgv_Config.ColumnHeadersHeight = 30;
            this.dgv_Config.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5,
            this.Column6,
            this.Column7,
            this.Column8});
            this.dgv_Config.Location = new System.Drawing.Point(305, 8);
            this.dgv_Config.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dgv_Config.Name = "dgv_Config";
            this.dgv_Config.RowHeadersWidth = 51;
            this.dgv_Config.RowTemplate.Height = 23;
            this.dgv_Config.Size = new System.Drawing.Size(905, 458);
            this.dgv_Config.TabIndex = 18;
            this.dgv_Config.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.Dgv_Config_CellValueChanged);
            // 
            // lab_Prompting
            // 
            this.lab_Prompting.AutoSize = true;
            this.lab_Prompting.Font = new System.Drawing.Font("宋体", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_Prompting.ForeColor = System.Drawing.Color.Red;
            this.lab_Prompting.Location = new System.Drawing.Point(44, 109);
            this.lab_Prompting.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lab_Prompting.Name = "lab_Prompting";
            this.lab_Prompting.Size = new System.Drawing.Size(0, 13);
            this.lab_Prompting.TabIndex = 11;
            // 
            // btn_SaveData
            // 
            this.btn_SaveData.Location = new System.Drawing.Point(27, 239);
            this.btn_SaveData.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btn_SaveData.Name = "btn_SaveData";
            this.btn_SaveData.Size = new System.Drawing.Size(133, 29);
            this.btn_SaveData.TabIndex = 10;
            this.btn_SaveData.Text = "保存";
            this.btn_SaveData.UseVisualStyleBackColor = true;
            this.btn_SaveData.Click += new System.EventHandler(this.Btn_SaveData_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(108, 58);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(160, 25);
            this.textBox1.TabIndex = 8;
            // 
            // lab_TableCount
            // 
            this.lab_TableCount.AutoSize = true;
            this.lab_TableCount.Location = new System.Drawing.Point(24, 61);
            this.lab_TableCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lab_TableCount.Name = "lab_TableCount";
            this.lab_TableCount.Size = new System.Drawing.Size(60, 15);
            this.lab_TableCount.TabIndex = 7;
            this.lab_TableCount.Text = "表位数:";
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "前置机程序";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon1_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Exit});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(109, 28);
            // 
            // ToolStripMenuItem_Exit
            // 
            this.ToolStripMenuItem_Exit.Name = "ToolStripMenuItem_Exit";
            this.ToolStripMenuItem_Exit.Size = new System.Drawing.Size(108, 24);
            this.ToolStripMenuItem_Exit.Text = "退出";
            this.ToolStripMenuItem_Exit.Click += new System.EventHandler(this.ToolStripMenuItem_Exit_Click);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "设备类型";
            this.Column1.Items.AddRange(new object[] {
            "cl303",
            "cl309",
            "cl3115",
            "cl3112",
            "cl311v2",
            "cl188m",
            "cl188l",
            "cl191B",
            "cl2029D",
            "上行485"});
            this.Column1.MinimumWidth = 6;
            this.Column1.Name = "Column1";
            this.Column1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column1.Width = 125;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "服务器IP";
            this.Column2.MinimumWidth = 6;
            this.Column2.Name = "Column2";
            this.Column2.Width = 125;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "起始端口";
            this.Column3.MinimumWidth = 6;
            this.Column3.Name = "Column3";
            this.Column3.Width = 125;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "远程端口";
            this.Column4.MinimumWidth = 6;
            this.Column4.Name = "Column4";
            this.Column4.Width = 125;
            // 
            // Column5
            // 
            this.Column5.HeaderText = "端口号";
            this.Column5.MinimumWidth = 6;
            this.Column5.Name = "Column5";
            this.Column5.Width = 125;
            // 
            // Column6
            // 
            this.Column6.HeaderText = "服务器类型";
            this.Column6.Items.AddRange(new object[] {
            "2018-电能",
            "2018-负控",
            "串口"});
            this.Column6.MinimumWidth = 6;
            this.Column6.Name = "Column6";
            this.Column6.Width = 125;
            // 
            // Column7
            // 
            this.Column7.HeaderText = "帧最大间隔";
            this.Column7.MinimumWidth = 6;
            this.Column7.Name = "Column7";
            this.Column7.Width = 125;
            // 
            // Column8
            // 
            this.Column8.HeaderText = "字节间间隔";
            this.Column8.MinimumWidth = 6;
            this.Column8.Name = "Column8";
            this.Column8.Width = 125;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1248, 538);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MinimumSize = new System.Drawing.Size(1253, 559);
            this.Name = "FrmMain";
            this.Text = "电表前置机(2018-03-21)";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.Resize += new System.EventHandler(this.FrmMain_Resize);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabTableLog.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Config)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TabControl tabTableLog;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.TabPage tabPage8;
        private System.Windows.Forms.TabPage tabPage9;
        private System.Windows.Forms.CheckBox chk_IsLog;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label lab_TableCount;
        private System.Windows.Forms.Button btn_SaveData;
        private System.Windows.Forms.Label lab_Prompting;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Exit;
        private System.Windows.Forms.DataGridView dgv_Config;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridViewComboBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewComboBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
    }
}

