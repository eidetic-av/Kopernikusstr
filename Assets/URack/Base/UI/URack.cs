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

        public URack(Rack rack) : base()
        {
            Instance = this;
            Rack = rack;

            foreach (var module in Rack.Modules)
            {
                Add(new ModuleElement(module));
            }

            OnAttach += e => RefreshLayout();
        }

        public void RefreshLayout()
        {
            if (Contains(RackControls.Instance))
                Remove(RackControls.Instance);
            Add(RackControls.Instance);

            if (Contains(CableLayer.Instance))
                Remove(CableLayer.Instance);
            Add(CableLayer.Instance);
        }
    }
}