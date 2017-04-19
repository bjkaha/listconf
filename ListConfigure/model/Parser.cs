using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;

namespace ListConfigure.model
{
    class Parser
    {
        public string[] OriginalCols;
        public string[] Cols;
        public List<Dictionary<string, string>> CsvRows = new List<Dictionary<string, string>>();

        public ParseResult Parse(string filepath, bool isCsv, bool ignoreFirst)
        {
            try
            {
                using (TextFieldParser parser = new TextFieldParser(filepath))
                {
                    string delimiter = isCsv ? "," : "\t";
                    parser.SetDelimiters(new string[] { delimiter });
                    parser.HasFieldsEnclosedInQuotes = true;

                    if (ignoreFirst) parser.ReadLine();

                    OriginalCols = parser.ReadFields();
                    Cols = new string [OriginalCols.Length];
                    for (int i=0; i< Cols.Length; i++)
                    {
                        Cols[i] = OriginalCols[i].ToUpper();
                    }

                    /*
                    for (int i = 0; i < Cols.Length; i++)
                    {
                        Cols[i] = Cols[i].ToUpper();
                    }*/

                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();

                        if (fields.Length != Cols.Length)
                        {
                            return new ParseResult(true, "Each row in csv should have the same number of fields");
                        }

                        Dictionary<string, string> dict = new Dictionary<string, string>();
                        for (int i = 0; i < fields.Length; i++)
                        {
                            dict[Cols[i].ToUpper()] = fields[i];
                        }
                        CsvRows.Add(dict);
                    }
                    if (CsvRows.Count == 0)
                    {
                        return new ParseResult(true, "There are no entries in the csv file");
                    }
                    return new ParseResult(false, null);
                }
            }
            catch (Exception e)
            {
                return new ParseResult(true, e.Message);
            }
        }

        public class ParseResult
        {
            public bool IsError { get; set; }
            public string Msg { get; set; }

            public ParseResult(bool isError, string msg)
            {
                IsError = isError;
                Msg = msg;
            }
        }
    }
}
