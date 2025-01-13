using System;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Collections.ObjectModel;

namespace Send_Whatsapp
{
    public partial class TagReplacer : Window
    {
        private const string DbPath = "Data Source=tagNcontent.db";
        private ObservableCollection<TagContent> tagsList = new ObservableCollection<TagContent>();

        public TagReplacer()
        {
            InitializeComponent();
            InitializeDatabase();
            LoadData();
            dataGridTags.ItemsSource = tagsList;
        }

        // Initialize database and create table if it doesn't exist
        private void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(DbPath))
            {
                connection.Open();
                string query = "CREATE TABLE IF NOT EXISTS Tags (Id INTEGER PRIMARY KEY AUTOINCREMENT, Tag TEXT, Content TEXT)";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        // Load data from the database into the DataGrid
        private void LoadData()
        {
            tagsList.Clear();
            using (var connection = new SQLiteConnection(DbPath))
            {
                connection.Open();
                string query = "SELECT * FROM Tags";
                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tagsList.Add(new TagContent
                        {
                            Id = reader.GetInt32(0),
                            Tag = reader.GetString(1),
                            Content = reader.GetString(2)
                        });
                    }
                }
            }
        }

        // Add new tag
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            string tag = txtTag.Text;
            string content = txtContent.Text;

            if (string.IsNullOrWhiteSpace(tag) || string.IsNullOrWhiteSpace(content))
            {
                MessageBox.Show("Please enter both Tag and Content.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var connection = new SQLiteConnection(DbPath))
            {
                connection.Open();
                string query = "INSERT INTO Tags (Tag, Content) VALUES (@tag, @content)";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@tag", tag);
                    command.Parameters.AddWithValue("@content", content);
                    command.ExecuteNonQuery();
                }
            }

            LoadData();
            txtTag.Clear();
            txtContent.Clear();
        }

        // Update selected tag
        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridTags.SelectedItem is TagContent selectedTag)
            {
                string tag = txtTag.Text;
                string content = txtContent.Text;

                if (string.IsNullOrWhiteSpace(tag) || string.IsNullOrWhiteSpace(content))
                {
                    MessageBox.Show("Please enter both Tag and Content.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (var connection = new SQLiteConnection(DbPath))
                {
                    connection.Open();
                    string query = "UPDATE Tags SET Tag = @tag, Content = @content WHERE Id = @id";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@tag", tag);
                        command.Parameters.AddWithValue("@content", content);
                        command.Parameters.AddWithValue("@id", selectedTag.Id);
                        command.ExecuteNonQuery();
                    }
                }

                LoadData();
                txtTag.Clear();
                txtContent.Clear();
            }
        }

        // Delete selected tag
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridTags.SelectedItem is TagContent selectedTag)
            {
                using (var connection = new SQLiteConnection(DbPath))
                {
                    connection.Open();
                    string query = "DELETE FROM Tags WHERE Id = @id";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", selectedTag.Id);
                        command.ExecuteNonQuery();
                    }
                }

                LoadData();
                txtTag.Clear();
                txtContent.Clear();
            }
        }

        // Handle DataGrid selection change
        private void DataGridTags_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dataGridTags.SelectedItem is TagContent selectedTag)
            {
                txtTag.Text = selectedTag.Tag;
                txtContent.Text = selectedTag.Content;
            }
        }
    }

    // Class to represent tag-content pairs
    public class TagContent
    {
        public int Id { get; set; }
        public string Tag { get; set; }
        public string Content { get; set; }
    }
}
