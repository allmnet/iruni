using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Reflection;
using System.IO;

namespace Iruniview
{
    public partial class RuleView : Form
    {
        public string string_sqlite_rule_path = null;
        public string string_sqlite_rule = null;
        public RuleView()
        {
            string_sqlite_rule_path = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string_sqlite_rule = "Data Source=" + string_sqlite_rule_path + "\\Rule.db";

            string_sqlite_rule_path += "\\Rule.db";

            FileInfo fileinfo2 = new FileInfo(string_sqlite_rule_path);
            if (fileinfo2.Exists == false)
            {
                string query = @"CREATE TABLE [Rule] (
                                      [Id] INTEGER PRIMARY KEY   AUTOINCREMENT
                                    , [enable] bit NULL
                                    , [title] nchar(100) NULL
                                    , [andpatten] nchar(100) NULL
                                    , [orpatten] nchar(100) NULL
                                    , [time] int NULL
                                    , [eventcount] int NULL
                                    , [diffmatch] bit NULL
                                    , [ipcollect] bit NULL
                                    , [logcollect] bit NULL
                                    , [actionip] nchar(100) NULL
                                    , [message] nchar(100) NULL
                                    , [script] nchar(100) NULL
                                    , [email] nchar(100) NULL
                                    , [hipchat] nchar(100) NULL
                                    , [hipchatgroup] nchar(100) NULL
                                    )";
                SQLiteConnection.CreateFile(string_sqlite_rule_path);
                using (SQLiteConnection conn = new SQLiteConnection(string_sqlite_rule))
                {
                    using (SQLiteCommand command = new SQLiteCommand(conn))
                    {
                        conn.Open();

                        command.CommandText = query;
                        command.ExecuteNonQuery();
                    }
                }
            }
            InitializeComponent();
        }

        private void RuleView_Load(object sender, EventArgs e)
        {
            // TODO: 이 코드는 데이터를 'ruleDataSet.Rule' 테이블에 로드합니다. 필요 시 이 코드를 이동하거나 제거할 수 있습니다.
            this.ruleTableAdapter.Fill(this.ruleDataSet.Rule);
            // TODO: 이 코드는 데이터를 'ruleDataSet.Rule' 테이블에 로드합니다. 필요한 경우 이 코드를 이동하거나 제거할 수 있습니다.
            try
            {
                this.ruleDataSet.Rule.Columns["title"].ColumnName = "Rule Title";
                this.ruleTableAdapter.Fill(this.ruleDataSet.Rule);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MyForm_Closing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
