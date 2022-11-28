namespace DbWriter
{
    partial class frmMain
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
            this.grpDbConn = new System.Windows.Forms.GroupBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.edtPassword = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.cbServers = new System.Windows.Forms.ComboBox();
            this.lblAuthentication = new System.Windows.Forms.Label();
            this.lblServers = new System.Windows.Forms.Label();
            this.cbAuthentication = new System.Windows.Forms.ComboBox();
            this.edtUserName = new System.Windows.Forms.TextBox();
            this.grpLog = new System.Windows.Forms.GroupBox();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.grpOptions = new System.Windows.Forms.GroupBox();
            this.cbCollation = new System.Windows.Forms.CheckBox();
            this.cbSavelLog = new System.Windows.Forms.CheckBox();
            this.cbOmitDbo = new System.Windows.Forms.CheckBox();
            this.cbEncloseWithBrackets = new System.Windows.Forms.CheckBox();
            this.cbScriptFunctions = new System.Windows.Forms.CheckBox();
            this.cbScriptViews = new System.Windows.Forms.CheckBox();
            this.cbScriptProcedures = new System.Windows.Forms.CheckBox();
            this.cbScriptTriggers = new System.Windows.Forms.CheckBox();
            this.cbScriptTables = new System.Windows.Forms.CheckBox();
            this.edtAuthor = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbDatabase = new System.Windows.Forms.ComboBox();
            this.lblDatabase = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.edtSaveFolder = new System.Windows.Forms.TextBox();
            this.lblDestFolder = new System.Windows.Forms.Label();
            this.btnRun = new System.Windows.Forms.Button();
            this.grpDbConn.SuspendLayout();
            this.grpLog.SuspendLayout();
            this.grpOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpDbConn
            // 
            this.grpDbConn.Controls.Add(this.btnConnect);
            this.grpDbConn.Controls.Add(this.edtPassword);
            this.grpDbConn.Controls.Add(this.lblPassword);
            this.grpDbConn.Controls.Add(this.lblUserName);
            this.grpDbConn.Controls.Add(this.cbServers);
            this.grpDbConn.Controls.Add(this.lblAuthentication);
            this.grpDbConn.Controls.Add(this.lblServers);
            this.grpDbConn.Controls.Add(this.cbAuthentication);
            this.grpDbConn.Controls.Add(this.edtUserName);
            this.grpDbConn.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpDbConn.Location = new System.Drawing.Point(0, 0);
            this.grpDbConn.Name = "grpDbConn";
            this.grpDbConn.Padding = new System.Windows.Forms.Padding(4);
            this.grpDbConn.Size = new System.Drawing.Size(424, 160);
            this.grpDbConn.TabIndex = 18;
            this.grpDbConn.TabStop = false;
            this.grpDbConn.Text = "Database Connection";
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnect.Location = new System.Drawing.Point(336, 128);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(80, 23);
            this.btnConnect.TabIndex = 4;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // edtPassword
            // 
            this.edtPassword.Location = new System.Drawing.Point(176, 128);
            this.edtPassword.Name = "edtPassword";
            this.edtPassword.PasswordChar = '*';
            this.edtPassword.Size = new System.Drawing.Size(152, 22);
            this.edtPassword.TabIndex = 3;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(176, 112);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(58, 14);
            this.lblPassword.TabIndex = 7;
            this.lblPassword.Text = "Password";
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(8, 112);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(66, 14);
            this.lblUserName.TabIndex = 6;
            this.lblUserName.Text = "User Name";
            // 
            // cbServers
            // 
            this.cbServers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbServers.FormattingEnabled = true;
            this.cbServers.Location = new System.Drawing.Point(8, 40);
            this.cbServers.Name = "cbServers";
            this.cbServers.Size = new System.Drawing.Size(408, 22);
            this.cbServers.TabIndex = 0;
            this.cbServers.SelectedIndexChanged += new System.EventHandler(this.cbServers_SelectedIndexChanged);
            // 
            // lblAuthentication
            // 
            this.lblAuthentication.AutoSize = true;
            this.lblAuthentication.Location = new System.Drawing.Point(8, 64);
            this.lblAuthentication.Name = "lblAuthentication";
            this.lblAuthentication.Size = new System.Drawing.Size(88, 14);
            this.lblAuthentication.TabIndex = 3;
            this.lblAuthentication.Text = "Authentication";
            // 
            // lblServers
            // 
            this.lblServers.AutoSize = true;
            this.lblServers.Location = new System.Drawing.Point(8, 24);
            this.lblServers.Name = "lblServers";
            this.lblServers.Size = new System.Drawing.Size(77, 14);
            this.lblServers.TabIndex = 2;
            this.lblServers.Text = "Server Name";
            // 
            // cbAuthentication
            // 
            this.cbAuthentication.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbAuthentication.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAuthentication.FormattingEnabled = true;
            this.cbAuthentication.Items.AddRange(new object[] {
            "SqlServer Authentication",
            "Windows Authentication"});
            this.cbAuthentication.Location = new System.Drawing.Point(8, 80);
            this.cbAuthentication.Name = "cbAuthentication";
            this.cbAuthentication.Size = new System.Drawing.Size(408, 22);
            this.cbAuthentication.TabIndex = 1;
            this.cbAuthentication.SelectedIndexChanged += new System.EventHandler(this.cbAuthentication_SelectedIndexChanged);
            // 
            // edtUserName
            // 
            this.edtUserName.Location = new System.Drawing.Point(8, 128);
            this.edtUserName.Name = "edtUserName";
            this.edtUserName.Size = new System.Drawing.Size(160, 22);
            this.edtUserName.TabIndex = 2;
            // 
            // grpLog
            // 
            this.grpLog.Controls.Add(this.txtLog);
            this.grpLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpLog.Location = new System.Drawing.Point(0, 416);
            this.grpLog.Name = "grpLog";
            this.grpLog.Padding = new System.Windows.Forms.Padding(4);
            this.grpLog.Size = new System.Drawing.Size(424, 95);
            this.grpLog.TabIndex = 20;
            this.grpLog.TabStop = false;
            this.grpLog.Text = "Log";
            // 
            // txtLog
            // 
            this.txtLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Location = new System.Drawing.Point(4, 19);
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(416, 72);
            this.txtLog.TabIndex = 0;
            this.txtLog.Text = "";
            // 
            // grpOptions
            // 
            this.grpOptions.Controls.Add(this.cbCollation);
            this.grpOptions.Controls.Add(this.cbSavelLog);
            this.grpOptions.Controls.Add(this.cbOmitDbo);
            this.grpOptions.Controls.Add(this.cbEncloseWithBrackets);
            this.grpOptions.Controls.Add(this.cbScriptFunctions);
            this.grpOptions.Controls.Add(this.cbScriptViews);
            this.grpOptions.Controls.Add(this.cbScriptProcedures);
            this.grpOptions.Controls.Add(this.cbScriptTriggers);
            this.grpOptions.Controls.Add(this.cbScriptTables);
            this.grpOptions.Controls.Add(this.edtAuthor);
            this.grpOptions.Controls.Add(this.label1);
            this.grpOptions.Controls.Add(this.cbDatabase);
            this.grpOptions.Controls.Add(this.lblDatabase);
            this.grpOptions.Controls.Add(this.btnBrowse);
            this.grpOptions.Controls.Add(this.edtSaveFolder);
            this.grpOptions.Controls.Add(this.lblDestFolder);
            this.grpOptions.Controls.Add(this.btnRun);
            this.grpOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpOptions.Location = new System.Drawing.Point(0, 160);
            this.grpOptions.Name = "grpOptions";
            this.grpOptions.Size = new System.Drawing.Size(424, 256);
            this.grpOptions.TabIndex = 21;
            this.grpOptions.TabStop = false;
            this.grpOptions.Text = "Operation";
            // 
            // cbCollation
            // 
            this.cbCollation.AutoSize = true;
            this.cbCollation.Location = new System.Drawing.Point(184, 152);
            this.cbCollation.Name = "cbCollation";
            this.cbCollation.Size = new System.Drawing.Size(115, 18);
            this.cbCollation.TabIndex = 20;
            this.cbCollation.Text = "Include Collation";
            this.cbCollation.UseVisualStyleBackColor = true;
            // 
            // cbSavelLog
            // 
            this.cbSavelLog.AutoSize = true;
            this.cbSavelLog.Location = new System.Drawing.Point(184, 200);
            this.cbSavelLog.Name = "cbSavelLog";
            this.cbSavelLog.Size = new System.Drawing.Size(113, 18);
            this.cbSavelLog.TabIndex = 19;
            this.cbSavelLog.Text = "Save Log to File";
            this.cbSavelLog.UseVisualStyleBackColor = true;
            // 
            // cbOmitDbo
            // 
            this.cbOmitDbo.AutoSize = true;
            this.cbOmitDbo.Location = new System.Drawing.Point(184, 184);
            this.cbOmitDbo.Name = "cbOmitDbo";
            this.cbOmitDbo.Size = new System.Drawing.Size(145, 18);
            this.cbOmitDbo.TabIndex = 18;
            this.cbOmitDbo.Text = "Omit dbo from names";
            this.cbOmitDbo.UseVisualStyleBackColor = true;
            // 
            // cbEncloseWithBrackets
            // 
            this.cbEncloseWithBrackets.AutoSize = true;
            this.cbEncloseWithBrackets.Location = new System.Drawing.Point(184, 168);
            this.cbEncloseWithBrackets.Name = "cbEncloseWithBrackets";
            this.cbEncloseWithBrackets.Size = new System.Drawing.Size(152, 18);
            this.cbEncloseWithBrackets.TabIndex = 17;
            this.cbEncloseWithBrackets.Text = "Enclose names with [ ]";
            this.cbEncloseWithBrackets.UseVisualStyleBackColor = true;
            // 
            // cbScriptFunctions
            // 
            this.cbScriptFunctions.AutoSize = true;
            this.cbScriptFunctions.Location = new System.Drawing.Point(8, 216);
            this.cbScriptFunctions.Name = "cbScriptFunctions";
            this.cbScriptFunctions.Size = new System.Drawing.Size(113, 18);
            this.cbScriptFunctions.TabIndex = 16;
            this.cbScriptFunctions.Text = "Script Functions";
            this.cbScriptFunctions.UseVisualStyleBackColor = true;
            // 
            // cbScriptViews
            // 
            this.cbScriptViews.AutoSize = true;
            this.cbScriptViews.Location = new System.Drawing.Point(8, 184);
            this.cbScriptViews.Name = "cbScriptViews";
            this.cbScriptViews.Size = new System.Drawing.Size(93, 18);
            this.cbScriptViews.TabIndex = 14;
            this.cbScriptViews.Text = "Script Views";
            this.cbScriptViews.UseVisualStyleBackColor = true;
            // 
            // cbScriptProcedures
            // 
            this.cbScriptProcedures.AutoSize = true;
            this.cbScriptProcedures.Location = new System.Drawing.Point(8, 200);
            this.cbScriptProcedures.Name = "cbScriptProcedures";
            this.cbScriptProcedures.Size = new System.Drawing.Size(122, 18);
            this.cbScriptProcedures.TabIndex = 15;
            this.cbScriptProcedures.Text = "Script Procedures";
            this.cbScriptProcedures.UseVisualStyleBackColor = true;
            // 
            // cbScriptTriggers
            // 
            this.cbScriptTriggers.AutoSize = true;
            this.cbScriptTriggers.Location = new System.Drawing.Point(8, 168);
            this.cbScriptTriggers.Name = "cbScriptTriggers";
            this.cbScriptTriggers.Size = new System.Drawing.Size(70, 18);
            this.cbScriptTriggers.TabIndex = 13;
            this.cbScriptTriggers.Text = "Triggers";
            this.cbScriptTriggers.UseVisualStyleBackColor = true;
            // 
            // cbScriptTables
            // 
            this.cbScriptTables.AutoSize = true;
            this.cbScriptTables.Location = new System.Drawing.Point(8, 152);
            this.cbScriptTables.Name = "cbScriptTables";
            this.cbScriptTables.Size = new System.Drawing.Size(96, 18);
            this.cbScriptTables.TabIndex = 11;
            this.cbScriptTables.Text = "Script Tables";
            this.cbScriptTables.UseVisualStyleBackColor = true;
            // 
            // edtAuthor
            // 
            this.edtAuthor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.edtAuthor.Location = new System.Drawing.Point(8, 80);
            this.edtAuthor.Name = "edtAuthor";
            this.edtAuthor.Size = new System.Drawing.Size(408, 22);
            this.edtAuthor.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 14);
            this.label1.TabIndex = 9;
            this.label1.Text = "Author";
            // 
            // cbDatabase
            // 
            this.cbDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDatabase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDatabase.FormattingEnabled = true;
            this.cbDatabase.Location = new System.Drawing.Point(8, 40);
            this.cbDatabase.Name = "cbDatabase";
            this.cbDatabase.Size = new System.Drawing.Size(408, 22);
            this.cbDatabase.TabIndex = 0;
            // 
            // lblDatabase
            // 
            this.lblDatabase.AutoSize = true;
            this.lblDatabase.Location = new System.Drawing.Point(8, 24);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Size = new System.Drawing.Size(57, 14);
            this.lblDatabase.TabIndex = 5;
            this.lblDatabase.Text = "Database";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(394, 121);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(21, 20);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "…";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // edtSaveFolder
            // 
            this.edtSaveFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.edtSaveFolder.Location = new System.Drawing.Point(8, 120);
            this.edtSaveFolder.Name = "edtSaveFolder";
            this.edtSaveFolder.Size = new System.Drawing.Size(408, 22);
            this.edtSaveFolder.TabIndex = 1;
            // 
            // lblDestFolder
            // 
            this.lblDestFolder.AutoSize = true;
            this.lblDestFolder.Location = new System.Drawing.Point(8, 104);
            this.lblDestFolder.Name = "lblDestFolder";
            this.lblDestFolder.Size = new System.Drawing.Size(70, 14);
            this.lblDestFolder.TabIndex = 1;
            this.lblDestFolder.Text = "Save Folder";
            // 
            // btnRun
            // 
            this.btnRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRun.Location = new System.Drawing.Point(336, 232);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(80, 23);
            this.btnRun.TabIndex = 4;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(424, 511);
            this.Controls.Add(this.grpLog);
            this.Controls.Add(this.grpOptions);
            this.Controls.Add(this.grpDbConn);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.MinimumSize = new System.Drawing.Size(440, 550);
            this.Name = "frmMain";
            this.Text = "Db Documents";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.grpDbConn.ResumeLayout(false);
            this.grpDbConn.PerformLayout();
            this.grpLog.ResumeLayout(false);
            this.grpOptions.ResumeLayout(false);
            this.grpOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpDbConn;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox edtPassword;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.ComboBox cbServers;
        private System.Windows.Forms.Label lblAuthentication;
        private System.Windows.Forms.Label lblServers;
        private System.Windows.Forms.ComboBox cbAuthentication;
        private System.Windows.Forms.TextBox edtUserName;
        private System.Windows.Forms.GroupBox grpLog;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.GroupBox grpOptions;
        private System.Windows.Forms.CheckBox cbScriptFunctions;
        private System.Windows.Forms.CheckBox cbScriptViews;
        private System.Windows.Forms.CheckBox cbScriptProcedures;
        private System.Windows.Forms.CheckBox cbScriptTriggers;
        private System.Windows.Forms.CheckBox cbScriptTables;
        private System.Windows.Forms.TextBox edtAuthor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbDatabase;
        private System.Windows.Forms.Label lblDatabase;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox edtSaveFolder;
        private System.Windows.Forms.Label lblDestFolder;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.CheckBox cbSavelLog;
        private System.Windows.Forms.CheckBox cbOmitDbo;
        private System.Windows.Forms.CheckBox cbEncloseWithBrackets;
        private System.Windows.Forms.CheckBox cbCollation;
    }
}

