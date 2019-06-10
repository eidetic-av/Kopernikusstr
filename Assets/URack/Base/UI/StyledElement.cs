using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Eidetic.URack.UI
{
    public class StyledElement : VisualElement
    {
        EventCallback<AttachToPanelEvent> Attach;
        EventCallback<DetachFromPanelEvent> Detach;

        public EventCallback<AttachToPanelEvent> OnAttach;
        public EventCallback<DetachFromPanelEvent> OnDetach;

        public StyledElement() : base()
        {
            Attach = e => OnAttach.Invoke(e);
            RegisterCallback(Attach);

            Detach = e => OnDetach.Invoke(e);
            RegisterCallback(Detach);

            OnAttach = e => LoadStyleSheets(this, GetType());
            OnDetach = e => { };
        }

        // Load StyleSheet of name 'ThisElementType.uss' inside 'Resources'.
        // If the Element Type inherits from a base class, load it's base
        // class StyleSheet (and it's n parents' StyleSheets).
        static public void LoadStyleSheets(StyledElement element, Type elementType)
        {
            if (elementType.BaseType != null && elementType.BaseType != typeof(StyledElement) && elementType.BaseType != typeof(Module))
                LoadStyleSheets(element, elementType.BaseType);

            var styleSheet = Resources.Load<StyleSheet>(elementType.Name + "Style");
            if (styleSheet != null) element.styleSheets.Add(styleSheet);
        }

        static public void ClearStyleSheets(StyledElement element)
        {
            if (element.styleSheets != null) element.styleSheets.Clear();
            element.ClearClassList();
        }
    }
}