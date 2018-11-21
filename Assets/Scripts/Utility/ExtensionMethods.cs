using System;
using System.Text.RegularExpressions;
using System.Linq;

public static class ExtensionMethods 
{
    public static string ToPascal(this string input)
    {
        var alphaNumeric = Regex.Replace(input, "[^A-Za-z0-9]", string.Empty);
        var output = alphaNumeric.First().ToString().ToUpper() + alphaNumeric.Substring(1);
        return output;
    }
}