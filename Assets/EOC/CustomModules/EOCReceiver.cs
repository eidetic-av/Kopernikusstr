using System.Collections.Generic;
using Eidetic.URack.Base;
using Eidetic.URack.Base.UI;
using OscJack;
using UnityEngine;

namespace Eidetic.URack.Networking
{
    public class EOCReceiver : Module
    {
        internal static Dictionary<int, OscServer> Servers = new Dictionary<int, OscServer>();
        internal static Dictionary<OscServer, List<EOCReceiver>> Tracks = new Dictionary<OscServer, List<EOCReceiver>>();

        [SerializeField] public int Port = 9000;

        [Input(0, 0.5f, 3, 0), Knob] public float GainReduction = 0f;
        [Input(0, 5, 2, 1), Knob] public float Scale = 1f;

        [Output, Indicator] public float azim { get; set; } // Degrees 0-180
        [Output, Indicator] public float dist { get; set; } // Metres 1-10
        // [Output, Indicator] public float centroid { get; set; }
        public float centroid { get; set; }
        // [Output, Indicator] public float flatness { get; set; }
        public float flatness { get; set; }
        // [Output, Indicator] public float flux { get; set; }
        public float flux { get; set; }

        [Output, Indicator] public float harmonicity { get; set; }
        // public float harmonicity { get; set; }

        [Output, Indicator] public float Energy { get; set; }
        // [Output, Indicator] public float pitch { get; set; }
        public float pitch { get; set; }

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

        IntSelector trackSelector;
        public void ElementAttach()
        {
            trackSelector?.RemoveFromHierarchy();
            Element.Add(trackSelector = new IntSelector(this, "Track"));
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
                        azim = data.GetElementAsFloat(0).Map(-180, 180, -1, 1);
                        break;
                    case "dist":
                        dist = data.GetElementAsFloat(0).Map(0, 10, -1, 1);
                        break;
                    case "ad":
                        azim = data.GetElementAsFloat(0);
                        dist = data.GetElementAsFloat(1);
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
                        Energy = ((data.GetElementAsFloat(1) * Scale) - GainReduction).Clamp(0, 1);
                        break;
                    case "pitch":
                        pitch = data.GetElementAsFloat(1);
                        break;
                }
            }
        }
    }
}