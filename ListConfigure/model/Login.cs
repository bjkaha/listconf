using ListConfigure.http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ListConfigure.model
{
    class Login : INotifyPropertyChanged
    {
        private string _error;
        public string Error
        {
            get { return _error; }
            set { Set(ref _error, value); }
        }

        public async Task<Response> SignIn(Boolean isDev, string email, string password)
        {
            Error = null;
            if (isDev) HttpHandler.BaseUri = new Uri("https://dev.mxdeposit.net");
            else HttpHandler.BaseUri = new Uri("https://app.mxdeposit.net");

            var res = await HttpHandler.Request(System.Net.Http.HttpMethod.Post, 
                                                "/login", 
                                                String.Format("{{ \"email\": \"{0}\", \"password\": \"{1}\" }}", email, password));
            if (!res.IsError) // success
            {
                var response = JsonConvert.DeserializeObject<LoginResponse>(res.Json);
                HttpHandler.Database = response.Databases.Count == 0 ? response.ID : response.Databases[0];
                HttpHandler.Token = response.Token;
                Error = null;
            }
            else // error
            {
                Error = JsonConvert.DeserializeObject<ErrorResponse>(res.Json).Error;
            }
            return res;
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

    class LoginResponse
    {
        [JsonProperty("_id")]
        public string ID;
        [JsonProperty("user")]
        public UserInfo User;
        [JsonProperty("databases")]
        public List<string> Databases;
        [JsonProperty("token")]
        public string Token;
    }

    class UserInfo
    {
        [JsonProperty("role")]
        public string Role;
        [JsonProperty("_id")]
        public string ID;
        [JsonProperty("profile")]
        public Profile Profile;
        [JsonProperty("email")]
        public string Email;
    }

    
    class Profile
    {
        [JsonProperty("color")]
        public string Color;
        [JsonProperty("initials")]
        public string Initials;
        [JsonProperty("name")]
        public string Name;
    }    
}
