using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;

namespace Send_Whatsapp
{
    public class DbHelper
    {
        private string dbPath = "Data Source=contacts.db;Version=3;";

        public DbHelper()
        {
            CreateDatabase();
        }

        // Create the database with Contacts and EmailCredentials tables
        public void CreateDatabase()
        {
            using (var connection = new SQLiteConnection(dbPath))
            {
                connection.Open();

                // Create Contacts Table
                string createContactsTable = @"CREATE TABLE IF NOT EXISTS Contacts (
                                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                                Name TEXT NOT NULL,
                                                ContactNumber TEXT NOT NULL,
                                                EmailId TEXT NOT NULL
                                              );";
                using (var command = new SQLiteCommand(createContactsTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                // Create EmailCredentials Table
                string createCredentialsTable = @"CREATE TABLE IF NOT EXISTS EmailCredentials (
                                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                                    SenderEmail TEXT NOT NULL,
                                                    SenderPassword TEXT NOT NULL
                                                  );";
                using (var command = new SQLiteCommand(createCredentialsTable, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        // Save sender email and password (clear existing credentials first)
        public void SaveSenderCredentials(string email, string password)
        {
            using (var connection = new SQLiteConnection(dbPath))
            {
                connection.Open();

                // Clear previous credentials
                string deleteQuery = "DELETE FROM EmailCredentials";
                using (var deleteCmd = new SQLiteCommand(deleteQuery, connection))
                {
                    deleteCmd.ExecuteNonQuery();
                }

                // Insert new credentials
                string insertQuery = "INSERT INTO EmailCredentials (SenderEmail, SenderPassword) VALUES (@Email, @Password)";
                using (var insertCmd = new SQLiteCommand(insertQuery, connection))
                {
                    insertCmd.Parameters.AddWithValue("@Email", email);
                    insertCmd.Parameters.AddWithValue("@Password", password);
                    insertCmd.ExecuteNonQuery();
                }
            }
        }

        // Retrieve stored sender credentials
        public (string, string) GetSenderCredentials()
        {
            using (var connection = new SQLiteConnection(dbPath))
            {
                connection.Open();
                string selectQuery = "SELECT SenderEmail, SenderPassword FROM EmailCredentials LIMIT 1";

                using (var command = new SQLiteCommand(selectQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string email = reader.GetString(0);
                            string password = reader.GetString(1);
                            return (email, password);
                        }
                    }
                }
            }
            return (string.Empty, string.Empty); // Return empty if no credentials found
        }

        // Load Contacts from the database
        public List<Contact> LoadContacts()
        {
            List<Contact> contacts = new List<Contact>();

            using (var connection = new SQLiteConnection(dbPath))
            {
                connection.Open();
                string selectQuery = "SELECT * FROM Contacts";

                using (var command = new SQLiteCommand(selectQuery, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            contacts.Add(new Contact
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                ContactNumber = reader.GetString(2),
                                EmailID = reader.GetString(3)
                            });
                        }
                    }
                }
            }
            return contacts;
        }

        // Add Contact to the database
        public void AddContact(string name, string contactNumber, string emailId)
        {
            using (var connection = new SQLiteConnection(dbPath))
            {
                connection.Open();

                string insertQuery = "INSERT INTO Contacts (Name, ContactNumber, EmailId) VALUES (@Name, @ContactNumber, @EmailId)";
                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@ContactNumber", contactNumber);
                    command.Parameters.AddWithValue("@EmailId", emailId);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Update an existing contact
        public void UpdateContact(int id, string name, string contactNumber, string emailId)
        {
            using (var connection = new SQLiteConnection(dbPath))
            {
                connection.Open();

                string updateQuery = "UPDATE Contacts SET Name = @Name, ContactNumber = @ContactNumber, EmailId = @EmailId WHERE Id = @Id";
                using (var command = new SQLiteCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@ContactNumber", contactNumber);
                    command.Parameters.AddWithValue("@EmailId", emailId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteAllContacts()
        {
            using (var connection = new SQLiteConnection(dbPath))
            {
                connection.Open();

                string deleteAllQuery = "DELETE FROM Contacts";
                using (var command = new SQLiteCommand(deleteAllQuery, connection))
                {
                    int rowsAffected = command.ExecuteNonQuery();
                    MessageBox.Show($"{rowsAffected} contact(s) deleted.");
                }
            }
        }


        // Delete a contact from the database
        public bool DeleteContact(int contactId)
        {
            try
            {
                using (var connection = new SQLiteConnection(dbPath))
                {
                    connection.Open();
                    string query = "DELETE FROM Contacts WHERE Id = @Id";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", contactId);
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0; // Return true if a row was deleted
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                MessageBox.Show($"An error occurred while deleting the contact: {ex.Message}");
                return false;
            }
        }
    }
}
