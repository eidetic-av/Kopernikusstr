using System;
using System.Collections.Generic;
using System.Linq;
using Eidetic.Utility;
using UnityEditor;
using UnityEngine;

namespace Eidetic.Unity.Utility
{
    public static partial class UnityEngineExtensionMethods
    {
        public static GameObject WithComponent<T>(this GameObject gameObject) where T : Component
        {
            gameObject.AddComponent<T>();
            return gameObject;
        }
        public static GameObject WithName(this GameObject gameObject, string name)
        {
            gameObject.name = name;
            return gameObject;
        }
        public static GameObject InGroup(this GameObject gameObject, string groupName)
        {
            var group = GameObject.Find(groupName);
            if (group == null)
                group = new GameObject().WithName(groupName);
            gameObject.transform.SetParent(group.transform);
            // gameObject.name = name;
            return gameObject;
        }
        public static GameObject WithHideFlags(this GameObject gameObject, params HideFlags[] flags)
        {
            HideFlags applyFlags = HideFlags.None;
            foreach (var flag in flags) applyFlags |= flag;
            gameObject.hideFlags = applyFlags;
            return gameObject;
        }
        public static GameObject InDontDestroyMode(this GameObject gameObject)
        {
            if (Application.isPlaying)
                GameObject.DontDestroyOnLoad(gameObject);
            else
                Threads.RunAtStart(() => GameObject.DontDestroyOnLoad(gameObject));
            return gameObject;
        }
        public static void Destroy(this GameObject gameObject)
        {
            if (gameObject == null) return;
            GameObject.Destroy(gameObject);
        }

        #if UNITY_EDITOR
        /// <summary>
        /// Set the value of a SerializedProperty without knowing the type
        /// </summary>
        public static void SetValue(this SerializedProperty serializedProperty, object value)
        {
            var valueTypeName = value.GetType().CSharpName();
            if (serializedProperty.type != valueTypeName)
                throw new UnityException("You're trying to set a SerializedProperty value of type '" + serializedProperty.type + "' with an object of type ' " + valueTypeName + " '.");
            switch (valueTypeName)
            {
                case "AnimationCurve":
                    serializedProperty.animationCurveValue = (AnimationCurve) value;
                    break;
                case "bool":
                    serializedProperty.boolValue = (bool) value;
                    break;
                case "BoundsInt":
                    serializedProperty.boundsIntValue = (BoundsInt) value;
                    break;
                case "Bounds":
                    serializedProperty.boundsValue = (Bounds) value;
                    break;
                case "Color":
                    serializedProperty.colorValue = (Color) value;
                    break;
                case "double":
                    serializedProperty.doubleValue = (double) value;
                    break;
                case "Object":
                    serializedProperty.exposedReferenceValue = (UnityEngine.Object) value;
                    break;
                case "float":
                    serializedProperty.floatValue = (float) value;
                    break;
                case "int":
                    serializedProperty.intValue = (int) value;
                    break;
            }
        }
#endif

        public static float Map(this float input, Vector2 inputRange, Vector2 outputRange)
        {
            return ((input - inputRange.x) / (inputRange.y - inputRange.x)) * (outputRange.y - outputRange.x) + outputRange.x;
        }

        public static double Map(this double input, Vector2 inputRange, Vector2 outputRange)
        {
            return ((input - inputRange.x) / (inputRange.y - inputRange.x)) * (outputRange.y - outputRange.x) + outputRange.x;
        }

        public static Vector3 RotateBy(this Vector3 input, Vector3 rotation) =>
            Quaternion.Euler(rotation) * input;

        public static Vector3 TranslateBy(this Vector3 input, Vector3 translation) =>
            input + translation;

        public static Vector3 ScaleBy(this Vector3 input, Vector3 scale) =>
            new Vector3(input.x * scale.x, input.y * scale.y, input.z * scale.z);

    }
}