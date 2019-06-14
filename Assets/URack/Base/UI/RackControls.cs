using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Eidetic.URack.Base.UI
{
    public class RackControls : StyledElement
    {
        static RackControls instance;
        public static RackControls Instance
        {
            get
            {
                if (instance == null) instance = new RackControls();
                return instance;
            }
        }

        Box Toolbar;
        Button NewModuleButton;

        Box DropBar;
        public DropBox DuplicateDropBox { get; private set; }
        public DropBox DeleteDropBox { get; private set; }

        public RackControls()
        {
            pickingMode = PickingMode.Ignore;

            Toolbar = new Box();
            Toolbar.name = "Toolbar";

            NewModuleButton = new Button(ShowNewModuleWindow);
            NewModuleButton.name = "NewModuleButton";
            NewModuleButton.Add(new Label("New Module"));

            Toolbar.Add(NewModuleButton);

            Add(Toolbar);

            DropBar = new Box();
            DropBar.name = "DropBar";

            DuplicateDropBox = new DropBox("Duplicate Module");
            DeleteDropBox = new DropBox("Delete Module");

            DropBar.Add(DuplicateDropBox);
            DropBar.Add(DeleteDropBox);

            Add(DropBar);
        }

        void ShowNewModuleWindow()
        {
            AddModule(typeof(Function.Oscillator4D));
            AddModule(typeof(Transform.RotationController));
        }

        void AddModule(Type moduleType)
        {
            var module = URack.Instance.Rack.AddModule(moduleType);
            var moduleElement = new ModuleElement(module);

            URack.Instance.Add(moduleElement);
        }


        public class DropBox : Box
        {
            public DropBox(string label) : base()
            {
                AddToClassList("dropbox");
                Add(new Label(label));
            }
        }

    }
}
