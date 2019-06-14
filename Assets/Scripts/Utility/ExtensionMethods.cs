using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Eidetic.Utility
{
    /// <summary>
    /// Utility extension methods for C# classes in System library.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Map a double from one range to another.
        /// </summary>
        /// <param name="input">The input double to map</param>
        /// <param name="minimumInput">Original minimum value</param>
        /// <param name="maximumInput">Original maximum value</param>
        /// <param name="minimumOutput">New minimum value</param>
        /// <param name="maximumOutput">New maximum value</param>
        /// <returns>Double mapped to the new range</returns>
        public static double Map(this double input, double minimumInput, double maximumInput, double minimumOutput, double maximumOutput)
        {
            return ((input - minimumInput) / (maximumInput - minimumInput)) * (maximumOutput - minimumOutput) + minimumOutput;
        }

        /// <summary>
        /// Map a float from one range to another.
        /// </summary>
        /// <param name="input">The input float to map</param>
        /// <param name="minimumInput">Original minimum value</param>
        /// <param name="maximumInput">Original maximum value</param>
        /// <param name="minimumOutput">New minimum value</param>
        /// <param name="maximumOutput">New maximum value</param>
        /// <returns>Float mapped to the new range</returns>
        public static float Map(this float input, float minimumInput, float maximumInput, float minimumOutput, float maximumOutput)
        {
            return ((input - minimumInput) / (maximumInput - minimumInput)) * (maximumOutput - minimumOutput) + minimumOutput;
        }

        /// <summary>
        /// Map an int from one float range to another.
        /// </summary>
        /// <param name="input">The input int to map</param>
        /// <param name="minimumInput">Original minimum value</param>
        /// <param name="maximumInput">Original maximum value</param>
        /// <param name="minimumOutput">New minimum value</param>
        /// <param name="maximumOutput">New maximum value</param>
        /// <returns>Float mapped to the new range</returns>
        public static float Map(this int input, float minimumInput, float maximumInput, float minimumOutput, float maximumOutput)
        {
            return ((((float)input) - minimumInput) / (maximumInput - minimumInput)) * (maximumOutput - minimumOutput) + minimumOutput;
        }

        /// <summary>
        /// Map an int from one range to another.
        /// </summary>
        /// <param name="input">The input int to map</param>
        /// <param name="minimumInput">Original minimum value</param>
        /// <param name="maximumInput">Original maximum value</param>
        /// <param name="minimumOutput">New minimum value</param>
        /// <param name="maximumOutput">New maximum value</param>
        /// <returns>Int mapped to the new range, rounded to nearest.</returns>
        public static int Map(this int input, int minimumInput, int maximumInput, int minimumOutput, int maximumOutput)
        {
            return (int)Math.Round(((input - minimumInput) / (float)(maximumInput - minimumInput)) * (maximumOutput - minimumOutput) + minimumOutput);
        }

        /// <summary>
        /// Clamps a value inside an arbitrary range.
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <param name="value">The input value to clamp</param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns>Value clamped to the specified range</returns>
        public static T Clamp<T>(this T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0) return min;
            else if (value.CompareTo(max) > 0) return max;
            else return value;
        }

        /// <summary>
        /// Clamps a value inside an arbitrary range.
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <param name="value">The input value to clamp</param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns>Value clamped to the specified range</returns>
        public static float Clamp(this float value, float min, float max)
        {
            if (value.CompareTo(min) < 0) return min;
            else if (value.CompareTo(max) > 0) return max;
            else return value;
        }

        /// <summary>
        /// Rounds a float to an int
        /// </summary>
        /// <param name="value">The input value to round</param>
        /// <returns>Value clamped to the specified range</returns>
        public static int RoundToInt(this float value) => (int)Math.Round(value);

        public static float Pow(this float baseValue, float exponent) => (float)System.Math.Pow(baseValue, exponent);

        /// <summary>
        /// Create a duplicate of a string.
        /// </summary>
        /// <param name="inputString">The input string to copy</param>
        /// <returns>Duplicate of input string</returns>
        public static string Copy(this string inputString)
        {
            return new string(inputString.ToCharArray());
        }

        /// <summary>
        /// Convert the string to PascalCase.
        /// </summary>
        /// <param name="inputString">The input string to convert</param>
        /// <returns>PascalCasedString</returns>
        public static string ToPascalCase(this string input)
        {
            var alphaNumeric = Regex.Replace(input, "[^A-Za-z0-9]", string.Empty);
            var output = alphaNumeric.First().ToString().ToUpper() + alphaNumeric.Substring(1);
            return output;
        }

        /// <summary>
        /// Convert the string to camelCase.
        /// </summary>
        /// <param name="inputString">The input string to convert</param>
        /// <returns>camelCasedString</returns>
        public static string ToCamelCase(this string input)
        {
            var alphaNumeric = Regex.Replace(input, "[^A-Za-z0-9]", string.Empty);
            var output = alphaNumeric.First().ToString().ToLower() + alphaNumeric.Substring(1);
            return output;
        }

        public static bool IsNullOrEmpty(this string input)
        {
            if (input == null) return true;
            else if (input.Trim().Length == 0) return true;
            else return false;
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        /// <summary>
        /// Cast the collection to a List<T> of Type T.
        /// </summary>
        public static List<T> ToList<T>(this IEnumerable<object> input) => input.Cast<T>().ToList();

        public static List<T> With<T>(this List<T> list, T item)
        {
            list.Add(item);
            return list;
        }

        /// <summary>
        /// Check if a member has an attribute of type T attached.
        /// </summary>
        /// <param name="member">MemberInfo to check</param>
        /// <typeparam name="T">Attribute type</typeparam>
        /// <returns>true if attribute exists on type</returns>
        public static bool ContainsAttribute<T>(this MemberInfo member) where T : Attribute
        {
            return member.GetCustomAttributes()
                .Where(attribute => attribute.GetType().Equals(typeof(T)) || attribute.GetType().IsSubclassOf(typeof(T)))
                .Any();
        }

        /// <summary>
        /// Filter a collection of MemberInfo's based on an Attribute Type.
        /// </summary>
        /// <param name="memberInfoCollection">The collection to filter</param>
        /// <typeparam name="T">Attribute Type</typeparam>
        /// <returns>Filtered collection</returns>
        public static IEnumerable<MemberInfo> WithAttribute<T>(this IEnumerable<MemberInfo> memberInfoCollection) where T : Attribute
        {
            return memberInfoCollection.Where(member => member.ContainsAttribute<T>());
        }

        /// <summary>
        /// Get a camelCased backing field for a PascalCased property
        /// </summary>
        /// <param name="property">Property with backing field</param>
        /// <returns>FieldInfo of backing field or null if it doesn't exist</returns>
        public static FieldInfo GetBackingField(this Type type, PropertyInfo property) =>
            type.GetField(property.Name.ToCamelCase(), BindingFlags.NonPublic | BindingFlags.Instance);

        public static void ForEach<K, T>(this Dictionary<K, T> dictionary, Action<K, T> action)
        {
            foreach (var keyValuePair in dictionary)
                action.Invoke(keyValuePair.Key, keyValuePair.Value);
        }

        public static void Add<T>(this List<T> list, T value, bool addIfNull)
        {
            if (value == null && !addIfNull) return;
            list.Add(value);
        }

        public static void Add<K, V>(this Dictionary<K, V> dictionary, K key, V value, bool addIfNull)
        {
            if (value == null && !addIfNull) return;
            dictionary.Add(key, value);
        }

        public static Dictionary<K, T> AsDictionary<K, T>(this IEnumerable<T> collection, Func<T, K> keySelector)
        {
            var dictionary = new Dictionary<K, T>();
            foreach (var value in collection.Cast<T>())
            {
                dictionary.Add(keySelector.Invoke(value), value);
            }
            return dictionary;
        }

        public static Dictionary<K, T> SelectPair<K, T>(this Dictionary<K, T> inputDictionary, Predicate<T> selector)
        {
            var outputDictionary = new Dictionary<K, T>();
            foreach (var key in inputDictionary.Keys)
            {
                var item = inputDictionary[key];
                if (selector.Invoke(item)) outputDictionary.Add(key, item);
            }
            return outputDictionary;
        }

        /// <summary> Return a prettiefied type name. </summary>
        // Originally from xNode by Siccity
        public static string PrettyName(this Type type)
        {
            if (type == null) return "null";
            if (type == typeof(System.Object)) return "object";
            if (type == typeof(float)) return "float";
            else if (type == typeof(int)) return "int";
            else if (type == typeof(long)) return "long";
            else if (type == typeof(double)) return "double";
            else if (type == typeof(string)) return "string";
            else if (type == typeof(bool)) return "bool";
            else if (type.IsGenericType)
            {
                string s = "";
                Type genericType = type.GetGenericTypeDefinition();
                if (genericType == typeof(List<>)) s = "List";
                else s = type.GetGenericTypeDefinition().ToString();

                Type[] types = type.GetGenericArguments();
                string[] stypes = new string[types.Length];
                for (int i = 0; i < types.Length; i++)
                {
                    stypes[i] = types[i].PrettyName();
                }
                return s + "<" + string.Join(", ", stypes) + ">";
            }
            else if (type.IsArray)
            {
                string rank = "";
                for (int i = 1; i < type.GetArrayRank(); i++)
                {
                    rank += ",";
                }
                Type elementType = type.GetElementType();
                if (!elementType.IsArray) return elementType.PrettyName() + "[" + rank + "]";
                else
                {
                    string s = elementType.PrettyName();
                    int i = s.IndexOf('[');
                    return s.Substring(0, i) + "[" + rank + "]" + s.Substring(i);
                }
            }
            else return type.Name;
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