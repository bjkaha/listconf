using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ListConfigure.model
{
    class Console : INotifyPropertyChanged
    {
        public Console()
        {
            Text =
            "   __ _     _       ___             __ _                       _   _             \n" +
            "  / /(_)___| |_    / __\\___  _ __  / _(_) __ _ _   _ _ __ __ _| |_(_) ___  _ __  \n" +
            " / / | / __| __|  / /  / _ \\| '_ \\| |_| |/ _` | | | | '__/ _` | __| |/ _ \\| '_ \\ \n" +
            "/ /__| \\__ \\ |_  / /__| (_) | | | |  _| | (_| | |_| | | | (_| | |_| | (_) | | | |\n" +
            "\\____/_|___/\\__| \\____/\\___/|_| |_|_| |_|\\__, |\\__,_|_|  \\__,_|\\__|_|\\___/|_| |_|\n" +
            "                                         |___/                                   \n\n";
            Info("Set source directory to start.");
        }

        private string _text;
        public string Text
        {
            get { return _text; }
            set { Set(ref _text, value); }
        }

        public void Info(string txt)
        {
            Text += "[Info] " + txt + "\n";
        }

        public void Error(string txt)
        {
            Text += "[Error] " + txt + "\n";
        }

        public void Log(ConsoleInput input)
        {
            if (input.Err) Error(input.Msg);
            else Info(input.Msg);
        }


        // Data Binding
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    class ConsoleInput
    {
        public ConsoleInput(Boolean err, string msg)
        {
            Err = err;
            Msg = msg;
        }
        public Boolean Err;
        public string Msg;
    }
}
