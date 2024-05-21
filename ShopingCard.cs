using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace Shop
{
    public partial class ShopingCard : MetroForm
    {
        private readonly checkUser _user;
        private readonly decimal _width;
        private readonly decimal _height;
        private readonly string _productName;
        private readonly decimal _finalCost;
        private readonly byte[] _productImage;

        public ShopingCard(checkUser user, decimal width, decimal height, string productName, decimal finalCost, byte[] productImage)
        {
            InitializeComponent();
            _user = user;
            _width = width;
            _height = height;
            _productName = productName;
            _finalCost = finalCost;
            _productImage = productImage; // Сохраняем изображение товара
            StartPosition = FormStartPosition.CenterScreen;

            // Отображаем информацию о товаре
            metroLabel3.Text = $"Ширина: {_width}";
            metroLabel4.Text = $"Высота: {_height}";
            metroLabel5.Text = $"Товар: {_productName}";
            metroLabel6.Text = $"Итоговая стоимость: {_finalCost} руб.";

            // Добавляем линию перед итоговой стоимостью
            Label lineLabel = new Label();
            lineLabel.AutoSize = false;
            lineLabel.Height = 1;
            lineLabel.Width = metroLabel6.Width;
            lineLabel.BackColor = Color.Black;
            lineLabel.Location = new Point(metroLabel6.Left, metroLabel6.Bottom + 5);
            Controls.Add(lineLabel);

            // Загружаем изображение товара, если оно доступно
            if (_productImage != null && _productImage.Length > 0)
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream(_productImage))
                    {
                        pictureBox1.Image = Image.FromStream(ms);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Данные изображения отсутствуют или повреждены.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShopingCard_Load(object sender, EventArgs e)
        {
            this.Text = "Корзина";
            metroLabel2.Text = $"{_user.Login}, Role: {_user.Status}";
        }
    }
}
