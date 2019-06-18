using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public static Label DuplicateDropBox { get; private set; } = new Label("Duplicate Module").WithClass("dropbox");
        public static Label DeleteDropBox { get; private set; } = new Label("Delete Module").WithClass("dropbox");

        static Box dropBar = new Box() { name = "drop-bar" };

        static Box newModuleWindow = new Box() { name = "new-module-window" };
        static bool showingNewModuleWindow => Instance.Contains(newModuleWindow);

        public RackControls()
        {
            pickingMode = PickingMode.Ignore;

            dropBar.Add(DuplicateDropBox);
            dropBar.Add(DeleteDropBox);

            Add(dropBar);

            var moduleList = new Box() { name = "module-list" };
            newModuleWindow.Add(moduleList);

            // Add all module classes to the new module window list
            foreach (var module in Assembly.GetAssembly(typeof(Module)).GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Module)) && !t.IsAbstract))
            {
                var assemblyName = module.FullName.Split('.');
                var moduleGroup = assemblyName[assemblyName.Length - 2];
                var moduleName = assemblyName.Last();
                var addButton = new Button() { text = moduleName };
                addButton.RegisterCallback<MouseUpEvent>(e => AddModule(module.UnderlyingSystemType));
                moduleList.Add(addButton);
            }

            URack.OnSwipeUp += ShowNewModuleWindow;
        }

        public static void ShowNewModuleWindow()
        {
            if (!showingNewModuleWindow)
            {
                Instance.Add(newModuleWindow);
                URack.OnSwipeDown += HideNewModuleWindow;
            }
        }

        public static void HideNewModuleWindow()
        {
            Instance.Remove(newModuleWindow);
            URack.OnSwipeDown -= HideNewModuleWindow;
        }

        void AddModule(Type moduleType)
        {
            var module = URack.Instance.Rack.AddModule(moduleType);
            var moduleElement = new ModuleElement(module);

            URack.Instance.Add(moduleElement);

            if (showingNewModuleWindow) HideNewModuleWindow();
        }

    }
}
