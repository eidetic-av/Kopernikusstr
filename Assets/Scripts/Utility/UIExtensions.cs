
#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using Eidetic.Utility;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Eidetic.Unity.UI.Utility
{
    public static partial class UIExtensionMethods
    {
        public static VisualElement WithClass(this VisualElement element, string className)
        {
            element.AddToClassList(className);
            return element;
        }
        public static VisualElement WithName(this VisualElement element, string name)
        {
            element.name = name;
            return element;
        }
        public static TextElement WithText(this TextElement element, string text)
        {
            element.text = text;
            return element;
        }
    }
}

#endif