using ListConfigure.model;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ListConfigure
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Login Relogin = new Login();

        public MainWindow(string name, string email, string password, string initial, string color, string server)
        {
            InitializeComponent();
            MyUser.Init(name, email, password, initial, color, server);
        }

        private async void ImportClick(object sender, RoutedEventArgs e)
        {
            ConsoleInfo("Setting source directory...");
            ConsoleInput log = MyConfig.SetSource();
            await ConsoleLog(log);
        }        

        private async void RefreshClick(object sender, RoutedEventArgs e)
        {
            ConsoleInfo("Refreshing source directory...");
            ConsoleInput log = MyConfig.GetFiles();
            await ConsoleLog(log);
        }

        private void SignOutButton_Click(object sender, RoutedEventArgs e)
        {
            EnableUI(false);
            ((App)Application.Current).SignOut();
            EnableUI(true);
        }

        private async void Relogin_Click(object sender, RoutedEventArgs e)
        {
            EnableUI(false);
            bool isdev = false;
            if (MyUser.Server.Contains("dev")) isdev = true;
            var res = await Relogin.SignIn(isdev, MyUser.Email, MyUser.Password);
            EnableUI(true);
        }

        private async void Run(object sender, RoutedEventArgs e)
        {
            foreach(ListFile f in MyConfig.ListFiles)
            {
                f.Status = "Ready";
            }
            EnableUI(false);
            ConsoleInfo("Validating files...");
            bool isThereError = false;
            await MyConfig.Init();
            // Validation
            foreach(ListFile f in MyConfig.ListFiles)
            {
                await ConsoleLog(new ConsoleInput(false, "Validating " + f.Path + "..."));
                model.ValidationResult res = model.Validation.ValidateFile(f, IsCsv.IsChecked.Value, IgnoreFirst.IsChecked.Value, EnableNewCategory.IsChecked.Value, EnableReplacing.IsChecked.Value);
                if (res.Status == "Valid")
                {
                    await ConsoleLog(new ConsoleInput(false, "Validating columns..."));
                    res = model.Validation.ValidateCols(f, IsCsv.IsChecked.Value, IgnoreFirst.IsChecked.Value, EnableNewCategory.IsChecked.Value, EnableReplacing.IsChecked.Value);
                    if (res.Status == "Valid")
                    {
                        await ConsoleLog(new ConsoleInput(false, "Validating rows..."));
                        res = model.Validation.ValidateRows(f, IsCsv.IsChecked.Value, IgnoreFirst.IsChecked.Value, EnableNewCategory.IsChecked.Value, EnableReplacing.IsChecked.Value);
                    }
                }
                if (res.Status == "Valid")
                {
                    await ConsoleLog(new ConsoleInput(false, String.Format("List {0}: Validated", f.Name)));
                    f.Status = "Validated";
                }
                else if (res.Status == "Error")
                {
                    await ConsoleLog(new ConsoleInput(true, res.Msg));
                    f.Status = "Error";
                    isThereError = true;
                }
                else
                {
                    await ConsoleLog(new ConsoleInput(false, res.Msg));
                    f.Status = "Skipped";
                }
            }
            // Config http calls
            if (isThereError)
            {
                ConsoleInfo("Configuration Incomplete: Error occurred while validating files.");
                EnableUI(true);
                return;
            }
            ConsoleInfo("Configuring lists through HTTP...");
            int skipcnt = 0;
            int succcnt = 0;
            int failcnt = 0;
            int total = MyConfig.ListFiles.Count;
            foreach (ListFile f in MyConfig.ListFiles)
            {
                var logs = await MyConfig.Configure(f, IsCsv.IsChecked.Value, IgnoreFirst.IsChecked.Value);
                if (logs.Count == 0)
                {
                    ConsoleInfo(String.Format("List {0}: Skipped", f.Name));
                    f.Status = "Skipped";
                    skipcnt += 1;
                }
                else {
                    bool iserror = false;
                    foreach (var log in logs)
                    {
                        await ConsoleLog(log);
                        if (log.Err) iserror = true;
                    }
                    if (iserror)
                    {
                        f.Status = "Failed";
                        failcnt += 1;
                    }
                    else
                    {
                        f.Status = "Succeed";
                        succcnt += 1;
                    }
                }
            }
            ConsoleInfo("Configuration Complete:");
            ConsoleInfo(String.Format("{0} success, {1} failure, {2} skipped out of total {3}", succcnt, failcnt, skipcnt, total));
            EnableUI(true);
        }

        private void EnableUI(Boolean enable)
        {
            MyRing.IsActive = !enable;
            DirSelector.IsEnabled = enable;
            FileListView.IsEnabled = enable;
            RightDock.IsEnabled = enable;
        }        
        
        private void ConsoleInfo(string msg)
        {
            MyConsole.Info(msg);
            ConsoleScroll.ScrollToBottom();
        }

        private void ConsoleError(string msg)
        {
            MyConsole.Error(msg);
            ConsoleScroll.ScrollToBottom();
        }

        private async Task<bool> ConsoleLog(ConsoleInput input)
        {
            await Task.Run(() =>
            {
                if (input != null)
                {
                    MyConsole.Log(input);
                }
            });
            App.Current.Dispatcher.Invoke(new Action(()=>ConsoleScroll.ScrollToBottom()));
            return true;
        }
    }
}
