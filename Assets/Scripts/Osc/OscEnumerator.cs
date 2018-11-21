using OscJack;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class OscEnumerator : MonoBehaviour
{
    // We seperate the actions into lists based on their parameter types because
    // invoking generic delegates at runtime is much slower.
    public static Dictionary<string, Action<int>> IntSetters = new Dictionary<string, Action<int>>();
    public static Dictionary<string, Action<Single>> SingleSetters = new Dictionary<string, Action<Single>>();
    public static Dictionary<string, Action<bool>> BooleanSetters = new Dictionary<string, Action<bool>>();
    public static Dictionary<string, Action<Vector3>> Vector3Setters = new Dictionary<string, Action<Vector3>>();
    public static Dictionary<string, Action<Vector2>> Vector2Setters = new Dictionary<string, Action<Vector2>>();
    public static Dictionary<string, Action<string>> StringSetters = new Dictionary<string, Action<string>>();


    // And then use another dictionary to know what dictionary we should use to grab the setter
    public static Dictionary<string, MemberType> ParameterTypes = new Dictionary<string, MemberType>();
    public enum MemberType
    {
        Int32, Single, Boolean, Vector3, Vector2, String
    }

    // This stores the addresses bound by the particular MonoBehaviour instance
    public List<string> InstanceAddresses = new List<string>();

    void OnEnable()
    {
        // Add our routing method to the OscServer callbacks
        OscReceiver.Server.MessageDispatcher.AddRootNodeCallback(gameObject.name.ToPascal(), RouteMessages);
        // And bind all components to addresses
        BindAddresses();
    }

    void OnDisable()
    {
        // Remove our routing method to the OscServer callbacks
        OscReceiver.Server.MessageDispatcher.RemoveRootNodeCallback(gameObject.name.ToPascal(), RouteMessages);
        // And remove the address bindings
        RemoveAllAddresses();
    }

    void RefreshAddressList()
    {
        RemoveAllAddresses();
        BindAddresses();
    }

    void BindAddresses()
    {
        // For every property/field on every component of this gameObject
        Component[] components = GetComponents(typeof(Component));
        foreach (Component component in components)
        {
            if (component.GetType() == typeof(OscEnumerator))
                continue;

            // First iterate properties
            foreach (var property in component.GetType().GetProperties())
            {
                // Create the OSC address to bind to
                var address = CreateAddress(component, property);

                var setMethod = property.GetSetMethod();
                if (setMethod == null) continue;

                var backingFieldType = setMethod.GetParameters()[0].ParameterType;

                // Add a delegate representing the set method to the appropriate dictionary
                bool addedToDictionary = false;
                try
                {
                    switch (backingFieldType.Name)
                    {
                        case "Int32":
                            ParameterTypes.Add(address, MemberType.Int32);
                            var intSetter = (Action<int>)Delegate.CreateDelegate(typeof(Action<int>), component, property.GetSetMethod());
                            IntSetters.Add(address, intSetter);
                            addedToDictionary = true;
                            break;
                        case "Single":
                            ParameterTypes.Add(address, MemberType.Single);
                            var singleSetter = (Action<Single>)Delegate.CreateDelegate(typeof(Action<Single>), component, property.GetSetMethod());
                            SingleSetters.Add(address, singleSetter);
                            addedToDictionary = true;
                            break;
                        case "Boolean":
                            ParameterTypes.Add(address, MemberType.Boolean);
                            var booleanSetter = (Action<bool>)Delegate.CreateDelegate(typeof(Action<bool>), component, property.GetSetMethod());
                            BooleanSetters.Add(address, booleanSetter);
                            addedToDictionary = true;
                            break;
                        case "Vector3":
                            ParameterTypes.Add(address, MemberType.Vector3);
                            var vector3Setter = (Action<Vector3>)Delegate.CreateDelegate(typeof(Action<Vector3>), component, property.GetSetMethod());
                            Vector3Setters.Add(address, vector3Setter);
                            addedToDictionary = true;
                            break;
                        case "Vector2":
                            ParameterTypes.Add(address, MemberType.Vector2);
                            var vector2Setter = (Action<Vector2>)Delegate.CreateDelegate(typeof(Action<Vector2>), component, property.GetSetMethod());
                            Vector2Setters.Add(address, vector2Setter);
                            addedToDictionary = true;
                            break;
                        case "String":
                            ParameterTypes.Add(address, MemberType.String);
                            var stringSetter = (Action<string>)Delegate.CreateDelegate(typeof(Action<string>), component, property.GetSetMethod());
                            StringSetters.Add(address, stringSetter);
                            addedToDictionary = true;
                            break;
                    }
                }
                catch (Exception exception)
                {
                    Debug.Log("Failed to bind " + "/" + address + "/");
                    Debug.LogException(exception);
                    addedToDictionary = false;
                }
                if (addedToDictionary)
                {
                    InstanceAddresses.Add(address);
                }
            }
            // Then we iterate through public fields
            foreach (var field in component.GetType().GetFields())
            {
                // Create the OSC address to bind to
                var address = CreateAddress(component, field);

                var fieldType = field.FieldType;

                // Add a delegate representing the set method to the appropriate dictionary
                bool addedToDictionary = false;
                try
                {
                    switch (fieldType.Name)
                    {
                        case "Int32":
                            ParameterTypes.Add(address, MemberType.Int32);
                            var intSetter = new Action<int>((int value) =>
                            {
                                field.SetValue(component, value);
                            });
                            IntSetters.Add(address, intSetter);
                            addedToDictionary = true;
                            break;
                        case "Single":
                            ParameterTypes.Add(address, MemberType.Single);
                            var singleSetter = new Action<Single>((Single value) =>
                            {
                                Debug.Log("setting single value: " + value);
                                field.SetValue(component, value);
                            });
                            SingleSetters.Add(address, singleSetter);
                            addedToDictionary = true;
                            break;
                        case "Boolean":
                            ParameterTypes.Add(address, MemberType.Boolean);
                            var booleanSetter = new Action<bool>((bool value) =>
                            {
                                field.SetValue(component, value);
                            });
                            BooleanSetters.Add(address, booleanSetter);
                            addedToDictionary = true;
                            break;
                        case "Vector3":
                            ParameterTypes.Add(address, MemberType.Vector3);
                            var vector3Setter = new Action<Vector3>((Vector3 value) =>
                            {
                                field.SetValue(component, value);
                            });
                            Vector3Setters.Add(address, vector3Setter);
                            addedToDictionary = true;
                            break;
                        case "Vector2":
                            ParameterTypes.Add(address, MemberType.Vector2);
                            var vector2Setter = new Action<Vector2>((Vector2 value) =>
                            {
                                field.SetValue(component, value);
                            });
                            Vector2Setters.Add(address, vector2Setter);
                            addedToDictionary = true;
                            break;
                        case "String":
                            ParameterTypes.Add(address, MemberType.String);
                            var stringSetter = new Action<string>((string value) =>
                            {
                                field.SetValue(component, value);
                            });
                            StringSetters.Add(address, stringSetter);
                            addedToDictionary = true;
                            break;
                    }
                }
                catch (Exception exception)
                {
                    Debug.Log("Failed to bind " + "/" + address + "/");
                    Debug.LogException(exception);
                    addedToDictionary = false;
                }
                if (addedToDictionary)
                {
                    InstanceAddresses.Add(address);
                }
            }
        }
    }

    void RemoveAllAddresses()
    {
        foreach (var address in InstanceAddresses)
        {
            InstanceAddresses.Remove(address);
            if (IntSetters.Remove(address))
                continue;
            else if (SingleSetters.Remove(address))
                continue;
            else if (BooleanSetters.Remove(address))
                continue;
            else if (Vector3Setters.Remove(address))
                continue;
            else if (Vector2Setters.Remove(address))
                continue;
            else if (StringSetters.Remove(address))
                continue;
        }
    }

    void RouteMessages(string address, OscDataHandle data)
    {
        UnityMainThreadDispatcher.Instance().Enqueue((string dataAddress) =>
        {
            if (InstanceAddresses.Contains(dataAddress))
            {
                switch (ParameterTypes[dataAddress])
                {
                    case MemberType.Int32:
                        IntSetters[dataAddress].Invoke(data.GetElementAsInt(0));
                        break;
                    case MemberType.Single:
                        SingleSetters[dataAddress].Invoke(data.GetElementAsFloat(0));
                        break;
                    case MemberType.Boolean:
                            // any non-zero value is true
                            var boolean = (data.GetElementAsInt(0) != 0);
                        BooleanSetters[dataAddress].Invoke(boolean);
                        break;
                    case MemberType.Vector2:
                        Vector2Setters[dataAddress].Invoke(new Vector2(data.GetElementAsFloat(0),
                                data.GetElementAsFloat(1)));
                        break;
                    case MemberType.Vector3:
                        Vector3Setters[dataAddress].Invoke(new Vector3(data.GetElementAsFloat(0),
                                data.GetElementAsFloat(1), data.GetElementAsFloat(2)));
                        break;
                    case MemberType.String:
                        int element = 0;
                        string message = data.GetElementAsString(element++);
                        while (data.GetElementAsString(element) != String.Empty)
                            message += " " + data.GetElementAsString(element++);
                        StringSetters[dataAddress].Invoke(message);
                        break;
                }
            }
        }, (string)address.Clone());
    }

    string CreateAddress(Component component, MemberInfo member)
    {
        var objectNode = component.name.ToPascal();
        var componentNode = component.GetType().Name.ToPascal();
        var memberNode = member.Name.ToPascal();

        string address = "/" + objectNode + "/" + componentNode + "/" + memberNode;

        int componentIndex = -1;
        while (InstanceAddresses.Contains(address))
        {
            address = "/" + objectNode + "/" + componentNode + (++componentIndex) + "/" + memberNode;
        }

        return address;
    }

}