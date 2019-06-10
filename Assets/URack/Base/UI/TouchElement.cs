using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Eidetic.Utility;

namespace Eidetic.URack.UI
{
    public class TouchElement : StyledElement
    {
        public bool TouchActive { get; private set; } = false;

        public EventCallback<MouseDownEvent> OnTouch = e => { };
        public EventCallback<MouseUpEvent> OnRelease = e => { };

        EventCallback<MouseDownEvent> TouchCallback;
        EventCallback<MouseUpEvent> ReleaseCallback;

        public TouchElement() : base()
        {
            TouchCallback = e => Touch(e);
            RegisterCallback(TouchCallback);

            ReleaseCallback = e => Release(e);
            // Release occurs on the whole rack
            OnAttach += e => URack.Instance.RegisterCallback(ReleaseCallback);
        }

        void Touch(MouseDownEvent mouseDownEvent)
        {
            if (mouseDownEvent.button == (int)MouseButton.LeftMouse)
            {
                TouchActive = true;
                AddToClassList("Touch");
                OnTouch(mouseDownEvent);
            }
        }

        void Release(MouseUpEvent mouseUpEvent)
        {
            if (TouchActive)
            {
                TouchActive = false;
                RemoveFromClassList("Touch");
                OnRelease(mouseUpEvent);
            }
        }
    }
}