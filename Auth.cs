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
    public partial class Auth : MetroForm
    {
        DataBase DB = new DataBase();
        public Auth()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;//открытие новой формы по центру экрана
        }

        private void Auth_Load(object sender, EventArgs e)
        {
            this.Text = "Авторизация";
            metroTextBox1.MaxLength = 50; /* установка максимум символов для полей*/
            metroTextBox2.MaxLength = 50;
            metroTextBox2.PasswordChar = '*';
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            var login = metroTextBox1.Text;
            var password = metroTextBox2.Text;

            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable dt = new DataTable();

            string Query = $"SELECT id_user,login_user,password_user, is_admin FROM users WHERE login_user = '{login}' AND password_user = '{password}'"; //*выборка с БД полей login password

            SqlCommand command = new SqlCommand(Query, DB.GetConnection());

            adapter.SelectCommand = command; /*Возвращает или задает инструкцию Transact - SQL или хранимую процедуру, используемую для выбора записей из источника данных*/
            adapter.Fill(dt); /*заполнение объекта table*/

            if (dt.Rows.Count == 1)
            {
                var user = new checkUser(dt.Rows[0].ItemArray[1].ToString(), Convert.ToBoolean(dt.Rows[0].ItemArray[3]));
                MessageBox.Show($"Вы авторизовались, как: {login}", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Catalog catalog = new Catalog(user);
                this.Hide();
                catalog.ShowDialog();
                this.Show();
            }
            else
            {
                MessageBox.Show($"Введен неправильно логин или пароль", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
