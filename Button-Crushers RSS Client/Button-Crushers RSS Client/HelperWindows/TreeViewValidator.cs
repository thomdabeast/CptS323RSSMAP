using System.Text;

namespace Button_Crushers_RSS_Client.HelperWindows
{
    static class TreeViewValidator
    {
        /// <summary>
        /// Removes invalid characters from a tree View item
        /// </summary>
        /// <param name="originalText"></param>
        /// <returns>sanatized tree view text</returns>
        public static string ValidateStringForTreeViewItem(string originalText)
        {
            var invalidChars = new[] {"@", ",", "!", " "};

            var output = new StringBuilder(originalText);

            foreach (var r in invalidChars)
            {
                output.Replace(r, string.Empty);
            }

            return output.ToString();
        }
    }
}

