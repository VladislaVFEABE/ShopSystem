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
    public partial class AddUser : MetroForm
    {
        private readonly checkUser _user;
        DataBase DB = new DataBase();
        public AddUser(checkUser user)
        {
            InitializeComponent();
            _user = user;
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void AddUser_Load(object sender, EventArgs e)
        {
            this.Text = "Добавление пользователя";
            metroLabel2.Text = $"{_user.Login}, Role: {_user.Status}";
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            DB.openConnection();//открытие соединения с БД
            var login = metroTextBox1.Text;
            var password = metroTextBox2.Text;
            var AddQuery = $"INSERT INTO users(login_user, password_user, is_admin) VALUES ('{login}', '{password}',0)";
            var cmd = new SqlCommand(AddQuery, DB.GetConnection());
            cmd.ExecuteNonQuery(); //выполняет sql-выражение и возвращает количество измененных записей.Подходит для sql - выражений INSERT, UPDATE, DELETE, CREATE.
            MessageBox.Show("Юзер добавлен!", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DB.closeConnection();
        }
    }
}
