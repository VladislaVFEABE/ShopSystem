using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;
using MetroFramework.Controls;
using MetroFramework.Forms;

namespace Shop
{
    enum RowState
    {
        Existed,
        New,
        Modifed,
        ModifedNew,
        Deleted
    }

    public partial class AdminMenu : MetroForm
    {
        int selectedRow;
        private readonly checkUser _user;
       
        private DataBase DB;
        public AdminMenu(checkUser user)
        {
            InitializeComponent();
            _user = user;
            StartPosition = FormStartPosition.CenterScreen;
            DB = new DataBase(); // Инициализация объекта DataBase
        }

        private void AdminMenu_Load(object sender, EventArgs e)
        {
            this.Text = "Админ меню";
            metroLabel2.Text = $"{_user.Login}, Role: {_user.Status}";
            CreateColumns();

        }

        /*создание колонок для таблицы datagridview*/
        private void CreateColumns()
        {
            dataGridView1.Columns.Add("ProductID", "id");
            dataGridView1.Columns.Add("ProductName", "Наименование");
            dataGridView1.Columns.Add("ProductDescription", "Описание");
            dataGridView1.Columns.Add("ProductPrice", "Цена");
            dataGridView1.Columns.Add("ProductPhoto", "Фото");
            dataGridView1.Columns.Add("IsNew", String.Empty);
        }
        /*передача параметров которые еще не использовались*/
        private void ReadSingleRows(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(record.GetInt32(0), record.GetString(1), record.GetString(2), record.GetDecimal(3), record.GetValue(4), RowState.ModifedNew);
        }

        private void RefreshDataGrid(DataGridView dgw, DataBase DB)
        {
            dgw.Rows.Clear();

            // Открываем подключение
            DB.openConnection();

            // Запрос к базе данных
            string queryString = "SELECT * FROM Products";

            // Используем текущее подключение из класса DataBase
            using (SqlCommand command = new SqlCommand(queryString, DB.GetConnection()))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ReadSingleRows(dgw, reader);
                    }
                }
            }

            // Закрываем подключение
            DB.closeConnection();
        }





        //Поиск по БД
        private void Search(DataGridView dgw, string connectionString)
        {
            dgw.Rows.Clear();
            string searchString = $"SELECT * FROM Products WHERE CONCAT(ProductID, ProductName, ProductDescription, ProductPrice, ProductPhoto) LIKE '%" + metroTextBox1.Text + "%'";

            // Создаем подключение с использованием переданной строки подключения
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Открываем подключение
                connection.Open();

                // Создаем команду с запросом и подключением
                using (SqlCommand command = new SqlCommand(searchString, connection))
                {
                    // Создаем объект для чтения данных
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Читаем данные и добавляем их в DataGridView
                        while (reader.Read())
                        {
                            ReadSingleRows(dgw, reader);
                        }
                    }
                }
            }
        }

        private void metroTextBox1_TextChanged(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=DESKTOP-EG8M5CN\DESKTOP;Initial Catalog=ShopSystem;Integrated Security=True";
            Search(dataGridView1, connectionString);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;

            if(e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];
                // Заполнение текстовых полей данными из выбранной строки
                metroTextBox2.Text = row.Cells[0].Value.ToString(); // ProductID
                metroTextBox3.Text = row.Cells[1].Value.ToString(); // ProductName
                metroTextBox4.Text = row.Cells[2].Value.ToString(); // ProductDescription
                metroTextBox5.Text = row.Cells[3].Value.ToString(); // ProductPrice
                // Получаем бинарные данные изображения и преобразуем их в массив байтов
                byte[] imageData = (byte[])row.Cells[4].Value;

                // Преобразование массива байтов в изображение и отображение его в PictureBox
                using (MemoryStream ms = new MemoryStream(imageData))
                {
                    pictureBox1.Image = Image.FromStream(ms);
                }
            }
        }

        private void DeleteRow()
        {
            int index = dataGridView1.CurrentCell.RowIndex;
            dataGridView1.Rows[index].Visible = false;
            if (dataGridView1.Rows[index].Cells[0].Value.ToString() == string.Empty)
            {
                dataGridView1.Rows[index].Cells[5].Value = RowState.Deleted;
                return;
            }

            dataGridView1.Rows[index].Cells[5].Value = RowState.Deleted;
        }

        private void Update()
        {
            DB.openConnection();
            for(int index = 0; index < dataGridView1.Rows.Count; index++)
            {
                var RowState = (RowState)dataGridView1.Rows[index].Cells[5].Value;

                if (RowState == RowState.Existed)
                    continue;

                if(RowState == RowState.Deleted)
                {
                    var id = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value);
                    var DelQuery = $"DELETE FROM Products WHERE ProductID = {id}";
                    var cmd = new SqlCommand(DelQuery, DB.GetConnection());
                    cmd.ExecuteNonQuery();
                }

                if (RowState == RowState.Modifed)
                {
                    var id = dataGridView1.Rows[index].Cells[0].Value.ToString();
                    var productName = dataGridView1.Rows[index].Cells[1].Value.ToString();
                    var productDescription = dataGridView1.Rows[index].Cells[2].Value.ToString();
                    var productPrice = dataGridView1.Rows[index].Cells[3].Value.ToString();

                    // Предполагается, что productPhoto не изменяется в этом коде, поэтому мы его не обновляем

                    var changeQuery = $"UPDATE Products SET ProductName = '{productName}', ProductDescription = '{productDescription}', ProductPrice = '{productPrice}' WHERE ProductID = '{id}'";
                    var command = new SqlCommand(changeQuery, DB.GetConnection());
                    command.ExecuteNonQuery();  
                    
                }
            }


            DB.closeConnection();
        }

        private void Change()
        {
            var selectedRowIndex = dataGridView1.CurrentCell.RowIndex;
            var id = metroTextBox2.Text;
            var name = metroTextBox3.Text;
            var description = metroTextBox4.Text;
            int price;

            if (dataGridView1.Rows[selectedRowIndex].Cells[0].Value.ToString() != string.Empty)
            {
                if (int.TryParse(metroTextBox5.Text, out price))
                {
                    dataGridView1.Rows[selectedRowIndex].SetValues(id, name, description,price);
                    dataGridView1.Rows[selectedRowIndex].Cells[5].Value = RowState.Modifed;
                }
                else
                {
                    MessageBox.Show("Цена должна иметь числовой формат");
                }

            }
        }

        private void ClearFields()
        {
            metroTextBox2.Text = "";
            metroTextBox3.Text = "";
            metroTextBox4.Text = "";
            metroTextBox5.Text = "";
            

        }


        //Кнопка подключиться к БД
        private void metroButton1_Click(object sender, EventArgs e)
        {
            // Подключение к базе данных
            using (SqlConnection connection = DB.GetConnection())
            {
                // Открытие подключения
                DB.openConnection();

                // SQL-запрос для выборки данных
                string queryString = "SELECT * FROM Products";

                // Создание команды для выполнения запроса
                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    // Создание объекта для чтения данных
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Очистка данных в DataGridView
                        dataGridView1.Rows.Clear();

                        // Чтение данных и добавление их в DataGridView
                        while (reader.Read())
                        {
                            ReadSingleRows(dataGridView1, reader);
                        }
                    }
                }
            }
        }

        //Кнопка работа с пользователями
        private void metroButton2_Click(object sender, EventArgs e)
        {
            AddUser adu = new AddUser(_user);
            adu.Show();
        }

        //Кнопка Управление пользователями
        private void metroButton3_Click(object sender, EventArgs e)
        {
            RedefinitionUsers RedfinitUsers = new RedefinitionUsers(_user);
            RedfinitUsers.Show();
        }


        //Кнопка обновить БД
        private void metroButton4_Click(object sender, EventArgs e)
        {
            // Вызываем метод обновления данных с передачей объекта DB
            RefreshDataGrid(dataGridView1, DB);
        }

        //Кнопка добавить товар
        private void metroButton5_Click(object sender, EventArgs e)
        {
            // Обработчик нажатия кнопки "Добавить товар в каталог"
            AddToCatalog ATC = new AddToCatalog(_user);
            ATC.Show();
        }
        //Кнопка удалить
        private void metroButton6_Click(object sender, EventArgs e)
        {
            DeleteRow();
        }
        //Кнопка сохранить
        private void metroButton7_Click(object sender, EventArgs e)
        {
            Update();
        }

        //Кнопка изменить
        private void metroButton8_Click(object sender, EventArgs e)
        {
            Change();
        }
        //Кнопка очистить
        private void metroButton9_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void metroButton10_Click(object sender, EventArgs e)
        {
            // Закрытие подключения к базе данных
            DB.closeConnection();
        }

        //Кнопка импортировать
        private void metroButton11_Click(object sender, EventArgs e)
        {
            // Создаем новый DataTable для хранения данных из DataGridView
            DataTable dataTable = new DataTable();

            // Добавляем столбцы в DataTable
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                dataTable.Columns.Add(column.HeaderText, typeof(string));
            }

            // Добавляем строки в DataTable
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;
                DataRow dataRow = dataTable.NewRow();
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    dataRow[i] = row.Cells[i].Value?.ToString();
                }
                dataTable.Rows.Add(dataRow);
            }

            // Создаем новую книгу Excel с использованием ClosedXML
            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                // Добавляем новый лист в книгу
                var worksheet = workbook.Worksheets.Add("Products");

                // Вставляем DataTable в лист
                var table = worksheet.Cell(1, 1).InsertTable(dataTable);

                // Добавляем границы ко всем ячейкам в таблице
                var range = table.RangeUsed();
                range.Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                range.Style.Border.InsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;

                // Сохраняем книгу в файл
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Excel files|*.xlsx";
                    saveFileDialog.Title = "Сохранить как Excel файл";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        workbook.SaveAs(saveFileDialog.FileName);
                        MessageBox.Show("Данные успешно экспортированы в Excel.", "Экспорт данных", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void metroButton12_Click(object sender, EventArgs e)
        {
            // Создаем объект печати
            PrintDocument printDocument = new PrintDocument();

            // Создаем диалоговое окно для настройки печати
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = printDocument;

            // Если пользователь выбрал принтер и нажал ОК в диалоговом окне, печатаем
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                // Устанавливаем обработчик события для печати
                printDocument.PrintPage += (s, ev) =>
                {
                    // Рассчитываем высоту строки
                    int rowHeight = (int)dataGridView1.Rows[0].Height;

                    // Рассчитываем координаты для печати данных
                    int x = ev.MarginBounds.Left;
                    int y = ev.MarginBounds.Top;

                    // Печатаем заголовки столбцов
                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    {
                        ev.Graphics.DrawString(dataGridView1.Columns[i].HeaderText, dataGridView1.Font, Brushes.Black, new Point(x, y));
                        x += dataGridView1.Columns[i].Width;
                    }

                    // Печатаем данные из DataGridView
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        y += rowHeight;
                        x = ev.MarginBounds.Left;
                        for (int j = 0; j < dataGridView1.Columns.Count; j++)
                        {
                            ev.Graphics.DrawString(dataGridView1.Rows[i].Cells[j].Value?.ToString(), dataGridView1.Font, Brushes.Black, new Point(x, y));
                            x += dataGridView1.Columns[j].Width;
                        }
                    }
                };

                // Выполняем печать
                printDocument.Print();
            }
        }
    }
}
