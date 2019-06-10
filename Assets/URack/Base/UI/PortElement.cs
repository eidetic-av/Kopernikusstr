using System;
using System.Collections.Generic;
using Eidetic.Unity.UI.Utility;
using Eidetic.Utility;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Eidetic.URack.UI
{
    public class PortElement : VisualElement
    {
        public PortElement() : base() { }

        public class Factory : UxmlFactory<PortElement, Traits> { }
        public class Traits : BindableElement.UxmlTraits
        {
            UxmlBoolAttributeDescription showLabelAttribute = new UxmlBoolAttributeDescription { name = "showLabel" };
            public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(element, bag, context);
                var port = element as PortElement;

                port.Add(new JackElement());

                var showLabel = true;
                showLabelAttribute.TryGetValueFromBag(bag, context, ref showLabel);
                if (showLabel && !port.name.IsNullOrEmpty())
                    port.Add(new TextElement().WithText(port.name).WithName("Label"));
            }

            // No children allowed in this element
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }
        class JackElement : DraggableElement
        {
            static JackElement()
            {
                URack.Instance.OnRelease += Release;
            }

            Port Port;
            public JackElement(Port port = null)
            {
                Port = port;
                URack.Instance.AddDragAction(this, Drag);
            }

            static JackElement hoveringJack;

            void Drag(MouseMoveEvent mouseMoveEvent)
            {
                if (mouseMoveEvent.target is JackElement && mouseMoveEvent.target != this)
                {
                    if (hoveringJack != null && hoveringJack != mouseMoveEvent.target)
                    {
                        hoveringJack.RemoveFromClassList("connectable");
                    }
                    hoveringJack = mouseMoveEvent.target as JackElement;
                    hoveringJack.AddToClassList("connectable");
                }
                else if (hoveringJack != null)
                {
                    hoveringJack.RemoveFromClassList("connectable");
                    hoveringJack = null;
                }
            }

            static void Release(MouseUpEvent mouseUpEvent)
            {
                if (hoveringJack != null)
                {
                    hoveringJack.RemoveFromClassList("connectable");
                    // connection logic here
                    hoveringJack = null;
                }
            }

        }
    }
}