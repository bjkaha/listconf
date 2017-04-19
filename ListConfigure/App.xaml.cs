using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ListConfigure
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private LoginWindow login = null;
        private MainWindow main = null;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            login = new LoginWindow();
            login.Show();
        }

        public void Authenticated(string name, string email, string password, string initial, string color, string server)
        {            
            main = new MainWindow(name, email, password, initial, color, server);
            login.Close();
            main.Show();
        }

        public void SignOut()
        {
            login = new LoginWindow();
            main.Close();
            login.Show();
        }
    }

}
