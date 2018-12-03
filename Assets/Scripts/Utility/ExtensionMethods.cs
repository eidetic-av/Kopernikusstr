using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;

namespace Utility
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
        /// Map a float from one range to another.
        /// </summary>
        /// <param name="input">The input float to map</param>
        /// <param name="inputRange">Original range</param>
        /// <param name="outputRange">New range</param>
        /// <returns>Float mapped to the new range.</returns>
        public static float Map(this float input, Vector2 inputRange, Vector2 outputRange)
        {
            return ((input - inputRange.x) / (inputRange.y - inputRange.x)) * (outputRange.y - outputRange.x) + outputRange.x;
        }

        /// <summary>
        /// Map a double from one range to another.
        /// </summary>
        /// <param name="input">The input double to map</param>
        /// <param name="inputRange">Original range</param>
        /// <param name="outputRange">New range</param>
        /// <returns>Double mapped to the new range.</returns>
        public static double Map(this double input, Vector2 inputRange, Vector2 outputRange)
        {
            return ((input - inputRange.x) / (inputRange.y - inputRange.x)) * (outputRange.y - outputRange.x) + outputRange.x;
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
        public static string ToPascal(this string input)
        {
            var alphaNumeric = Regex.Replace(input, "[^A-Za-z0-9]", string.Empty);
            var output = alphaNumeric.First().ToString().ToUpper() + alphaNumeric.Substring(1);
            return output;
        }
    }
}