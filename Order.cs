using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Spreadsheet;
using MetroFramework;
using MetroFramework.Controls;
using MetroFramework.Forms;

namespace Shop
{
    public partial class Order : MetroForm
    {
        private readonly checkUser _user;
        DataBase DB = new DataBase();

        public Order(checkUser user)
        {
            InitializeComponent();
            _user = user;
            StartPosition = FormStartPosition.CenterScreen;
            // Вызываем метод для заполнения комбобокса данными из базы данных
            FillComboBox();
        }

        private void Order_Load(object sender, EventArgs e)
        {
            this.Text = "Оформление заказа";
            metroLabel2.Text = $"{_user.Login}, Role: {_user.Status}";
        }


        private void FillComboBox()
        {
            try
            {
                DB.openConnection(); // Открываем подключение к базе данных

                string query = "SELECT ProductName, ProductPrice FROM Products";
                SqlCommand command = new SqlCommand(query, DB.GetConnection()); // Создаем команду для выполнения запроса
                SqlDataReader reader = command.ExecuteReader(); // Получаем данные из базы данных

                // Читаем результаты запроса и добавляем элементы в комбобокс
                while (reader.Read())
                {
                    string productName = reader.GetString(0);
                    decimal productPrice = reader.GetDecimal(1);
                    string itemText = $"{productName}, {productPrice} руб."; // Формируем текст элемента комбобокса
                    metroComboBox1.Items.Add(itemText);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при заполнении комбобокса: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                DB.closeConnection(); // В любом случае закрываем подключение к базе данных
            }
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            try
            {
                decimal width = decimal.Parse(metroTextBox1.Text);
                decimal height = decimal.Parse(metroTextBox2.Text);

                string selectedProduct = metroComboBox1.SelectedItem.ToString().Split(',')[0]; // Получаем только имя продукта
                decimal productPrice = decimal.Parse(metroComboBox1.SelectedItem.ToString().Split(',')[1]);

                decimal finalCost = Math.Round(productPrice * width * height * 17, 2);
                DialogResult result = MessageBox.Show($"Итоговая стоимость: {finalCost} руб. Хотите перенести введенные данные в корзину?", "Расчет стоимости", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                {
                    // Получаем изображение товара из базы данных
                    byte[] productImageData = GetProductImageDataFromDatabase(selectedProduct);

                    if (productImageData != null && productImageData.Length > 0)
                    {
                        // Переходим к корзине и передаем данные о товаре
                        ShopingCard shoppingCardForm = new ShopingCard(_user, width, height, selectedProduct, finalCost, productImageData);
                        shoppingCardForm.Show();
                    }
                    else
                    {
                        MessageBox.Show("Данные изображения отсутствуют или повреждены.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при расчете стоимости или добавлении данных в корзину: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private byte[] GetProductImageDataFromDatabase(string selectedProduct)
        {
            byte[] imageData = null;
            try
            {
                DB.openConnection();

                string query = "SELECT ProductPhoto FROM Products WHERE ProductName = @ProductName";
                SqlCommand command = new SqlCommand(query, DB.GetConnection());
                command.Parameters.AddWithValue("@ProductName", selectedProduct); // Исправлено на @ProductName
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    imageData = (byte[])reader["ProductPhoto"];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении изображения товара из базы данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                DB.closeConnection();
            }
            return imageData;
        }

        private void metroComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
