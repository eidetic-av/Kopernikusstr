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
        
        public static Action OnSwipeUp = () => { };
        public static Action OnSwipeDown = () => { };
        public static Action OnSwipeLeft = () => { };
        public static Action OnSwipeRight = () => { };

        float lastTouchTime = 0;
        Vector2 lastTouchPosition = new Vector2();

        public URack(Rack rack) : base()
        {
            Instance = this;
            Rack = rack;

            foreach (var module in Rack.Modules)
            {
                Add(new ModuleElement(module));
            }

            OnAttach += e => RefreshLayout();

            OnDrag += e =>
            {
                // Track touch gestures over entire URack

                // Don't capture gestures if we're patching something up or moving a module
                if (!PortElement.IsDragging && !ModuleElement.IsMoving)
                {
                    var touchPosition = e.localMousePosition;
                    if (lastTouchPosition == Vector2.zero) lastTouchPosition = touchPosition;

                    var time = (System.DateTime.Now.Minute / 60) + System.DateTime.Now.Second + (System.DateTime.Now.Millisecond / 1000f);
                    if (lastTouchTime == 0) lastTouchTime = time;

                    var deltaTime = time - lastTouchTime;

                    if (deltaTime > 0.5)
                    {
                        lastTouchTime = 0;
                        lastTouchPosition = Vector2.zero;
                        return;
                    }

                    var velocity = (touchPosition - lastTouchPosition) / deltaTime;

                    // Swipe up
                    if (velocity.y < -4000)
                    {
                        OnSwipeUp();
                        lastTouchTime = 0;
                        lastTouchPosition = Vector2.zero;
                    } else if (velocity.y > 1000)
                    {
                        OnSwipeDown();
                        lastTouchTime = 0;
                        lastTouchPosition = Vector2.zero;
                    }
                    else
                    {
                        lastTouchTime = time;
                        lastTouchPosition = e.mousePosition;
                    }

                }
            };
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