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
        public static Dictionary<PortElement, Port> Ports = new Dictionary<PortElement, Port>();

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
                var memberInfo = moduleType.GetMember(memberName)?.SingleOrDefault();

                // if we can't find the member info, don't create the port
                if (memberInfo == null) return;

                // add appropriate classes to the port element like the return type
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
                Ports[portElement] = portElement.Port;
            }

            // No children allowed in this element
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        void Drag(MouseMoveEvent mouseMoveEvent)
        {
            if (Port.IsOutput)
                draggingPortElement = this;
            else if (Port.IsInput && Port.IsConnected)
                draggingPortElement = PortElements[Port.Connection];

            if (draggingPortElement.Port.IsConnected)
                draggingPortElement.Port.Disconnect(0);

            if (mouseMoveEvent.target is PortElement && ((PortElement)mouseMoveEvent.target).Port.IsInput)
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
                draggingPortElement.worldBound.center + CableLayer.PortMouseOffset,
                mouseMoveEvent.mousePosition + CableLayer.PortMouseOffset);
        }

        void Release(MouseUpEvent mouseUpEvent)
        {
            if (draggingPortElement != null)
            {
                CableLayer.Instance.RemoveCable(draggingPortElement.Port.GetHashCode());

                if (hoveringPortElement != null)
                {
                    // remove the temporary hovering class
                    hoveringPortElement.RemoveFromClassList("connectable");

                    if (hoveringPortElement.Port.IsInput)
                    {
                        // disconnect the target port if it already is connected
                        // to a different output
                        if (hoveringPortElement.Port.IsConnected)
                            hoveringPortElement.Port.Disconnect(0);

                        // connect the ports
                        draggingPortElement.Port.Connect(hoveringPortElement.Port);
                    }
                }

                hoveringPortElement = null;
                draggingPortElement = null;
            }
        }

        public class Factory : UxmlFactory<PortElement, Traits> { }
    }
}