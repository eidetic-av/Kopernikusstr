using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Eidetic.URack.UI
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
        DropBox DeleteModuleDropBox;

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

            DeleteModuleDropBox = new DropBox("Delete Module");

            DropBar.Add(DeleteModuleDropBox);

            Add(DropBar);
        }

        void ShowNewModuleWindow()
        {
            Debug.Log("New Module");
        }

        void DeleteModule()
        {
            Debug.Log("Delete Module");
        }

        class DropBox : DraggableElement
        {
            public DropBox(string label) : base()
            {
                AddToClassList("dropbox");
                Add(new Label(label));
            }
        }

    }
}
