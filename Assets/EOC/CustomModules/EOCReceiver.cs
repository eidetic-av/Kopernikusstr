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

        [Output, Indicator] public float Angle;
        [Output, Indicator] public float OnsetAngle;
        [Output, Indicator] public float Energy;

        [Output] public bool Onset;

        public int Track => int.Parse(Values["Track"]);

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

        public void LateUpdate()
        {
            Onset = false;
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
                        Angle = data.GetElementAsFloat(0).Map(-180, 180, -1, 1);
                        break;
                    case "ad":
                        Angle = data.GetElementAsFloat(0);
                        break;
                }
            }
            else if (data.GetElementAsString(0) != null && data.GetElementAsString(0) == Values["Track"])
            {
                switch (subAddress[1])
                {
                    case "energy":
                        var incomingEnergy = ((data.GetElementAsFloat(1) * Scale) - GainReduction).Clamp(0, 1);
                        if (Energy == 0 && incomingEnergy > 0)
                        {
                            Onset = true;
                            OnsetAngle = Angle;
                        }
                        Energy = incomingEnergy;
                        break;
                }
            }
        }
    }
}