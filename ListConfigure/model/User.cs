using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ListConfigure.model
{
    class User : INotifyPropertyChanged
    {
        public User() { }

        public void Init(string name, string email, string pw, string initial, string color, string server)
        {
            Name = name; Email = email; Initial = initial; Color = color; Server = server; Password = pw;
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        private string _email;
        public string Email
        {
            get { return _email; }
            set { Set(ref _email, value); }
        }

        private string _initial;
        public string Initial
        {
            get { return _initial; }
            set { Set(ref _initial, value); }
        }

        private string _color;
        public string Color
        {
            get { return _color; }
            set { Set(ref _color, value); }
        }

        private string _server;
        public string Server
        {
            get { return _server; }
            set { Set(ref _server, value); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { Set(ref _password, value); }
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
}
