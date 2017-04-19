using ListConfigure.http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ListConfigure.model
{
    class Configuration : INotifyPropertyChanged
    {
        private string _selectedDirectory;
        public string SelectedDirectory
        {
            get { return _selectedDirectory; }
            set { Set(ref _selectedDirectory, value); }
        }

        private ObservableCollection<ListFile> _listfiles;
        public ObservableCollection<ListFile> ListFiles
        {
            get { return _listfiles; }
            set { Set(ref _listfiles, value); }
        }

        public ConsoleInput SetSource()
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.ShowNewFolderButton = true;
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    SelectedDirectory = dialog.SelectedPath;
                    return GetFiles();
                }
                else return new ConsoleInput(false, "Setting directory cancelled.");
            }
        }

        public ConsoleInput GetFiles()
        {
            if (SelectedDirectory != null || SelectedDirectory != "")
            {
                string[] paths;
                try
                {
                    paths = Directory.GetFiles(SelectedDirectory, "*", SearchOption.AllDirectories);
                }
                catch (Exception e)
                {
                    return new ConsoleInput(true, e.Message);
                }
                if (paths.Length > 100)
                {
                    string dir = SelectedDirectory;
                    SelectedDirectory = null;
                    return new ConsoleInput(true, "Too many files in the directory "+ dir +". Please select a different directory");
                }
                if (ListFiles == null)
                {
                    ListFiles = new ObservableCollection<ListFile>();
                }
                else ListFiles.Clear();
                foreach (string path in paths)
                {
                    if (path.EndsWith(".csv") || path.EndsWith(".txt"))
                    {
                        int len = path.Split('\\').Length;
                        if (len > 2)
                        {
                            string file = path.Split('\\')[len - 1];
                            string cat = path.Split('\\')[len - 2];
                            string lst;
                            if (file.EndsWith(".csv"))
                            {
                                lst = file.Split(new string[] { ".csv" }, StringSplitOptions.None)[0];
                            }
                            else
                            {
                                lst = file.Split(new string[] { ".txt" }, StringSplitOptions.None)[0];
                            }
                            ListFiles.Add(new ListFile(lst, cat, path, "Ready", null, true));
                        }
                    }
                }
                if (ListFiles != null && ListFiles.Count == 0)
                {
                    ListFiles = null;
                    return new ConsoleInput(false, "Selected directory has no .csv or .txt files");
                }
                return new ConsoleInput(false, "Source directory is set to " + SelectedDirectory);
            }
            return null;
        }

        public async Task<ConsoleInput> Init()
        {
            bool succ = await Existing.Init();
            if (succ)
            {
                return new ConsoleInput(false, "Succeed to retrieve list data from the server.");
            }
            else
            {
                return new ConsoleInput(true, "Failed to retrieve list data from the server.");
            }
        }

        //
        public async Task<List<ConsoleInput>> Configure(ListFile f, bool isCsv, bool ignoreFirst)
        {
            List<ConsoleInput> output = new List<ConsoleInput>();
            if (f.IsIncluded)
            {                
                Parser parser = new Parser();
                
                var parseresult = parser.Parse(f.Path, isCsv, ignoreFirst);
                //output.Add(new ConsoleInput(false, "COLS: " + String.Join(",", parser.OriginalCols.ToArray())));
                if (parseresult.IsError)
                {
                    output.Add(new ConsoleInput(true, String.Format("Parse error, {0}", parseresult.Msg)));
                    return output;
                }
                var list_id = Existing.IsListExist(f.Name);
                var cat_id = Existing.IsCategoryExist(f.Category);
                // for new lists
                if (list_id == null)
                {
                    Response res;
                    dynamic response;
                    // new category
                    if (cat_id == null)
                    {
                        res = await HttpHandler.Request(HttpMethod.Post, "/list-categories/", String.Format("{{ \"name\" : \"{0}\"}}", f.Category));
                        response = JsonConvert.DeserializeObject<dynamic>(res.Json);
                        if (res.IsError)
                        {
                            if (res.Status != null) output.Add(new ConsoleInput(true, String.Format("List '{0}': {1}, Could not create category '{2}'", f.Name, res.Status, f.Category)));
                            else output.Add(new ConsoleInput(true, String.Format("List '{0}': {1}, Could not create category '{2}'", f.Name, response.Error, f.Category)));
                            return output;
                        }
                        else
                        {
                            cat_id = response._id;
                            output.Add(new ConsoleInput(false, String.Format("List '{0}': {1}, Created category '{2}' with _id '{3}'", f.Name, res.Status, f.Category, cat_id)));
                        }
                    }
                    // new list
                    res = await HttpHandler.Request(HttpMethod.Post, "/lists/", String.Format("{{ \"name\" : \"{0}\" , \"category\" : \"{1}\" }}", f.Name, cat_id));
                    response = JsonConvert.DeserializeObject<dynamic>(res.Json);
                    if (res.IsError)
                    {
                        if (res.Status != null) output.Add(new ConsoleInput(true, String.Format("List '{0}': {1}, Could not create the list", f.Name, res.Status)));
                        else output.Add(new ConsoleInput(true, String.Format("List '{0}': {1}, Could not create the list", f.Name, response.Error)));
                        return output;
                    }
                    else
                    {
                        list_id = response._id;
                        output.Add(new ConsoleInput(false, String.Format("List '{0}': {1}, Created list with _id '{2}'", f.Name, res.Status, list_id)));
                    }
                    // new cols
                    Dictionary<string, string> colname_to_id = new Dictionary<string, string>();
                    res = await HttpHandler.Request(HttpMethod.Get, String.Format("/lists/{0}/columns", list_id), null);
                    if (res.IsError)
                    {
                        string msg = (res.Status != null) ? res.Status : "Connection Error";
                        output.Add(new ConsoleInput(true, String.Format("List '{0}': {1}, Connection error while getting column list", f.Name, msg)));
                        return output;
                    }
                    else
                    {
                        Dictionary<string, dynamic> coldict = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(res.Json);
                        foreach (KeyValuePair<string, dynamic> entry in coldict)
                        {
                            var colid = entry.Value._id;
                            var colname = entry.Value.name;
                            colname_to_id.Add(((string)colname).ToUpper(), (string)colid);
                        }
                    }
                    foreach (string c in parser.OriginalCols)
                    {
                        if (c.ToUpper() != "VALUE" && c.ToUpper() != "DESCRIPTION" && c.ToUpper() != "IN USE")
                        {
                            res = await HttpHandler.Request(HttpMethod.Post, String.Format("lists/{0}/columns", list_id), String.Format("{{ \"name\": \"{0}\", \"type\":\"text\", \"length\":\"255\" }}", c));
                            if (res.IsError)
                            {
                                string msg = (res.Status != null) ? res.Status : "Connection Error";
                                output.Add(new ConsoleInput(true, String.Format("List '{0}': {1}, Connection error while creating new column '{2}'", f.Name, msg, c)));
                                return output;
                            }
                            response = JsonConvert.DeserializeObject<dynamic>(res.Json);
                            colname_to_id.Add(c.ToUpper(), (string)(response._id));
                            output.Add(new ConsoleInput(false, String.Format("List '{0}': {1}, Created column '{2}', list id '{3}'", f.Name, res.Status, c, list_id)));
                        }
                    }
                    // first row
                    res = await HttpHandler.Request(HttpMethod.Get, String.Format("/lists/{0}/rows", list_id), null);
                    if (res.IsError)
                    {
                        string msg = (res.Status != null) ? res.Status : "Connection Error";
                        output.Add(new ConsoleInput(true, String.Format("List '{0}': {1}, error while getting rows", f.Name, msg)));
                        return output;
                    }
                    Dictionary<string, dynamic> rowdict = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(res.Json);
                    string firstrowid = null;
                    foreach (KeyValuePair<string, dynamic> entry in rowdict)
                    {
                        firstrowid = entry.Value._id;
                    }
                    if (firstrowid == null)
                    {
                        output.Add(new ConsoleInput(true, String.Format("List '{0}': Could not find the first row", f.Name)));
                        return output;
                    }
                    // new rows
                    for (int i = 0; i < parser.CsvRows.Count; i++)
                    {
                        var val_entry = "{ \"values\": {";
                        bool isInUseSeen = false;
                        List<string> val_sub_entries = new List<string>();
                        foreach (string c in parser.Cols)
                        {
                            string c_id = colname_to_id[c];
                            string c_val = parser.CsvRows[i][c];
                            if (c == "IN USE")
                            {
                                if (c_val.ToUpper() == "TRUE" || c_val.ToUpper() == "FALSE")
                                {
                                    isInUseSeen = true;
                                    val_sub_entries.Add(String.Format("\"{0}\":{1}", c_id, c_val.ToLower()));
                                }
                            }
                            else val_sub_entries.Add(String.Format("\"{0}\":\"{1}\"", c_id, c_val));
                        }
                        if (!isInUseSeen)
                        {
                            val_sub_entries.Add(String.Format("\"{0}\": true", colname_to_id["IN USE"]));
                        }
                        val_entry += String.Join(",", val_sub_entries.ToArray());
                        val_entry += "}}";
                        if (i == 0)
                        {
                            res = await HttpHandler.Request(HttpMethod.Put, String.Format("/lists/{0}/rows/{1}", list_id, firstrowid), val_entry);
                            if (res.IsError)
                            {
                                output.Add(new ConsoleInput(true, String.Format("List '{0}': Could not create rows", f.Name)));
                                return output;
                            }
                        }
                        else
                        {
                            res = await HttpHandler.Request(HttpMethod.Post, String.Format("/lists/{0}/rows", list_id), val_entry);
                            if (res.IsError)
                            {
                                output.Add(new ConsoleInput(true, String.Format("List '{0}': Could not create rows", f.Name)));
                                return output;
                            }
                        }
                        output.Add(new ConsoleInput(false, String.Format("List '{0}': {1} a row added to the list", f.Name, res.Status)));
                    }
                    // publish
                    res = await HttpHandler.Request(HttpMethod.Put, String.Format("/lists/{0}", list_id), "{\"published\":true}");
                    if (res.IsError)
                    {
                        output.Add(new ConsoleInput(true, String.Format("List '{0}': {1}, Could not publish the list", f.Name, res.Status)));
                        return output;
                    }
                    output.Add(new ConsoleInput(false, String.Format("List '{0}': {1}, list is published", f.Name, res.Status)));
                }
                // for updating lists
                else
                {
                    Response res;
                    dynamic response;
                    // create colname to id dict:
                    Dictionary<string, string> colname_to_id = new Dictionary<string, string>();
                    res = await HttpHandler.Request(HttpMethod.Get, String.Format("/lists/{0}/columns", list_id), null);
                    if (res.IsError)
                    {
                        string msg = (res.Status != null) ? res.Status : "Connection Error";
                        output.Add(new ConsoleInput(true, String.Format("List '{0}': {1}, Connection error while getting column list", f.Name, msg)));
                        return output;
                    }
                    else
                    {
                        Dictionary<string, dynamic> coldict = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(res.Json);
                        foreach (KeyValuePair<string, dynamic> entry in coldict)
                        {
                            var colid = entry.Value._id;
                            var colname = entry.Value.name;
                            colname_to_id.Add(((string)colname).ToUpper(), (string)colid);
                        }
                    }
                    foreach (var c in parser.OriginalCols)
                    {
                        if (Existing.IsColumnInList(f.Name, c.ToUpper()) == null)
                        {
                            res = await HttpHandler.Request(HttpMethod.Post, String.Format("lists/{0}/columns", list_id), String.Format("{{ \"name\": \"{0}\", \"type\":\"text\", \"length\":\"255\" }}", c));
                            if (res.IsError)
                            {
                                string msg = (res.Status != null) ? res.Status : "Connection Error";
                                output.Add(new ConsoleInput(true, String.Format("List '{0}': {1}, Connection error while creating new column '{2}'", f.Name, msg, c)));
                                return output;
                            }
                            response = JsonConvert.DeserializeObject<dynamic>(res.Json);
                            colname_to_id.Add(c.ToUpper(), (string)(response._id));
                            output.Add(new ConsoleInput(false, String.Format("List '{0}': {1}, Created column '{2}', list id '{3}'", f.Name, res.Status, c, list_id)));
                        }
                    }
                    for (int i = 0; i < parser.CsvRows.Count; i++)
                    {
                        var val_entry = "{ \"values\": {";
                        bool isInUseSeen = false;
                        List<string> val_sub_entries = new List<string>();
                        foreach (string c in parser.Cols)
                        {
                            var c_id = colname_to_id[c];
                            var c_val = parser.CsvRows[i][c];
                            if (c == "IN USE")
                            {
                                if (c_val.ToUpper() == "TRUE" || c_val.ToUpper() == "FALSE")
                                {
                                    isInUseSeen = true;
                                    val_sub_entries.Add(String.Format("\"{0}\":{1}", c_id, c_val.ToLower()));
                                }
                            }
                            else val_sub_entries.Add(String.Format("\"{0}\":\"{1}\"", c_id, c_val));
                        }

                        if (!isInUseSeen)
                        {
                            val_sub_entries.Add(String.Format("\"{0}\": true", colname_to_id["IN USE"]));
                        }
                        val_entry += String.Join(",", val_sub_entries.ToArray());
                        val_entry += "}}";
                        string val = parser.CsvRows[i]["VALUE"];
                        string row_id = Existing.IsValueInList(f.Name, val);

                        if (row_id == null)
                        {
                            res = await HttpHandler.Request(HttpMethod.Post, String.Format("/lists/{0}/rows", list_id), val_entry);
                            if (res.IsError)
                            {
                                output.Add(new ConsoleInput(true, String.Format("List '{0}': Could not create rows", f.Name)));
                                return output;
                            }
                        }
                        else
                        {
                            res = await HttpHandler.Request(HttpMethod.Put, String.Format("/lists/{0}/rows/{1}", list_id, row_id), val_entry);
                            if (res.IsError)
                            {
                                output.Add(new ConsoleInput(true, String.Format("List '{0}': Could not update rows", f.Name)));
                                return output;
                            }
                        }
                        output.Add(new ConsoleInput(false, String.Format("List '{0}': {1} a row added to the list", f.Name, res.Status)));

                    }
                    output.Add(new ConsoleInput(false, String.Format("List '{0}': updated the list.", f.Name)));

                }
                
            }
            return output;

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

    class ListFile : INotifyPropertyChanged
    {
        public ListFile(string name, string category, string path, string status, string error, Boolean included)
        {
            Name = name;
            Category = category;
            Path = path;
            Status = status;
            Error = error;
            IsIncluded = included;
        }
        private string _name;
        public string Name {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        private string _category;
        public string Category {
            get { return _category; }
            set { Set(ref _category, value); }
        }

        private string _path;
        public string Path {
            get { return _path; }
            set { Set(ref _path, value); }
        }

        private string _status;
        public string Status {
            get { return _status; }
            set { Set(ref _status, value); }
        }

        private string _error;
        public string Error {
            get { return _error; }
            set { Set(ref _error, value); }
        }

        private Boolean _isIncluded;
        public Boolean IsIncluded {
            get { return _isIncluded; }
            set { Set(ref _isIncluded, value); }
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
