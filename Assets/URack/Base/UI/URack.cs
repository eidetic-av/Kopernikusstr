using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Eidetic.URack.Base.UI
{
    public class URack : DraggableElement
    {
        public static URack Instance { get; private set; }

        public Rack Rack { get; private set; }

        public static void Instantiate(Rack rack) => Instance = new URack(rack);

        CableLayer CableLayer;

        public URack(Rack rack) : base()
        {
            Instance = this;
            Rack = rack;

            Add(UI.RackControls.Instance);

            Add(CableLayer = new CableLayer());

            foreach (var module in Rack.Modules)
            {
                Add(ModuleElement.Create(module));
            }
        }

        public void UpdateLayout()
        {
            // Make sure the RackControls are always on top of the modules
            //Remove(RackControls.Instance);
            //Add(RackControls.Instance);
            //UI.RackControls.Instance.BringToFront();
        }
    }
}