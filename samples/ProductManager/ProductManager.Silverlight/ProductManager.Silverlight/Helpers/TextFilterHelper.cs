using System.Collections.Generic;
using System.Linq;
using C1.Silverlight.DataGrid;

namespace ProductManager.Silverlight
{
    public class TextFilterHelper
    {
        // *******************************************************************
        // Column's type specific filter logic
        // *******************************************************************
        public static DataGridFilterState BuildFilterState(C1.Silverlight.DataGrid.DataGridColumn column, string text)
        {
            var fs = new DataGridFilterState();
            fs.FilterInfo = new List<FilterInfo>();
            fs.Tag = text;

            // check if there is text to filter
            if (text == string.Empty)
                return null;

            if (column is DataGridComboBoxColumn)
            {
                // We need special handling for combo columns 
                // because diplayed text is different than the value in the datasource (id)
                // We need to define the "Contains" operator taking ids into account
                fs.FilterInfo.Add(new FilterInfo()
                {
                    Value = GetIdsContainingText(text, column as DataGridComboBoxColumn),
                    FilterOperation = FilterOperation.IsOneOf,
                });
            }
            else
            {
                // Contains for text columns
                fs.FilterInfo.Add(new FilterInfo()
                {
                    FilterType = FilterType.Text,
                    Value = text,
                    FilterOperation = FilterOperation.Contains,
                });
            }
            return fs;
        }

        public static string GetTextFromState(C1.Silverlight.DataGrid.DataGridColumn column)
        {
            string text = "";
            if (column.FilterState != null
                && column.FilterState.FilterInfo != null
                && column.FilterState.FilterInfo.Count > 0)
            {
                if (column is DataGridComboBoxColumn)
                {
                    if (column.FilterState.Tag is string)
                    {
                        text = column.FilterState.Tag.ToString();
                    }
                }
                else
                {
                    if (column.FilterState.FilterInfo[0].FilterOperation == FilterOperation.Contains)
                    {
                        text = column.FilterState.FilterInfo[0].Value.ToString();
                    }

                }
            }
            return text;
        }
        /// <summary>
        /// Retrieves all the ids whose display label contains the desired text.
        /// (Uses the same properties used by the combo colum itself)
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        private static List<object> GetIdsContainingText(string text, DataGridComboBoxColumn column)
        {
            // combo items source
            var qItemsSource = column.ItemsSource.Cast<object>();

            if (qItemsSource.Count() > 0)
            {
                // type of combo items
                var itemType = qItemsSource.First().GetType();

                // get the property used as "text display" and "id" of the item
                var displayProp = itemType.GetProperty(column.DisplayMemberPath);
                var idProp = itemType.GetProperty(column.SelectedValuePath);

                // do the query and return it
                var ids = from item in qItemsSource.Cast<object>()
                          where displayProp.GetValue(item, null).ToString().ToLower().Contains(text.ToLower())
                          select idProp.GetValue(item, null);
                return ids.ToList();
            }
            else
            {
                // return empty list
                return new List<object>();
            }
        }
    }
}
