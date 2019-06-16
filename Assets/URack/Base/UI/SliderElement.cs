using System;
using System.Collections.Generic;
using Eidetic.Unity.UI.Utility;
using Eidetic.Utility;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.Reflection;
using System.Linq;

namespace Eidetic.URack.Base.UI
{
    public class SliderElement : StyledElement
    {
        public SliderElement() : base() { }

        public class Factory : UxmlFactory<SliderElement, Traits> { }
        public class Traits : BindableElement.UxmlTraits
        {

            UxmlStringAttributeDescription typeAttribute = new UxmlStringAttributeDescription { name = "type" };

            UxmlEnumAttributeDescription<SliderDirection> directionAttribute = new UxmlEnumAttributeDescription<SliderDirection> { name = "direction" };

            UxmlStringAttributeDescription dictionaryNameAttribute = new UxmlStringAttributeDescription { name = "dictionaryName" };

            UxmlBoolAttributeDescription readonlyValueAttribute = new UxmlBoolAttributeDescription { name = "readonlyValue" };

            UxmlBoolAttributeDescription showValueAttribute = new UxmlBoolAttributeDescription { name = "showValue" };

            UxmlStringAttributeDescription memberNameAttribute = new UxmlStringAttributeDescription { name = "member" };

            public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(element, bag, context);

                var container = element as SliderElement;
                container.AddToClassList("Slider");

                var portElement = (PortElement)new PortElement.Factory().Create(bag, context);

                container.Add(portElement);

                var direction = SliderDirection.Vertical;
                directionAttribute.TryGetValueFromBag(bag, context, ref direction);

                var type = "float";
                typeAttribute.TryGetValueFromBag(bag, context, ref type);

                VisualElement slider;
                if (type == "int" || type == "dictionary")
                {
                    slider = new SliderInt.UxmlFactory().Create(bag, context);
                    ((SliderInt)slider).direction = direction;
                }
                else
                {
                    slider = new Slider.UxmlFactory().Create(bag, context);
                    ((Slider)slider).direction = SliderDirection.Vertical;
                }

                slider.AddToClassList("Slider");

                container.Add(slider);


                Action<object> setter = null;
                Func<object> getter = null;

                var memberName = "";
                memberNameAttribute.TryGetValueFromBag(bag, context, ref memberName);
                if (memberName != "")
                {
                    var moduleElement = context.target as ModuleElement;
                    var module = moduleElement.Module;
                    if (module.Setters != null && module.Setters.ContainsKey(memberName))
                            setter = module.Setters[memberName];
                    if (module.Getters != null && module.Getters.ContainsKey(memberName))
                            getter = module.Getters[memberName];
                }


                var showValue = true;
                showValueAttribute.TryGetValueFromBag(bag, context, ref showValue);
                if (showValue)
                {
                    if (type == "int")
                    {
                        var intValueBox = new IntegerField();
                        intValueBox.AddToClassList("Value");

                        container.Add(intValueBox);

                        var readonlyValue = false;
                        readonlyValueAttribute.TryGetValueFromBag(bag, context, ref readonlyValue);
                        if (readonlyValue)
                        {
                            intValueBox.isReadOnly = true;
                            intValueBox.AddToClassList("readonly");
                        }

                        // make sure to invoke the setter on a change
                        if (setter != null)
                            ((Slider)slider).RegisterCallback<ChangeEvent<int>>(e =>
                            {
                                setter.Invoke(e.newValue);
                                // set the display value box too
                                intValueBox.value = e.newValue;
                            });

                        // and use the getter to set to stored values on attachment to the layout
                        if (getter != null)
                            container.OnAttach += e =>
                            {
                                var value = (int)getter.Invoke();
                                // position of the slider
                                ((SliderInt)slider).value = value;
                                // and the display value
                                intValueBox.value = value;
                            };


                    }
                    else if (type == "float")
                    {
                        var floatValueBox = new FloatField();
                        floatValueBox.AddToClassList("Value");

                        container.Add(floatValueBox);

                        var readonlyValue = false;
                        readonlyValueAttribute.TryGetValueFromBag(bag, context, ref readonlyValue);
                        if (readonlyValue)
                        {
                            floatValueBox.isReadOnly = true;
                            floatValueBox.AddToClassList("readonly");
                        }

                        // make sure to invoke the setter on a change
                        if (setter != null)
                            ((Slider)slider).RegisterCallback<ChangeEvent<float>>(e =>
                            {
                                setter.Invoke(e.newValue);
                                // set the display value box too
                                floatValueBox.value = (float)System.Math.Round(e.newValue, 2);
                            });

                        // and use the getter to set to stored values on attachment to the layout
                        if (getter != null)
                            container.OnAttach += e =>
                            {
                                var value = (float)getter.Invoke();
                                // position of the slider
                                ((Slider)slider).value = value;
                                // and the display value
                                floatValueBox.value = (float)System.Math.Round(value, 2);
                            };
                    }
                    else if (type == "dictionary")
                    {
                        string qualifiedDictionaryName = "";
                        dictionaryNameAttribute.TryGetValueFromBag(bag, context, ref qualifiedDictionaryName);
                        if (!qualifiedDictionaryName.IsNullOrEmpty())
                        {
                            var dictionaryParentTypeName = qualifiedDictionaryName.Substring(0, qualifiedDictionaryName.LastIndexOf('.'));
                            var dictionaryName = qualifiedDictionaryName.Substring(qualifiedDictionaryName.LastIndexOf('.') + 1);

                            var dictionaryParentType = Type.GetType(dictionaryParentTypeName);
                            var dictionaryInfo = dictionaryParentType.GetField(dictionaryName, BindingFlags.Public | BindingFlags.Static);
                            var dictionaryType = dictionaryInfo.GetValue(null).GetType();

                            var valueBox = new TextField();
                            valueBox.isReadOnly = true;

                            container.Add(valueBox);

                            var sliderIntElement = (SliderInt)slider;

                            sliderIntElement.lowValue = 0;

                            if (dictionaryType == typeof(Dictionary<string, float>))
                            {
                                var floatDictionary = (Dictionary<string, float>)dictionaryInfo.GetValue(null);
                                sliderIntElement.highValue = floatDictionary.Count - 1;

                                // make sure to invoke the setter on a change
                                if (setter != null)
                                    ((SliderInt)slider).RegisterCallback<ChangeEvent<int>>(e =>
                                    {
                                        setter.Invoke(e.newValue);
                                        // set the display value box too
                                        valueBox.value = floatDictionary.Keys.ElementAt(Mathf.RoundToInt(e.newValue));
                                    });

                                // and use the getter to set to stored values on attachment to the layout
                                if (getter != null)
                                    container.OnAttach += e =>
                                    {
                                        var value = (int)getter.Invoke();
                                        // position of the slider
                                        ((SliderInt)slider).value = value;
                                        // and the display value
                                        valueBox.value = floatDictionary.Keys.ElementAt(Mathf.RoundToInt(value));
                                    };
                            }
                            else if (dictionaryType == typeof(Dictionary<string, int>))
                            {
                                var intDictionary = (Dictionary<string, int>)dictionaryInfo.GetValue(null);
                                sliderIntElement.highValue = intDictionary.Count - 1;

                                // make sure to invoke the setter on a change
                                if (setter != null)
                                    ((SliderInt)slider).RegisterCallback<ChangeEvent<int>>(e =>
                                    {
                                        setter.Invoke(e.newValue);
                                        // set the display value box too
                                        valueBox.value = intDictionary.Keys.ElementAt(Mathf.RoundToInt(e.newValue));
                                    });

                                // and use the getter to set to stored values on attachment to the layout
                                if (getter != null)
                                    container.OnAttach += e =>
                                    {
                                        var value = (int)getter.Invoke();
                                        // position of the slider
                                        ((SliderInt)slider).value = value;
                                        // and the display value
                                        valueBox.value = intDictionary.Keys.ElementAt(Mathf.RoundToInt(value));
                                    };
                            }
                            else Debug.LogWarning("Invalid dictionary type for Slider " + container.name);

                        }
                    }
                }

            }

            // No children allowed in this element
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }
    }
}