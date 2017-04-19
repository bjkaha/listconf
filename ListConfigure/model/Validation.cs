using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListConfigure.model
{
    class Validation
    {       
        public static ValidationResult ValidateFile(ListFile f, bool isCsv, bool ignoreFirstRow, bool enableNewCategory, bool enableReplacing)
        {
            
                if (f.IsIncluded)
                {
                    var list_id = Existing.IsListExist(f.Name);
                    var cat_id = Existing.IsCategoryExist(f.Category);
                    // list doesn't exist
                    if (list_id == null) 
                    {
                        // category also doesn't exist but new category creation disabled.
                        if (cat_id == null && !enableNewCategory)
                            return new ValidationResult("Error", String.Format("List '{0}': Category '{1}' does not exist.", f.Category));
                        // category exist or category creation enabled.
                        else return new ValidationResult("Valid", String.Format("List '{0}': Validated.", f.Name));
                    }
                    // list exist
                    else
                    {
                        // list already exist but not in the specified category
                        if (Existing.IsListInCategory(f.Name, f.Category) == null)
                            return new ValidationResult("Error", String.Format("List '{0}': List is in a different category, not in '{1}'", f.Name, f.Category));
                        // list exist in the specified category
                        else return new ValidationResult("Valid", String.Format("List '{0}': Validated.", f.Name));
                    }
                }
                // skip
                return new ValidationResult("Skip", String.Format("List '{0}': Skipped.", f.Name));
        }

        public static ValidationResult ValidateCols(ListFile f, bool isCsv, bool ignoreFirstRow, bool enableNewCategory, bool enableReplacing)
        {
                var parser = new Parser();
                var res = parser.Parse(f.Path, isCsv, ignoreFirstRow);
                if (res.IsError)
                {
                    return new ValidationResult("Error", String.Format("List '{0}': ", f.Name) + res.Msg);
                }

                var list_id = Existing.IsListExist(f.Name);
                var cat_id = Existing.IsCategoryExist(f.Category);

                if (parser.Cols.FirstOrDefault(c => c.ToUpper() == "Value".ToUpper()) == null)
                {
                    return new ValidationResult("Error", String.Format("List '{0}': No Value column in the file", f.Name));
                }
                // existing list
                if (list_id != null)
                {
                    foreach(var col in parser.Cols)
                    {
                        var col_id = Existing.IsColumnInList(f.Name, col);
                        if (col_id == null && !enableNewCategory)
                            return new ValidationResult("Error", String.Format("List '{0}': Column '{1}' is missing in the list.", f.Name, col));
                    }
                }
                return new ValidationResult("Valid", null);
        }

        public static ValidationResult ValidateRows(ListFile f, bool isCsv, bool ignoreFirstRow, bool enableNewCategory, bool enableReplacing)
        {
            var parser = new Parser();
            var res = parser.Parse(f.Path, isCsv, ignoreFirstRow);
                if (res.IsError)
                {
                    return new ValidationResult("Error", String.Format("List '{0}': ", f.Name) + res.Msg);
                }
                List<string> seen = new List<string>();

                foreach (var row in parser.CsvRows)
                {
                    // all values filled out
                    if (row["VALUE"] == null || row["VALUE"] == "")
                    {
                        return new ValidationResult("Error", String.Format("List '{0}': All row's value column must be filled out.", f.Name));
                    }
                    // no duplicate
                    if (seen.Contains(row["VALUE"]))
                    {
                        return new ValidationResult("Error", String.Format("List '{0}': Duplicate row values in the file.", f.Name));
                    }
                    // no conflict with existing values
                    if (!enableReplacing && Existing.IsValueInList(f.Name, row["VALUE"]) != null)
                    {
                        return new ValidationResult("Error", String.Format("List '{0}': There are already a row with value '{1}' on the server.", f.Name, row["VALUE"]));
                    }
                }
                return new ValidationResult("Valid", null);
        }
       
    }
    public class ValidationResult
    {
        public ValidationResult(string status, string msg)
        {
            Status = status;
            Msg = msg;
        }
        public string Status { get; set; }
        public string Msg { get; set; }
    }
}
