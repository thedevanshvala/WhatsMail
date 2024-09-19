using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Windows;
using System.Windows.Documents;
using System.Net;
using System.Net.Mail;
using System.Printing;
using System.Data.SQLite;
using System.Windows.Controls;
using System.IO;
using System.Text;
using System.Windows.Input;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration.Attributes;
using CsvHelper.Configuration;

namespace Send_Whatsapp
{
    public partial class MainWindow : Window
    {
        private IWebDriver? webDriver;
        private const string WhatsAppWebUrl = "https://web.whatsapp.com/";
        private DbHelper dbHelper;

        public MainWindow()
        {
            InitializeComponent();
            //InitializeWebDriver();
            dbHelper = new DbHelper(); // Instantiate the helper class
            dbHelper.CreateDatabase(); // Ensure the database and table exist
            LoadContactsToDataGrid();  // Load contacts into DataGrid on startup
        }

        //public void LoadContactsToDataGrid()
        //{
        //    // Load data into the DataGrid
        //    dataGridContacts.ItemsSource = dbHelper.LoadContacts();

        //    // Get the count of items in the DataGrid
        //    int itemCount = dataGridContacts.Items.Count;

        //    //MessageBox.Show($"Total contacts: {itemCount}");
        //}
        public void LoadContactsToDataGrid()
        {
            dataGridContacts.ItemsSource = dbHelper.LoadContacts();
            dataGridContacts.Items.Refresh(); // Refresh the DataGrid to show the updated list
        }



        private void InitializeWebDriver()
        {
            var options = new ChromeOptions();
            options.AddArguments("--start-maximized");
            webDriver = new ChromeDriver(options);
            webDriver.Navigate().GoToUrl(WhatsAppWebUrl);

            // Load cookies if available
            LoadCookies();
        }

        private bool IsLoggedIn()
        {
            try
            {
                // Check if an element that's only present after login exists
                webDriver.FindElement(By.XPath("//div[@title='Search or start new chat']"));
                return true;  // Successfully logged in
            }
            catch (NoSuchElementException)
            {
                return false;  // Not logged in
            }
        }


        //private void btnSendWhatsApp_Click(object sender, RoutedEventArgs e)
        //{
        //    if (listBoxStatus.Visibility == Visibility.Collapsed && Keyboard.IsKeyDown(Key.LeftCtrl) )
        //    {
        //        listBoxStatus.Visibility = Visibility.Visible;
        //    }else if (listBoxStatus.Visibility == Visibility.Visible && Keyboard.IsKeyDown(Key.RightCtrl))
        //    {
        //        listBoxStatus.Visibility = Visibility.Collapsed;
        //    }
        //    // Initialize WebDriver if not already initialized
        //    if (webDriver == null)
        //    {
        //        InitializeWebDriver();
        //    }
        //    // Check if the Ctrl key is pressed
        //    bool isCtrlPressed = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
        //    string[] contactNumbers = txtContactNumberEntry.Text.Split(',');  // Assuming contact numbers are separated by commas
        //    string message = new TextRange(rtbMessage.Document.ContentStart, rtbMessage.Document.ContentEnd).Text;

        //    if (contactNumbers.Length == 0 || string.IsNullOrWhiteSpace(message))
        //    {
        //        MessageBox.Show("Contact number(s) and message cannot be empty.");
        //        return;
        //    }

        //    WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(30));

        //    foreach (string contactNumber in contactNumbers)
        //    {
        //        string trimmedNumber = contactNumber.Trim();

        //        if (string.IsNullOrWhiteSpace(trimmedNumber))
        //            continue;

        //        try
        //        {
        //            // Search for the contact and send the WhatsApp message
        //            var searchInputBox = wait.Until(driver =>
        //            {
        //                try
        //                {
        //                    return driver.FindElement(By.XPath("//div[@class='x1hx0egp x6ikm8r x1odjw0f x6prxxf x1k6rcq7 x1whj5v']"));
        //                }
        //                catch (NoSuchElementException)
        //                {
        //                    return null;
        //                }
        //            });

        //            if (searchInputBox == null)
        //            {
        //                throw new Exception("Search input box not found.");
        //            }

        //            searchInputBox.Clear();
        //            searchInputBox.SendKeys(trimmedNumber);
        //            searchInputBox.SendKeys(Keys.Enter);


        //            var chatInputBox = wait.Until(driver =>
        //            {
        //                try
        //                {
        //                    return driver.FindElement(By.XPath("//div[@aria-placeholder='Type a message']"));
        //                }
        //                catch (NoSuchElementException)
        //                {
        //                    return null;
        //                }
        //            });


        //            if (chatInputBox == null)
        //            {
        //                throw new Exception("Chat input box not found.");
        //            }
        //            var selectedContact = new Contact();
        //                string personalizedMessage = message.Replace("{Name}", selectedContact.Name);
        //            chatInputBox.Click();
        //            chatInputBox.SendKeys(personalizedMessage);
        //            chatInputBox.SendKeys(Keys.Enter);

        //            listBoxStatus.Items.Add($"Message sent to {trimmedNumber}.");
        //            System.Threading.Thread.Sleep(2000);  // Add delay between sending messages

        //        }
        //        catch (Exception ex)
        //        {
        //            listBoxStatus.Items.Add($"Error sending message to {trimmedNumber}: {ex.Message}");
        //        }
        //    }
        //    // If Ctrl key is pressed, send emails as well
        //    if (isCtrlPressed)
        //    {
        //        SendEmailsToSelectedContacts();
        //    }
        //}
        private void btnSendWhatsApp_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxStatus.Visibility == Visibility.Collapsed && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                listBoxStatus.Visibility = Visibility.Visible;
            }
            else if (listBoxStatus.Visibility == Visibility.Visible && Keyboard.IsKeyDown(Key.RightCtrl))
            {
                listBoxStatus.Visibility = Visibility.Collapsed;
            }

            // Initialize WebDriver if not already initialized
            if (webDriver == null)
            {
                InitializeWebDriver();
            }

            // Check if the Ctrl key is pressed
            bool isCtrlPressed = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
            string[] contactNumbers = txtContactNumberEntry.Text.Split(',');  // Assuming contact numbers are separated by commas
            string message = new TextRange(rtbMessage.Document.ContentStart, rtbMessage.Document.ContentEnd).Text;

            if (contactNumbers.Length == 0 || string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show("Contact number(s) and message cannot be empty.");
                return;
            }

            WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(30));

            foreach (string contactNumber in contactNumbers)
            {
                string trimmedNumber = contactNumber.Trim();

                if (string.IsNullOrWhiteSpace(trimmedNumber))
                    continue;

                try
                {
                    // Find the matching contact by phone number
                    var selectedContact = dbHelper.LoadContacts().FirstOrDefault(c => c.ContactNumber == trimmedNumber);

                    if (selectedContact == null)
                    {
                        throw new Exception($"Contact with number {trimmedNumber} not found.");
                    }

                    // Search for the contact and send the WhatsApp message
                    var searchInputBox = wait.Until(driver =>
                    {
                        try
                        {
                            return driver.FindElement(By.XPath("//div[@class='x1hx0egp x6ikm8r x1odjw0f x6prxxf x1k6rcq7 x1whj5v']"));
                        }
                        catch (NoSuchElementException)
                        {
                            return null;
                        }
                    });

                    if (searchInputBox == null)
                    {
                        throw new Exception("Search input box not found.");
                    }

                    searchInputBox.Clear();
                    searchInputBox.SendKeys(trimmedNumber);
                    searchInputBox.SendKeys(Keys.Enter);

                    var chatInputBox = wait.Until(driver =>
                    {
                        try
                        {
                            return driver.FindElement(By.XPath("//div[@aria-placeholder='Type a message']"));
                        }
                        catch (NoSuchElementException)
                        {
                            return null;
                        }
                    });

                    if (chatInputBox == null)
                    {
                        throw new Exception("Chat input box not found.");
                    }

                    // Replace {Name} with the actual name of the selected contact

                    string personalizedMessage = message.Replace("{Name}", selectedContact.Name);

                    //// Add emoji directly to the message (e.g., a smiley face emoji)
                    //personalizedMessage += " :hey";

                    // Split the message into lines based on newline characters
                    string[] messageLines = personalizedMessage.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                    // Click the chat input box to focus
                    chatInputBox.Click();

                    // Loop through each line and send it
                    for (int i = 0; i < messageLines.Length; i++)
                    {
                        // Send the current line
                        chatInputBox.SendKeys(messageLines[i]);

                        // If it's not the last line, add a Shift+Enter for a new line
                        if (i < messageLines.Length - 1)
                        {
                            chatInputBox.SendKeys(Keys.Shift + Keys.Enter);
                        }
                    }

                    chatInputBox.Click();
                    //chatInputBox.SendKeys(personalizedMessage);
                    chatInputBox.SendKeys(Keys.Enter);
                    foreach (var line in messageLines)
                    {
                        if (line.Contains(":clap") || line.Contains(":white heart") || line.Contains(":heart") || line.Contains(":hey"))
                        {
                            chatInputBox.SendKeys(Keys.Enter);
                            break; // Exit the loop once found and Enter is pressed
                        }
                    }


                    listBoxStatus.Items.Add($"Message sent to {selectedContact.Name} ({trimmedNumber}).");
                    System.Threading.Thread.Sleep(2000);  // Add delay between sending messages

                }
                catch (Exception ex)
                {
                    listBoxStatus.Items.Add($"Error sending message to {trimmedNumber}: {ex.Message}");
                }
            }

            // If Ctrl key is pressed, send emails as well
            if (isCtrlPressed)
            {
                SendEmailsToSelectedContacts();
            }
        }

        private void btnSendEmails_Click(object sender, RoutedEventArgs e)
        {
            SendEmailsToSelectedContacts();
        }


        private void SendEmailsToSelectedContacts()
        {
            string message = new TextRange(rtbMessage.Document.ContentStart, rtbMessage.Document.ContentEnd).Text.Trim();
            string sub = new TextRange(rtbSubject.Document.ContentStart, rtbSubject.Document.ContentEnd).Text.Trim();
            string subject = sub;
            string body = message;

            // Retrieve the saved credentials
            var credentials = dbHelper.GetSenderCredentials();
            string senderEmail = credentials.Item1;
            string senderPassword = credentials.Item2;

            foreach (var item in dataGridContacts.SelectedItems)
            {
                if (item is Contact selectedContact && !string.IsNullOrWhiteSpace(selectedContact.EmailID))
                {
                    string recipientEmail = selectedContact.EmailID.Trim();

                    if (!recipientEmail.Contains("@"))
                    {
                        listBoxStatus.Items.Add($"Invalid email for {selectedContact.Name}: {recipientEmail}");
                        continue;
                    }

                    try
                    {
                        string personalizedBody = body.Replace("{Name}", selectedContact.Name);
                        MailMessage mail = new MailMessage
                        {
                            From = new MailAddress(senderEmail),
                            Subject = subject,
                            Body = personalizedBody,
                            IsBodyHtml = true
                        };

                        mail.To.Add(recipientEmail);

                        SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)
                        {
                            Credentials = new NetworkCredential(senderEmail, senderPassword),
                            EnableSsl = true
                        };

                        smtpClient.Send(mail);
                        listBoxStatus.Items.Add($"Email sent to {recipientEmail}.");
                    }
                    catch (SmtpException smtpEx)
                    {
                        listBoxStatus.Items.Add($"SMTP error for {recipientEmail}: {smtpEx.Message}");
                    }
                    catch (Exception ex)
                    {
                        listBoxStatus.Items.Add($"Error sending email to {recipientEmail}: {ex.Message}");
                    }
                }
            }
        }

        private void CopyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxStatus.SelectedItem != null)
            {
                Clipboard.SetText(listBoxStatus.SelectedItem.ToString());
            }
            else
            {
                MessageBox.Show("No item selected to copy.");
            }
        }

        private void SaveCookies()
        {
            // Ensure the browser is logged in before saving cookies
            var cookies = webDriver.Manage().Cookies.AllCookies;
            using (StreamWriter file = new StreamWriter("cookies.txt"))
            {
                foreach (var cookie in cookies)
                {
                    file.WriteLine($"{cookie.Name};{cookie.Value};{cookie.Domain};{cookie.Path};{cookie.Expiry};{cookie.Secure}");
                }
            }
        }

        private void LoadCookies()
        {
            if (File.Exists("cookies.txt"))
            {
                using (StreamReader file = new StreamReader("cookies.txt"))
                {
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        var cookieDetails = line.Split(';');

                        // Parse expiry date if it's not null
                        DateTime? expiry = null;
                        if (DateTime.TryParse(cookieDetails[4], out DateTime parsedExpiry))
                        {
                            expiry = parsedExpiry;
                        }

                        // Create a new Selenium cookie with all required parameters
                        OpenQA.Selenium.Cookie cookie = new OpenQA.Selenium.Cookie(
                            cookieDetails[0],  // Name
                            cookieDetails[1],  // Value
                            cookieDetails[2],  // Domain
                            cookieDetails[3],  // Path
                            expiry,            // Expiry (can be null)
                            cookieDetails[5] == "True",  // Secure flag
                            false,  // isHttpOnly (set to false, you can adjust based on your needs)
                            "Lax"   // SameSite policy (you can adjust if needed)
                        );

                        webDriver.Manage().Cookies.AddCookie(cookie);
                    }
                }

                // Refresh after loading cookies to apply them
                webDriver.Navigate().Refresh();
            }
        }



        private List<Contact> LoadContacts()
        {
            string dbPath = "Data Source=contacts.db;Version=3;";
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

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                // Make Delete All button visible
                if (btnImport.Visibility == Visibility.Collapsed)
                {
                    btnImport.Visibility = Visibility.Visible;
                }else if(btnImport.Visibility == Visibility.Visible)
                {
                    btnImport.Visibility = Visibility.Collapsed;
                }
                // Call the Delete All Contacts functionality
                //btnDeleteAll_Click(sender, e);
            }
            else
            {
                string name = txtName.Text.Trim();
                string contactNumber = txtContactNumber.Text.Trim();
                string emailID = txtEmailEntry.Text.Trim();

                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(contactNumber) || string.IsNullOrWhiteSpace(emailID))
                {
                    MessageBox.Show("Please fill all the fields.");
                    return;
                }

                dbHelper.AddContact(name, contactNumber, emailID);
                MessageBox.Show("Contact added successfully!");
            }
                LoadContactsToDataGrid();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridContacts.SelectedItem is Contact selectedContact)
            {
                string name = txtName.Text.Trim();
                string contactNumber = txtContactNumberEntry.Text.Trim();
                string emailID = txtEmailEntry.Text.Trim();

                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(contactNumber) || string.IsNullOrWhiteSpace(emailID))
                {
                    MessageBox.Show("Please fill all the fields.");
                    return;
                }

                dbHelper.UpdateContact(selectedContact.Id, name, contactNumber, emailID);
                MessageBox.Show("Contact updated successfully!");

                LoadContactsToDataGrid();
            }
            else
            {
                MessageBox.Show("Please select a contact to update.");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check if Ctrl key is pressedbtnDeleteAll
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    if (btnDeleteAll.Visibility == Visibility.Collapsed)
                    {
                        btnDeleteAll.Visibility = Visibility.Visible;
                    }
                    else if (btnDeleteAll.Visibility == Visibility.Visible)
                    {
                        btnDeleteAll.Visibility = Visibility.Collapsed;
                    }
                    // Call the Delete All Contacts functionality
                    //btnDeleteAll_Click(sender, e);
                }
                else if (btnDeleteAll.Visibility == Visibility.Visible)
                {
                    btnDeleteAll.Visibility = Visibility.Collapsed;
                }
                else
                {
                    // Normal delete functionality
                    if (dataGridContacts.SelectedItem is Contact selectedContact)
                    {
                        // Attempt to delete the contact
                        dbHelper.DeleteContact(selectedContact.Id);

                        // Optionally, you could check if the contact still exists to determine if deletion was successful
                        MessageBox.Show("Contact deleted successfully!");

                        // Reload contacts into DataGrid
                        LoadContactsToDataGrid();
                    }
                    else
                    {
                        MessageBox.Show("Please select a contact to delete.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any unexpected errors
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void dataGridContacts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Initialize StringBuilders for contact numbers and email IDs
            var contactNumberSb = new StringBuilder();
            var emailIdSb = new StringBuilder();

            // Iterate through all selected items in the DataGrid
            foreach (var item in dataGridContacts.SelectedItems)
            {
                if (item is Contact selectedContact)
                {
                    // Extract the selected contact's number and email
                    string contactNumber = selectedContact.ContactNumber;
                    string emailID = selectedContact.EmailID;

                    // Append contact numbers if they exist
                    if (!string.IsNullOrWhiteSpace(contactNumber))
                    {
                        if (contactNumberSb.Length > 0)
                        {
                            contactNumberSb.Append(", ");
                        }
                        contactNumberSb.Append(contactNumber);
                    }

                    // Append email IDs if they exist
                    if (!string.IsNullOrWhiteSpace(emailID))
                    {
                        if (emailIdSb.Length > 0)
                        {
                            emailIdSb.Append(", ");
                        }
                        emailIdSb.Append(emailID);
                    }
                }
            }

            // Set the updated text to txtContactNumberEntry and txtEmailIDEntry
            txtContactNumberEntry.Text = contactNumberSb.ToString();
            txtEmailIDEntry.Text = emailIdSb.ToString();
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            // Open File Dialog to select Excel or CSV
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Excel Files|*.xlsx|CSV Files|*.csv|All Files|*.*",
                Title = "Select a file to import"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string fileExtension = System.IO.Path.GetExtension(filePath).ToLower();

                try
                {
                    if (fileExtension == ".xlsx")
                    {
                        ImportFromExcel(filePath); // Handle Excel file
                    }
                    else if (fileExtension == ".csv")
                    {
                        ImportFromCsv(filePath); // Handle CSV file
                    }
                    else
                    {
                        MessageBox.Show("Unsupported file format. Please select a CSV or Excel file.");
                    }

                    MessageBox.Show("Contacts imported successfully.");
                    LoadContactsToDataGrid(); // Refresh DataGrid after import
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error during import: {ex.Message}");
                }
            }
        }
        private void ImportFromExcel(string filePath)
        {
            using (var package = new OfficeOpenXml.ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Read the first worksheet
                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++) // Assuming the first row contains headers
                {
                    string name = worksheet.Cells[row, 1].Value?.ToString().Trim();       // Name in the first column
                    string contactNumber = worksheet.Cells[row, 2].Value?.ToString().Trim(); // Contact Number in the second column
                    string emailId = worksheet.Cells[row, 3].Value?.ToString().Trim();     // Email ID in the third column

                    // Validate fields before inserting into the database
                    if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(contactNumber) && !string.IsNullOrWhiteSpace(emailId))
                    {
                        dbHelper.AddContact(name, contactNumber, emailId);
                    }
                }
            }
        }
        private void ImportFromCsv(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<ContactMap>(); // Register the class map (optional if manual mapping is used)
                var contacts = csv.GetRecords<Contact>().ToList();
                foreach (var contact in contacts)
                {
                    dbHelper.AddContact(contact.Name, contact.ContactNumber, contact.EmailID); // Match the property name
                }
                MessageBox.Show("Contacts imported successfully from CSV!");
                LoadContactsToDataGrid();
            }
        }

        // Optional: Manual column mapping
        public sealed class ContactMap : ClassMap<Contact>
        {
            public ContactMap()
            {
                Map(m => m.Name).Name("Name");
                Map(m => m.ContactNumber).Name("ContactNumber");
                Map(m => m.EmailID).Name("EmailID");
            }
        }


        // Define a model to map CSV fields
        public class ContactCsvModel
        {
            public string Name { get; set; }
            public string ContactNumber { get; set; }
            public string EmailId { get; set; }
        }

            private void btnDeleteAll_Click(object sender, RoutedEventArgs e)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete all contacts?", "Delete All", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    dbHelper.DeleteAllContacts();
                    MessageBox.Show("All contacts deleted successfully!");

                    LoadContactsToDataGrid(); // Refresh DataGrid after deletion
                }
            }
    }

    public class Contact
    {
        public int Id { get; set; }
        [Name("Name")]
        public string Name { get; set; }
        [Name("ContactNumber")]
        public string ContactNumber { get; set; }
        [Name("EmailID")]
        public string EmailID { get; set; }
        public bool SendOnDetails { get; set; } // This is for the checkbox selection
    }

    // Assume DbHelper is properly implemented for managing SQLite database operations
}