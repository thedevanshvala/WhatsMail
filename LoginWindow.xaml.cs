using System.Windows;

namespace Send_Whatsapp
{
    public partial class LoginWindow : Window
    {
        private DbHelper dbHelper;

        public LoginWindow()
        {
            InitializeComponent();
            dbHelper = new DbHelper();

            // Check if credentials already exist
            var credentials = dbHelper.GetSenderCredentials();
            if (!string.IsNullOrEmpty(credentials.Item1) && !string.IsNullOrEmpty(credentials.Item2))
            {
                // Credentials exist, so go directly to the main window
                OpenMainWindowAndCloseLogin();
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both email and password.");
                return;
            }

            // Store credentials in the database
            dbHelper.SaveSenderCredentials(email, password);

            // Proceed to the main window
            OpenMainWindowAndCloseLogin();
        }

        private void OpenMainWindowAndCloseLogin()
        {
            // Create and show the MainWindow
            var mainWindow = new MainWindow();
            mainWindow.Show();

            // Close the LoginWindow on the Dispatcher to avoid potential cross-threading issues
            Dispatcher.BeginInvoke(new Action(() =>
            {
                this.Close();
            }), System.Windows.Threading.DispatcherPriority.Background);
        }
    }
}
