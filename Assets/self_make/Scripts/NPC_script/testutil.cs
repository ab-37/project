using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace teast
{
    public static class testutil
    {
        public static Dictionary<string, List<string>> ReadGutenbergFile(string text)
        {
            Dictionary<string, List<string>> messages = new Dictionary<string, List<string>>();
            Regex nameRegex = new Regex("^[A-Z]+$");

            string currentCharacter = null;

            foreach (string line in text.Split('\n'))
            {
                string trimmed = line.Trim();
                if (nameRegex.IsMatch(trimmed))
                {
                    currentCharacter = trimmed;
                    if (!messages.ContainsKey(currentCharacter))
                        messages[currentCharacter] = new List<string>();
                }
                else if (!string.IsNullOrWhiteSpace(trimmed) && currentCharacter != null)
                {
                    messages[currentCharacter].Add(trimmed);
                }
            }
            return messages;
        }
    }
}
