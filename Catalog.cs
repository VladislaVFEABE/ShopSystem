using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MetroFramework.Controls;
using MetroFramework.Forms;

namespace Shop
{
    public partial class Catalog : MetroForm
    {
        private readonly string connectionString = @"Data Source=DESKTOP-EG8M5CN\DESKTOP;Initial Catalog=ShopSystem;Integrated Security=True";
        private int totalProducts; // Переменная для хранения общего количества товаров
        private int curIndex = 0; // Текущий индекс товара
        private readonly checkUser _user;
        public Catalog(checkUser user)
        {
            InitializeComponent();
            _user = user;
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void Catalog_Load(object sender, EventArgs e)
        {
            this.Text = "Каталог";
            metroLabel2.Text = $"{_user.Login}, Role: {_user.Status}";

            using (SqlConnection connect = new SqlConnection(connectionString))
            {
                connect.Open();
                string query = "SELECT COUNT(*) FROM Products";
                SqlCommand cmd = new SqlCommand(query, connect);
                totalProducts = (int)cmd.ExecuteScalar();
            }

            LoadProduction();
        }

        private void LoadProduction()
        {
            using (SqlConnection connect = new SqlConnection(connectionString))
            {
                connect.Open();
                string query = "SELECT ProductName, ProductDescription, ProductPrice, ProductPhoto FROM Products";
                SqlCommand cmd = new SqlCommand(query, connect);
                SqlDataReader reader = cmd.ExecuteReader();
                int currentProductIndex = 0;
                while (reader.Read())
                {
                    currentProductIndex++;
                    if (currentProductIndex == curIndex + 1)
                    {
                        metroLabel3.Text = $"Имя: {reader.GetString(0)}";
                        metroLabel4.Text = $"Описание: {reader.GetString(1)}";
                        metroLabel5.Text = $"Цена: {reader.GetDecimal(2).ToString()} руб ПМР";

                        // Отображение фото продукта
                        byte[] imageData = (byte[])reader["ProductPhoto"];
                        if (imageData != null)
                        {
                            using (MemoryStream ms = new MemoryStream(imageData))
                            {
                                pictureBox1.Image = Image.FromStream(ms);
                            }
                        }
                        break; // Прерываем цикл, когда достигнем нужного товара
                    }
                }
                reader.Close();

                // Вывод общего количества товаров на складе
                metroLabel6.Text = $"Всего товаров: {totalProducts}";
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            // Переход к предыдущему товару
            curIndex--;
            if (curIndex < 0)
            {
                curIndex = totalProducts - 1; // Если текущий индекс меньше 0, переходим к последнему товару
            }
            LoadProduction(); // Загружаем данные для нового товара
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            // Переход к следующему товару
            curIndex++;
            if (curIndex >= totalProducts)
            {
                curIndex = 0; // Если текущий индекс больше или равен общему количеству товаров, переходим к первому товару
            }
            LoadProduction(); // Загружаем данные для нового товара
        }

       

        private void metroButton3_Click(object sender, EventArgs e)
        {
            AdminMenu adm = new AdminMenu(_user);
            adm.Show();
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            Order order = new Order(_user);
            order.Show();
        }
    }
}
