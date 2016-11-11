using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApplication
{
    /// <summary>
    ///  Класс Form1
    ///  основной класс программы
    ///  предназначен для работы с базой данных
    /// </summary>
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            db = new ModelDBContainer();
        }
        ModelDBContainer db;

        private void Form1_Load(object sender, EventArgs e)
        {
            // Подгружаем данные из БД в таблицу
            db.ProductsSet.Load();
            dataGridView1.DataSource = db.ProductsSet.Local.ToBindingList();
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку для добавления значения в БД buttonAdd_Click(object sender, EventArgs e) 
        /// </summary>
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (textBoxName.Text == String.Empty || textBoxCost.Text == String.Empty || textBoxAge.Text == String.Empty)
            {
                MessageBox.Show("Текстовые поля не заполнены!");
                return;
            }
            Products product = new Products()
            {
                Product = textBoxName.Text,
                Cost = Convert.ToInt32(textBoxCost.Text),
                Age = Convert.ToInt16(textBoxAge.Text),
                Dimensions = Dimensions.Text,
                Count = ((textBoxCount.Text != string.Empty) ? Convert.ToInt32(textBoxCount.Text) : (int?)null),
                Weigth = ((textBoxWeigth.Text != string.Empty) ? Convert.ToInt32(textBoxWeigth.Text) : (int?)null),
                Notes = textBoxNotes.Text
            };
            db.ProductsSet.Add(product);
            db.SaveChanges();
            dataGridView1.Refresh();
        }

        /// <summary>
        /// Обработчик события выбора ячейки в таблице dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e) 
        /// </summary>
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;

            var product = dataGridView1.CurrentRow.DataBoundItem as Products;

            if (product == null) return;

            labelID.Text = Convert.ToString(product.Id);
            textBoxName.Text = product.Product;
            textBoxCost.Text = Convert.ToString(product.Cost);
            textBoxAge.Text = Convert.ToString(product.Age);
            Dimensions.Text = product.Dimensions;
            textBoxCount.Text = Convert.ToString(product.Count);
            textBoxWeigth.Text = Convert.ToString(product.Weigth);
            textBoxNotes.Text = product.Notes;
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку для изменения значения в БД buttonMod_Click(object sender, EventArgs e) 
        /// </summary>
        private void buttonMod_Click(object sender, EventArgs e)
        {
            if (labelID.Text == String.Empty) return;

            var id = Convert.ToInt32(labelID.Text);
            var product = db.ProductsSet.Find(id);

            if (product == null) return;

            product.Product = textBoxName.Text;
            product.Cost = Convert.ToInt32(textBoxCost.Text);
            product.Age = Convert.ToInt16(textBoxAge.Text);
            product.Dimensions = Dimensions.Text;
            product.Count = ((textBoxCount.Text != string.Empty) ? Convert.ToInt32(textBoxCount.Text) : (int?)null);
            product.Weigth = ((textBoxWeigth.Text != string.Empty) ? Convert.ToInt32(textBoxWeigth.Text) : (int?)null);
            product.Notes = textBoxNotes.Text;

            //db.Entry(product).State = EntityState.Modified;
            db.ProductsSet.AddOrUpdate(product);

            db.SaveChanges();

            dataGridView1.Refresh();

        }

        /// <summary>
        /// Обработчик события нажатия на кнопку для удаления значения в БД buttonDelete_Click(object sender, EventArgs e) 
        /// </summary>
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (labelID.Text == String.Empty) return;

            var id = Convert.ToInt32(labelID.Text);
            var product = db.ProductsSet.Find(id);

            db.Entry(product).State = EntityState.Deleted;
            //db.ProductsSet.Remove(product);
            db.SaveChanges();
            product.Id = -1;
            dataGridView1.Refresh();
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку для сохранения выборки из БД в файл buttonSave_Click(object sender, EventArgs e) 
        /// </summary>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            var products = db.ProductsSet.OrderBy(c => c.Cost).ToList();

            var file = new FileInfo("Text.txt");

            StreamWriter writer = file.CreateText();
            foreach (var item in products)
            {
                writer.WriteLine(string.Format("{0,7}  Цена: {1,4}грн. Возраст: {2,2}+ Примечания: {3}", item.Product, item.Cost, item.Age, item.Notes));
            }

            writer.Close();
        }
    }
}
