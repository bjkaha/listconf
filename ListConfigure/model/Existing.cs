using ListConfigure.http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ListConfigure.model
{
    class Existing
    {
        public static List<MxList> Lists;
        public static List<MxCategory> Categories;

        public static async Task<Boolean> Init()
        {
            var list_res = await HttpHandler.Request(HttpMethod.Get, "/lists/", null);
            var cat_res = await HttpHandler.Request(HttpMethod.Get, "/list-categories/", null);

            if (!list_res.IsError && !cat_res.IsError)
            {
                ListResponse listres = JsonConvert.DeserializeObject<ListResponse>(list_res.Json);
                CategoryResponse catres = JsonConvert.DeserializeObject<CategoryResponse>(cat_res.Json);
                Lists = listres.Lists;
                Categories = catres.ListCategory;
                return true;
            }
            else
            {
                return false;
            }
        }

        // getting list
        public static MxList GetList(string name)
        {
            try
            {
                var lst = Lists.FirstOrDefault(l => l.Name.ToUpper() == name.ToUpper());
                return lst;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                return null;
            }
        }

        // getting category
        public static MxCategory GetCategory(string name)
        {
            var cat = Categories.FirstOrDefault(c => c.Name.ToUpper() == name.ToUpper());
            return cat;
        }

        // returns list_id
        public static string IsListExist(string listname)
        {
            var lst = GetList(listname);
            if (lst != null)
            {
                return lst.ID;
            }
            else return null;
        }

        // returns category_id
        public static string IsCategoryExist(string catname)
        {
            var cat = GetCategory(catname);
            if (cat != null)
            {
                return cat.ID;
            }
            else return null;
        }

        // return list_id
        public static string IsListInCategory(string listname, string catname)
        {
            var lst = GetList(listname);
            var cat = GetCategory(catname);
            if (lst != null && cat != null)
            {
                if (lst.Category == cat.ID)
                {
                    return lst.ID;
                }
            }
            return null;
        }

        // returns column_id
        public static string IsColumnInList(string listname, string colname)
        {
            var lst = GetList(listname);
            foreach (KeyValuePair<string, dynamic> entry in lst.Columns)
            {
                if (((string)entry.Value.name).ToUpper() == colname.ToUpper())
                {
                    return entry.Value._id;
                }
            }
            return null;
        }

        // returns row_id
        public static string IsValueInList(string listname, string value)
        {
            var lst = GetList(listname);
            foreach (KeyValuePair<string, dynamic> entry in lst.Rows)
            {
                if (((string)(entry.Value.values[lst.PrimaryColumn])).ToUpper() == value.ToUpper())
                {
                    return entry.Value._id;
                }
            }
            return null;
        }
    }

    class CategoryResponse
    {
        [JsonProperty("ListCategory")]
        public List<MxCategory> ListCategory;
    }

    class MxCategory
    {
        [JsonProperty("_id")]
        public string ID;

        [JsonProperty("name")]
        public string Name;
    }

    class ListResponse
    {
        [JsonProperty("List")]
        public List<MxList> Lists;
    }

    class MxList
    {
        [JsonProperty("_id")]    
        public string ID;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("category")]
        public string Category;

        [JsonProperty("rows")]
         public Dictionary<string, dynamic> Rows;

        [JsonProperty("columns")]
        public Dictionary<string, dynamic> Columns;

        [JsonProperty("inuse_column")]
        public string InuseColumn;

        [JsonProperty("primary_column")]
        public string PrimaryColumn;
    }
}
