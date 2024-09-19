using System.Windows;

namespace Send_Whatsapp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DbHelper dbHelper = new DbHelper();

            // Check if credentials already exist
            var credentials = dbHelper.GetSenderCredentials();
            if (!string.IsNullOrEmpty(credentials.Item1) && !string.IsNullOrEmpty(credentials.Item2))
            {
                // Credentials exist, so go directly to the MainWindow
                //var mainWindow = new MainWindow();
                //mainWindow.Show();
            }
            else
            {
                // Show the LoginWindow first
                var loginWindow = new LoginWindow();
                loginWindow.Show();
            }
        }
    }
}
