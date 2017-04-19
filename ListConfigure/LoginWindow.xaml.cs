using ListConfigure.model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ListConfigure
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();            
        }

        private async void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            EnableUI(false);
            var res = await MyLogin.SignIn(IsDev.IsSelected, Email.Text, Password.Password);
            if (!res.IsError)
            {
                var response = JsonConvert.DeserializeObject<LoginResponse>(res.Json);
                string server = IsDev.IsSelected ? "https://dev.mxdeposit.net" : "https://app.mxdeposit.net";
                ((App)Application.Current).Authenticated(response.User.Profile.Name, response.User.Email, Password.Password, response.User.Profile.Initials, response.User.Profile.Color, server);
            }
            EnableUI(true);
        }

        private void EnableUI(Boolean enable)
        {
            Server.IsEnabled = enable;
            SignInButton.IsEnabled = enable;
            Email.IsEnabled = enable;
            Password.IsEnabled = enable;
        }
    }
}
