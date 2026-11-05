using System.Text.RegularExpressions;

/// <summary>
/// String helpers for converting territory names between compact and
/// space-separated representations.
/// </summary>
public static class StringExtensions {
    /*
        Territory names use two representations:
            AmericaDelSud <-> America Del Sud
    */

    
    /// <summary>
    /// Inserts a space before each internal uppercase letter.
    /// </summary>
    /// <param name="text">Compact territory name.</param>
    /// <returns>Space-separated territory name.</returns>
	public static string AddSpaces(this string text) {
        // The first parameter uses the ``this`` modifier to declare an
        // extension method on string.
        return Regex.Replace(text, "(?<!^)([A-Z])", " $1");

        /*
            Regex.Replace arguments:
            - input string;
            - pattern: (?<!^)([A-Z]) captures uppercase letters while the
              negative lookbehind (?<!^) prevents a leading space;
            - replacement: " $1" inserts a space before each captured letter.
        */

        // Strings are immutable, so the transformed value is returned.
	}

    /// <summary>
    /// Removes spaces from a territory name.
    /// </summary>
    /// <param name="text">Space-separated territory name.</param>
    /// <returns>Compact territory name.</returns>
    public static string RemoveSpaces(this string text) {
        string[] words = text.Split(' ');  // Space separator
        string newText = string.Join("", words);
        return newText;
    }
}