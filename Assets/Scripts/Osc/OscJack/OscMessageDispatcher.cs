// Based on:
// OSC Jack - Open Sound Control plugin for Unity
// https://github.com/keijiro/OscJack
//
// Extended by eidetic

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OscJack
{
    public sealed class OscMessageDispatcher
    {
        #region Callback delegate definition

        public delegate void MessageCallback(string address, OscDataHandle data);

        #endregion

        #region Public accessors

        public void AddCallback(string address, MessageCallback callback)
        {
            if (address == null || address == "" || address == " ") return;

            if (address[0] != '/') address = address.Insert(0, "/");
            
            lock (AddressCallbackMap)
            {
                if (AddressCallbackMap.ContainsKey(address))
                    AddressCallbackMap[address] += callback;
                else
                    AddressCallbackMap[address] = callback;
            }
        }

        public void RemoveCallback(string address, MessageCallback callback)
        {
            if (address == null || address == "" || address == " ") return;
            if (!AddressCallbackMap.ContainsKey(address)) return;

            if (address[0] != '/') address = address.Insert(0, "/");

            lock (AddressCallbackMap)
            {
                var temp = AddressCallbackMap[address] - callback;
                if (temp != null)
                    AddressCallbackMap[address] = temp;
                else
                    AddressCallbackMap.Remove(address);
            }
        }


        public void AddRootNodeCallback(string rootNode, MessageCallback callback)
        {
            lock (RootNodeCallbackMap)
            {
                if (RootNodeCallbackMap.ContainsKey(rootNode))
                    RootNodeCallbackMap[rootNode] += callback;
                else
                    RootNodeCallbackMap[rootNode] = callback;
            }
        }

        public void RemoveRootNodeCallback(string rootNode, MessageCallback callback)
        {
            lock (RootNodeCallbackMap)
            {
                var temp = RootNodeCallbackMap[rootNode] - callback;
                if (temp != null)
                    RootNodeCallbackMap[rootNode] = temp;
                else
                    RootNodeCallbackMap.Remove(rootNode);
            }
        }

        #endregion

        #region Handler invocation

        internal void Dispatch(string address, OscDataHandle data)
        {
            // First see if there are any callbacks assigned to the root node to handle
            lock (RootNodeCallbackMap)
            {
                var rootNode = address.Split('/')[1];
                MessageCallback callback;
                // Address-specified callback
                if (RootNodeCallbackMap.TryGetValue(rootNode, out callback))
                    callback(address, data);      
            }

            // Then specific addresses
            lock (AddressCallbackMap)
            {
                MessageCallback callback;
                // Address-specified callback
                if (AddressCallbackMap.TryGetValue(address, out callback))
                    callback(address, data);
                    
            }
        }

        #endregion

        #region Private fields

        Dictionary<string, MessageCallback> AddressCallbackMap = new Dictionary<string, MessageCallback>();
        Dictionary<string, MessageCallback> RootNodeCallbackMap = new Dictionary<string, MessageCallback>();

        #endregion
    }
}
