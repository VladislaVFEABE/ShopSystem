using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MetroFramework.Controls;
using MetroFramework.Forms;


namespace Shop
{
    public partial class AddToCatalog : MetroForm
    {
        private readonly checkUser _user;
        private DataBase DB = new DataBase();
        private Image selectedImage;

        public AddToCatalog(checkUser user)
        {
            InitializeComponent();
            _user = user;
        }

        private void AddToCatalog_Load(object sender, EventArgs e)
        {
            this.Text = "Добавление товара в каталог";
            metroLabel2.Text = $"{_user.Login}, Role: {_user.Status}";
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            DB.openConnection();
            var productName = metroTextBox1.Text;
            var productDescription = metroTextBox2.Text;
            var productPrice = metroTextBox3.Text;

            if (selectedImage != null)
            {
                byte[] productPhotoBytes = ConvertImageToByteArray(selectedImage);
                string query = "INSERT INTO Products (ProductName, ProductDescription, ProductPrice, ProductPhoto) " +
                               "VALUES (@ProductName, @ProductDescription, @ProductPrice, @ProductPhoto)";

                SqlCommand command = new SqlCommand(query, DB.GetConnection());
                command.Parameters.AddWithValue("@ProductName", productName);
                command.Parameters.AddWithValue("@ProductDescription", productDescription);
                command.Parameters.AddWithValue("@ProductPrice", productPrice);
                command.Parameters.AddWithValue("@ProductPhoto", productPhotoBytes);

                command.ExecuteNonQuery();

                MessageBox.Show("Продукт добавлен!", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Выберите изображение для продукта!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            DB.closeConnection();

        }
        private byte[] ConvertImageToByteArray(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        private void metroButton3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.jpg; *.jpeg; *.png; *.gif)|*.jpg; *.jpeg; *.png; *.gif";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                selectedImage = Image.FromFile(openFileDialog.FileName);
                pictureBox1.Image = selectedImage;
            }
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            Catalog catalog = new Catalog(_user);
            catalog.Show();
        }
    }
}
