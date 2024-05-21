using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MetroFramework.Controls;
using MetroFramework.Forms;


namespace Shop
{
    public partial class RedefinitionUsers : MetroForm
    {
        private readonly checkUser _user;
        DataBase DB = new DataBase();
        public RedefinitionUsers(checkUser user)
        {
            InitializeComponent();
            _user = user;
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void RedefinitionUsers_Load(object sender, EventArgs e)
        {
            this.Text = "Управление пользователями";
            metroLabel2.Text = $"{_user.Login}, Role: {_user.Status}";
            CreateColumns();
            RefreshDataGrid();
        }

        private void CreateColumns()
        {
            dataGridView1.Columns.Add("id_user", "ID");
            dataGridView1.Columns.Add("login_user", "Login");
            dataGridView1.Columns.Add("password_user", "Password");
            var checkColumn = new DataGridViewCheckBoxColumn();
            checkColumn.HeaderText = "IsAdmin";
            dataGridView1.Columns.Add(checkColumn);
        }
        private void ReadSingleRows(IDataRecord record)
        {
            dataGridView1.Rows.Add(record.GetInt32(0), record.GetString(1), record.GetString(2), record.GetBoolean(3));
        }
        private void RefreshDataGrid()
        {
            dataGridView1.Rows.Clear();
            string Query = $"SELECT * FROM users";
            SqlCommand cmd = new SqlCommand(Query, DB.GetConnection());
            DB.openConnection();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRows(reader);
            }
            reader.Close();

            DB.closeConnection();
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            DB.openConnection();

            for (int index = 0; index < dataGridView1.Rows.Count; index++)
            {
                var id = dataGridView1.Rows[index].Cells[0].Value.ToString();
                var isadmin = dataGridView1.Rows[index].Cells[3].Value.ToString();

                var ChangeQuery = $"UPDATE users SET is_admin = '{isadmin}' WHERE id_user = '{id}'";

                var command = new SqlCommand(ChangeQuery, DB.GetConnection());
                command.ExecuteNonQuery();
            }

            DB.closeConnection();

            RefreshDataGrid();
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            DB.openConnection();
            var selectedRowIndex = dataGridView1.CurrentCell.RowIndex;
            var id = Convert.ToInt32(dataGridView1.Rows[selectedRowIndex].Cells[0].Value);
            var delete = $"DELETE FROM users WHERE id_user = '{id}'";
            var command = new SqlCommand(delete, DB.GetConnection());
            command.ExecuteNonQuery();
            DB.closeConnection();

            RefreshDataGrid();
        }
    }
}
