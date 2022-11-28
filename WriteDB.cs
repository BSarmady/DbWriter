using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;


namespace DbWriter {

    internal class WriteDB: IDisposable {

        private SqlConnection DbConn;
        private LoggerDelegate Log;
        private string Author;
        private string Database;
        private string SaveFolder;
        private bool ScriptTables;
        private bool ScriptTriggers;
        private bool ScriptViews;
        private bool ScriptProcedures;
        private bool ScriptFunctions;
        private bool EncloseWithBrackets;
        private bool Collation;
        private bool OmitDbo;

        private DataTable Db_Tables;
        private DataTable Db_Objects;
        private DataTable Db_Fk;
        private DataTable Db_Schemas;
        private DataTable Db_Params;

        #region private const string SQL_Schemas
        private const string SQL_Schemas = @"SELECT 
                schema_name = name,
                create_date = (SELECT min (create_date) from sys.objects o WHERE o.schema_id = ss.schema_id),
                modify_date = (SELECT max (modify_date) from sys.objects o WHERE o.schema_id = ss.schema_id)
            FROM 
                sys.schemas ss
            WHERE principal_id = 1";
        #endregion

        #region private const string SQL_Tables
        private const string SQL_Tables = @"DECLARE @default_collation varchar(100);
            SELECT @default_collation = collation_name FROM sys.databases;
            SELECT
                o_type = 'TBL',
                schema_name             = SCHEMA_NAME(so.schema_id),
                table_name              = OBJECT_NAME(so.OBJECT_ID),
                column_id = 0,
                o_name = null, 
                type_name = null, 
                collation_name = null, 
                is_nullable = null, 
                identity_value = null, 
                formula = null,
                default_value = null, 
                type_desc = NULL,
                comment = CAST(se.value AS nvarchar(MAX)),
                create_date,
                modify_date
            FROM
                sys.objects so
                LEFT JOIN sys.extended_properties se ON se.name = 'MS_Description' AND se.minor_id = 0 AND so.object_id = se.major_id
            WHERE
                so.type  in ('U') AND
                so.is_ms_shipped=0 AND
                OBJECT_NAME(so.object_id) NOT IN ('sysdiagrams', 'sp_alterdiagram','sp_creatediagram','sp_dropdiagram','sp_helpdiagramdefinition','sp_helpdiagrams','sp_renamediagram','sp_upgraddiagrams')
            UNION SELECT 
                o_type = 'COL',
                schema_name = object_schema_name(sc.object_id),
                table_name = object_name(sc.object_id),
                sc.column_id,
                o_name = sc.name,

                --parent_name = object_name(parent_object_id),
                type_name = type_name(sc.system_type_id) + (CASE
                        WHEN type_name(sc.system_type_id) IN ('decimal', 'numeric') THEN concat('(', sc.precision, ', ', sc.scale, ')')
                        WHEN sc.max_length = -1 AND type_name(sc.system_type_id) <> 'xml' THEN '(max)'
                        WHEN type_name(sc.system_type_id) in ('nchar', 'nvarchar') THEN concat('(', sc.max_length/2, ')')
                        WHEN type_name(sc.system_type_id) in ('char', 'varchar','binary','varbinary', 'image','time', 'datetimeoffset', 'datetime2') THEN concat('(', sc.max_length, ')')
                        ELSE ''
                END),
                collation_name = CASE WHEN sc.collation_name = @default_collation THEN NULL ELSE sc.collation_name END,
                sc.is_nullable,
                identity_value = CASE WHEN sc.is_identity = 0 THEN NULL ELSE concat('(', cast(sidc.seed_value as varchar), ',' , cast(sidc.increment_value as varchar),')') END,
                formula = CASE WHEN sc.is_computed = 0 THEN NULL ELSE (SELECT definition FROM sys.computed_columns scc WHERE scc.object_id = sc.OBJECT_ID AND scc.column_id = sc.column_id) END,
                default_value = dc.definition,
                type_desc = NULL,
                comment = CAST(se.value AS nvarchar(MAX)),
                create_date = null,
                modify_date = null
            FROM 
                sys.columns sc
                LEFT JOIN sys.identity_columns sidc ON sidc.object_id = sc.object_id AND sidc.column_id = sc.column_id
                LEFT JOIN sys.default_constraints dc ON dc.parent_object_id = sc.object_id AND dc.parent_column_id = sc.column_id
                LEFT JOIN sys.extended_properties se ON se.name = 'MS_Description' AND se.minor_id = sc.column_id AND sc.object_id = se.major_id
            WHERE
                object_schema_name(sc.object_id) NOT IN ('SYS') AND
                OBJECT_NAME(sc.object_id) NOT IN ('sysdiagrams', 'sp_alterdiagram','sp_creatediagram','sp_dropdiagram','sp_helpdiagramdefinition','sp_helpdiagrams','sp_renamediagram','sp_upgraddiagrams')
            UNION SELECT
                o_type = CASE WHEN is_primary_key = 1 THEN 'X_1_PK' WHEN is_unique = 1 THEN 'X_2_UQ' WHEN is_unique_constraint = 1 THEN 'X_3_UC' ELSE 'X_4_IX' END,
                schema_name = object_schema_name(object_id),
                table_name = object_name(object_id),
                column_id = index_id,
                o_name = sys.indexes.name,
                type_name = null, 
                collation_name = null, 
                is_nullable = null, 
                identity_value = null, 
                formula = null,
                default_value = null, 
                type_desc,
                comment = STUFF((SELECT ', ' + COl_NAME(object_id, column_id) + (CASE WHEN is_descending_key = 1 THEN ' DESC' ELSE '' END) FROM sys.index_columns WHERE sys.indexes.index_id = sys.index_columns.index_id AND sys.indexes.object_id = sys.index_columns.object_id  FOR XML PATH('')), 1, 2,''),
                create_date = null,
                modify_date = null
            FROM
                sys.indexes
            WHERE
                type in (1,2) AND -- type 0 is HEAP
                OBJECT_SCHEMA_NAME(object_id) <> 'sys'
            UNION SELECT
                o_type = 'Y_CHK',
                schema_name = object_schema_name(object_id),
                table_name = object_name(parent_object_id),
                column_id = NULL,
                o_name = name,
                type_name = null, 
                collation_name = null, 
                is_nullable = null, 
                identity_value = null, 
                formula = null,
                default_value = null, 
                type_desc = NULL,
                comment = definition,
                create_date = null,
                modify_date = null
            FROM 
                sys.check_constraints
            ORDER BY 
                schema_name, table_name, o_type, column_id
            ";
        #endregion

        #region private const string SQL_FK
        private const string SQL_FK = @"SELECT 
                fk_name = object_name(constraint_object_id), 
                master_schema = object_schema_name(referenced_object_id), 
                master_name = object_name(referenced_object_id), 
                master_columns = STUFF((SELECT concat(', ', col_name(referenced_object_id, referenced_column_id)) FROM sys.foreign_key_columns fkcr WHERE fkcr.constraint_object_id = fkcm.constraint_object_id FOR XML PATH('')), 1, 2, ''),
                detail_schema = object_schema_name(parent_object_id), 
                detail_name = object_name(parent_object_id), 
                detail_columns = stuff((SELECT concat(', ', col_name(parent_object_id, parent_column_id)) FROM sys.foreign_key_columns fkcp WHERE fkcp.constraint_object_id = fkcm.constraint_object_id FOR XML PATH('')), 1, 2, ''),
                on_update = (SELECT update_referential_action_desc FROM sys.foreign_keys WHERE constraint_object_id = object_id),
                on_delete = (SELECT delete_referential_action_desc FROM sys.foreign_keys WHERE constraint_object_id = object_id)
            FROM 
                sys.foreign_key_columns fkcm
            GROUP BY 
                constraint_object_id, 
                parent_object_id, 
                referenced_object_id";
        #endregion

        #region private const string SQL_Objects
        private const string SQL_Objects = @"SELECT
                type_name = type,
                schema_name = schema_name(so.schema_id),
                object_name = so.name,
                text = (SELECT sc.text + '' FROM syscomments sc WHERE sc.id = so.object_id FOR XML PATH('')),
                comment = se.value
            FROM
                sys.objects so
                LEFT JOIN sys.extended_properties se ON se.name = 'MS_Description' AND se.minor_id = 0 AND so.object_id = se.major_id
            WHERE
                type in ('TR', 'V', 'FN', 'TF', 'IF', 'P') AND
                EXISTS(SELECT * FROM sys.schemas ss WHERE ss.principal_id=1 AND so.schema_id=schema_id) AND
                is_ms_shipped = 0 AND
                OBJECT_NAME(so.object_id) NOT IN ('sysdiagrams', 'sp_alterdiagram','sp_creatediagram','sp_dropdiagram','sp_helpdiagramdefinition','sp_helpdiagrams','sp_renamediagram','sp_upgraddiagrams', 'fn_diagramobjects')
            ORDER BY
                type_name,
                schema_name,
                object_name";
        #endregion

        #region private const string SQL_Params
        private const string SQL_Params = @"SELECT 
                schema_name = object_schema_name(object_id),
                object_name = object_name(object_id),
                parameter_name = ap.name,
                comment = sc.value
            FROM 
                sys.all_parameters ap
                LEFT JOIN sys.extended_properties sc ON sc.major_id = ap.object_id AND sc.minor_id = ap.parameter_id
            WHERE
                object_schema_name(object_id) NOT IN ('SYS') AND
                OBJECT_NAME(object_id) NOT IN ('sysdiagrams', 'sp_alterdiagram','sp_creatediagram','sp_dropdiagram','sp_helpdiagramdefinition','sp_helpdiagrams','sp_renamediagram','sp_upgraddiagrams')
            ORDER BY 
                schema_name, object_name, parameter_id";
        #endregion


        #region public WriteDB(...)
        public WriteDB(SqlConnection DbConn, LoggerDelegate Log, string Author, string Database, string SaveFolder, bool ScriptTables, bool ScriptTriggers, bool ScriptViews, bool ScriptProcedures, bool ScriptFunctions, bool EncloseWithBrackets, bool Collation, bool OmitDbo) {
            this.DbConn = DbConn;
            this.Log = Log;
            this.Author = Author;
            this.Database = Database;
            this.SaveFolder = SaveFolder;
            this.ScriptTables = ScriptTables;
            this.ScriptTriggers = ScriptTriggers;
            this.ScriptViews = ScriptViews;
            this.ScriptProcedures = ScriptProcedures;
            this.ScriptFunctions = ScriptFunctions;
            this.EncloseWithBrackets = EncloseWithBrackets;
            this.Collation = Collation;
            this.OmitDbo = OmitDbo;
        }
        #endregion

        #region public void Dispose()
        public void Dispose() {
        }
        #endregion

        #region public void Write(...);
        public void Write() {
            try {
                if (DbConn.State != ConnectionState.Open)
                    DbConn.Open();
                new SqlCommand("USE [" + Database + "]", DbConn).ExecuteNonQuery();

                #region Reading Database Information
                Log("Reading Database Information ...", Color.Blue);

                Log("    Schemas ... ", Color.Gray);
                Db_Schemas = get_data(SQL_Schemas, DbConn);
                Log("        " + Db_Schemas.Rows.Count + " rows", Color.Gray);

                Log("    Tables ... ", Color.Gray);
                Db_Tables = get_data(SQL_Tables, DbConn);
                Log("        " + Db_Tables.Rows.Count + " rows", Color.Gray);

                Log("    Foreign keys ... ", Color.Gray);
                Db_Fk = get_data(SQL_FK, DbConn);
                Log("        " + Db_Fk.Rows.Count + " rows", Color.Gray);

                Log("    Other objects ... ", Color.Gray);
                Db_Objects = get_data(SQL_Objects, DbConn);
                Log("        " + Db_Objects.Rows.Count + " rows", Color.Gray);

                Log("    Parameters ... ", Color.Gray);
                Db_Params = get_data(SQL_Params, DbConn);
                Log("        " + Db_Params.Rows.Count + " rows", Color.Gray);

                DbConn.Close();
                #endregion

                Log("Generating SQL documents ...", Color.Blue);

                List<string> Single_File_List = new List<string> { };
                List<string> foreign_keys = new List<string> { };

                Single_File_List.Add(write_database_file());

                // Schemas
                Log("Writing schemas ...", Color.Blue);
                foreach (DataRow row in Db_Schemas.Rows)
                    Single_File_List.Add(write_schema_files(row["schema_name"].ToString(), row["create_date"].ToDateTime(DateTime.Now), row["modify_date"].ToDateTime(DateTime.Now)));

                // Tables
                if (ScriptTables) {
                    Log("Writing tables ...", Color.Blue);
                    DataTable tableList = new DataView(Db_Tables, "o_type = 'TBL'", "", DataViewRowState.CurrentRows).ToTable();
                    foreach (DataRow row in tableList.Rows) {
                        List<string> chunks = write_table_files(row["schema_name"].ToString(), row["table_name"].ToString(), row["comment"].ToString(), row["create_date"].ToDateTime(DateTime.Now), row["modify_date"].ToDateTime(DateTime.Now));
                        // it always returns 3 parts, index 0 is table script, 1 is foreign keys, 2 is comments
                        Single_File_List.Add(chunks[0]);
                        if (chunks[1] != "")
                            foreign_keys.Add(chunks[1]);
                        Single_File_List.Add(chunks[2]);
                    }
                    Single_File_List.AddRange(foreign_keys);
                    foreign_keys.Clear();
                } else {
                    Log("Skipping tables", Color.Blue);
                }

                // Triggers
                if (ScriptTriggers) {
                    Log("Writing triggers", Color.Blue);
                    DataTable tableList = new DataView(Db_Objects, "type_name='TR'", "", DataViewRowState.CurrentRows).ToTable();
                    foreach (DataRow row in tableList.Rows) {
                        Single_File_List.Add(write_trigger_sql(row["schema_name"].ToString(), row["object_name"].ToString(), row["text"].ToString()));
                    }
                } else {
                    Log("Skipping triggers", Color.Blue);
                }

                // Views
                if (ScriptViews) {
                    Log("Writing views", Color.Blue);
                    DataTable tableList = new DataView(Db_Objects, "type_name='V'", "", DataViewRowState.CurrentRows).ToTable();
                    foreach (DataRow row in tableList.Rows) {
                        Single_File_List.Add(write_view_sql(row["schema_name"].ToString(), row["object_name"].ToString(), row["text"].ToString()));
                    }
                } else {
                    Log("Skipping views", Color.Blue);
                }

                // Functions
                if (ScriptFunctions) {
                    Log("Writing functions", Color.Blue);
                    DataTable tableList = new DataView(Db_Objects, "type_name in ('FN', 'TF', 'IF')", "", DataViewRowState.CurrentRows).ToTable();
                    foreach (DataRow row in tableList.Rows) {
                        Single_File_List.Add(write_function_sql(row["schema_name"].ToString(), row["object_name"].ToString(), row["comment"].ToString(), row["text"].ToString()));
                    }
                } else {
                    Log("Skipping functions", Color.Blue);
                }

                // Procedures
                if (ScriptProcedures) {
                    Log("Writing procedures", Color.Blue);
                    DataTable tableList = new DataView(Db_Objects, "type_name='P'", "", DataViewRowState.CurrentRows).ToTable();
                    foreach (DataRow row in tableList.Rows) {
                        Single_File_List.Add(write_procedure_sql(row["schema_name"].ToString(), row["object_name"].ToString(), row["comment"].ToString(), row["text"].ToString()));
                    }
                } else {
                    Log("Skipping procedures", Color.Blue);
                }

                File.WriteAllText(SaveFolder + "Latest.sql", string.Join(Environment.NewLine, Single_File_List));
                Log("Done.", Color.Blue);
            } catch (Exception ex) {
                if (ex is gen_exception)
                    Log(ex.Message, Color.Red);
                else
                    Log(ex.ToString(), Color.Red);
            }
        }
        #endregion


        #region private DataTable get_data(...)
        private DataTable get_data(string sql, SqlConnection conn) {
            using (SqlDataAdapter adapter = new SqlDataAdapter(sql, conn)) {
                DataTable table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }
        #endregion

        #region private string enclose_with_brackets()
        private string enclose_with_brackets(string instr) {
            if (!EncloseWithBrackets || instr == "")
                return instr;
            return "[" + instr + "]";

        }
        #endregion

        #region private string omit_dbo()
        private string omit_dbo(string instr) {
            if (!OmitDbo || instr.ToLower() != "dbo")
                return instr + ".";
            return "";
        }
        #endregion


        #region private string write_database_file(...)
        private string write_database_file() {
            try {
                DateTime create_date = DateTime.Now;
                DateTime modify_date = DateTime.Now;
                string collation_name = "";
                string server_collation_name = "";
                DataTable table = get_data(@"SELECT 
                        collation_name, 
                        server_collation_name = CONVERT(varchar(256), SERVERPROPERTY('collation')), 
                        create_date, 
                        modify_date = (SELECT max (modify_date) FROM [" + Database + @"].sys.objects)
                    FROM sys.databases
                    WHERE name = '" + Database + "'", DbConn);
                if (table != null && table.Rows.Count == 1) {
                    create_date = table.Rows[0]["create_date"].ToDateTime(DateTime.Now);
                    modify_date = table.Rows[0]["modify_date"].ToDateTime(DateTime.Now);
                    collation_name = table.Rows[0]["collation_name"].ToString();
                    server_collation_name = table.Rows[0]["server_collation_name"].ToString();
                }

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("/*------------------------------------------------------------------------------------------------------");
                sb.AppendLine("  Description   : Last scripted by " + Author);
                sb.AppendLine("  Created       : " + create_date.ToString("yyyy-MM-dd"));
                if ((modify_date - create_date).Days > 0)
                    sb.AppendLine("  Modified      : " + modify_date.ToString("yyyy-MM-dd"));
                sb.AppendLine("  ------------------------------------------------------------------------------------------------------*/");
                sb.AppendLine("USE master");
                sb.AppendLine("--DROP DATABASE IF EXISTS " + enclose_with_brackets(Database));
                sb.AppendLine("GO");
                sb.AppendLine("CREATE DATABASE " + enclose_with_brackets(Database) + (collation_name == "" || collation_name == server_collation_name ? "" : " COLLATE " + collation_name));
                sb.AppendLine("GO");
                sb.AppendLine("USE " + enclose_with_brackets(Database));
                sb.AppendLine("GO");

                if (!Directory.Exists(SaveFolder))
                    Directory.CreateDirectory(SaveFolder);

                File.WriteAllText(SaveFolder + "database.sql", sb.ToString());

                return sb.ToString();
            } catch (Exception ex) {
                if (ex is gen_exception)
                    Log("    Error: " + ex.Message, Color.Red);
                else
                    Log("    Error: " + ex.ToString(), Color.Red);
                return "";
            }
        }
        #endregion

        #region private string write_schema_files(...)
        private string write_schema_files(string schema_name, DateTime create_date, DateTime modify_date) {
            try {
                if (schema_name == "dbo")
                    return "";

                Log("    " + schema_name, Color.Black);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("/* ------------------------------------------------------------------------------------------------------");
                sb.AppendLine("  Description : Create " + schema_name + " Schema");
                sb.AppendLine("  Created     : " + create_date.ToString("yyyy-MM-dd") + " " + Author);
                if ((modify_date - create_date).Days > 0)
                    sb.AppendLine("  Modified    : " + modify_date.ToString("yyyy-MM-dd") + " " + Author);
                sb.AppendLine("------------------------------------------------------------------------------------------------------ */");
                sb.AppendLine("IF NOT EXISTS ( SELECT * FROM sys.schemas WHERE name='" + enclose_with_brackets(schema_name) + "' )");
                sb.AppendLine("    EXEC sp_executesql N'CREATE SCHEMA " + schema_name + "'");
                sb.AppendLine("GO");

                if (!Directory.Exists(SaveFolder + schema_name))
                    Directory.CreateDirectory(SaveFolder + schema_name);
                File.WriteAllText(SaveFolder + schema_name + "\\schema.sql", sb.ToString());
                return sb.ToString();
            } catch (Exception ex) {
                if (ex is gen_exception)
                    Log(ex.Message, Color.Red);
                else
                    Log(ex.ToString(), Color.Red);
                return "";
            }
        }
        #endregion

        #region private List<string> write_table_files(...);
        private List<string> write_table_files(string schema_name, string table_name, string table_comment, DateTime create_date, DateTime modified_date) {
            try {
                List<string> chunks = new List<string>();
                Log("    " + enclose_with_brackets(omit_dbo(schema_name)) + enclose_with_brackets(table_name), Color.Black);

                DataTable table = new DataView(Db_Tables, "schema_name='" + schema_name + "' AND table_name = '" + table_name + "'", "", DataViewRowState.CurrentRows).ToTable();

                StringBuilder script = new StringBuilder();

                #region Add comment and header
                script.AppendLine("/*------------------------------------------------------------------------------------------------------");
                script.AppendLine("  Description : " + table_comment.PadStringAtNewLines(16));
                script.AppendLine("  Created     : " + create_date.ToString("yyyy-MM-dd") + " " + Author);
                if ((modified_date - create_date).TotalDays > 0)
                    script.AppendLine("  Modified    : " + modified_date.ToString("yyyy-MM-dd") + " " + Author);

                script.AppendLine("  ------------------------------------------------------------------------------------------------------*/");
                script.AppendLine("CREATE TABLE " + enclose_with_brackets(omit_dbo(schema_name)) + enclose_with_brackets(table_name) + "(");
                #endregion

                #region Measure column name max length
                int max_name_length = 0;
                foreach (DataRow row in table.Rows) {
                    if (row["o_type"].ToString() != "COL")
                        continue;
                    int Length = enclose_with_brackets(row["o_name"].ToString()).Length;
                    if (max_name_length < Length)
                        max_name_length = Length;
                }
                max_name_length++;
                #endregion

                #region Add columns
                List<string> ColumnList = new List<string> { };
                foreach (DataRow row in table.Rows) {
                    if (row["o_type"].ToString() != "COL")
                        continue;
                    StringBuilder sb = new StringBuilder();
                    sb.Append(enclose_with_brackets(row["o_name"].ToString()).PadRight(max_name_length));
                    if (row["formula"] != DBNull.Value) {
                        string formula = row["formula"].ToString();
                        while (formula[0] == '(' && formula[formula.Length - 1] == ')')
                            formula = formula.Substring(1, formula.Length - 2);
                        sb.Append("AS " + formula);
                    } else {
                        sb.Append(row["type_name"].ToString());
                        if (Collation && row["collation_name"].ToString() != "")
                            sb.Append(" COLLATE " + row["collation_name"]);
                        if (row["identity_value"] != DBNull.Value)
                            sb.Append(" IDENTITY " + row["identity_value"]);
                        if (!row["is_nullable"].ToBoolean())
                            sb.Append(" NOT NULL");
                        if (row["default_value"].ToString() != "") {
                            string default_value = row["default_value"].ToString();
                            while (default_value[0] == '(' && default_value[default_value.Length - 1] == ')')
                                default_value = default_value.Substring(1, default_value.Length - 2);
                            sb.Append(" DEFAULT " + default_value);
                        }
                    }
                    ColumnList.Add(sb.ToString());
                }
                #endregion

                ColumnList.Add("");

                #region Add primary key
                foreach (DataRow row in table.Rows) {
                    if (row["o_type"].ToString() != "X_1_PK")
                        continue;
                    ColumnList.Add("CONSTRAINT " + enclose_with_brackets(row["o_name"].ToString()) + " PRIMARY KEY" + (row["type_desc"].ToString() == "CLUSTERED" ? "" : " " + row["type_desc"].ToString()) + " (" + row["comment"].ToString().TrimEnd(new char[] { ',', ' ' }) + ")");
                    //break; // Only one PK per table
                }
                #endregion

                #region Add indexes
                DataTable indexes = new DataView(table, "o_type in ('X_2_UQ', 'X_3_UC', 'X_4_IX')", "", DataViewRowState.CurrentRows).ToTable();
                foreach (DataRow row in indexes.Rows) {
                    // index default type is NONCLUSTERED
                    ColumnList.Add("INDEX " + enclose_with_brackets(row["o_name"].ToString()) +
                        (row["o_type"].ToString() == "X_2_UQ" ? " UNIQUE" : "") +
                        (row["type_desc"].ToString() == "CLUSTERED" ? " CLUSTERED" : "") +
                        " (" + enclose_with_brackets(row["comment"].ToString().TrimEnd((new char[] { ',', ' ' })).Replace(", ", EncloseWithBrackets ? "], [" : ", ")) + ")");
                }
                #endregion

                ColumnList.Add("");

                #region Add check constraints
                DataTable check_constraints = new DataView(table, "o_type = 'Y_CHK'", "", DataViewRowState.CurrentRows).ToTable();
                if (check_constraints.Rows.Count > 0) {
                    foreach (DataRow row in check_constraints.Rows) {
                        string formula = row["comment"].ToString();
                        while (formula[0] == '(' && formula[formula.Length - 1] == ')')
                            formula = formula.Substring(1, formula.Length - 2);
                        // Fix sql server bug that forcefully adds [] to column names
                        if (!EncloseWithBrackets)
                            formula = formula.Replace("[", "").Replace("]", "");
                        ColumnList.Add("CONSTRAINT " + enclose_with_brackets(row["o_name"].ToString()) + " CHECK (" + formula + ")");
                    }
                }
                #endregion

                script.AppendLine("    " + string.Join("," + Environment.NewLine, ColumnList).Replace(Environment.NewLine + "," + Environment.NewLine, Environment.NewLine + Environment.NewLine).PadStringAtNewLines());
                script.AppendLine(")");
                script.AppendLine("GO");

                chunks.Add(script.ToString());
                script.Clear();

                #region Add foreign keys
                DataTable foreign_keys = new DataView(Db_Fk, "detail_schema = '" + schema_name + "' AND detail_name='" + table_name + "'", "", DataViewRowState.CurrentRows).ToTable();
                if (foreign_keys.Rows.Count > 0) {
                    script.AppendLine("-- Foreign keys -----------------------------------------------------------------------------------------");
                    foreach (DataRow row in foreign_keys.Rows) {
                        script.AppendLine("ALTER TABLE " + enclose_with_brackets(omit_dbo(schema_name)) + enclose_with_brackets(table_name) + " WITH CHECK ADD CONSTRAINT " + enclose_with_brackets(row["fk_name"].ToString()) + " FOREIGN KEY(" + enclose_with_brackets(row["detail_columns"].ToString().Replace(", ", EncloseWithBrackets ? "], [" : ", ")) + ")");
                        script.AppendLine("    REFERENCES " + enclose_with_brackets(omit_dbo(row["master_schema"].ToString())) + enclose_with_brackets(row["master_name"].ToString()) + "(" + enclose_with_brackets(row["master_columns"].ToString().Replace(", ", EncloseWithBrackets ? "], [" : ", ")) + ")");
                        if (row["on_update"].ToString() != "NO_ACTION")
                            script.AppendLine("    ON UPDATE " + row["on_update"]);
                        if (row["on_delete"].ToString() != "NO_ACTION")
                            script.AppendLine("    ON DELETE " + row["on_delete"]);
                        script.AppendLine("GO");
                    }
                }
                #endregion

                chunks.Add(script.ToString());
                script.Clear();

                #region Add table and column comments
                script.AppendLine("-- Table Comments ---------------------------------------------------------------------------------------");
                DataTable comments = new DataView(table, "o_type in ('COL','TBL')", "column_id", DataViewRowState.CurrentRows).ToTable();
                foreach (DataRow row in comments.Rows) {
                    if (row["o_type"].ToString() == "TBL")
                        script.AppendLine("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'" + schema_name + "', @level1type=N'TABLE', @level1name=N'" + table_name + "', @value=N'" + table_comment.Replace("'", "''") + "'");
                    else if (row["o_type"].ToString() == "COL")
                        script.AppendLine("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'" + schema_name + "', @level1type=N'TABLE', @level1name=N'" + table_name + "', @level2type=N'COLUMN', @level2name=N'" + row["o_name"] + "', @value=N'" + row["comment"].ToString().Replace("'", "''") + "'");
                }
                script.AppendLine("GO");
                #endregion

                chunks.Add(script.ToString());
                script.Clear();

                string folder = SaveFolder + schema_name + "\\Tables\\";
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                File.WriteAllText(folder + table_name + ".sql",
                    string.Join(Environment.NewLine, chunks)
                    // sometimes table doesn't have foreign key and an extra new line is added that we remove here
                    .Replace(Environment.NewLine + Environment.NewLine + Environment.NewLine, Environment.NewLine + Environment.NewLine));

                return chunks;
            } catch (Exception ex) {
                if (ex is gen_exception)
                    Log(ex.Message, Color.Red);
                else
                    Log(ex.ToString(), Color.Red);
                return new List<string>();
            }
        }
        #endregion

        #region private string write_trigger_sql(...);
        private string write_trigger_sql(string schema_name, string trigger_name, string trigger_text) {
            try {
                Log("    " + enclose_with_brackets(omit_dbo(schema_name)) + enclose_with_brackets(trigger_name), Color.Black);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("DROP TRIGGER IF EXISTS " + enclose_with_brackets(omit_dbo(schema_name)) + enclose_with_brackets(trigger_name) + "]");
                sb.AppendLine("GO");
                // Text returned by procedure is formatted with html representation of newline and special characters
                sb.AppendLine(trigger_text.DecodeHTML());
                sb.AppendLine("GO");

                string folder = SaveFolder + schema_name + "\\Triggers\\";
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                File.WriteAllText(folder + trigger_name + ".sql", sb.ToString());
                return sb.ToString();
            } catch (Exception ex) {
                if (ex is gen_exception)
                    Log(ex.Message, Color.Red);
                else
                    Log(ex.ToString(), Color.Red);
                return "";
            }
        }
        #endregion

        #region private string write_view_sql(...);
        private string write_view_sql(string schema_name, string view_name, string view_text) {
            try {
                Log("    " + enclose_with_brackets(omit_dbo(schema_name)) + enclose_with_brackets(view_name), Color.Black);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("DROP VIEW IF EXISTS " + enclose_with_brackets(omit_dbo(schema_name)) + enclose_with_brackets(view_name));
                sb.AppendLine("GO");
                // Text returned by procedure is formatted with html representation of newline and special characters
                sb.AppendLine(view_text.DecodeHTML());
                sb.AppendLine("GO");

                string folder = SaveFolder + schema_name + "\\Views\\";
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                File.WriteAllText(folder + view_name + ".sql", sb.ToString());
                return sb.ToString();
            } catch (Exception ex) {
                if (ex is gen_exception)
                    Log(ex.Message, Color.Red);
                else
                    Log(ex.ToString(), Color.Red);
                return "";
            }
        }
        #endregion

        #region private string write_function_sql(...);
        private string write_function_sql(string schema_name, string function_name, string function_comment, string function_text) {
            try {
                Log("    " + enclose_with_brackets(omit_dbo(schema_name)) + enclose_with_brackets(function_name), Color.Black);
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("DROP FUNCTION IF EXISTS " + enclose_with_brackets(omit_dbo(schema_name)) + enclose_with_brackets(function_name));
                sb.AppendLine("GO");
                // Text returned by procedure is formatted with html representation of newline and special characters
                sb.AppendLine(function_text.DecodeHTML());
                sb.AppendLine("GO");

                DataTable table = new DataView(Db_Params, "schema_name='" + schema_name + "' AND object_name='" + function_name + "'", "", DataViewRowState.CurrentRows).ToTable();
                sb.AppendLine();
                sb.AppendLine("-- stored procedure and parameter descriptions ---------------------------------------------------------------");
                sb.AppendLine("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'" + schema_name + "', @level1type=N'PROCEDURE', @level1name=N'" + function_name + "', @value=N'" + function_comment.Replace("'", "''") + "'");
                foreach (DataRow row in table.Rows) {
                    if (row["parameter_name"].ToString() != "") // return value doesn't have a prameter name
                        sb.AppendLine("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'" + schema_name + "', @level1type=N'PROCEDURE', @level1name=N'" + function_name + "', @level2type=N'PARAMETER', @level2name=N'" + row["parameter_name"] + "', @value=N'" + row["comment"].ToString().Replace("'", "''") + "'");
                }
                sb.AppendLine("GO");

                //string folder = SaveFolder + schema_name + "\\UserDefinedFunction\\";
                string folder = SaveFolder + schema_name + "\\Functions\\";
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                File.WriteAllText(folder + function_name + ".sql", sb.ToString());
                return sb.ToString();
            } catch (Exception ex) {
                if (ex is gen_exception)
                    Log(ex.Message, Color.Red);
                else
                    Log(ex.ToString(), Color.Red);
                return "";
            }
        }
        #endregion

        #region private string write_procedure_sql(...);
        private string write_procedure_sql(string schema_name, string procedure_name, string procedure_comment, string procedure_text) {
            try {
                Log("    " + enclose_with_brackets(omit_dbo(schema_name)) + enclose_with_brackets(procedure_name), Color.Black);
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("DROP PROCEDURE IF EXISTS " + enclose_with_brackets(omit_dbo(schema_name)) + enclose_with_brackets(procedure_name));
                sb.AppendLine("GO");
                sb.AppendLine(procedure_text.DecodeHTML());
                sb.AppendLine("GO");

                DataTable table = new DataView(Db_Params, "schema_name='" + schema_name + "' AND object_name='" + procedure_name + "'", "", DataViewRowState.CurrentRows).ToTable();
                sb.AppendLine();
                sb.AppendLine("-- stored procedure and parameter descriptions ---------------------------------------------------------------");
                sb.AppendLine("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'" + schema_name + "', @level1type=N'PROCEDURE', @level1name=N'" + procedure_name + "', @value=N'" + procedure_comment.Replace("'", "''") + "'");
                foreach (DataRow row in table.Rows) {
                    sb.AppendLine("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'" + schema_name + "', @level1type=N'PROCEDURE', @level1name=N'" + procedure_name + "', @level2type=N'PARAMETER', @level2name=N'" + row["parameter_name"] + "', @value=N'" + row["comment"].ToString().Replace("'", "''") + "'");
                }
                sb.AppendLine("GO");
                //string folder = SaveFolder + schema_name + "\\StoredProcedures\\";
                string folder = SaveFolder + schema_name + "\\Procedures\\";
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                File.WriteAllText(folder + procedure_name + ".sql", sb.ToString());
                return sb.ToString();
            } catch (Exception ex) {
                if (ex is gen_exception)
                    Log(ex.Message, Color.Red);
                else
                    Log(ex.ToString(), Color.Red);
                return "";
            }
        }
        #endregion
    }
}