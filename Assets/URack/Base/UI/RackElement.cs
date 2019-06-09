using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Eidetic.URack.UI
{
    public class RackElement : DraggableElement
    {
        public static RackElement Instance { get; private set; }

        public Rack Rack { get; private set; }

        public static void Instantiate(Rack rack) => Instance = new RackElement(rack);

        public RackElement(Rack rack) : base()
        {
            Instance = this;
            Rack = rack;

            Instance.Add(ModuleElement.Create(ScriptableObject.CreateInstance<Function.Oscillator4D>()));
        }
    }
}