﻿using System;
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
            //var module = URack.Instance.Rack.AddModule<Transform.RotationController>();
            var module = URack.Instance.Rack.AddModule<Function.Oscillator4D>();
            var moduleElement = ModuleElement.Create(module);

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
