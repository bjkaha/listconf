using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;

namespace ListConfigure.http
{
    static class HttpHandler
    {
        private static string _token;
        public static string Token
        {
            get { return _token; }
            set { _token = value; }
        }

        private static string _database;
        public static string Database
        {
            get { return _database; }
            set { _database = value; }
        }

        private static Uri _baseUri = new Uri("https://dev.mxdeposit.net");
        public static Uri BaseUri
        {
            get { return _baseUri; }
            set { _baseUri = value; }
        }

        public static async Task<Response> Request(HttpMethod method, string subUrl, string json)
        {
            using (HttpClient client = new HttpClient { BaseAddress = BaseUri })
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                if (Token != null) client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", Token);
                if (Database != null) client.DefaultRequestHeaders.TryAddWithoutValidation("database", Database);
                try
                {
                    HttpResponseMessage res;
                    if (method == HttpMethod.Get)
                    {
                        res = await client.GetAsync(subUrl);
                    }
                    else if (method == HttpMethod.Post)
                    {
                        res = await client.PostAsync(subUrl, new StringContent(json, Encoding.UTF8, "application/json"));
                    }
                    else if (method == HttpMethod.Put)
                    {
                        res = await client.PutAsync(subUrl, new StringContent(json, Encoding.UTF8, "application/json"));
                    }
                    else {
                        throw new Exception("Only supports GET, POST, and PUT.");
                    }
                    
                    string resjson = await res.Content.ReadAsStringAsync();
                    if (res.StatusCode == HttpStatusCode.OK || res.StatusCode == HttpStatusCode.Created)
                    {                        
                        return new Response(false, resjson, res.StatusCode.ToString());
                    }                    
                    else
                    {
                        return new Response(true, resjson, res.StatusCode.ToString());
                    }
                }   
                catch (Exception ex)
                {
                    return new Response(true, "{\"Error\":\"" + ex.Message + "\"}", null);
                }
            }
        }
    }

    class Response
    {
        public Response(Boolean isError, string json, string statusDesc)
        {
            IsError = isError;
            Json = json;
            Status = statusDesc;
        }

        private string _json;
        public string Json
        {
            get { return _json; }
            set { _json = value; }
        }

        private Boolean _isError;
        public Boolean IsError
        {
            get { return _isError; }
            set { _isError = value; }
        }

        private string _status;
        public string Status
        {
            get { return _status; }
            set { _status = value; }
        }
    }

    class ErrorResponse
    {
        [JsonProperty("error")]
        public string Error { get; set; }
    }
}
