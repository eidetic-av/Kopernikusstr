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
        public Action<MouseDownEvent> OnTouch;
        public Action<MouseUpEvent> OnRelease;

        public bool TouchActive { get; private set; } = false;
        public bool HoldActive { get; private set; } = false;

        public TouchElement() : base()
        {
            OnTouch = BaseTouchCallback;
            RegisterCallback<MouseDownEvent>(e => OnTouch.Invoke(e));

            // Release occurs on the whole rack only
            if (this is URack)
            {
                OnRelease = BaseReleaseCallback;
                RegisterCallback<MouseUpEvent>(e => OnRelease.Invoke(e));
            }
            else OnRelease = e => { };
        }

        void BaseTouchCallback(MouseDownEvent mouseDownEvent)
        {
            if (mouseDownEvent.button != (int)MouseButton.LeftMouse) return;
            TouchActive = true;
            AddToClassList("Touch");
            if (!(this is URack)) URack.Instance.OnRelease += OnRelease;
        }
        void BaseReleaseCallback(MouseUpEvent mouseUpEvent)
        {
            TouchActive = false;
            RemoveFromClassList("Touch");
            RemoveFromClassList("Hold");
            if (!(this is URack)) URack.Instance.OnRelease -= OnRelease;
        }
    }
}