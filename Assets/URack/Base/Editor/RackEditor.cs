using System;
using System.Collections.Generic;
using Eidetic.URack.Base;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;


namespace Eidetic.URack.Editor
{
    public class RackEditor : EditorWindow
    {
        static Rack Rack;

        [MenuItem("Window/URack")]
        public static RackEditor GetWindow() => GetWindow<RackEditor>(true, "URack");

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

        public void OnEnable()
        {
            if (!Rack)
            {
                Rack = Resources.LoadAll<Rack>("").FirstOrDefault(r => r.Open);
                if (!Rack) return;
            }
            Base.UI.URack.Instantiate(Rack);
            var root = GetWindow().rootVisualElement;
            root.Clear();
            root.Add(Base.UI.URack.Instance);
        }

        public void OnDestroy()
        {
            Rack.Open = false;
        }
    }
}