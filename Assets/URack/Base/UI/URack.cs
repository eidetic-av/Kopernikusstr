using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Eidetic.URack.UI
{
    public class URack : DraggableElement
    {
        public static URack Instance { get; private set; }

        public Rack Rack { get; private set; }

        public static void Instantiate(Rack rack) => Instance = new URack(rack);

        public URack(Rack rack) : base()
        {
            Instance = this;
            Rack = rack;

            Instance.Add(ModuleElement.Create(ScriptableObject.CreateInstance<Function.Oscillator4D>()));

            Instance.Add(UI.RackControls.Instance);
        }
    }
}