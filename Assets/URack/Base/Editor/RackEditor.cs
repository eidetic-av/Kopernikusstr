using System;
using System.Collections.Generic;
using Eidetic.URack.UI;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;


namespace Eidetic.URack.Editor
{
    [CustomEditor(typeof(Rack))]
    public class RackEditor : EditorWindow
    {
        static Rack Rack;

        [UnityEditor.Callbacks.OnOpenAsset(0)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            Rack = Selection.activeObject as Rack;
            if (Rack != null)
            {
                Rack.Open = true;
                GetWindow();
                return true;
            }
            return false;
        }

        [MenuItem("Window/URack")]
        static RackEditor GetWindow() => GetWindow<RackEditor>(true, "URack");

        public void OnEnable()
        {
            Debug.Log("Enable");
            if (!Rack)
            {
                Rack = Resources.LoadAll<Rack>("").FirstOrDefault(r => r.Open);
                if (!Rack) return;
            }
            UI.URack.Instantiate(Rack);
            var root = GetWindow().rootVisualElement;
            root.Clear();
            root.Add(UI.URack.Instance);
        }

        public void OnDestroy()
        {
            Rack.Open = false;
        }
    }
}