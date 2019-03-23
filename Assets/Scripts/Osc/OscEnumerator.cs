using OscJack;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Eidetic.Utility;

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
        OscReceiver.Server.MessageDispatcher.AddRootNodeCallback(gameObject.name.ToPascalCase(), RouteMessages);
        // And bind all components to addresses
        BindAddresses();
    }

    void OnDisable()
    {
        // Remove our routing method to the OscServer callbacks
        OscReceiver.Server.MessageDispatcher.RemoveRootNodeCallback(gameObject.name.ToPascalCase(), RouteMessages);
        // Remove the address bindings
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

            // Iterate through members
            foreach (var member in component.GetType().GetMembers())
            {
                // Create the OSC address to bind to
                var address = CreateAddress(component, member);

                // Work out whether it's a property or field
                var memberType = member.MemberType;
                // and fill in appropriate variables
                MethodInfo setMethod = null;
                Type fieldType;
                if (memberType == MemberTypes.Property)
                {
                    setMethod = ((PropertyInfo)member).GetSetMethod();
                    // continue if there is no accessible set method to the property
                    if (setMethod == null) continue;
                    // fieldType is the type of the backing field in the case of properties
                    fieldType = setMethod.GetParameters()[0].ParameterType;
                }
                else if (memberType == MemberTypes.Field)
                {
                    fieldType = ((FieldInfo)member).FieldType;
                }
                else continue;

                // Add a delegate representing the set method to the appropriate dictionary
                bool addedToDictionary = false;
                try
                {
                    switch (fieldType.Name)
                    {
                        case "Int32":
                            ParameterTypes.Add(address, MemberType.Int32);
                            Action<int> intSetter;
                            if (memberType == MemberTypes.Property)
                                intSetter = (Action<int>)Delegate.CreateDelegate(typeof(Action<int>), component, setMethod);
                            else
                                intSetter = new Action<int>((int value) => ((FieldInfo)member).SetValue(component, value));
                            IntSetters.Add(address, intSetter);
                            addedToDictionary = true;
                            break;
                        case "Single":
                            ParameterTypes.Add(address, MemberType.Single);
                            Action<Single> singleSetter;
                            if (memberType == MemberTypes.Property)
                                singleSetter = (Action<Single>)Delegate.CreateDelegate(typeof(Action<Single>), component, setMethod);
                            else
                                singleSetter = new Action<Single>((Single value) => ((FieldInfo)member).SetValue(component, value));
                            SingleSetters.Add(address, singleSetter);
                            addedToDictionary = true;
                            break;
                        case "Boolean":
                            ParameterTypes.Add(address, MemberType.Boolean);
                            Action<bool> booleanSetter;
                            if (memberType == MemberTypes.Property)
                                booleanSetter = (Action<bool>)Delegate.CreateDelegate(typeof(Action<bool>), component, setMethod);
                            else
                                booleanSetter = new Action<bool>((bool value) => ((FieldInfo)member).SetValue(component, value));
                            BooleanSetters.Add(address, booleanSetter);
                            addedToDictionary = true;
                            break;
                        case "Vector3":
                            ParameterTypes.Add(address, MemberType.Vector3);
                            Action<Vector3> vector3Setter;
                            if (memberType == MemberTypes.Property)
                                vector3Setter = (Action<Vector3>)Delegate.CreateDelegate(typeof(Action<Vector3>), component, setMethod);
                            else
                                vector3Setter = new Action<Vector3>((Vector3 value) => ((FieldInfo)member).SetValue(component, value));
                            Vector3Setters.Add(address, vector3Setter);
                            addedToDictionary = true;
                            break;
                        case "Vector2":
                            ParameterTypes.Add(address, MemberType.Vector2);
                            Action<Vector2> vector2Setter;
                            if (memberType == MemberTypes.Property)
                                vector2Setter = (Action<Vector2>)Delegate.CreateDelegate(typeof(Action<Vector2>), component, setMethod);
                            else
                                vector2Setter = new Action<Vector2>((Vector2 value) => ((FieldInfo)member).SetValue(component, value));
                            Vector2Setters.Add(address, vector2Setter);
                            addedToDictionary = true;
                            break;
                        case "String":
                            ParameterTypes.Add(address, MemberType.String);
                            Action<string> stringSetter;
                            if (memberType == MemberTypes.Property)
                                stringSetter = (Action<string>)Delegate.CreateDelegate(typeof(Action<string>), component, setMethod);
                            else
                                stringSetter = new Action<string>((string value) => ((FieldInfo)member).SetValue(component, value));
                            StringSetters.Add(address, stringSetter);
                            addedToDictionary = true;
                            break;
                    }
                }
                catch (Exception exception)
                {
                    Debug.Log("Failed to bind " + address);
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
        foreach (var address in InstanceAddresses.ToArray())
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
        switch (ParameterTypes[address])
        {
            case MemberType.Boolean:
                // any non-zero is true
                var boolData = (data.GetElementAsInt(0) != 0);
                Threads.RunOnMain(BooleanSetters[address], boolData);
                break;
            case MemberType.Int32:
                var intData = data.GetElementAsInt(0);
                Threads.RunOnMain(IntSetters[address], intData);
                break;
            case MemberType.Single:
                var singleData = data.GetElementAsFloat(0);
                Threads.RunOnMain(SingleSetters[address], singleData);
                break;
            case MemberType.Vector2:
                var vector2Data = new Vector2(data.GetElementAsFloat(0), data.GetElementAsFloat(1));
                Threads.RunOnMain(Vector2Setters[address], vector2Data);
                break;
            case MemberType.Vector3:
                var vector3Data = new Vector3(
                    data.GetElementAsFloat(0),
                    data.GetElementAsFloat(1),
                    data.GetElementAsFloat(2));
                Threads.RunOnMain(Vector3Setters[address], vector3Data);
                break;
            case MemberType.String:
                // each word comes as a different index (delimited by a space)
                int element = 0;
                string message = data.GetElementAsString(element++);
                while (data.GetElementAsString(element) != String.Empty)
                    message += " " + data.GetElementAsString(element++);
                Threads.RunOnMain(StringSetters[address], message);
                break;
        }
    }

    string CreateAddress(Component component, MemberInfo member)
    {
        var objectNode = component.name.ToPascalCase();
        var componentNode = component.GetType().Name.ToPascalCase();
        var memberNode = member.Name.ToPascalCase();

        string address = "/" + objectNode + "/" + componentNode + "/" + memberNode;

        int componentIndex = -1;
        while (InstanceAddresses.Contains(address))
        {
            address = "/" + objectNode + "/" + componentNode + (++componentIndex) + "/" + memberNode;
        }

        return address;
    }

}