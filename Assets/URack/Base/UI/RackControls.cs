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

        Button NewModuleButton;
        Button DeleteModuleButton;

        public RackControls()
        {
            NewModuleButton = new Button(ShowNewModuleWindow);
            NewModuleButton.name = "NewModuleButton";
            NewModuleButton.Add(new Label("New Module"));
            Add(NewModuleButton);
            
            DeleteModuleButton = new Button(DeleteModule);
            DeleteModuleButton.name = "DeleteModuleButton";
            DeleteModuleButton.Add(new Label("Delete Module"));
            Add(DeleteModuleButton);
        }

        void ShowNewModuleWindow()
        {
            Debug.Log("New Module");
        }

        void DeleteModule()
        {
            Debug.Log("Delete Module");
        }

    }
}
