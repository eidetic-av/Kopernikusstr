using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eidetic.Unity.UI.Utility;
using Eidetic.Utility;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Eidetic.URack.Base
{
    public static class ExtensionMethods
    {
        public static string ToPascalCase(this string input)
        {
            var alphaNumeric = System.Text.RegularExpressions.Regex.Replace(input, "[^A-Za-z0-9]", string.Empty);
            var output = alphaNumeric.First().ToString().ToUpper() + alphaNumeric.Substring(1);
            return output;
        }
    }
}
