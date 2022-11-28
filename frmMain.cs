using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DbWriter {

    delegate void LoggerDelegate(string Message, Color? color = null);

    public partial class frmMain: Form {

        private const string AppName = "DbWriter";
        private const string AppTitle = "Db Writer";

        private const string AppRegistryKey = @"Software\JGhost\" + AppName;
        private const string Key = "Sd6Ci36qT3t6VTUv/62TGQ==";
        private bool Running = false;
        private SqlConnection DbConn = new SqlConnection();
        private List<ServerSettings> Servers;
        private string LastSelectedDatabase = "";

        #region public frmMain()
        public frmMain() {
            InitializeComponent();
        }
        #endregion

        #region private void Log(...)
        private void Log(string Message, Color? color) {
            if (this.InvokeRequired) {
                this.Invoke((MethodInvoker) delegate {
                    _Log(Message, color);
                });
            } else {
                _Log(Message, color);
            }
        }
        private void _Log(string Message, Color? color) {
            if (txtLog.IsDisposed)
                return;
            txtLog.SelectionColor = color == null ? Color.Black : color.Value;
            txtLog.AppendText(Message + "\n");
            Control control = this.ActiveControl;
            txtLog.Focus();
            // Return focus back to the control that had focus before
            control.Focus();
            Application.DoEvents();
        }
        #endregion

        #region private void ReadDatabases()
        private void ReadDatabases() {
            using (SqlDataAdapter adapter = new SqlDataAdapter("sp_databases", DbConn)) {
                DataTable table = new DataTable();
                adapter.Fill(table);
                cbDatabase.Items.Clear();
                if (table != null && table.Rows.Count > 0) {
                    foreach (DataRow row in table.Rows) {
                        cbDatabase.Items.Add(row["database_name"].ToString());
                    }
                }
            }
            if (cbDatabase.Items.Count > 0)
                cbDatabase.SelectedIndex = 0;
            if (LastSelectedDatabase != "" && cbDatabase.Items.IndexOf(LastSelectedDatabase) > -1)
                cbDatabase.SelectedIndex = cbDatabase.Items.IndexOf(LastSelectedDatabase);
        }
        #endregion


        #region private void frmMain_Load(...)
        private void frmMain_Load(object sender, EventArgs e) {
            this.Icon = Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location);
            this.Text = AppTitle;
            grpOptions.Enabled = false;
            grpDbConn.Enabled = true;

            //Log("Reading Network SQL Servers ...", Color.Blue);
            //FillDbServersList();

            Servers = ServerList.List(AppRegistryKey);
            foreach (ServerSettings settings in Servers)
                cbServers.Items.Add(settings.Server);
            if (cbServers.Items.Count > 0)
                cbServers.SelectedIndex = 0;

            cbAuthentication.SelectedIndex = Reg.Read(AppRegistryKey, "Authentication", 0);
            edtUserName.Text = Reg.Read(AppRegistryKey, "user", edtUserName.Text);
            edtPassword.Text = Reg.Read(AppRegistryKey, "pass", edtPassword.Text);

            edtAuthor.Text = Reg.Read(AppRegistryKey, "Author", edtAuthor.Text);
            edtSaveFolder.Text = Reg.Read(AppRegistryKey, "SaveFolder", Application.StartupPath.AddTrailingBackSlashes());

            cbScriptTables.Checked = Reg.Read(AppRegistryKey, "ScriptTables", 1) == 1;
            cbScriptTriggers.Checked = Reg.Read(AppRegistryKey, "ScriptTriggers", 1) == 1;
            cbScriptViews.Checked = Reg.Read(AppRegistryKey, "ScriptViews", 1) == 1;
            cbScriptProcedures.Checked = Reg.Read(AppRegistryKey, "ScriptProcedures", 1) == 1;
            cbScriptFunctions.Checked = Reg.Read(AppRegistryKey, "ScriptFunctions", 1) == 1;

            cbCollation.Checked = Reg.Read(AppRegistryKey, "Collation", 0) == 1;
            cbEncloseWithBrackets.Checked = Reg.Read(AppRegistryKey, "EncloseWithBrackets", 0) == 1;
            cbOmitDbo.Checked = Reg.Read(AppRegistryKey, "OmitDbo", 1) == 1;
            cbSavelLog.Checked = Reg.Read(AppRegistryKey, "SavelLog", 0) == 1;
            Reg.LoadWindowPos(AppRegistryKey, this);
        }
        #endregion

        #region private void frmMain_FormClosed(...)
        private void frmMain_FormClosed(object sender, FormClosedEventArgs e) {
            Reg.SaveWindowPos(AppRegistryKey, this);

            Reg.Write(AppRegistryKey, "Author", edtAuthor.Text);
            Reg.Write(AppRegistryKey, "SaveFolder", edtSaveFolder.Text);

            Reg.Write(AppRegistryKey, "ScriptTables", cbScriptTables.Checked ? 1 : 0);
            Reg.Write(AppRegistryKey, "ScriptTriggers", cbScriptTriggers.Checked ? 1 : 0);
            Reg.Write(AppRegistryKey, "ScriptViews", cbScriptViews.Checked ? 1 : 0);
            Reg.Write(AppRegistryKey, "ScriptProcedures", cbScriptProcedures.Checked ? 1 : 0);
            Reg.Write(AppRegistryKey, "ScriptFunctions", cbScriptFunctions.Checked ? 1 : 0);

            Reg.Write(AppRegistryKey, "Collation", cbCollation.Checked ? 1 : 0);
            Reg.Write(AppRegistryKey, "EncloseWithBrackets", cbEncloseWithBrackets.Checked ? 1 : 0);
            Reg.Write(AppRegistryKey, "OmitDbo", cbOmitDbo.Checked ? 1 : 0);
            Reg.Write(AppRegistryKey, "SavelLog", cbSavelLog.Checked ? 1 : 0);
        }
        #endregion

        #region private void cbServers_SelectedIndexChanged(...)
        private void cbServers_SelectedIndexChanged(object sender, EventArgs e) {
            // Read database configuration when selected server changes
            if (cbServers.Text != "") {
                foreach (ServerSettings setting in Servers) {
                    if (cbServers.Text == setting.Server) {
                        cbAuthentication.SelectedIndex = setting.Authentication;
                        edtUserName.Text = setting.Username;
                        try {
                            edtPassword.Text = Crypto.AES.Decrypt(setting.Password, Key);
                        } catch {
                            edtPassword.Text = "";
                        }
                        LastSelectedDatabase = setting.LastDatabaseName;
                    }
                }
            }
        }
        #endregion

        #region private void cbAuthentication_SelectedIndexChanged(...)
        private void cbAuthentication_SelectedIndexChanged(object sender, EventArgs e) {
            if (cbAuthentication.SelectedIndex == 0) {
                edtPassword.Enabled = true;
                edtUserName.Enabled = true;
                lblPassword.Enabled = true;
                lblUserName.Enabled = true;
            } else {
                edtPassword.Enabled = false;
                edtUserName.Enabled = false;
                lblPassword.Enabled = false;
                lblUserName.Enabled = false;
            }
        }
        #endregion

        #region private void btnBrowse_Click(...)
        private void btnBrowse_Click(object sender, EventArgs e) {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowNewFolderButton = true;
            if (Directory.Exists(edtSaveFolder.Text)) {
                dialog.SelectedPath = edtSaveFolder.Text;
            }
            if (dialog.ShowDialog(this) == DialogResult.OK) {
                edtSaveFolder.Text = dialog.SelectedPath.AddTrailingBackSlashes();
            }
        }
        #endregion

        #region private void btnConnect_Click(...)
        private void btnConnect_Click(object sender, EventArgs e) {
            try {
                if (DbConn.State != ConnectionState.Open) {
                    Log("Trying to connect to " + cbServers.Text + " ...", Color.Blue);
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                    if (cbAuthentication.SelectedIndex == 0) {
                        builder.DataSource = cbServers.Text;
                        builder.UserID = edtUserName.Text;
                        builder.Password = edtPassword.Text;
                        builder.IntegratedSecurity = false;

                    } else {
                        builder["integrated Security"] = true;
                    }
                    builder["Initial Catalog"] = "master";
                    DbConn.ConnectionString = builder.ConnectionString;
                    DbConn.Open();

                    #region Save connected server to registry
                    ServerList.Update(AppRegistryKey, new ServerSettings(
                        cbServers.Text,
                        cbAuthentication.SelectedIndex,
                        edtUserName.Text,
                        Crypto.AES.Encrypt(edtPassword.Text, Key)
                    ));
                    Servers = ServerList.List(AppRegistryKey);
                    #endregion

                    grpOptions.Enabled = true;
                    ReadDatabases();

                    cbServers.Enabled = false;
                    cbAuthentication.Enabled = false;
                    edtUserName.Enabled = false;
                    edtPassword.Enabled = false;
                    lblUserName.Enabled = false;
                    lblPassword.Enabled = false;
                    lblAuthentication.Enabled = false;
                    lblServers.Enabled = false;

                    Log("  Connected", Color.Black);
                    btnConnect.Text = "Disconnect";
                } else {
                    DbConn.Close();
                    btnConnect.Text = "Connect";
                    Log("Server Disconnected", Color.Black);
                    grpOptions.Enabled = false;

                    lblAuthentication.Enabled = true;
                    lblServers.Enabled = true;
                    cbServers.Enabled = true;
                    cbAuthentication.Enabled = true;
                    cbAuthentication_SelectedIndexChanged(null, null);
                }
            } catch (Exception ex) {
                Log(ex.Message, Color.Red);
            }
        }
        #endregion

        #region private async void btnRun_Click(...)
        private async void btnRun_Click(object sender, EventArgs e) {
            try {
                if (Running)
                    return;
                Running = true;
                grpOptions.Enabled = false;
                txtLog.Clear();
                edtSaveFolder.Text = edtSaveFolder.Text.AddTrailingBackSlashes();

                #region Save connected server to registry
                ServerList.Update(AppRegistryKey, new ServerSettings(
                    cbServers.Text,
                    cbAuthentication.SelectedIndex,
                    edtUserName.Text,
                    Crypto.AES.Encrypt(edtPassword.Text, Key),
                    cbDatabase.Text
                ));
                Servers = ServerList.List(AppRegistryKey);
                #endregion


                Reg.Write(AppRegistryKey, "Author", edtAuthor.Text);
                Reg.Write(AppRegistryKey, "SaveFolder", edtSaveFolder.Text);

                Reg.Write(AppRegistryKey, "ScriptTables", cbScriptTables.Checked ? 1 : 0);
                Reg.Write(AppRegistryKey, "ScriptTriggers", cbScriptTriggers.Checked ? 1 : 0);
                Reg.Write(AppRegistryKey, "ScriptViews", cbScriptViews.Checked ? 1 : 0);
                Reg.Write(AppRegistryKey, "ScriptProcedures", cbScriptProcedures.Checked ? 1 : 0);
                Reg.Write(AppRegistryKey, "ScriptFunctions", cbScriptFunctions.Checked ? 1 : 0);

                Reg.Write(AppRegistryKey, "Collation", cbCollation.Checked ? 1 : 0);
                Reg.Write(AppRegistryKey, "EncloseWithBrackets", cbEncloseWithBrackets.Checked ? 1 : 0);
                Reg.Write(AppRegistryKey, "OmitDbo", cbOmitDbo.Checked ? 1 : 0);
                Reg.Write(AppRegistryKey, "SavelLog", cbSavelLog.Checked ? 1 : 0);

                string author = edtAuthor.Text;
                string database = cbDatabase.SelectedItem.ToString();
                string saveFolder = edtSaveFolder.Text + cbDatabase.Text.AddTrailingBackSlashes();

                bool scriptTables = cbScriptTables.Checked;
                bool scriptTriggers = cbScriptTriggers.Checked;
                bool scriptViews = cbScriptViews.Checked;
                bool scriptProcedures = cbScriptProcedures.Checked;
                bool scriptFunctions = cbScriptFunctions.Checked;
                bool encloseWithBrackets = cbEncloseWithBrackets.Checked;
                bool collation = cbCollation.Checked;
                bool omitDbo = cbOmitDbo.Checked;

                await Task.Run(() => {
                    using (WriteDB writeDB = new WriteDB(DbConn, Log, author, database, saveFolder, scriptTables, scriptTriggers, scriptViews, scriptProcedures, scriptFunctions, encloseWithBrackets, collation, omitDbo)) {
                        writeDB.Write();
                    }
                });
                if (cbSavelLog.Checked)
                    txtLog.SaveFile(saveFolder + AppName + ".log.doc");
            } catch (Exception ex) {
                Log(ex.Message, Color.Red);
            } finally {
                grpOptions.Enabled = true;
                Running = false;
            }
        }
        #endregion

    }
}
