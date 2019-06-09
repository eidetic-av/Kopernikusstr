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
        public EventCallback<AttachToPanelEvent> OnAttach;
        public EventCallback<DetachFromPanelEvent> OnDetach;

        public StyledElement() : base()
        {
            OnAttach = e => LoadStyleSheets(this, GetType());
            RegisterCallback(OnAttach);
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