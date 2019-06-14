using System;
using System.Reflection;
using System.Collections.Generic;
using Eidetic.Unity.UI.Utility;
using Eidetic.Utility;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

namespace Eidetic.URack.Base.UI
{
    public class PortElement : DraggableElement
    {
        public static Dictionary<Port, PortElement> PortElements = new Dictionary<Port, PortElement>();

        static PortElement draggingPortElement;
        static PortElement hoveringPortElement;

        Port Port;

        public class Traits : BindableElement.UxmlTraits
        {
            UxmlBoolAttributeDescription showLabelAttribute = new UxmlBoolAttributeDescription { name = "showLabel" };

            UxmlStringAttributeDescription memberNameAttribute = new UxmlStringAttributeDescription { name = "member" };

            public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(element, bag, context);
                var portElement = element as PortElement;

                var moduleElement = context.target as ModuleElement;
                // the type name of the module is stored in it's class
                var moduleTypeName = moduleElement.GetClasses().First();
                var moduleType = ModuleElement.ModuleTypes[moduleTypeName];

                // now that we have the module type, assign this port to it's respective
                // property from the Module object
                var memberName = "";
                memberNameAttribute.TryGetValueFromBag(bag, context, ref memberName);

                // don't create the port if there is no member attached
                if (memberName == "") return;

                // try and get the member information from it's type
                var memberInfo = moduleType.GetMember(memberName).Single();

                // if we can't find the member info, don't create the port
                if (memberInfo == null) return;

                // add appropriate classes to the port element like the return type and the port direction
                portElement.AddToClassList(memberInfo.DeclaringType.ToString());

                // link this PortElement to the port property in the Module instance
                var module = moduleElement.Module;

                portElement.Port = module.GetPort(memberInfo.Name);

                portElement.OnDrag += portElement.Drag;
                portElement.OnRelease += portElement.Release;

                var showLabel = true;
                showLabelAttribute.TryGetValueFromBag(bag, context, ref showLabel);
                if (showLabel && !portElement.name.IsNullOrEmpty())
                    portElement.Add(new TextElement().WithText(portElement.name).WithName("Label"));


                PortElements[portElement.Port] = portElement;
            }

            // No children allowed in this element
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        // don't know why this offset is needed
        static Vector2 mouseOffset = new Vector2(-5, -26);

        void Drag(MouseMoveEvent mouseMoveEvent)
        {
            draggingPortElement = this;

            Debug.Log(draggingPortElement.Port.GetHashCode());

            if (draggingPortElement.Port.IsConnected)
            {
                draggingPortElement.Port.Disconnect(0);
                CableLayer.Instance.RemoveCable(draggingPortElement.Port.GetHashCode());
            }

            if (mouseMoveEvent.target is PortElement && mouseMoveEvent.target != this)
            {
                if (hoveringPortElement != null && hoveringPortElement != mouseMoveEvent.target)
                {
                    hoveringPortElement.RemoveFromClassList("connectable");
                }
                hoveringPortElement = mouseMoveEvent.target as PortElement;
                hoveringPortElement.AddToClassList("connectable");
            }
            else if (hoveringPortElement != null)
            {
                hoveringPortElement.RemoveFromClassList("connectable");
                hoveringPortElement = null;
            }

            CableLayer.Instance.DrawCable(draggingPortElement.Port.GetHashCode(),
                worldBound.center + mouseOffset,
                mouseMoveEvent.mousePosition + mouseOffset);

            CableLayer.Instance.MarkDirtyRepaint();
        }

        void Release(MouseUpEvent mouseUpEvent)
        {
            if (hoveringPortElement != null && hoveringPortElement.Port.IsInput)
            {
                // connect the ports
                draggingPortElement.Port.Connect(hoveringPortElement.Port);

                // draw the static cable
                CableLayer.Instance.DrawCable(draggingPortElement.Port.GetHashCode(),
                    draggingPortElement.worldBound.center + mouseOffset,
                    hoveringPortElement.worldBound.center + mouseOffset);

                // remove the temporary hovering class
                hoveringPortElement.RemoveFromClassList("connectable");
                hoveringPortElement = null;
            }
            else if (draggingPortElement != null)
            {
                CableLayer.Instance.RemoveCable(draggingPortElement.Port.GetHashCode());
            }
        }

        public class Factory : UxmlFactory<PortElement, Traits> { }
    }
}