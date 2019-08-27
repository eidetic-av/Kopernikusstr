using Eidetic.URack.Base;
using Eidetic.URack.Base.UI;
using OscJack;
using System.Collections.Generic;
using UnityEngine;

namespace Eidetic.URack.Networking
{
    public class EOCReceiver : Module
    {
        public ValueStore Values = new ValueStore();
        public ConnectionStore Connections = new ConnectionStore();

        internal static Dictionary<int, OscServer> Servers = new Dictionary<int, OscServer>();
        internal static Dictionary<OscServer, List<EOCReceiver>> Tracks = new Dictionary<OscServer, List<EOCReceiver>>();

        [SerializeField] public int Port = 9000;

        [Output] public float azim { get; set; } // Degrees 0-180
        [Output] public float dist { get; set; } // Metres 1-10
        [Output] public float centroid { get; set; }
        [Output] public float flatness { get; set; }
        [Output] public float flux { get; set; }
        [Output] public float harmonicity { get; set; }
        [Output] public float energy { get; set; }
        [Output] public float pitch { get; set; }

        OscServer Server;

        new public void OnEnable()
        {
            base.OnEnable();

            if (!Servers.ContainsKey(Port))
                Servers.Add(Port, Server = new OscServer(Port));
            else Server = Servers[Port];

            if (Tracks.ContainsKey(Server))
                Tracks[Server].Add(this);
            else Tracks[Server] = new List<EOCReceiver>().With(this);

            Server.MessageDispatcher.AddRootNodeCallback("track", OnMessageReceived);
        }

        public override void ElementAttach(ModuleElement element)
        {
            base.ElementAttach(element);
            element.Add(new IntSelector(this, "Track"));
        }

        new void OnDestroy()
        {
            Tracks[Server].Remove(this);
            if (Tracks[Server].Count == 0)
            {
                Tracks.Remove(Server);
                Servers.Remove(Port);
                Server.Dispose();
            }

            Server.MessageDispatcher.RemoveRootNodeCallback("track", OnMessageReceived);
        }

        void OnMessageReceived(string address, OscDataHandle data)
        {
            var subAddressStartIndex = address.IndexOf('/', 1);
            var subAddress = address.Substring(subAddressStartIndex, address.Length - subAddressStartIndex).Split('/');

            if (subAddress[1] == Values["Track"])
            {
                switch (subAddress[2])
                {
                    case "azim":
                        azim = data.GetElementAsFloat(0);
                        break;
                    case "dist":
                        dist = data.GetElementAsFloat(0);
                        break;
                }
            }
            else if (data.GetElementAsString(0) != null && data.GetElementAsString(0) == Values["Track"])
            {
                switch (subAddress[1])
                {
                    case "centroid":
                        centroid = data.GetElementAsFloat(1);
                        break;
                    case "flatness":
                        flatness = data.GetElementAsFloat(1);
                        break;
                    case "flux":
                        flux = data.GetElementAsFloat(1);
                        break;
                    case "harmonicity":
                        harmonicity = data.GetElementAsFloat(1);
                        break;
                    case "energy":
                        energy = data.GetElementAsFloat(1);
                        break;
                    case "pitch":
                        pitch = data.GetElementAsFloat(1);
                        break;
                }
            }
        }
    }
}