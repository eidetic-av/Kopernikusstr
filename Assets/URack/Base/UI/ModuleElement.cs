using System;
using Eidetic.Unity.UI.Utility;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Eidetic.Utility;

namespace Eidetic.URack.UI
{
    public partial class ModuleElement : DraggableElement
    {
        public bool MovingModule { get; private set; }

        public Vector2 StartDragMousePosition { get; private set; }
        public Vector2 CurrentDragMousePosition { get; private set; }

        public Vector2 StartDragElementPosition { get; private set; }
        public int StartDragModuleIndex { get; private set; }
        public int ModuleDropIndex { get; private set; } = -1;

        public Module Module { get; set; }

        ModuleHeader Header;
        class ModuleHeader : TouchElement
        {
            public bool DragActive;
            ModuleElement ModuleElement;
            public ModuleHeader(ModuleElement parentModule) : base()
            {
                ModuleElement = parentModule;
                AddToClassList("header");
                Add(new TextElement().WithText(parentModule.Module.Name.Prettify()));
                OnTouch += e => DragActive = true;
            }
        }

        static BlankPanel blank;
        static BlankPanel Blank
        {
            get
            {
                if (blank == null)
                    blank = new BlankPanel();
                return blank;
            }
        }
        class BlankPanel : ModuleElement
        {
            public BlankPanel()
            {
                name = "BlankPanel";
            }
        }

        ModuleElement() : base() { }

        public static ModuleElement Create(Module module)
        {
            if (module != null)
            {
                var element = new ModuleElement();
                module.BindElement(element);

                element.Header = new ModuleHeader(element);
                element.Add(element.Header);

                var moduleTemplate = Resources.Load<VisualTreeAsset>(module.GetType().Name + "Layout");
                moduleTemplate.CloneTree(element);

                LoadStyleSheets(element, module.GetType());

                URack.Instance.AddDragAction(element, element.DragModule);
                URack.Instance.OnRelease += element.DropModule;

                return element;
            }
            else return null;
        }

        void DragModule(MouseMoveEvent mouseMoveEvent)
        {
            if (!Header.DragActive || !this.Dragging) return;

            if (MovingModule == false)
            {
                StartDragMousePosition = mouseMoveEvent.localMousePosition;
                CurrentDragMousePosition = StartDragMousePosition;
                StartDragElementPosition = new Vector2(layout.x, layout.y);
                StartDragModuleIndex = URack.Instance.IndexOf(this);

                this.style.position = Position.Absolute;
                BringToFront();

                Blank.style.width = this.layout.width;
                Blank.style.height = this.layout.height;

                URack.Instance.Insert(StartDragModuleIndex, Blank);

                MovingModule = true;

                AddToClassList("Drag");
            }

            CurrentDragMousePosition = mouseMoveEvent.localMousePosition;

            this.style.left = StartDragElementPosition.x + (CurrentDragMousePosition.x - StartDragMousePosition.x);
            this.style.top = StartDragElementPosition.y + (CurrentDragMousePosition.y - StartDragMousePosition.y);

            // see if we are overlapping the edges of other modules to
            // add the insert blank in between and update the drop index
            foreach (var module in URack.Instance.Children())
            {
                if (module == Blank) continue;
                var leftCatchZone = new Rect(module.layout.x - 50, 0, 100, 400);
                if (leftCatchZone.Contains(CurrentDragMousePosition))
                {
                    URack.Instance.Remove(Blank);
                    URack.Instance.Insert(URack.Instance.IndexOf(module), Blank);
                    ModuleDropIndex = URack.Instance.IndexOf(Blank);
                    break;
                }
                var rightCatchZone = new Rect(module.layout.xMax - 50, 0, 100, 400);
                if (rightCatchZone.Contains(CurrentDragMousePosition))
                {
                    URack.Instance.Remove(Blank);
                    URack.Instance.Insert(URack.Instance.IndexOf(module) + 1, Blank);
                    ModuleDropIndex = URack.Instance.IndexOf(Blank);
                    break;
                }
            }
        }

        void DropModule(MouseUpEvent mouseUpEvent)
        {
            Header.DragActive = false;
            if (!MovingModule) return;

            URack.Instance.Remove(this);

            this.style.position = Position.Relative;
            this.style.left = 0;
            this.style.top = 0;

            var rowIndex = ModuleDropIndex != -1 ? ModuleDropIndex : StartDragModuleIndex;

            URack.Instance.Insert(rowIndex, this);

            URack.Instance.Remove(Blank);

            MovingModule = false;
            StartDragMousePosition = Vector2.zero;
            CurrentDragMousePosition = Vector2.zero;
            StartDragModuleIndex = -1;
            ModuleDropIndex = -1;

            RemoveFromClassList("Drag");
        }
    }
}