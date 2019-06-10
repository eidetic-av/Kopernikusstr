using System;
using System.Collections.Generic;
using Eidetic.Utility;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Eidetic.URack.Base.UI
{
    public class DraggableElement : TouchElement
    {
        public bool Dragging { get; protected set; } = false;

        public EventCallback<MouseMoveEvent> OnDrag = e => { };

        EventCallback<MouseMoveEvent> DragCallback;

        public DraggableElement() : base()
        {
            OnRelease += ReleaseDrag;

            DragCallback = e => Drag(e);
            // Drag occurs on the whole rack
            OnAttach += e => URack.Instance.RegisterCallback(DragCallback);
        }
        void Drag(MouseMoveEvent mouseMoveEvent)
        {
            if (TouchActive)
            {
                Dragging = true;
                if (!ClassListContains("Dragging"))
                    AddToClassList("Dragging");
                OnDrag(mouseMoveEvent);
            }
        }

        void ReleaseDrag(MouseUpEvent mouseUpEvent)
        {
            Dragging = false;
            RemoveFromClassList("Dragging");
        }
    }
}