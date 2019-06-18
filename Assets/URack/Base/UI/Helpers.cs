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
        // Todo: there must be a way to make these following "WithClass" methods generic

        // Add a class to a visual element and return the visual element
        public static VisualElement WithClass(this VisualElement element, string className)
        {
            element.AddToClassList(className);
            return element;
        }
        // Add a class to a label and return the label
        public static Label WithClass(this Label label, string className)
        {
            label.AddToClassList(className);
            return label;
        }
        // Add a class to a box and return the label
        public static Box WithClass(this Box box, string className)
        {
            box.AddToClassList(className);
            return box;
        }

        public static string ToPascalCase(this string input)
        {
            var alphaNumeric = System.Text.RegularExpressions.Regex.Replace(input, "[^A-Za-z0-9]", string.Empty);
            var output = alphaNumeric.First().ToString().ToUpper() + alphaNumeric.Substring(1);
            return output;
        }

        // Takes a PascalCase string (like a Type Name)
        // and adds spaces, etc.
        public static string Prettify(this string pascalCaseInput)
        {
            var output = "";
            for (int i = 0; i < pascalCaseInput.Length; i++)
            {
                var c = pascalCaseInput[i];
                if (i == 0)
                    output += char.ToUpper(c);
                else
                {
                    var previousCharacter = output[output.Length - 1];
                    if (char.IsLetter(previousCharacter))
                    {
                        if (char.IsDigit(c)) output += " " + c;
                        else if (char.IsUpper(previousCharacter)) output += c;
                        else if (char.IsUpper(c)) output += " " + c;
                        else output += c;
                    }
                    else if (char.IsDigit(previousCharacter))
                        output += c;
                }
            }
            return output;
        }


        // Return the C# name of a Type, as opposed to the .Net name that is
        // returned with Type.Name
        public static string CSharpName(this Type type)
        {
            switch (type.Name)
            {
                case "Object":
                    return "object";
                case "String":
                    return "string";
                case "Boolean":
                    return "bool";
                case "Byte":
                    return "byte";
                case "Char":
                    return "char";
                case "Decimal":
                    return "decimal";
                case "Double":
                    return "double";
                case "Int16":
                    return "short";
                case "Int32":
                    return "int";
                case "Int64":
                    return "long";
                case "SByte":
                    return "sbyte";
                case "Single":
                    return "float";
                case "UInt16":
                    return "ushort";
                case "UInt32":
                    return "uint";
                case "UInt64":
                    return "ulong";
                case "Void":
                    return "void";
                default:
                    return type.Name;
            }
        }
    }
}
