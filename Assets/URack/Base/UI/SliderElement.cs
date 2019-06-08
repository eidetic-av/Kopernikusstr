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

namespace Eidetic.URack.UI
{
    public class SliderElement : StyledElement
    {
        public SliderElement() : base() { }

        public class Factory : UxmlFactory<SliderElement, Traits> { }
        public class Traits : BindableElement.UxmlTraits
        {
            UxmlBoolAttributeDescription showLabelAttribute = new UxmlBoolAttributeDescription { name = "showLabel" };

            UxmlBoolAttributeDescription showValueAttribute = new UxmlBoolAttributeDescription { name = "showValue" };

            UxmlStringAttributeDescription typeAttribute = new UxmlStringAttributeDescription { name = "type" };

            UxmlFloatAttributeDescription defaultValueAttribute = new UxmlFloatAttributeDescription { name = "defaultValue" };

            UxmlBoolAttributeDescription readonlyValueAttribute = new UxmlBoolAttributeDescription { name = "readonlyValue" };

            UxmlStringAttributeDescription dictionaryNameAttribute = new UxmlStringAttributeDescription { name = "dictionaryName" };

            PortElement PortElement;

            public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(element, bag, context);

                var container = element as SliderElement;
                container.AddToClassList("Slider");

                PortElement = (PortElement)new PortElement.Factory().Create(bag, context);

                container.Add(PortElement);

                float defaultValue = -99f;
                if (!defaultValueAttribute.TryGetValueFromBag(bag, context, ref defaultValue)) defaultValue = 0f;

                var sliderValue = defaultValue;

                var type = "float";
                typeAttribute.TryGetValueFromBag(bag, context, ref type);
                VisualElement sliderElement;
                if (type == "int" || type == "dictionary")
                {
                    sliderElement = new SliderInt.UxmlFactory().Create(bag, context);
                    ((SliderInt)sliderElement).direction = SliderDirection.Vertical;
                    ((SliderInt)sliderElement).value = Mathf.RoundToInt(sliderValue);
                }
                else
                {
                    sliderElement = new Slider.UxmlFactory().Create(bag, context);
                    ((Slider)sliderElement).direction = SliderDirection.Vertical;
                    ((Slider)sliderElement).value = sliderValue;
                }

                sliderElement.AddToClassList("Slider");

                container.Add(sliderElement);

                var showValue = false;
                showValueAttribute.TryGetValueFromBag(bag, context, ref showValue);
                if (showValue)
                {
                    if (type == "int")
                    {
                        var intValueBox = new IntegerField();
                        intValueBox.AddToClassList("Value");
                        intValueBox.value = Mathf.RoundToInt(sliderValue);
                        ((SliderInt)sliderElement)
                            .RegisterCallback<ChangeEvent<int>>(e => intValueBox.value = e.newValue);
                        container.Add(intValueBox);
                        var readonlyValue = false;
                        readonlyValueAttribute.TryGetValueFromBag(bag, context, ref readonlyValue);
                        if (readonlyValue)
                        {
                            intValueBox.isReadOnly = true;
                            intValueBox.AddToClassList("readonly");
                        }
                    }
                    else if (type == "float")
                    {
                        var floatValueBox = new FloatField();
                        floatValueBox.AddToClassList("Value");
                        floatValueBox.value = sliderValue;
                        ((Slider)sliderElement)
                            .RegisterCallback<ChangeEvent<float>>(e =>
                            floatValueBox.value = (float)System.Math.Round(e.newValue, 2));
                        container.Add(floatValueBox);
                        var readonlyValue = false;
                        readonlyValueAttribute.TryGetValueFromBag(bag, context, ref readonlyValue);
                        if (readonlyValue)
                        {
                            floatValueBox.isReadOnly = true;
                            floatValueBox.AddToClassList("readonly");
                        }
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

                            var sliderIntElement = (SliderInt)sliderElement;

                            if (dictionaryType == typeof(Dictionary<string, float>))
                            {
                                var floatDictionary = (Dictionary<string, float>)dictionaryInfo.GetValue(null);
                                sliderIntElement.RegisterCallback<ChangeEvent<int>>(e =>
                                        valueBox.value = floatDictionary.Keys.ElementAt(e.newValue));
                                valueBox.value = floatDictionary.Keys.ElementAt(Mathf.RoundToInt(sliderValue));
                            }
                            else if (dictionaryType == typeof(Dictionary<string, int>))
                            {
                                var intDictionary = (Dictionary<string, int>)dictionaryInfo.GetValue(null);
                                sliderIntElement.RegisterCallback<ChangeEvent<int>>(e =>
                                        valueBox.value = intDictionary.Keys.ElementAt(e.newValue));
                                valueBox.value = intDictionary.Keys.ElementAt(Mathf.RoundToInt(sliderValue));
                            }

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